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
using Android.Support.Design.Widget;
using Android.Util;
using Android.Support.V4.View;
using Android.Support.V4.View.Animation;
using Android.Views.Animations;


namespace Cnblogs.Droid.UI.Behaviors
{
    [Register("Cnblogs/Droid/UI/Behaviors/BottomBehavior")]
    public class BottomBehavior : CoordinatorLayout.Behavior
    {
        private int totalDy = 0;

        private ScrollDirection _scrollDirection = ScrollDirection.ScrollNone;

        private static readonly IInterpolator InInterpolator = new LinearOutSlowInInterpolator();
        private ViewPropertyAnimatorCompat translationAnimator;
        private readonly int _defaultOffset = 0;
        private bool _scrollingEnabled = true;
        public BottomBehavior() : base() { }

        public BottomBehavior(Context context, IAttributeSet attrs) : base(context, attrs) { }


        private void HandleDirection(View child, ScrollDirection scrollDirection)
        {
            if (!_scrollingEnabled)
                return;
            if (scrollDirection == ScrollDirection.ScrollDirectionDown)
            {
                AnimateOffset(child, _defaultOffset);
            }
            else if (scrollDirection == ScrollDirection.ScrollDirectionUp)
            {
                AnimateOffset(child, child.Height + _defaultOffset);
            }
        }

        private void AnimateOffset(View child, int offset)
        {
            EnsureOrCancelAnimator(child);
            translationAnimator.TranslationY(offset).Start();
        }

        private void EnsureOrCancelAnimator(View child)
        {
            if (translationAnimator == null)
            {
                translationAnimator = ViewCompat.Animate(child);
                translationAnimator.SetDuration(300);
                translationAnimator.SetInterpolator(InInterpolator);
            }
            else
            {
                translationAnimator.Cancel();
            }
        }

        #region
        public override bool OnStartNestedScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View directTargetChild, View target, int nestedScrollAxes)
        {
            return (nestedScrollAxes & (int)ScrollAxis.Vertical) != 0;
        }

        public override void OnNestedScrollAccepted(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View directTargetChild, View target, int nestedScrollAxes)
        {
            base.OnNestedScrollAccepted(coordinatorLayout, child, directTargetChild, target, nestedScrollAxes);
        }

        public override void OnStopNestedScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target)
        {
            base.OnStopNestedScroll(coordinatorLayout, child, target);
        }

        public override void OnNestedScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target, int dxConsumed, int dyConsumed, int dxUnconsumed, int dyUnconsumed)
        {
            base.OnNestedScroll(coordinatorLayout, child, target, dxConsumed, dyConsumed, dxUnconsumed, dyUnconsumed);
        }

        public override void OnNestedPreScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target, int dx, int dy, int[] consumed)
        {
            base.OnNestedPreScroll(coordinatorLayout, child, target, dx, dy, consumed);
            if (dy > 0 && totalDy < 0)
            {
                totalDy = 0;
                _scrollDirection = ScrollDirection.ScrollDirectionUp;
            }
            else if (dy < 0 && totalDy >= 0)
            {
                totalDy = 0;
                _scrollDirection = ScrollDirection.ScrollDirectionDown;
            }
            totalDy += dy;
            HandleDirection(child as View, _scrollDirection);
        }

        public override bool OnNestedFling(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target, float velocityX, float velocityY, bool consumed)
        {
            base.OnNestedFling(coordinatorLayout, child, target, velocityX, velocityY, consumed);
            _scrollDirection = velocityY > 0 ? ScrollDirection.ScrollDirectionUp : ScrollDirection.ScrollDirectionDown;
            HandleDirection(child as View, _scrollDirection);
            return true;
        }

        public override bool OnNestedPreFling(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target, float velocityX, float velocityY)
        {
            return base.OnNestedPreFling(coordinatorLayout, child, target, velocityX, velocityY);
        }

        public override WindowInsetsCompat OnApplyWindowInsets(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, WindowInsetsCompat insets)
        {
            return base.OnApplyWindowInsets(coordinatorLayout, child, insets);
        }
        #endregion
    }
}