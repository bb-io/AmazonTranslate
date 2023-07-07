namespace Apps.AmazonTranslate.Models.RequestModels;

public class CreateTerminologyRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public byte[] FileContent { get; set; }
    public string Format { get; set; }
}