using Blackbird.Applications.Sdk.Common;

namespace Apps.AmazonTranslate.Models.ResponseModels;

public record TranslatedStringResult
{
    [Display("Translation")]
    public string TranslatedText { get; init; }
}
