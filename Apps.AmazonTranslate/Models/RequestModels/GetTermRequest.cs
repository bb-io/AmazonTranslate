using Apps.AmazonTranslate.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.AmazonTranslate.Models.RequestModels;

public class GetTermRequest : TerminologyRequest
{
    [StaticDataSource(typeof(FormatDataHandler))]
    public string Format { get; set; }
}