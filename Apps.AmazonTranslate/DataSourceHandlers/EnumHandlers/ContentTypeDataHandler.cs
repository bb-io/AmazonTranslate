using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.AmazonTranslate.DataSourceHandlers.EnumHandlers;

public class ContentTypeDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return new List<DataSourceItem>()
        {
            new DataSourceItem( "text/html", "HTML" ),
            new DataSourceItem( "text/plain", "Text" ),
            new DataSourceItem( "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "DOCX" ),
            new DataSourceItem( "application/vnd.openxmlformats-officedocument.presentationml.presentation", "PPTX" ),
            new DataSourceItem( "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "XLSX" ),
            new DataSourceItem( "application/x-xliff+xml", "XLIFF" ),
        };
    }
}