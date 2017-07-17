using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Text.Style;
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
    public class SearchAdapter : BaseAdapter.BaseQuickAdapter<SearchModel>, View.IOnClickListener
    {
        private int position;
        public SearchAdapter(int position) : base(Resource.Layout.fragment_search_item)
        {
            this.position = position;
        }
        protected override void ConvertAsync(RecyclerView.ViewHolder holder, SearchModel model)
        {
            var baseHolder = holder as BaseAdapter.BaseViewHolder;
            var linearLayout = (baseHolder.GetView(Resource.Id.linearLayout) as LinearLayout);
            if (model.Id == null)
            {
                linearLayout.Tag = model.Uri.ToString();
            }
            else
            {
                linearLayout.Tag = model.Id.ToString();
            }
            linearLayout.SetOnClickListener(this);
            
            (baseHolder.GetView(Resource.Id.txtTitle) as TextView).Text = HtmlUtils.GetHtml(model.Title).ToString();
            (baseHolder.GetView(Resource.Id.txtDscription) as TextView).Text = HtmlUtils.GetHtml(model.Content).ToString();
            (baseHolder.GetView(Resource.Id.txtPostdate) as TextView).Text = DateTimeUtils.CommonTime(Convert.ToDateTime(model.PublishTime));
            (baseHolder.GetView(Resource.Id.txtDiggCount) as TextView).Text = model.VoteTimes + " " + context.Resources.GetString(Resource.String.digg);
            (baseHolder.GetView(Resource.Id.txtViewCount) as TextView).Text = model.ViewTimes + " " + context.Resources.GetString(Resource.String.read);
            (baseHolder.GetView(Resource.Id.txtCommentCount) as TextView).Text = model.CommentTimes + " " + context.Resources.GetString(Resource.String.comment);
            var txtAuthor = (baseHolder.GetView(Resource.Id.txtAuthor) as TextView);
            if (model.UserName != null)
            {
                (txtAuthor.Parent as View).Visibility = ViewStates.Visible;
                txtAuthor.Text = model.UserName;
                try
                {
                    Picasso.With(context)
                                .Load(model.UserAlias)
                                .Placeholder(Resource.Drawable.placeholder)
                                .Error(Resource.Drawable.placeholder)
                                .Transform(new CircleTransform())
                                .Into(baseHolder.GetView(Resource.Id.llAvatar) as ImageView);
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                (txtAuthor.Parent as View).Visibility = ViewStates.Gone;
            }
        }
        public void OnClick(View v)
        {
            if (v.Tag != null)
            {
                switch (position)
                {
                    case 0:
                        ArticleActivity.Start(context, Convert.ToInt32(v.Tag.ToString()));
                        break;
                    case 1:
                        NewsActivity.Start(context, Convert.ToInt32(v.Tag.ToString()));
                        break;
                    case 2:
                        KbArticleActivity.Start(context, Convert.ToInt32(v.Tag.ToString()));
                        break;
                    case 3:
                        var tag = v.Tag.ToString();
                        var id = tag.Substring(tag.LastIndexOf("/") + 1);
                        QuestionActivity.Start(context, Convert.ToInt32(id));
                        break;
                    default:
                        break;
                }
            }
        }
    }
}