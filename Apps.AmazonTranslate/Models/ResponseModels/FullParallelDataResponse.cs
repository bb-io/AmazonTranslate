using Amazon.Translate.Model;
using Blackbird.Applications.Sdk.Common;

namespace Apps.AmazonTranslate.Models.ResponseModels;

public record FullParallelDataResponse
{
    public FullParallelDataResponse(ParallelDataProperties parallelData)
    {
        Arn = parallelData.Arn;
        Name = parallelData.Name;
        Description = parallelData.Description;
        Message = parallelData.Message;
        Status = parallelData.Status.Value;
        CreatedAt = parallelData.CreatedAt;
        SourceLanguageCode = parallelData.SourceLanguageCode;
        TargetLanguageCodes = parallelData.TargetLanguageCodes;
    }

    [Display("ARN")]
    public string Arn { get; }

    public string Name { get; }

    public string Description { get; }

    public string Message { get; }
    
    [Display("Created at")]
    public DateTime CreatedAt { get; set; }
    public string Status { get; }
    
    [Display("Source language code")]
    public string SourceLanguageCode { get; }
    
    [Display("Target language codes")]
    public IEnumerable<string> TargetLanguageCodes { get; }
}