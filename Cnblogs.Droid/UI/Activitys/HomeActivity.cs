using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using BottomNavigationBar;
using BottomNavigationBar.Listeners;
using Cnblogs.Droid.UI.Fragments;
using Cnblogs.Droid.UI.Widgets;
using Com.Iflytek.Autoupdate;
using System;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using SearchView = Android.Support.V7.Widget.SearchView;
using Android.Support.V4.View;
using Cnblogs.Droid.UI.Adapters;
using System.Linq;
using Android.Views.InputMethods;

namespace Cnblogs.Droid.UI.Activitys
{
    [Activity( LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
    public class HomeActivity : BaseActivity, IOnTabClickListener, IFlytekUpdateListener, View.IOnClickListener, View.IOnFocusChangeListener, SearchView.IOnQueryTextListener
    {
        private Handler handler;
        private Toolbar toolbar;
        private SearchView searchView;
        private BottomBar bottomBar;
        private NewsFragment newsFragment;
        private ArticlesFragment articlesFragment;
        private KbArticlesFragment kbarticlesFragment;
        private StatusFragment statusesFragment;
        private QuestionsFragment questionsFragment;
        private BookmarksFragment bookmarksFragment;
        private SearchFragment searchFragment;
        private int lastSelecteID;
        private DateTime firstBackPressedTime = DateTime.MinValue;
        private IFlytekUpdate updManager;
        private UMengSharesWidget sharesWidget;
        private string oldSearchText = "";
        private bool isSearchOpen = false;

        protected override int LayoutResource => Resource.Layout.home;
        public static void Start(Context context)
        {
            Intent intent = new Intent(context, typeof(HomeActivity));
            context.StartActivity(intent);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            handler = new Handler();

            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            toolbar.SetNavigationIcon(Resource.Drawable.search_dark_24dp);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            toolbar.SetNavigationOnClickListener(this);

            searchView = FindViewById<SearchView>(Resource.Id.searchView);
            searchView.Focusable = false;
            searchView.SetIconifiedByDefault(true);
            searchView.OnActionViewExpanded();
            searchView.SetOnQueryTextListener(this);

            var searchAutoComplete = (SearchView.SearchAutoComplete)searchView.FindViewById(Resource.Id.search_src_text);
            searchAutoComplete.SetTextColor(GetColorStateList(Resource.Color.title_color));
            searchAutoComplete.SetTextSize(Android.Util.ComplexUnitType.Sp, 14);
            searchAutoComplete.SetHintTextColor(GetColorStateList(Resource.Color.subtitle_color));
            searchAutoComplete.OnFocusChangeListener = this;

            StatusBarCompat.SetOrdinaryToolBar(this);

            bottomBar = BottomBar.AttachShy(FindViewById<CoordinatorLayout>(Resource.Id.coordinatorLayout), FindViewById(Resource.Id.frameContent), savedInstanceState);

            bottomBar.UseFixedMode();
            bottomBar.SetItems(Resource.Menu.bottombar_menu);
            bottomBar.SetOnTabClickListener(this);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);

            bottomBar.OnSaveInstanceState(outState);
        }
        protected override void OnResume()
        {
            base.OnResume();
            searchView.Post(() =>
            {
                if (searchView.IsIconfiedByDefault)
                {
                    searchView.ClearFocus();
                }
            });
        }
        public void OnTabReSelected(int position)
        {
        }

        public void OnTabSelected(int position)
        {
            SwitchNavigationBar(position);
        }

        #region SwitchNavigationBar
        public void SwitchNavigationBar(int position)
        {
            if (lastSelecteID > -1)
            {
                HideNavigationBar(lastSelecteID);
            }
            switch (position)
            {
                case -1:
                    SwitchSearch();
                    break;
                case 0:
                    SwitchHome();
                    break;
                case 1:
                    SwitchNews();
                    break;
                case 2:
                    SwitchKbArticles();
                    break;
                case 3:
                    SwitchStatuses();
                    break;
                case 4:
                    SwitchQuestions();
                    break;
                case 5:
                    SwitchBookmarks();
                    break;
            }
            if (position > -1)
            {
                lastSelecteID = position;
            }
        }
        public void HideNavigationBar(int position)
        {
            switch (position)
            {
                case -1:
                    HideSearch();
                    break;
                case 0:
                    HideHome();
                    break;
                case 1:
                    HideNews();
                    break;
                case 2:
                    HideKbArticles();
                    break;
                case 3:
                    HideStatuses();
                    break;
                case 4:
                    HideQuestions();
                    break;
                case 5:
                    HideBookmarks();
                    break;
            }
        }
        public void SwitchHome()
        {
            FragmentTransaction transaction = SupportFragmentManager.BeginTransaction();
            if (articlesFragment == null)
            {
                articlesFragment = new ArticlesFragment();
                transaction.Add(Resource.Id.frameContent, articlesFragment).CommitNowAllowingStateLoss();
            }
            else
            {
                transaction.Show(articlesFragment).CommitNowAllowingStateLoss();
            }
        }
        public void SwitchNews()
        {
            FragmentTransaction transaction = SupportFragmentManager.BeginTransaction();
            if (newsFragment == null)
            {
                newsFragment = new NewsFragment();
                transaction.Add(Resource.Id.frameContent, newsFragment).CommitNowAllowingStateLoss();
            }
            else
            {
                transaction.Show(newsFragment).CommitNowAllowingStateLoss();
            }
        }
        public void SwitchKbArticles()
        {
            FragmentTransaction transaction = SupportFragmentManager.BeginTransaction();
            if (kbarticlesFragment == null)
            {
                kbarticlesFragment = new KbArticlesFragment();
                transaction.Add(Resource.Id.frameContent, kbarticlesFragment).CommitNowAllowingStateLoss();
            }
            else
            {
                transaction.Show(kbarticlesFragment).CommitNowAllowingStateLoss();
            }
        }
        public void SwitchStatuses()
        {
            FragmentTransaction transaction = SupportFragmentManager.BeginTransaction();
            if (statusesFragment == null)
            {
                statusesFragment = new StatusFragment();
                transaction.Add(Resource.Id.frameContent, statusesFragment).CommitNowAllowingStateLoss();
            }
            else
            {
                transaction.Show(statusesFragment).CommitNowAllowingStateLoss();
            }
        }
        public void SwitchQuestions()
        {
            FragmentTransaction transaction = SupportFragmentManager.BeginTransaction();
            if (questionsFragment == null)
            {
                questionsFragment = new QuestionsFragment();
                transaction.Add(Resource.Id.frameContent, questionsFragment).CommitNowAllowingStateLoss();
            }
            else
            {
                transaction.Show(questionsFragment).CommitNowAllowingStateLoss();
            }
        }
        public void SwitchBookmarks()
        {
            FragmentTransaction transaction = SupportFragmentManager.BeginTransaction();
            if (bookmarksFragment == null)
            {
                bookmarksFragment = new BookmarksFragment();
                transaction.Add(Resource.Id.frameContent, bookmarksFragment).CommitNowAllowingStateLoss();
            }
            else
            {
                transaction.Show(bookmarksFragment).CommitNowAllowingStateLoss();
            }
        }
        public void SwitchSearch()
        {
            FragmentTransaction transaction = SupportFragmentManager.BeginTransaction();
            if (searchFragment == null)
            {
                searchFragment = new SearchFragment();
                transaction.Add(Resource.Id.frameContent, searchFragment).CommitNowAllowingStateLoss();
            }
            else
            {
                transaction.Show(searchFragment).CommitNowAllowingStateLoss();
            }
        }
        public void HideHome()
        {
            FragmentTransaction transaction = SupportFragmentManager.BeginTransaction();
            if (articlesFragment != null)
            {
                transaction.Hide(articlesFragment).CommitNowAllowingStateLoss();
            }
        }
        public void HideNews()
        {
            FragmentTransaction transaction = SupportFragmentManager.BeginTransaction();
            if (newsFragment != null)
            {
                transaction.Hide(newsFragment).CommitNowAllowingStateLoss();
            }
        }
        public void HideKbArticles()
        {
            FragmentTransaction transaction = SupportFragmentManager.BeginTransaction();
            if (kbarticlesFragment != null)
            {
                transaction.Hide(kbarticlesFragment).CommitNowAllowingStateLoss();
            }
        }
        public void HideStatuses()
        {
            FragmentTransaction transaction = SupportFragmentManager.BeginTransaction();
            if (statusesFragment != null)
            {
                transaction.Hide(statusesFragment).CommitNowAllowingStateLoss();
            }
        }
        public void HideQuestions()
        {
            FragmentTransaction transaction = SupportFragmentManager.BeginTransaction();
            if (questionsFragment != null)
            {
                transaction.Hide(questionsFragment).CommitNowAllowingStateLoss();
            }
        }
        public void HideBookmarks()
        {
            FragmentTransaction transaction = SupportFragmentManager.BeginTransaction();
            if (bookmarksFragment != null)
            {
                transaction.Hide(bookmarksFragment).CommitNowAllowingStateLoss();
            }
        }
        public void HideSearch()
        {
            FragmentTransaction transaction = SupportFragmentManager.BeginTransaction();
            if (searchFragment != null)
            {
                transaction.Hide(searchFragment).CommitNowAllowingStateLoss();
            }
        }
        #endregion

        public void OnResult(int errorcode, UpdateInfo result)
        {
            handler.Post(() =>
            {
                if (errorcode == UpdateErrorCode.Ok && result != null)
                {
                    if (result.UpdateType == UpdateType.NoNeed)
                    {
                        return;
                    }
                    updManager.ShowUpdateInfo(this, result);
                }
            });
        }

        public override void OnBackPressed()
        {
            if (isSearchOpen)
            {
                HideSearchView();
            }
            else if (lastSelecteID != Resource.Id.home)
            {
                SwitchNavigationBar(0);
            }
            else
            {
                if (firstBackPressedTime == DateTime.MinValue)
                {
                    Toast.MakeText(this, "再按一次退出程序", ToastLength.Short).Show();
                    firstBackPressedTime = DateTime.Now;
                }
                else if (firstBackPressedTime.AddSeconds(2) < DateTime.Now)
                {
                    Android.Support.V4.App.ActivityCompat.FinishAfterTransition(this);
                }
                else
                {
                    base.OnBackPressed();
                }
            }
        }

        public void OnClick(View v)
        {
            if (isSearchOpen)
            {
                HideSearchView();
            }
        }
        public bool OnQueryTextChange(string newText)
        {
            if (newText != oldSearchText)
            {
                searchFragment.SetSearchText(newText);
            }
            return true;
        }

        public bool OnQueryTextSubmit(string query)
        {
            return true;
        }

        public void OnFocusChange(View v, bool hasFocus)
        {
            if (hasFocus)
            {
                ShowSearchView();
            }
        }
        private void ShowSearchView()
        {
            toolbar.SetNavigationIcon(Resource.Drawable.back_dark_24dp);
            SwitchNavigationBar(-1);
            isSearchOpen = true;
        }
        private void HideSearchView()
        {
            searchView.ClearFocus();
            HideSearch();
            SwitchNavigationBar(lastSelecteID);
            toolbar.SetNavigationIcon(Resource.Drawable.search_dark_24dp);
            isSearchOpen = false;

            InputMethodManager imm = (InputMethodManager)searchView.Context.GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(searchView.WindowToken, 0);
        }
    }
}