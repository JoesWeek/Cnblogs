using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Cnblogs.Droid.UI.Views;
using System.Threading.Tasks;
using Cnblogs.Droid.Model;
using Newtonsoft.Json;
using Cnblogs.Droid.Utils;
using System.Net;
using System.Net.Http;
using System.IO;
using Square.OkHttp3;
using Java.Util.Concurrent;
using System.Net.Http.Headers;

namespace Cnblogs.Droid.Presenter
{
    public class NewsCommentsPresenter : INewsCommentsPresenter
    {
        private INewsCommentView commentView;
        private int pageSize = 10;
        public NewsCommentsPresenter(INewsCommentView commentView)
        {
            this.commentView = commentView;
        }
        public async Task GetComment(AccessToken token, int id, int pageIndex = 1)
        {
            try
            {
                var result = await OkHttpUtils.Instance(token).GetAsyn(string.Format(ApiUtils.NewsComment, id, pageIndex, pageSize));
                if (result.IsError)
                {
                    commentView.GetCommentFail(result.Message);
                }
                else
                {
                    var comments = JsonConvert.DeserializeObject<List<NewsCommentModel>>(result.Message);
                    commentView.GetCommentSuccess(comments);
                }
            }
            catch (Exception ex)
            {
                commentView.GetCommentFail(ex.Message);
            }
        }
        public void PostComment(AccessToken token, int id, string content)
        {
            try
            {
                var url = string.Format(ApiUtils.NewsCommentAdd, id.ToString());

                var param = new List<OkHttpUtils.Param>()
                {
                    new OkHttpUtils.Param("ParentId","0") ,
                    new OkHttpUtils.Param("Content",content) ,
                };

                OkHttpUtils.Instance(token).Post(url, param, async (call, response) =>
                {
                    var code = response.Code();
                    var body = await response.Body().StringAsync();
                    if (code == (int)System.Net.HttpStatusCode.Created)
                    {
                        var user = await SQLiteUtils.Instance().QueryUser();
                        NewsCommentModel news = new NewsCommentModel();
                        news.UserName = user.DisplayName;
                        news.FaceUrl = user.Face;
                        news.CommentContent = content;
                        news.DateAdded = DateTime.Now;
                        news.Floor = 0;
                        news.CommentID = Convert.ToInt32(body);
                        news.AgreeCount = 0;
                        news.AntiCount = 0;
                        news.ContentID = 0;
                        news.UserGuid = user.UserId;
                        commentView.PostCommentSuccess(news);
                    }
                    else
                    {
                        try
                        {
                            var error = JsonConvert.DeserializeObject<ErrorMessage>(body);
                            commentView.PostCommentFail(error.Message);
                        }
                        catch (Exception e)
                        {
                            commentView.PostCommentFail(e.Message);
                        }
                    }
                }, (call, ex) =>
                 {
                     commentView.PostCommentFail(ex.Message);
                 });
            }
            catch (Exception e)
            {
                commentView.PostCommentFail(e.Message);
            }
        }
        public void DeleteComment(AccessToken token, int id)
        {
            try
            {
                var url = string.Format(ApiUtils.NewsCommentDelete, id);

                OkHttpUtils.Instance(token).Delete(url, async (call, response) =>
                 {
                     var code = response.Code();
                     var body = await response.Body().StringAsync();
                     if (code == (int)System.Net.HttpStatusCode.OK)
                     {
                         commentView.DeleteCommentSuccess(id);
                     }
                     else
                     {
                         try
                         {
                             var error = JsonConvert.DeserializeObject<ErrorMessage>(body);
                             commentView.DeleteCommentFail(id, error.Message);
                         }
                         catch (Exception e)
                         {
                             commentView.DeleteCommentFail(id, e.Message);
                         }
                     }
                 }, (call, ex) =>
                 {
                     commentView.DeleteCommentFail(id, ex.Message);
                 });
            }
            catch (Exception ex)
            {
                commentView.DeleteCommentFail(id, ex.Message);
            }
        }
    }
}