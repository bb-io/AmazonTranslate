using Apps.AmazonTranslate.Models.RequestModels.Base;

namespace Apps.AmazonTranslate.Models.RequestModels;

public class TranslateFileRequest : TranslateRequest
{
    public byte[] FileContent { get; set; }
}