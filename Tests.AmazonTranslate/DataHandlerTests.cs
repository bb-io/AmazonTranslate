using Apps.AmazonTranslate.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.AmazonTranslate.Base;

namespace Tests.AmazonTranslate;

[TestClass]
public class DataHandlerTests : TestBase
{
    [TestMethod]
    public async Task Languages_returns_items()
    {
        var handler = new LanguageDataHandler(InvocationContext);
        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);
        Assert.IsNotNull(result);
        foreach(var item in result)
        {
            Console.WriteLine($"{item.Value}: {item.DisplayName}");
        }
    }

    [TestMethod]
    public async Task Parallel_data_returns_items()
    {
        var handler = new ParallelDataHandler(InvocationContext);
        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);
        Assert.IsNotNull(result);
        foreach (var item in result)
        {
            Console.WriteLine($"{item.Value}: {item.DisplayName}");
        }
    }

    [TestMethod]
    public async Task Terminologies_returns_items()
    {
        var handler = new TerminologyDataHandler(InvocationContext);
        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);
        Assert.IsNotNull(result);
        foreach (var item in result)
        {
            Console.WriteLine($"{item.Value}: {item.DisplayName}");
        }
    }
}
