
using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Cnblogs.Droid.UI.Widgets
{
    public class StatusBarCompat
    {
        private static View statusBarView;

        public IntPtr Handle => throw new NotImplementedException();

        /// <summary>
        /// ºÚµ•–Õ◊¥Ã¨¿∏(ToolBar)
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="coordinatorLayout"></param>
        public static void SetOrdinaryToolBar(Activity activity)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                activity.Window.SetStatusBarColor(activity.Resources.GetColor(Resource.Color.primary_dark));
            }
            else if (Build.VERSION.SdkInt == BuildVersionCodes.Kitkat)
            {
                SetKKStatusBar(activity, Resource.Color.primary_dark);
            }
        }
        /// <summary>
        /// DrawerLayout+ToolBar+TabLayout◊¥Ã¨¿∏(ToolBarø……ÏÀı)
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="coordinatorLayout"></param>
        public static void SetDrawerToolbarTabLayout(Activity activity, CoordinatorLayout coordinatorLayout)
        {
            if (Build.VERSION.SdkInt == BuildVersionCodes.Kitkat)
            {
                ViewGroup contentLayout = activity.FindViewById<ViewGroup>(Android.Resource.Id.Content);
                contentLayout.GetChildAt(0).SetFitsSystemWindows(false);
                coordinatorLayout.SetFitsSystemWindows(true);
                SetKKStatusBar(activity, Resource.Color.statusBar);
            }
        }
        /// <summary>
        /// CollapsingToolbarLayout◊¥Ã¨¿∏
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="coordinatorLayout"></param>
        /// <param name="appBarLayout"></param>
        /// <param name="imageView"></param>
        /// <param name="toolbar"></param>
        public static void SetCollapsingToolbar(Activity activity, CoordinatorLayout coordinatorLayout, AppBarLayout appBarLayout, View view, Toolbar toolbar)
        {
            if (Build.VERSION.SdkInt == BuildVersionCodes.Kitkat)
            {
                coordinatorLayout.SetFitsSystemWindows(false);
                appBarLayout.SetFitsSystemWindows(false);
                view.SetFitsSystemWindows(false);
                toolbar.SetFitsSystemWindows(true);
                CollapsingToolbarLayout.LayoutParams lp = (CollapsingToolbarLayout.LayoutParams)toolbar.LayoutParameters;
                lp.Height = (int)(GetStatusBarHeight(activity) +
                        activity.Resources.GetDimension(Resource.Dimension.abc_action_bar_default_height_material));
                toolbar.LayoutParameters = lp;
                SetKKStatusBar(activity, Resource.Color.statusBar);
                SetCollapsingToolbarStatus(appBarLayout);
            }
        }
        /// <summary>
        /// Android4.4…œCollapsingToolbar’€µ˛ ±statusBarœ‘ æ∫Õ“˛≤ÿ
        /// </summary>
        /// <param name="appBarLayout"></param>
        private static void SetCollapsingToolbarStatus(AppBarLayout appBarLayout)
        {
            ViewCompat.SetAlpha(statusBarView, 1);
            appBarLayout.AddOnOffsetChangedListener(new AppBarLayoutOffsetChanged());
        }
        private static void SetKKStatusBar(Activity activity, int statusBarColor)
        {
            ViewGroup contentView = activity.FindViewById<ViewGroup>(Android.Resource.Id.Content);
            statusBarView = contentView.GetChildAt(0);
            //∏ƒ±‰—’…´ ±±‹√‚÷ÿ∏¥ÃÌº”statusBarView
            if (statusBarView != null && statusBarView.MeasuredHeight == GetStatusBarHeight(activity))
            {
                statusBarView.SetBackgroundColor(activity.Resources.GetColor(statusBarColor));
                return;
            }
            statusBarView = new View(activity);
            ViewGroup.LayoutParams lp = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                    GetStatusBarHeight(activity));
            statusBarView.SetBackgroundColor(activity.Resources.GetColor(statusBarColor));
        }

        private static int GetStatusBarHeight(Context context)
        {
            int resourceId = context.Resources.GetIdentifier("status_bar_height", "dimen", "android");
            return context.Resources.GetDimensionPixelSize(resourceId);
        }
        private class AppBarLayoutOffsetChanged : Java.Lang.Object, AppBarLayout.IOnOffsetChangedListener
        {
            public void OnOffsetChanged(AppBarLayout appBarLayout, int verticalOffset)
            {
                int maxScroll = appBarLayout.TotalScrollRange;
                float percentage = (float)Math.Abs(verticalOffset) / (float)maxScroll;
                ViewCompat.SetAlpha(statusBarView, percentage);
            }
        }
    }
}