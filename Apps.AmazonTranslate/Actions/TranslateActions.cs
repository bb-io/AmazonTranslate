using System.Net.Mime;
using Amazon.Translate;
using Amazon.Translate.Model;
using Apps.AmazonTranslate.Factories;
using Apps.AmazonTranslate.Handlers;
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
            SourceLanguageCode = translateData.SourceLanguageCode ?? "auto",
            TargetLanguageCode = translateData.TargetLanguageCode,
            Settings = new()
            {
                Formality = TranslateSettingsParser.ParseFormality(translateData.Formality!),
                Profanity = translateData.MaskProfanity.HasValue
                    ? (translateData.MaskProfanity.Value ? Profanity.MASK : null)
                    : null,
                Brevity = translateData.TurnOnBrevity is true ? Brevity.ON : default
            },
            TerminologyNames = translateData.Terminologies?.ToList(),
            Text = translateData.Text
        };

        var translator = TranslatorFactory.CreateTranslator(authenticationCredentialsProviders.ToArray());
        var translateResult = await AwsRequestHandler.ExecuteAction<TranslateTextResponse>(()
            => translator.TranslateTextAsync(request));

        return new TranslatedStringResult
        {
            TranslatedText = translateResult.TranslatedText,
        };
    }

    [Action("Translate document", Description = "Translate a document")]
    public async Task<TranslatedFileResult> TranslateDocument(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] TranslateFileRequest translateData)
    {
        var allowedContentTypes = new Dictionary<string, string>
        {
            { ".html", MediaTypeNames.Text.Html },
            { ".txt", MediaTypeNames.Text.Plain },
            { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" }
        };
        var fileContentType = translateData.File.ContentType;
        var fileExtension = Path.GetExtension(translateData.File.Name);
        
        if (!allowedContentTypes.Values.Contains(fileContentType) && !allowedContentTypes.Keys.Contains(fileExtension))
            throw new Exception("The file must be in one of the following formats: HTML, TXT, or DOCX.");

        var contentType = allowedContentTypes.Values.Contains(fileContentType)
            ? fileContentType
            : allowedContentTypes[fileExtension];
        
        var request = new TranslateDocumentRequest
        {
            Document = new()
            {
                Content = new(translateData.File.Bytes),
                ContentType = contentType
            },
            Settings = new()
            {
                Formality = TranslateSettingsParser.ParseFormality(translateData.Formality),
                Profanity = translateData.MaskProfanity.HasValue
                    ? (translateData.MaskProfanity.Value ? Profanity.MASK : null)
                    : null,
                Brevity = translateData.TurnOnBrevity is true ? Brevity.ON : default
            },
            SourceLanguageCode = translateData.SourceLanguageCode ?? "auto",
            TargetLanguageCode = translateData.TargetLanguageCode,
        };

        var translator = TranslatorFactory.CreateTranslator(authenticationCredentialsProviders.ToArray());
        var translatedFile = await AwsRequestHandler.ExecuteAction<TranslateDocumentResponse>(()
            => translator.TranslateDocumentAsync(request));

        return new TranslatedFileResult
        {
            File = new(translatedFile.TranslatedDocument.Content.ToArray())
            {
                ContentType = contentType == MediaTypeNames.Text.Plain ? MediaTypeNames.Text.RichText : contentType,
                Name = Path.GetFileNameWithoutExtension(translateData.File.Name) 
                       + $"_{translatedFile.SourceLanguageCode}_{translatedFile.TargetLanguageCode}{fileExtension}"
            }
        };
    }

    #endregion
}