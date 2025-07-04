using Apps.AmazonTranslate.Actions;
using Apps.AmazonTranslate.Models.RequestModels;
using Blackbird.Applications.Sdk.Common.Files;
using Tests.AmazonTranslate.Base;

namespace Tests.AmazonTranslate;

[TestClass]
public class TranslateTests : TestBase
{
    private const string ExampleText = "Hello world!";

    [TestMethod]
    public async Task Translate_works()
    {
        var actions = new TranslateActions(InvocationContext, FileManager);

        var result = await actions.Translate(new TranslateStringRequest { Text = ExampleText, TargetLanguage = "nl" });
        Console.WriteLine(result.TranslatedText);
        Assert.IsNotNull(result.TranslatedText);
    }

    [TestMethod]
    public async Task Translate_file_works()
    {
        var actions = new TranslateActions(InvocationContext, FileManager);

        var file = new FileReference { Name = "contentful.html" };
        var result = await actions.TranslateContent(new TranslateFileRequest { File = file, TargetLanguage = "nl" });
        Assert.AreEqual(result.File.Name, "contentful.html.xlf");
    }

    [TestMethod]
    public async Task Translate_file_natively_works()
    {
        var actions = new TranslateActions(InvocationContext, FileManager);

        var file = new FileReference { Name = "contentful.html" };
        var result = await actions.TranslateContent(new TranslateFileRequest { File = file, TargetLanguage = "nl", FileTranslationStrategy = "amazon" });
        Assert.AreEqual(result.File.Name, "contentful_nl.html");
    }
}