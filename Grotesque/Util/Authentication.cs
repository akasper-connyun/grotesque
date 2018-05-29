using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Grotesque.Util
{
    public class Authentication
    {
        private static string ApplicationClientId = System.Environment.GetEnvironmentVariable("TSI_SP_CID");
        private static string ApplicationClientSecret = System.Environment.GetEnvironmentVariable("TSI_SP_SECRET");
        private static string Tenant = "connyun.onmicrosoft.com";
        private static AuthenticationContext authenticationContext = new AuthenticationContext($"https://login.windows.net/{Tenant}", TokenCache.DefaultShared);
        private static ClientCredential clientCredential = new ClientCredential(clientId: ApplicationClientId, clientSecret: ApplicationClientSecret);

        public static async Task<string> AcquireAccessTokenAsync()
        {
            AuthenticationResult token = await authenticationContext.AcquireTokenAsync(resource: "https://api.timeseries.azure.com/", clientCredential: clientCredential);

            return token.AccessToken;
        }
    }
}
