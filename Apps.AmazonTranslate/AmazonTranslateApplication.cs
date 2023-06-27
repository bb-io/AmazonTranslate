using Blackbird.Applications.Sdk.Common;

namespace Apps.AmazonTranslate;

public class AmazonTranslateApplication : IApplication
{
    public string Name
    {
        get => "Amazon Translate";
        set { }
    }

    public T GetInstance<T>()
    {
        throw new NotImplementedException();
    }
}