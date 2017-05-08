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
using Cnblogs.Droid.UI.Listeners;
using Cnblogs.Droid.UI.Shareds;
using Cnblogs.Droid.UI.Views;
using Cnblogs.Droid.UI.Widgets;
using Cnblogs.Droid.Utils;
using Com.Umeng.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fragment = Android.Support.V4.App.Fragment;

namespace Cnblogs.Droid.UI.Fragments
{
    public class StatusColumnFragment : Fragment, IStatusColumnView, SwipeRefreshLayout.IOnRefreshListener, BaseAdapter.IOnLoadMoreListener, IOnDeleteClickListener
    {
        public View view;
        public int position;
        private IStatusColumnPresenter statusesPresenter;
        private SwipeRefreshLayout swipeRefreshLayout;
        private RecyclerView recyclerView;
        private View notDataView;
        private View errorView;
        private View nologinView;
        private StatusAdapter adapter;
        private int pageIndex = 1;
        private DateTime refreshTime;
        private Handler handler;

        public static StatusColumnFragment NewInstance(int position)
        {
            StatusColumnFragment columnFragment = new StatusColumnFragment();
            Bundle b = new Bundle();
            b.PutInt("position", position);
            columnFragment.Arguments = b;
            return columnFragment;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            position = Arguments.GetInt("position");
            statusesPresenter = new StatusColumnPresenter(this);
            handler = new Handler();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            return view = inflater.Inflate(Resource.Layout.fragment_statuses_column, container, false);
        }
        public override async void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            try
            {
                this.HasOptionsMenu = true;
                swipeRefreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
                swipeRefreshLayout.SetColorSchemeResources(Resource.Color.primary);
                swipeRefreshLayout.SetOnRefreshListener(this);

                recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView);
                var manager = new LinearLayoutManager(this.Activity);
                recyclerView.SetLayoutManager(manager);

                adapter = new StatusAdapter();
                adapter.SetOnLoadMoreListener(this);
                adapter.OnDeleteClickListener = this;
                adapter.User = await SQLiteUtils.Instance().QueryUser();
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
                    if (position == 0)
                    {
                        await statusesPresenter.GetClientStatus();
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
            catch (Exception ex)
            {
                MobclickAgent.ReportError(Context, ex.Message + ex.StackTrace);
            }
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
            if (position == 0)
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

                    recyclerView.Post(async () =>
                    {
                        adapter.User = await SQLiteUtils.Instance().QueryUser();
                        adapter.SetNewData(new List<StatusModel>());
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

            await statusesPresenter.GetServiceStatus(position > 0 ? UserShared.GetAccessToken(this.Activity) : TokenShared.GetAccessToken(this.Activity), position, pageIndex);
        }
        public void GetServiceStatusFail(string msg)
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
        public void GetServiceStatusSuccess(List<StatusModel> lists)
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
                    refreshTime = DateTime.Now;
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
        public void GetClientStatusSuccess(List<StatusModel> lists)
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
            await statusesPresenter.GetServiceStatus(position > 0 ? UserShared.GetAccessToken(this.Activity) : TokenShared.GetAccessToken(this.Activity), position, pageIndex);
        }
        public override async void OnHiddenChanged(bool hidden)
        {
            base.OnHiddenChanged(hidden);
            if (hidden)
            {
                if (swipeRefreshLayout.Refreshing)
                {
                    swipeRefreshLayout.Refreshing = false;
                }
            }
            else
            {
                adapter.User = await SQLiteUtils.Instance().QueryUser();
            }
            this.Activity.InvalidateOptionsMenu();
        }
        public async void OnDelete(int id)
        {
            //是否登录
            var user = UserShared.GetAccessToken(this.Activity);
            if (user.access_token == "" || user.RefreshTime.AddSeconds(user.expires_in) < DateTime.Now)
            {
                //未登录或清空Token失效
                //清空Token
                UserShared.Update(this.Activity, new AccessToken());
                await SQLiteUtils.Instance().DeleteUserAll();
                Android.Support.V7.App.AlertDialog.Builder dialog = new Android.Support.V7.App.AlertDialog.Builder(this.Activity);
                dialog.SetMessage(Resources.GetString(Resource.String.need_login_tip));
                dialog.SetPositiveButton(Resources.GetString(Resource.String.confirm), delegate
                {
                    StartActivityForResult(new Intent(this.Activity, typeof(LoginActivity)), (int)RequestCode.LoginCode);
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
                var item = adapter.GetData().Where(a => a.Id == id).FirstOrDefault();
                var child = recyclerView.FindViewWithTag(item.Id);
                child.FindViewById(Resource.Id.imgDelete).Visibility = ViewStates.Gone;
                child.FindViewById(Resource.Id.progressBar).Visibility = ViewStates.Visible;
                statusesPresenter.DeleteStatus(user, item.Id);
            }
        }
        public void DeleteStatusFail(int id, string msg)
        {
            handler.Post(() =>
            {
                var child = recyclerView.FindViewWithTag(id);
                child.FindViewById(Resource.Id.imgDelete).Visibility = ViewStates.Visible;
                child.FindViewById(Resource.Id.progressBar).Visibility = ViewStates.Gone;
                Toast.MakeText(this.Activity, msg, ToastLength.Short).Show();
            });
        }
        public void DeleteStatusSuccess(int id)
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
                Toast.MakeText(this.Activity, Resources.GetString(Resource.String.delete_success), ToastLength.Short).Show();
            });
        }
    }
}