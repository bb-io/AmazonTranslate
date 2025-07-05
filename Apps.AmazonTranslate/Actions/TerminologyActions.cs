using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using Amazon.Translate;
using Amazon.Translate.Model;
using Apps.AmazonTranslate.Models.RequestModels;
using Apps.AmazonTranslate.Models.ResponseModels;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Glossaries.Utils.Converters;
using Blackbird.Applications.Sdk.Glossaries.Utils.Dtos;
using Blackbird.Applications.Sdk.Glossaries.Utils.Parsers;
using RestSharp;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.AmazonTranslate.Actions;

[ActionList("Terminology")]
public class TerminologyActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : AmazonInvocable(invocationContext)
{
    #region Import

    [Action("Import terminology", Description = "Creates or updates a custom terminology")]
    public async Task<TerminologyResponse> ImportTerminology([ActionParameter] CreateTerminologyRequest requestData)
    {
        var file = await fileManagementClient.DownloadAsync(requestData.File);
        
        var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

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

        var response = await ExecuteAction(() => TranslateClient.ImportTerminologyAsync(request));
        return new(response.TerminologyProperties);
    }

    [Action("Import glossary", Description = "Creates or updates a custom terminology")]
    public async Task<TerminologyResponse> ImportTerminologyFromTbx([ActionParameter] CreateTerminologyFromTbxRequest requestData)
    {
        await using var fileStream = await fileManagementClient.DownloadAsync(requestData.File);
        
        var glossary = await fileStream.ConvertFromTBX();

        await using var memoryStream = new MemoryStream();
        await using var writer = new StreamWriter(memoryStream, Encoding.UTF8);

        var languageHeaders = new List<string>();
        var conceptEntryTerms = new Dictionary<string, Dictionary<string, string>>();
        
        var supportedLanguages = await GetAllLanguages();

        foreach (var entry in glossary.ConceptEntries)
        {
            var entryId = entry.Id;
            var termsForEntry = new Dictionary<string, string>();

            foreach (var languageSection in entry.LanguageSections)
            {
                var languageCode =
                    (supportedLanguages.Any(language => language.LanguageCode == languageSection.LanguageCode)
                        ? languageSection.LanguageCode
                        : languageSection.LanguageCode.Split('-')[0]).Trim();

                if (!termsForEntry.TryGetValue(languageCode, out _))
                    termsForEntry[languageCode] = languageSection.Terms.First().Term.Trim();
            }
            
            if (termsForEntry.Count < 2) // skip if no translations
                continue;
            
            conceptEntryTerms[entryId] = termsForEntry;

            foreach (var languageCode in termsForEntry.Keys)
            {
                if (!languageHeaders.Contains(languageCode))
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

        var response = await ExecuteAction(() => TranslateClient.ImportTerminologyAsync(request));
        return new(response.TerminologyProperties);
    }

    #endregion

    #region Export

    [Action("Export glossary", Description = "Export a custom terminology")]
    public async Task<ExportTerminologyAsTbxResponse> ExportTerminologyAsTbx([ActionParameter] TerminologyRequest input)
    {
        var request = new GetTerminologyRequest
        {
            Name = input.Terminology,
            TerminologyDataFormat = TerminologyDataFormat.CSV
        };

        var response = await ExecuteAction(() => TranslateClient.GetTerminologyAsync(request));

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

                if (i < terms.Count && !string.IsNullOrWhiteSpace(terms[i])) // Check if the list contains the current index and term for the language exists
                    languageSections.Add(new(languageCode.Trim(), new GlossaryTermSection[] { new(terms[i].Trim()) }));
            }
            
            glossaryConceptEntries.Add(new(Guid.NewGuid().ToString(), languageSections));
        }

        var glossary = new Glossary(glossaryConceptEntries)
        {
            Title = response.TerminologyProperties.Name, 
            SourceDescription = response.TerminologyProperties.Description
        };

        await using var tbxStream = glossary.ConvertToTBX();
        var tbxFileReference = await fileManagementClient.UploadAsync(tbxStream, MediaTypeNames.Text.Xml,
            $"{response.TerminologyProperties.Name}.tbx");
        return new(tbxFileReference);
    }

    #endregion

    #region Delete

    [Action("Delete terminology", Description = "Deletes a terminology resource")]
    public Task DeleteTerminology([ActionParameter] TerminologyRequest terminology)
    {
        var request = new DeleteTerminologyRequest
        {
            Name = terminology.Terminology
        };

        return ExecuteAction(() => TranslateClient.DeleteTerminologyAsync(request));
    }

    #endregion

    private string SanitizeTerminologyName(string name)
    {
        var trimmed = Regex.Replace(name, "^[^a-zA-Z0-9_-]+|[^a-zA-Z0-9_-]+$", "");
        var sanitized = Regex.Replace(trimmed, "[^a-zA-Z0-9_-]+", "_");
        return sanitized;
    }
}