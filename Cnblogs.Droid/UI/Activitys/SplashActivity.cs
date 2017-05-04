using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using Cnblogs.Droid.Model;
using Cnblogs.Droid.Presenter;
using Cnblogs.Droid.UI.Shareds;
using Cnblogs.Droid.UI.Views;
using Cnblogs.Droid.Utils;
using System;
using System.Text;

namespace Cnblogs.Droid.UI.Activitys
{
    [Activity(MainLauncher = true, LaunchMode = LaunchMode.SingleTop, Theme = "@style/LaunchStyle")]
    public class SplashActivity : BaseActivity, ISplashView
    {
        private ISplashPresenter splashPresenter;
        private string ClientId;
        private string ClientSercret;
        protected override int LayoutResource => Resource.Layout.splash;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetTheme(Resource.Style.AppTheme);
            splashPresenter = new SplashPresenter(this);
            ClientId = Resources.GetString(Resource.String.ClientId);
            ClientSercret = Resources.GetString(Resource.String.ClientSercret);
            if (ClientId == "" || ClientSercret == "")
            {
                Toast.MakeText(this, Resources.GetString(Resource.String.client_error), ToastLength.Long).Show();
            }
        }
        protected override async void OnResume()
        {
            base.OnResume();
            if (ClientId != "" && ClientSercret != "")
            {
                var token = TokenShared.GetAccessToken(this);
                if (token.access_token == "" || token.RefreshTime.AddSeconds(token.expires_in) < DateTime.Now)
                {
                    var basic = Square.OkHttp3.Credentials.Basic(Resources.GetString(Resource.String.ClientId), Resources.GetString(Resource.String.ClientSercret));
                    splashPresenter.GetAccessToken(TokenShared.GetAccessToken(this), basic);
                }
                else
                {
                    var user = UserShared.GetAccessToken(this);
                    if (user.access_token != "" && user.RefreshTime.AddSeconds(user.expires_in) < DateTime.Now)
                    {
                        Toast.MakeText(this, Resources.GetString(Resource.String.access_token_out_of_date), ToastLength.Long).Show();
                    }
                    if (user.access_token == "" || user.RefreshTime.AddSeconds(user.expires_in) < DateTime.Now)
                    {
                        UserShared.Update(this, new AccessToken());
                        await SQLiteUtils.Instance().DeleteUserAll();
                    }
                    StartMain();
                }
            }
        }
        public void GetAccessTokenFail(string msg)
        {
            Toast.MakeText(this, msg, ToastLength.Long).Show();
            StartMain();
        }
        public void GetAccessTokenSuccess(AccessToken token)
        {
            TokenShared.Update(this, token);
            StartMain();
        }
        public void StartMain()
        {
            MainActivity.Start(this);
            this.Finish();
        }
    }
}