using Cnblogs.Droid.Model;
using Cnblogs.Droid.UI.Views;
using Cnblogs.Droid.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cnblogs.Droid.Presenter
{
    public class ArticleColumnPresenter : IArticleColumnPresenter
    {
        private IArticleColumnView articlesView;
        private int pageSize = 10;
        public ArticleColumnPresenter(IArticleColumnView articlesView)
        {
            this.articlesView = articlesView;
        }
        public async Task GetServiceArticles(AccessToken token, int position, int pageIndex = 1)
        {
            try
            {
                var url = "";
                switch (position)
                {
                    case 0:
                        url = string.Format(ApiUtils.ArticleHome, pageIndex, pageSize);
                        break;
                    case 1:
                        url = string.Format(ApiUtils.ArticleHot, pageIndex, pageSize);
                        break;
                }
                var result = await OkHttpUtils.Instance(token).GetAsyn(url);
                if (result.IsError)
                {
                    articlesView.GetServiceArticlesFail(result.Message);
                }
                else
                {
                    var articles = JsonConvert.DeserializeObject<List<ArticlesModel>>(result.Message);
                    await SQLiteUtils.Instance().UpdateArticles(articles);
                    articlesView.GetServiceArticlesSuccess(articles);
                }
            }
            catch (Exception ex)
            {
                articlesView.GetServiceArticlesFail(ex.Message);
            }
        }
        public async Task GetClientArticles(int position)
        {
            List<ArticlesModel> list = new List<ArticlesModel>();
            switch (position)
            {
                case 0:
                    list = await SQLiteUtils.Instance().QueryArticles(pageSize);
                    break;
                case 1:
                    list = await SQLiteUtils.Instance().QueryArticlesByDigg(pageSize);
                    break;
            }
            articlesView.GetClientArticlesSuccess(list);
        }
    }
}