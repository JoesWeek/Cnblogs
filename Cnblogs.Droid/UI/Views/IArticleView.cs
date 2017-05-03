using Cnblogs.Droid.Model;

namespace Cnblogs.Droid.UI.Views
{
    public interface IArticleView
    {
        void GetServiceArticleFail(string msg);
        void GetServiceArticleSuccess(ArticlesModel model);
        void GetClientArticleSuccess(ArticlesModel model);
    }
}