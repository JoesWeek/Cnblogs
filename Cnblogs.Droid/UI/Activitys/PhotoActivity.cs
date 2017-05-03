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
using Android.Support.V4.View;
using Cnblogs.Droid.UI.Widgets;
using Cnblogs.Droid.UI.Adapters;

namespace Cnblogs.Droid.UI.Activitys
{
    [Activity(Theme = "@style/PhotoTheme")]
    public class PhotoActivity : BaseActivity, ViewPager.IOnPageChangeListener
    {
        private ViewPager viewpager;
        private TextView txtIndex;
        private TextView txtCount;
        protected override int LayoutResource => Resource.Layout.photo;
        public static void Start(Context context, string[] urls, int index)
        {
            Intent intent = new Intent(context, typeof(PhotoActivity));
            intent.PutExtra("urls", urls);
            intent.PutExtra("index", index);
            context.StartActivity(intent);
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var urls = Intent.GetStringArrayExtra("urls").ToList();
            urls.RemoveAt(urls.Count - 1);
            var index = Intent.GetIntExtra("index", 0);
            var count = urls.Count;

            viewpager = FindViewById<HackyViewPager>(Resource.Id.viewpager);
            viewpager.OffscreenPageLimit = count;

            txtIndex = FindViewById<TextView>(Resource.Id.index);
            txtCount = FindViewById<TextView>(Resource.Id.count);
            txtCount.Text = count.ToString();

            viewpager.Adapter = new PhotoAdapter(urls.ToList());
            viewpager.AddOnPageChangeListener(this);
        }
        public void OnPageScrollStateChanged(int state)
        {
        }

        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {
        }

        public void OnPageSelected(int position)
        {
            txtIndex.Text = (position + 1).ToString();
        }
    }
}