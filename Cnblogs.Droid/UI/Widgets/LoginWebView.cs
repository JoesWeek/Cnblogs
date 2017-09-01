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
using Android.Graphics.Drawables;
using Android.Graphics;

namespace Cnblogs.Droid.UI.Widgets
{
    public class LoginWebView : WebView
    {
        public const int ProgressbarID = 100001;
        public LoginWebView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            ProgressBar progressbar = new ProgressBar(context, null,Android.Resource.Attribute.ProgressBarStyleHorizontal);
            progressbar.Id = ProgressbarID;
            progressbar.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, 10);
            progressbar.SetBackgroundColor(Color.Red);

            Drawable drawable = context.Resources.GetDrawable(Resource.Drawable.progressbar_bg);
            progressbar.ProgressDrawable = drawable;
            AddView(progressbar);
        }
    }
}