using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Cnblogs.Droid.UI.Widgets;
using Cnblogs.Droid.Utils;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Cnblogs.Droid.UI.Activitys
{
    [Activity(Label = "@string/articlehome", LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
    public class BookmarkActivity : BaseActivity, View.IOnClickListener
    {
        private int Id;
        private Handler handler;

        private Toolbar toolbar;
        private ProgressBar progressBar;
        private WebView txtBody;
        protected override int LayoutResource => Resource.Layout.bookmark;
        public async static void Start(Context context, int id)
        {
            var bookmark = await SQLiteUtils.Instance().QueryBookmark(id);
            if (bookmark != null)
            {
                Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(bookmark.LinkUrl));
                intent.SetClassName("com.android.browser", "com.android.browser.BrowserActivity");
                intent.AddFlags(ActivityFlags.NewTask);
                context.StartActivity(intent);
            }
            //Intent intent = new Intent(context, typeof(BookmarkActivity));
            //intent.PutExtra("id", id);
            //context.StartActivity(intent);
        }

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Id = Intent.GetIntExtra("id", 0);
            handler = new Handler();

            StatusBarCompat.SetOrdinaryToolBar(this);
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            toolbar.SetNavigationIcon(Resource.Drawable.back_24dp);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            toolbar.SetNavigationOnClickListener(this);

            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);

            txtBody = FindViewById<WebView>(Resource.Id.txtBody);
            txtBody.Settings.JavaScriptEnabled = true;
            txtBody.Settings.DomStorageEnabled = true;
            txtBody.Settings.LoadsImagesAutomatically = true;
            txtBody.Settings.DefaultTextEncodingName = "utf-8";
            txtBody.SetWebViewClient(BodyWebViewClient.With(this));
            txtBody.ScrollBarStyle = ScrollbarStyles.InsideOverlay;
            txtBody.Settings.SetSupportZoom(true);
            txtBody.Settings.BuiltInZoomControls = false;
            txtBody.Settings.CacheMode = CacheModes.CacheElseNetwork;
            txtBody.Settings.SetLayoutAlgorithm(WebSettings.LayoutAlgorithm.SingleColumn);
            txtBody.SetWebChromeClient(new ProgressBarWebChromeClient(progressBar));

            var bookmark = await SQLiteUtils.Instance().QueryBookmark(Id);
            if (bookmark != null && bookmark.FromCNBlogs)
            {
                txtBody.LoadUrl(bookmark.LinkUrl);
            }
        }
        public void OnClick(View v)
        {
            this.Finish();
        }
        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back)
            {
                if (txtBody.CanGoBack())
                {
                    txtBody.GoBack();
                    return true;
                }
                else
                {
                    this.Finish();
                }
            }
            return base.OnKeyDown(keyCode, e);
        }
        public class ProgressBarWebChromeClient : WebChromeClient
        {
            private ProgressBar progressBar;
            public ProgressBarWebChromeClient(ProgressBar progressBar)
            {
                this.progressBar = progressBar;
            }
            public override void OnProgressChanged(WebView view, int newProgress)
            {
                if (newProgress == 100)
                {
                    progressBar.Visibility = ViewStates.Gone;
                }
                else
                {
                    progressBar.Visibility = ViewStates.Visible;
                    progressBar.Progress = newProgress;
                }
            }
        }
    }
}