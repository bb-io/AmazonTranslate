using Apps.AmazonTranslate.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.AmazonTranslate.Models.RequestModels;

public class CreateTerminologyFromTbxRequest
{
    public FileReference File { get; set; }
    
    public string? Name { get; set; }

    public string? Description { get; set; }
    
    [StaticDataSource(typeof(TerminologyDirectionalityDataHandler))]
    public string? Directionality { get; set; }
}