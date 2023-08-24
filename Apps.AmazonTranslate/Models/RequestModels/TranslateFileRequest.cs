using Apps.AmazonTranslate.Models.RequestModels.Base;
using File = Blackbird.Applications.Sdk.Common.Files.File;

namespace Apps.AmazonTranslate.Models.RequestModels;

public class TranslateFileRequest : TranslateRequest
{
    public File File { get; set; }
}