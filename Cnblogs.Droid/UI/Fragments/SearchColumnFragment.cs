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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Fragment = Android.Support.V4.App.Fragment;

namespace Cnblogs.Droid.UI.Fragments
{
    public class SearchColumnFragment : Fragment, ISearchColumnView, SwipeRefreshLayout.IOnRefreshListener, BaseAdapter.IOnLoadMoreListener
    {
        public View view;
        public int position;
        private ISearchColumnPresenter searchPresenter;
        private SwipeRefreshLayout swipeRefreshLayout;
        private RecyclerView recyclerView;
        private SearchAdapter searchAdapter;
        private View notDataView;
        private View errorView;
        private int pageIndex = 1;
        private DateTime refreshTime;
        private Handler handler;
        private string lastKeyWords = "";

        public static SearchColumnFragment NewInstance(int position)
        {
            SearchColumnFragment columnFragment = new SearchColumnFragment();
            Bundle b = new Bundle();
            b.PutInt("position", position);
            columnFragment.Arguments = b;
            return columnFragment;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            position = Arguments.GetInt("position");
            searchPresenter = new SearchColumnPresenter(this);
            handler = new Handler();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            return view = inflater.Inflate(Resource.Layout.fragment_search, container, false);
        }
        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            swipeRefreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            swipeRefreshLayout.SetColorSchemeResources(Resource.Color.primary);
            swipeRefreshLayout.SetOnRefreshListener(this);
            swipeRefreshLayout.Enabled = false;

            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView);
            var manager = new LinearLayoutManager(this.Activity);
            recyclerView.SetLayoutManager(manager);

            searchAdapter = new SearchAdapter(position);
            searchAdapter.SetOnLoadMoreListener(this);

            recyclerView.SetAdapter(searchAdapter);

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
        }
        public void Refresh(string keyWords)
        {
            if (keyWords != "" && keyWords != lastKeyWords)
            {
                lastKeyWords = keyWords;
                OnRefresh();
            }
            else
            {
                swipeRefreshLayout.Refreshing = false;
                swipeRefreshLayout.Enabled = false;
            }
        }
        public async void OnRefresh()
        {
            if (lastKeyWords != "")
            {
                if (pageIndex > 1)
                    pageIndex = 1;
                swipeRefreshLayout.Refreshing = true;
                swipeRefreshLayout.Enabled = true;

                await searchPresenter.Search(TokenShared.GetAccessToken(this.Activity), position, lastKeyWords, pageIndex);
            }
        }
        public void SearchFail(string msg)
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
                    searchAdapter.LoadMoreFail();
                }
                else if (searchAdapter.GetData().Count() == 0)
                {
                    searchAdapter.SetEmptyView(errorView);
                }
                Toast.MakeText(this.Activity, msg, ToastLength.Short).Show();
            });
        }
        public void SearchSuccess(List<SearchModel> lists)
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
                        searchAdapter.SetNewData(lists);
                        if (lists.Count < 10)
                        {
                            searchAdapter.LoadMoreEnd();
                        }
                        else
                        {
                            searchAdapter.SetEnableLoadMore(true);
                            pageIndex++;
                        }
                        refreshTime = DateTime.Now;
                    }
                    else if (searchAdapter.GetData().Count() == 0)
                    {
                        searchAdapter.SetEmptyView(notDataView);
                    }
                }
                else
                {
                    if (lists.Count > 0)
                    {
                        searchAdapter.AddData(lists);
                        searchAdapter.LoadMoreComplete();
                        pageIndex++;
                    }
                    else
                    {
                        searchAdapter.LoadMoreEnd();
                    }
                }
            });
        }
        public async void OnLoadMoreRequested()
        {
            swipeRefreshLayout.Enabled = false;
            await searchPresenter.Search(TokenShared.GetAccessToken(this.Activity), position, lastKeyWords, pageIndex);
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