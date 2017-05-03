using Android.App;
using Com.Umeng.Socialize.Media;

namespace Cnblogs.Droid
{
    [Activity(Name = "com.android.cnblogs.WBShareActivity", ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class WBShareActivity : WBShareCallBackActivity
    {
    }
}