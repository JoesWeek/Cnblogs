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
    public class StatusColumnPresenter : IStatusColumnPresenter
    {
        private IStatusColumnView statusView;
        private int pageSize = 10;
        public StatusColumnPresenter(IStatusColumnView statusView)
        {
            this.statusView = statusView;
        }
        public async Task GetServiceStatus(AccessToken token, int position, int pageIndex = 1)
        {
            try
            {
                string statusType = "all";
                switch (position)
                {
                    case 0:
                        statusType = "all";
                        break;
                    case 1:
                        statusType = "following";
                        break;
                    case 2:
                        statusType = "my";
                        break;
                    case 3:
                        statusType = "mycomment";
                        break;
                    case 4:
                        statusType = "recentcomment";
                        break;
                    case 5:
                        statusType = "mention";
                        break;
                    case 6:
                        statusType = "comment";
                        break;
                    default:
                        statusType = "all";
                        break;
                }
                var url = string.Format(ApiUtils.Status, statusType, pageIndex, pageSize);
                var result = await OkHttpUtils.Instance(token).GetAsyn(url);
                if (result.IsError)
                {
                    statusView.GetServiceStatusFail(result.Message);
                    return;
                }
                else
                {
                    var statuses = JsonConvert.DeserializeObject<List<StatusModel>>(result.Message);
                    if (position == 0)
                        await SQLiteUtils.Instance().UpdateStatuses(statuses);
                    statusView.GetServiceStatusSuccess(statuses);
                }
            }
            catch (Exception ex)
            {
                statusView.GetServiceStatusFail(ex.Message);
            }
        }
        public async Task GetClientStatus(int position)
        {
            List<StatusModel> list = new List<StatusModel>();
            switch (position)
            {
                case 0:
                    list = await SQLiteUtils.Instance().QueryStatuses(pageSize);
                    break;
                case 1:
                    break;
            }
            statusView.GetClientStatusSuccess(list);
        }
        public void DeleteStatus(AccessToken token, int id)
        {
            try
            {
                var url = string.Format(ApiUtils.StatusDelete, id);

                OkHttpUtils.Instance(token).Delete(url, async (call, response) =>
                {
                    var code = response.Code();
                    var body = await response.Body().StringAsync();
                    if (code == (int)System.Net.HttpStatusCode.OK)
                    {
                        statusView.DeleteStatusSuccess(id);
                    }
                    else
                    {
                        try
                        {
                            var error = JsonConvert.DeserializeObject<ErrorMessage>(body);
                            statusView.DeleteStatusFail(id, error.Message);
                        }
                        catch (Exception e)
                        {
                            statusView.DeleteStatusFail(id, e.Message);
                        }
                    }
                }, (call, ex) =>
                {
                    statusView.DeleteStatusFail(id, ex.Message);
                });
            }
            catch (Exception ex)
            {
                statusView.DeleteStatusFail(id, ex.Message);
            }
        }
    }
}