using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;

namespace Apps.AmazonTranslate.DataSourceHandlers.EnumHandlers;

public class FormatDataHandler : EnumDataHandler
{
    protected override Dictionary<string, string> EnumValues => new()
    {
        {"CSV", "CSV"},
        {"TMX", "TMX"},
        {"TSV", "TSV"}
    };
}