using Cnblogs.Droid.Model;
using Cnblogs.Droid.UI.Views;
using Cnblogs.Droid.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cnblogs.Droid.Presenter
{
    public class SearchColumnPresenter : ISearchColumnPresenter
    {
        private ISearchColumnView searchView;
        private int pageSize = 10;
        public SearchColumnPresenter(ISearchColumnView searchView)
        {
            this.searchView = searchView;
        }
        public async Task Search(AccessToken token, int position, string keyWords, int pageIndex = 1)
        {
            try
            {
                var url = "";
                switch (position)
                {
                    case 0:
                        url = string.Format(ApiUtils.Search, "bolgs", keyWords, pageIndex, pageSize);
                        break;
                    case 1:
                        url = string.Format(ApiUtils.Search, "news", keyWords, pageIndex, pageSize);
                        break;
                    case 2:
                        url = string.Format(ApiUtils.Search, "kb", keyWords, pageIndex, pageSize);
                        break;
                    case 3:
                        url = string.Format(ApiUtils.Search, "question", keyWords, pageIndex, pageSize);
                        break;
                }
                var result = await OkHttpUtils.Instance(token).GetAsyn(url);
                if (result.IsError)
                {
                    searchView.SearchFail(result.Message);
                }
                else
                {
                    var searchs = JsonConvert.DeserializeObject<List<SearchModel>>(result.Message);
                    switch (position)
                    {
                        case 0:
                            var articles = new List<ArticlesModel>();
                            foreach (var item in searchs)
                            {
                                articles.Add(new ArticlesModel()
                                {
                                    Author = item.UserName,
                                    Avatar = item.UserName,
                                    BlogApp = item.UserAlias,
                                    Body = "",
                                    CommentCount = item.CommentTimes,
                                    Description = item.Content,
                                    DiggCount = item.VoteTimes,
                                    Id = int.Parse(item.Id),
                                    PostDate = DateTime.Parse(item.PublishTime),
                                    Title = item.Title.Replace("<strong>", "").Replace("</strong>", ""),
                                    Url = item.Uri,
                                    ViewCount = item.ViewTimes
                                });
                            }
                            await SQLiteUtils.Instance().UpdateArticles(articles);
                            url = string.Format(ApiUtils.Search, "bolgs", keyWords, pageIndex, pageSize);
                            break;
                        case 1:
                            url = string.Format(ApiUtils.Search, "news", keyWords, pageIndex, pageSize);
                            var news = new List<NewsModel>();
                            foreach (var item in searchs)
                            {
                                news.Add(new NewsModel()
                                {
                                    IsHot = false,
                                    IsRecommend = false,
                                    TopicIcon = "",
                                    TopicId = 0,
                                    Body = "",
                                    CommentCount = item.CommentTimes,
                                    Summary = item.Content,
                                    DiggCount = item.VoteTimes,
                                    Id = int.Parse(item.Id),
                                    DateAdded = DateTime.Parse(item.PublishTime),
                                    Title = item.Title.Replace("<strong>", "").Replace("</strong>", ""),
                                    ViewCount = item.ViewTimes
                                });
                            }
                            await SQLiteUtils.Instance().UpdateNews(news);
                            break;
                        case 2:
                            url = string.Format(ApiUtils.Search, "kb", keyWords, pageIndex, pageSize);
                            var kbArticles = new List<KbArticlesModel>();
                            foreach (var item in searchs)
                            {
                                kbArticles.Add(new KbArticlesModel()
                                {
                                    Author = item.UserName,
                                    Body = "",
                                    Summary = item.Content,
                                    DiggCount = item.VoteTimes,
                                    Id = int.Parse(item.Id),
                                    DateAdded = DateTime.Parse(item.PublishTime),
                                    Title = item.Title.Replace("<strong>", "").Replace("</strong>", ""),
                                    ViewCount = item.ViewTimes
                                });
                            }
                            await SQLiteUtils.Instance().UpdateKbArticles(kbArticles);
                            break;
                        case 3:
                            url = string.Format(ApiUtils.Search, "question", keyWords, pageIndex, pageSize);
                            //var questions = new List<QuestionsModel>();
                            //foreach (var item in searchs)
                            //{
                            //    questions.Add(new QuestionsModel()
                            //    { 
                            //        Author = item.UserName,
                            //        Body = "",
                            //        Summary = item.Content,
                            //        DiggCount = item.VoteTimes,
                            //        Id = int.Parse(item.Id),
                            //        DateAdded = DateTime.Parse(item.PublishTime),
                            //        Title = item.Title.Replace("<strong>", "").Replace("</strong>", ""),
                            //        ViewCount = item.ViewTimes
                            //    });
                            //}
                            //await SQLiteUtils.Instance().UpdateQuestions(questions);
                            break;
                    }
                    searchView.SearchSuccess(searchs);
                }
            }
            catch (Exception ex)
            {
                searchView.SearchFail(ex.Message);
            }
        }
    }
}