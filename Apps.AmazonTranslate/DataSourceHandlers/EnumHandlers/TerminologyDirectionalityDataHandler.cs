using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;

namespace Apps.AmazonTranslate.DataSourceHandlers.EnumHandlers;

public class TerminologyDirectionalityDataHandler : EnumDataHandler
{
    protected override Dictionary<string, string> EnumValues => new()
    {
        { "UNI", "Uni-directional" },
        { "MULTI", "Multi-directional" }
    };
}