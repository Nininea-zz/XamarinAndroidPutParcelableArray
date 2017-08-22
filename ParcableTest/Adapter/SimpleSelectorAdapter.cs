using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using ParcableTest.Dialogs;

namespace ParcableTest.Adapter
{
    public class SimpleSelectorListAdapter : BaseAdapter<SimpleSelectorItem>, Android.Widget.IFilterable
    {
        private Activity _context;

        public List<SimpleSelectorItem> mDataset;
        public List<SimpleSelectorItem> mDatasetFileted { get; set; }

        private LayoutInflater mInflater;

        public SimpleSelectorListAdapter(Context context, List<SimpleSelectorItem> data)
        {
            mDataset = data;
            mDatasetFileted = data;

            mInflater = LayoutInflater.From(context);
        }
        public override int Count
        {
            get { return mDatasetFileted.Count(); }
        }

        public Filter Filter => new SimpleSelectorListFilter(this);

        public override SimpleSelectorItem this[int position]
        {
            get { return mDatasetFileted[position]; }
        }

        public EventHandler DataSetChanged;

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            SimpleSelectorItemViewHolder holder;

            if (convertView == null)
            {
                convertView = mInflater.Inflate(Resource.Layout.SelectorItem, null);
                holder = new SimpleSelectorItemViewHolder();
                holder.Name = (TextView)convertView.FindViewById(Resource.Id.txtName);

                convertView.Tag = holder;
            }
            else
            {
                holder = (SimpleSelectorItemViewHolder)convertView.Tag;
            }

            if (position < mDatasetFileted.Count)
            {
                holder.Name.Text = mDatasetFileted[position].Name;
            }

            return convertView;
        }
    }

    public class SimpleSelectorListFilter : Filter
    {
        SimpleSelectorListAdapter _adapter;

        public SimpleSelectorListFilter(SimpleSelectorListAdapter adapter)
        {
            _adapter = adapter;
        }

        protected override FilterResults PerformFiltering(ICharSequence constraint)
        {
            string charString = constraint?.ToString()?.ToLower();

            if (string.IsNullOrWhiteSpace(charString))
            {
                _adapter.mDatasetFileted = _adapter.mDataset;
            }
            else
            {
                _adapter.mDatasetFileted = new List<SimpleSelectorItem>();
                _adapter.mDatasetFileted
                        .AddRange(_adapter.mDataset
                                  .Where(i => (i.Name != null && i.Name.ToLower().Contains(charString))));
            }

            FilterResults filterResults = new FilterResults();
            filterResults.Values = new JavaList<SimpleSelectorItem>(_adapter.mDatasetFileted);
            return filterResults;

        }

        protected override void PublishResults(ICharSequence constraint, FilterResults results)
        {
            _adapter.mDatasetFileted = ((JavaList<SimpleSelectorItem>)results.Values).ToList();
            _adapter.NotifyDataSetChanged();
        }
    }

    public class SimpleSelectorItemViewHolder : Java.Lang.Object
    {
        public TextView Name { get; set; }
    }
}
