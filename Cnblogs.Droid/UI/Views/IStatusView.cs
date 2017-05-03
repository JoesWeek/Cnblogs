using Cnblogs.Droid.Model;

namespace Cnblogs.Droid.UI.Views
{
    public interface IStatusView
    {
        void GetServiceStatusSuccess(StatusModel model);
        void GetServiceStatusFail(string msg);
        void GetClientStatusSuccess(StatusModel model);
    }
}