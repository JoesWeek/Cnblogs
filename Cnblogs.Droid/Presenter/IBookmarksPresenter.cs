using Android.Views;
using Cnblogs.Droid.Model;
using System.Threading.Tasks;

namespace Cnblogs.Droid.Presenter
{
    public interface IBookmarksPresenter
    {
        Task GetServiceBookmarks(AccessToken token,  int pageIndex = 1);
        Task GetClientBookmarks();
        void DeleteBookmark(AccessToken token, int id);
    }
}