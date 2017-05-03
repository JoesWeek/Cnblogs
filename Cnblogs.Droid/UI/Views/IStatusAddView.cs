using Cnblogs.Droid.Model;

namespace Cnblogs.Droid.UI.Views
{
    public interface IStatusAddView
    {
        void StatusAddFail(string msg);
        void StatusAddSuccess(StatusModel model);
    }
}