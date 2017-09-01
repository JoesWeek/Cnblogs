using Cnblogs.Droid.Model;
using Cnblogs.Droid.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cnblogs.Droid.Presenter
{
    public interface ILoginPresenter
    {
        Task LoginAsync(AccessToken token, string basic, string account, string password);
        void Login(AccessToken token, string content);
    }
}