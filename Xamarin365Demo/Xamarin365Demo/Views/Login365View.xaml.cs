using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin365Demo.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login365View : ContentPage
    {
        public Login365View()
        {
            InitializeComponent();
        }

        private async void OnSignOut(object sender, EventArgs e)
        {
            var signout = await DisplayAlert("Logout?", "End session?", "Yes", "No");

            if (signout)
            {
                await (Application.Current as App).SignOut();
            }
        }

        private async void OnSignIn(object sender, EventArgs e)
        {
            try
            {
                await (Application.Current as App).SignIn();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Authentication error", ex.Message, "OK");
            }
        }
    }
}