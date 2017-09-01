using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Cnblogs.Droid.Model;
using Cnblogs.Droid.Presenter;
using Cnblogs.Droid.UI.Adapters;
using Cnblogs.Droid.UI.Listeners;
using Cnblogs.Droid.UI.Shareds;
using Cnblogs.Droid.UI.Views;
using Cnblogs.Droid.UI.Widgets;
using Cnblogs.Droid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Cnblogs.Droid.UI.Activitys
{
    [Activity(Label = "@string/question_answer")]
    public class QuestionAnswersActivity : BaseActivity, View.IOnClickListener, IQuestionAnswersView, BaseAdapter.IOnLoadMoreListener, SwipeRefreshLayout.IOnRefreshListener, IOnDeleteClickListener, Toolbar.IOnMenuItemClickListener
    {
        private int Id;
        private Handler handler;
        private IQuestionAnswersPresenter answersPresenter;
        private ProgressDialog dialog;

        private Toolbar toolbar;
        private SwipeRefreshLayout swipeRefreshLayout;
        private RecyclerView recyclerView;
        private QuestionAnswersAdapter adapter;
        private View notDataView;
        private View errorView;
        private int pageIndex = 1;
        protected override int LayoutResource => Resource.Layout.question_answers;
        public static void Start(Context context, int id)
        {
            Intent intent = new Intent(context, typeof(QuestionAnswersActivity));
            intent.PutExtra("id", id);
            context.StartActivity(intent);
        }
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Id = Intent.GetIntExtra("id", 0);
            handler = new Handler();
            answersPresenter = new QuestionAnswersPresenter(this);
            StatusBarCompat.SetOrdinaryToolBar(this);
            dialog = new ProgressDialog(this);
            dialog.SetCancelable(false);

            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            toolbar.SetNavigationIcon(Resource.Drawable.back_24dp);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            toolbar.SetNavigationOnClickListener(this);
            toolbar.SetOnMenuItemClickListener(this);

            swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            swipeRefreshLayout.SetColorSchemeResources(Resource.Color.primary);
            swipeRefreshLayout.SetOnRefreshListener(this);

            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            recyclerView.SetLayoutManager(new LinearLayoutManager(this));
            adapter = new QuestionAnswersAdapter();
            adapter.SetOnLoadMoreListener(this);
            adapter.User = await SQLiteUtils.Instance().QueryUser();
            adapter.OnDeleteClickListener = this;

            notDataView = this.LayoutInflater.Inflate(Resource.Layout.empty_view, (ViewGroup)recyclerView.Parent, false);
            notDataView.Click += delegate (object sender, EventArgs e)
            {
                OnRefresh();
            };
            errorView = this.LayoutInflater.Inflate(Resource.Layout.error_view, (ViewGroup)recyclerView.Parent, false);
            errorView.Click += delegate (object sender, EventArgs e)
            {
                OnRefresh();
            };
            recyclerView.SetAdapter(adapter);
            recyclerView.Post(() =>
            {
                swipeRefreshLayout.Refreshing = true;
                OnRefresh();
            });
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            this.MenuInflater.Inflate(Resource.Menu.addstatus, menu);
            return true;
        }
        public bool OnMenuItemClick(IMenuItem item)
        {
            //ÊÇ·ñµÇÂ¼
            var user = UserShared.GetAccessToken(this);
            if (user.access_token == "" || user.RefreshTime.AddSeconds(user.expires_in) < DateTime.Now)
            {
                ShowLogin();
            }
            else
            {
                Intent intent = new Intent(this, typeof(QuestionAnswersAddActivity));
                intent.PutExtra("id", Id);
                StartActivityForResult(intent, (int)RequestCode.QuestionAnswersAddCode);
            }
            return true;
        }
        public async void OnRefresh()
        {
            if (pageIndex > 1)
                pageIndex = 1;
            await answersPresenter.GetAnswers(TokenShared.GetAccessToken(this), Id, pageIndex);
        }
        public async void OnLoadMoreRequested()
        {
            swipeRefreshLayout.Enabled = false;
            await answersPresenter.GetAnswers(TokenShared.GetAccessToken(this), Id, pageIndex);
        }
        public void GetAnswersFail(string msg)
        {
            recyclerView.Post(() =>
            {
                if (swipeRefreshLayout.Refreshing)
                {
                    swipeRefreshLayout.Refreshing = false;
                }
                if (!swipeRefreshLayout.Enabled)
                {
                    swipeRefreshLayout.Enabled = true;
                }
                if (adapter.GetData().Count() == 0)
                {
                    adapter.SetEmptyView(errorView);
                }
                else
                {
                    adapter.LoadMoreFail();
                }
                Toast.MakeText(this, msg, ToastLength.Short).Show();
            });
        }
        public void GetAnswersSuccess(List<QuestionAnswersModel> lists)
        {
            recyclerView.Post(() =>
            {
                if (swipeRefreshLayout.Refreshing)
                {
                    swipeRefreshLayout.Refreshing = false;
                }
                if (!swipeRefreshLayout.Enabled)
                {
                    swipeRefreshLayout.Enabled = true;
                }
                adapter.SetNewData(lists.OrderByDescending(q => q.IsBest).ToList());
                if (lists.Count > 0)
                {
                    adapter.LoadMoreEnd();
                }
                else
                {
                    adapter.SetEmptyView(notDataView);
                }
            });
        }
        protected async override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == (int)RequestCode.LoginCode && resultCode == Result.Ok)
            {
                adapter.User = await SQLiteUtils.Instance().QueryUser();
                adapter.NotifyDataSetChanged();
            }
            else if (requestCode == (int)RequestCode.QuestionAnswersAddCode && resultCode == Result.Ok)
            {
                OnRefresh();
            }
        }
        public void OnDelete(int answerId)
        {
            //ÊÇ·ñµÇÂ¼
            var user = UserShared.GetAccessToken(this);
            if (user.access_token == "" || user.RefreshTime.AddSeconds(user.expires_in) < DateTime.Now)
            {
                ShowLogin();
            }
            else
            {
                var item = adapter.GetData().Where(a => a.AnswerID == answerId).FirstOrDefault();
                var child = recyclerView.FindViewWithTag(item.AnswerID);
                child.FindViewById(Resource.Id.imgDelete).Visibility = ViewStates.Gone;
                child.FindViewById(Resource.Id.progressBar).Visibility = ViewStates.Visible;
                answersPresenter.DeleteAnswer(user, Id, item.AnswerID);
            }
        }
        public void DeleteAnswerFail(int answerId, string msg)
        {
            handler.Post(() =>
            {
                var child = recyclerView.FindViewWithTag(answerId);
                child.FindViewById(Resource.Id.imgDelete).Visibility = ViewStates.Visible;
                child.FindViewById(Resource.Id.progressBar).Visibility = ViewStates.Gone;
                Toast.MakeText(this, msg, ToastLength.Short).Show();
            });
        }
        public void DeleteAnswerSuccess(int answerId)
        {
            handler.Post(() =>
            {
                var child = recyclerView.FindViewWithTag(answerId);
                child.FindViewById(Resource.Id.imgDelete).Visibility = ViewStates.Visible;
                child.FindViewById(Resource.Id.progressBar).Visibility = ViewStates.Gone;

                var data = adapter.GetData();
                var index = data.IndexOf(data.Where(a => a.AnswerID == answerId).FirstOrDefault());
                adapter.Remove(index);
                if (data.Count == 0)
                {
                    adapter.SetEmptyView(notDataView);
                }
                Toast.MakeText(this, Resources.GetString(Resource.String.delete_success), ToastLength.Short).Show();
            });
        }
        public void OnClick(View v)
        {
            ActivityCompat.FinishAfterTransition(this);
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
    }
}