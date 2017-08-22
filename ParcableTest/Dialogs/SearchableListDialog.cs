using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using ParcableTest.Adapter;
using Object = Java.Lang.Object;

namespace ParcableTest.Dialogs
{
    public class SearchableListDialog : DialogFragment, SearchView.IOnQueryTextListener, SearchView.IOnCloseListener
    {
        const string ITEMS = "items";


        string _strTitle;

        string _strPositiveButtonText;

        SearchView _searchView;

        ListView _lvSelector;


        IOnSearchTextChanged _onSearchTextChanged;

        ISearchableItem<SimpleSelectorItem> _searchableItem;

        //Context _context;
        List<SimpleSelectorItem> _items = new System.Collections.Generic.List<SimpleSelectorItem>();

        public static SearchableListDialog newInstance(SimpleSelectorItem[] items)
        {

            SearchableListDialog multiSelectExpandableFragment = new
                    SearchableListDialog();
            
            Bundle args = new Bundle();
            args.PutParcelableArray(ITEMS, items);
            multiSelectExpandableFragment.Arguments = args;

            return multiSelectExpandableFragment;
        }

        public override Dialog OnCreateDialog(Android.OS.Bundle savedInstanceState)
        {
            LayoutInflater inflater = Activity.LayoutInflater;

            if (null != savedInstanceState)
            {
                _searchableItem = (ISearchableItem<SimpleSelectorItem>)savedInstanceState.GetSerializable("item");
            }
            // Change End

            View rootView = inflater.Inflate(Resource.Layout.SearchableDialog, null);
            SetData(rootView);

            AlertDialog.Builder alertDialog = new AlertDialog.Builder(Activity);
            alertDialog.SetView(rootView);

            string strPositiveButton = _strPositiveButtonText == null ? "CLOSE" : _strPositiveButtonText;
            alertDialog.SetPositiveButton(_strPositiveButtonText, (sender, e) => { });

            string strTitle = _strTitle == null ? "Select Item" : _strTitle;
            alertDialog.SetTitle(strTitle);

            AlertDialog dialog = alertDialog.Create();
            dialog.Window.SetSoftInputMode(SoftInput.StateHidden);
            return dialog;
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutSerializable("item", _searchableItem);
            base.OnSaveInstanceState(outState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Dialog.Window.SetSoftInputMode(SoftInput.StateHidden);
            return base.OnCreateView(inflater, container, savedInstanceState);
        }

        public void SetTitle(string strTitle)
        {
            _strTitle = strTitle;
        }

        public void SetPositiveButton(string strPositiveButtonText)
        {
            _strPositiveButtonText = strPositiveButtonText;
        }

        //public void SetPositiveButton(String strPositiveButtonText, Android.Content.IDialogInterface..OnClickListener onClickListener)
        //{
        //    _strPositiveButtonText = strPositiveButtonText;
        //    _onClickListener = onClickListener;
        //}

        public void SetOnSearchableItemClickListener(ISearchableItem<SimpleSelectorItem> searchableItem)
        {
            this._searchableItem = searchableItem;
        }

        public void setOnSearchTextChangedListener(IOnSearchTextChanged onSearchTextChanged)
        {
            this._onSearchTextChanged = onSearchTextChanged;
        }


        private void SetData(View rootView)
        {
            SearchManager searchManager = (SearchManager)Activity.GetSystemService(Android.Content.Context.SearchService);

            _searchView = rootView.FindViewById<SearchView>(Resource.Id.search);
            _searchView.SetSearchableInfo(searchManager.GetSearchableInfo(Activity.ComponentName));

            _searchView.SetIconifiedByDefault(false);
            _searchView.SetOnQueryTextListener(this);
            _searchView.SetOnCloseListener(this);
            _searchView.ClearFocus();

            Android.Views.InputMethods.InputMethodManager mgr = (Android.Views.InputMethods.InputMethodManager)Activity.GetSystemService(Android.Content.Context.InputMethodService);
            mgr.HideSoftInputFromWindow(_searchView.WindowToken, 0);
            SimpleSelectorItem[] items = (ParcableTest.Dialogs.SimpleSelectorItem[])Arguments.GetParcelableArray(ITEMS);

            _lvSelector = (ListView)rootView.FindViewById(Resource.Id.lvSelector);
            SimpleSelectorListAdapter adapter = new SimpleSelectorListAdapter(Activity, items.ToList());
            _lvSelector.Adapter = adapter;
        }


        public bool OnQueryTextChange(string newText)
        {
            ((SimpleSelectorListAdapter)_lvSelector.Adapter).Filter.InvokeFilter(newText);

            if (null != _onSearchTextChanged)
            {
                _onSearchTextChanged.OnSearchTextChanged(newText);
            }

            return true;
        }

        public bool OnQueryTextSubmit(string query)
        {
            _searchView.ClearFocus();
            return true;
        }

        public bool OnClose()
        {
            return false;
        }
    }
    public interface ISearchableItem<T> : ISerializable
    {
        void OnSearchableItemClicked(T item, int position);
    }

    public interface IOnSearchTextChanged
    {
        void OnSearchTextChanged(string strText);
    }

    public class SimpleSelectorItem : Object, IParcelable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Selected { get; set; }

        public SimpleSelectorItem()
        {

        }

        public SimpleSelectorItem(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public SimpleSelectorItem(Parcel parcel)
        {
            Id = parcel.ReadInt();
            Name = parcel.ReadString();
        }

        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
            dest.WriteInt(Id);
            dest.WriteString(Name);
        }

        // static readonly GenericParcelableCreator<SimpleSelectorItem> _creator
        //= new GenericParcelableCreator<SimpleSelectorItem>((parcel) => new SimpleSelectorItem(parcel));

        [Java.Interop.ExportField("CREATOR")]
        public static MyParcelableCreator InitializeCreator()
        {
            return new MyParcelableCreator();
        }

        public int DescribeContents()
        {
            return 0;
        }
    }

    public class MyParcelableCreator : Object, IParcelableCreator
    {
        Object IParcelableCreator.CreateFromParcel(Parcel source)
        {
            return new SimpleSelectorItem(source);
        }

        Object[] IParcelableCreator.NewArray(int size)
        {
            return new SimpleSelectorItem[size];
        }
    }
}
