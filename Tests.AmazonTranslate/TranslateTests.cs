using Apps.AmazonTranslate.Actions;
using Apps.AmazonTranslate.Connections;
using Apps.AmazonTranslate.Models.RequestModels;
using Blackbird.Applications.Sdk.Common.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        var result = await actions.Translate(new TranslateStringRequest { Text = ExampleText, TargetLanguageCode = "nl" });
        Console.WriteLine(result.TranslatedText);
        Assert.IsNotNull(result.TranslatedText);
    }

    [TestMethod]
    public async Task Translate_with_terminologies_works()
    {
        var actions = new TranslateActions(InvocationContext, FileManager);

        var result = await actions.Translate(new TranslateStringRequest { Text = ExampleText, TargetLanguageCode = "nl", Terminologies = new List<string> { } });
        Console.WriteLine(result.TranslatedText);
        Assert.IsNotNull(result.TranslatedText);
    }
}