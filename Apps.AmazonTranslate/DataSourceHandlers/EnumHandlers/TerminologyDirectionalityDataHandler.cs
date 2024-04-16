using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.AmazonTranslate.DataSourceHandlers.EnumHandlers;

public class TerminologyDirectionalityDataHandler : IStaticDataSourceHandler
{
    public Dictionary<string, string> GetData()
        => new()
        {
            { "UNI", "Uni-directional" },
            { "MULTI", "Multi-directional" }
        };
}