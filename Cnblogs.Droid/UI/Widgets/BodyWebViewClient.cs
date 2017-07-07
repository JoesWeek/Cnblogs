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
using Android.Webkit;
using Android.Util;
using Android.Graphics;
using Com.Umeng.Analytics;

namespace Cnblogs.Droid.UI.Widgets
{
    public class BodyWebViewClient : WebViewClient
    {
        private volatile static BodyWebViewClient singleton;

        public static BodyWebViewClient With(Context context)
        {
            if (singleton == null)
            {
                lock (typeof(BodyWebViewClient))
                {
                    if (singleton == null)
                    {
                        singleton = new BodyWebViewClient(context);
                    }
                }
            }
            return singleton;
        }

        private Context context;

        private BodyWebViewClient(Context context)
        {
            this.context = context.ApplicationContext;
        }
        public override WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request)
        {
            return base.ShouldInterceptRequest(view, request);
        }

        public override void OnPageFinished(WebView view, string url)
        {
            base.OnPageFinished(view, url);
            view.LoadUrl("javascript:(function(){" +
                                    "var imgs = document.getElementsByTagName(\"img\"); " +
                                    "var srcs=new Array();" +
                                    "for(var i=0;i<imgs.length;i++)  " +
                                    "{" +
                                    "    srcs.push(imgs[i].src);" +
                                    "    imgs[i].onclick=function()  " +
                                    "    {  " +
                                    "        openlistner.OpenImage(srcs.toString(),i);  " +
                                    "    };  " +
                                    "};" +
                                    "})()");
        }
        [Obsolete]
        public override bool ShouldOverrideUrlLoading(WebView view, string url)
        {
            try
            {
                Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(url));
                intent.SetClassName("com.android.browser", "com.android.browser.BrowserActivity");
                intent.AddFlags(ActivityFlags.NewTask);
                context.StartActivity(intent);
            }
            catch (Exception ex)
            {
                MobclickAgent.ReportError(context, ex.Message);
                Toast.MakeText(context, "系统中没有安装浏览器客户端", ToastLength.Short).Show();
            }
            return true;
        }
    }
}