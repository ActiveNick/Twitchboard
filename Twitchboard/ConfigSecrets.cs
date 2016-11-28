namespace Twitchboard
{
    /// <summary>
    /// This static class is used to store all the application keys and secret codes and
    /// connection string details to connect your app to online services, including
    /// Twitter, Azure, HockeyApp, Email, etc.
    /// 
    /// </summary>
    static class ConfigSecrets
    {
        // TO DO: Once you have configured the required Twitter services, you can set the 
        // following bool constant to true
        public const bool ISTWITTERCONFIGDONE = false;

        // TO DO: IMPORTANT - These are my Twitter keys and I may change them when I feel like it (I may have already).
        // Go get your own at http://dev.twitter.com.
        public const string TwitterConsumerKey = "{INSERT YOUR KEY HERE}";
        public const string TwitterConsumerSecret = "{INSERT YOUR SECRET HERE}";
        public const string TwitterCallbackUri = "{INSERT YOUR TWITTER CALLBACK URI HERE - MUST MATCH YOUR TWITTER PORTAL APP SETTINGS";

        // HockeyApp
        public const string HockeyAppID = "{INSERT YOUR HOCKEYAPP APP ID HERE}";
        
        // TO DO: Once you have configured the required Azure services, you can set the 
        // following bool constant to true
        public const bool ISAZURECONFIGDONE = false;

        // Azure App Services secrets
        public const string AzureAppServicesURI = "https://yourmobileservice.azurewebsites.net/";
        public const string AzureAppServicesAppKey = "{INSERTYOURAZUREAPPSERVICEAPPKEYHERE}";

        // Azure Notification Hub secrets
        public const string AzureNotificationHubName = "{insertyourpushnotificationhubnamehere}";
        public const string AzureNotificationHubCnxString = "{InsertYourAzureNotificationHubSharedAccessConnectionStringHere}";

        // Developer details for support and such
        public const string DeveloperSupportEmail = "support@yourdomain.com";
    }

}
