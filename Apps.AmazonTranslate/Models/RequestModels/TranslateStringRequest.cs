namespace Apps.AmazonTranslate.Models.RequestModels;

public class TranslateStringRequest
{
    public string TargetLanguageCode { get; set; }
    public string SourceLanguageCode { get; set; }
    public string Text { get; set; }
};