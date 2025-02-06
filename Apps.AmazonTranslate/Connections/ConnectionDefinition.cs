using Amazon;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;

namespace Apps.AmazonTranslate.Connections;

public class ConnectionDefinition : IConnectionDefinition
{
    public IEnumerable<ConnectionPropertyGroup> ConnectionPropertyGroups => new List<ConnectionPropertyGroup>
    {
        new()
        {
            Name = "Developer API key",
            AuthenticationType = ConnectionAuthenticationType.Undefined,
            ConnectionProperties = new List<ConnectionProperty>
            {
                new("access_key") { DisplayName = "Access key"},
                new("access_secret") { DisplayName = "Access secret", Sensitive= true },
                new("region") { DisplayName = "Region", DataItems = RegionEndpoint.EnumerableAllRegions.Select(x => new ConnectionPropertyValue(x.SystemName, x.SystemName)) }
            }
        }
    };

    public IEnumerable<AuthenticationCredentialsProvider> CreateAuthorizationCredentialsProviders(Dictionary<string, string> values)
    {
        return values.Select(x => new AuthenticationCredentialsProvider(x.Key, x.Value)).ToList();
    }
}