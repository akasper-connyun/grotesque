using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Grotesque.Util
{
    public class Authentication
    {
        private static string ApplicationClientId = System.Environment.GetEnvironmentVariable("TSI_SP_CID");
        private static string ApplicationClientSecret = System.Environment.GetEnvironmentVariable("TSI_SP_SECRET");
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
