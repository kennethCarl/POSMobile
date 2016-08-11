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
    class SalesReportItem
    {
        public string salesDate { get; set; }
        public string noOfSales { get; set; }
        public string salesNoOfItems { get; set; }
        public string salesTotalAmount { get; set; }
        public string salesConsumer { get; set; }
        public string salesVendor { get; set; }
        public int type { get; set; }
    }
}