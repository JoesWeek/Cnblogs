using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Cnblogs.Droid.Utils;
using Cnblogs.Droid.Model;
using Cnblogs.Droid.UI.Views;

namespace Cnblogs.Droid.Presenter
{
    public class ArticlePresenter : IArticlePresenter
    {
        private IArticleView articlesView;
        public ArticlePresenter(IArticleView articlesView)
        {
            this.articlesView = articlesView;
        }
        public async Task GetServiceArticle(AccessToken token, int id)
        {
            try
            {
                var result = await OkHttpUtils.Instance(token).GetAsyn(string.Format(ApiUtils.ArticleBody, id));
                if (result.IsError)
                {
                    articlesView.GetServiceArticleFail(result.Message);
                }
                else
                {
                    await SQLiteUtils.Instance().QueryArticle(id).ContinueWith(async (response) =>
                    {
                        var article = response.Result;
                        article.Body = result.Message;
                        await SQLiteUtils.Instance().UpdateArticle(article);
                        articlesView.GetServiceArticleSuccess(article);
                    });
                }
            }
            catch (Exception ex)
            {
                articlesView.GetServiceArticleFail(ex.Message);
            }
        }
        public async Task GetClientArticle(int id)
        {
            articlesView.GetClientArticleSuccess(await SQLiteUtils.Instance().QueryArticle(id));
        }
    }
}