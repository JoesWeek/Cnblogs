using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using Android.Views.InputMethods;
using Android.Text;
using Java.Lang;
using Cnblogs.Droid.UI.Listeners;

namespace Cnblogs.Droid.UI.Widgets
{
    public class CnblogsSearchView : FrameLayout, TextView.IOnEditorActionListener, ITextWatcher, View.IOnFocusChangeListener, View.IOnClickListener, IMenuItemOnMenuItemClickListener
    {
        private IMenuItem menuItem;
        private bool isSearchOpen = false;
        private int animationDuration = 400;
        private bool clearingFocus;

        private Context context;
        //Views
        private View searchLayout;
        private View tintView;
        private ListView suggestionsListView;
        private EditText searchSrcTextView;
        private ImageButton backBtn;
        private ImageButton emptyBtn;
        private RelativeLayout searchTopBar;

        private ICharSequence oldQueryText;
        private ICharSequence userQuery;
        
        public IOnSearchViewListener OnSearchViewListener { get; set; }
        public IOnQueryChangeListener OnQueryChangeListener { get; set; }

        public CnblogsSearchView(Context context) : this(context, null) { }

        public CnblogsSearchView(Context context, IAttributeSet attrs) : this(context, attrs, 0) { }

        public CnblogsSearchView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs)
        {
            this.context = context;

            InitiateView();
        }
        private void InitiateView()
        {
            LayoutInflater.From(context).Inflate(Resource.Layout.search_view, this, true);
            searchLayout = FindViewById(Resource.Id.search_layout);

            searchTopBar = (RelativeLayout)searchLayout.FindViewById(Resource.Id.search_top_bar);
            suggestionsListView = (ListView)searchLayout.FindViewById(Resource.Id.suggestion_list);
            searchSrcTextView = (EditText)searchLayout.FindViewById(Resource.Id.searchTextView);
            backBtn = (ImageButton)searchLayout.FindViewById(Resource.Id.action_up_btn);
            emptyBtn = (ImageButton)searchLayout.FindViewById(Resource.Id.action_empty_btn);
            tintView = searchLayout.FindViewById(Resource.Id.transparent_view);

            searchSrcTextView.SetOnClickListener(this);
            backBtn.SetOnClickListener(this);
            emptyBtn.SetOnClickListener(this);
            tintView.SetOnClickListener(this);
            
            InitSearchView();

            suggestionsListView.Visibility = ViewStates.Gone;
        }
        private void InitSearchView()
        {
            searchSrcTextView.SetOnEditorActionListener(this);
            searchSrcTextView.AddTextChangedListener(this);
            searchSrcTextView.OnFocusChangeListener = this;
        }
        public void SetMenuItem(IMenuItem menuItem)
        {
            this.menuItem = menuItem;
            menuItem.SetOnMenuItemClickListener(this);
        } 

        #region Listener
        public bool OnEditorAction(TextView v, [GeneratedEnum] ImeAction actionId, KeyEvent e)
        {
            OnSubmitQuery();
            return true;
        }

        #region ITextWatcher
        public void AfterTextChanged(IEditable s)
        {
        }

        public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
        {
        }

        public void OnTextChanged(ICharSequence s, int start, int before, int count)
        {
            userQuery = s;
            //startFilter(s);
            this.OnTextChanged(s);
        }
        #endregion

        public void OnFocusChange(View v, bool hasFocus)
        {
            if (hasFocus)
            {
                ShowKeyboard(searchSrcTextView);
                //ShowSuggestions();
            }
        }

        public void OnClick(View v)
        {
        }
        public bool OnMenuItemClick(IMenuItem item)
        {
            ShowSearch();
            return true;
        }
        #endregion

        private void OnSubmitQuery()
        {
            ICharSequence query = searchSrcTextView.TextFormatted;
            if (query != null && TextUtils.GetTrimmedLength(query) > 0)
            {
                if (OnQueryChangeListener == null || !OnQueryChangeListener.OnQueryTextSubmit(query.ToString()))
                {
                    CloseSearch();
                    searchSrcTextView.Text = null;
                }
            }
        }

        private void OnTextChanged(ICharSequence newText)
        {
            ICharSequence text = searchSrcTextView.TextFormatted;
            userQuery = text;
            bool hasText = !TextUtils.IsEmpty(text);
            if (hasText)
            {
                emptyBtn.Visibility = ViewStates.Visible;
            }
            else
            {
                emptyBtn.Visibility = ViewStates.Gone;
            }

            if (OnQueryChangeListener != null && !TextUtils.Equals(newText, oldQueryText))
            {
                OnQueryChangeListener.OnQueryTextChange(newText.ToString());
            }
            oldQueryText = newText;
        }
        private void ShowSearch(bool animate = true)
        {
            if (isSearchOpen)
            {
                return;
            }

            //Request Focus
            searchSrcTextView.Text = null;
            searchSrcTextView.RequestFocus();
               
            //if (animate)
            //{
            //    //SetVisibleWithAnimation();
            //}
            //else
            //{
                searchLayout.Visibility = ViewStates.Visible;
                if (OnSearchViewListener != null)
                {
                    OnSearchViewListener.OnSearchViewShown();
                }
            //}
            isSearchOpen = true;
        }
        private void CloseSearch()
        {
            if (!isSearchOpen)
            {
                return;
            }

            searchSrcTextView.Text = null;
            //DismissSuggestions();
            ClearFocus();

            searchLayout.Visibility = ViewStates.Gone;
            if (OnSearchViewListener != null)
            {
                OnSearchViewListener.OnSearchViewClosed();
            }
            isSearchOpen = false;
        }
        public override void ClearFocus()
        {
            clearingFocus = true;
            HideKeyboard(this);
            backBtn.ClearFocus();
            searchSrcTextView.ClearFocus();
            clearingFocus = false;
        }
        private void HideKeyboard(View view)
        {
            InputMethodManager imm = (InputMethodManager)view.Context.GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(view.WindowToken, 0);
        }
        private void ShowKeyboard(View view)
        {
            if (Build.VERSION.SdkInt <= BuildVersionCodes.GingerbreadMr1 && view.HasFocus)
            {
                view.ClearFocus();
            }
            view.RequestFocus();
            InputMethodManager imm = (InputMethodManager)view.Context.GetSystemService(Context.InputMethodService);
            imm.ShowSoftInput(view, 0);
        }
    }
}