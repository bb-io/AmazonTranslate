using Apps.AmazonTranslate.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.AmazonTranslate.Models.RequestModels;

public class CreatePDRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string S3Uri { get; set; }
    
    [StaticDataSource(typeof(FormatDataHandler))]
    public string Format { get; set; }
}