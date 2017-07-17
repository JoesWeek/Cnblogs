using Android.Support.V7.Widget;
using Android.Text;
using Android.Views;
using Android.Widget;
using Cnblogs.Droid.Model;
using Cnblogs.Droid.UI.Activitys;
using Cnblogs.Droid.UI.Listeners;
using Cnblogs.Droid.UI.Widgets;
using Cnblogs.Droid.Utils;
using Square.Picasso;
using System;
using System.Linq;

namespace Cnblogs.Droid.UI.Adapters
{
    public class QuestionAnswersAdapter : BaseAdapter.BaseQuickAdapter<QuestionAnswersModel>, View.IOnClickListener
    {
        public UserModel User { get; set; }
        public IOnDeleteClickListener OnDeleteClickListener { get; set; }
        public QuestionAnswersAdapter() : base(Resource.Layout.question_answers_item)
        {
        }
        protected override async void ConvertAsync(RecyclerView.ViewHolder holder, QuestionAnswersModel model)
        {
            var baseHolder = holder as BaseAdapter.BaseViewHolder;
            var linearLayout = (baseHolder.GetView(Resource.Id.linearLayout) as LinearLayout);
            linearLayout.Tag = model.AnswerID;
            linearLayout.SetOnClickListener(this);
            (baseHolder.GetView(Resource.Id.txtDateAdded) as TextView).Text = DateTimeUtils.CommonTime(Convert.ToDateTime(model.DateAdded));
            (baseHolder.GetView(Resource.Id.txtContent) as TextView).SetText(HtmlUtils.GetHtml(model.Answer), TextView.BufferType.Spannable);
            (baseHolder.GetView(Resource.Id.progressBar) as ProgressBar).Visibility = ViewStates.Gone;
            var imgUserUrl = (baseHolder.GetView(Resource.Id.imgIconName) as ImageView);
            var imgDelete = (baseHolder.GetView(Resource.Id.imgDelete) as ImageButton);
            imgDelete.Tag = model.AnswerID;
            imgDelete.SetOnClickListener(this);
            (baseHolder.GetView(Resource.Id.progressBar) as ProgressBar).Visibility = ViewStates.Gone;
            var txtCommentCount = (baseHolder.GetView(Resource.Id.txtCommentCount) as TextView);
            if (model.CommentCounts > 0)
                txtCommentCount.Text = " " + model.CommentCounts;
            else
                txtCommentCount.Text = "";
            var txtIsBest = (baseHolder.GetView(Resource.Id.txtIsBest) as TextView);
            txtIsBest.Selected = false;
            if (model.IsBest)
            {
                txtIsBest.Visibility = ViewStates.Visible;
            }
            else
            {
                txtIsBest.Visibility = ViewStates.Gone;
            }
            if (User != null && User.UserId == model.AnswerUserInfo.UCUserID)
            {
                User.Score = model.AnswerUserInfo.QScore;
                await SQLiteUtils.Instance().UpdateUser(User);
                imgDelete.Visibility = ViewStates.Visible;
            }
            else
            {
                imgDelete.Visibility = ViewStates.Gone;
            }
            if (model.AnswerUserInfo != null && model.AnswerUserInfo.UserID > 0)
            {
                (baseHolder.GetView(Resource.Id.txtUserName) as TextView).Text = HtmlUtils.GetHtml(model.AnswerUserInfo.UserName).ToString();
                (baseHolder.GetView(Resource.Id.txtScore) as TextView).Text = HtmlUtils.GetScoreName(model.AnswerUserInfo.QScore) + " ¡¤ " + model.AnswerUserInfo.QScore + "Ô°¶¹ ¡¤ ";
                try
                {
                    Picasso.With(context)
                                .Load("https://pic.cnblogs.com/face/" + model.AnswerUserInfo.IconName)
                                .Placeholder(Resource.Drawable.placeholder)
                                .Error(Resource.Drawable.placeholder)
                                .Transform(new CircleTransform())
                                .Into(imgUserUrl);
                }
                catch
                {

                }
            }
        }
        public new void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.imgDelete:
                    if (OnDeleteClickListener != null)
                    {
                        OnDeleteClickListener.OnDelete(Convert.ToInt32(v.Tag.ToString()));
                    }
                    break;
                default:
                    if (v.Tag != null)
                    {
                        var answer = GetData().Where(d => d.AnswerID == Convert.ToInt32(v.Tag.ToString())).FirstOrDefault();
                        QuestionCommentsActivity.Start(context, answer.Qid, answer.AnswerID);
                    }
                    break;
            }
        }
    }
}