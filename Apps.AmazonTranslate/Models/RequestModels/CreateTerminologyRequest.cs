using Apps.AmazonTranslate.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;
using File = Blackbird.Applications.Sdk.Common.Files.File;

namespace Apps.AmazonTranslate.Models.RequestModels;

public class CreateTerminologyRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public File File { get; set; }
    
    [DataSource(typeof(FormatDataHandler))]
    public string Format { get; set; }
}