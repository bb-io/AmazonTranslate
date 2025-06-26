using Apps.AmazonTranslate.Models.RequestModels.Base;
using Blackbird.Applications.SDK.Blueprints.Interfaces.Translate;

namespace Apps.AmazonTranslate.Models.RequestModels;

public class TranslateStringRequest : TranslateRequest, ITranslateTextInput
{
    public string Text { get; set; }
};