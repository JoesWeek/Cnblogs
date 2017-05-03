using Cnblogs.Droid.Model;

namespace Cnblogs.Droid.Presenter
{
    public interface ISplashPresenter
    {
        void GetAccessToken(AccessToken token, string basic);
    }
}