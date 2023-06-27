using Amazon;
using Amazon.Translate;
using Amazon.Translate.Model;
using Apps.AmazonTranslate.Models.RequestModels;
using Apps.AmazonTranslate.Models.ResponseModels;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;

namespace Apps.AmazonTranslate.Actions;

[ActionList]
public class TranslateActions
{
    [Action("Translate", Description = "Translate a string")]
    public Task<TranslateTextResponse> Translate(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] TranslateStringRequest translateData)
    {
        var request = new TranslateTextRequest
        {
            SourceLanguageCode = translateData.SourceLanguageCode,
            TargetLanguageCode = translateData.TargetLanguageCode,
            Text = translateData.Text
        };

        var translator = CreateTranslator(authenticationCredentialsProviders.ToArray());
        return translator.TranslateTextAsync(request);
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
            SourceLanguageCode = translateData.SourceLanguageCode,
            TargetLanguageCode = translateData.TargetLanguageCode
        };
            
        var translator = CreateTranslator(authenticationCredentialsProviders.ToArray());
        var translatedFile = await translator.TranslateDocumentAsync(request);
        
        return new(translatedFile.TranslatedDocument.Content.ToArray());
    }

    private static AmazonTranslateClient CreateTranslator(
        AuthenticationCredentialsProvider[] authenticationCredentialsProviders)
    {
        var key = authenticationCredentialsProviders.First(p => p.KeyName == "access_key");
        var secret = authenticationCredentialsProviders.First(p => p.KeyName == "access_secret");
       
        return new(key.Value, secret.Value, new AmazonTranslateConfig
        {
            RegionEndpoint = RegionEndpoint.USWest2
        });
    }
}