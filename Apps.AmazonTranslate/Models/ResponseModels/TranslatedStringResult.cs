using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.SDK.Blueprints.Interfaces.Translate;

namespace Apps.AmazonTranslate.Models.ResponseModels;

public record TranslatedStringResult : ITranslateTextOutput
{
    [Display("Translation")]
    public string TranslatedText { get; set; }
}
