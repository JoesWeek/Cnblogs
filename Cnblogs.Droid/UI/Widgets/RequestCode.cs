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

namespace Cnblogs.Droid.UI.Widgets
{
    public enum RequestCode
    {
        LoginCode = 1024,
        StatusAddCode,
        QuestionAddCode,
        QuestionAnswersAddCode,
        BookmarkEditCode,
        BookmarkAddCode
    }
}