using Android.Support.V7.Widget;
using Android.Text;
using Android.Views;
using Android.Widget;
using Cnblogs.Droid.Model;
using Cnblogs.Droid.UI.Listeners;
using Cnblogs.Droid.UI.Widgets;
using Cnblogs.Droid.Utils;
using Square.Picasso;
using System;

namespace Cnblogs.Droid.UI.Adapters
{
    public class QuestionCommentsAdapter : BaseAdapter.BaseQuickAdapter<QuestionCommentsModel>, View.IOnClickListener
    {
        public UserModel User { get; set; }
        public IOnDeleteClickListener OnDeleteClickListener { get; set; }
        public QuestionCommentsAdapter() : base(Resource.Layout.question_answers_comments_item)
        {
        }
        protected override async void ConvertAsync(RecyclerView.ViewHolder holder, QuestionCommentsModel model)
        {
            var baseHolder = holder as BaseAdapter.BaseViewHolder;
            var itemview = baseHolder.GetConvertView();
            itemview.Tag = model.CommentID;
            (baseHolder.GetView(Resource.Id.txtDateAdded) as TextView).Text = DateTimeUtils.CommonTime(Convert.ToDateTime(model.DateAdded));
            (baseHolder.GetView(Resource.Id.txtContent) as TextView).SetText(Html.FromHtml(model.Content), TextView.BufferType.Spannable);
            var imgUserUrl = (baseHolder.GetView(Resource.Id.imgIconName) as ImageView);
            var imgDelete = (baseHolder.GetView(Resource.Id.imgDelete) as ImageButton);
            (baseHolder.GetView(Resource.Id.progressBar) as ProgressBar).Visibility = ViewStates.Gone;

            imgDelete.Tag = model.CommentID;
            imgDelete.SetOnClickListener(this);
            if (User != null && User.UserId == model.PostUserInfo.UCUserID)
            {
                User.Score = model.PostUserInfo.QScore;
                await SQLiteUtils.Instance().UpdateUser(User);
                imgDelete.Visibility = ViewStates.Visible;
            }
            else
            {
                imgDelete.Visibility = ViewStates.Gone;
            }
            if (model.PostUserInfo != null && model.PostUserInfo.UserID > 0)
            {
                (baseHolder.GetView(Resource.Id.txtUserName) as TextView).Text = Html.FromHtml(model.PostUserInfo.UserName).ToString();
                (baseHolder.GetView(Resource.Id.txtScore) as TextView).Text = HtmlUtils.GetScoreName(model.PostUserInfo.QScore) + " ¡¤ " + model.PostUserInfo.QScore + "Ô°¶¹";
                try
                {
                    Picasso.With(context)
                                .Load("https://pic.cnblogs.com/face/" + model.PostUserInfo.IconName)
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
            }
        }
    }
}