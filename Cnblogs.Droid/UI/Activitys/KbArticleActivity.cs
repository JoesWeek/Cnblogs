
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
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Cnblogs.Droid.UI.Activitys
{
    [Activity(Label = "@string/kbArticles", LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
    public class KbArticleActivity : BaseActivity, IKbArticleView, View.IOnClickListener, SwipeRefreshLayout.IOnRefreshListener, Toolbar.IOnMenuItemClickListener
    {
        private int Id;
        private IKbArticlePresenter kbarticlePresenter;
        private Handler handler;

        private Toolbar toolbar;
        private SwipeRefreshLayout swipeRefreshLayout;
        private NestedScrollView scrollView;
        private TextView txtPostdate;
        private WebView txtBody;
        private TextView txtDigg;
        private TextView txtRead;
        private TextView txtComments;
        private TextView txtBookmark;

        private KbArticlesModel kbarticle;
        private UMengSharesWidget sharesWidget;
        protected override int LayoutResource => Resource.Layout.news;
        public static void Start(Context context, int id)
        {
            Intent intent = new Intent(context, typeof(KbArticleActivity));
            intent.PutExtra("id", id);
            context.StartActivity(intent);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Id = Intent.GetIntExtra("id", 0);
            kbarticlePresenter = new KbArticlePresenter(this);
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
                await kbarticlePresenter.GetClientKbArticle(Id);
            });
            sharesWidget = new UMengSharesWidget(this);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.share, menu);
            return true;
        }
        public bool OnMenuItemClick(IMenuItem item)
        {
            if (kbarticle != null)
            {
                sharesWidget.Open("http://kb.cnblogs.com/page/" + kbarticle.Id + "/", kbarticle.Title);
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
            sharesWidget.Close();
        }
        public async void OnRefresh()
        {
            swipeRefreshLayout.Refreshing = true;
            await kbarticlePresenter.GetServiceKbArticle(TokenShared.GetAccessToken(this), Id);
        }
        public void GetClientKbArticleSuccess(KbArticlesModel model)
        {
            if (model != null)
            {
                kbarticle = model;
                toolbar.Title = model.Title;
                if (model.Author != null)
                {
                    toolbar.Title += " - " + HtmlUtils.GetHtml(model.Author).ToString();
                }
                txtPostdate.Text = "发布于：" + DateTimeUtils.CommonTime(kbarticle.DateAdded);

                if (kbarticle.DiggCount > 0)
                    txtDigg.Text = kbarticle.DiggCount.ToString();
                if (kbarticle.ViewCount > 0)
                    txtRead.Text = kbarticle.ViewCount.ToString();
                (txtComments.Parent as FrameLayout).Click += delegate
                {
                    Toast.MakeText(this, "很抱歉,暂未开放此功能。", ToastLength.Short).Show();
                };
                (txtBookmark.Parent as FrameLayout).Click += delegate
                {
                    var linkurl = "http://kb.cnblogs.com/page/" + kbarticle.Id + "/";
                    var title = kbarticle.Title + "_知识库_博客园";
                    BookmarkAddActivity.Start(this, linkurl, title, true);
                };

                if (kbarticle.Body == null || kbarticle.Body == "")
                {
                    OnRefresh();
                }
                else
                {
                    GetServiceKbArticleSuccess(model);
                }
            }
        }

        public void GetServiceKbArticleFail(string msg)
        {
            if (swipeRefreshLayout.Refreshing)
            {
                swipeRefreshLayout.Refreshing = false;
            }
            Toast.MakeText(this, msg, ToastLength.Short).Show();
        }

        public void GetServiceKbArticleSuccess(KbArticlesModel model)
        {
            if (model != null)
            {
                handler.Post(() =>
                {
                    if (swipeRefreshLayout.Refreshing)
                    {
                        swipeRefreshLayout.Refreshing = false;
                    }
                    kbarticle = model;
                    if (swipeRefreshLayout.Refreshing)
                    {
                        swipeRefreshLayout.Refreshing = false;
                    }
                    var content = HtmlUtils.ReadHtml(Assets);
                    var body = HtmlUtils.ReplaceHtml(model.Body).Trim('"');
                    txtBody.LoadDataWithBaseURL("file:///android_asset/", content.Replace("#title#", model.Title).Replace("#body#", body), "text/html", "utf-8", null);
                });
            }
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
    }
}