using Android.Support.V7.Widget;
using Android.Text;
using Android.Widget;
using Cnblogs.Droid.Model;
using Cnblogs.Droid.UI.Widgets;
using Cnblogs.Droid.Utils;
using Square.Picasso;
using System;

namespace Cnblogs.Droid.UI.Adapters
{
    public class ArticleCommentsAdapter : BaseAdapter.BaseQuickAdapter<ArticleCommentModel>
    {
        public ArticleCommentsAdapter() : base(Resource.Layout.article_comment_item)
        {
        }
        protected override void ConvertAsync(RecyclerView.ViewHolder holder, ArticleCommentModel model)
        {
            var baseHolder = holder as BaseAdapter.BaseViewHolder;
            (baseHolder.GetView(Resource.Id.author) as TextView).Text = HtmlUtils.GetHtml(model.Author).ToString();
            (baseHolder.GetView(Resource.Id.body) as TextView).Text = HtmlUtils.GetHtml(model.Body).ToString();
            (baseHolder.GetView(Resource.Id.createAt) as TextView).Text = DateTimeUtils.CommonTime(Convert.ToDateTime(model.DateAdded));
            (baseHolder.GetView(Resource.Id.floor) as TextView).Text = model.Floor + context.Resources.GetString(Resource.String.floor1);
            try
            {
                var face = model.FaceUrl == "" ? "https://pic.cnblogs.com/face/sample_face.gif" : model.FaceUrl;
                Picasso.With(context)
                            .Load(face)
                            .Placeholder(Resource.Drawable.placeholder)
                            .Error(Resource.Drawable.placeholder)
                            .Transform(new CircleTransform())
                            .Into(baseHolder.GetView(Resource.Id.avatar) as ImageView);
            }
            catch (Exception)
            {

            }
        }
    }
}