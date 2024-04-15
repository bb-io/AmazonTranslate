using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.AmazonTranslate.DataSourceHandlers.EnumHandlers;

public class ContentTypeDataHandler : IStaticDataSourceHandler
{
    public Dictionary<string, string> GetData()
        => new()
        {
            { "text/html", "HTML" },
            { "text/plain", "Text" },
            { "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "DOCX" },
            { "application/vnd.openxmlformats-officedocument.presentationml.presentation", "PPTX" },
            { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "XLSX" },
            { "application/x-xliff+xml", "XLIFF" },
        };
}