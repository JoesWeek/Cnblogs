using Cnblogs.Droid.Model;
using System.Collections.Generic;

namespace Cnblogs.Droid.UI.Views
{
    public interface IKbArticlesView
    {
        void GetServiceKbArticlesFail(string msg);
        void GetServiceKbArticlesSuccess(List<KbArticlesModel> lists);
        void GetClientKbArticlesSuccess(List<KbArticlesModel> lists);
    }
}