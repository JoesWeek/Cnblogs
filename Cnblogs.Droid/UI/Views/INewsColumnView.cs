using Cnblogs.Droid.Model;
using System.Collections.Generic;

namespace Cnblogs.Droid.UI.Views
{
    public interface INewsColumnView
    {
        void GetServiceNewsFail(string msg);
        void GetServiceNewsSuccess(List<NewsModel> lists);
        void GetClientNewsSuccess(List<NewsModel> lists);
    }
}