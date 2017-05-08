using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Cnblogs.Droid.Utils;
using Com.Umeng.Analytics;
using Com.Umeng.Socialize;
using System;
using System.Threading.Tasks;

namespace Cnblogs.Droid
{
    [Application]
    public class App : Application
    {
        public App() { }
        public App(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }
        public override void OnCreate()
        {
            base.OnCreate();

            MobclickAgent.SetDebugMode(true);
            MobclickAgent.OpenActivityDurationTrack(false);
            MobclickAgent.SetScenarioType(this, MobclickAgent.EScenarioType.EUmNormal);

            OkHttpUtils.Context = this;
            SQLiteUtils.Instance();

            PlatformConfig.SetWeixin("wxcf8f642a8aa4c630", "76ebc29b4194164aee32eedff2e17900");
            PlatformConfig.SetSinaWeibo("1422675167", "02975c36afd93d3ae983f8da9e596b86", "https://api.weibo.com/oauth2/default.html");
            if (!BuildConfig.Debug)
            {
                AndroidEnvironment.UnhandledExceptionRaiser += AndroidEnvironment_UnhandledExceptionRaiser;
            }
        }

        protected override void Dispose(bool disposing)
        {
            AndroidEnvironment.UnhandledExceptionRaiser -= AndroidEnvironment_UnhandledExceptionRaiser;
            base.Dispose(disposing);
        }
        async void AndroidEnvironment_UnhandledExceptionRaiser(object sender, RaiseThrowableEventArgs ex)
        {
            await Task.Run(() =>
            {
                Looper.Prepare();
                Toast.MakeText(this, Resources.GetString(Resource.String.log_tip), ToastLength.Long).Show();
                Looper.Loop();
            });
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("生产厂商\n");
            sb.Append(Build.Manufacturer).Append("\n\n");
            sb.Append("手机型号\n");
            sb.Append(Build.Model).Append("\n\n");
            sb.Append("系统版本\n");
            sb.Append(Build.VERSION.Release).Append("\n\n");
            sb.Append("异常时间\n");
            sb.Append(DateTime.Now.ToString()).Append("\n\n");
            sb.Append("异常信息\n");
            sb.Append(ex.Exception).Append("\n");
            sb.Append(ex.Exception.Message).Append("\n");
            sb.Append(ex.Exception.StackTrace).Append("\n\n");

            MobclickAgent.ReportError(this, sb.ToString());

            System.Threading.Thread.Sleep(2000);
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
            Java.Lang.JavaSystem.Exit(1);
        }
    }
}