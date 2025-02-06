using Amazon.Translate.Model;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.AmazonTranslate.DataSourceHandlers;

public class JobIdDataHandler(InvocationContext invocationContext) : AmazonInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var jobs = await ExecuteAction(() => TranslateClient.ListTextTranslationJobsAsync(new ListTextTranslationJobsRequest 
        {
            Filter = context.SearchString != null ? new TextTranslationJobFilter { JobName = context.SearchString } : null,
        }));        
        
        return jobs.TextTranslationJobPropertiesList.Select(x => new DataSourceItem(x.JobId, x.JobName));
    }
}