using Amazon.Translate.Model;

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
        SourceLanguageCode = parallelData.SourceLanguageCode;
        TargetLanguageCodes = parallelData.TargetLanguageCodes;
    }

    public string Arn { get; }

    public string Name { get; }

    public string Description { get; }

    public string Message { get; }

    public string Status { get; }
    public string SourceLanguageCode { get; }
    public List<string> TargetLanguageCodes { get; }
}