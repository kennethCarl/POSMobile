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
using System.IO;
using Mono.Data.Sqlite;
//using SQLite;

namespace POSV1.Model
{
    class SaleDetail
    {
        //[PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int SaleMasterId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public string ItemName { get; set; }
        public string Barcode { get; set; }
        public double? RetailPrice { get; set; }
        public string SalesDateTime {get; set;}

    }
}