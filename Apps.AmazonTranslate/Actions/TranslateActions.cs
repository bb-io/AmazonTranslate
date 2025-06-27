using System.IO;
using System.Net.Mime;
using Amazon.Runtime.Internal;
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
using Blackbird.Filters.Enums;
using Blackbird.Filters.Extensions;
using Blackbird.Filters.Transformations;

namespace Apps.AmazonTranslate.Actions;

[ActionList]
public class TranslateActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : AmazonInvocable(invocationContext)
{
    #region Actions

    [Action("Translate text", Description = "Translate a small text string")]
    public async Task<TranslatedStringResult> Translate([ActionParameter] TranslateStringRequest translateData)
    {
        var request = new TranslateTextRequest
        {
            SourceLanguageCode = translateData.SourceLanguage ?? "auto",
            TargetLanguageCode = translateData.TargetLanguage,
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

    [Action("Translate", Description = "Translate a file containing content")]
    public async Task<TranslatedFileResult> TranslateContent([ActionParameter] TranslateFileRequest translateData)
    {
        try
        {

            async Task<IEnumerable<TranslatedStringResult>> BatchTranslate(IEnumerable<Segment> batch)
            {
                return await Task.WhenAll(batch.Select(x => Translate(new TranslateStringRequest
                {
                    Text = x.GetSource(),
                    Formality = translateData.Formality,
                    MaskProfanity = translateData.MaskProfanity,
                    SourceLanguage = translateData.SourceLanguage,
                    TargetLanguage = translateData.TargetLanguage,
                    Terminologies = translateData.Terminologies,
                    TurnOnBrevity = translateData.TurnOnBrevity,
                })));
            }

            var stream = await fileManagementClient.DownloadAsync(translateData.File);
            var content = await Transformation.Parse(stream);
            var segmentTranslations = await content.GetSegments().Where(x => !x.IsIgnorbale && x.IsInitial).Batch(5).Process(BatchTranslate);

            foreach (var (segment, translation) in segmentTranslations)
            {
                segment.SetTarget(translation.TranslatedText);
                segment.State = SegmentState.Translated;
            }

            if (translateData.OutputFileHandling == null || translateData.OutputFileHandling == "xliff")
            {
                var xliffStream = content.Serialize().ToStream();
                var fileName = translateData.File.Name.EndsWith("xliff") || translateData.File.Name.EndsWith("xlf") ? translateData.File.Name : translateData.File.Name + ".xliff";
                var uploadedFile = await fileManagementClient.UploadAsync(xliffStream, "application/xliff+xml", fileName);
                return new TranslatedFileResult { File = uploadedFile };
            }
            else
            {
                var resultStream = content.Target().Serialize().ToStream();
                var uploadedFile = await fileManagementClient.UploadAsync(resultStream, translateData.File.ContentType, translateData.File.Name);
                return new TranslatedFileResult { File = uploadedFile };
            }

        }
        catch (Exception)
        {
            return await TranslateDocument(translateData);
        }
    }

    private async Task<TranslatedFileResult> TranslateDocument([ActionParameter] TranslateFileRequest translateData)
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
            SourceLanguageCode = translateData.SourceLanguage ?? "auto",
            TargetLanguageCode = translateData.TargetLanguage,
        };

        var translatedFile = await ExecuteAction<TranslateDocumentResponse>(() => TranslateClient.TranslateDocumentAsync(request));

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