using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IdentityModel.Claims;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Owin;
using AADGraphAPI.Models;

namespace AADGraphAPI
{
    public partial class Startup
    {
        private static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static string appKey = ConfigurationManager.AppSettings["ida:ClientSecret"];
        private static string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        private static string tenantId = ConfigurationManager.AppSettings["ida:TenantId"];
        private static string postLogoutRedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];

        public static readonly string Authority = aadInstance + tenantId;

        // This is the resource ID of the AAD Graph API.  We'll need this to request a token to call the Graph API.
        string graphResourceId = "https://graph.windows.net";

        public void ConfigureAuth(IAppBuilder app)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseOpenIdConnectAuthentication(
                   new OpenIdConnectAuthenticationOptions
                   {
                       ClientId = Constants.ClientId,
                       Authority = Constants.Authority,

                    //TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters
                    //{
                    //    // Use the default validation (validating against a single issuer value, in line of business apps (single tenant apps))
                    //    //
                    //    // NOTE:
                    //    // * In a multitenant scenario, you can never validate against a fixed issuer string, as every tenant will send a different one.
                    //    // * If you don’t care about validating tenants, as is the case for apps giving access to 1st party resources, you just turn off validation.
                    //    // * If you do care about validating tenants, think of the case in which your app sells access to premium content and you want to limit access only to the tenant that paid a fee, 
                    //    //       you still need to turn off the default validation but you do need to add logic that compares the incoming issuer to a list of tenants that paid you, 
                    //    //       and block access if that’s not the case.
                    //    // * Refer to the following sample for a custom validation logic: https://github.com/AzureADSamples/WebApp-WebAPI-MultiTenant-OpenIdConnect-DotNet

                    //    ValidateIssuer = true
                    //},

                    Notifications = new OpenIdConnectAuthenticationNotifications()
                       {
                        //
                        // If there is a code in the OpenID Connect response, redeem it for an access token and refresh token, and store those away.

                        AuthorizationCodeReceived = (context) =>
                           {
                               var code = context.Code;

                               ClientCredential credential = new ClientCredential(Constants.ClientId,
                                   Constants.ClientSecret);
                               String UserObjectId = context.AuthenticationTicket.Identity.FindFirst(
                                   ClaimTypes.NameIdentifier).Value;

                               AuthenticationContext authContext = new AuthenticationContext(
                                   Constants.Authority, new ADALTokenCache(UserObjectId));

                               var result = authContext.AcquireTokenByAuthorizationCode(code,
                                   new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path)),
                                   credential, Constants.ResourceUrl);

                               AuthenticationHelper.token = result.AccessToken;

                               return Task.FromResult(0);
                           },


                           RedirectToIdentityProvider = (context) =>
                           {
                            // This ensures that the address used for sign in and sign out is picked up dynamically from the request
                            // this allows you to deploy your app (to Azure Web Sites, for example)without having to change settings
                            // Remember that the base URL of the address used here must be provisioned in Azure AD beforehand.
                            string appBaseUrl = context.Request.Scheme + "://" + context.Request.Host + context.Request.PathBase;
                               context.ProtocolMessage.RedirectUri = appBaseUrl + "/";
                               context.ProtocolMessage.PostLogoutRedirectUri = appBaseUrl;

                               return Task.FromResult(0);
                           },

                           AuthenticationFailed = (context) =>
                           {
                            // Suppress the exception if you don't want to see the error
                            context.HandleResponse();

                               return Task.FromResult(0);
                           }

                       }

                   });
        }
    }
}
