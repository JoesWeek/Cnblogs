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
using Cnblogs.Droid.Model;

namespace Cnblogs.Droid.UI.Shareds
{
    public class SettingShared
    {
        private const string Tag = "Setting";

        private const string KeyWiFi = "wifi";
        public static void SetWiFi(Context context, bool wifi)
        {
            BaseShared.Instance(context, Tag).SetBool(KeyWiFi, wifi);
        }
        public static bool GetWiFi(Context context)
        {
            return BaseShared.Instance(context, Tag).GetBool(KeyWiFi, false);
        }
    }
}