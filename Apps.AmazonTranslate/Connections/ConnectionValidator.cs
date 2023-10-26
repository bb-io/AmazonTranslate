using Amazon.Translate.Model;
using Apps.AmazonTranslate.Factories;
using Apps.AmazonTranslate.Handlers;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;

namespace Apps.AmazonTranslate.Connections
{
    public class ConnectionValidator : IConnectionValidator
    {
        public async ValueTask<ConnectionValidationResponse> ValidateConnection(IEnumerable<AuthenticationCredentialsProvider> authProviders, CancellationToken cancellationToken)
        {
            var translator = TranslatorFactory.CreateTranslator(authProviders.ToArray());
            var request = new ListLanguagesRequest()
            {
                NextToken = null,
                MaxResults = 100
            };            

            try
            {
                var response = await AwsRequestHandler.ExecuteAction(() => translator.ListLanguagesAsync(request, cancellationToken));
                
                return new()
                {
                    IsValid = true,
                    Message = "Success"
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    IsValid = false,
                    Message = ex.Message
                };
            }
        }
    }
}
