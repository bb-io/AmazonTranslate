using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Blueprints.Interfaces.Translate;

namespace Apps.AmazonTranslate.Models.ResponseModels;

public record TranslatedFileResult : ITranslateFileOutput
{
    [Display("Translated file")]
    public FileReference File { get; set; }
}