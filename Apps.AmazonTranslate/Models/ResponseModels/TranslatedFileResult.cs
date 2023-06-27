namespace Apps.AmazonTranslate.Models.ResponseModels;

public record TranslatedFileResult
{
    public byte[] File { get; init; }
    public string Formality { get; init; }
    public string Profanity { get; init; }
}