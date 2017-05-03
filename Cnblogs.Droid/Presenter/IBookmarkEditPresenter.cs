using Cnblogs.Droid.Model;

namespace Cnblogs.Droid.Presenter
{
    public interface IBookmarkEditPresenter
    {
        void BookmarkEdit(AccessToken token, BookmarksModel bookmark,int position);
    }
}