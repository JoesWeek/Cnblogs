using Android.Content;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using System;

namespace Cnblogs.Droid.UI.Widgets
{
    public class HackyViewPager : ViewPager
    {
        public HackyViewPager(Context context) : base(context)
        {

        }

        public HackyViewPager(Context context, IAttributeSet attrs) : base(context, attrs)
        {

        }
        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            try
            {
                return base.OnInterceptTouchEvent(ev);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}