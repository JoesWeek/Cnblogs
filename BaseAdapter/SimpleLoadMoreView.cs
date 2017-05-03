namespace BaseAdapter
{
    public class SimpleLoadMoreView : LoadMoreView
    {
        public override int GetLayoutId()
        {
            return Resource.Layout.quick_view_load_more;
        }

        protected override int GetLoadingViewId()
        {
            return Resource.Id.load_more_loading_view;
        }

        protected override int GetLoadFailViewId()
        {
            return Resource.Id.load_more_load_fail_view;
        }

        protected override int GetLoadEndViewId()
        {
            return Resource.Id.load_more_load_end_view;
        }
    }
}