using Cnblogs.Droid.Model;
using Cnblogs.Droid.UI.Views;
using Cnblogs.Droid.Utils;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Cnblogs.Droid.Presenter
{
    public class StatusPresenter : IStatusPresenter
    {
        private IStatusView statusView;
        public StatusPresenter(IStatusView statusView)
        {
            this.statusView = statusView;
        }
        public async Task GetServiceStatus(AccessToken token, int id)
        {
            try
            {
                var result = await OkHttpUtils.Instance(token).GetAsyn(string.Format(ApiUtils.StatusBody, id));
                if (result.IsError)
                {
                    statusView.GetServiceStatusFail(result.Message);
                }
                else
                {
                    var status = JsonConvert.DeserializeObject<StatusModel>(result.Message);
                    await SQLiteUtils.Instance().UpdateStatus(status);
                    statusView.GetServiceStatusSuccess(status);
                }

            }
            catch (Exception e)
            {
                statusView.GetServiceStatusFail(e.Message);
            }
        }
        public async Task GetClientStatus(int id)
        {
            statusView.GetClientStatusSuccess(await SQLiteUtils.Instance().QueryStatus(id));
        }
    }
}