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
using Cnblogs.Droid.UI.Widgets;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using SearchView = Android.Support.V7.Widget.SearchView;
using Android.Support.V4.App;
using Com.Materialsearchview;
using Com.Materialsearchview.Utils;
using Android.Support.V4.Content;
using Android.Support.V4.View;
using Android.Support.Design.Widget;
using Cnblogs.Droid.UI.Adapters;

namespace Cnblogs.Droid.UI.Activitys
{
    [Activity(Label = "", LaunchMode = Android.Content.PM.LaunchMode.SingleTask, Theme = "@style/SearchStyle")]
    public class SearchActivity : BaseActivity, View.IOnClickListener, TabLayout.IOnTabSelectedListener, SearchView.IOnQueryTextListener
    {
        private Toolbar toolbar;
        private SearchView searchView;
        private ViewPager viewPager;
        private SearchTabsAdapter adapter;
        private string oldSearchText = "";
        protected override int LayoutResource => Resource.Layout.search;
        public static void Start(Context context)
        {
            context.StartActivity(new Intent(context, typeof(SearchActivity)));
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            StatusBarCompat.SetOrdinaryToolBar(this);

            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            toolbar.SetNavigationIcon(Resource.Drawable.back_24dp);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            toolbar.SetNavigationOnClickListener(this);
            searchView = FindViewById<SearchView>(Resource.Id.searchView);
            searchView.SetIconifiedByDefault(true);
            searchView.OnActionViewExpanded();
            searchView.SetOnQueryTextListener(this);

            viewPager = FindViewById<ViewPager>(Resource.Id.viewPager);
            viewPager.OffscreenPageLimit = 4;

            TabLayout tabs = FindViewById<TabLayout>(Resource.Id.tabLayout);
            adapter = new SearchTabsAdapter(this.SupportFragmentManager, Resources.GetStringArray(Resource.Array.SearchTabs).ToList());

            viewPager.Adapter = adapter;
            tabs.SetupWithViewPager(viewPager);
            tabs.AddOnTabSelectedListener(this);
        }
        public void OnTabReselected(TabLayout.Tab tab)
        {
        }

        public void OnTabSelected(TabLayout.Tab tab)
        {
            viewPager.CurrentItem = tab.Position;
            adapter.OnRefresh(tab.Position);
        }

        public void OnTabUnselected(TabLayout.Tab tab)
        {
        }
        public void OnClick(View v)
        {
            ActivityCompat.FinishAfterTransition(this);
        }

        public bool OnQueryTextChange(string newText)
        {
            if (newText != oldSearchText)
            {
                adapter.SearchText = newText;
                adapter.OnRefresh(viewPager.CurrentItem);
            }
            return true;
        }

        public bool OnQueryTextSubmit(string query)
        {
            return true;
        }
    }
}