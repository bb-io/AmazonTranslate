using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.AmazonTranslate.DataSourceHandlers.EnumHandlers;

public class FormatDataHandler : IStaticDataSourceHandler
{
    public Dictionary<string, string> GetData()
        => new()
        {
            { "CSV", "CSV" },
            { "TMX", "TMX" },
            { "TSV", "TSV" }
        };
}