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

namespace POSV1.Model
{
    class SaleDetailWithItem
    {
        public int ID { get; set; }
        public int SALEID { get; set; }
        public int ITEMID { get; set; }
        public int QUANTITY { get; set; }
        public string NAME { get; set; }
        public Nullable<double> RETAILPRICE { get; set; }
    }
}