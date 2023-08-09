using Apps.AmazonTranslate.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.AmazonTranslate.Models.RequestModels;

public class TerminologyRequest
{
    [DataSource(typeof(TerminologyDataHandler))]
    public string Terminology { get; set; }
}