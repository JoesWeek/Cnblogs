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
    public class NewsColumnPresenter : INewsColumnPresenter
    {
        private INewsColumnView newsView;
        private int pageSize = 10;
        public NewsColumnPresenter(INewsColumnView newsView)
        {
            this.newsView = newsView;
        }
        public async Task GetServiceNews(AccessToken token, int position, int pageIndex = 1)
        {
            try
            {
                var url = "";
                switch (position)
                {
                    case 0:
                        url = string.Format(ApiUtils.NewsHome, pageIndex, pageSize);
                        break;
                    case 1:
                        url = string.Format(ApiUtils.NewsRecommend, pageIndex, pageSize);
                        break;
                    case 2:
                        url = string.Format(ApiUtils.NewsWorkHot, pageIndex, pageSize);
                        break;
                }
                var result = await OkHttpUtils.Instance(token).GetAsyn(url);
                if (result.IsError)
                {
                    newsView.GetServiceNewsFail(result.Message);
                }
                else
                {
                    var news = JsonConvert.DeserializeObject<List<NewsModel>>(result.Message);
                    switch (position)
                    {
                        case 1:
                            news.ForEach(s => s.IsRecommend = true);
                            break;
                        case 2:
                            news.ForEach(s => s.IsHot = true);
                            break;
                    }
                    await SQLiteUtils.Instance().UpdateNews(news);
                    newsView.GetServiceNewsSuccess(news);
                }
            }
            catch (Exception ex)
            {
                newsView.GetServiceNewsFail(ex.Message);
            }
        }
        public async Task GetClientNews(int position)
        {
            List<NewsModel> list = new List<NewsModel>();
            switch (position)
            {
                case 0:
                    list = await SQLiteUtils.Instance().QueryNews(pageSize);
                    break;
                case 1:
                    list = await SQLiteUtils.Instance().QueryNewsByRecommend(pageSize);
                    break;
                case 2:
                    var startdate = DateTimeUtils.GetMondayDate(DateTime.Now);
                    list = await SQLiteUtils.Instance().QueryNewsByWorkHot(pageSize, startdate);
                    break;
            }
            newsView.GetClientNewsSuccess(list);
        }
    }
}