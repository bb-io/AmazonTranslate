using Blackbird.Applications.Sdk.Common;

namespace Apps.AmazonTranslate.Models.RequestModels.Base;

// TODO: Add terminology parameter
public class TranslateRequest
{
    [Display("Source language code")] public string SourceLanguageCode { get; set; }
    [Display("Target language code")] public string TargetLanguageCode { get; set; }
    public IEnumerable<string>? Terminologies { get; set; }
    public string? Formality { get; set; }
    [Display("Mask profanity?")] public bool MaskProfanity { get; set; }
}