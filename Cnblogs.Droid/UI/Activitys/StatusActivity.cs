using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Text;
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
using Square.Picasso;
using System;
using System.Collections.Generic;
using System.Linq;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Cnblogs.Droid.UI.Activitys
{
    [Activity(Label = "@string/statuses", LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
    public class StatusActivity : BaseActivity, IStatusView, IStatusCommentsView, View.IOnClickListener, SwipeRefreshLayout.IOnRefreshListener, IOnDeleteClickListener
    {
        private int Id;
        private IStatusPresenter statusPresenter;
        private IStatusCommentsPresenter commentPresenter;
        private Handler handler;

        private Toolbar toolbar;
        private SwipeRefreshLayout swipeRefreshLayout;
        private RecyclerView recyclerView;
        private TextView userName;
        private ImageView imgUserUrl;
        private ImageView imgPrivate;
        private ImageButton imgDelete;
        private TextView txtPostdate;
        private TextView txtBody;
        private TextView commentCount;
        private StatusCommentsAdapter adapter;
        private View notDataView;
        private View errorView;
        private TextView txtContent;
        private TextView btnComment;
        private ProgressBar proLoading;

        protected override int LayoutResource => Resource.Layout.status;
        public static void Start(Context context, int id)
        {
            Intent intent = new Intent(context, typeof(StatusActivity));
            intent.PutExtra("id", id);
            context.StartActivity(intent);
        }

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Id = Intent.GetIntExtra("id", 0);
            statusPresenter = new StatusPresenter(this);
            commentPresenter = new StatusCommentsPresenter(this);
            handler = new Handler();

            StatusBarCompat.SetOrdinaryToolBar(this);
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            toolbar.SetNavigationIcon(Resource.Drawable.back_24dp);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            toolbar.SetNavigationOnClickListener(this);

            swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            swipeRefreshLayout.SetColorSchemeResources(Resource.Color.primary);
            swipeRefreshLayout.SetOnRefreshListener(this);

            userName = FindViewById<TextView>(Resource.Id.txtUserName);
            imgUserUrl = FindViewById<ImageView>(Resource.Id.imgUserUrl);
            imgPrivate = FindViewById<ImageView>(Resource.Id.imgPrivate);
            imgDelete = FindViewById<ImageButton>(Resource.Id.imgDelete);
            txtPostdate = FindViewById<TextView>(Resource.Id.txtPostdate);
            txtBody = FindViewById<TextView>(Resource.Id.txtBody);
            commentCount = FindViewById<TextView>(Resource.Id.txtCommentCount);

            txtContent = FindViewById<TextView>(Resource.Id.txtContent);
            proLoading = FindViewById<ProgressBar>(Resource.Id.proLoading);
            btnComment = FindViewById<TextView>(Resource.Id.btnComment);
            btnComment.SetOnClickListener(this);

            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            recyclerView.SetLayoutManager(new LinearLayoutManager(this));
            recyclerView.NestedScrollingEnabled = false;
            adapter = new StatusCommentsAdapter();
            adapter.OnDeleteClickListener = this;
            adapter.User = await SQLiteUtils.Instance().QueryUser();
            recyclerView.SetAdapter(adapter);

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
            recyclerView.Post(async () =>
            {
                await statusPresenter.GetClientStatus(Id);
                OnRefresh();
            });
        }
        public async void OnRefresh()
        {
            swipeRefreshLayout.Refreshing = true;
            //ÊÇ·ñµÇÂ¼
            var user = UserShared.GetAccessToken(this);
            if (user.access_token != "" && user.RefreshTime.AddSeconds(user.expires_in) > DateTime.Now)
            {
                await statusPresenter.GetServiceStatus(user, Id);
            }
            await commentPresenter.GetServiceComments(TokenShared.GetAccessToken(this), Id);
        }
        public void GetClientStatusSuccess(StatusModel model)
        {
            if (model != null)
                BindView(model);
        }
        public void GetServiceStatusFail(string msg)
        {
            recyclerView.Post(() =>
            {
                Toast.MakeText(this, msg, ToastLength.Short).Show();
            });
        }
        public void GetServiceStatusSuccess(StatusModel model)
        {
            BindView(model);
        }
        private void BindView(StatusModel model)
        {
            handler.Post(() =>
            {
                toolbar.Title = Html.FromHtml(model.UserDisplayName).ToString() + "µÄÉÁ´æ";
                userName.Text = Html.FromHtml(model.UserDisplayName).ToString();
                txtPostdate.Text = DateTimeUtils.CommonTime(Convert.ToDateTime(model.DateAdded));
                if (model.CommentCount > 0)
                    commentCount.Text = model.CommentCount + " " + Resources.GetString(Resource.String.comment);
                imgDelete.SetOnClickListener(this);
                if (model.IsPrivate)
                {
                    imgPrivate.Visibility = ViewStates.Visible;
                    try
                    {
                        Picasso.With(this)
                                    .Load(Resource.Drawable.isPrivate)
                                    .Into(imgPrivate);
                    }
                    catch
                    {

                    }
                }
                else
                {
                    imgPrivate.Visibility = ViewStates.Gone;
                }
                if (model.IsLucky)
                {
                    var Content = model.Content + " ";
                    var spanText = new SpannableString(Html.FromHtml(Content));
                    spanText.SetSpan(new CenteredImageSpan(this.ApplicationContext, Resource.Drawable.luckystar), spanText.Length() - 1, spanText.Length(), SpanTypes.InclusiveExclusive);

                    txtBody.SetText(spanText, TextView.BufferType.Spannable);
                }
                else
                {
                    txtBody.SetText(Html.FromHtml(model.Content), TextView.BufferType.Spannable);
                }
                try
                {
                    Picasso.With(this)
                                .Load(model.UserIconUrl)
                                .Placeholder(Resource.Drawable.placeholder)
                                .Error(Resource.Drawable.placeholder)
                                .Transform(new CircleTransform())
                                .Into(imgUserUrl);
                }
                catch
                {

                }
            });
        }
        public void GetServiceCommentsFail(string msg)
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
        public void GetServiceCommentsSuccess(List<StatusCommentsModel> lists)
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
                adapter.SetNewData(lists);
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

                    ActivityCompat.FinishAfterTransition(this);
                    break;
            }
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
        public void PostCommentSuccess(StatusCommentsModel comment)
        {
            handler.Post(() =>
            {
                txtContent.Enabled = true;
                txtContent.Text = "";
                proLoading.Visibility = ViewStates.Gone;
                btnComment.Visibility = ViewStates.Visible;
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
                var item = adapter.GetData().Where(a => a.Id == id).FirstOrDefault();
                var child = recyclerView.FindViewWithTag(item.Id);
                child.FindViewById(Resource.Id.imgDelete).Visibility = ViewStates.Gone;
                child.FindViewById(Resource.Id.progressBar).Visibility = ViewStates.Visible;
                commentPresenter.DeleteComment(user, item.StatusId, item.Id);
            }
        }
        public void DeleteCommentFail(int id, string msg)
        {
            handler.Post(() =>
            {
                var child = recyclerView.FindViewWithTag(id);
                child.FindViewById(Resource.Id.imgDelete).Visibility = ViewStates.Visible;
                child.FindViewById(Resource.Id.progressBar).Visibility = ViewStates.Gone;
                Toast.MakeText(this, msg, ToastLength.Short).Show();
            });
        }
        public void DeleteCommentSuccess(int id)
        {
            handler.Post(() =>
            {
                var child = recyclerView.FindViewWithTag(id);
                child.FindViewById(Resource.Id.imgDelete).Visibility = ViewStates.Visible;
                child.FindViewById(Resource.Id.progressBar).Visibility = ViewStates.Gone;

                var data = adapter.GetData();
                var index = data.IndexOf(data.Where(a => a.Id == id).FirstOrDefault());
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