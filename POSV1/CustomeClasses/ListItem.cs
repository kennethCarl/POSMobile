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
    class ListItem
    {
        public int Id { get; set; }
        public bool IsChecked { get; set; }
        public string Description { get; set; }
    }
}