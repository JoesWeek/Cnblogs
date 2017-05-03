using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Cnblogs.Droid.Model;
using Cnblogs.Droid.Presenter;
using Cnblogs.Droid.UI.Shareds;
using Cnblogs.Droid.UI.Views;
using Cnblogs.Droid.UI.Widgets;
using Cnblogs.Droid.Utils;
using System;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Cnblogs.Droid.UI.Activitys
{
    [Activity(Label = "@string/addstatus", LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
    public class StatusAddActivity : BaseActivity, IStatusAddView, View.IOnClickListener, Toolbar.IOnMenuItemClickListener
    {
        private Handler handler;
        private IStatusAddPresenter statusPresenter;
        private Toolbar toolbar;
        private ProgressDialog dialog;
        private EditText txtContent;
        private CheckBox checkIsPrivate;
        protected override int LayoutResource => Resource.Layout.status_add;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            statusPresenter = new StatusAddPresenter(this);
            handler = new Handler();
            dialog = new ProgressDialog(this);
            dialog.SetCancelable(false);
            StatusBarCompat.SetOrdinaryToolBar(this);
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            toolbar.SetNavigationIcon(Resource.Drawable.back_24dp);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            toolbar.SetNavigationOnClickListener(this);
            toolbar.SetOnMenuItemClickListener(this);

            txtContent = FindViewById<EditText>(Resource.Id.txtContent);
            checkIsPrivate = FindViewById<CheckBox>(Resource.Id.checkIsPrivate);
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            this.MenuInflater.Inflate(Resource.Menu.send, menu);
            return true;
        }
        public bool OnMenuItemClick(IMenuItem item)
        {
            PostComment();
            return true;
        }
        public async void PostComment()
        {
            //ÊÇ·ñµÇÂ¼
            var user = UserShared.GetAccessToken(this);
            if (user.access_token == "" || user.RefreshTime.AddSeconds(user.expires_in) < DateTime.Now)
            {
                //Î´µÇÂ¼»òÇå¿ÕTokenÊ§Ð§
                //Çå¿ÕToken
                UserShared.Update(this, new AccessToken());
                await SQLiteUtils.Instance().DeleteUserAll();
                Android.Support.V7.App.AlertDialog.Builder dialog = new Android.Support.V7.App.AlertDialog.Builder(this);
                dialog.SetMessage(Resources.GetString(Resource.String.need_login_tip));
                dialog.SetPositiveButton(Resources.GetString(Resource.String.confirm), delegate
                {
                    StartActivityForResult(new Intent(this, typeof(LoginActivity)), (int)RequestCode.LoginCode);
                    dialog.Dispose();
                });
                dialog.SetNegativeButton(Resources.GetString(Resource.String.cancel), delegate
                {
                    dialog.Dispose();
                });
                dialog.Create().Show();
            }
            else
            {
                var content = txtContent.Text;
                if (content.Length > 0)
                {
                    dialog.SetMessage(Resources.GetString(Resource.String.addstatusing));
                    dialog.Show();
                    statusPresenter.StatusAdd(user, content, checkIsPrivate.Checked);
                }
                else
                {
                    Toast.MakeText(this, Resources.GetString(Resource.String.addstatus_tip), ToastLength.Short).Show();
                }
            }
        }
        public void StatusAddFail(string msg)
        {
            handler.Post(() =>
            {
                if (dialog.IsShowing)
                {
                    dialog.Dismiss();
                }
                if (msg == null)
                {
                    Toast.MakeText(this, Resources.GetString(Resource.String.addstatus_fail), ToastLength.Short).Show();
                }
                else
                {
                    Toast.MakeText(this, msg, ToastLength.Short).Show();
                }
            });
        }

        public void StatusAddSuccess(StatusModel model)
        {
            handler.Post(() =>
            {
                if (dialog.IsShowing)
                {
                    dialog.Dismiss();
                }
                txtContent.Text = "";
                Toast.MakeText(this, Resources.GetString(Resource.String.addstatus_success), ToastLength.Short).Show();

                SetResult(Result.Ok);
                this.Finish();
            });
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == (int)RequestCode.LoginCode && resultCode == Result.Ok)
            {
                PostComment();
            }
        }
        public void OnClick(View v)
        {
            this.Finish();
        }

    }
}