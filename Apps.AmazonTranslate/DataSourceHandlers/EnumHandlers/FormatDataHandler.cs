using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.AmazonTranslate.DataSourceHandlers.EnumHandlers;

public class FormatDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return new List<DataSourceItem>()
        {
            new DataSourceItem( "CSV", "CSV" ),
            new DataSourceItem( "TMX", "TMX" ),
            new DataSourceItem( "TSV", "TSV") ,
        };
    }
}