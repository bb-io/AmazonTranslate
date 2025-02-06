using System.Net.Mime;
using Amazon.Translate;
using Amazon.Translate.Model;
using Apps.AmazonTranslate.Models.RequestModels;
using Apps.AmazonTranslate.Models.ResponseModels;
using Apps.AmazonTranslate.Utils;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.AmazonTranslate.Actions;

[ActionList]
public class TranslateActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : AmazonInvocable(invocationContext)
{
    #region Actions

    [Action("Translate", Description = "Translate a string")]
    public async Task<TranslatedStringResult> Translate([ActionParameter] TranslateStringRequest translateData)
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
        var translateResult = await ExecuteAction<TranslateTextResponse>(()=> TranslateClient.TranslateTextAsync(request));

        return new TranslatedStringResult
        {
            TranslatedText = translateResult.TranslatedText,
        };
    }

    [Action("Translate document", Description = "Translate a document")]
    public async Task<TranslatedFileResult> TranslateDocument([ActionParameter] TranslateFileRequest translateData)
    {
        var allowedContentTypes = new Dictionary<string, string>
        {
            { ".html", MediaTypeNames.Text.Html },
            { ".txt", MediaTypeNames.Text.Plain },
            { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" }
        };
        var fileContentType = translateData.File.ContentType!;
        var fileExtension = Path.GetExtension(translateData.File.Name)!;

        if (!allowedContentTypes.Values.Contains(fileContentType) && !allowedContentTypes.Keys.Contains(fileExtension))
            throw new PluginMisconfigurationException("The file must be in one of the following formats: HTML, TXT, or DOCX.");

        var contentType = allowedContentTypes.Values.Contains(fileContentType)
            ? fileContentType
            : allowedContentTypes[fileExtension];

        var file = await fileManagementClient.DownloadAsync(translateData.File);

        var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        var request = new TranslateDocumentRequest
        {
            Document = new()
            {
                Content = memoryStream,
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

        var translatedFile = await ExecuteAction<TranslateDocumentResponse>(()=> TranslateClient.TranslateDocumentAsync(request));

        var translatedFileName = translateData.OutputFilename != null
            ? Path.GetFileNameWithoutExtension(translateData.OutputFilename) + fileExtension
            : Path.GetFileNameWithoutExtension(translateData.File.Name)
              + $"_{translatedFile.TargetLanguageCode}{fileExtension}";

        var uploadedFile = await fileManagementClient.UploadAsync(translatedFile.TranslatedDocument.Content,
            contentType == MediaTypeNames.Text.Plain ? MediaTypeNames.Text.RichText : contentType, translatedFileName);

        return new()
        {
            File = uploadedFile
        };
    }

    #endregion
}