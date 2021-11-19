using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace PiGraphQlDesktopClient.Helpers
{
    public class MsalAuthHelper
    {

        private IPublicClientApplication _app;

        private static readonly string ClientId = "fa079832-3e7b-4a74-8e3b-fea4efcd0f65";

        //https://github.com/Azure-Samples/active-directory-b2c-dotnet-desktop/blob/msalv3/active-directory-b2c-wpf/App.xaml.cs
        private static readonly string TenantName = "ThinkIQB2C";
        private static readonly string Tenant = $"{TenantName}.onmicrosoft.com";
        private static readonly string AzureAdB2CHostname = $"{TenantName}.b2clogin.com";

        public static string PolicySignUpSignIn = "B2C_1_SignUpAndSignIn";

        private static string AuthorityBase = $"https://{AzureAdB2CHostname}/tfp/{Tenant}/";
        public static string AuthoritySignUpSignIn = $"{AuthorityBase}{PolicySignUpSignIn}";

        private static readonly string RedirectUri = $"https://{TenantName}.b2clogin.com/oauth2/nativeclient";

        //https://ThinkIQB2C.onmicrosoft.com/634b70ac-b69c-4c3f-91b6-c3b403877e35/queue.manage
        private static readonly string SpListScope = $"https://{Tenant}/634b70ac-b69c-4c3f-91b6-c3b403877e35/queue.manage";

        private static readonly string[] Scopes = { SpListScope };

        private IList<IAccount> accounts;
        public AuthenticationResult AuthenticationResult;

        public IAccount ActiveAccount
        {
            get
            {
                return accounts.FirstOrDefault();
            }
        }

        public async Task<string> GetTokenAsync()
        {
            var aAccount = accounts.FirstOrDefault();
            if (aAccount == null)
            {
                return "";
            }
            else
            {
                AuthenticationResult = await _app.AcquireTokenSilent(Scopes, aAccount)
                    .ExecuteAsync()
                    .ConfigureAwait(false);
                return AuthenticationResult.AccessToken;
            }
        }

        public MsalAuthHelper()
        {
            //ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            _app = PublicClientApplicationBuilder.Create(ClientId)
                .WithB2CAuthority(AuthoritySignUpSignIn)
                .WithRedirectUri("http://localhost")
                .Build();
            TokenCacheHelper.EnableSerialization(_app.UserTokenCache);
            accounts = _app.GetAccountsAsync().Result.ToList();
        }

        public async Task SignInAsync()
        {
            accounts = (await _app.GetAccountsAsync()).ToList();
            AuthenticationResult = null;
            try
            {
                AuthenticationResult = await _app.AcquireTokenInteractive(Scopes)
                    .WithUseEmbeddedWebView(true)
                    //.WithParentActivityOrWindow(new WindowInteropHelper(this).Handle)
                    .WithAccount(accounts.FirstOrDefault())
                    //.WithPrompt(Prompt.SelectAccount)
                    .ExecuteAsync();

                accounts = (await _app.GetAccountsAsync()).ToList();

            }
            catch (MsalUiRequiredException)
            {

            }
            catch (MsalException ex)
            {
                // An unexpected error occurred.
                string message = ex.Message;
                if (ex.InnerException != null)
                {
                    message += "Error Code: " + ex.ErrorCode + "Inner Exception : " + ex.InnerException.Message;
                }
                MessageBox.Show(message);
            }


        }

        public async Task SignOutAsync()
        {
            // clear the cache
            while (accounts.Any())
            {
                await _app.RemoveAsync(accounts.First());
                accounts = (await _app.GetAccountsAsync()).ToList();
            }
            AuthenticationResult = null;
        }

        public bool IsSignedIn()
        {
            if (accounts.Any())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal async Task<object> GetNameOfActiveAccountAsync()
        {
            //using System.IdentityModel.Tokens.Jwt;
            var handler = new JwtSecurityTokenHandler();
            var tokenContent = (JwtSecurityToken)handler.ReadToken(await GetTokenAsync());
            var email = tokenContent.Claims.FirstOrDefault(x => x.Type == "emails")?.Value;
            var name = tokenContent.Claims.FirstOrDefault(x => x.Type == "name")?.Value;
            return name;
        }
    }
}
