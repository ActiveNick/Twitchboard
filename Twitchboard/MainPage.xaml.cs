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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Twitchboard
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!ConfigSecrets.ISTWITTERCONFIGDONE)
            {
                await new MessageDialog("You forgot to initialize your own Twitter app settings. See the comments in the ConfigSecrets.cs file for details.").ShowAsync();
                return;
            }

            TwitterService.Instance.Initialize(ConfigSecrets.TwitterConsumerKey, ConfigSecrets.TwitterConsumerSecret, ConfigSecrets.TwitterCallbackUri);

            if (!await TwitterService.Instance.LoginAsync())
            {
                var error = new MessageDialog("Unable to log to Twitter");
                await error.ShowAsync();
                return;
            }

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

            // Temprorarily fetching the HoloLens user timeline just to test formatting with two columns
            lstTimeline.ItemsSource = await TwitterService.Instance.GetUserTimeLineAsync("HoloLens", 50);
            lstTweets.ItemsSource = await TwitterService.Instance.GetUserTimeLineAsync(user.ScreenName, 50);
        }

        private void HamburgerMenu_OnItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void HamburgerMenu_OnOptionsItemClick(object sender, ItemClickEventArgs e)
        {

        }
    }
}
