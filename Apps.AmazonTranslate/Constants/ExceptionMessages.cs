namespace Apps.AmazonTranslate.Constants;

public static class ExceptionMessages
{
    public const string TooManyRequests = "You have made too many requests within a short period of time. Wait for a short time and then try your request again.";
    public const string TextSizeLimit = "The size of the text you submitted exceeds the size limit. Reduce the size of the text or use a smaller document and then retry your request";
    public const string ServiceUnavailable = "The Amazon Translate service is temporarily unavailable. Wait a bit and then retry your request.";
    public const string TryAgain = "Something went wrong, please try again";
    public const string WrongAccessKey = "Wrong access key provided";
    public const string WrongSecret = "Wrong access secret provided";
    public const string AccessDenied = "User with the provided credentials is not permitted to perform translations. It should have Translate permission policy";
    public const string CredentialsMissing = "AWS User credentials missing. You need to specify access key and access secret to use Amazon Translate";
}