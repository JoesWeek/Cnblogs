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
    public class KbArticlePresenter : IKbArticlePresenter
    {
        private IKbArticleView kbarticleView;
        public KbArticlePresenter(IKbArticleView kbarticleView)
        {
            this.kbarticleView = kbarticleView;
        }
        public async Task GetServiceKbArticle(AccessToken token, int id)
        {
            try
            {
                var result = await OkHttpUtils.Instance(token).GetAsyn(string.Format(ApiUtils.KbArticlesBody, id));
                if (result.IsError)
                {
                    kbarticleView.GetServiceKbArticleFail(result.Message);
                }
                else
                {
                    await SQLiteUtils.Instance().QueryKbArticle(id).ContinueWith(async (response) =>
                    {
                        var article = response.Result;
                        article.Body = result.Message;
                        await SQLiteUtils.Instance().UpdateKbArticle(article);
                        kbarticleView.GetServiceKbArticleSuccess(article);
                    });
                }
            }
            catch (Exception ex)
            {
                kbarticleView.GetServiceKbArticleFail(ex.Message);
            }
        }
        public async Task GetClientKbArticle(int id)
        {
            kbarticleView.GetClientKbArticleSuccess(await SQLiteUtils.Instance().QueryKbArticle(id));
        }
    }
}