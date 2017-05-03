using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Views;
using Cnblogs.Droid.Model;
using Cnblogs.Droid.UI.Adapters;
using System.Collections.Generic;
using System.Linq;
using Fragment = Android.Support.V4.App.Fragment;

namespace Cnblogs.Droid.UI.Fragments
{
    public class ArticlesFragment : Fragment, TabLayout.IOnTabSelectedListener
    {
        private View view;
        private ViewPager viewPager;
        private ArticlesTabsAdapter adapter;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            return view = inflater.Inflate(Resource.Layout.fragment_home, container, false);
        }
        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            viewPager = view.FindViewById<ViewPager>(Resource.Id.viewPager);
            viewPager.OffscreenPageLimit = 2;

            TabLayout tabs = view.FindViewById<TabLayout>(Resource.Id.tabs);
            adapter = new ArticlesTabsAdapter(this.ChildFragmentManager, this.Activity.Resources.GetStringArray(Resource.Array.ArticlesTabs).ToList());

            viewPager.Adapter = adapter;
            tabs.TabMode = TabLayout.GravityCenter;
            tabs.SetupWithViewPager(viewPager);
            tabs.AddOnTabSelectedListener(this);
            tabs.Post(() =>
            {
                adapter.OnRefresh(0);
            });
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
    }
}