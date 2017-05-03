using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Cnblogs.Droid.Model;
using Cnblogs.Droid.UI.Activitys;
using Cnblogs.Droid.UI.Listeners;
using Cnblogs.Droid.Utils;
using System;

namespace Cnblogs.Droid.UI.Adapters
{
    public class BookmarksAdapter : BaseAdapter.BaseQuickAdapter<BookmarksModel>, View.IOnClickListener
    {
        public IOnDeleteClickListener OnDeleteClickListener { get; set; }
        public IOnEditClickListener OnEditClickListener { get; set; }
        public BookmarksAdapter() : base(Resource.Layout.fragment_bookmarks_item)
        {
        }
        protected override void ConvertAsync(RecyclerView.ViewHolder holder, BookmarksModel model)
        {
            var baseHolder = holder as BaseAdapter.BaseViewHolder;
            var linearLayout = (baseHolder.GetView(Resource.Id.linearLayout) as LinearLayout);
            linearLayout.Tag = model.WzLinkId.ToString();
            linearLayout.SetOnClickListener(this);
            var txtSummary = (baseHolder.GetView(Resource.Id.txtSummary) as TextView);
            if (model.Summary != null && model.Summary.Length > 0)
            {
                txtSummary.Visibility = ViewStates.Visible;
                txtSummary.Text = model.Summary;
            }
            else
            {
                txtSummary.Visibility = ViewStates.Gone;
            }
            (baseHolder.GetView(Resource.Id.txtPostdate) as TextView).Text = DateTimeUtils.CommonTime(Convert.ToDateTime(model.DateAdded));
            (baseHolder.GetView(Resource.Id.txtTitle) as TextView).Text = model.Title;
            (baseHolder.GetView(Resource.Id.txtLink) as TextView).Text = model.LinkUrl;
            (baseHolder.GetView(Resource.Id.progressBar) as ProgressBar).Visibility = ViewStates.Gone;

            var txtTag = (baseHolder.GetView(Resource.Id.txtTag) as TextView);
            if (model.Tag != null && model.Tag != "")
            {
                txtTag.Visibility = ViewStates.Visible;
                txtTag.Text = " " + model.Tag.Replace(',', ' ');
            }
            else
            {
                txtTag.Visibility = ViewStates.Gone;
            }
            var imgDelete = (baseHolder.GetView(Resource.Id.imgDelete) as ImageButton);
            imgDelete.Tag = model.WzLinkId.ToString();
            imgDelete.SetOnClickListener(this);
            var imgEdit = (baseHolder.GetView(Resource.Id.imgEdit) as ImageButton);
            imgEdit.Tag = baseHolder.AdapterPosition.ToString();
            imgEdit.SetOnClickListener(this);
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
                    if (OnEditClickListener != null)
                    {
                        OnEditClickListener.OnEdit(Convert.ToInt32(v.Tag.ToString()));
                    }
                    break;
                default:
                    if (v.Tag != null)
                    {
                        BookmarkActivity.Start(context, Convert.ToInt32(v.Tag.ToString()));
                    }
                    break;
            }
        }
    }
}