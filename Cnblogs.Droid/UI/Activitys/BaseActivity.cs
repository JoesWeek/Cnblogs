using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Com.Umeng.Analytics;

namespace Cnblogs.Droid.UI.Activitys
{
    [Activity]
    public abstract class BaseActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(LayoutResource);

        }
        protected abstract int LayoutResource
        {
            get;
        }
        protected override void OnResume()
        {
            base.OnResume();
            MobclickAgent.OnResume(this);
        }
        protected override void OnPause()
        {
            base.OnPause();
            MobclickAgent.OnPause(this);
        }

    }
}