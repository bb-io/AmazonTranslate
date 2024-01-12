using Apps.AmazonTranslate.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.AmazonTranslate.Models.RequestModels;

public class CreateTerminologyFromTbxRequest
{
    public FileReference File { get; set; }
    
    public string? Name { get; set; }

    public string? Description { get; set; }
    
    [DataSource(typeof(TerminologyDirectionalityDataHandler))]
    public string? Directionality { get; set; }
}