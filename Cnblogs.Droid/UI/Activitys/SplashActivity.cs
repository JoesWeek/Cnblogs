using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.App;
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
        protected override int LayoutResource => Resource.Layout.splash;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetTheme(Resource.Style.AppTheme);
            splashPresenter = new SplashPresenter(this, this);
            var ClientId = Resources.GetString(Resource.String.ClientId);
            var ClientSercret = Resources.GetString(Resource.String.ClientSercret);
            if (ClientId == "" || ClientSercret == "")
            {
                Toast.MakeText(this, Resources.GetString(Resource.String.client_error), ToastLength.Long).Show();
            }
            else
            {
                var basic = Square.OkHttp3.Credentials.Basic(Resources.GetString(Resource.String.ClientId), Resources.GetString(Resource.String.ClientSercret));
                splashPresenter.GetAccessToken(TokenShared.GetAccessToken(this), basic);
            }
            RunOnUiThread(() =>
            {
                new Handler().PostDelayed(() =>
                {
                    MainActivity.Start(this);
                    ActivityCompat.FinishAfterTransition(this);
                }, 3000);
            });
        }
    }
}