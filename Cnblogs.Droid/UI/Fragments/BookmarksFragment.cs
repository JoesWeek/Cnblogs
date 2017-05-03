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
using Cnblogs.Droid.UI.Listeners;
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
    public class BookmarksFragment : Fragment, IBookmarksView, BaseAdapter.IOnLoadMoreListener, SwipeRefreshLayout.IOnRefreshListener, IOnDeleteClickListener, IOnEditClickListener
    {
        private View view;
        private Handler handler;
        private IBookmarksPresenter bookmarksPresenter;
        private SwipeRefreshLayout swipeRefreshLayout;
        private RecyclerView recyclerView;
        private BookmarksAdapter adapter;
        private View notDataView;
        private View errorView;
        private View notloginView;
        private int pageIndex = 1;
        private bool notlogin = false;
        private DateTime refreshTime;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            handler = new Handler();
            bookmarksPresenter = new BookmarksPresenter(this);
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            return view = inflater.Inflate(Resource.Layout.fragment_bookmarks, container, false);
        }
        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            this.HasOptionsMenu = true;
            swipeRefreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            swipeRefreshLayout.SetColorSchemeResources(Resource.Color.primary);
            swipeRefreshLayout.SetOnRefreshListener(this);

            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerView);
            recyclerView.SetLayoutManager(new LinearLayoutManager(this.Activity));
            adapter = new BookmarksAdapter();
            adapter.SetOnLoadMoreListener(this);
            adapter.OnDeleteClickListener = this;
            adapter.OnEditClickListener = this;

            recyclerView.SetAdapter(adapter);
            notloginView = this.Activity.LayoutInflater.Inflate(Resource.Layout.nologin_view, (ViewGroup)recyclerView.Parent, false);
            notloginView.Click += delegate (object sender, EventArgs e)
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
                if (await ChenkLogin())
                {
                    return;
                }
                else
                {
                    await bookmarksPresenter.GetClientBookmarks();
                }
            });
        }
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.addstatus, menu);
        }
        public override void OnResume()
        {
            base.OnResume();
            NeedRefresh();
        }
        public async void NeedRefresh()
        {
            if (await ChenkLogin())
            {
                return;
            }
            else
            {
                if (!notlogin && refreshTime.AddMinutes(15) < DateTime.Now)
                {
                    OnRefresh();
                }
            }
        }
        public async void OnRefresh()
        {
            if (await ChenkLogin())
            {
                return;
            }
            else
            {
                if (pageIndex > 1)
                    pageIndex = 1;
                swipeRefreshLayout.Refreshing = true;
                await bookmarksPresenter.GetServiceBookmarks(UserShared.GetAccessToken(this.Activity), pageIndex);
            }
        }
        public async void OnLoadMoreRequested()
        {
            await bookmarksPresenter.GetServiceBookmarks(UserShared.GetAccessToken(this.Activity), pageIndex);
        }
        public async Task<bool> ChenkLogin()
        {
            var user = UserShared.GetAccessToken(this.Activity);
            if (user.access_token == "" || user.RefreshTime.AddSeconds(user.expires_in) < DateTime.Now)
            {
                //Î´µÇÂ¼»òÇå¿ÕTokenÊ§Ð§
                //Çå¿ÕToken
                UserShared.Update(this.Activity, new AccessToken());
                await SQLiteUtils.Instance().DeleteUserAll();
                recyclerView.Post(() =>
                {
                    adapter.SetEmptyView(notloginView);
                });
                return notlogin = true;
            }
            return notlogin = false;
        }
        public void GetServiceBookmarksFail(string msg)
        {
            recyclerView.Post(() =>
            {
                if (swipeRefreshLayout.Refreshing)
                {
                    swipeRefreshLayout.Refreshing = false;
                }
                if (!swipeRefreshLayout.Enabled)
                {
                    swipeRefreshLayout.Refreshing = true;
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
        public void GetServiceBookmarksSuccess(List<BookmarksModel> lists)
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
        public void GetClientBookmarksSuccess(List<BookmarksModel> lists)
        {
            recyclerView.Post(() =>
            {
                if (lists.Count > 0)
                {
                    adapter.SetNewData(lists);
                }
            });
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
            else
            {
                NeedRefresh();
            }
            this.Activity.InvalidateOptionsMenu();
        }
        public void OnDelete(int id)
        {
            //ÊÇ·ñµÇÂ¼
            var user = UserShared.GetAccessToken(this.Activity);
            if (user.access_token == "" || user.RefreshTime.AddSeconds(user.expires_in) < DateTime.Now)
            {
                ShowLogin();
            }
            else
            {
                var item = adapter.GetData().Where(a => a.WzLinkId == id).FirstOrDefault();
                var child = recyclerView.FindViewWithTag(item.WzLinkId.ToString());
                child.FindViewById(Resource.Id.imgDelete).Visibility = ViewStates.Gone;
                child.FindViewById(Resource.Id.progressBar).Visibility = ViewStates.Visible;
                bookmarksPresenter.DeleteBookmark(user, item.WzLinkId);
            }
        }
        public void DeleteBookmarkFail(int id, string msg)
        {
            handler.Post(() =>
            {
                var child = recyclerView.FindViewWithTag(id.ToString());
                child.FindViewById(Resource.Id.imgDelete).Visibility = ViewStates.Visible;
                child.FindViewById(Resource.Id.progressBar).Visibility = ViewStates.Gone;
                Toast.MakeText(this.Activity, msg, ToastLength.Short).Show();
            });
        }
        public void DeleteBookmarkSuccess(int id)
        {
            handler.Post(() =>
            {
                var child = recyclerView.FindViewWithTag(id.ToString());
                child.FindViewById(Resource.Id.imgDelete).Visibility = ViewStates.Visible;
                child.FindViewById(Resource.Id.progressBar).Visibility = ViewStates.Gone;

                var data = adapter.GetData();
                var index = data.IndexOf(data.Where(a => a.WzLinkId == id).FirstOrDefault());
                adapter.Remove(index);
                if (data.Count == 0)
                {
                    adapter.SetEmptyView(notDataView);
                }
                Toast.MakeText(this.Activity, this.Activity.Resources.GetString(Resource.String.delete_success), ToastLength.Short).Show();
            });
        }
        public void OnEdit(int position)
        {
            //ÊÇ·ñµÇÂ¼
            var user = UserShared.GetAccessToken(this.Activity);
            if (user.access_token == "" || user.RefreshTime.AddSeconds(user.expires_in) < DateTime.Now)
            {
                ShowLogin();
            }
            else
            {
                var item = adapter.GetItem(position);
                var intent = new Intent(this.Activity, typeof(BookmarkEditActivity));
                intent.PutExtra("id", item.WzLinkId);
                intent.PutExtra("position", position);
                StartActivityForResult(intent, (int)RequestCode.BookmarkEditCode);
            }
        }
        public async void NotifyItemChanged(int id, int position)
        {
            var bookmarks = adapter.GetData();
            var index = bookmarks.IndexOf(adapter.GetItem(position));
            bookmarks[index] = await SQLiteUtils.Instance().QueryBookmark(id);
            adapter.NotifyItemChanged(position);

        }
        public async override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == (int)RequestCode.BookmarkEditCode && resultCode == (int)Result.Ok)
            {
                var id = data.GetIntExtra("id", 0);
                var position = data.GetIntExtra("position", 0);
                var bookmarks = adapter.GetData();
                var index = bookmarks.IndexOf(adapter.GetItem(position));
                bookmarks[index] = await SQLiteUtils.Instance().QueryBookmark(id);
                adapter.NotifyItemChanged(position);
            }
        }
        public async void ShowLogin()
        {
            //Î´µÇÂ¼»òÇå¿ÕTokenÊ§Ð§
            //Çå¿ÕToken
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
    }
}