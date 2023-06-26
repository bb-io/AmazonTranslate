namespace Apps.AmazonTranslate.Models.RequestModels;

public record TranslateFileRequest(string TargetLanguageCode, string SourceLanguageCode, byte[] FileContent);