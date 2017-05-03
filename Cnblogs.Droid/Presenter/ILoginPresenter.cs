using Cnblogs.Droid.Model;

namespace Cnblogs.Droid.Presenter
{
    public interface ILoginPresenter
    {
        void Login(AccessToken token, string basic, string account,string password);
    }
}