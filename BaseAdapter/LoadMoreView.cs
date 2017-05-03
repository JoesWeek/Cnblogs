using System;

namespace BaseAdapter
{
    public abstract class LoadMoreView
    {
        public const int StausDefault = 1;
        public const int StausLoading = 2;
        public const int StausFail = 3;
        public const int StausEnd = 4;

        private int loadMoreStatus = StausDefault;
        private bool loadMoreEndGone = false;

        public void SetLoadMoreStatus(int loadMoreStatus)
        {
            this.loadMoreStatus = loadMoreStatus;
        }

        public int GetLoadMoreStatus()
        {
            return loadMoreStatus;
        }

        public void Convert(BaseViewHolder holder)
        {
            switch (loadMoreStatus)
            {
                case StausLoading:
                    SetVisibleLoading(holder, true);
                    SetVisibleLoadFail(holder, false);
                    SetVisibleLoadEnd(holder, false);
                    break;
                case StausFail:
                    SetVisibleLoading(holder, false);
                    SetVisibleLoadFail(holder, true);
                    SetVisibleLoadEnd(holder, false);
                    break;
                case StausEnd:
                    SetVisibleLoading(holder, false);
                    SetVisibleLoadFail(holder, false);
                    SetVisibleLoadEnd(holder, true);
                    break;
                case StausDefault:
                    SetVisibleLoading(holder, false);
                    SetVisibleLoadFail(holder, false);
                    SetVisibleLoadEnd(holder, false);
                    break;
            }
        }

        private void SetVisibleLoading(BaseViewHolder holder, bool visible)
        {
            holder.SetVisible(GetLoadingViewId(), visible);
        }

        private void SetVisibleLoadFail(BaseViewHolder holder, bool visible)
        {
            holder.SetVisible(GetLoadFailViewId(), visible);
        }

        private void SetVisibleLoadEnd(BaseViewHolder holder, bool visible)
        {
            int loadEndViewId = GetLoadEndViewId();
            if (loadEndViewId != 0)
            {
                holder.SetVisible(loadEndViewId, visible);
            }
        }

        public void SetLoadMoreEndGone(bool loadMoreEndGone)
        {
            this.loadMoreEndGone = loadMoreEndGone;
        }

        public bool IsLoadEndMoreGone()
        {
            if (GetLoadEndViewId() == 0)
            {
                return true;
            }
            return loadMoreEndGone;
        }

        [Obsolete]
        public bool IsLoadEndGone() { return loadMoreEndGone; }

        public abstract int GetLayoutId();

        protected abstract int GetLoadingViewId();

        protected abstract int GetLoadFailViewId();

        protected abstract int GetLoadEndViewId();
    }
}