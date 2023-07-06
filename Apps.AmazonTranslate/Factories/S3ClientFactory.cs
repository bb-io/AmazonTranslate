using Amazon;
using Amazon.S3;
using Apps.AmazonTranslate.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;

namespace Apps.AmazonTranslate.Factories;

public static class S3ClientFactory
{
    public static AmazonS3Client CreateClient(
        AuthenticationCredentialsProvider[] authenticationCredentialsProviders)
    {
        var key = authenticationCredentialsProviders.First(p => p.KeyName == "access_key");
        var secret = authenticationCredentialsProviders.First(p => p.KeyName == "access_secret");

        if (string.IsNullOrEmpty(key.Value) || string.IsNullOrEmpty(secret.Value))
            throw new Exception(ExceptionMessages.CredentialsMissing);

        return new(key.Value, secret.Value, new AmazonS3Config
        {
            RegionEndpoint = RegionEndpoint.USWest1
        });
    }
}