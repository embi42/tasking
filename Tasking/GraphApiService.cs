using Microsoft.Identity.Client;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Tasking
{
    public class GraphApiService
    {
        private const string ClientId = "4c26d2d2-da1f-4dcd-a292-e235b7a73989";
        private readonly string[] _scopes = { "user.read", "notes.read", "notes.readwrite" };
        private readonly PublicClientApplication _publicClientApp;
        private AuthenticationResult _authResult;

        public GraphApiService()
        {
            _publicClientApp = new PublicClientApplication(ClientId);
        }

        public async Task<AuthenticationResult> Login()
        {
            _authResult = null;
            var accounts = await _publicClientApp.GetAccountsAsync();
            try
            {
                _authResult = await _publicClientApp.AcquireTokenSilentAsync(_scopes, accounts.FirstOrDefault());
            }
            catch (MsalUiRequiredException ex)
            {
                _authResult = await _publicClientApp.AcquireTokenAsync(_scopes);
            }

            return _authResult;
        }

        public async Task Logout()
        {
            var accounts = await _publicClientApp.GetAccountsAsync();
            var enumerable = accounts as IAccount[] ?? accounts.ToArray();
            if (enumerable.Any())
            {
                await _publicClientApp.RemoveAsync(enumerable.FirstOrDefault());
            }
        }

        public async Task<string> GetWithToken(string url)
        {
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                //Add the token in Authorization header
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authResult.AccessToken);
                var response = await httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
        }

        public async Task<string> PatchWithToken(string url, string content)
        {
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(new HttpMethod("PATCH"), url)
                {
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };
                //Add the token in Authorization header
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authResult.AccessToken);
                var response = await httpClient.SendAsync(request);
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
        }
    }
}
