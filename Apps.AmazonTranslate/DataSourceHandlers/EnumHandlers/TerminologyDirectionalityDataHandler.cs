using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.AmazonTranslate.DataSourceHandlers.EnumHandlers;

public class TerminologyDirectionalityDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return new List<DataSourceItem>()
        {
            new DataSourceItem( "UNI", "Uni-directional" ),
            new DataSourceItem( "MULTI", "Multi-directional" ),
        };
    }
}