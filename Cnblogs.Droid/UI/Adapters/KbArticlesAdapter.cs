using Android.Support.V7.Widget;
using Android.Text;
using Android.Views;
using Android.Widget;
using Cnblogs.Droid.Model;
using Cnblogs.Droid.UI.Activitys;
using Cnblogs.Droid.Utils;
using System;

namespace Cnblogs.Droid.UI.Adapters
{
    public class KbArticlesAdapter : BaseAdapter.BaseQuickAdapter<KbArticlesModel>, View.IOnClickListener
    {
        public KbArticlesAdapter() : base(Resource.Layout.fragment_kbArticles_item)
        {
        }
        protected override void ConvertAsync(RecyclerView.ViewHolder holder, KbArticlesModel model)
        {
            var baseHolder = holder as BaseAdapter.BaseViewHolder;
            var linearLayout = (baseHolder.GetView(Resource.Id.linearLayout) as LinearLayout);
            linearLayout.Tag = model.Id.ToString();
            linearLayout.SetOnClickListener(this);
            (baseHolder.GetView(Resource.Id.txtSummary) as TextView).Text = model.Summary;
            (baseHolder.GetView(Resource.Id.txtPostdate) as TextView).Text = DateTimeUtils.CommonTime(Convert.ToDateTime(model.DateAdded));
            (baseHolder.GetView(Resource.Id.txtDiggCount) as TextView).Text = model.DiggCount + " " + context.Resources.GetString(Resource.String.digg);
            (baseHolder.GetView(Resource.Id.txtViewCount) as TextView).Text = model.ViewCount + " " + context.Resources.GetString(Resource.String.read);

            var Title = (baseHolder.GetView(Resource.Id.txtTitle) as TextView);
            Title.Text = model.Title;
            if (model.Author != null)
            {
                Title.Text += " - " + Html.FromHtml(model.Author).ToString();
            }
        }
        public new void OnClick(View v)
        {
            if (v.Tag != null)
            {
                KbArticleActivity.Start(context, Convert.ToInt32(v.Tag.ToString()));
            }
        }
    }
}