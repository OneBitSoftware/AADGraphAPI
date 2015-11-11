namespace AADGraphAPI
{
    using System.Configuration;
    public class Constants
    {
        public static string ResourceUrl = ConfigurationManager.AppSettings["ida:GraphUrl"];
        public static string ClientId = ConfigurationManager.AppSettings["ida:ClientId"];
        public static string AppKey = ConfigurationManager.AppSettings["ida:AppKey"];
        public static string TenantId = ConfigurationManager.AppSettings["ida:TenantId"];
        public static string AuthString = ConfigurationManager.AppSettings["ida:Auth"] +
                                          ConfigurationManager.AppSettings["ida:TenantId"];
        public static string ClientSecret = ConfigurationManager.AppSettings["ida:ClientSecret"];
        public static string Authority = "https://login.windows.net/" + TenantId;
        public static string DiscoveryServiceEndpointUri = "https://api.office.com/discovery/v1.0/me/";
        public static string DiscoveryServiceResourceId = "https://api.office.com/discovery/";
    }
}