using Android.Support.V4.App;
using Cnblogs.Droid.UI.Fragments;
using System.Collections.Generic;
using System.Linq;

namespace Cnblogs.Droid.UI.Adapters
{
    public class HomeAdapter : FragmentPagerAdapter
    {
        private Android.Support.V4.App.FragmentManager fragmentManager;
        private List<ArticleColumnFragment> columnFragmentList = new List<ArticleColumnFragment>();
        public HomeAdapter(Android.Support.V4.App.FragmentManager p0) : base(p0)
        {
            fragmentManager = p0;
        }
        public override int Count
        {
            get
            {
                return 5;
            }
        }

        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            var fragment = ArticleColumnFragment.NewInstance(position);
            columnFragmentList.Add(fragment);
            return fragment;
        }
        public override Java.Lang.ICharSequence GetPageTitleFormatted(int p0)
        {
            return new Java.Lang.String("");
        }
    }
}