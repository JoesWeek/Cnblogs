using Android.Views;
using Cnblogs.Droid.Model;
using System.Collections.Generic;

namespace Cnblogs.Droid.UI.Views
{
    public interface IStatusColumnView
    {
        void GetServiceStatusFail(string msg);
        void GetServiceStatusSuccess(List<StatusModel> lists);
        void GetClientStatusSuccess(List<StatusModel> lists);
        void DeleteStatusFail(int position, string msg);
        void DeleteStatusSuccess(int position);
    }
}