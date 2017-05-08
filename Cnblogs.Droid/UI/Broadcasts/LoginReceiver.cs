using Android.Content;
using System;

namespace Cnblogs.Droid.UI.Broadcasts
{
    [BroadcastReceiver]
    public class LoginReceiver : BroadcastReceiver
    {
        public Action Login;
        public Action Logout;
        public override void OnReceive(Context context, Intent intent)
        {
            string receiver = intent.GetStringExtra("loginreceiver");
            switch (receiver)
            {
                case "login":
                    Login?.Invoke();
                    break;
                case "logout":
                    Logout?.Invoke();
                    break;
            }
        }
    }
}