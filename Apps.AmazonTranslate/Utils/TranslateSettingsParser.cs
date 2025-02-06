using Amazon.Translate;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.AmazonTranslate.Utils;

public static class TranslateSettingsParser
{
    public static Formality? ParseFormality(string formalityInput)
    {
        return formalityInput switch
        {
            "FORMAL" => Formality.FORMAL,
            "INFORMAL" => Formality.INFORMAL,
            null => null,
            "" => null,
            _ => throw new PluginMisconfigurationException("Formality field values can be FORMAL or INFORMAL")
        };
    } 
}