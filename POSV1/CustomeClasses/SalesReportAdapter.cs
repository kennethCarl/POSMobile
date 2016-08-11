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
using Android.Support.V7.Widget;

namespace POSV1.CustomeClasses
{
    class SalesReportAdapter: BaseAdapter<SalesReportItem>
    {
        private List<SalesReportItem> items;
        Activity context;
        public SalesReportAdapter(Activity context, List<SalesReportItem> items)
            : base()
        {
            this.context = context;
            this.items = items;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override SalesReportItem this[int position]
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

            view = context.LayoutInflater.Inflate(Resource.Layout.SalesReportRowLayout, null);
            view.FindViewById<TextView>(Resource.Id.tvSalesDate).Text = view.FindViewById<TextView>(Resource.Id.tvSalesDate).Text + " " + item.salesDate;
            view.FindViewById<TextView>(Resource.Id.tvNoOfSales).Text = view.FindViewById<TextView>(Resource.Id.tvNoOfSales).Text + " " + item.noOfSales;
            view.FindViewById<TextView>(Resource.Id.tvNoOfItems).Text = view.FindViewById<TextView>(Resource.Id.tvNoOfItems).Text + " " + item.salesNoOfItems;
            view.FindViewById<TextView>(Resource.Id.tvTotalSales).Text = view.FindViewById<TextView>(Resource.Id.tvTotalSales).Text + " " + item.salesTotalAmount;
            if (item.type == 1)//Sales By Date
            {
                view.FindViewById<TextView>(Resource.Id.tvVendor).Visibility = ViewStates.Gone;
                view.FindViewById<TextView>(Resource.Id.tvConsumer).Visibility = ViewStates.Gone;
            }
            else if (item.type == 2)//Sales By Consumer
            {
                view.FindViewById<TextView>(Resource.Id.tvVendor).Visibility = ViewStates.Gone;
                view.FindViewById<TextView>(Resource.Id.tvConsumer).Visibility = ViewStates.Visible;
                view.FindViewById<TextView>(Resource.Id.tvConsumer).Text = view.FindViewById<TextView>(Resource.Id.tvConsumer).Text + " " + item.salesConsumer;
            }
            else//Sales By Vendor
            {
                view.FindViewById<TextView>(Resource.Id.tvVendor).Visibility = ViewStates.Visible;
                view.FindViewById<TextView>(Resource.Id.tvConsumer).Visibility = ViewStates.Gone;
                view.FindViewById<TextView>(Resource.Id.tvVendor).Text = view.FindViewById<TextView>(Resource.Id.tvVendor).Text + " " + item.salesVendor;
            }
            return view;
        }
    }
}