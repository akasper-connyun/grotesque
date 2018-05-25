using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Grotesque.Util
{
    public class Authentication
    {
        private static string ApplicationClientId = "c5a4e5c6-0faf-47ff-b0d9-e3be81df496e";
        private static string ApplicationClientSecret = "oQBYxeSKcHyUod291a74q8rEKQ6X2rlcI2jacOnI7cg=";
        private static string Tenant = "connyun.onmicrosoft.com";

        public static async Task<string> AcquireAccessTokenAsync()
        {
            var authenticationContext = new AuthenticationContext($"https://login.windows.net/{Tenant}", TokenCache.DefaultShared);

            AuthenticationResult token = await authenticationContext.AcquireTokenAsync(resource: "https://api.timeseries.azure.com/",
                clientCredential: new ClientCredential(clientId: ApplicationClientId, clientSecret: ApplicationClientSecret));

            return token.AccessToken;
        }
    }
}
