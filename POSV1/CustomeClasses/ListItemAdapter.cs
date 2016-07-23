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

namespace POSV1.CustomeClasses
{
    class ListItemAdapter: BaseAdapter<ListItem>
    {
        List<ListItem> items;
        Activity context;
        List<ListItem> selectedItems = new List<ListItem>();
        List<ListItem> unSelectedItems = new List<ListItem>();
        List<int> getIndexForSelectedItems = new List<int>();
        public ListItemAdapter(Activity context, List<ListItem> items)
            : base()
        {
            this.context = context;
            this.items = items;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override ListItem this[int position]
        {
            get { return items[position]; }
        }
        public override int Count
        {
            get { return items.Count; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];
            View view;

            view = context.LayoutInflater.Inflate(Resource.Layout.WithCheckboxRowLayout, null);
            view.FindViewById<TextView>(Resource.Id.Text1).Text = item.Description;
            if (item.IsChecked == true)
            {
                view.FindViewById<CheckBox>(Resource.Id.cb).Checked = true;
                //this.unSelectedItems.Remove(item);
                //this.selectedItems.Add(item);
            }
            else
            {
                view.FindViewById<CheckBox>(Resource.Id.cb).Checked = false; 
                //this.selectedItems.Remove(item);
                //this.unSelectedItems.Add(item);
            }
            view.Tag = position;
            view.Click += new EventHandler(this.OnItemClicked);
            return view;
        }

        public void OnItemClicked(object sender, EventArgs e)
        {
            var view = (View)sender;

            if (view.FindViewById<CheckBox>(Resource.Id.cb).Checked)
            {
                this.selectedItems.Remove(this[Convert.ToInt16(view.Tag)]);
                view.FindViewById<CheckBox>(Resource.Id.cb).Checked = false;
                this[Convert.ToInt16(view.Tag)].IsChecked = false;
                this.unSelectedItems.Add(this[Convert.ToInt16(view.Tag)]);
            }
            else
            {
                this.unSelectedItems.Remove(this[Convert.ToInt16(view.Tag)]);
                view.FindViewById<CheckBox>(Resource.Id.cb).Checked = true;
                this[Convert.ToInt16(view.Tag)].IsChecked = true;
                this.selectedItems.Add(this[Convert.ToInt16(view.Tag)]);
            }
        }

        public List<ListItem> getSelectedItems()
        {
            this.selectedItems.Clear();
            this.getIndexForSelectedItems.Clear();
            for (int i = 0; i < this.items.Count; i++)
            {
                if(this.items[i].IsChecked)
                {
                    this.selectedItems.Add(this.items[i]);
                    this.getIndexForSelectedItems.Add(i);
                }
            }
            return this.selectedItems;
        }
        public List<int> indexOfSelecteItems()
        {
            return this.getIndexForSelectedItems;
        }
        public List<ListItem> getUnSelectedItems()
        {
            this.unSelectedItems.Clear();
            for (int i = 0; i < this.items.Count; i++)
            {
                if (!this.items[i].IsChecked)
                {
                    this.unSelectedItems.Add(this.items[i]);
                }
            }
            return this.unSelectedItems;
        }
        public void uncheckedAllItems() 
        {
            this.selectedItems.Clear();
            this.unSelectedItems = this.items;
        }
        public void checkedAllItems()
        {
            this.unSelectedItems.Clear();
            this.selectedItems = this.items;
        }
        public void resetInstance()
        {
            this.selectedItems.Clear();
            this.unSelectedItems.Clear();
        }
    }
}