using Amazon.Translate.Model;
using Apps.AmazonTranslate.Factories;
using Apps.AmazonTranslate.Handlers;
using Blackbird.Applications.Sdk.Common.Authentication;

namespace Apps.AmazonTranslate.Extensions;

public static class LanguageExtensions
{
    public static async Task<List<Language>> GetAllLanguages(IEnumerable<AuthenticationCredentialsProvider> credentials)
    {
        string? next = null;
        var results = new List<Language>();
        var translator = TranslatorFactory.CreateTranslator(credentials.ToArray());

        do
        {
            var request = new ListLanguagesRequest
            {
                NextToken = next,
                MaxResults = 100
            };

            var response = await AwsRequestHandler.ExecuteAction(()
                => translator.ListLanguagesAsync(request));

            next = response.NextToken;
            results.AddRange(response.Languages);
        } while (!string.IsNullOrEmpty(next));

        return results;
    }
}