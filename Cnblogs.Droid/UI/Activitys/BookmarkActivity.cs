using Android.Content;
using Android.Widget;
using Cnblogs.Droid.UI.Widgets;
using Cnblogs.Droid.Utils;

namespace Cnblogs.Droid.UI.Activitys
{
    public class BookmarkActivity
    {
        public async static void Start(Context context, int id)
        {
            var bookmark = await SQLiteUtils.Instance().QueryBookmark(id);
            if (bookmark != null)
            {
                try
                {
                    Intent intent = new Intent(Intent.ActionView);
                    intent.SetData(Android.Net.Uri.Parse(bookmark.LinkUrl));
                    context.StartActivity(intent);
                }
                catch (System.Exception)
                {
                    Toast.MakeText(context, "系统中没有安装浏览器客户端", ToastLength.Short).Show();
                }
            }
        }
    }
}