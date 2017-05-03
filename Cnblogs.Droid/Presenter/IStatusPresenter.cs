using Cnblogs.Droid.Model;
using System.Threading.Tasks;

namespace Cnblogs.Droid.Presenter
{
    public interface IStatusPresenter
    {
        Task GetServiceStatus(AccessToken token, int id);
        Task GetClientStatus(int id);
    }
}