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
using Android.Support.V7.App;
using System.IO;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using ZXing.Mobile;
using POSV1.Model;
using Mono.Data.Sqlite;
using System.Data;
using System.Threading;
using Android.Support.Design.Widget;

namespace POSV1
{
    [Activity(MainLauncher = true, Theme = "@style/AppTheme")]

    public class PreMainActivity : ActionBarActivity
    {
        public static string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "my_store.db");

        private Button btnButton1;
        private Button btnButton2;
        private string command;
        private ProgressDialog progressBar;
        private LinearLayout parentLayout;
        private SQLiteADO sqliteADO = new SQLiteADO();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.PreMain);

            btnButton1 = FindViewById<Button>(Resource.Id.btnButton1);
            btnButton2 = FindViewById<Button>(Resource.Id.btnButton2);
            parentLayout = FindViewById<LinearLayout>(Resource.Id.parentLayout);
            this.initializeProgressBar();

            try
            {
                if (!File.Exists(dbPath))
                {
                    btnButton1.Click += btnAdmin_Clicked;
                    btnButton2.Click += btnCode_Clicked;

                    Response createDatabase = new Response();
                    Response createTable = new Response();
                    Response insertConfiguration = new Response();

                    createDatabase = sqliteADO.CreateDatabase("my_store.db");
                    this.progressBar.SetMessage("Creating Database...");
                    this.progressBar.Show();
                    if (createDatabase.status.Equals("SUCCESS"))
                    {
                        this.progressBar.Progress = 15;
                        this.progressBar.SetMessage("Creating Configuration Table...");
                        createTable = sqliteADO.CreateTable("CREATE TABLE Configuration (Id INTEGER PRIMARY KEY AUTOINCREMENT , Description TEXT, IsGranted INTEGER);", dbPath);
                        if (createTable.status.Equals("SUCCESS"))
                        {
                            this.progressBar.Progress += 15;
                            this.progressBar.SetMessage("Creating Item Table...");
                            createTable = sqliteADO.CreateTable("CREATE TABLE Item (Id INTEGER PRIMARY KEY AUTOINCREMENT, Barcode TEXT, NAME TEXT, PurchasedPrice REAL, RetailPrice REAL, AvailableStock INTEGER, AverageCost REAL);", dbPath);
                            if (createTable.status.Equals("SUCCESS"))
                            {
                                this.progressBar.Progress += 15;
                                this.progressBar.SetMessage("Creating SaleMaster Table...");
                                createTable = sqliteADO.CreateTable("CREATE TABLE SaleMaster (Id INTEGER PRIMARY KEY AUTOINCREMENT, SalesDateTime TEXT, Amount REAL, SoldBy TEXT, SoldTo TEXT);", dbPath);
                                if (createTable.status.Equals("SUCCESS"))
                                {
                                    this.progressBar.Progress += 15;
                                    this.progressBar.SetMessage("Creating SaleDetail Table...");
                                    createTable = sqliteADO.CreateTable("CREATE TABLE SaleDetail (Id INTEGER PRIMARY KEY AUTOINCREMENT, SaleMasterId INTEGER, ItemId INTEGER, Quantity INTEGER);", dbPath);
                                    if (createTable.status.Equals("SUCCESS"))
                                    {
                                        this.progressBar.Progress += 15;
                                        this.progressBar.SetMessage("Inserting data to Configuration Table...");
                                        command = "INSERT INTO Configuration(Description, IsGranted) VALUES('Activated', 0);" + "\n";
                                        command += "INSERT INTO Configuration(Description, IsGranted) VALUES('AllowScanning', 0);" + "\n";
                                        command += "INSERT INTO Configuration(Description, IsGranted) VALUES('AllowEmailing', 0);" + "\n";

                                        insertConfiguration = sqliteADO.ExecuteNonQuery("BEGIN TRANSACTION;" + "\n" + command + "COMMIT;", dbPath);
                                        if (!insertConfiguration.status.Equals("SUCCESS"))
                                        {
                                            this.showMessage("Configuration(Insert): " + createDatabase.message);
                                        }
                                        else
                                        {
                                            this.progressBar.Progress += 25;
                                            this.progressBar.Dismiss();
                                            // Instantiate the delegate
                                            Del handler = SnackBarHideMethod;
                                            this.showSnackBarWithOption("Succesfully Installed", "Close", handler);
                                        }
                                    }
                                    else
                                    {
                                        this.showMessage(createDatabase.message);
                                    }
                                }
                                else
                                {
                                    this.showMessage(createDatabase.message);
                                }
                            }
                            else
                            {
                                this.showMessage(createDatabase.message);
                            }
                        }
                        else
                        {
                            this.showMessage(createDatabase.message);
                        }
                    }
                    else
                    {
                        this.showMessage(createDatabase.message);
                    }
                }
                else
                {
                    Response getAccess = new Response();
                    Configuration configurationModel = new Configuration();
                    getAccess = configurationModel.GetAllConfiguration("my_store.db");

                    // get access
                    if (getAccess.status.Equals("SUCCESS"))
                    {
                        if (getAccess.configurationList.ElementAt(0).IsGranted == 1)
                        {
                            btnButton1.Text = "Continue";
                            btnButton2.Text = "Configure";

                            btnButton1.Click += btnContinue_Clicked;
                            btnButton2.Click += btnConfigure_Clicked;
                        }
                        else
                        {
                            btnButton1.Click += btnAdmin_Clicked;
                            btnButton2.Click += btnCode_Clicked;
                        }
                    }
                    else
                    {
                        this.showMessage(getAccess.message);
                    }
                }
            }
            catch
            {
                this.showMessage("An error has occured in creating database.");
            }
        }
        public delegate void Del();
        public static void SnackBarHideMethod()
        {
            //Do something here...
        }
        public void initializeProgressBar()
        {
            this.progressBar = new ProgressDialog(this);
            this.progressBar.SetCancelable(false);
            this.progressBar.SetProgressStyle(ProgressDialogStyle.Horizontal);
            this.progressBar.Progress = 0;
            this.progressBar.Max = 100;
        }
        public void showMessage(string message)
        {
            if (this.progressBar != null)
            {
                this.progressBar.Dismiss();
            }
            var builder = new AlertDialog.Builder(this);
            builder.SetTitle("Application Message");
            builder.SetMessage(message);
            builder.SetPositiveButton("OK", delegate { });
            builder.Create().Show();
        }
        public void showSnackBar(string message)
        {
            Snackbar
            .Make(this.parentLayout, message, Snackbar.LengthIndefinite)
                //.SetAction("Undo", (view) => { /*Undo message sending here.*/ })
            .Show(); // Don’t forget to show!});
        }
        public void showSnackBarWithOption(string message, string optionDesc, Del callBack)
        {
            Snackbar
            .Make(this.parentLayout, message, Snackbar.LengthIndefinite)
            .SetAction(optionDesc, (view) => { callBack(); })
            .Show();
        }
        public async void btnAdmin_Clicked(object sender, EventArgs e)
        {
            #if __ANDROID__
            // Initialize the scanner first so it can track the current context
            MobileBarcodeScanner.Initialize(Application);
            #endif
            var scanner = new ZXing.Mobile.MobileBarcodeScanner();
            scanner.AutoFocus();
            // Create your application here
            var result = await scanner.Scan();
            if (result != null)
            {
                if (result.ToString().Equals("ARJOCAMAHAMAGEMYSTORE"))
                {
                    var intentConfigureActivity = new Intent(this, typeof(ConfigureActivity));
                    intentConfigureActivity.PutExtra("DBPATH", dbPath);
                    StartActivity(intentConfigureActivity);
                }
                else
                {
                    this.showMessage("Request is not authenticated.");
                }
            }
        }
        public void btnCode_Clicked(object sender, EventArgs e)
        {

        }
        public void btnContinue_Clicked(object sender, EventArgs e)
        {
            var intentMainActivity = new Intent(this, typeof(MainActivity));
            StartActivity(intentMainActivity);
        }
        public async void btnConfigure_Clicked(object sender, EventArgs e)
        {
            #if __ANDROID__
            // Initialize the scanner first so it can track the current context
            MobileBarcodeScanner.Initialize(Application);
            #endif
            var scanner = new ZXing.Mobile.MobileBarcodeScanner();
            scanner.AutoFocus();
            // Create your application here
            var result = await scanner.Scan();
            if (result != null)
            {
                if (result.ToString().Equals("ARJOCAMAHAMAGEMYSTORE"))
                {
                    var intentConfigureActivity = new Intent(this, typeof(ConfigureActivity));
                    intentConfigureActivity.PutExtra("DBPATH", dbPath);
                    StartActivity(intentConfigureActivity);
                }
                else
                {
                    this.showMessage("Configuration is not authenticated.");
                }
            }
        }
    }
}