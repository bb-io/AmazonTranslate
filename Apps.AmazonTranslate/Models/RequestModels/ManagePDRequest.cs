namespace Apps.AmazonTranslate.Models.RequestModels;

public class ManagePDRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string S3Uri { get; set; }
    public string Format { get; set; }
}