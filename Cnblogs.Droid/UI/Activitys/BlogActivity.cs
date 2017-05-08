using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Cnblogs.Droid.Model;
using Cnblogs.Droid.Presenter;
using Cnblogs.Droid.UI.Adapters;
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
    [Activity(Label = "", LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
    public class BlogActivity : BaseActivity, IBlogView, View.IOnClickListener, SwipeRefreshLayout.IOnRefreshListener, BaseAdapter.IOnLoadMoreListener
    {
        private string blogApp;
        private IBlogPresenter blogPresenter;
        private Handler handler;

        private CoordinatorLayout coordinatorLayout;
        private AppBarLayout appBarLayout;
        private LinearLayout linearLayout;
        private Toolbar toolbar;
        private ImageView imgAvatar;
        private TextView txtTitle;
        private TextView txtSubTitle;
        private SwipeRefreshLayout swipeRefreshLayout;
        private RecyclerView recyclerView;
        private ArticlesAdapter adapter;
        private View notDataView;
        private View errorView;
        private int pageIndex = 1;

        private BlogModel blog;
        private UserModel user;
        protected override int LayoutResource => Resource.Layout.blog;
        public static void Start(Context context, string blogApp)
        {
            Intent intent = new Intent(context, typeof(BlogActivity));
            intent.PutExtra("blogApp", blogApp);
            context.StartActivity(intent);
        }
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            blogApp = Intent.GetStringExtra("blogApp");
            blogPresenter = new BlogPresenter(this);
            handler = new Handler();

            coordinatorLayout = FindViewById<CoordinatorLayout>(Resource.Id.coordinatorLayout);
            appBarLayout = FindViewById<AppBarLayout>(Resource.Id.appbar);
            linearLayout = FindViewById<LinearLayout>(Resource.Id.linearLayout);

            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            toolbar.SetNavigationIcon(Resource.Drawable.back_24dp);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            toolbar.SetNavigationOnClickListener(this);

            StatusBarCompat.SetCollapsingToolbar(this, coordinatorLayout, appBarLayout, linearLayout, toolbar);

            imgAvatar = FindViewById<ImageView>(Resource.Id.avatar);
            txtTitle = FindViewById<TextView>(Resource.Id.txtTitle);
            txtSubTitle = FindViewById<TextView>(Resource.Id.txtSubTitle);

            user = await SQLiteUtils.Instance().QueryUser();

            swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            swipeRefreshLayout.SetColorSchemeResources(Resource.Color.primary);
            swipeRefreshLayout.SetOnRefreshListener(this);

            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            var manager = new LinearLayoutManager(this);
            recyclerView.SetLayoutManager(manager);

            adapter = new ArticlesAdapter();
            adapter.SetOnLoadMoreListener(this);

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
                await blogPresenter.GetClientBlog(blogApp);
                OnRefresh();
            });
        }
        public async void OnRefresh()
        {
            if (pageIndex > 1)
                pageIndex = 1;
            swipeRefreshLayout.Refreshing = true;

            await blogPresenter.GetServiceBlog(UserShared.GetAccessToken(this), blogApp);
            await blogPresenter.GetServiceBlogPosts(UserShared.GetAccessToken(this), blogApp, pageIndex);
        }
        public async void OnLoadMoreRequested()
        {
            swipeRefreshLayout.Enabled = false;
            await blogPresenter.GetServiceBlog(UserShared.GetAccessToken(this), blogApp);
            await blogPresenter.GetServiceBlogPosts(UserShared.GetAccessToken(this), blogApp, pageIndex);
        }
        public void GetClientBlogSuccess(BlogModel model)
        {
            if (model != null)
            {
                blog = model;
                BindView();
            }
        }
        public void GetClientBlogPostsSuccess(List<ArticlesModel> lists)
        {
            recyclerView.Post(() =>
            {
                if (lists.Count > 0)
                {
                    adapter.SetNewData(lists);
                }
            });
        }
        public void GetServiceBlogFail(string msg)
        {
            Toast.MakeText(this, msg, ToastLength.Short).Show();
        }

        public void GetServiceBlogSuccess(BlogModel model)
        {
            if (model != null)
            {
                blog = model;
                BindView();
            }
        }
        private void BindView()
        {
            txtTitle.Text = blog.Title;
            txtSubTitle.Text = blog.SubTitle;
            try
            {
                Picasso.With(this)
                            .Load(user.Avatar)
                            .Placeholder(Resource.Drawable.placeholder)
                            .Error(Resource.Drawable.placeholder)
                            .Transform(new CircleTransform())
                            .Into(imgAvatar);
            }
            catch (Exception)
            {

            }
        }

        public void GetServiceBlogPostsFail(string msg)
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

        public void GetServiceBlogPostsSuccess(List<ArticlesModel> lists)
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
        public void OnClick(View v)
        {
            ActivityCompat.FinishAfterTransition(this);
        }
    }
}