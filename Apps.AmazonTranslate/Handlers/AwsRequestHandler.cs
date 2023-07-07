using Amazon.Translate;
using Amazon.Translate.Model;
using Apps.AmazonTranslate.Constants;

namespace Apps.AmazonTranslate.Handlers;

public static class AwsRequestHandler
{
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
                ServiceUnavailableException => ExceptionMessages. ServiceUnavailable,
                AmazonTranslateException aex => GetAmazonTranslateExceptionMessage(aex),
                _ => ExceptionMessages.TryAgain
            };
            
            throw new Exception(message, ex);
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
}