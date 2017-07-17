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
    public class StatusCommentsAdapter : BaseAdapter.BaseQuickAdapter<StatusCommentsModel>, View.IOnClickListener
    {
        public UserModel User { get; set; }
        public IOnDeleteClickListener OnDeleteClickListener { get; set; }
        public StatusCommentsAdapter() : base(Resource.Layout.status_comments_item)
        {
        }
        protected override void ConvertAsync(RecyclerView.ViewHolder holder, StatusCommentsModel model)
        {
            var baseHolder = holder as BaseAdapter.BaseViewHolder;
            var itemview = baseHolder.GetConvertView();
            itemview.Tag = model.Id;
            (baseHolder.GetView(Resource.Id.txtUserName) as TextView).Text = HtmlUtils.GetHtml(model.UserDisplayName).ToString();
            (baseHolder.GetView(Resource.Id.txtPostdate) as TextView).Text = DateTimeUtils.CommonTime(Convert.ToDateTime(model.DateAdded));
            (baseHolder.GetView(Resource.Id.txtContent) as TextView).SetText(HtmlUtils.GetHtml(model.Content), TextView.BufferType.Spannable);
            var imgUserUrl = (baseHolder.GetView(Resource.Id.imgUserUrl) as ImageView);
            (baseHolder.GetView(Resource.Id.progressBar) as ProgressBar).Visibility = ViewStates.Gone;
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
            if (OnDeleteClickListener != null)
            {
                OnDeleteClickListener.OnDelete(Convert.ToInt32(v.Tag.ToString()));
            }
        }
    }
}