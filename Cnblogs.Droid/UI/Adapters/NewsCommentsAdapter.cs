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
    public class NewsCommentsAdapter : BaseAdapter.BaseQuickAdapter<NewsCommentModel>, View.IOnClickListener
    {
        public UserModel User { get; set; }
        public IOnDeleteClickListener OnDeleteClickListener { get; set; }
        public NewsCommentsAdapter() : base(Resource.Layout.news_comment_item)
        {
        }
        protected override void ConvertAsync(RecyclerView.ViewHolder holder, NewsCommentModel model)
        {
            var baseHolder = holder as BaseAdapter.BaseViewHolder;
            var itemview = baseHolder.GetConvertView();
            itemview.Tag = model.CommentID.ToString();
            (baseHolder.GetView(Resource.Id.username) as TextView).Text = HtmlUtils.GetHtml(model.UserName).ToString();
            (baseHolder.GetView(Resource.Id.comment) as TextView).Text = HtmlUtils.GetHtml(model.CommentContent).ToString();
            (baseHolder.GetView(Resource.Id.dateAdded) as TextView).Text = DateTimeUtils.CommonTime(model.DateAdded);
            (baseHolder.GetView(Resource.Id.floor) as TextView).Text = model.Floor + context.Resources.GetString(Resource.String.floor1);
            if (model.AgreeCount > 0)
                (baseHolder.GetView(Resource.Id.agreeCount) as TextView).Text = " " + model.AgreeCount.ToString();
            if (model.AntiCount > 0)
                (baseHolder.GetView(Resource.Id.antiCount) as TextView).Text = " " + model.AntiCount.ToString();
            (baseHolder.GetView(Resource.Id.progressBar) as ProgressBar).Visibility = ViewStates.Gone;
            var imgDelete = (baseHolder.GetView(Resource.Id.imgDelete) as ImageButton);
            var imgEdit = (baseHolder.GetView(Resource.Id.imgEdit) as ImageButton);
            if (model.CommentID > 0 && User != null && model.UserName == User.DisplayName)
            {
                imgDelete.Visibility = ViewStates.Visible;
                imgDelete.Tag = model.CommentID.ToString();
                imgDelete.SetOnClickListener(this);
                imgEdit.Visibility = ViewStates.Visible;
                imgEdit.Tag = baseHolder.AdapterPosition.ToString();
                imgEdit.SetOnClickListener(this);
            }
            else
            {
                imgDelete.Visibility = ViewStates.Gone;
                imgEdit.Visibility = ViewStates.Gone;
            }
            try
            {
                var face = model.FaceUrl == "" ? "https://pic.cnblogs.com/face/sample_face.gif" : model.FaceUrl;
                Picasso.With(context)
                            .Load(face)
                            .Placeholder(Resource.Drawable.placeholder)
                            .Error(Resource.Drawable.placeholder)
                            .Transform(new CircleTransform())
                            .Into(baseHolder.GetView(Resource.Id.faceUrl) as ImageView);
            }
            catch (Exception)
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
                case Resource.Id.imgEdit:
                    break;
            }
        }
    }
}