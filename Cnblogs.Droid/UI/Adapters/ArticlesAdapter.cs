using Android.Support.V7.Widget;
using Android.Text;
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
    public class ArticlesAdapter : BaseAdapter.BaseQuickAdapter<ArticlesModel>, View.IOnClickListener
    {
        public ArticlesAdapter() : base(Resource.Layout.fragment_article_item)
        {
        }
        protected override void ConvertAsync(RecyclerView.ViewHolder holder, ArticlesModel model)
        {
            var baseHolder = holder as BaseAdapter.BaseViewHolder;
            var linearLayout = (baseHolder.GetView(Resource.Id.linearLayout) as LinearLayout);
            linearLayout.Tag = model.Id.ToString();
            linearLayout.SetOnClickListener(this);
            (baseHolder.GetView(Resource.Id.txtTitle) as TextView).Text = model.Title;
            (baseHolder.GetView(Resource.Id.txtDscription) as TextView).Text = model.Description;
            (baseHolder.GetView(Resource.Id.txtPostdate) as TextView).Text = DateTimeUtils.CommonTime(Convert.ToDateTime(model.PostDate));
            (baseHolder.GetView(Resource.Id.txtDiggCount) as TextView).Text = model.DiggCount + " " + context.Resources.GetString(Resource.String.digg);
            (baseHolder.GetView(Resource.Id.txtViewCount) as TextView).Text = model.ViewCount + " " + context.Resources.GetString(Resource.String.read);
            (baseHolder.GetView(Resource.Id.txtCommentCount) as TextView).Text = model.CommentCount + " " + context.Resources.GetString(Resource.String.comment);
            if (model.Author != null)
            {
                (baseHolder.GetView(Resource.Id.txtAuthor) as TextView).Text = HtmlUtils.GetHtml(model.Author).ToString();
                try
                {
                    Picasso.With(context)
                                .Load(model.Avatar)
                                .Placeholder(Resource.Drawable.placeholder)
                                .Error(Resource.Drawable.placeholder)
                                .Transform(new CircleTransform())
                                .Into(baseHolder.GetView(Resource.Id.llAvatar) as ImageView);
                }
                catch (Exception ex)
                {

                }
            }
        }
        public void OnClick(View v)
        {
            if (v.Tag != null)
            {
                ArticleActivity.Start(context, Convert.ToInt32(v.Tag.ToString()));
            }
        }
    }
}