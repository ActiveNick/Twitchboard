using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.Services.Twitter;
using Windows.UI.Popups;

// Social connection dashboard app for Windows 10
namespace Twitchboard
{
    /// <summary>
    /// Main application page used to navigate various social feeds
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // This is just to give you a warning in case you got the code on GitHub and just ran it without
            // configuring your Twitter settings.
            if (!ConfigSecrets.ISTWITTERCONFIGDONE)
            {
                await new MessageDialog("You forgot to initialize your own Twitter app settings. See the comments in the ConfigSecrets.cs file for details.").ShowAsync();
                return;
            }

            // Twitter initialization & user login
            TwitterService.Instance.Initialize(ConfigSecrets.TwitterConsumerKey, ConfigSecrets.TwitterConsumerSecret, ConfigSecrets.TwitterCallbackUri);
            if (!await TwitterService.Instance.LoginAsync())
            {
                var error = new MessageDialog("Unable to log to Twitter");
                await error.ShowAsync();
                return;
            }

            // Retrieve the Twitter user settings
            TwitterUser user;
            try
            {
                user = await TwitterService.Instance.GetUserAsync();
            }
            catch (TwitterException ex)
            {
                if ((ex.Errors?.Errors?.Length > 0) && (ex.Errors.Errors[0].Code == 89))
                {
                    await new MessageDialog("Invalid or expired token. Logging out. Re-connect for new token.").ShowAsync();
                    TwitterService.Instance.Logout();
                    return;
                }
                else
                {
                    throw ex;
                }
            }

            // Fetches the home feed of the user (i.e. what the user's followers tweeted)
            TwitterDataConfig tc = new TwitterDataConfig();
            tc.QueryType = TwitterQueryType.Home;
            lstTimeline.ItemsSource = await TwitterService.Instance.RequestAsync(tc, 50);

            // Fetches the status timeline of the user (i.e. what the user tweeted)
            lstTweets.ItemsSource = await TwitterService.Instance.GetUserTimeLineAsync(user.ScreenName, 50);

            // Fetches tweets for a specific query
            // TO DO: Add the ability to change this hardcoded query from the UI
            string query = "#HoloLens";
            lblQuery.Text = "Query: " + query;
            lstQuery.ItemsSource = await TwitterService.Instance.SearchAsync(query, 50);
        }

        private void HamburgerMenu_OnItemClick(object sender, ItemClickEventArgs e)
        {
            // Not implemented yet
        }

        private void HamburgerMenu_OnOptionsItemClick(object sender, ItemClickEventArgs e)
        {
            // Not implemented yet
        }
    }
}
