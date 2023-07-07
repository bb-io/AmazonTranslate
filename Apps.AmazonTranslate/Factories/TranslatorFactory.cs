using Amazon;
using Amazon.Translate;
using Apps.AmazonTranslate.Constants;
using Apps.AmazonTranslate.Utils;
using Blackbird.Applications.Sdk.Common.Authentication;

namespace Apps.AmazonTranslate.Factories;

public static class TranslatorFactory
{
    public static AmazonTranslateClient CreateTranslator(
        AuthenticationCredentialsProvider[] authenticationCredentialsProviders,
        RegionEndpoint? region = default)
    {
        var key = authenticationCredentialsProviders.First(p => p.KeyName == "access_key");
        var secret = authenticationCredentialsProviders.First(p => p.KeyName == "access_secret");

        if (string.IsNullOrEmpty(key.Value) || string.IsNullOrEmpty(secret.Value))
            throw new Exception(ExceptionMessages.CredentialsMissing);
            
        return new(key.Value, secret.Value, new AmazonTranslateConfig
        {
            RegionEndpoint = region ?? RegionEndpoint.USWest2
        });
    }
    
    // The client should have the same region as the bucket, so we have to find out
    // bucket's region in the first place and create new client with this region
    public static async Task<AmazonTranslateClient> CreateBucketTranslator(
        AuthenticationCredentialsProvider[] authenticationCredentialsProviders,
        string sourceLocation,
        string targetLocation)
    {
        var sourceBucketRegion =
            await S3BucketUtils.GetBucketRegion(authenticationCredentialsProviders, sourceLocation);
        
        var targetBucketRegion =
            await S3BucketUtils.GetBucketRegion(authenticationCredentialsProviders, targetLocation);

        if (sourceBucketRegion.SystemName != targetBucketRegion.SystemName)
            throw new("Source and target bucket regions must match");
        
        return CreateTranslator(authenticationCredentialsProviders, sourceBucketRegion);
    }    
    
    public static async Task<AmazonTranslateClient> CreateBucketTranslator(
        AuthenticationCredentialsProvider[] authenticationCredentialsProviders,
        string location)
    {
        var bucketRegion =
            await S3BucketUtils.GetBucketRegion(authenticationCredentialsProviders, location);
        
        return CreateTranslator(authenticationCredentialsProviders, bucketRegion);
    }
}