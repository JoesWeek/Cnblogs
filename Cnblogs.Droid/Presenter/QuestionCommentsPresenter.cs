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
using System.Net.Http;

namespace Cnblogs.Droid.Presenter
{
    public class QuestionCommentsPresenter : IQuestionCommentsPresenter
    {
        private IQuestionCommentsView commentsView;
        public QuestionCommentsPresenter(IQuestionCommentsView statusView)
        {
            this.commentsView = statusView;
        }
        public async Task GetServiceComments(AccessToken token, int id)
        {
            try
            {
                var result = await OkHttpUtils.Instance(token).GetAsyn(string.Format(ApiUtils.QuestionsAnswerComments, id));
                if (result.IsError)
                {
                    commentsView.GetCommentsFail(result.Message);
                }
                else
                {
                    var comments = JsonConvert.DeserializeObject<List<QuestionCommentsModel>>(result.Message);
                    commentsView.GetCommentsSuccess(comments);
                }
            }
            catch (Exception ex)
            {
                commentsView.GetCommentsFail(ex.Message);
            }
        }
        public void PostComment(AccessToken token, int questionId, int answerId, string content)
        {
            try
            {
                var url = string.Format(ApiUtils.QuestionsAnswerCommentsAdd, questionId, answerId.ToString());

                var param = new List<OkHttpUtils.Param>()
                {
                    new OkHttpUtils.Param("ParentCommentId","0") ,
                    new OkHttpUtils.Param("Content",content) ,
                };

                OkHttpUtils.Instance(token).Post(url, param, async (call, response) =>
                {
                    var code = response.Code();
                    var body = await response.Body().StringAsync();
                    if (code == (int)System.Net.HttpStatusCode.OK)
                    {
                        var user = await SQLiteUtils.Instance().QueryUser();
                        QuestionCommentsModel news = new QuestionCommentsModel();
                        news.PostUserInfo = new QuestionUserInfoModel()
                        {
                            UserID = user.SpaceUserId,
                            IconName = user.Face,
                            UCUserID = user.UserId,
                            UserName = user.DisplayName,
                            QScore = user.Score
                        };
                        news.Content = content;
                        news.DateAdded = DateTime.Now;
                        news.CommentID = answerId;
                        commentsView.PostCommentSuccess(news);
                    }
                    else
                    {
                        try
                        {
                            var error = JsonConvert.DeserializeObject<ErrorMessage>(body);
                            commentsView.PostCommentFail(error.Message);
                        }
                        catch (Exception e)
                        {
                            commentsView.PostCommentFail(e.Message);
                        }
                    }
                }, (call, ex) =>
                {
                    commentsView.PostCommentFail(ex.Message);
                });
            }
            catch (Exception e)
            {
                commentsView.PostCommentFail(e.Message);
            }
        }
        public void DeleteComment(AccessToken token, int questionId, int answerId, int commentId)
        {
            try
            {
                var url = string.Format(ApiUtils.QuestionsAnswerCommentsDelete, questionId, answerId, commentId);

                OkHttpUtils.Instance(token).Delete(url, async (call, response) =>
                {
                    var code = response.Code();
                    var body = await response.Body().StringAsync();
                    if (code == (int)System.Net.HttpStatusCode.OK)
                    {
                        commentsView.DeleteCommentSuccess(commentId);
                    }
                    else
                    {
                        try
                        {
                            var error = JsonConvert.DeserializeObject<ErrorMessage>(body);
                            commentsView.DeleteCommentFail(commentId, error.Message);
                        }
                        catch (Exception e)
                        {
                            commentsView.DeleteCommentFail(commentId, e.Message);
                        }
                    }
                }, (call, ex) =>
                {
                    commentsView.DeleteCommentFail(commentId, ex.Message);
                });
            }
            catch (Exception ex)
            {
                commentsView.DeleteCommentFail(commentId, ex.Message);
            }
        }
    }
}