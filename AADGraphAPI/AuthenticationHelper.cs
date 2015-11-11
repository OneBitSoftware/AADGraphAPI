namespace AADGraphAPI
{
    using AADGraphAPI.Models;
    using Microsoft.Azure.ActiveDirectory.GraphClient;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Microsoft.Office365.Discovery;
    using Microsoft.Office365.SharePoint.CoreServices;
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    public class AuthenticationHelper
    {
        public static string token;

        /// <summary>
        ///     Async task to acquire token for Application.
        /// </summary>
        /// <returns>Async Token for application.</returns>
        internal static async Task<string> AcquireTokenAsync()
        {
            if (token == null || string.IsNullOrWhiteSpace(token))
            {
                throw new Exception("Authorization Required.");
            }
            return token;
        }

        /// <summary>
        ///     Get Active Directory Client for Application.
        /// </summary>
        /// <returns>ActiveDirectoryClient for Application.</returns>
        internal static ActiveDirectoryClient GetActiveDirectoryClient()
        {
            //This example uses a static property class to store the aquired token
            //during the execution of Startup.Auth.cs

            Uri baseServiceUri = new Uri(Constants.ResourceUrl);
            ActiveDirectoryClient activeDirectoryClient =
                new ActiveDirectoryClient(
                    new Uri(baseServiceUri, Constants.TenantId),
                    async () => await AcquireTokenAsync()
                    );
            return activeDirectoryClient;
        }

        internal static async Task<SharePointClient> EnsureSharePointClientCreatedAsync(string capabilityName)
        {
            var signInUserId = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userObjectId = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

            //pay attention to the ADALTokenCache in this example
            AuthenticationContext authContext = new AuthenticationContext(
                Constants.Authority, new ADALTokenCache(signInUserId));

            try
            {
                //The discovery client gives us the discovery URL
                DiscoveryClient discClient = new DiscoveryClient(new Uri(Constants.DiscoveryServiceEndpointUri),
                    async () =>
                    {
                        var authResult =
                            await authContext.AcquireTokenSilentAsync(Constants.DiscoveryServiceResourceId,
                            new ClientCredential(Constants.ClientId,
                            Constants.ClientSecret),
                            new UserIdentifier(userObjectId,
                            UserIdentifierType.UniqueId));

                        return authResult.AccessToken;
                    });

                var dcr = await discClient.DiscoverCapabilityAsync(capabilityName);

                //We can now create a SharePointClient
                return new SharePointClient(dcr.ServiceEndpointUri,
                    async () =>
                    {
                        var authResult = await authContext.AcquireTokenSilentAsync(
                            dcr.ServiceResourceId,
                            new ClientCredential(Constants.ClientId,Constants.ClientSecret),
                            new UserIdentifier(userObjectId,UserIdentifierType.UniqueId));

                        return authResult.AccessToken;
                    });
            }
            catch (AdalException exception)
            {
                //Partially handle token acquisition failure here and bubble it up to the controller
                if (exception.ErrorCode == AdalError.FailedToAcquireTokenSilently)
                {
                    authContext.TokenCache.Clear();
                    throw exception;
                }
                return null;
            }
        }
    }
}