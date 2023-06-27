using Amazon;
using Amazon.Translate;
using Amazon.Translate.Model;
using Apps.AmazonTranslate.Constants;
using Apps.AmazonTranslate.Models.RequestModels;
using Apps.AmazonTranslate.Models.ResponseModels;
using Apps.AmazonTranslate.Utils;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;

namespace Apps.AmazonTranslate.Actions;

[ActionList]
public class TranslateActions
{
    #region Actions

    [Action("Translate", Description = "Translate a string")]
    public async Task<TranslatedStringResult> Translate(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] TranslateStringRequest translateData)
    {
        var request = new TranslateTextRequest
        {
            SourceLanguageCode = translateData.SourceLanguageCode,
            TargetLanguageCode = translateData.TargetLanguageCode,
            Settings = new()
            {
                Formality = TranslateSettingsParser.ParseFormality(translateData.Formality),
                Profanity = translateData.MaskProfanity ? Profanity.MASK : null
            },
            Text = translateData.Text
        };

        var translator = CreateTranslator(authenticationCredentialsProviders.ToArray());
        var translateResult = await ExecuteTranslation<TranslateTextResponse>(() => translator.TranslateTextAsync(request));
        
        return new TranslatedStringResult
        {
            TranslatedText = translateResult.TranslatedText,
            Formality = translateResult.AppliedSettings.Formality,
            Profanity = translateResult.AppliedSettings.Profanity,
        };
    }

    [Action("Translate document", Description = "Translate a document")]
    public async Task<TranslatedFileResult> TranslateDocument(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] TranslateFileRequest translateData)
    {
        var request = new TranslateDocumentRequest
        {
            Document = new()
            {
                Content = new(translateData.FileContent),
                ContentType = "text/plain"
            },
            Settings = new()
            {
                Formality = TranslateSettingsParser.ParseFormality(translateData.Formality),
                Profanity = translateData.MaskProfanity ? Profanity.MASK : null
            },
            SourceLanguageCode = translateData.SourceLanguageCode,
            TargetLanguageCode = translateData.TargetLanguageCode
        };

        var translator = CreateTranslator(authenticationCredentialsProviders.ToArray());
        var translatedFile = await ExecuteTranslation<TranslateDocumentResponse>(() => translator.TranslateDocumentAsync(request));

        return new TranslatedFileResult
        {
            File = translatedFile.TranslatedDocument.Content.ToArray(),
            Formality = translatedFile.AppliedSettings.Formality,
            Profanity = translatedFile.AppliedSettings.Profanity
        };
    }

    #endregion

    #region Utils

    private AmazonTranslateClient CreateTranslator(
        AuthenticationCredentialsProvider[] authenticationCredentialsProviders)
    {
        var key = authenticationCredentialsProviders.First(p => p.KeyName == "access_key");
        var secret = authenticationCredentialsProviders.First(p => p.KeyName == "access_secret");

        if (string.IsNullOrEmpty(key.Value) || string.IsNullOrEmpty(secret.Value))
            throw new Exception(ExceptionMessages.CredentialsMissing);
            
        return new(key.Value, secret.Value, new AmazonTranslateConfig
        {
            RegionEndpoint = RegionEndpoint.USWest2
        });
    }

    private async Task<T> ExecuteTranslation<T>(Func<Task<T>> func)
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

    private string GetAmazonTranslateExceptionMessage(AmazonTranslateException aex)
    {
        return aex.ErrorCode switch
        {
            "InvalidSignatureException" => ExceptionMessages.WrongAccessKey,
            "UnrecognizedClientException" => ExceptionMessages.WrongSecret,
            "AccessDeniedException" => ExceptionMessages.AccessDenied,
            _ => aex.Message
        };
    }

    #endregion
}