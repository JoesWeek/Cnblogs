using Android.Views;
using Cnblogs.Droid.Model;
using System.Collections.Generic;

namespace Cnblogs.Droid.UI.Views
{
    public interface IBookmarksView
    {
        void GetServiceBookmarksFail(string msg);
        void GetServiceBookmarksSuccess(List<BookmarksModel> lists);
        void GetClientBookmarksSuccess(List<BookmarksModel> lists);
        void DeleteBookmarkFail(int position, string msg);
        void DeleteBookmarkSuccess(int position);
    }
}