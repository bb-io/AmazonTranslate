using Amazon.Translate.Model;
using Apps.AmazonTranslate.Factories;
using Apps.AmazonTranslate.Handlers;
using Apps.AmazonTranslate.Models.RequestModels;
using Apps.AmazonTranslate.Models.ResponseModels;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;

namespace Apps.AmazonTranslate.Actions;

[ActionList]
public class ParallelDataActions
{
    [Action("Create parallel data", Description = "Creates a parallel data resource in Amazon Translate")]
    public async Task<ParallelDataResponse> CreateParallelData(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] ManagePDRequest requestData)
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

        var translator = await 
            TranslatorFactory.CreateBucketTranslator(authenticationCredentialsProviders.ToArray(), requestData.S3Uri);

        var response = await AwsRequestHandler.ExecuteAction(()
            => translator.CreateParallelDataAsync(request));

        return new(response.Name, response.Status);
    }

    [Action("List parallel data", Description = "Lists your parallel data resources in Amazon Translate")]
    public async Task<AllParallelDataResponse> ListParallelData(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
    {
        string? next = null;
        var results = new List<FullParallelDataResponse>();
        var translator = TranslatorFactory
            .CreateTranslator(authenticationCredentialsProviders.ToArray());

        do
        {
            var request = new ListParallelDataRequest
            {
                NextToken = next,
                MaxResults = 100
            };

            var response = await AwsRequestHandler.ExecuteAction(()
                => translator.ListParallelDataAsync(request));

            next = response.NextToken;
            results.AddRange(response.ParallelDataPropertiesList
                .Select(x => new FullParallelDataResponse(x)));
        } while (!string.IsNullOrEmpty(next));


        return new(results);
    }

    [Action("Get parallel data", Description = "Provides information about a parallel data resource")]
    public async Task<FullParallelDataResponse> GetParallelData(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] [Display("Name")] string name)
    {
        var translator = TranslatorFactory
            .CreateTranslator(authenticationCredentialsProviders.ToArray());

        var request = new GetParallelDataRequest
        {
            Name = name
        };
        
        var response = await AwsRequestHandler.ExecuteAction(() => translator.GetParallelDataAsync(request));

        return new(response.ParallelDataProperties);
    }
    
    [Action("Update parallel data", Description = "Updates a previously created parallel data resource")]
    public async Task<ParallelDataResponse> UpdateParallelData(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] ManagePDRequest requestData)
    {
        var translator = TranslatorFactory
            .CreateTranslator(authenticationCredentialsProviders.ToArray());

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
        
        var response = await AwsRequestHandler.ExecuteAction(() => translator.UpdateParallelDataAsync(request));

        return new(response.Name, response.Status);
    }    
    
    [Action("Delete parallel data", Description = "Deletes a parallel data resource in Amazon Translate")]
    public Task DeleteParallelData(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] [Display("Name")] string name)
    {
        var translator = TranslatorFactory
            .CreateTranslator(authenticationCredentialsProviders.ToArray());

        var request = new DeleteParallelDataRequest
        {
            Name = name
        };
        
        return AwsRequestHandler.ExecuteAction(() => translator.DeleteParallelDataAsync(request));
    }
}