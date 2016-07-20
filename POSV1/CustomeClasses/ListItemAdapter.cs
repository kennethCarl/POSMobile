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
            }
            else
            {
                view.FindViewById<CheckBox>(Resource.Id.cb).Checked = false;
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
                view.FindViewById<CheckBox>(Resource.Id.cb).Checked = false;
                this[Convert.ToInt16(view.Tag)].IsChecked = false;
            }
            else
            {
                view.FindViewById<CheckBox>(Resource.Id.cb).Checked = true;
                this[Convert.ToInt16(view.Tag)].IsChecked = true;
            }
        } 
    }
}