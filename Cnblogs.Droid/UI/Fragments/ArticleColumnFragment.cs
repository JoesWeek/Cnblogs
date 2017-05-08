using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Cnblogs.Droid.Model;
using Cnblogs.Droid.Presenter;
using Cnblogs.Droid.UI.Adapters;
using Cnblogs.Droid.UI.Shareds;
using Cnblogs.Droid.UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using Fragment = Android.Support.V4.App.Fragment;

namespace Cnblogs.Droid.UI.Fragments
{
    public class ArticleColumnFragment : Fragment, IArticleColumnView, SwipeRefreshLayout.IOnRefreshListener, BaseAdapter.IOnLoadMoreListener
    {
        public View view;
        public int position;
        private IArticleColumnPresenter articlesPresenter;
        private SwipeRefreshLayout swipeRefreshLayout;
        private RecyclerView recyclerView;
        private ArticlesAdapter adapter;
        private View notDataView;
        private View errorView;
        private int pageIndex = 1;
        private DateTime refreshTime;
        private Handler handler;

        public static ArticleColumnFragment NewInstance(int position)
        {
            ArticleColumnFragment columnFragment = new ArticleColumnFragment();
            Bundle b = new Bundle();
            b.PutInt("position", position);
            columnFragment.Arguments = b;
            return columnFragment;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            position = Arguments.GetInt("position");
            articlesPresenter = new ArticleColumnPresenter(this);
            handler = new Handler();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            return view = inflater.Inflate(Resource.Layout.fragment_article, container, false);
        }
        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            swipeRefreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            swipeRefreshLayout.SetColorSchemeResources(Resource.Color.primary);
            swipeRefreshLayout.SetOnRefreshListener(this);

            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView);
            var manager = new LinearLayoutManager(this.Activity);
            recyclerView.SetLayoutManager(manager);

            adapter = new ArticlesAdapter();
            adapter.SetOnLoadMoreListener(this);

            recyclerView.SetAdapter(adapter);
            notDataView = this.Activity.LayoutInflater.Inflate(Resource.Layout.empty_view, (ViewGroup)recyclerView.Parent, false);
            notDataView.Click += delegate (object sender, EventArgs e)
            {
                OnRefresh();
            };
            errorView = this.Activity.LayoutInflater.Inflate(Resource.Layout.error_view, (ViewGroup)recyclerView.Parent, false);
            errorView.Click += delegate (object sender, EventArgs e)
            {
                OnRefresh();
            };
            recyclerView.Post(async () =>
            {
                await articlesPresenter.GetClientArticles(position);
            });
        }
        public void Refresh()
        {
            if (refreshTime.AddMinutes(15) < DateTime.Now)
            {
                OnRefresh();
            }
        }
        public async void OnRefresh()
        {
            if (pageIndex > 1)
                pageIndex = 1;
            swipeRefreshLayout.Refreshing = true;

            await articlesPresenter.GetServiceArticles(TokenShared.GetAccessToken(this.Activity), position, pageIndex);
        }
        public void GetServiceArticlesFail(string msg)
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
                Toast.MakeText(this.Activity, msg, ToastLength.Short).Show();
            });
        }
        public void GetServiceArticlesSuccess(List<ArticlesModel> lists)
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
                        refreshTime = DateTime.Now;
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
        public void GetClientArticlesSuccess(List<ArticlesModel> lists)
        {
            recyclerView.Post(() =>
            {
                if (lists.Count > 0)
                {
                    adapter.SetNewData(lists);
                }
            });
        }
        public async void OnLoadMoreRequested()
        {
            swipeRefreshLayout.Enabled = false;
            await articlesPresenter.GetServiceArticles(TokenShared.GetAccessToken(this.Activity), position, pageIndex);
        }
        public override void OnHiddenChanged(bool hidden)
        {
            base.OnHiddenChanged(hidden);
            if (hidden)
            {
                if (swipeRefreshLayout.Refreshing)
                {
                    swipeRefreshLayout.Refreshing = false;
                }
            }
        }
    }
}