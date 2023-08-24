using Amazon.Translate;
using Amazon.Translate.Model;
using Apps.AmazonTranslate.Factories;
using Apps.AmazonTranslate.Handlers;
using Apps.AmazonTranslate.Models.RequestModels;
using Apps.AmazonTranslate.Models.ResponseModels;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;

namespace Apps.AmazonTranslate.Actions;

[ActionList]
public class TerminologyActions
{
    [Action("Import terminology", Description = "Creates or updates a custom terminology")]
    public async Task<TerminologyResponse> ImportTerminology(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] CreateTerminologyRequest requestData)
    {
        var translator = TranslatorFactory
            .CreateTranslator(authenticationCredentialsProviders.ToArray());

        var request = new ImportTerminologyRequest
        {
            Name = requestData.Name,
            Description = requestData.Description,
            MergeStrategy = MergeStrategy.OVERWRITE,
            TerminologyData = new()
            {
                File = new(requestData.File.Bytes),
                Format = requestData.Format
            }
        };

        var response = await AwsRequestHandler.ExecuteAction(()
            => translator.ImportTerminologyAsync(request));

        return new(response.TerminologyProperties);
    }

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
}