using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using Cnblogs.Droid.Model;

namespace Cnblogs.Droid.Presenter
{
    public interface INewsPresenter
    {
        Task GetServiceNews(AccessToken token, int pageIndex = 1);
    }
}