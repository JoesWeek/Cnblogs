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
using Android.Webkit;
using Square.OkHttp3;
using Cnblogs.Droid.UI.Shareds;
using Cnblogs.Droid.Utils;
using Cnblogs.Droid.UI.Widgets;
using Cnblogs.Droid.Presenter;
using Cnblogs.Droid.UI.Views;
using Cnblogs.Droid.Model;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V4.App;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Cnblogs.Droid.UI.Activitys
{
    [Activity(Label = "@string/login")]
    public class AuthorizeActivity : BaseActivity, LoginWebViewClient.IOnLoginListener, View.IOnClickListener, ILoginView
    {
        protected override int LayoutResource => Resource.Layout.authorize;
        private Toolbar toolbar;
        private Handler handler;
        private WebView loginView;
        private ProgressBar progressBar;
        private ProgressDialog dialog;
        private ILoginPresenter loginPresenter;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            StatusBarCompat.SetOrdinaryToolBar(this);
            handler = new Handler();
            loginPresenter = new LoginPresenter(this);
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            toolbar.SetNavigationIcon(Resource.Drawable.back_24dp);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            toolbar.SetNavigationOnClickListener(this);

            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
            progressBar.Max = 100;
            dialog = new ProgressDialog(this);
            dialog.SetCancelable(false);
            dialog.SetMessage("正在获取token");

            loginView = FindViewById<WebView>(Resource.Id.loginView);
            loginView.Settings.JavaScriptEnabled = true;
            loginView.Settings.SetSupportZoom(true);
            loginView.Settings.BuiltInZoomControls = true;
            loginView.SetWebChromeClient(new LoginWebChromeClient(progressBar));
            loginView.SetWebViewClient(new LoginWebViewClient(this));
            loginView.LoadUrl(string.Format(ApiUtils.Authorize, Resources.GetString(Resource.String.ClientId)) + new Random().Next(1000, 9999));
        }

        public void OnLogin(string code)
        {
            dialog.Show();

            var cientId = Resources.GetString(Resource.String.ClientId);
            var clientSercret = Resources.GetString(Resource.String.ClientSercret);
            var grant_type = "authorization_code";
            var redirect_uri = "https://oauth.cnblogs.com/auth/callback";

            var content = string.Format("client_id={0}&client_secret={1}&grant_type={2}&redirect_uri={3}&code={4}", cientId, clientSercret, grant_type, redirect_uri, code);
            loginPresenter.Login(TokenShared.GetAccessToken(this), content);
        }

        public void LoginFail(string msg)
        {
            handler.Post(() =>
            {
                Toast.MakeText(this, msg, ToastLength.Short).Show();
                if (dialog.IsShowing)
                {
                    dialog.Dismiss();
                }
            });
        }

        public void LoginSuccess(AccessToken token, UserModel user)
        {
            handler.Post(() =>
            {
                if (dialog.IsShowing)
                {
                    dialog.Dismiss();
                }
                if (token.access_token != null && token.access_token != "" && user != null && user.UserId != null)
                {
                    UserShared.Update(this, token);
                    Toast.MakeText(this, Resources.GetString(Resource.String.login_success), ToastLength.Short).Show();
                    SetResult(Result.Ok);
                    this.Finish();
                }
                else
                {
                    Toast.MakeText(this, Resources.GetString(Resource.String.load_failed), ToastLength.Short).Show();
                }
            });
        }

        public void OnClick(View v)
        {
            SetResult(Result.Canceled);
            ActivityCompat.FinishAfterTransition(this);
        }
    }
    public class LoginWebChromeClient : WebChromeClient
    {
        private ProgressBar progressBar;
        public LoginWebChromeClient(ProgressBar progressBar)
        {
            this.progressBar = progressBar;
        }
        public override void OnProgressChanged(WebView view, int newProgress)
        {
            progressBar.Progress = newProgress;
            if (newProgress < 100)
            {
                if (progressBar.Visibility == ViewStates.Gone)
                    progressBar.Visibility = ViewStates.Visible;
            }
            else
            {
                progressBar.Visibility = ViewStates.Gone;
            }
            base.OnProgressChanged(view, newProgress);
        }
    }
    public class LoginWebViewClient : WebViewClient
    {
        public IOnLoginListener OnLoginListener { get; set; }
        public LoginWebViewClient(IOnLoginListener OnLoginListener)
        {
            this.OnLoginListener = OnLoginListener;
        }
        public override WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request)
        {
            return base.ShouldInterceptRequest(view, request);
        }
        [Obsolete]
        public override bool ShouldOverrideUrlLoading(WebView view, string url)
        {
            if (url.IndexOf("https://oauth.cnblogs.com/auth/callback") > -1)
            {
                Uri uri = new Uri(url.Replace("#", "?"));
                var query = uri.Query.TrimStart('?').Split('&');
                foreach (var item in query)
                {
                    var q = item.Split('=');
                    if (q[0] == "code")
                    {
                        var code = q[1];
                        if (OnLoginListener != null)
                        {
                            OnLoginListener.OnLogin(code);
                        }
                    }
                }
                view.StopLoading();
            }
            return base.ShouldOverrideUrlLoading(view, url);
        }
        public interface IOnLoginListener
        {
            void OnLogin(string code);
        }
    }
}