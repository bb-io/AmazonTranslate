using Apps.AmazonTranslate.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.AmazonTranslate.Models.RequestModels;

public class UpdatePdRequest : CreatePDRequest
{
    [Display("Parallel data")]
    [DataSource(typeof(ParallelDataHandler))]
    public new string Name { get; set; }
}