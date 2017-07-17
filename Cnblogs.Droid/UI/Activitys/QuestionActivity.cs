using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Widget;
using Android.Text;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Cnblogs.Droid.Model;
using Cnblogs.Droid.Presenter;
using Cnblogs.Droid.UI.Shareds;
using Cnblogs.Droid.UI.Views;
using Cnblogs.Droid.UI.Widgets;
using Cnblogs.Droid.Utils;
using Com.Umeng.Socialize;
using Com.Umeng.Socialize.Bean;
using Com.Umeng.Socialize.Media;
using Com.Umeng.Socialize.Shareboard;
using Com.Umeng.Socialize.Utils;
using Square.Picasso;
using System;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Cnblogs.Droid.UI.Activitys
{
    [Activity(Label = "@string/question_details", LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
    public class QuestionActivity : BaseActivity, IQuestionView, View.IOnClickListener, SwipeRefreshLayout.IOnRefreshListener, Toolbar.IOnMenuItemClickListener
    {
        private int Id;
        private IQuestionPresenter questionPresenter;
        private Handler handler;

        private Toolbar toolbar;
        private SwipeRefreshLayout swipeRefreshLayout;
        private TextView txtTitle;
        private ImageView imgIconName;
        private TextView txtUserName;
        private TextView txtScore;
        private TextView txtDateAdded;
        private TextView txtGold;
        private TextView txtDealFlag;
        private WebView txtBody;
        private TextView txtTag;
        private TextView txtDigg;
        private TextView txtRead;
        private TextView txtComments;
        private TextView txtBookmark;
        private DateTime lastDatetime;

        private QuestionsModel question;
        private UMengSharesWidget sharesWidget;
        protected override int LayoutResource => Resource.Layout.question;
        public static void Start(Context context, int id)
        {
            Intent intent = new Intent(context, typeof(QuestionActivity));
            intent.PutExtra("id", id);
            context.StartActivity(intent);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Id = Intent.GetIntExtra("id", 0);
            questionPresenter = new QuestionPresenter(this);
            handler = new Handler();

            StatusBarCompat.SetOrdinaryToolBar(this);
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            toolbar.SetNavigationIcon(Resource.Drawable.back_24dp);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            toolbar.SetNavigationOnClickListener(this);
            toolbar.SetOnMenuItemClickListener(this);

            swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            swipeRefreshLayout.SetColorSchemeResources(Resource.Color.primary);
            swipeRefreshLayout.SetOnRefreshListener(this);

            txtTitle = FindViewById<TextView>(Resource.Id.txtTitle);
            imgIconName = FindViewById<ImageView>(Resource.Id.imgIconName);
            txtUserName = FindViewById<TextView>(Resource.Id.txtUserName);
            txtScore = FindViewById<TextView>(Resource.Id.txtScore);
            txtDateAdded = FindViewById<TextView>(Resource.Id.txtDateAdded);
            txtGold = FindViewById<TextView>(Resource.Id.txtGold);
            txtDealFlag = FindViewById<TextView>(Resource.Id.txtDealFlag);
            txtTag = FindViewById<TextView>(Resource.Id.txtTag);
            this.txtDealFlag.Selected = true;
            txtBody = FindViewById<WebView>(Resource.Id.txtBody);
            txtBody.Settings.JavaScriptEnabled = true;
            txtBody.Settings.DomStorageEnabled = true;
            txtBody.Settings.LoadsImagesAutomatically = true;
            txtBody.Settings.DefaultTextEncodingName = "utf-8";
            txtBody.SetWebViewClient(BodyWebViewClient.With(this));
            txtBody.ScrollBarStyle = ScrollbarStyles.InsideOverlay;
            txtBody.Settings.SetSupportZoom(false);
            txtBody.Settings.BuiltInZoomControls = false;
            txtBody.Settings.CacheMode = CacheModes.CacheElseNetwork;
            txtBody.Settings.SetLayoutAlgorithm(WebSettings.LayoutAlgorithm.SingleColumn);
            var jsInterface = new WebViewJSInterface(this);
            txtBody.AddJavascriptInterface(jsInterface, "openlistner");
            jsInterface.CallFromPageReceived += delegate (object sender, WebViewJSInterface.CallFromPageReceivedEventArgs e)
            {
                PhotoActivity.Start(this, e.Result.Split(','), e.Index);
            };
            txtDigg = FindViewById<TextView>(Resource.Id.txtDigg);
            txtRead = FindViewById<TextView>(Resource.Id.txtRead);
            txtBookmark = FindViewById<TextView>(Resource.Id.txtBookmark);
            txtComments = FindViewById<TextView>(Resource.Id.txtComments);
            txtComments.Text = Resources.GetString(Resource.String.answer);

            (txtComments.Parent as FrameLayout).Click += delegate
            {
                if (question != null && question.Qid > 0)
                    QuestionAnswersActivity.Start(this, question.Qid);
            };
            (txtBookmark.Parent as FrameLayout).Click += delegate
            {
                if (question != null && question.Qid > 0)
                {
                    var linkurl = "https://q.cnblogs.com/q/" + question.Qid + "/";
                    var title = question.Title + "_≤©Œ _≤©øÕ‘∞";
                    BookmarkAddActivity.Start(this, linkurl, title, true);
                }
            };
            swipeRefreshLayout.Post(async () =>
            {
                await questionPresenter.GetClientQuestion(Id);
            });
            sharesWidget = new UMengSharesWidget(this);
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.share, menu);
            return true;
        }
        public bool OnMenuItemClick(IMenuItem item)
        {
            if (question != null)
            {
                sharesWidget.Open("https://q.cnblogs.com/q/" + question.Qid + "/", question.Title);
            }
            return true;
        }
        /// <summary>
        /// ∆¡ƒª∫· ˙∆¡«–ªª ±±‹√‚≥ˆœ÷window leakµƒŒ Ã‚
        /// </summary>
        /// <param name="newConfig"></param>
        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            sharesWidget.Close();
        }
        protected override void OnResume()
        {
            base.OnResume();
            if (lastDatetime.AddMinutes(15) < DateTime.Now)
            {
                OnRefresh();
            }
        }
        public async void OnRefresh()
        {
            swipeRefreshLayout.Refreshing = true;
            await questionPresenter.GetServiceQuestion(TokenShared.GetAccessToken(this), Id);
        }
        public void GetClientQuestionSuccess(QuestionsModel model)
        {
            question = model;
            if (question != null)
                BindView();
        }
        public void GetServiceQuestionFail(string msg)
        {
            handler.Post(() =>
            {
                if (swipeRefreshLayout.Refreshing)
                {
                    swipeRefreshLayout.Refreshing = false;
                }
                Toast.MakeText(this, msg, ToastLength.Short).Show();
            });
        }
        public void GetServiceQuestionSuccess(QuestionsModel model)
        {
            handler.Post(() =>
            {
                if (swipeRefreshLayout.Refreshing)
                {
                    swipeRefreshLayout.Refreshing = false;
                }
                question = model;
                lastDatetime = DateTime.Now;
                BindView();
            });
        }
        private void BindView()
        {
            handler.Post(() =>
            {
                txtTitle.Text = question.Title;
                txtDateAdded.Text = DateTimeUtils.CommonTime(question.DateAdded);
                if (question.Award > 0)
                {
                    this.txtGold.Text = question.Award.ToString();
                    this.txtGold.Visibility = ViewStates.Visible;
                }
                else
                {
                    this.txtGold.Visibility = ViewStates.Gone;
                }
                if (question.DealFlag == 1)
                {
                    this.txtDealFlag.Text = Resources.GetString(Resource.String.question_dealflag_1);
                    this.txtDealFlag.Selected = false;
                }
                else if (question.DealFlag == -1)
                {
                    this.txtDealFlag.Text = Resources.GetString(Resource.String.question_dealflag_2);
                    this.txtDealFlag.Selected = true;
                }
                else
                {
                    this.txtDealFlag.Text = Resources.GetString(Resource.String.question_dealflag_0);
                    this.txtDealFlag.Selected = true;
                }
                if (question.Tags != null && question.Tags.Length > 0)
                {
                    txtTag.Visibility = ViewStates.Visible;
                    txtTag.Text = " " + question.Tags.Replace(',', ' ');
                }
                else
                {
                    txtTag.Visibility = ViewStates.Gone;
                }
                if (question.DiggCount > 0)
                    txtDigg.Text = question.DiggCount.ToString();
                if (question.ViewCount > 0)
                    txtRead.Text = question.ViewCount.ToString();
                if (question.AnswerCount > 0)
                {
                    txtComments.Text = question.AnswerCount.ToString();
                }

                if (question.Content == null || question.Content == "")
                {
                    OnRefresh();
                }
                else
                {
                    var content = HtmlUtils.ReadHtml(Assets);
                    var body = HtmlUtils.ReplaceHtml(question.Content).Trim('"');
                    if (question.Addition != null)
                    {
                        body += " <h2>Œ Ã‚≤π≥‰£∫</h2>" + question.Addition.Content;
                    }
                    txtBody.LoadDataWithBaseURL("file:///android_asset/", content.Replace("#title#", "").Replace("#body#", body), "text/html", "utf-8", null);
                }
                if (question.QuestionUserInfo != null && question.QuestionUserInfo.UserID > 0)
                {
                    txtUserName.Text = HtmlUtils.GetHtml(question.QuestionUserInfo.UserName).ToString();
                    txtScore.Text = HtmlUtils.GetScoreName(question.QuestionUserInfo.QScore) + " °§ " + question.QuestionUserInfo.QScore + "‘∞∂π °§ ";
                    try
                    {
                        Picasso.With(this)
                                    .Load("https://pic.cnblogs.com/face/" + question.QuestionUserInfo.IconName)
                                    .Placeholder(Resource.Drawable.placeholder)
                                    .Error(Resource.Drawable.placeholder)
                                    .Transform(new CircleTransform())
                                    .Into(imgIconName);
                    }
                    catch (Exception)
                    {

                    }
                }
            });
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            UMShareAPI.Get(this).OnActivityResult(requestCode, (int)resultCode, data);
        }
        public void OnClick(View v)
        {
            ActivityCompat.FinishAfterTransition(this);
        }
    }
}