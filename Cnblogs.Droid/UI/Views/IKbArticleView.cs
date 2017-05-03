using Cnblogs.Droid.Model;

namespace Cnblogs.Droid.UI.Views
{
    public interface IKbArticleView
    {
        void GetServiceKbArticleFail(string msg);
        void GetServiceKbArticleSuccess(KbArticlesModel model);
        void GetClientKbArticleSuccess(KbArticlesModel model);
    }
}