namespace Xamarin365Demo.Helpers
{
    public static class OAuthSettings
    {
        public const string ApplicationId = "REPLACE-THIS-VALUE";
        public const string Scopes = "ChannelMessage.Send Directory.ReadWrite.All Files.ReadWrite Group.ReadWrite.All Team.ReadBasic.All User.Read";
        // THIS VALUE SHOULD BE YOUR PACKAGE NAME 
        public const string RedirectUri = "msauth://com.companyname.Xamarin365Demo";
    }
}
