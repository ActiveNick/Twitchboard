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
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.HockeyApp;

// Social connection dashboard app for Windows 10
namespace Twitchboard
{
    /// <summary>
    /// Main application page used to navigate various social feeds
    /// </summary>
    public sealed partial class MainPage : Page
    {
        TwitterUser user;

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

            // We won't await this to get the results faster in parallel
            PullHomeFeed();
            PullUserTimeline(user);
            PullQueryResults();
        }

        private async Task PullHomeFeed()
        {
            // Fetches the home feed of the user (i.e. what the user's followers tweeted)
            TwitterDataConfig tc = new TwitterDataConfig();
            tc.QueryType = TwitterQueryType.Home;
            lstHome.ItemsSource = await TwitterService.Instance.RequestAsync(tc, 50);
        }
        private async Task PullUserTimeline(TwitterUser user)
        {
            // Fetches the status timeline of the user (i.e. what the user tweeted)
            lstTimeline.ItemsSource = await TwitterService.Instance.GetUserTimeLineAsync(user.ScreenName, 50);
        }

        private async Task PullQueryResults()
        {
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

        private async void btnTweet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Post a tweet
                await TwitterService.Instance.TweetStatusAsync(txtTweet.Text);

                txtTweet.Text = "";
                // Refresh the users's timeline and home feeds every time they tweet
                PullHomeFeed();
                PullUserTimeline(user);

                HockeyClient.Current.TrackEvent("PostTweet");
            }
            catch (Exception ex)
            {
                // TO DO: Log the exception
                await new MessageDialog("Oops! Something went wrong when we tried to post your new tweet.").ShowAsync();

                HockeyClient.Current.TrackEvent("PostTweetError");
            }
        }

        // When the user taps a list header, that lists scrolls back to the top
        private void lblHome_Tapped(object sender, TappedRoutedEventArgs e)
        {
            lstHome.ScrollIntoView(lstHome.Items[0], ScrollIntoViewAlignment.Default);

            HockeyClient.Current.TrackEvent("HomeHeaderTapped");
        }

        // When the user taps a list header, that lists scrolls back to the top
        private void lblTimeline_Tapped(object sender, TappedRoutedEventArgs e)
        {
            lstTimeline.ScrollIntoView(lstTimeline.Items[0], ScrollIntoViewAlignment.Default);

            HockeyClient.Current.TrackEvent("TimelineHeaderTapped");
        }

        // When the user taps a list header, that lists scrolls back to the top
        private void lblQuery_Tapped(object sender, TappedRoutedEventArgs e)
        {
            lstQuery.ScrollIntoView(lstQuery.Items[0], ScrollIntoViewAlignment.Default);

            HockeyClient.Current.TrackEvent("QueryHeaderTapped");
        }

        private void lstHome_RefreshRequested(object sender, EventArgs e)
        {
            PullHomeFeed();

            HockeyClient.Current.TrackEvent("PullHomeFeed");
        }

        private void lstTimeline_RefreshRequested(object sender, EventArgs e)
        {
            PullUserTimeline(user);

            HockeyClient.Current.TrackEvent("PullUserTimeline");
        }

        private void lstQuery_RefreshRequested(object sender, EventArgs e)
        {
            PullQueryResults();

            HockeyClient.Current.TrackEvent("PullQueryResults");
        }

        // Favorite
        private void SlidableListItem_LeftCommandRequested(object sender, EventArgs e)
        {
            Tweet tw = (Tweet)((SlidableListItem)sender).DataContext;

            // TO DO: Favorite the selected tweet

            HockeyClient.Current.TrackEvent("SlideFavorited");
        }

        // Reply
        private void SlidableListItem_RightCommandRequested(object sender, EventArgs e)
        {
            // Get the selected tweet
            Tweet tw = (Tweet)((SlidableListItem)sender).DataContext;

            // Get the name of the user who posted this tweet and add it to the current tweet text
            txtTweet.Text += $"@{tw.User.ScreenName} ";
            txtTweet.Focus(FocusState.Programmatic);

            HockeyClient.Current.TrackEvent("SlideReply");
        }
    }
}
