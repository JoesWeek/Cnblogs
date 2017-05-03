namespace Cnblogs.Droid.UI.Views
{
    public interface IBookmarkEditView
    {
        void BookmarkEditFail(string msg);
        void BookmarkEditSuccess(int position);
    }
}