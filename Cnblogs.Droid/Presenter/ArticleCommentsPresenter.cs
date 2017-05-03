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

namespace Cnblogs.Droid.Presenter
{
    public class ArticleCommentsPresenter : IArticleCommentsPresenter
    {
        private IArticleCommentView commentView;
        private int pageSize = 10;
        public ArticleCommentsPresenter(IArticleCommentView commentView)
        {
            this.commentView = commentView;
        }

        public async Task GetComment(AccessToken token, string blogApp, int id, int pageIndex = 1)
        {
            try
            {
                var result = await OkHttpUtils.Instance(token).GetAsyn(string.Format(ApiUtils.ArticleComment, blogApp, id, pageIndex, pageSize));
                if (result.IsError)
                {
                    commentView.GetCommentFail(result.Message);
                }
                else
                {
                    var comments = JsonConvert.DeserializeObject<List<ArticleCommentModel>>(result.Message);
                    commentView.GetCommentSuccess(comments);
                }
            }
            catch (Exception ex)
            {
                commentView.GetCommentFail(ex.Message);
            }
        }

        public void PostComment(AccessToken token, string blogApp, int id, string content)
        {
            try
            {
                var url = string.Format(ApiUtils.ArticleCommentAdd, blogApp, id);
                OkHttpUtils.Instance(token).Post(url, content, async (call, response) =>
               {
                   var code = response.Code();
                   var body = await response.Body().StringAsync();
                   if (code == (int)System.Net.HttpStatusCode.OK)
                   {
                       var user = await SQLiteUtils.Instance().QueryUser();
                       ArticleCommentModel article = new ArticleCommentModel();
                       article.Author = user.DisplayName;
                       article.AuthorUrl = user.Avatar;
                       article.FaceUrl = user.Face;
                       article.Body = content;
                       article.DateAdded = DateTime.Now;
                       article.Floor = 0;
                       article.Id = 0;
                       commentView.PostCommentSuccess(article);
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
    }
}