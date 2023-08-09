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
public class JobActions
{
    [Action("Start job", Description = "Start a translation job")]
    public async Task<SmallJobResponse> StartJob(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] StartJobRequest requestData)
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

        var translator = await TranslatorFactory
            .CreateBucketTranslator(authenticationCredentialsProviders.ToArray(),
                requestData.S3SourceLocation,
                requestData.S3TargetLocation);

        var response = await AwsRequestHandler.ExecuteAction(()
            => translator.StartTextTranslationJobAsync(request));

        return new()
        {
            JobId = response.JobId,
            JobStatus = response.JobStatus
        };
    }

    [Action("List translation jobs", Description = "List all translation jobs")]
    public async Task<AllJobsResponse> ListJobs(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
    {
        var results = new List<JobResponse>();
        var translator = TranslatorFactory
            .CreateTranslator(authenticationCredentialsProviders.ToArray());

        string? nextToken = null;

        do
        {
            var request = new ListTextTranslationJobsRequest
            {
                NextToken = nextToken
            };
            var response = await AwsRequestHandler.ExecuteAction(()
                => translator.ListTextTranslationJobsAsync(request));

            nextToken = response.NextToken;

            var jobs = response.TextTranslationJobPropertiesList
                .Select(x => new JobResponse(x))
                .ToArray();

            results.AddRange(jobs);
        } while (!string.IsNullOrEmpty(nextToken));

        return new(results);
    }

    [Action("Describe a translation job", Description = "Describe a specific translation job")]
    public async Task<JobResponse> DescribeJob(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] JobRequest job)
    {
        var translator = TranslatorFactory
            .CreateTranslator(authenticationCredentialsProviders.ToArray());

        var response = await translator.DescribeTextTranslationJobAsync(new()
        {
            JobId = job.JobId
        });

        return new(response.TextTranslationJobProperties);
    }
    
    [Action("Stop a translation job", Description = "Stop a specific translation job")]
    public async Task<SmallJobResponse> StopJob(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        [ActionParameter] JobRequest job)
    {
        var translator = TranslatorFactory
            .CreateTranslator(authenticationCredentialsProviders.ToArray());

        var response = await translator.StopTextTranslationJobAsync(new()
        {
            JobId = job.JobId
        });

        return new()
        {
            JobId = response.JobId,
            JobStatus = response.JobStatus
        };
    }
}