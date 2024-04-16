using Apps.AmazonTranslate.DataSourceHandlers;
using Apps.AmazonTranslate.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.AmazonTranslate.Models.RequestModels.Base;

public class TranslateRequest
{
    [Display("Source language")]
    [DataSource(typeof(LanguageDataHandler))]
    public string? SourceLanguageCode { get; set; }

    [Display("Target language")]
    [DataSource(typeof(LanguageDataHandler))]
    public string TargetLanguageCode { get; set; }

    public IEnumerable<string>? Terminologies { get; set; }

    [StaticDataSource(typeof(FormalityDataHandler))]
    public string? Formality { get; set; }

    [Display("Mask profanity?")] public bool? MaskProfanity { get; set; }
    
    [Display("Turn on brevity")] public bool? TurnOnBrevity { get; set; }
}