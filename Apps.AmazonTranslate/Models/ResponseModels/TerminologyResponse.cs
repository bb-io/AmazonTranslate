using Amazon.Translate.Model;

namespace Apps.AmazonTranslate.Models.ResponseModels;

public class TerminologyResponse
{
    public TerminologyResponse(TerminologyProperties terminology)
    {
        Arn = terminology.Arn;
        Name = terminology.Name;
        Description = terminology.Description;
        Message = terminology.Message;
        CreatedAt = terminology.CreatedAt;
        Format = terminology.Format.Value;
        SourceLanguageCode = terminology.SourceLanguageCode;
        TargetLanguageCodes = terminology.TargetLanguageCodes;
        TermCount = terminology.TermCount;
    }
    
    public string Arn { get; }
    public string Name { get; }
    public string Description { get; }
    public string Message { get; }
    public DateTime CreatedAt { get; }
    public string Format { get; }
    public string SourceLanguageCode { get; }
    public List<string> TargetLanguageCodes { get; }
    public int TermCount { get; }
}