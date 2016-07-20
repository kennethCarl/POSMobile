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
using ZXing.Mobile;
using System.Threading.Tasks;
using Android.Support.V7.App;
using AlertDialog = Android.Support.V7.App.AlertDialog;
namespace POSV1
{
    [Activity(Label = "SalesActivity", Theme = "@style/AppTheme")]
    public class SalesActivity : ActionBarActivity 
    {
        private Button btnScan;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Sales);

            btnScan = FindViewById<Button>(Resource.Id.btnScan);
            btnScan.Click += btnScan_Clicked;
        }

        public async void btnScan_Clicked(object sender, EventArgs e)
        {
            #if __ANDROID__
            // Initialize the scanner first so it can track the current context
            MobileBarcodeScanner.Initialize(Application);
            #endif
            var scanner = new ZXing.Mobile.MobileBarcodeScanner();

            // Create your application here
            var result = await scanner.Scan();
            if (result != null)
            {
                //.WriteLine("Scanned Barcode: ");
                var builder = new AlertDialog.Builder(this);

                builder.SetTitle("Application Message")
                       .SetMessage("Scanned Barcode: " + result.ToString())
                       .SetPositiveButton("OK", delegate { });
                builder.Create().Show();
            }
        }
    }
}