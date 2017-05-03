using Cnblogs.Droid.Model;

namespace Cnblogs.Droid.UI.Views
{
    public interface IBookmarkAddView
    {
        void BookmarkAddFail(string msg);
        void BookmarkAddSuccess();
    }
}