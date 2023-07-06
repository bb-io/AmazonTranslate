using Amazon.Translate.Model;
using Blackbird.Applications.Sdk.Common;

namespace Apps.AmazonTranslate.Models.ResponseModels;

public class JobResponse
{
    public JobResponse(TextTranslationJobProperties job)
    {
        Id = job.JobId;
        Name = job.JobName;
        Status = job.JobStatus;
        Message = job.Message;
        SourceLanguageCode = job.SourceLanguageCode;
        TargetLanguageCodes = job.TargetLanguageCodes;
        InputDocumentsCount = job.JobDetails.InputDocumentsCount;
        TranslatedDocumentsCount = job.JobDetails.TranslatedDocumentsCount;
    }

    public string Id { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
    public string Message { get; set; }
    [Display("Source language code")] public string SourceLanguageCode { get; set; }
    [Display("Target language code")] public List<string> TargetLanguageCodes { get; set; }
    [Display("Input documents count")] public int InputDocumentsCount { get; set; }
    [Display("Translated documents count")] public int TranslatedDocumentsCount { get; set; }
}