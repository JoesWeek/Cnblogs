using Android.Views;
using Cnblogs.Droid.Model;
using System.Threading.Tasks;

namespace Cnblogs.Droid.Presenter
{
    public interface IBookmarksPresenter
    {
        Task GetServiceBookmarks(AccessToken token,  int pageIndex = 1);
        Task GetClientBookmarks();
        void DeleteBookmarkAsync(AccessToken token, int id);
    }
}