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
using POSV1.Model;

namespace POSV1
{
    class Response
    {
        public int param1 { get; set; }
        public int param2 { get; set; }
        public double dblParam1 { get; set; }
        public string strParam1 { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public List<Configuration> configurationList;
        public List<Item> itemList;
        public List<SaleMaster> saleList;
        public List<SaleDetail> saleDetailList;
    }
}