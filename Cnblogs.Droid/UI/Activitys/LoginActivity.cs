using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Cnblogs.Droid.Model;
using Cnblogs.Droid.Presenter;
using Cnblogs.Droid.UI.Shareds;
using Cnblogs.Droid.UI.Views;
using Cnblogs.Droid.UI.Widgets;
using Cnblogs.Droid.Utils;
using Square.OkHttp3;
using System;
using System.Text;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Cnblogs.Droid.UI.Activitys
{
    [Activity(Label = "@string/login")]
    public class LoginActivity : BaseActivity, View.IOnClickListener, ILoginView
    {
        private Toolbar toolbar;
        private Handler handler;
        private EditText editAccount;
        private EditText editPassword;
        private Button btnLogin;
        private ILoginPresenter loginPresenter;
        private ProgressDialog dialog;
        protected override int LayoutResource => Resource.Layout.login;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            handler = new Handler();
            loginPresenter = new LoginPresenter(this);
            StatusBarCompat.SetOrdinaryToolBar(this);

            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            toolbar.SetNavigationIcon(Resource.Drawable.back_24dp);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            toolbar.SetNavigationOnClickListener(this);

            editAccount = FindViewById<EditText>(Resource.Id.account);
            editPassword = FindViewById<EditText>(Resource.Id.password);
            btnLogin = FindViewById<Button>(Resource.Id.login);
            btnLogin.SetOnClickListener(this);
            dialog = new ProgressDialog(this);
            dialog.SetCancelable(false);
        }
        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.login:
                    var account = editAccount.Text;
                    var password = editPassword.Text;
                    if (account.Trim() == "")
                    {
                        Toast.MakeText(this, Resources.GetString(Resource.String.need_account), ToastLength.Short).Show();
                        return;
                    }
                    if (password.Trim() == "")
                    {
                        Toast.MakeText(this, Resources.GetString(Resource.String.need_password), ToastLength.Short).Show();
                        return;
                    }
                    var publicKey = Resources.GetString(Resource.String.PublicKey);
                    if (publicKey == "")
                    {
                        Toast.MakeText(this, Resources.GetString(Resource.String.publicKey_error), ToastLength.Short).Show();
                        return;
                    }
                    RSAUtils rsaUtils = new RSAUtils(publicKey);
                    dialog.SetMessage(Resources.GetString(Resource.String.logining));
                    dialog.Show();

                    var basic = Square.OkHttp3.Credentials.Basic(Resources.GetString(Resource.String.ClientId), Resources.GetString(Resource.String.ClientSercret));

                    loginPresenter.Login(TokenShared.GetAccessToken(this), basic, rsaUtils.Encrypt(account), rsaUtils.Encrypt(password));
                    break;
                default:
                    InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                    imm.HideSoftInputFromWindow(editPassword.WindowToken, 0);
                    SetResult(Result.Canceled);
                    ActivityCompat.FinishAfterTransition(this);
                    break;
            }
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
    }
}