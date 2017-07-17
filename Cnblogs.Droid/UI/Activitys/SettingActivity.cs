using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Cnblogs.Droid.UI.Shareds;
using Cnblogs.Droid.UI.Widgets;
using System;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Cnblogs.Droid.UI.Activitys
{
    [Activity(Label = "@string/setting", LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
    public class SettingActivity : BaseActivity, View.IOnClickListener
    {
        private Handler handler;
        private Toolbar toolbar;
        private UMengSharesWidget sharesWidget;

        protected override int LayoutResource => Resource.Layout.setting;
        public static void Start(Context context)
        {
            context.StartActivity(new Intent(context, typeof(SettingActivity)));
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            handler = new Handler();

            StatusBarCompat.SetOrdinaryToolBar(this);
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            toolbar.SetNavigationIcon(Resource.Drawable.back_24dp);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            toolbar.SetNavigationOnClickListener(this);
            var packageInfo = this.PackageManager.GetPackageInfo(this.PackageName, 0);
            var txtVersion = FindViewById<TextView>(Resource.Id.txtVersion);
            txtVersion.Text = packageInfo.VersionName;
            FindViewById<LinearLayout>(Resource.Id.layoutSurvey).Click += (object sender, EventArgs e) =>
            {
                Intent intent = new Intent(Intent.ActionSendto);
                intent.AddFlags(ActivityFlags.NewTask);
                intent.SetData(Android.Net.Uri.Parse("mailto:" + Resources.GetString(Resource.String.survey_email)));
                if (intent.ResolveActivity(this.PackageManager) != null)
                {
                    intent.PutExtra(Intent.ExtraSubject, "来自 " + packageInfo.PackageName + " - " + packageInfo.VersionName + " 的反馈意见");
                    intent.PutExtra(Intent.ExtraText, "设备信息：Android " + Build.VERSION.Release + " - " + Build.Manufacturer + " - " + Build.Model + "\n（如果涉及隐私请手动删除这个内容）\n\n");
                    StartActivity(intent);
                }
                else
                {
                    Toast.MakeText(this, "系统中没有安装邮件客户端", ToastLength.Short).Show();
                }
            };
            FindViewById<LinearLayout>(Resource.Id.layoutQQ).Click += (object sender, EventArgs e) =>
            {
                sharesWidget.Open("http://shang.qq.com/wpa/qunwpa?idkey=5c281d37638467fb0f411484dcd513b89ba82b58decb8518cc2523b95232dd9b", "博客园第三方APP开发交流群");
            };
            FindViewById<LinearLayout>(Resource.Id.layoutOpenSourceUrl).Click += (object sender, EventArgs e) =>
            {
                sharesWidget.Open(Resources.GetString(Resource.String.open_source_url), "博客园第三方Android客户端，Xamarin App，Material Design风格");
            };
            sharesWidget = new UMengSharesWidget(this);
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

        public void OnClick(View v)
        {
            ActivityCompat.FinishAfterTransition(this);
        }

    }
}