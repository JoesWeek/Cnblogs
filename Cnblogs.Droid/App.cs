using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Cnblogs.Droid.Utils;
using Com.Umeng.Socialize;

namespace Cnblogs.Droid
{
    [Application]
    public class App : Application
    {
        public App()
        {

        }
        public App(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {

        }
        public override void OnCreate()
        {
            base.OnCreate();
            SQLiteUtils.Instance();

            PlatformConfig.SetWeixin("wxcf8f642a8aa4c630", "76ebc29b4194164aee32eedff2e17900");
            PlatformConfig.SetSinaWeibo("1422675167", "02975c36afd93d3ae983f8da9e596b86", "https://api.weibo.com/oauth2/default.html");
        }
    }
}