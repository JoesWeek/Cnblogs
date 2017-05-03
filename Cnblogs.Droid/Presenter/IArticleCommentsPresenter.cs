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
    public interface IArticleCommentsPresenter
    {
        Task GetComment(AccessToken token, string blogApp, int id, int pageIndex = 1);
        void PostComment(AccessToken token, string blogApp, int id, string content);
    }
}