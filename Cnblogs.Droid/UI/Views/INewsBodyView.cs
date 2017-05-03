using Cnblogs.Droid.Model;

namespace Cnblogs.Droid.UI.Views
{
    public interface INewsBodyView
    {
        void GetServiceNewsFail(string msg);
        void GetServiceNewsSuccess(NewsModel model);
        void GetClientNewsSuccess(NewsModel model);
    }
}