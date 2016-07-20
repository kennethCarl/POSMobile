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
using AlertDialog = Android.Support.V7.App.AlertDialog;
namespace POSV1.CustomeClasses
{
    class ConfigureAdapter : BaseAdapter<ConfigureItem>
    {
        List<ConfigureItem> items;
        Activity context;
        public ConfigureAdapter(Activity context, List<ConfigureItem> items)
            : base()
        {
            this.context = context;
            this.items = items;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override ConfigureItem this[int position]
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
            View view = convertView;
            if (view == null) // no view to re-use, create new
            {
                view = context.LayoutInflater.Inflate(Resource.Layout.WithCheckboxRowLayout, null);
                view.FindViewById<TextView>(Resource.Id.Text1).Text = item.ItemDescription;
                if (item.IsGranted == 1)
                {
                    view.FindViewById<CheckBox>(Resource.Id.cb).Checked = true;
                }
                else
                {
                    view.FindViewById<CheckBox>(Resource.Id.cb).Checked = false;
                }
                view.Tag = position;
                view.Click += new EventHandler(this.OnItemClicked);
            }
            
            return view;
        }

        public void OnItemClicked(object sender, EventArgs e)
        {
            var view = (View)sender;

            if (view.FindViewById<CheckBox>(Resource.Id.cb).Checked)
            {
                view.FindViewById<CheckBox>(Resource.Id.cb).Checked = false;
                this[Convert.ToInt16(view.Tag)].IsGranted = 0;
            }
            else
            {
                view.FindViewById<CheckBox>(Resource.Id.cb).Checked = true;
                this[Convert.ToInt16(view.Tag)].IsGranted = 1;
            }
            //var builder = new AlertDialog.Builder(this.context);
            //builder.SetTitle("");
            //builder.SetMessage("Test");
            //builder.SetPositiveButton("ok", delegate { view.FindViewById<CheckBox>(Resource.Id.cb).Checked = true; });
            //builder.Create().Show();
        } 
    }
}