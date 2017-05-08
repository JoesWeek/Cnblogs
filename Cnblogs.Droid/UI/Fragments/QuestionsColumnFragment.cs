using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Cnblogs.Droid.Model;
using Cnblogs.Droid.Presenter;
using Cnblogs.Droid.UI.Activitys;
using Cnblogs.Droid.UI.Adapters;
using Cnblogs.Droid.UI.Shareds;
using Cnblogs.Droid.UI.Views;
using Cnblogs.Droid.UI.Widgets;
using Cnblogs.Droid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fragment = Android.Support.V4.App.Fragment;

namespace Cnblogs.Droid.UI.Fragments
{
    public class QuestionsColumnFragment : Fragment, IQuestionColumnView, SwipeRefreshLayout.IOnRefreshListener, BaseAdapter.IOnLoadMoreListener
    {
        public View view;
        public int position;
        private IQuestionColumnPresenter questionPresenter;
        private SwipeRefreshLayout swipeRefreshLayout;
        private RecyclerView recyclerView;
        private QuestionsAdapter adapter;
        private View notDataView;
        private View errorView;
        private View nologinView;
        private int pageIndex = 1;
        private DateTime refreshTime;
        private Handler handler;

        public static QuestionsColumnFragment NewInstance(int position)
        {
            QuestionsColumnFragment columnFragment = new QuestionsColumnFragment();
            Bundle b = new Bundle();
            b.PutInt("position", position);
            columnFragment.Arguments = b;
            return columnFragment;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            position = Arguments.GetInt("position");
            questionPresenter = new QuestionColumnPresenter(this);
            handler = new Handler();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            return view = inflater.Inflate(Resource.Layout.fragment_questions_column, container, false);
        }
        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            this.HasOptionsMenu = true;

            swipeRefreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            swipeRefreshLayout.SetColorSchemeResources(Resource.Color.primary);
            swipeRefreshLayout.SetOnRefreshListener(this);

            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView);
            var manager = new LinearLayoutManager(this.Activity);
            recyclerView.SetLayoutManager(manager);

            adapter = new QuestionsAdapter();
            adapter.SetOnLoadMoreListener(this);
            recyclerView.SetAdapter(adapter);

            nologinView = this.Activity.LayoutInflater.Inflate(Resource.Layout.nologin_view, (ViewGroup)recyclerView.Parent, false);
            nologinView.Click += delegate (object sender, EventArgs e)
            {
                StartActivityForResult(new Intent(this.Activity, typeof(LoginActivity)), (int)RequestCode.LoginCode);
            };
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
                if (position < 4)
                {
                    await questionPresenter.GetClientQuestions(position);
                }
                else if (!LoginUtils.Instance(this.Activity).GetLoginStatus())
                {
                    recyclerView.Post(() =>
                    {
                        adapter.SetEmptyView(nologinView);
                    });
                }
            });
        }
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            if (position == 0)
            {
                inflater.Inflate(Resource.Menu.addstatus, menu);
            }
        }
        public override void OnResume()
        {
            base.OnResume();
            OnRefresh(false);
        }
        public void OnRefresh()
        {
            OnRefresh(true);
        }
        public void OnRefresh(bool isRefresh)
        {
            if (position < 4)
            {
                if (isRefresh)
                {
                    GetServiceData();
                }
                else if (refreshTime.AddMinutes(15) < DateTime.Now)
                {
                    //获取数据
                    GetServiceData();
                }
            }
            else
            {
                if (LoginUtils.Instance(this.Activity).GetLoginStatus())
                {
                    if (isRefresh)
                    {
                        GetServiceData();
                    }
                    else if (refreshTime.AddMinutes(15) < DateTime.Now)
                    {
                        //获取数据
                        GetServiceData();
                    }
                }
                else
                {
                    swipeRefreshLayout.Refreshing = false;

                    recyclerView.Post(() =>
                    {
                        adapter.SetNewData(new List<QuestionsModel>());
                        adapter.SetEmptyView(nologinView);
                    });
                }
            }
        }
        public async void GetServiceData()
        {
            if (pageIndex > 1)
                pageIndex = 1;
            swipeRefreshLayout.Refreshing = true;

            await questionPresenter.GetServiceQuestions(position == 4 ? UserShared.GetAccessToken(this.Activity) : TokenShared.GetAccessToken(this.Activity), position, pageIndex);
        }
        public void GetServiceQuestionsFail(string msg)
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
        public void GetServiceQuestionsSuccess(List<QuestionsModel> lists)
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
        public void GetClientQuestionsSuccess(List<QuestionsModel> lists)
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
            await questionPresenter.GetServiceQuestions(position == 4 ? UserShared.GetAccessToken(this.Activity) : TokenShared.GetAccessToken(this.Activity), position, pageIndex);
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
            this.Activity.InvalidateOptionsMenu();
        }
    }
}