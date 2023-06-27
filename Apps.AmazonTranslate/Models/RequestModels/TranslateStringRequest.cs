using Apps.AmazonTranslate.Models.RequestModels.Base;

namespace Apps.AmazonTranslate.Models.RequestModels;

public class TranslateStringRequest : TranslateRequest
{
    public string Text { get; set; }
};