using Android.Content;
using Android.Webkit;
using System;

namespace Cnblogs.Droid.UI.Widgets
{
    public class WebViewJSInterface : Java.Lang.Object
    {
        Context context { get; set; }

        public WebViewJSInterface(Context context)
        {
            this.context = context;
        }

        [Java.Interop.Export]
        [JavascriptInterface]
        public void OpenImage(string srcs, int index)
        {
            CallFromPageReceived?.Invoke(this, new CallFromPageReceivedEventArgs
            {
                Result = srcs,
                Index = index
            });
        }
        [Java.Interop.Export]
        [JavascriptInterface]
        public void OpenHref(string href)
        {
            CallFromPageReceived?.Invoke(this, new CallFromPageReceivedEventArgs
            {
                Result = href,
                Index = 0
            });
        }

        public event EventHandler<CallFromPageReceivedEventArgs> CallFromPageReceived;
        public class CallFromPageReceivedEventArgs : EventArgs
        {
            public string Result { get; set; }
            public int Index { get; set; }
        }
    }
}