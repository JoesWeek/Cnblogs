using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.InputMethods;
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
    [Activity(Label = "@string/comment")]
    public class NewsCommentsActivity : BaseActivity, View.IOnClickListener, INewsCommentView, BaseAdapter.IOnLoadMoreListener, SwipeRefreshLayout.IOnRefreshListener, IOnDeleteClickListener
    {
        private int Id;
        private Handler handler;
        private INewsCommentsPresenter commentPresenter;

        private Toolbar toolbar;
        private SwipeRefreshLayout swipeRefreshLayout;
        private RecyclerView recyclerView;
        private NewsCommentsAdapter adapter;
        private View notDataView;
        private View errorView;
        private TextView txtContent;
        private TextView btnComment;
        private ProgressBar proLoading;
        private int pageIndex = 1;
        protected override int LayoutResource => Resource.Layout.news_comment;
        public static void Start(Context context, int id)
        {
            Intent intent = new Intent(context, typeof(NewsCommentsActivity));
            intent.PutExtra("id", id);
            context.StartActivity(intent);
        }

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Id = Intent.GetIntExtra("id", 0);
            handler = new Handler();
            commentPresenter = new NewsCommentsPresenter(this);
            StatusBarCompat.SetOrdinaryToolBar(this);

            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            toolbar.SetNavigationIcon(Resource.Drawable.back_24dp);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            toolbar.SetNavigationOnClickListener(this);

            txtContent = FindViewById<TextView>(Resource.Id.txtContent);
            proLoading = FindViewById<ProgressBar>(Resource.Id.proLoading);
            btnComment = FindViewById<TextView>(Resource.Id.btnComment);
            btnComment.SetOnClickListener(this);

            swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            swipeRefreshLayout.SetColorSchemeResources(Resource.Color.primary);
            swipeRefreshLayout.SetOnRefreshListener(this);

            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            recyclerView.SetLayoutManager(new LinearLayoutManager(this));
            adapter = new NewsCommentsAdapter();
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
        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.btnComment:
                    PostComment();
                    break;
                default:
                    InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                    imm.HideSoftInputFromWindow(txtContent.WindowToken, 0);
                    this.Finish();
                    break;
            }
        }
        public async void OnRefresh()
        {
            if (pageIndex > 1)
                pageIndex = 1;
            await commentPresenter.GetComment(TokenShared.GetAccessToken(this), Id, pageIndex);
        }
        public async void OnLoadMoreRequested()
        {
            swipeRefreshLayout.Enabled = false;
            await commentPresenter.GetComment(TokenShared.GetAccessToken(this), Id, pageIndex);
        }
        public void GetCommentFail(string msg)
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
                if (pageIndex > 1)
                {
                    adapter.LoadMoreFail();
                }
                else if (adapter.GetData().Count() == 0)
                {
                    adapter.SetEmptyView(errorView);
                }
                Toast.MakeText(this, msg, ToastLength.Short).Show();
            });
        }
        public void GetCommentSuccess(List<NewsCommentModel> lists)
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
                if (pageIndex == 1)
                {
                    if (lists.Count > 0)
                    {
                        adapter.SetNewData(lists);
                        if (lists.Count < 10)
                        {
                            adapter.LoadMoreEnd();
                        }
                        else
                        {
                            adapter.SetEnableLoadMore(true);
                            pageIndex++;
                        }
                    }
                    else if (adapter.GetData().Count() == 0)
                    {
                        adapter.SetEmptyView(notDataView);
                    }
                }
                else
                {
                    if (lists.Count > 0)
                    {
                        adapter.AddData(lists);
                        adapter.LoadMoreComplete();
                        pageIndex++;
                    }
                    else
                    {
                        adapter.LoadMoreEnd();
                    }
                }
            });
        }
        public void PostComment()
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
                if (content.Length == 0)
                {
                    Toast.MakeText(this, Resources.GetString(Resource.String.comment_tip), ToastLength.Short).Show();
                }
                else if (content.Length < 4)
                {
                    Toast.MakeText(this, Resources.GetString(Resource.String.comment_tip2), ToastLength.Short).Show();
                }
                else
                {
                    txtContent.Enabled = false;
                    proLoading.Visibility = ViewStates.Visible;
                    btnComment.Visibility = ViewStates.Gone;
                    commentPresenter.PostComment(user, Id, content);
                }
            }
        }
        public void PostCommentFail(string msg)
        {
            handler.Post(() =>
            {
                txtContent.Enabled = true;
                proLoading.Visibility = ViewStates.Gone;
                btnComment.Visibility = ViewStates.Visible;
                if (msg == null)
                {
                    Toast.MakeText(this, Resources.GetString(Resource.String.comment_fail), ToastLength.Short).Show();
                }
                else
                {
                    Toast.MakeText(this, msg, ToastLength.Short).Show();
                }
            });
        }
        public void PostCommentSuccess(NewsCommentModel comment)
        {
            handler.Post(() =>
            {
                txtContent.Enabled = true;
                txtContent.Text = "";
                proLoading.Visibility = ViewStates.Gone;
                btnComment.Visibility = ViewStates.Visible;
                var data = adapter.GetData();
                if (data.Count > 0)
                {
                    comment.Floor = data[data.Count - 1].Floor + 1;
                }
                else
                {
                    comment.Floor = 1;
                }
                adapter.AddData(comment);
                adapter.AddData(comment);
                Toast.MakeText(this, Resources.GetString(Resource.String.comment_success), ToastLength.Short).Show();
            });
        }
        protected async override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == (int)RequestCode.LoginCode && resultCode == Result.Ok)
            {
                adapter.User = await SQLiteUtils.Instance().QueryUser();
                adapter.NotifyDataSetChanged();
                PostComment();
            }
        }
        public void OnDelete(int id)
        {
            //ÊÇ·ñµÇÂ¼
            var user = UserShared.GetAccessToken(this);
            if (user.access_token == "" || user.RefreshTime.AddSeconds(user.expires_in) < DateTime.Now)
            {
                ShowLogin();
            }
            else
            {
                var item = adapter.GetData().Where(a => a.CommentID == id).FirstOrDefault();
                var child = recyclerView.FindViewWithTag(item.CommentID.ToString());
                child.FindViewById(Resource.Id.imgDelete).Visibility = ViewStates.Gone;
                child.FindViewById(Resource.Id.progressBar).Visibility = ViewStates.Visible;
                commentPresenter.DeleteComment(user, item.CommentID);
            }
        }
        public void DeleteCommentFail(int id, string msg)
        {
            handler.Post(() =>
            {
                var child = recyclerView.FindViewWithTag(id.ToString());
                child.FindViewById(Resource.Id.imgDelete).Visibility = ViewStates.Visible;
                child.FindViewById(Resource.Id.progressBar).Visibility = ViewStates.Gone;
                Toast.MakeText(this, msg, ToastLength.Short).Show();
            });
        }
        public void DeleteCommentSuccess(int id)
        {
            handler.Post(() =>
            {
                var child = recyclerView.FindViewWithTag(id.ToString());
                child.FindViewById(Resource.Id.imgDelete).Visibility = ViewStates.Visible;
                child.FindViewById(Resource.Id.progressBar).Visibility = ViewStates.Gone;

                var data = adapter.GetData();
                var index = data.IndexOf(data.Where(a => a.CommentID == id).FirstOrDefault());
                adapter.Remove(index);
                if (data.Count == 0)
                {
                    adapter.SetEmptyView(notDataView);
                }
                Toast.MakeText(this, Resources.GetString(Resource.String.delete_success), ToastLength.Short).Show();
            });
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
                StartActivityForResult(new Intent(this, typeof(LoginActivity)), (int)RequestCode.LoginCode);
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