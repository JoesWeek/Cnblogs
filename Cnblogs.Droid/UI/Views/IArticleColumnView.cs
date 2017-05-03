using Cnblogs.Droid.Model;
using System.Collections.Generic;

namespace Cnblogs.Droid.UI.Views
{
    public interface IArticleColumnView
    {
        void GetServiceArticlesFail(string msg);
        void GetServiceArticlesSuccess(List<ArticlesModel> lists);
        void GetClientArticlesSuccess(List<ArticlesModel> lists);
    }
}