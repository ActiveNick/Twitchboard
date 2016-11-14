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
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // TO DO: IMPORTANT - These are my Twitter keys and I may change them when I feel like it (I may have already).
        // Go get your own at http://dev.twitter.com.
        string twConsumerKey = "d8zOFA676xPQcvo0tkxfFuNIC";
        string twConsumerSecret = "zsVLyILEgYRGaAUXapaEyPT0QQeiLAFsbj7vBPGiTStIEKtgdM";
        string twCallbackUri = "http://ageofmobility.com";  // Dummy callback url because I'm not really using it

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            TwitterService.Instance.Initialize(twConsumerKey, twConsumerSecret, twCallbackUri);

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
