using Amazon.Translate.Model;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.AmazonTranslate.DataSourceHandlers;

public class TerminologyDataHandler(InvocationContext invocationContext) : AmazonInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var data = await ExecutePaginated(TranslateClient.Paginators.ListTerminologies(new ListTerminologiesRequest()).Responses, (x) => x.TerminologyPropertiesList);

        return data
            .Where(x => context.SearchString == null ||
                        x.Name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(x => x.CreatedAt)
            .Take(20)
            .Select(x => new DataSourceItem(x.Name, x.Name));
    }
}