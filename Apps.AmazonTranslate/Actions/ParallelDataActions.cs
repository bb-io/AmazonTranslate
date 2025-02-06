using Amazon.Translate.Model;
using Apps.AmazonTranslate.Models.RequestModels;
using Apps.AmazonTranslate.Models.ResponseModels;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.AmazonTranslate.Actions;

[ActionList]
public class ParallelDataActions(InvocationContext invocationContext) : AmazonInvocable(invocationContext)
{
    [Action("Create parallel data", Description = "Creates a parallel data resource in Amazon Translate")]
    public async Task<ParallelDataResponse> CreateParallelData([ActionParameter] CreatePDRequest requestData)
    {
        var request = new CreateParallelDataRequest
        {
            Name = requestData.Name,
            Description = requestData.Description,
            ParallelDataConfig = new()
            {
                S3Uri = requestData.S3Uri,
                Format = requestData.Format
            }
        };

        var response = await ExecuteAction(() => TranslateClient.CreateParallelDataAsync(request));

        return new(response.Name, response.Status);
    }   

    [Action("Get parallel data", Description = "Provides information about a parallel data resource")]
    public async Task<FullParallelDataResponse> GetParallelData([ActionParameter] ParallelDataRequest pd)
    {
        var request = new GetParallelDataRequest
        {
            Name = pd.PdName
        };
        
        var response = await ExecuteAction(() => TranslateClient.GetParallelDataAsync(request));

        return new(response.ParallelDataProperties);
    }
    
    [Action("Update parallel data", Description = "Updates a previously created parallel data resource")]
    public async Task<ParallelDataResponse> UpdateParallelData([ActionParameter] UpdatePdRequest requestData)
    {
        var request = new UpdateParallelDataRequest
        {
            Name = requestData.Name,
            Description = requestData.Description,
            ParallelDataConfig = new()
            {
                S3Uri = requestData.S3Uri,
                Format = requestData.Format
            }
        };
        
        var response = await ExecuteAction(() => TranslateClient.UpdateParallelDataAsync(request));

        return new(response.Name, response.Status);
    }    
    
    [Action("Delete parallel data", Description = "Deletes a parallel data resource in Amazon Translate")]
    public Task DeleteParallelData([ActionParameter] ParallelDataRequest pd)
    {
        var request = new DeleteParallelDataRequest
        {
            Name = pd.PdName
        };
        
        return ExecuteAction(() => TranslateClient.DeleteParallelDataAsync(request));
    }
}