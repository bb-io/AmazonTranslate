using Apps.AmazonTranslate.Connections;
using Blackbird.Applications.Sdk.Common.Authentication;
using DocumentFormat.OpenXml.Drawing;
using Newtonsoft.Json;
using Tests.AmazonTranslate.Base;

namespace Tests.AmazonTranslate;

[TestClass]
public class ValidatorTests : TestBase
{
    [TestMethod]
    public async Task ValidatesCorrectConnection()
    {
        var validator = new ConnectionValidator();

        var result = await validator.ValidateConnection(Creds, CancellationToken.None);
        Console.WriteLine(result.Message);
        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public async Task DoesNotValidateIncorrectConnection()
    {
        var validator = new ConnectionValidator();

        var newCreds = Creds.Select(x => new AuthenticationCredentialsProvider(x.KeyName, x.Value + "_incorrect"));
        var result = await validator.ValidateConnection(newCreds, CancellationToken.None);
        Console.WriteLine(result.Message);
        Assert.IsFalse(result.IsValid);
    }

    [TestMethod]
    public async Task ShowConnectionDefinition()
    {
        var definition = new ConnectionDefinition();

        Console.WriteLine(JsonConvert.SerializeObject(definition.ConnectionPropertyGroups, Formatting.Indented));
    }
}