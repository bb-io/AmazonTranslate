using Apps.AmazonTranslate.Models.RequestModels.Base;
using Blackbird.Applications.Sdk.Common;
using File = Blackbird.Applications.Sdk.Common.Files.File;

namespace Apps.AmazonTranslate.Models.RequestModels;

public class TranslateFileRequest : TranslateRequest
{
    [Display("File", Description = "File in HTML, TXT, or DOCX format.")]
    public File File { get; set; }
}