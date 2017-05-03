using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Cnblogs.Droid.Model;
using Cnblogs.Droid.UI.Activitys;
using Cnblogs.Droid.UI.Widgets;
using Cnblogs.Droid.Utils;
using Square.Picasso;
using System;

namespace Cnblogs.Droid.UI.Adapters
{
    public class NewsAdapter : BaseAdapter.BaseQuickAdapter<NewsModel>, View.IOnClickListener
    {
        public NewsAdapter() : base(Resource.Layout.fragment_newshome_item)
        {
        }
        protected override void ConvertAsync(RecyclerView.ViewHolder holder, NewsModel model)
        {
            var baseHolder = holder as BaseAdapter.BaseViewHolder;
            var linearLayout = (baseHolder.GetView(Resource.Id.linearLayout) as LinearLayout);
            linearLayout.Tag = model.Id.ToString();
            linearLayout.SetOnClickListener(this);
            (baseHolder.GetView(Resource.Id.txtTitle) as TextView).Text = model.Title;
            (baseHolder.GetView(Resource.Id.txtDscription) as TextView).Text = model.Summary;
            (baseHolder.GetView(Resource.Id.txtPostdate) as TextView).Text = DateTimeUtils.CommonTime(Convert.ToDateTime(model.DateAdded));
            (baseHolder.GetView(Resource.Id.txtDiggCount) as TextView).Text = model.DiggCount + " " + context.Resources.GetString(Resource.String.digg);
            (baseHolder.GetView(Resource.Id.txtViewCount) as TextView).Text = model.ViewCount + " " + context.Resources.GetString(Resource.String.read);
            (baseHolder.GetView(Resource.Id.txtCommentCount) as TextView).Text = model.CommentCount + " " + context.Resources.GetString(Resource.String.comment);

            try
            {
                var icon = model.TopicIcon;
                if (icon.IndexOf("https://") == -1)
                {
                    icon = "https:" + icon;
                }
                Picasso.With(context)
                            .Load(icon)
                            .Placeholder(Resource.Drawable.placeholder)
                            .Error(Resource.Drawable.placeholder)
                            .Into(baseHolder.GetView(Resource.Id.llAvatar) as ImageView);
            }
            catch (Exception)
            {

            }
        }
        public new void OnClick(View v)
        {
            if (v.Tag != null)
            {
                NewsActivity.Start(context, Convert.ToInt32(v.Tag.ToString()));
            }
        }
    }
}