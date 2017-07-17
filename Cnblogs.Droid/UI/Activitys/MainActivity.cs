using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Cnblogs.Droid.Model;
using Cnblogs.Droid.UI.Activitys;
using Cnblogs.Droid.UI.Fragments;
using Cnblogs.Droid.UI.Shareds;
using Cnblogs.Droid.UI.Widgets;
using Cnblogs.Droid.Utils;
using Com.Iflytek.Autoupdate;
using Square.Picasso;
using System;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Cnblogs.Droid
{
    [Activity(LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
    public class MainActivity : BaseActivity, DrawerLayout.IDrawerListener, NavigationView.IOnNavigationItemSelectedListener, View.IOnClickListener, Toolbar.IOnMenuItemClickListener, IFlytekUpdateListener
    {
        private Handler handler;
        private CoordinatorLayout coordinatorLayout;
        private Toolbar toolbar;
        private DrawerLayout drawerLayout;
        private ActionBarDrawerToggle drawerToggle;
        private int lastSelecteID;
        private FragmentManager fm;
        private NewsFragment newsFragment;
        private ArticlesFragment articlesFragment;
        private KbArticlesFragment kbarticlesFragment;
        private StatusFragment statusesFragment;
        private QuestionsFragment questionsFragment;
        private BookmarksFragment bookmarksFragment;
        private NavigationView navigationView;
        private ImageView Avatar;
        private TextView Author;
        private TextView Seniority;
        private TextView txtLogout;
        // 首次按下返回键时间戳
        private DateTime firstBackPressedTime = DateTime.MinValue;
        private IFlytekUpdate updManager;
        private UMengSharesWidget sharesWidget;

        protected override int LayoutResource => Resource.Layout.Main;
        public static void Start(Context context)
        {
            Intent intent = new Intent(context, typeof(MainActivity));
            context.StartActivity(intent);
        }
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            handler = new Handler();
            fm = SupportFragmentManager;

            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            toolbar.SetOnMenuItemClickListener(this);

            coordinatorLayout = FindViewById<CoordinatorLayout>(Resource.Id.coordinatorLayout);

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawerLayout);
            navigationView = FindViewById<NavigationView>(Resource.Id.navigationview);
            var headerLayout = navigationView.InflateHeaderView(Resource.Layout.header_layout);
            navigationView.InflateMenu(Resource.Menu.main);
            navigationView.SetNavigationItemSelectedListener(this);
            Avatar = headerLayout.FindViewById<ImageView>(Resource.Id.headerAvatar);
            Avatar.SetOnClickListener(this);
            Author = headerLayout.FindViewById<TextView>(Resource.Id.headerAuthor);
            Seniority = headerLayout.FindViewById<TextView>(Resource.Id.headerSeniority);
            txtLogout = headerLayout.FindViewById<TextView>(Resource.Id.headerLogout);
            txtLogout.Click += delegate
            {
                LoginUtils.Instance(this).DeleteUser();
                UpdateUserView();
            };

            drawerToggle = new ActionBarDrawerToggle(this, drawerLayout, toolbar, Resource.String.drawer_open, Resource.String.drawer_close);
            drawerLayout.AddDrawerListener(this);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);

            StatusBarCompat.SetDrawerToolbarTabLayout(this, coordinatorLayout);

            UpdateUserView();

            navigationView.Post(() =>
            {
                SwitchNavigationBar(Resource.Id.home);
            });

            updManager = IFlytekUpdate.GetInstance(this.ApplicationContext);
            updManager.SetDebugMode(true);
            updManager.SetParameter(UpdateConstants.ExtraWifionly, "true");
            updManager.SetParameter(UpdateConstants.ExtraNotiIcon, "true");
            updManager.SetParameter(UpdateConstants.ExtraStyle, UpdateConstants.UpdateUiDialog);
            updManager.AutoUpdate(this, this);

            sharesWidget = new UMengSharesWidget(this);

        }

        #region DrawerLayout
        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            drawerToggle.SyncState();
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            return drawerToggle.OnOptionsItemSelected(item) || base.OnOptionsItemSelected(item);
        }
        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            drawerToggle.OnConfigurationChanged(newConfig);
            sharesWidget.Close();
        }
        public void OnDrawerClosed(View drawerView)
        {
            if (!LoginUtils.Instance(this).GetLoginStatus())
            {
                switch (lastSelecteID)
                {
                    case Resource.Id.bookmarks:
                        bookmarksFragment.OnRefresh();
                        break;
                    case Resource.Id.Statuses:
                        statusesFragment.SetTabSelected();
                        break;
                    case Resource.Id.Question:
                        questionsFragment.SetTabSelected();
                        break;
                }
            }
        }
        public void OnDrawerOpened(View drawerView)
        {
            UpdateUserView();
        }
        public void OnDrawerSlide(View drawerView, float slideOffset)
        {
        }
        public void OnDrawerStateChanged(int newState)
        {
        }
        #endregion

        public bool OnNavigationItemSelected(IMenuItem menuItem)
        {
            if (lastSelecteID != menuItem.ItemId)
            {
                switch (menuItem.ItemId)
                {
                    case Resource.Id.Setting:
                        SettingActivity.Start(this);
                        break;
                    case Resource.Id.Share:
                        sharesWidget.Open(Resources.GetString(Resource.String.open_source_url), Resources.GetString(Resource.String.share_title), Resource.Mipmap.ic_launcher);
                        break;
                    default:
                        SwitchNavigationBar(menuItem.ItemId);
                        toolbar.Title = menuItem.ToString();
                        break;
                }
            }
            drawerLayout.CloseDrawer(GravityCompat.Start);
            return true;
        }
        public bool OnMenuItemClick(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.search)
            {
                SearchActivity.Start(this);
            }
            else
            {
                //是否登录
                var user = UserShared.GetAccessToken(this);
                if (user.access_token == "" || user.RefreshTime.AddSeconds(user.expires_in) < DateTime.Now)
                {
                    ShowLogin();
                }
                else
                {
                    switch (lastSelecteID)
                    {
                        case Resource.Id.Statuses:
                            StartActivityForResult(new Intent(this, typeof(StatusAddActivity)), (int)RequestCode.StatusAddCode);
                            break;
                        case Resource.Id.Question:
                            StartActivityForResult(new Intent(this, typeof(QuestionAddActivity)), (int)RequestCode.QuestionAddCode);
                            break;
                        case Resource.Id.bookmarks:
                            StartActivityForResult(new Intent(this, typeof(BookmarkAddActivity)), (int)RequestCode.BookmarkAddCode);
                            break;
                    }
                }
            }
            return true;
        }

        #region SwitchNavigationBar
        public void SwitchNavigationBar(int id)
        {
            if (lastSelecteID > 0)
            {
                HideNavigationBar(lastSelecteID);
            }
            switch (id)
            {
                case Resource.Id.home:
                    SwitchHome();
                    break;
                case Resource.Id.NewsItems:
                    SwitchNews();
                    break;
                case Resource.Id.KbArticles:
                    SwitchKbArticles();
                    break;
                case Resource.Id.Statuses:
                    SwitchStatuses();
                    break;
                case Resource.Id.Question:
                    SwitchQuestions();
                    break;
                case Resource.Id.bookmarks:
                    SwitchBookmarks();
                    break;
                default:
                    Toast.MakeText(this, "很抱歉，还未实现这个功能。", ToastLength.Long).Show();
                    break;
            }
            lastSelecteID = id;
            InvalidateOptionsMenu();
        }
        public void HideNavigationBar(int id)
        {
            switch (id)
            {
                case Resource.Id.home:
                    HideHome();
                    break;
                case Resource.Id.NewsItems:
                    HideNews();
                    break;
                case Resource.Id.KbArticles:
                    HideKbArticles();
                    break;
                case Resource.Id.Statuses:
                    HideStatuses();
                    break;
                case Resource.Id.Question:
                    HideQuestions();
                    break;
                case Resource.Id.bookmarks:
                    HideBookmarks();
                    break;
                default:
                    break;
            }
        }
        public void SwitchHome()
        {
            FragmentTransaction transaction = fm.BeginTransaction();
            if (articlesFragment == null)
            {
                articlesFragment = new ArticlesFragment();
                transaction.Add(Resource.Id.frameContent, articlesFragment).Commit();
            }
            else
            {
                transaction.Show(articlesFragment).Commit();
            }
        }
        public void SwitchNews()
        {
            FragmentTransaction transaction = fm.BeginTransaction();
            if (newsFragment == null)
            {
                newsFragment = new NewsFragment();
                transaction.Add(Resource.Id.frameContent, newsFragment).Commit();
            }
            else
            {
                transaction.Show(newsFragment).Commit();
            }
        }
        public void SwitchKbArticles()
        {
            FragmentTransaction transaction = fm.BeginTransaction();
            if (kbarticlesFragment == null)
            {
                kbarticlesFragment = new KbArticlesFragment();
                transaction.Add(Resource.Id.frameContent, kbarticlesFragment).Commit();
            }
            else
            {
                transaction.Show(kbarticlesFragment).Commit();
            }
        }
        public void SwitchStatuses()
        {
            FragmentTransaction transaction = fm.BeginTransaction();
            if (statusesFragment == null)
            {
                statusesFragment = new StatusFragment();
                transaction.Add(Resource.Id.frameContent, statusesFragment).Commit();
            }
            else
            {
                transaction.Show(statusesFragment).Commit();
            }
        }
        public void SwitchQuestions()
        {
            FragmentTransaction transaction = fm.BeginTransaction();
            if (questionsFragment == null)
            {
                questionsFragment = new QuestionsFragment();
                transaction.Add(Resource.Id.frameContent, questionsFragment).Commit();
            }
            else
            {
                transaction.Show(questionsFragment).Commit();
            }
        }
        public void SwitchBookmarks()
        {
            FragmentTransaction transaction = fm.BeginTransaction();
            if (bookmarksFragment == null)
            {
                bookmarksFragment = new BookmarksFragment();
                transaction.Add(Resource.Id.frameContent, bookmarksFragment).Commit();
            }
            else
            {
                transaction.Show(bookmarksFragment).Commit();
            }
        }
        public void HideHome()
        {
            FragmentTransaction transaction = fm.BeginTransaction();
            if (articlesFragment != null)
            {
                transaction.Hide(articlesFragment).Commit();
            }
        }
        public void HideNews()
        {
            FragmentTransaction transaction = fm.BeginTransaction();
            if (newsFragment != null)
            {
                transaction.Hide(newsFragment).Commit();
            }
        }
        public void HideKbArticles()
        {
            FragmentTransaction transaction = fm.BeginTransaction();
            if (kbarticlesFragment != null)
            {
                transaction.Hide(kbarticlesFragment).Commit();
            }
        }
        public void HideStatuses()
        {
            FragmentTransaction transaction = fm.BeginTransaction();
            if (statusesFragment != null)
            {
                transaction.Hide(statusesFragment).Commit();
            }
        }
        public void HideQuestions()
        {
            FragmentTransaction transaction = fm.BeginTransaction();
            if (questionsFragment != null)
            {
                transaction.Hide(questionsFragment).Commit();
            }
        }
        public void HideBookmarks()
        {
            FragmentTransaction transaction = fm.BeginTransaction();
            if (bookmarksFragment != null)
            {
                transaction.Hide(bookmarksFragment).Commit();
            }
        }
        #endregion

        public async void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.headerAvatar:
                    var token = UserShared.GetAccessToken(this);
                    if (token == null || token.access_token == "")
                    {
                        StartActivityForResult(new Intent(this, typeof(LoginActivity)), (int)RequestCode.LoginCode);
                    }
                    else
                    {
                        var user = await SQLiteUtils.Instance().QueryUser();
                        if (string.IsNullOrEmpty(user.BlogApp))
                        {
                            Toast.MakeText(this, "未开通博客", ToastLength.Short).Show();
                        }
                        else
                        {
                            BlogActivity.Start(this, user.BlogApp);
                        }
                    }
                    break;
            }
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok)
            {
                switch (requestCode)
                {
                    case (int)RequestCode.LoginCode:
                        UpdateUserView();
                        break;
                    case (int)RequestCode.StatusAddCode:
                        statusesFragment.SetTabSelected(0);
                        break;
                    case (int)RequestCode.QuestionAddCode:
                        questionsFragment.SetTabSelected(0);
                        break;
                    case (int)RequestCode.BookmarkAddCode:
                        bookmarksFragment.OnRefresh();
                        break;
                }
            }
        }
        public async void UpdateUserView()
        {
            if (LoginUtils.Instance(this).GetLoginStatus())
            {
                var user = await LoginUtils.Instance(this).GetUser();
                Author.Text = user.DisplayName;
                Seniority.Text = Resources.GetString(Resource.String.seniority) + "：" + user.Seniority;
                txtLogout.Visibility = ViewStates.Visible;
                Picasso.With(this)
                            .Load(user.Avatar)
                            .Placeholder(Resource.Drawable.placeholder)
                            .Error(Resource.Drawable.placeholder)
                            .Transform(new CircleTransform())
                            .Into(Avatar);
            }
            else
            {
                Author.Text = Resources.GetString(Resource.String.need_login);
                Seniority.Text = "";
                txtLogout.Visibility = ViewStates.Gone;
                Picasso.With(this)
                            .Load(Resource.Drawable.placeholder)
                            .Placeholder(Resource.Drawable.placeholder)
                            .Error(Resource.Drawable.placeholder)
                            .Transform(new CircleTransform())
                            .Into(Avatar);
            }
        }
        public override void OnBackPressed()
        {
            if (drawerLayout.IsDrawerOpen(GravityCompat.Start))
            {
                drawerLayout.CloseDrawer(GravityCompat.Start);
            }
            else if (lastSelecteID != Resource.Id.home)
            {
                navigationView.SetCheckedItem(Resource.Id.home);
                SwitchNavigationBar(Resource.Id.home);
                toolbar.Title = Resources.GetString(Resource.String.bolgs);
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
        public async void ShowLogin()
        {
            //未登录或清空Token失效
            //清空Token
            UserShared.Update(this, new AccessToken());
            await SQLiteUtils.Instance().DeleteUserAll();
            Android.Support.V7.App.AlertDialog.Builder dialog = new Android.Support.V7.App.AlertDialog.Builder(this);
            dialog.SetMessage(Resources.GetString(Resource.String.need_login_tip));
            dialog.SetPositiveButton(Resources.GetString(Resource.String.confirm), delegate
            {
                StartActivityForResult(new Intent(this, typeof(LoginActivity)), (int)RequestCode.LoginCode);
                dialog.Dispose();
            });
            dialog.SetNegativeButton(Resources.GetString(Resource.String.cancel), delegate
            {
                dialog.Dispose();
            });
            dialog.Create().Show();
        }
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
    }
}

