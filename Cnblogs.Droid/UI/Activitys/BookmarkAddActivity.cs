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
using System.Linq;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Cnblogs.Droid.UI.Activitys
{
    [Activity(Label = "@string/bookmarks_add", LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
    public class BookmarkAddActivity : BaseActivity, IBookmarkAddView, View.IOnClickListener, Toolbar.IOnMenuItemClickListener
    {
        private Handler handler;
        private IBookmarkAddPresenter bookmarkPresenter;
        private Toolbar toolbar;
        private ProgressDialog dialog;
        private EditText txtLinkUrl;
        private EditText txtTitle;
        private EditText txtTags;
        private EditText txtContent;
        private BookmarksModel bookmark;
        protected override int LayoutResource => Resource.Layout.bookmark_edit;
        public static void Start(Context context, string linkurl, string title, bool fromCNBlogs)
        {
            Intent intent = new Intent(context, typeof(BookmarkAddActivity));
            intent.PutExtra("linkurl", linkurl);
            intent.PutExtra("title", title);
            intent.PutExtra("fromCNBlogs", fromCNBlogs);
            context.StartActivity(intent);
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var linkUrl = Intent.GetStringExtra("linkurl");
            var title = Intent.GetStringExtra("title");
            var fromCNBlogs = Intent.GetBooleanExtra("fromCNBlogs", false);
            bookmarkPresenter = new BookmarkAddPresenter(this);
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

            txtLinkUrl = FindViewById<EditText>(Resource.Id.txtLinkUrl);
            txtTitle = FindViewById<EditText>(Resource.Id.txtTitle);
            txtTags = FindViewById<EditText>(Resource.Id.txtTags);
            txtContent = FindViewById<EditText>(Resource.Id.txtContent);

            txtLinkUrl.Text = linkUrl;
            if (linkUrl != null && linkUrl != "")
                txtLinkUrl.SetSelection(txtLinkUrl.Text.Length);
            txtTitle.Text = title;

            bookmark = new BookmarksModel();
            bookmark.FromCNBlogs = fromCNBlogs;
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            this.MenuInflater.Inflate(Resource.Menu.send, menu);
            return true;
        }
        public bool OnMenuItemClick(IMenuItem item)
        {
            PostBookmark();
            return true;
        }
        public async void PostBookmark()
        {
            //是否登录
            var user = UserShared.GetAccessToken(this);
            if (user.access_token == "" || user.RefreshTime.AddSeconds(user.expires_in) < DateTime.Now)
            {
                //未登录或清空Token失效
                //清空Token
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
            else
            {
                var linkurl = txtLinkUrl.Text;
                var title = txtTitle.Text;
                var tags = txtTags.Text;
                var content = txtContent.Text;
                if (linkurl.Length == 0)
                {
                    Toast.MakeText(this, "请输入网址", ToastLength.Short).Show();
                }
                else if (linkurl.Length < 3)
                {
                    Toast.MakeText(this, "网址的内容太少了,至少3个字吧", ToastLength.Short).Show();
                }
                else if (title.Length == 0)
                {
                    Toast.MakeText(this, "请输入标题", ToastLength.Short).Show();
                }
                else if (title.Length < 3)
                {
                    Toast.MakeText(this, "标题的内容太少了,至少3个字吧", ToastLength.Short).Show();
                }
                else
                {
                    dialog.SetMessage(Resources.GetString(Resource.String.addstatusing));
                    dialog.Show();

                    bookmark.LinkUrl = linkurl;
                    bookmark.Summary = content;
                    bookmark.Tags = tags.Split(',').ToList();
                    bookmark.Title = title;
                    bookmarkPresenter.BookmarkAdd(user, bookmark);
                }
            }
        }
        public void BookmarkAddFail(string msg)
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
        public void BookmarkAddSuccess()
        {
            handler.Post(() =>
            {
                if (dialog.IsShowing)
                {
                    dialog.Dismiss();
                }
                Toast.MakeText(this, Resources.GetString(Resource.String.addstatus_success), ToastLength.Short).Show();
                SetResult(Result.Ok);
                ActivityCompat.FinishAfterTransition(this);
            });
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == (int)RequestCode.LoginCode && resultCode == Result.Ok)
            {
                PostBookmark();
            }
        }
        public void OnClick(View v)
        {
            ActivityCompat.FinishAfterTransition(this);
        }
    }
}