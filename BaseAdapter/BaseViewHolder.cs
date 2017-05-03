using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Java.Lang;
using System.Collections.Generic;

namespace BaseAdapter
{
    public class BaseViewHolder : RecyclerView.ViewHolder
    {
        private SparseArray<View> views;
        public HashSet<Integer> GetNestViews()
        {
            return nestViews;
        }

        private HashSet<Integer> nestViews;

        private HashSet<Integer> childClickViewIds;

        private HashSet<Integer> itemChildLongClickViewIds;

        Java.Lang.Object associatedObject;

        public View convertView;
        public BaseViewHolder(View view) : base(view)
        {
            this.views = new SparseArray<View>();
            this.childClickViewIds = new HashSet<Integer>();
            this.itemChildLongClickViewIds = new HashSet<Integer>();
            this.nestViews = new HashSet<Integer>();
            convertView = view;
        }
        public View GetConvertView()
        {
            return convertView;
        }
        public BaseViewHolder SetVisible(int viewId, bool visible)
        {
            View view = GetView(viewId);
            view.Visibility = (visible ? ViewStates.Visible : ViewStates.Gone);
            return this;
        }
        public View GetView(int viewId)
        {
            View view = views.Get(viewId);
            if (view == null)
            {
                view = convertView.FindViewById(viewId);
                views.Put(viewId, view);
            }
            return view;
        }
        
        public Java.Lang.Object GetAssociatedObject()
        {
            return associatedObject;
        }
        
        public void SetAssociatedObject(Java.Lang.Object associatedObject)
        {
            this.associatedObject = associatedObject;
        }
    }
}