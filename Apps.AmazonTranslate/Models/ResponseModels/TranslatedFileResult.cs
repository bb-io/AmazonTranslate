using Blackbird.Applications.Sdk.Common;
using File = Blackbird.Applications.Sdk.Common.Files.File;

namespace Apps.AmazonTranslate.Models.ResponseModels;

public record TranslatedFileResult
{
    [Display("Translated file")]
    public File File { get; init; }
}