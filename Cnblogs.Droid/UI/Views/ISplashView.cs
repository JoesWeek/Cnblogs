using Cnblogs.Droid.Model;

namespace Cnblogs.Droid.UI.Views
{
    public interface ISplashView
    {
        void GetAccessTokenFail(string msg);
        void GetAccessTokenSuccess(AccessToken token);
    }
}