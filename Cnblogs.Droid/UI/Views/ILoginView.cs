using Cnblogs.Droid.Model;

namespace Cnblogs.Droid.UI.Views
{
    public interface ILoginView
    {
        void LoginFail(string msg);
        void LoginSuccess(AccessToken token, UserModel user);
    }
}