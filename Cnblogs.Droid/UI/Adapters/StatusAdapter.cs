using Android.Support.V7.Widget;
using Android.Text;
using Android.Views;
using Android.Widget;
using Cnblogs.Droid.Model;
using Cnblogs.Droid.UI.Activitys;
using Cnblogs.Droid.UI.Listeners;
using Cnblogs.Droid.UI.Widgets;
using Cnblogs.Droid.Utils;
using Com.Umeng.Analytics;
using Square.Picasso;
using System;

namespace Cnblogs.Droid.UI.Adapters
{
    public class StatusAdapter : BaseAdapter.BaseQuickAdapter<StatusModel>, View.IOnClickListener
    {
        public IOnDeleteClickListener OnDeleteClickListener;
        public UserModel User { get; set; }
        public StatusAdapter() : base(Resource.Layout.fragment_statuses_column_item)
        {
        }
        protected override void ConvertAsync(RecyclerView.ViewHolder holder, StatusModel model)
        {
            var baseHolder = holder as BaseAdapter.BaseViewHolder;
            var linearLayout = (baseHolder.GetView(Resource.Id.linearLayout) as LinearLayout);
            linearLayout.Tag = model.Id;
            linearLayout.SetOnClickListener(this);
            var txtDesc = (baseHolder.GetView(Resource.Id.txtDesc) as TextView);
            var txtParentCommentContent = (baseHolder.GetView(Resource.Id.txtParentCommentContent) as TextView);
            (baseHolder.GetView(Resource.Id.txtUserName) as TextView).Text = Html.FromHtml(model.UserDisplayName).ToString();
            (baseHolder.GetView(Resource.Id.txtPostdate) as TextView).Text = DateTimeUtils.CommonTime(Convert.ToDateTime(model.DateAdded));
            var commentCount = (baseHolder.GetView(Resource.Id.txtCommentCount) as TextView);
            if (model.CommentCount > 0)
                commentCount.Text = " " + model.CommentCount;
            else
                commentCount.Text = "";
            var content = (baseHolder.GetView(Resource.Id.txtContent) as TextView);
            (baseHolder.GetView(Resource.Id.progressBar) as ProgressBar).Visibility = ViewStates.Gone;
            var imgUserUrl = (baseHolder.GetView(Resource.Id.imgUserUrl) as ImageView);
            var imgPrivate = (baseHolder.GetView(Resource.Id.imgPrivate) as ImageView);
            var imgDelete = (baseHolder.GetView(Resource.Id.imgDelete) as ImageButton);
            imgDelete.Tag = model.Id;
            imgDelete.SetOnClickListener(this);
            if (User != null && User.UserId == model.UserGuid && model.Id > 0)
            {
                imgDelete.Visibility = ViewStates.Visible;
            }
            else
            {
                imgDelete.Visibility = ViewStates.Gone;
            }
            if (model.StatusId > 0)
            {
                commentCount.Visibility = ViewStates.Gone;
                txtDesc.Text = "回复了您的评论";
                txtParentCommentContent.Visibility = ViewStates.Visible;
                txtParentCommentContent.SetText(Html.FromHtml(model.ParentCommentContent), TextView.BufferType.Spannable);
            }
            else
            {
                commentCount.Visibility = ViewStates.Visible;
                txtParentCommentContent.Visibility = ViewStates.Gone;
                txtDesc.Text = "发布了";
                imgDelete.Tag = model.Id;
                linearLayout.Tag = model.Id;
            }
            if (model.IsPrivate)
            {
                imgPrivate.Visibility = ViewStates.Visible;
                try
                {
                    Picasso.With(context)
                                .Load(Resource.Drawable.isPrivate)
                                .Into(imgPrivate);
                }
                catch
                {

                }
            }
            else
            {
                imgPrivate.Visibility = ViewStates.Gone;
            }
            if (model.IsLucky)
            {
                var Content = "\u3000" + model.Content + " ";
                var spanText = new SpannableString(Html.FromHtml(Content));
                try
                {
                    var imageSpan = new CenteredImageSpan(context, Resource.Drawable.luckystar);
                    spanText.SetSpan(imageSpan, spanText.Length() - 1, spanText.Length(), SpanTypes.InclusiveExclusive);
                }
                catch (Exception ex)
                {
                    MobclickAgent.ReportError(context, ex.Message);
                    spanText.SetSpan(context, spanText.Length() - 1, spanText.Length(), SpanTypes.InclusiveExclusive);
                }

                content.SetText(spanText, TextView.BufferType.Spannable);
            }
            else
            {
                content.SetText(Html.FromHtml("\u3000" + model.Content), TextView.BufferType.Spannable);
            }
            try
            {
                Picasso.With(context)
                            .Load(model.UserIconUrl)
                            .Placeholder(Resource.Drawable.placeholder)
                            .Error(Resource.Drawable.placeholder)
                            .Transform(new CircleTransform())
                            .Into(imgUserUrl);
            }
            catch
            {

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
                        StatusActivity.Start(context, Convert.ToInt32(v.Tag.ToString()));
                    }
                    break;
            }
        }
    }
}