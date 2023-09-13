﻿using Amazon.Translate;
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
            SourceLanguageCode = translateData.SourceLanguageCode,
            TargetLanguageCode = translateData.TargetLanguageCode,
            Settings = new()
            {
                Formality = TranslateSettingsParser.ParseFormality(translateData.Formality!),
                Profanity = translateData.MaskProfanity ? Profanity.MASK : null
            },
            TerminologyNames = translateData.Terminologies?.ToList(),
            Text = translateData.Text,
        };

        var translator = TranslatorFactory.CreateTranslator(authenticationCredentialsProviders.ToArray());
        var translateResult = await AwsRequestHandler.ExecuteAction<TranslateTextResponse>(()
            => translator.TranslateTextAsync(request));

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
        var contentType = translateData.File.ContentType.Contains("html") ? "text/html" : "text/plain";
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
                Profanity = translateData.MaskProfanity ? Profanity.MASK : null
            },
            SourceLanguageCode = translateData.SourceLanguageCode,
            TargetLanguageCode = translateData.TargetLanguageCode,
        };

        var translator = TranslatorFactory.CreateTranslator(authenticationCredentialsProviders.ToArray());
        var translatedFile = await AwsRequestHandler.ExecuteAction<TranslateDocumentResponse>(()
            => translator.TranslateDocumentAsync(request));

        return new TranslatedFileResult
        {
            File = new(translatedFile.TranslatedDocument.Content.ToArray())
            {
                ContentType = contentType,
                Name = translateData.File.Name
            },
            Formality = translatedFile.AppliedSettings?.Formality,
            Profanity = translatedFile.AppliedSettings?.Profanity
        };
    }

    #endregion
}