using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Net.Http.Headers;

using Microsoft.Graph;
using Microsoft.Identity.Client;

using Xamarin.Forms;
using Xamarin365Demo.Helpers;

namespace Xamarin365Demo
{
    public partial class App : Xamarin.Forms.Application, INotifyPropertyChanged
    {
        #region Static Variables
        // UIParent used by Android version of the app
        public static object AuthUIParent = null;

        // Keychain security group used by iOS version of the app
        public static string iOSKeychainSecurityGroup = null;

        // Microsoft Authentication client for native/mobile apps
        public static IPublicClientApplication PCA;

        // Microsoft Graph client
        public static GraphServiceClient GraphClient;

        // Microsoft Graph permissions used by app
        private readonly string[] Scopes = OAuthSettings.Scopes.Split(' ');
        #endregion

        #region Properties and Fields

        public static bool IsSigned;
        public bool IsSignedOut { get { return !isSignedIn; } }

        // Is a user signed in?
        private bool isSignedIn;

        public bool IsSignedIn
        {
            get => isSignedIn;
            set
            {
                isSignedIn = value;
                IsSigned = isSignedIn;
                OnPropertyChanged("IsSignedIn");
                OnPropertyChanged("IsSignedOut");
            }
        }

        // The user's display name
        private string userName;

        public string UserName
        {
            get => userName; 
            set { userName = value; OnPropertyChanged("UserName"); }
        }

        // The user's email address
        private string userEmail;

        public string UserEmail
        {
            get => userEmail;
            set { userEmail = value; OnPropertyChanged("UserEmail"); }
        }

        // The user's profile photo
        private ImageSource userPhoto;

        public ImageSource UserPhoto
        {
            get => userPhoto; 
            set { userPhoto = value; OnPropertyChanged("UserPhoto"); }
        }

        private Stream GetUserPhoto() =>
            Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("Xamarin365Demo.no-profile-pic.png");
        #endregion

        #region Constructor and Methods
        public App()
        {
            InitializeComponent();

            InitialSetup();
            MainPage = new AppShell();
        }

        void InitialSetup()
        {
            var builder = PublicClientApplicationBuilder
                .Create(OAuthSettings.ApplicationId)
                .WithRedirectUri(OAuthSettings.RedirectUri);

            if (!string.IsNullOrEmpty(iOSKeychainSecurityGroup))
                builder = builder.WithIosKeychainSecurityGroup(iOSKeychainSecurityGroup);

            PCA = builder.Build();
        }

        public async Task SignIn()
        {
            try
            {
                var accounts = await PCA.GetAccountsAsync();

                var silentAuthResult = await PCA
                    .AcquireTokenSilent(Scopes, accounts.FirstOrDefault())
                    .ExecuteAsync();

                Debug.WriteLine("User already signed in.");
                Debug.WriteLine($"Successful silent authentication for: {silentAuthResult.Account.Username}");
                Debug.WriteLine($"Access token: {silentAuthResult.AccessToken}");
            }
            catch (MsalUiRequiredException msalEx)
            {
                Debug.WriteLine("Silent token request failed, user needs to sign-in: " + msalEx.Message);
                var interactiveRequest = PCA.AcquireTokenInteractive(Scopes);

                if (AuthUIParent != null)
                    interactiveRequest = interactiveRequest.WithParentActivityOrWindow(AuthUIParent);

                var interactiveAuthResult = await interactiveRequest.ExecuteAsync();
                Debug.WriteLine($"Successful interactive authentication for: {interactiveAuthResult.Account.Username}");
                Debug.WriteLine($"Access token: {interactiveAuthResult.AccessToken}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Authentication failed. See exception messsage for more details: " + ex.Message);
            }

            await InitializeGraphClientAsync();
        }

        public async Task SignOut()
        {
            var accounts = await PCA.GetAccountsAsync();

            while (accounts.Any())
            {
                await PCA.RemoveAsync(accounts.First());
                accounts = await PCA.GetAccountsAsync();
            }

            UserPhoto = null;
            UserName = string.Empty;
            UserEmail = string.Empty;
            IsSignedIn = false;
        }

        private async Task InitializeGraphClientAsync()
        {
            var currentAccounts = await PCA.GetAccountsAsync();

            try
            {
                if (currentAccounts.Count() > 0)
                {
                    var provider = new DelegateAuthenticationProvider(
                        async (requestMessage) =>
                        {
                            var firstAccount = currentAccounts.FirstOrDefault();
                            var result = await PCA.AcquireTokenSilent(Scopes, firstAccount).ExecuteAsync();

                            requestMessage.Headers.Authorization =
                                new AuthenticationHeaderValue("Bearer", result.AccessToken);
                        });

                    GraphClient = new GraphServiceClient(provider);

                    await GetUserInfo();
                    IsSignedIn = true;
                }
                else
                    IsSignedIn = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to initialized graph client.");
                Debug.WriteLine($"Accounts in the msal cache: {currentAccounts.Count()}.");
                Debug.WriteLine($"See exception message for details: {ex.Message}");
            }
        }

        private async Task GetUserInfo()
        {
            var user = await GraphClient.Me.Request().GetAsync();

            UserName = user.DisplayName;
            UserPhoto = ImageSource.FromStream(() => GetUserPhoto());
            UserEmail = string.IsNullOrEmpty(user.Mail) ? user.UserPrincipalName : user.Mail;
        }
        #endregion
    }
}