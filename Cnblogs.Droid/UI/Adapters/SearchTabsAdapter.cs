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
using Android.Support.V4.App;
using Cnblogs.Droid.UI.Fragments;

namespace Cnblogs.Droid.UI.Adapters
{
    public class SearchTabsAdapter : FragmentPagerAdapter
    {
        private List<string> Columns;
        private Android.Support.V4.App.FragmentManager fragmentManager;
        private List<SearchColumnFragment> columnFragmentList = new List<SearchColumnFragment>();
        public string SearchText { get; set; }
        public SearchTabsAdapter(Android.Support.V4.App.FragmentManager p0, List<string> Columns) : base(p0)
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
            var fragment = SearchColumnFragment.NewInstance(position);
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
                columnFragmentList[position].Refresh(SearchText);
        }
    }
}