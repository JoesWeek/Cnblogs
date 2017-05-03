using Android.Content;
using Android.Widget;
using Com.Umeng.Socialize;
using Com.Umeng.Socialize.Bean;
using Java.Lang;

namespace Cnblogs.Droid.UI.Widgets
{
    public class UMengCustomShare : Java.Lang.Object, IUMShareListener
    {
        private Context context;
        public UMengCustomShare(Context context)
        {
            this.context = context;
        }
        public void OnCancel(SHARE_MEDIA platform)
        {
            Toast.MakeText(context, "分享取消了", ToastLength.Short).Show();
        }

        public void OnError(SHARE_MEDIA platform, Throwable p1)
        {
            Toast.MakeText(context, "分享失败了", ToastLength.Short).Show();
        }

        public void OnResult(SHARE_MEDIA platform)
        {
            if (platform == SHARE_MEDIA.WeixinFavorite)
            {
                Toast.MakeText(context, "收藏成功", ToastLength.Short).Show();
            }
            else
            {
                Toast.MakeText(context, "分享成功", ToastLength.Short).Show();
            }
        }

        public void OnStart(SHARE_MEDIA platform)
        {
        }
    }
}