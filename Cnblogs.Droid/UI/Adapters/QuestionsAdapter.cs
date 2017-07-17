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
    public class QuestionsAdapter : BaseAdapter.BaseQuickAdapter<QuestionsModel>, View.IOnClickListener
    {
        public QuestionsAdapter() : base(Resource.Layout.fragment_questions_column_item)
        {
        }
        protected override void ConvertAsync(RecyclerView.ViewHolder holder, QuestionsModel model)
        {
            var baseHolder = holder as BaseAdapter.BaseViewHolder;
            var linearLayout = (baseHolder.GetView(Resource.Id.linearLayout) as LinearLayout);
            linearLayout.Tag = model.Qid.ToString();
            linearLayout.SetOnClickListener(this);
            (baseHolder.GetView(Resource.Id.txtTitle) as TextView).Text = model.Title;
            (baseHolder.GetView(Resource.Id.txtSummary) as TextView).Text = model.Summary;
            (baseHolder.GetView(Resource.Id.txtDateAdded) as TextView).Text = DateTimeUtils.CommonTime(Convert.ToDateTime(model.DateAdded));
            (baseHolder.GetView(Resource.Id.txtAnswerCount) as TextView).Text = model.AnswerCount + " " + context.Resources.GetString(Resource.String.answer);
            (baseHolder.GetView(Resource.Id.txtViewCount) as TextView).Text = model.ViewCount + " " + context.Resources.GetString(Resource.String.read);

            var txtTag = (baseHolder.GetView(Resource.Id.txtTag) as TextView);
            if (model.Tags != null && model.Tags.Length > 0)
            {
                txtTag.Visibility = ViewStates.Visible;
                txtTag.Text = " "+model.Tags.Replace(',', ' ');
            }
            else
            {
                txtTag.Visibility = ViewStates.Gone;
            }

            var txtGold = (baseHolder.GetView(Resource.Id.txtGold) as TextView);
            if (model.Award > 0)
            {
                txtGold.Visibility = ViewStates.Visible;
                txtGold.Text = model.Award.ToString();
            }
            else
            {
                txtGold.Visibility = ViewStates.Gone;
            }
            if (model.QuestionUserInfo != null && model.QuestionUserInfo.UserID > 0)
            {
                (baseHolder.GetView(Resource.Id.txtUserName) as TextView).Text = HtmlUtils.GetHtml(model.QuestionUserInfo.UserName).ToString();
                try
                {
                    Picasso.With(context)
                                .Load("https://pic.cnblogs.com/face/" + model.QuestionUserInfo.IconName)
                                .Placeholder(Resource.Drawable.placeholder)
                                .Error(Resource.Drawable.placeholder)
                                .Transform(new CircleTransform())
                                .Into(baseHolder.GetView(Resource.Id.imgIconName) as ImageView);
                }
                catch (Exception)
                {

                }
            }
        }
        public new void OnClick(View v)
        {
            if (v.Tag != null)
            {
                QuestionActivity.Start(context, Convert.ToInt32(v.Tag.ToString()));
            }
        }
    }
}