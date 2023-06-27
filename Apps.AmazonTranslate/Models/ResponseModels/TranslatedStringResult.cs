using Blackbird.Applications.Sdk.Common;

namespace Apps.AmazonTranslate.Models.ResponseModels;

public record TranslatedStringResult
{
    [Display("Translated text")]
    public string TranslatedText { get; init; }
    public string Formality { get; init; }
    public string Profanity { get; init; }
}
