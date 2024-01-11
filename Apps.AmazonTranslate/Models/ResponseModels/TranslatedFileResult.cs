using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.AmazonTranslate.Models.ResponseModels;

public record TranslatedFileResult
{
    [Display("Translated file")]
    public FileReference File { get; init; }
}