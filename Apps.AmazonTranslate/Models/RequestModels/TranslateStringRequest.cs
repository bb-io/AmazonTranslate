namespace Apps.AmazonTranslate.Models.RequestModels;

public record TranslateStringRequest(string TargetLanguageCode, string SourceLanguageCode, string Text);