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
using Android.Text.Style;
using Android.Graphics;
using Java.Lang;
using Android.Graphics.Drawables;

namespace Cnblogs.Droid.UI.Widgets
{
    public class CenteredImageSpan : ImageSpan
    {
        private Drawable drawable;
        public CenteredImageSpan(Context context, int drawableRes) : base(context, drawableRes)
        {
            drawable = context.GetDrawable(drawableRes);
        }
        public override int GetSize(Paint paint, ICharSequence text, int start, int end, Paint.FontMetricsInt fm)
        {
            Rect rect = drawable.Bounds;
            if (fm != null)
            {
                Paint.FontMetricsInt fmPaint = paint.GetFontMetricsInt();
                int fontHeight = fmPaint.Bottom - fmPaint.Top;
                int drHeight = rect.Bottom - rect.Top;

                int top = drHeight / 2 - fontHeight / 4;
                int bottom = drHeight / 2 + fontHeight / 4;

                fm.Ascent = -bottom;
                fm.Top = -bottom;
                fm.Bottom = top;
                fm.Descent = top;
            }
            return rect.Right;
        }
        public override void Draw(Canvas canvas, ICharSequence text, int start, int end, float x, int top, int y, int bottom, Paint paint)
        {
            base.Draw(canvas, text, start, end, x, top, y, bottom, paint);
            canvas.Save();
           var transY = ((bottom - top) - drawable.Bounds.Bottom) / 2 + top;
            canvas.Translate(x, transY);
            drawable.Draw(canvas);
            canvas.Restore();
        }
    }
}