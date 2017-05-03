using Android.Support.V4.App;
using Cnblogs.Droid.UI.Fragments;
using System.Collections.Generic;
using System.Linq;

namespace Cnblogs.Droid.UI.Adapters
{
    public class StatusTabsAdapter : FragmentPagerAdapter
    {
        private List<string> Columns;
        private Android.Support.V4.App.FragmentManager fragmentManager;
        private List<StatusColumnFragment> columnFragmentList = new List<StatusColumnFragment>();
        public StatusTabsAdapter(Android.Support.V4.App.FragmentManager p0, List<string> Columns) : base(p0)
        {
            fragmentManager = p0;
            this.Columns = Columns;
        }
        public override int Count
        {
            get
            {
                return Columns.Count();
            }
        }

        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            var fragment = StatusColumnFragment.NewInstance(position);
            columnFragmentList.Add(fragment);
            return fragment;
        }
        public override Java.Lang.ICharSequence GetPageTitleFormatted(int p0)
        {
            return new Java.Lang.String(Columns[p0]);
        }
        public void NeedRefresh(int position)
        {
            if (columnFragmentList.Count > 0)
                columnFragmentList[position].NeedRefresh();
        }
        public void Refresh(int position)
        {
            if (columnFragmentList.Count > 0)
                columnFragmentList[position].OnRefresh();
        }
    }
}