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
    public class StatusCommentsPresenter : IStatusCommentsPresenter
    {
        private IStatusCommentsView statusView;
        public StatusCommentsPresenter(IStatusCommentsView statusView)
        {
            this.statusView = statusView;
        }
        public async Task GetServiceComments(AccessToken token, int id)
        {
            try
            {
                var result = await OkHttpUtils.Instance(token).GetAsyn(string.Format(ApiUtils.StatusComments, id));
                if (result.IsError)
                {
                    statusView.GetServiceCommentsFail(result.Message);
                }
                else
                {
                    var comments = JsonConvert.DeserializeObject<List<StatusCommentsModel>>(result.Message);
                    statusView.GetServiceCommentsSuccess(comments);
                }
            }
            catch (Exception ex)
            {
                statusView.GetServiceCommentsFail(ex.Message);
            }
        }

        public void PostComment(AccessToken token, int id, string content)
        {
            try
            {
                var url = string.Format(ApiUtils.StatusCommentAdd, id.ToString());

                var param = new List<OkHttpUtils.Param>()
                {
                    new OkHttpUtils.Param("ReplyTo","0") ,
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
                        StatusCommentsModel news = new StatusCommentsModel();
                        news.UserDisplayName = user.DisplayName;
                        news.UserIconUrl = user.Face;
                        news.Content = content;
                        news.DateAdded = DateTime.Now;
                        news.StatusId = id;
                        news.UserGuid = user.UserId;
                        statusView.PostCommentSuccess(news);
                    }
                    else
                    {
                        try
                        {
                            var error = JsonConvert.DeserializeObject<ErrorMessage>(body);
                            statusView.PostCommentFail(error.Message);
                        }
                        catch (Exception e)
                        {
                            statusView.PostCommentFail(e.Message);
                        }
                    }
                }, (call, ex) =>
                {
                    statusView.PostCommentFail(ex.Message);
                });
            }
            catch (Exception e)
            {
                statusView.PostCommentFail(e.Message);
            }
        }
        public void DeleteComment(AccessToken token, int statusId, int id)
        {
            try
            {
                var url = string.Format(ApiUtils.StatusCommentDelete, statusId, id);

                OkHttpUtils.Instance(token).Delete(url, async (call, response) =>
                {
                    var code = response.Code();
                    var body = await response.Body().StringAsync();
                    if (code == (int)System.Net.HttpStatusCode.OK)
                    {
                        statusView.DeleteCommentSuccess(id);
                    }
                    else
                    {
                        try
                        {
                            var error = JsonConvert.DeserializeObject<ErrorMessage>(body);
                            statusView.DeleteCommentFail(id, error.Message);
                        }
                        catch (Exception e)
                        {
                            statusView.DeleteCommentFail(id, e.Message);
                        }
                    }
                }, (call, ex) =>
                {
                    statusView.DeleteCommentFail(id, ex.Message);
                });
            }
            catch (Exception ex)
            {
                statusView.DeleteCommentFail(id, ex.Message);
            }
        }
    }
}