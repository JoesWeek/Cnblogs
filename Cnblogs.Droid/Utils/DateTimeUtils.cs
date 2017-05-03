using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cnblogs.Droid.Utils
{
    public class DateTimeUtils
    {
        public static string CommonTime(DateTime dt)
        {
            TimeSpan span = DateTime.Now.Subtract(dt);
            if (span.Days > 0)
            {
                var month = (DateTime.Now.Year - dt.Year) * 12 + DateTime.Now.Month - dt.Month;

                if (month >= 12)
                {
                    return string.Format("{0}年前", (month / 12).ToString());
                }
                else if (month > 0)
                {
                    return string.Format("{0}月前", month.ToString());
                }
                else
                {
                    return string.Format("{0}天前", span.Days.ToString());
                }
            }
            else
            {
                if (span.Hours > 0)
                {
                    return string.Format("{0}小时前", span.Hours.ToString());
                }
                else
                {
                    if (span.Minutes > 0)
                    {
                        return string.Format("{0}分钟前", span.Minutes.ToString());
                    }
                    else
                    {
                        if (span.Seconds > 5)
                        {
                            return string.Format("{0}秒前", span.Seconds.ToString());
                        }
                        else
                        {
                            return "刚刚";
                        }
                    }
                }
            }

        }

        /// <summary>
        /// 计算周一的日期
        /// </summary>
        /// <param name="someDate"></param>
        /// <returns></returns>
        public static DateTime GetMondayDate(DateTime someDate)
        {
            int i = someDate.DayOfWeek - DayOfWeek.Monday;
            if (i == -1) i = 6;// i值 > = 0 ，因为枚举原因，Sunday排在最前，此时Sunday-Monday=-1，必须+7=6。 
            TimeSpan ts = new TimeSpan(i, 0, 0, 0);
            return someDate.Subtract(ts);
        }
    }
}