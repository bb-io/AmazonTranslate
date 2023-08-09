using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;

namespace Apps.AmazonTranslate.DataSourceHandlers.EnumHandlers;

public class FormalityDataHandler : EnumDataHandler
{
    protected override Dictionary<string, string> EnumValues => new()
    {
        { "FORMAL", "Formal" },
        { "INFORMAL", "Informal" }
    };
}