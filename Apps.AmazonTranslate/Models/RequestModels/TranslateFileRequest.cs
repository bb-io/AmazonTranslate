using Apps.AmazonTranslate.Models.RequestModels.Base;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.AmazonTranslate.Models.RequestModels;

public class TranslateFileRequest : TranslateRequest
{
    [Display("File", Description = "File in HTML, TXT, or DOCX format.")]
    public FileReference File { get; set; }
    
    [Display("Output filename", Description = "Name of the resulting file without an extension.")]
    public string? OutputFilename { get; set; }
}