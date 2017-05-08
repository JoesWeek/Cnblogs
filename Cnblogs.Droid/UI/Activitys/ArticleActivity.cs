using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Widget;
using Android.Text;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Cnblogs.Droid.Model;
using Cnblogs.Droid.Presenter;
using Cnblogs.Droid.UI.Shareds;
using Cnblogs.Droid.UI.Views;
using Cnblogs.Droid.UI.Widgets;
using Cnblogs.Droid.Utils;
using Com.Umeng.Socialize;
using Com.Umeng.Socialize.Bean;
using Com.Umeng.Socialize.Media;
using Com.Umeng.Socialize.Shareboard;
using Com.Umeng.Socialize.Utils;
using Square.Picasso;
using System;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Cnblogs.Droid.UI.Activitys
{
    [Activity(Label = "@string/articlehome", LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
    public class ArticleActivity : BaseActivity, IArticleView, View.IOnClickListener, SwipeRefreshLayout.IOnRefreshListener, Toolbar.IOnMenuItemClickListener, IShareBoardlistener
    {
        private int Id;
        private IArticlePresenter articlePresenter;
        private Handler handler;

        private Toolbar toolbar;
        private SwipeRefreshLayout swipeRefreshLayout;
        private NestedScrollView scrollView;
        private TextView txtTitle;
        private ImageView imgAvatar;
        private TextView txtAuthor;
        private TextView txtPostdate;
        private WebView txtBody;
        private TextView txtDigg;
        private TextView txtRead;
        private TextView txtComments;
        private TextView txtBookmark;

        private ArticlesModel article;
        private ShareAction shareAction;
        protected override int LayoutResource => Resource.Layout.article;
        public static void Start(Context context, int id)
        {
            Intent intent = new Intent(context, typeof(ArticleActivity));
            intent.PutExtra("id", id);
            context.StartActivity(intent);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Id = Intent.GetIntExtra("id", 0);
            articlePresenter = new ArticlePresenter(this);
            handler = new Handler();

            StatusBarCompat.SetOrdinaryToolBar(this);
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            toolbar.SetNavigationIcon(Resource.Drawable.back_24dp);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            toolbar.SetNavigationOnClickListener(this);
            toolbar.SetOnMenuItemClickListener(this);
            swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            swipeRefreshLayout.SetColorSchemeResources(Resource.Color.primary);
            swipeRefreshLayout.SetOnRefreshListener(this);
            scrollView = FindViewById<NestedScrollView>(Resource.Id.scrollView);

            txtTitle = FindViewById<TextView>(Resource.Id.txtTitle);
            imgAvatar = FindViewById<ImageView>(Resource.Id.llAvatar);
            txtAuthor = FindViewById<TextView>(Resource.Id.txtAuthor);
            txtPostdate = FindViewById<TextView>(Resource.Id.txtPostdate);
            txtBody = FindViewById<WebView>(Resource.Id.txtBody);
            txtBody.Settings.JavaScriptEnabled = true;
            txtBody.Settings.DomStorageEnabled = true;
            txtBody.Settings.LoadsImagesAutomatically = true;
            txtBody.Settings.DefaultTextEncodingName = "utf-8";
            txtBody.SetWebViewClient(BodyWebViewClient.With(this));
            txtBody.ScrollBarStyle = ScrollbarStyles.InsideOverlay;
            txtBody.Settings.SetSupportZoom(false);
            txtBody.Settings.BuiltInZoomControls = false;
            txtBody.Settings.CacheMode = CacheModes.CacheElseNetwork;
            txtBody.Settings.SetLayoutAlgorithm(WebSettings.LayoutAlgorithm.SingleColumn);
            var jsInterface = new WebViewJSInterface(this);
            txtBody.AddJavascriptInterface(jsInterface, "openlistner");
            jsInterface.CallFromPageReceived += delegate (object sender, WebViewJSInterface.CallFromPageReceivedEventArgs e)
            {
                PhotoActivity.Start(this, e.Result.Split(','), e.Index);
            };

            txtDigg = FindViewById<TextView>(Resource.Id.txtDigg);
            txtRead = FindViewById<TextView>(Resource.Id.txtRead);
            txtComments = FindViewById<TextView>(Resource.Id.txtComments);
            txtBookmark = FindViewById<TextView>(Resource.Id.txtBookmark);

            swipeRefreshLayout.Post(async () =>
            {
                await articlePresenter.GetClientArticle(Id);
            });

            shareAction = new ShareAction(this).SetDisplayList(SHARE_MEDIA.Weixin, SHARE_MEDIA.WeixinCircle, SHARE_MEDIA.WeixinFavorite, SHARE_MEDIA.Sina).SetShareboardclickCallback(this);

        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.share, menu);
            return true;
        }
        public bool OnMenuItemClick(IMenuItem item)
        {
            if (article != null)
            {
                shareAction.Open();
            }
            return true;
        }
        /// <summary>
        /// 屏幕横竖屏切换时避免出现window leak的问题
        /// </summary>
        /// <param name="newConfig"></param>
        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            shareAction.Close();
        }
        public async void OnRefresh()
        {
            swipeRefreshLayout.Refreshing = true;
            await articlePresenter.GetServiceArticle(TokenShared.GetAccessToken(this), Id);
        }
        public void GetClientArticleSuccess(ArticlesModel model)
        {
            handler.Post(() =>
            {
                article = model;
                txtTitle.Text = article.Title;
                txtAuthor.Text = Html.FromHtml(article.Author).ToString();
                txtPostdate.Text = DateTimeUtils.CommonTime(article.PostDate);

                if (article.DiggCount > 0)
                    txtDigg.Text = article.DiggCount.ToString();
                if (article.ViewCount > 0)
                    txtRead.Text = article.ViewCount.ToString();
                if (article.CommentCount > 0)
                {
                    txtComments.Text = article.CommentCount.ToString();
                }
                (txtComments.Parent as FrameLayout).Click += delegate
                {
                    ArticleCommentsActivity.Start(this, article.BlogApp, article.Id);
                };
                (txtBookmark.Parent as FrameLayout).Click += delegate
                {
                    var linkurl = article.Url;
                    var title = article.Title + " - " + article.Author + " - 博客园";
                    BookmarkAddActivity.Start(this, linkurl, title, true);
                };

                if (article.Body == null || article.Body == "")
                {
                    OnRefresh();
                }
                else
                {
                    GetServiceArticleSuccess(model);
                }
                try
                {
                    Picasso.With(this)
                                .Load(article.Avatar)
                                .Placeholder(Resource.Drawable.placeholder)
                                .Error(Resource.Drawable.placeholder)
                                .Transform(new CircleTransform())
                                .Into(imgAvatar);
                }
                catch (Exception)
                {

                }
            });
        }
        public void GetServiceArticleFail(string msg)
        {
            if (swipeRefreshLayout.Refreshing)
            {
                swipeRefreshLayout.Refreshing = false;
            }
            Toast.MakeText(this, msg, ToastLength.Short).Show();
        }
        public void GetServiceArticleSuccess(ArticlesModel model)
        {
            handler.Post(() =>
            {
                article = model;
                if (swipeRefreshLayout.Refreshing)
                {
                    swipeRefreshLayout.Refreshing = false;
                }
                var content = HtmlUtils.ReadHtml(Assets);
                var body = HtmlUtils.ReplaceHtml(model.Body).Trim('"');
                txtBody.LoadDataWithBaseURL("file:///android_asset/", content.Replace("#title#", "").Replace("#body#", body), "text/html", "utf-8", null);
            });
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            UMShareAPI.Get(this).OnActivityResult(requestCode, (int)resultCode, data);
        }
        public void OnClick(View v)
        {
            ActivityCompat.FinishAfterTransition(this);
        }
        public void Onclick(SnsPlatform snsPlatform, SHARE_MEDIA media)
        {
            UMWeb web = new UMWeb(article.Url);
            web.Title = article.Title;
            web.Description = article.Description;
            new ShareAction(this).WithMedia(web)
                    .SetPlatform(media)
                    .SetCallback(new UMengCustomShare(this))
                    .Share();
        }
    }
}