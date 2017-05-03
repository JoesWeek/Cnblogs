using Android.Support.V4.App;
using Cnblogs.Droid.UI.Fragments;
using System.Collections.Generic;
using System.Linq;

namespace Cnblogs.Droid.UI.Adapters
{
    public class QuestionsTabsAdapter : FragmentPagerAdapter
    {
        private List<string> Columns;
        private Android.Support.V4.App.FragmentManager fragmentManager;
        private List<QuestionsColumnFragment> columnFragmentList = new List<QuestionsColumnFragment>();
        public QuestionsTabsAdapter(Android.Support.V4.App.FragmentManager p0, List<string> Columns) : base(p0)
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
            var fragment = QuestionsColumnFragment.NewInstance(position);
            columnFragmentList.Add(fragment);
            return fragment;
        }
        public override Java.Lang.ICharSequence GetPageTitleFormatted(int p0)
        {
            return new Java.Lang.String(Columns[p0]);
        }
        public void OnRefresh(int position)
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