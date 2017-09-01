using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
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
    [Activity(Label = "@string/question_answer_add", LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
    public class QuestionAnswersAddActivity : BaseActivity, IQuestionAnswersAddView, View.IOnClickListener, Toolbar.IOnMenuItemClickListener
    {
        private int questionId;
        private Handler handler;
        private IQuestionAnswersAddPresenter answersPresenter;
        private Toolbar toolbar;
        private ProgressDialog dialog;
        private EditText txtContent;
        protected override int LayoutResource => Resource.Layout.question_answers_add;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            questionId = Intent.GetIntExtra("id", 0);
            answersPresenter = new QuestionAnswersAddPresenter(this);
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
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            this.MenuInflater.Inflate(Resource.Menu.send, menu);
            return true;
        }
        public bool OnMenuItemClick(IMenuItem item)
        {
            PostAnswers();
            return true;
        }
        public void PostAnswers()
        {
            //ÊÇ·ñµÇÂ¼
            var user = UserShared.GetAccessToken(this);
            if (user.access_token == "" || user.RefreshTime.AddSeconds(user.expires_in) < DateTime.Now)
            {
                ShowLogin();
            }
            else
            {
                var content = txtContent.Text;
                if (content.Length > 0)
                {
                    dialog.SetMessage(Resources.GetString(Resource.String.addstatusing));
                    dialog.Show();
                    answersPresenter.CheckAnswersByUser(user, questionId);
                }
                else
                {
                    Toast.MakeText(this, Resources.GetString(Resource.String.addstatus_tip), ToastLength.Short).Show();
                }
            }
        }
        public void AnswersAddFail(string msg)
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
        public void AnswersAddSuccess(QuestionAnswersModel model)
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
                ActivityCompat.FinishAfterTransition(this);
            });
        }
        public void CheckAnswersUserFail(string msg)
        {
            handler.Post(() =>
            {
                if (dialog.IsShowing)
                {
                    dialog.Dismiss();
                }
                if (msg == null)
                {
                    Toast.MakeText(this, Resources.GetString(Resource.String.question_answer_add_fail), ToastLength.Short).Show();
                }
                else
                {
                    Toast.MakeText(this, msg, ToastLength.Short).Show();
                }
            });
        }
        public void CheckAnswersUserSuccess()
        {
            handler.Post(() =>
            {
                var content = txtContent.Text;
                answersPresenter.AnswersAdd(UserShared.GetAccessToken(this), questionId, content);
            });
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == (int)RequestCode.LoginCode && resultCode == Result.Ok)
            {
                PostAnswers();
            }
        }
        public async void ShowLogin()
        {
            //Î´µÇÂ¼»òÇå¿ÕTokenÊ§Ð§
            //Çå¿ÕToken
            UserShared.Update(this, new AccessToken());
            await SQLiteUtils.Instance().DeleteUserAll();
            Android.Support.V7.App.AlertDialog.Builder dialog = new Android.Support.V7.App.AlertDialog.Builder(this);
            dialog.SetMessage(Resources.GetString(Resource.String.need_login_tip));
            dialog.SetPositiveButton(Resources.GetString(Resource.String.confirm), delegate
            {
                StartActivityForResult(new Intent(this, typeof(AuthorizeActivity)), (int)RequestCode.LoginCode);
                dialog.Dispose();
            });
            dialog.SetNegativeButton(Resources.GetString(Resource.String.cancel), delegate
            {
                dialog.Dispose();
            });
            dialog.Create().Show();
        }
        public void OnClick(View v)
        {
            ActivityCompat.FinishAfterTransition(this);
        }
    }
}