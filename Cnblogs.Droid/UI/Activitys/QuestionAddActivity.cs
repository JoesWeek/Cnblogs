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
    [Activity(Label = "@string/question_add", LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
    public class QuestionAddActivity : BaseActivity, IQuestionAddView, View.IOnClickListener, Toolbar.IOnMenuItemClickListener
    {
        private int questionId;
        private Handler handler;
        private IQuestionAddPresenter questionPresenter;
        private Toolbar toolbar;
        private ProgressDialog dialog;
        private EditText txtTitle;
        private EditText txtTags;
        private EditText txtContent;
        protected override int LayoutResource => Resource.Layout.question_add;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            questionId = Intent.GetIntExtra("id", 0);
            questionPresenter = new QuestionAddPresenter(this);
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

            txtTitle = FindViewById<EditText>(Resource.Id.txtTitle);
            txtTags = FindViewById<EditText>(Resource.Id.txtTags);
            txtContent = FindViewById<EditText>(Resource.Id.txtContent);
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            this.MenuInflater.Inflate(Resource.Menu.send, menu);
            return true;
        }
        public bool OnMenuItemClick(IMenuItem item)
        {
            PostQuestion();
            return true;
        }
        public async void PostQuestion()
        {
            //是否登录
            var user = UserShared.GetAccessToken(this);
            if (user.access_token == "" || user.RefreshTime.AddSeconds(user.expires_in) < DateTime.Now)
            {
                //未登录或清空Token失效
                //清空Token
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
                var title = txtTitle.Text;
                var tags = txtTags.Text;
                var content = txtContent.Text;
                if (title.Length == 0)
                {
                    Toast.MakeText(this, "请输入标题", ToastLength.Short).Show();
                }
                else if (title.Length < 3)
                {
                    Toast.MakeText(this, "标题的内容太少了,至少3个字吧", ToastLength.Short).Show();
                }
                else if (content.Length == 0)
                {
                    Toast.MakeText(this, "请输入提问内容", ToastLength.Short).Show();
                }
                else if (content.Length < 3)
                {
                    Toast.MakeText(this, "提问内容太少了,至少3个字吧", ToastLength.Short).Show();
                }
                else
                {
                    dialog.SetMessage(Resources.GetString(Resource.String.addstatusing));
                    dialog.Show();
                    questionPresenter.QuestionAdd(user, title, content, tags, 1);
                }
            }
        }
        public void QuestionAddFail(string msg)
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

        public void QuestionAddSuccess(QuestionsModel model)
        {
            handler.Post(() =>
            {
                if (dialog.IsShowing)
                {
                    dialog.Dismiss();
                }
                txtTitle.Text = "";
                txtTags.Text = "";
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
                PostQuestion();
            }
        }
        public void OnClick(View v)
        {
            this.Finish();
        }
    }
}