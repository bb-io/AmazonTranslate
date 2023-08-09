using Apps.AmazonTranslate.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.AmazonTranslate.Models.RequestModels;

public class GetTermRequest : TerminologyRequest
{
    [DataSource(typeof(FormatDataHandler))]
    public string Format { get; set; }
}