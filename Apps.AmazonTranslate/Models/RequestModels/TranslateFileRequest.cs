using Apps.AmazonTranslate.Models.RequestModels.Base;
using Blackbird.Applications.Sdk.Common;

namespace Apps.AmazonTranslate.Models.RequestModels;

public class TranslateFileRequest : TranslateRequest
{
    public byte[] FileContent { get; set; }
}