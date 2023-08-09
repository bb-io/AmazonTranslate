using Apps.AmazonTranslate.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.AmazonTranslate.Models.RequestModels;

public class ParallelDataRequest
{
    [Display("Parallel data")]
    [DataSource(typeof(ParallelDataHandler))]
    public string PdName { get; set; }
}