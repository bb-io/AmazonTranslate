using Amazon.Translate.Model;
using Apps.AmazonTranslate.Factories;
using Apps.AmazonTranslate.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.AmazonTranslate.DataSourceHandlers;

public class LanguageDataHandler : BaseInvocable, IAsyncDataSourceHandler
{
    private IEnumerable<AuthenticationCredentialsProvider> Creds =>
        InvocationContext.AuthenticationCredentialsProviders;

    public LanguageDataHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var languages = await GetAllLanguages();
        
        return languages
            .Where(x => context.SearchString == null ||
                        x.LanguageName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .ToDictionary(x => x.LanguageCode, x => x.LanguageName);
    }

    private async Task<List<Language>> GetAllLanguages()
    {
        string? next = null;
        var results = new List<Language>();
        var translator = TranslatorFactory.CreateTranslator(Creds.ToArray());

        do
        {
            var request = new ListLanguagesRequest()
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