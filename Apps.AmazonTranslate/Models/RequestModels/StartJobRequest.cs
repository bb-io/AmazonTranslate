using Blackbird.Applications.Sdk.Common;

namespace Apps.AmazonTranslate.Models.RequestModels;

public class StartJobRequest
{
    [Display("Job name")] public required string JobName { get; set; }
    [Display("S3 source uri")] public required string S3SourceLocation { get; set; }
    [Display("S3 target uri")] public required string S3TargetLocation { get; set; }
    [Display("Source language code")] public required string SourceLanguageCode { get; set; }
    [Display("Target language codes")] public required IEnumerable<string> TargetLanguageCodes { get; set; }
    [Display("Content type")] public required string ContentType { get; set; }
    [Display("Data access role arn")] public required string DataAccessRoleArn { get; set; }
}