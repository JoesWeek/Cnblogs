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

namespace Cnblogs.Droid.UI.Behaviors
{
    public enum ScrollDirection
    {
        ScrollDirectionUp = 1,
        ScrollDirectionDown = -1,
        ScrollNone = 0
    }
}