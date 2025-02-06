using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.Translate;
using Amazon.Translate.Model;
using Apps.AmazonTranslate.Constants;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.AmazonTranslate;
public class AmazonInvocable : BaseInvocable
{
    protected AmazonTranslateClient TranslateClient { get; }

    protected AmazonS3Client S3Client { get; }

    public AmazonInvocable(InvocationContext invocationContext) : base(invocationContext)
    {
        var key = invocationContext.AuthenticationCredentialsProviders.First(p => p.KeyName == "access_key");
        var secret = invocationContext.AuthenticationCredentialsProviders.First(p => p.KeyName == "access_secret");
        var region = invocationContext.AuthenticationCredentialsProviders.First(p => p.KeyName == "region");

        if (string.IsNullOrEmpty(key.Value) || string.IsNullOrEmpty(secret.Value) || string.IsNullOrEmpty(region.Value))
            throw new PluginMisconfigurationException(ExceptionMessages.CredentialsMissing);

        TranslateClient = new(key.Value, secret.Value, new AmazonTranslateConfig
        {
            RegionEndpoint = RegionEndpoint.GetBySystemName(region.Value)
        });

        S3Client = new(key.Value, secret.Value, new AmazonS3Config
        {
            RegionEndpoint = RegionEndpoint.GetBySystemName(region.Value)
        });
    }

    public static async Task<T> ExecuteAction<T>(Func<Task<T>> func)
    {
        try
        {
            return await func();
        }
        catch (Exception ex)
        {
            var message = ex switch
            {
                TooManyRequestsException => ExceptionMessages.TooManyRequests,
                TextSizeLimitExceededException => ExceptionMessages.TextSizeLimit,
                ServiceUnavailableException => ExceptionMessages.ServiceUnavailable,
                AmazonTranslateException aex => GetAmazonTranslateExceptionMessage(aex),
                _ => ExceptionMessages.TryAgain
            };

            throw new PluginApplicationException(message, ex);
        }
    }

    private static string GetAmazonTranslateExceptionMessage(AmazonTranslateException aex)
    {
        if (aex.ErrorCode is "NotAuthorizedException" && aex.Message.Contains("Your account has been block-listed"))
            return ExceptionMessages.ParallelDataRegion;

        return aex.ErrorCode switch
        {
            "InvalidSignatureException" => ExceptionMessages.WrongAccessKey,
            "UnrecognizedClientException" => ExceptionMessages.WrongSecret,
            _ => aex.Message
        };
    }

    public async Task<IEnumerable<Language>> GetAllLanguages()
    {
        return await ExecutePaginated(TranslateClient.Paginators.ListLanguages(new ListLanguagesRequest()).Responses, (x) => x.Languages);
    }

    public async Task<IEnumerable<T>> ExecutePaginated<T, TResult>(IPaginatedEnumerable<TResult> paginatorResponses, Func<TResult, IEnumerable<T>> selector)
    {
        return await ExecuteAction(async () => {
            var response = new List<T>();
            await foreach (var page in paginatorResponses)
            {
                response.AddRange(selector(page));
            }
            return response;
        });
    }
}
