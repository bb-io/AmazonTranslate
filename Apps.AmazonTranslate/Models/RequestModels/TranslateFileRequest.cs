namespace Apps.AmazonTranslate.Models.RequestModels;

public class TranslateFileRequest
{
    public string TargetLanguageCode { get; set; }
    public string SourceLanguageCode { get; set; }
    public byte[] FileContent { get; set; }
}