using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;

namespace Apps.AmazonTranslate.DataSourceHandlers.EnumHandlers;

public class ContentTypeDataHandler : EnumDataHandler
{
    protected override Dictionary<string, string> EnumValues => new()
    {
        {"text/html", "HTML"},
        {"text/plain", "Text"},
        {"application/vnd.openxmlformats-officedocument.wordprocessingml.document", "DOCX"},
        {"application/vnd.openxmlformats-officedocument.presentationml.presentation", "PPTX"},
        {"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "XLSX"},
        {"application/x-xliff+xml", "XLIFF"},
    };
}