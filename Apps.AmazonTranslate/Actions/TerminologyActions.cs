using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using Amazon.Translate;
using Amazon.Translate.Model;
using Apps.AmazonTranslate.Extensions;
using Apps.AmazonTranslate.Factories;
using Apps.AmazonTranslate.Handlers;
using Apps.AmazonTranslate.Models.RequestModels;
using Apps.AmazonTranslate.Models.ResponseModels;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Glossaries.Utils.Converters;
using Blackbird.Applications.Sdk.Glossaries.Utils.Dtos;
using Blackbird.Applications.Sdk.Glossaries.Utils.Parsers;
using RestSharp;

namespace Apps.AmazonTranslate.Actions;

[ActionList]
public class TerminologyActions
{
    private readonly IFileManagementClient _fileManagementClient;

    public TerminologyActions(IFileManagementClient fileManagementClient)
    {
        _fileManagementClient = fileManagementClient;
    }

    #region Import

    [Action("Import terminology", Description = "Creates or updates a custom terminology")]
    public async Task<TerminologyResponse> ImportTerminology(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] CreateTerminologyRequest requestData)
    {
        var file = await _fileManagementClient.DownloadAsync(requestData.File);
        
        var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        
        var translator = TranslatorFactory.CreateTranslator(authenticationCredentialsProviders.ToArray());

        var request = new ImportTerminologyRequest
        {
            Name = SanitizeTerminologyName(requestData.Name),
            Description = requestData.Description,
            MergeStrategy = MergeStrategy.OVERWRITE,
            TerminologyData = new()
            {
                File = memoryStream,
                Format = requestData.Format,
                Directionality = requestData.Directionality
            }
        };

        var response = await AwsRequestHandler.ExecuteAction(() => translator.ImportTerminologyAsync(request));
        return new(response.TerminologyProperties);
    }

    [Action("Import glossary", Description = "Creates or updates a custom terminology")]
    public async Task<TerminologyResponse> ImportTerminologyFromTbx(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] CreateTerminologyFromTbxRequest requestData)
    {
        await using var fileStream = await _fileManagementClient.DownloadAsync(requestData.File);
        
        var glossary = await fileStream.ConvertFromTBX();

        await using var memoryStream = new MemoryStream();
        await using var writer = new StreamWriter(memoryStream, Encoding.UTF8);

        var languageHeaders = new List<string>();
        var conceptEntryTerms = new Dictionary<string, Dictionary<string, string>>();
        
        var supportedLanguages = await LanguageExtensions.GetAllLanguages(authenticationCredentialsProviders);

        foreach (var entry in glossary.ConceptEntries)
        {
            var entryId = entry.Id;
            var termsForEntry = entry.LanguageSections.ToDictionary(section => section.LanguageCode,
                section => section.Terms.First().Term);
            conceptEntryTerms[entryId] = termsForEntry;

            foreach (var languageCode in termsForEntry.Keys)
            {
                if (!languageHeaders.Contains(languageCode) 
                    && supportedLanguages.Any(language => language.LanguageCode == languageCode))
                    languageHeaders.Add(languageCode);
            }
        }

        await writer.WriteLineAsync(string.Join(", ", languageHeaders));

        foreach (var entryId in conceptEntryTerms.Keys)
        {
            var termsForRow = new List<string>();

            foreach (var header in languageHeaders)
            {
                // If the term exists for this language code, add it, otherwise add an empty string
                termsForRow.Add(conceptEntryTerms[entryId].TryGetValue(header, out var term) ? term : string.Empty);
            }
            
            if (termsForRow.Any(term => term != string.Empty))
                await writer.WriteLineAsync(string.Join(", ", termsForRow));
        }

        await writer.FlushAsync();
        memoryStream.Position = 0;
        
        var translator = TranslatorFactory.CreateTranslator(authenticationCredentialsProviders.ToArray());

        var request = new ImportTerminologyRequest
        {
            Name = SanitizeTerminologyName((requestData.Name ?? glossary.Title) ??
                                           Path.GetFileNameWithoutExtension(requestData.File.Name)!),
            Description = requestData.Description ?? glossary.SourceDescription,
            MergeStrategy = MergeStrategy.OVERWRITE,
            TerminologyData = new()
            {
                File = memoryStream,
                Format = TerminologyDataFormat.CSV,
                Directionality = requestData.Directionality
            }
        };

        var response = await AwsRequestHandler.ExecuteAction(() => translator.ImportTerminologyAsync(request));
        return new(response.TerminologyProperties);
    }

    #endregion

    #region Export

    [Action("Export glossary", Description = "Export a custom terminology")]
    public async Task<ExportTerminologyAsTbxResponse> ExportTerminologyAsTbx(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] TerminologyRequest input)
    {
        var translator = TranslatorFactory.CreateTranslator(authenticationCredentialsProviders.ToArray());

        var request = new GetTerminologyRequest
        {
            Name = input.Terminology,
            TerminologyDataFormat = TerminologyDataFormat.CSV
        };

        var response = await AwsRequestHandler.ExecuteAction(() => translator.GetTerminologyAsync(request));

        var downloadTerminologyResponse =
            await new RestClient().ExecuteAsync(new(response.TerminologyDataLocation.Location));

        await using var terminologyMemoryStream = new MemoryStream(downloadTerminologyResponse.RawBytes);
        var parsedTerminology = await terminologyMemoryStream.ParseCsvFile();
        
        var maxLength = parsedTerminology.Values.Max(list => list.Count);

        var glossaryConceptEntries = new List<GlossaryConceptEntry>();

        for (var i = 0; i < maxLength; i++)
        {
            var languageSections = new List<GlossaryLanguageSection>();

            foreach (var terminology in parsedTerminology)
            {
                var languageCode = terminology.Key;
                var terms = terminology.Value;

                if (i < terms.Count)  // Check if the list contains the current index
                    languageSections.Add(new(languageCode, new GlossaryTermSection[] { new(terms[i].Trim()) }));
                else
                    languageSections.Add(new(languageCode, new GlossaryTermSection[] { new(string.Empty) }));
            }
            
            glossaryConceptEntries.Add(new(Guid.NewGuid().ToString(), languageSections));
        }

        var glossary = new Glossary(glossaryConceptEntries)
        {
            Title = response.TerminologyProperties.Name, 
            SourceDescription = response.TerminologyProperties.Description
        };

        var tbxStream = glossary.ConvertToTBX();
        var tbxFileReference = await _fileManagementClient.UploadAsync(tbxStream, MediaTypeNames.Text.Xml,
            $"{response.TerminologyProperties.Name}.tbx");
        return new(tbxFileReference);
    }

    #endregion

    #region Get

    [Action("List terminologies", Description = "Lists custom terminologies associated with your account")]
    public async Task<AllTerminologiesResponse> ListTerminologies(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
    {
        string? next = null;
        var results = new List<TerminologyResponse>();
        var translator = TranslatorFactory
            .CreateTranslator(authenticationCredentialsProviders.ToArray());

        do
        {
            var request = new ListTerminologiesRequest
            {
                NextToken = next,
                MaxResults = 100
            };

            var response = await AwsRequestHandler.ExecuteAction(()
                => translator.ListTerminologiesAsync(request));

            next = response.NextToken;
            results.AddRange(response.TerminologyPropertiesList
                .Select(x => new TerminologyResponse(x)));
        } while (!string.IsNullOrEmpty(next));

        return new(results);
    }

    [Action("Get terminology", Description = "Retrieves a custom terminology")]
    public async Task<TerminologyResponse> GetTerminology(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] GetTermRequest input)
    {
        var translator = TranslatorFactory
            .CreateTranslator(authenticationCredentialsProviders.ToArray());

        var request = new GetTerminologyRequest
        {
            Name = input.Terminology,
            TerminologyDataFormat = input.Format
        };

        var response = await AwsRequestHandler.ExecuteAction(()
            => translator.GetTerminologyAsync(request));

        return new(response.TerminologyProperties);
    }

    #endregion

    #region Delete

    [Action("Delete terminology", Description = "Retrieves a custom terminology")]
    public Task DeleteTerminology(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] TerminologyRequest terminology)
    {
        var translator = TranslatorFactory
            .CreateTranslator(authenticationCredentialsProviders.ToArray());

        var request = new DeleteTerminologyRequest
        {
            Name = terminology.Terminology
        };

        return AwsRequestHandler.ExecuteAction(()
            => translator.DeleteTerminologyAsync(request));
    }

    #endregion

    private string SanitizeTerminologyName(string name)
    {
        var trimmed = Regex.Replace(name, "^[^a-zA-Z0-9_-]+|[^a-zA-Z0-9_-]+$", "");
        var sanitized = Regex.Replace(trimmed, "[^a-zA-Z0-9_-]+", "_");
        return sanitized;
    }
}