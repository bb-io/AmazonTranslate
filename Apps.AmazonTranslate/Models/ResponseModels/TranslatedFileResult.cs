using File = Blackbird.Applications.Sdk.Common.Files.File;

namespace Apps.AmazonTranslate.Models.ResponseModels;

public record TranslatedFileResult
{
    public File File { get; init; }
    public string Formality { get; init; }
    public string Profanity { get; init; }
}