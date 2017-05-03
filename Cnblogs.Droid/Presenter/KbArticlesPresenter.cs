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
    public class KbArticlesPresenter : IKbArticlesPresenter
    {
        private IKbArticlesView articlesView;
        private int pageSize = 10;
        public KbArticlesPresenter(IKbArticlesView articlesView)
        {
            this.articlesView = articlesView;
        }
        public async Task GetServiceKbArticles(AccessToken token, int pageIndex = 1)
        {
            try
            {
                var result = await OkHttpUtils.Instance(token).GetAsyn(string.Format(ApiUtils.KbArticles, pageIndex, pageSize));
                if (result.IsError)
                {
                    articlesView.GetServiceKbArticlesFail(result.Message);
                }
                else
                {
                    var articles = JsonConvert.DeserializeObject<List<KbArticlesModel>>(result.Message);
                    await SQLiteUtils.Instance().UpdateKbArticles(articles);
                    articlesView.GetServiceKbArticlesSuccess(articles);
                }
            }
            catch (Exception ex)
            {
                articlesView.GetServiceKbArticlesFail(ex.Message);
            }
        }
        public async Task GetClientKbArticles()
        {
            articlesView.GetClientKbArticlesSuccess(await SQLiteUtils.Instance().QueryKbArticles(pageSize));
        }
    }
}