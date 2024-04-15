using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.AmazonTranslate.DataSourceHandlers.EnumHandlers;

public class FormalityDataHandler : IStaticDataSourceHandler
{
    public Dictionary<string, string> GetData()
        => new()
        {
            { "FORMAL", "Formal" },
            { "INFORMAL", "Informal" }
        };
}