using Cnblogs.Droid.Model;
using Cnblogs.Droid.UI.Views;
using Cnblogs.Droid.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cnblogs.Droid.Presenter
{
    public class StatusAddPresenter : IStatusAddPresenter
    {
        private IStatusAddView statusView;
        public StatusAddPresenter(IStatusAddView statusView)
        {
            this.statusView = statusView;
        }
        public void StatusAdd(AccessToken token, string content, bool isPrivate)
        {
            try
            {
                var url = ApiUtils.StatusADD;
                
                var param = new List<OkHttpUtils.Param>()
                {
                    new OkHttpUtils.Param("IsPrivate",isPrivate.ToString()) ,
                    new OkHttpUtils.Param("Content",content) ,
                };

                OkHttpUtils.Instance(token).Post(url, param, async (call, response) =>
                {
                    var code = response.Code();
                    var body = await response.Body().StringAsync();
                    if (code == (int)System.Net.HttpStatusCode.OK)
                    {
                        var user = await SQLiteUtils.Instance().QueryUser();
                        StatusModel status = new StatusModel();
                        status.UserDisplayName = user.DisplayName;
                        status.UserIconUrl = user.Face;
                        status.Content = content;
                        status.IsPrivate = isPrivate;
                        status.DateAdded = DateTime.Now;
                        status.UserGuid = user.UserId;
                        statusView.StatusAddSuccess(status);
                    }
                    else
                    {
                        try
                        {
                            var error = JsonConvert.DeserializeObject<ErrorMessage>(body);
                            statusView.StatusAddFail(error.Message);
                        }
                        catch (Exception e)
                        {
                            statusView.StatusAddFail(e.Message);
                        }
                    }
                }, (call, ex) =>
                {
                    statusView.StatusAddFail(ex.Message);
                });
            }
            catch (Exception e)
            {
                statusView.StatusAddFail(e.Message);
            }
        }
    }
}