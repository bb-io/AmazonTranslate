using Amazon.Translate.Model;
using Apps.AmazonTranslate.Models.RequestModels;
using Apps.AmazonTranslate.Models.ResponseModels;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.AmazonTranslate.Actions;

[ActionList]
public class JobActions(InvocationContext invocationContext) : AmazonInvocable(invocationContext)
{
    [Action("Start translation job", Description = "Start a translation job")]
    public async Task<SmallJobResponse> StartJob([ActionParameter] StartJobRequest requestData)
    {
        var request = new StartTextTranslationJobRequest
        {
            SourceLanguageCode = requestData.SourceLanguageCode,
            TargetLanguageCodes = requestData.TargetLanguageCodes.ToList(),
            JobName = requestData.JobName,
            DataAccessRoleArn = requestData.DataAccessRoleArn,
            InputDataConfig = new()
            {
                S3Uri = requestData.S3SourceLocation,
                ContentType = requestData.ContentType
            },
            OutputDataConfig = new()
            {
                S3Uri = requestData.S3TargetLocation
            }
        };

        var response = await ExecuteAction(()=> TranslateClient.StartTextTranslationJobAsync(request));

        return new()
        {
            JobId = response.JobId,
            JobStatus = response.JobStatus
        };
    }

    [Action("Get translation job", Description = "Get information on a specific translation job")]
    public async Task<JobResponse> DescribeJob([ActionParameter] JobRequest job)
    {
        var response = await ExecuteAction(() => TranslateClient.DescribeTextTranslationJobAsync(new()
        {
            JobId = job.JobId
        }));

        return new(response.TextTranslationJobProperties);
    }
    
    [Action("Stop translation job", Description = "Stop a specific translation job")]
    public async Task<SmallJobResponse> StopJob([ActionParameter] JobRequest job)
    {
        var response = await ExecuteAction(() => TranslateClient.StopTextTranslationJobAsync(new()
        {
            JobId = job.JobId
        }));

        return new()
        {
            JobId = response.JobId,
            JobStatus = response.JobStatus
        };
    }
}