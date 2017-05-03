using Android.Views;
using Cnblogs.Droid.Model;
using System.Threading.Tasks;

namespace Cnblogs.Droid.Presenter
{
    public interface IStatusCommentsPresenter
    {
        Task GetServiceComments(AccessToken token, int id);
        void PostComment(AccessToken token, int id, string content);
        void DeleteComment(AccessToken token, int statusId, int id);
    }
}