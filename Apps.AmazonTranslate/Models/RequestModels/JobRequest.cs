using Apps.AmazonTranslate.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.AmazonTranslate.Models.RequestModels;

public class JobRequest
{
    [Display("Job")]
    [DataSource(typeof(JobIdDataHandler))]
    public string JobId { get; set; }
}