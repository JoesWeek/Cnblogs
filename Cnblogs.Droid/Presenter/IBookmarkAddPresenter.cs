using Cnblogs.Droid.Model;

namespace Cnblogs.Droid.Presenter
{
    public interface IBookmarkAddPresenter
    {
        void BookmarkAdd(AccessToken token, BookmarksModel bookmark);
    }
}