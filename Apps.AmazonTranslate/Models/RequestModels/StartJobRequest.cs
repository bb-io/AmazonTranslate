using Apps.AmazonTranslate.DataSourceHandlers;
using Apps.AmazonTranslate.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.AmazonTranslate.Models.RequestModels;

public class StartJobRequest
{
    [Display("Job name")]
    public string JobName { get; set; }

    [Display("S3 source uri")] public string S3SourceLocation { get; set; }
    [Display("S3 target uri")] public string S3TargetLocation { get; set; }
    
    [Display("Source language")] 
    [DataSource(typeof(LanguageDataHandler))]
    public string SourceLanguageCode { get; set; }
    
    [Display("Target language codes")] public IEnumerable<string> TargetLanguageCodes { get; set; }

    [Display("Content type")]
    [StaticDataSource(typeof(ContentTypeDataHandler))]
    public string ContentType { get; set; }

    [Display("Data access role arn")] public string DataAccessRoleArn { get; set; }
}