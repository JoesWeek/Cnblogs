using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseAdapter
{
    public abstract class BaseQuickAdapter<T> : RecyclerView.Adapter, View.IOnClickListener
    {
        private bool nextLoadEnable = false;
        private bool loadMoreEnable = false;
        private bool loading = false;
        private LoadMoreView loadMoreView = new SimpleLoadMoreView();
        private IOnLoadMoreListener requestLoadMoreListener;
        private int duration = 300;

        //header footer
        private LinearLayout headerLayout;
        private LinearLayout footerLayout;
        //empty
        private FrameLayout emptyLayout;
        private bool isUseEmpty = true;
        private bool headAndEmptyEnable;
        private bool footAndEmptyEnable;

        protected Context context;
        protected int layoutResId;
        protected LayoutInflater layoutInflater;
        protected List<T> data;
        public const int HeaderView = 0x00000111;
        public const int LoadingView = 0x00000222;
        public const int FooterView = 0x00000333;
        public const int EmptyView = 0x00000555;
        public void SetOnLoadMoreListener(IOnLoadMoreListener requestLoadMoreListener)
        {
            this.requestLoadMoreListener = requestLoadMoreListener;
            nextLoadEnable = true;
            loadMoreEnable = true;
            loading = false;
        }
        public void SetLoadMoreView(LoadMoreView loadingView)
        {
            this.loadMoreView = loadingView;
        }

        public int GetLoadMoreViewCount()
        {
            if (requestLoadMoreListener == null || !loadMoreEnable)
            {
                return 0;
            }
            if (!nextLoadEnable && loadMoreView.IsLoadEndMoreGone())
            {
                return 0;
            }
            if (data.Count() == 0)
            {
                return 0;
            }
            return 1;
        }
        public bool IsLoading()
        {
            return loading;
        }

        public void LoadMoreEnd()
        {
            LoadMoreEnd(false);
        }

        public void LoadMoreEnd(bool gone)
        {
            if (GetLoadMoreViewCount() == 0)
            {
                return;
            }
            loading = false;
            nextLoadEnable = false;
            loadMoreView.SetLoadMoreEndGone(gone);
            if (gone)
            {
                NotifyItemRemoved(GetHeaderLayoutCount() + data.Count() + GetFooterLayoutCount());
            }
            else
            {
                loadMoreView.SetLoadMoreStatus(LoadMoreView.StausEnd);
                NotifyItemChanged(GetHeaderLayoutCount() + data.Count() + GetFooterLayoutCount());
            }
        }
        public void LoadMoreComplete()
        {
            if (GetLoadMoreViewCount() == 0)
            {
                return;
            }
            loading = false;
            loadMoreView.SetLoadMoreStatus(LoadMoreView.StausDefault);
            NotifyItemChanged(GetHeaderLayoutCount() + data.Count() + GetFooterLayoutCount());
        }

        public void LoadMoreFail()
        {
            if (GetLoadMoreViewCount() == 0)
            {
                return;
            }
            loading = false;
            loadMoreView.SetLoadMoreStatus(LoadMoreView.StausFail);
            NotifyItemChanged(GetHeaderLayoutCount() + data.Count() + GetFooterLayoutCount());
        }

        public void SetEnableLoadMore(bool enable)
        {
            int oldLoadMoreCount = GetLoadMoreViewCount();
            loadMoreEnable = enable;
            int newLoadMoreCount = GetLoadMoreViewCount();

            if (oldLoadMoreCount == 1)
            {
                if (newLoadMoreCount == 0)
                {
                    NotifyItemRemoved(GetHeaderLayoutCount() + data.Count() + GetFooterLayoutCount());
                }
            }
            else
            {
                if (newLoadMoreCount == 1)
                {
                    loadMoreView.SetLoadMoreStatus(LoadMoreView.StausDefault);
                    NotifyItemInserted(GetHeaderLayoutCount() + data.Count() + GetFooterLayoutCount());
                }
            }
        }
        public bool IsLoadMoreEnable()
        {
            return loadMoreEnable;
        }
        public void SetDuration(int duration)
        {
            this.duration = duration;
        }
        public BaseQuickAdapter(int layoutResId, List<T> data)
        {
            this.data = data == null ? new List<T>() : data;
            if (layoutResId != 0)
            {
                this.layoutResId = layoutResId;
            }
        }

        public BaseQuickAdapter(List<T> data) : this(0, data)
        {

        }

        public BaseQuickAdapter(int layoutResId) : this(layoutResId, null)
        {
        }
        public void SetNewData(List<T> data)
        {
            this.data = data == null ? new List<T>() : data;
            if (requestLoadMoreListener != null)
            {
                nextLoadEnable = true;
                loadMoreEnable = true;
                loading = false;
                loadMoreView.SetLoadMoreStatus(LoadMoreView.StausDefault);
            }
            NotifyDataSetChanged();
        }

        [Obsolete]
        public void Add(int position, T item)
        {
            AddData(position, item);
        }

        public void AddData(int position, T data)
        {
            this.data.Insert(position, data);
            NotifyItemInserted(position + GetHeaderLayoutCount());
            CompatibilityDataSizeChanged(1);
        }
        public void AddData(T data)
        {
            this.data.Add(data);
            NotifyItemInserted(this.data.Count() + GetHeaderLayoutCount());
            CompatibilityDataSizeChanged(1);
        }

        public void Remove(int position)
        {
            data.RemoveAt(position);
            NotifyItemRemoved(position + GetHeaderLayoutCount());
            CompatibilityDataSizeChanged(0);
        }

        public void SetData(int index, T data)
        {
            this.data.Insert(index, data);
            NotifyItemChanged(index + GetHeaderLayoutCount());
        }

        public void AddData(int position, List<T> data)
        {
            this.data.InsertRange(position, data);
            NotifyItemRangeInserted(position + GetHeaderLayoutCount(), data.Count());
            CompatibilityDataSizeChanged(data.Count());
        }

        public void AddData(List<T> newData)
        {
            this.data.AddRange(newData);
            NotifyItemRangeInserted(data.Count() - newData.Count() + GetHeaderLayoutCount(), newData.Count());
            CompatibilityDataSizeChanged(newData.Count());
        }

        private void CompatibilityDataSizeChanged(int size)
        {
            int dataSize = data == null ? 0 : data.Count();
            if (dataSize == size)
            {
                NotifyDataSetChanged();
            }
        }

        public List<T> GetData()
        {
            return data;
        }
        public T GetItem(int position)
        {
            if (position != -1)
                return data[position];
            else
                return default(T);
        }

        [Obsolete]
        public int GetHeaderViewsCount()
        {
            return GetHeaderLayoutCount();
        }

        [Obsolete]
        public int GetFooterViewsCount()
        {
            return GetFooterLayoutCount();
        }

        public int GetHeaderLayoutCount()
        {
            if (headerLayout == null || headerLayout.ChildCount == 0)
            {
                return 0;
            }
            return 1;
        }

        public int GetFooterLayoutCount()
        {
            if (footerLayout == null || footerLayout.ChildCount == 0)
            {
                return 0;
            }
            return 1;
        }

        public int GetEmptyViewCount()
        {
            if (emptyLayout == null || emptyLayout.ChildCount == 0)
            {
                return 0;
            }
            if (!isUseEmpty)
            {
                return 0;
            }
            if (data.Count() != 0)
            {
                return 0;
            }
            return 1;
        }
        public override int ItemCount
        {
            get
            {
                int count;
                if (GetEmptyViewCount() == 1)
                {
                    count = 1;
                    if (headAndEmptyEnable && GetHeaderLayoutCount() != 0)
                    {
                        count++;
                    }
                    if (footAndEmptyEnable && GetFooterLayoutCount() != 0)
                    {
                        count++;
                    }
                }
                else
                {
                    count = GetHeaderLayoutCount() + data.Count() + GetFooterLayoutCount() + GetLoadMoreViewCount();
                }
                return count;
            }
        }
        public override int GetItemViewType(int position)
        {
            if (GetEmptyViewCount() == 1)
            {
                bool header = headAndEmptyEnable && GetHeaderLayoutCount() != 0;
                switch (position)
                {
                    case 0:
                        if (header)
                        {
                            return HeaderView;
                        }
                        else
                        {
                            return EmptyView;
                        }
                    case 1:
                        if (header)
                        {
                            return EmptyView;
                        }
                        else
                        {
                            return FooterView;
                        }
                    case 2:
                        return FooterView;
                    default:
                        return EmptyView;
                }
            }
            AutoLoadMore(position);
            int numHeaders = GetHeaderLayoutCount();
            if (position < numHeaders)
            {
                return HeaderView;
            }
            else
            {
                int adjPosition = position - numHeaders;
                int adapterCount = data.Count();
                if (adjPosition < adapterCount)
                {
                    return GetDefItemViewType(adjPosition);
                }
                else
                {
                    adjPosition = adjPosition - adapterCount;
                    int numFooters = GetFooterLayoutCount();
                    if (adjPosition < numFooters)
                    {
                        return FooterView;
                    }
                    else
                    {
                        return LoadingView;
                    }
                }
            }
        }
        protected virtual int GetDefItemViewType(int position)
        {
            return base.GetItemViewType(position);
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            RecyclerView.ViewHolder baseViewHolder = null;
            this.context = parent.Context;
            this.layoutInflater = LayoutInflater.From(context);
            switch (viewType)
            {
                case LoadingView:
                    baseViewHolder = GetLoadingView(parent);
                    break;
                case HeaderView:
                    baseViewHolder = CreateBaseViewHolder(headerLayout);
                    break;
                case EmptyView:
                    baseViewHolder = CreateBaseViewHolder(emptyLayout);
                    break;
                case FooterView:
                    baseViewHolder = CreateBaseViewHolder(footerLayout);
                    break;
                default:
                    baseViewHolder = OnCreateDefViewHolder(parent, viewType);
                    break;
            }
            return baseViewHolder;
        }
        private RecyclerView.ViewHolder GetLoadingView(ViewGroup parent)
        {
            View view = GetItemView(loadMoreView.GetLayoutId(), parent);
            RecyclerView.ViewHolder holder = CreateBaseViewHolder(view);
            holder.ItemView.SetOnClickListener(this);
            //´ý²¹³ä
            return holder;
        }
        public override void OnViewAttachedToWindow(Java.Lang.Object holder)
        {
            base.OnViewAttachedToWindow(holder);
            RecyclerView.ViewHolder viewHolder = holder as RecyclerView.ViewHolder;
            int type = viewHolder.ItemViewType;
            if (type == EmptyView || type == HeaderView || type == FooterView || type == LoadingView)
            {
                SetFullSpan(viewHolder);
            }
            else
            {
                //AddAnimation(viewHolder);
            }
        }
        protected void SetFullSpan(RecyclerView.ViewHolder holder)
        {
            if (holder.ItemView.LayoutParameters is StaggeredGridLayoutManager.LayoutParams)
            {
                StaggeredGridLayoutManager.LayoutParams p = (StaggeredGridLayoutManager.LayoutParams)holder
                        .ItemView.LayoutParameters;
                p.FullSpan = true;
            }
        }
        public override void OnAttachedToRecyclerView(RecyclerView recyclerView)
        {
            base.OnAttachedToRecyclerView(recyclerView);
            RecyclerView.LayoutManager manager = recyclerView.GetLayoutManager();
            if (manager is GridLayoutManager)
            {
                GridLayoutManager gridManager = ((GridLayoutManager)manager);
                //´ý²¹³ä
            }
        }
        private SpanSizeLookup spanSizeLookup;

        public interface SpanSizeLookup
        {
            int GetSpanSize(GridLayoutManager gridLayoutManager, int position);
        }

        public void SetSpanSizeLookup(SpanSizeLookup spanSizeLookup)
        {
            this.spanSizeLookup = spanSizeLookup;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            int viewType = holder.ItemViewType;

            switch (viewType)
            {
                case 0:
                    ConvertAsync(holder, data[holder.LayoutPosition - GetHeaderLayoutCount()]);
                    break;
                case LoadingView:
                    loadMoreView.Convert(holder as BaseViewHolder);
                    break;
                case HeaderView:
                    break;
                case EmptyView:
                    break;
                case FooterView:
                    break;
                default:
                    ConvertAsync(holder, data[holder.LayoutPosition - GetHeaderLayoutCount()]);
                    break;
            }
        }
        protected virtual RecyclerView.ViewHolder OnCreateDefViewHolder(ViewGroup parent, int viewType)
        {
            return CreateBaseViewHolder(parent, layoutResId);
        }

        protected RecyclerView.ViewHolder CreateBaseViewHolder(ViewGroup parent, int layoutResId)
        {
            return CreateBaseViewHolder(GetItemView(layoutResId, parent));
        }
        protected RecyclerView.ViewHolder CreateBaseViewHolder(View view)
        {
            return new BaseViewHolder(view);
        }
        public LinearLayout GetHeaderLayout()
        {
            return headerLayout;
        }

        public LinearLayout GetFooterLayout()
        {
            return footerLayout;
        }
        public void AddHeaderView(View header)
        {
            AddHeaderView(header, -1);
        }
        public void AddHeaderView(View header, int index)
        {
            AddHeaderView(header, index, Orientation.Vertical);
        }

        public void AddHeaderView(View header, int index, Orientation orientation)
        {
            if (headerLayout == null)
            {
                headerLayout = new LinearLayout(header.Context);
                if (orientation == Orientation.Vertical)
                {
                    headerLayout.Orientation = Orientation.Vertical;
                    headerLayout.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                }
                else
                {
                    headerLayout.Orientation = Orientation.Horizontal;
                    headerLayout.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent);
                }
            }
            index = index >= headerLayout.ChildCount ? -1 : index;
            headerLayout.AddView(header, index);
            if (headerLayout.ChildCount == 1)
            {
                int position = GetHeaderViewPosition();
                if (position != -1)
                {
                    NotifyItemInserted(position);
                }
            }
        }

        public void SetHeaderView(View header)
        {
            SetHeaderView(header, 0, Orientation.Vertical);
        }

        public void SetHeaderView(View header, int index)
        {
            SetHeaderView(header, index, Orientation.Vertical);
        }

        public void SetHeaderView(View header, int index, Orientation orientation)
        {
            if (headerLayout == null || headerLayout.ChildCount <= index)
            {
                AddHeaderView(header, index, orientation);
            }
            else
            {
                headerLayout.RemoveViewAt(index);
                headerLayout.AddView(header, index);
            }
        }
        public void AddFooterView(View footer)
        {
            AddFooterView(footer, -1, Orientation.Vertical);
        }

        public void AddFooterView(View footer, int index)
        {
            AddFooterView(footer, index, Orientation.Vertical);
        }

        public void AddFooterView(View footer, int index, Orientation orientation)
        {
            if (footerLayout == null)
            {
                footerLayout = new LinearLayout(footer.Context);
                if (orientation == Orientation.Vertical)
                {
                    footerLayout.Orientation = Orientation.Vertical;
                    footerLayout.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                }
                else
                {
                    footerLayout.Orientation = Orientation.Horizontal;
                    footerLayout.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent);
                }
            }
            index = index >= footerLayout.ChildCount ? -1 : index;
            footerLayout.AddView(footer, index);
            if (footerLayout.ChildCount == 1)
            {
                int position = GetFooterViewPosition();
                if (position != -1)
                {
                    NotifyItemInserted(position);
                }
            }
        }

        public void SetFooterView(View header)
        {
            SetFooterView(header, 0, Orientation.Vertical);
        }

        public void SetFooterView(View header, int index)
        {
            SetFooterView(header, index, Orientation.Vertical);
        }

        public void SetFooterView(View header, int index, Orientation orientation)
        {
            if (footerLayout == null || footerLayout.ChildCount <= index)
            {
                AddFooterView(header, index, orientation);
            }
            else
            {
                footerLayout.RemoveViewAt(index);
                footerLayout.AddView(header, index);
            }
        }
        public void RemoveHeaderView(View header)
        {
            if (GetHeaderLayoutCount() == 0) return;

            headerLayout.RemoveView(header);
            if (headerLayout.ChildCount == 0)
            {
                int position = GetHeaderViewPosition();
                if (position != -1)
                {
                    NotifyItemRemoved(position);
                }
            }
        }

        public void RemoveFooterView(View footer)
        {
            if (GetFooterLayoutCount() == 0) return;

            footerLayout.RemoveView(footer);
            if (footerLayout.ChildCount == 0)
            {
                int position = GetFooterViewPosition();
                if (position != -1)
                {
                    NotifyItemRemoved(position);
                }
            }
        }

        public void RemoveAllHeaderView()
        {
            if (GetHeaderLayoutCount() == 0) return;

            headerLayout.RemoveAllViews();
            int position = GetHeaderViewPosition();
            if (position != -1)
            {
                NotifyItemRemoved(position);
            }
        }

        public void RemoveAllFooterView()
        {
            if (GetFooterLayoutCount() == 0) return;

            footerLayout.RemoveAllViews();
            int position = GetFooterViewPosition();
            if (position != -1)
            {
                NotifyItemRemoved(position);
            }
        }

        private int GetHeaderViewPosition()
        {
            if (GetEmptyViewCount() == 1)
            {
                if (headAndEmptyEnable)
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
            return -1;
        }

        private int GetFooterViewPosition()
        {
            if (GetEmptyViewCount() == 1)
            {
                int position = 1;
                if (headAndEmptyEnable && GetHeaderLayoutCount() != 0)
                {
                    position++;
                }
                if (footAndEmptyEnable)
                {
                    return position;
                }
            }
            else
            {
                return GetHeaderLayoutCount() + data.Count();
            }
            return -1;
        }

        public void SetEmptyView(int layoutResId, ViewGroup viewGroup)
        {
            View view = LayoutInflater.From(viewGroup.Context).Inflate(layoutResId, viewGroup, false);
            SetEmptyView(view);
        }

        public void SetEmptyView(View emptyView)
        {
            bool insert = false;
            if (emptyLayout == null)
            {
                emptyLayout = new FrameLayout(emptyView.Context);
                ViewGroup.LayoutParams layoutParams = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
                ViewGroup.LayoutParams lp = emptyView.LayoutParameters;
                if (lp != null)
                {
                    layoutParams.Width = lp.Width;
                    layoutParams.Height = lp.Height;
                }
                emptyLayout.LayoutParameters = layoutParams;
                insert = true;
            }
            emptyLayout.RemoveAllViews();
            emptyLayout.AddView(emptyView);
            isUseEmpty = true;
            if (insert)
            {
                if (GetEmptyViewCount() == 1)
                {
                    int position = 0;
                    if (headAndEmptyEnable && GetHeaderLayoutCount() != 0)
                    {
                        position++;
                    }
                    NotifyItemInserted(position);
                }
            }
        }
        public void SetHeaderAndEmpty(bool isHeadAndEmpty)
        {
            SetHeaderFooterEmpty(isHeadAndEmpty, false);
        }

        public void SetHeaderFooterEmpty(bool isHeadAndEmpty, bool isFootAndEmpty)
        {
            headAndEmptyEnable = isHeadAndEmpty;
            footAndEmptyEnable = isFootAndEmpty;
        }

        public void IsUseEmpty(bool isUseEmpty)
        {
            this.isUseEmpty = isUseEmpty;
        }

        public View GetEmptyView()
        {
            return emptyLayout;
        }

        private int autoLoadMoreSize = 1;

        public void SetAutoLoadMoreSize(int autoLoadMoreSize)
        {
            if (autoLoadMoreSize > 1)
            {
                this.autoLoadMoreSize = autoLoadMoreSize;
            }
        }

        private void AutoLoadMore(int position)
        {
            if (GetLoadMoreViewCount() == 0)
            {
                return;
            }
            if (position < ItemCount - autoLoadMoreSize)
            {
                return;
            }
            if (loadMoreView.GetLoadMoreStatus() != LoadMoreView.StausDefault)
            {
                return;
            }
            loadMoreView.SetLoadMoreStatus(LoadMoreView.StausLoading);
            if (!loading)
            {
                loading = true;
                requestLoadMoreListener.OnLoadMoreRequested();
            }
        }
        protected View GetItemView(int layoutResId, ViewGroup parent)
        {
            return layoutInflater.Inflate(layoutResId, parent, false);
        }
        protected abstract void ConvertAsync(RecyclerView.ViewHolder helper, T item);
        public override long GetItemId(int position)
        {
            return position;
        }
        private int GetItemPosition(T item)
        {
            return item != null && data != null && !(data.Count() > 0) ? data.IndexOf(item) : -1;
        }

        public void OnClick(View v)
        {
            if (loadMoreView.GetLoadMoreStatus() == LoadMoreView.StausFail)
            {
                loadMoreView.SetLoadMoreStatus(LoadMoreView.StausDefault);
                NotifyItemChanged(GetHeaderLayoutCount() + data.Count() + GetFooterLayoutCount());
            }
        }
    }
}
