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
using Blackbird.Applications.SDK.Blueprints;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Filters.Constants;
using Blackbird.Filters.Enums;
using Blackbird.Filters.Extensions;
using Blackbird.Filters.Transformations;

namespace Apps.AmazonTranslate.Actions;

[ActionList]
public class TranslateActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : AmazonInvocable(invocationContext)
{
    #region Actions

    [BlueprintActionDefinition(BlueprintAction.TranslateText)]
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

    [BlueprintActionDefinition(BlueprintAction.TranslateFile)]
    [Action("Translate", Description = "Translate a file containing content retrieved from a CMS or file storage. The output can be used in compatible actions.")]
    public async Task<TranslatedFileResult> TranslateContent([ActionParameter] TranslateFileRequest input)
    {
        if (input.FileTranslationStrategy == "amazon")
        {
            return await TranslateDocument(input);
        }

        try
        {
            return await TranslateWithBlackbird(input);
        }
        catch (Exception e)
        {
            if (e.Message.Contains("This file format is not supported"))
            {
                throw new PluginMisconfigurationException("The file format is not supported by the Blackbird interoperable setting. Try setting the file translation strategy to Amazon native.");
            }
            throw;
        }
    }

    private async Task<TranslatedFileResult> TranslateWithBlackbird([ActionParameter] TranslateFileRequest translateData)
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
        var content = await Transformation.Parse(stream, translateData.File.Name);
        var segmentTranslations = await content.GetSegments().Where(x => !x.IsIgnorbale && x.IsInitial).Batch(5).Process(BatchTranslate);

        foreach (var (segment, translation) in segmentTranslations)
        {
            segment.SetTarget(translation.TranslatedText);
            segment.State = SegmentState.Translated;
        }

        if (translateData.OutputFileHandling == "original")
        {
            var target = content.Target();
            return new TranslatedFileResult { File = await fileManagementClient.UploadAsync(target.Serialize().ToStream(), target.OriginalMediaType, target.OriginalName) };
        }

        content.SourceLanguage ??= translateData.SourceLanguage;
        content.TargetLanguage ??= translateData.TargetLanguage;
        return new TranslatedFileResult { File = await fileManagementClient.UploadAsync(content.Serialize().ToStream(), MediaTypes.Xliff, content.XliffFileName) };
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

        var uploadedFile = await fileManagementClient.UploadAsync(translatedFile.TranslatedDocument.Content,
            contentType == MediaTypeNames.Text.Plain ? MediaTypeNames.Text.RichText : contentType, translateData.File.Name);

        return new()
        {
            File = uploadedFile
        };
    }

    #endregion
}