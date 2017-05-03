using Cnblogs.Droid.Model;
using Cnblogs.Droid.UI.Views;
using Cnblogs.Droid.Utils;
using System;
using System.Threading.Tasks;

namespace Cnblogs.Droid.Presenter
{
    public class NewsBodyPresenter : INewsBodyPresenter
    {
        private INewsBodyView newsView;
        public NewsBodyPresenter(INewsBodyView newsView)
        {
            this.newsView = newsView;
        }
        public async Task GetServiceNews(AccessToken token, int id)
        {
            try
            {
                var result = await OkHttpUtils.Instance(token).GetAsyn(string.Format(ApiUtils.NewsBody, id));
                if (result.IsError)
                {
                    newsView.GetServiceNewsFail(result.Message);
                }
                else
                {
                    await SQLiteUtils.Instance().QueryNew(id).ContinueWith(async (response) =>
                    {
                        var news = response.Result;
                        news.Body = result.Message;
                        await SQLiteUtils.Instance().UpdateNew(news);
                        newsView.GetServiceNewsSuccess(news);
                    });
                }
            }
            catch (Exception ex)
            {
                newsView.GetServiceNewsFail(ex.Message);
            }
        }
        public async Task GetClientNews(int id)
        {
            newsView.GetClientNewsSuccess(await SQLiteUtils.Instance().QueryNew(id));
        }
    }
}