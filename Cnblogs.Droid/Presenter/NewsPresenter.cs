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
    public class NewsPresenter : INewsPresenter
    {
        private INewsColumnView newsView;
        private int pageSize = 10;
        public NewsPresenter(INewsColumnView newsView)
        {
            this.newsView = newsView;
        }
        public async Task GetServiceNews(AccessToken token, int pageIndex = 1)
        {
            try
            {
                var result = await OkHttpUtils.Instance(token).GetAsyn(string.Format(ApiUtils.News, pageIndex, pageSize));
                if (result.IsError)
                {
                    newsView.GetServiceNewsFail(result.Message);
                }
                else
                {
                    var news = JsonConvert.DeserializeObject<List<NewsModel>>(result.Message);
                    newsView.GetServiceNewsSuccess(news);
                }
            }
            catch (Exception ex)
            {
                newsView.GetServiceNewsFail(ex.Message);
            }
        }
    }
}