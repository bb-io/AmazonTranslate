using Apps.AmazonTranslate.DataSourceHandlers.EnumHandlers;
using Apps.AmazonTranslate.Models.RequestModels.Base;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Blueprints.Handlers;
using Blackbird.Applications.SDK.Blueprints.Interfaces.Translate;

namespace Apps.AmazonTranslate.Models.RequestModels;

public class TranslateFileRequest : TranslateRequest, ITranslateFileInput
{
    [Display("File", Description = "File containing content.")]
    public FileReference File { get; set; }

    [Display("Output file handling", Description = "Determine the format of the output file. The default Blackbird behavior is to convert to XLIFF for future steps."), StaticDataSource(typeof(ProcessFileFormatHandler))]
    public string? OutputFileHandling { get; set; }

    [Display("File translation strategy", Description = "Select whether to use Amazon's own file processing capabilities or use Blackbird interoperability mode"), StaticDataSource(typeof(FileTranslationStrategyHandler))]
    public string? FileTranslationStrategy { get; set; }
}