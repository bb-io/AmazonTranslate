using Amazon;
using Apps.AmazonTranslate.Extensions;
using Apps.AmazonTranslate.Factories;
using Apps.AmazonTranslate.Handlers;
using Blackbird.Applications.Sdk.Common.Authentication;

namespace Apps.AmazonTranslate.Utils;

public static class S3BucketUtils
{
    public static async Task<RegionEndpoint> GetBucketRegion(
        AuthenticationCredentialsProvider[] authenticationCredentialsProviders,
        string s3uri)
    {
        var bucketName = s3uri.GetBucket();
        
        var client = S3ClientFactory.CreateClient(authenticationCredentialsProviders);
        var locationResponse = await AwsRequestHandler.ExecuteAction(() 
            => client.GetBucketLocationAsync(bucketName));

        return RegionEndpoint.GetBySystemName(locationResponse.Location.Value);
    }
}