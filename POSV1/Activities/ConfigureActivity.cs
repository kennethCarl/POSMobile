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
using POSV1.CustomeClasses;
using com.refractored.fab;
using System.Reflection;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using POSV1.Model;

namespace POSV1
{
    [Activity(Label = "Configure", Theme = "@style/AppTheme")]
    public class ConfigureActivity : ActionBarActivity, IScrollDirectorListener, AbsListView.IOnScrollListener
    {
        private List<ConfigureItem> items;
        private ListView lvConfigure;
        private FloatingActionButton fab;
        private SQLiteADO sqliteADO;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Configure);

            Response getAccess = new Response();
            Configuration configurationModel = new Configuration();

            lvConfigure = FindViewById<ListView>(Resource.Id.lvConfigure);
            fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            items = new List<ConfigureItem>();

            getAccess = configurationModel.GetAllConfiguration("my_store.db");
            if (getAccess.status.Equals("SUCCESS"))
            {
                foreach (Configuration conf in getAccess.configurationList)
                {
                    items.Add(new ConfigureItem() { Id = conf.Id, IsGranted = conf.IsGranted, ItemDescription = conf.Description });
                }
            }
            else
            {
                this.showMessage(getAccess.message);
            }

            lvConfigure.Adapter = new ConfigureAdapter(this, items);
            lvConfigure.ChoiceMode = ChoiceMode.Multiple;

            fab.AttachToListView(lvConfigure, this, this);
            fab.Click += fab_Clicked;
        }
        public void fab_Clicked(object sender, EventArgs e)
        {
            sqliteADO = new SQLiteADO();
            string command = "";
            for (int i = 0; i < items.Count(); i++)
            {
                Type lvType = lvConfigure.GetItemAtPosition(i).GetType();
                IList<PropertyInfo> props = new List<PropertyInfo>(lvType.GetProperties());

                foreach (PropertyInfo prop in props)
                {
                    if (prop.Name.Equals("Instance"))
                    {
                        object propValue = prop.GetValue(lvConfigure.GetItemAtPosition(i), null);
                        ConfigureItem ci = new ConfigureItem();
                        ci = (ConfigureItem)propValue;
                        command += string.Format("UPDATE Configuration SET IsGranted = {0} WHERE Id = {1};", ci.IsGranted, ci.Id) + "\n";
                        break;
                    }
                }
            }
            Response updateConfiguration = new Response();
            updateConfiguration = sqliteADO.ExecuteNonQuery("BEGIN TRANSACTION;" + command + "COMMIT;", Intent.GetStringExtra("DBPATH"));
            if (updateConfiguration.status.Equals("SUCCESS"))
            {
                var mainActivity = new Intent(this, typeof(MainActivity));
                mainActivity.PutExtra("DBPATH", Intent.GetStringExtra("DBPATH"));
                StartActivity(mainActivity);
            }
            else
            {
                this.showMessage("Configuration(Update): " + updateConfiguration.message);
            }
        }
        public void showMessage(string message)
        {
            var builder = new AlertDialog.Builder(this);
            builder.SetTitle("Application Message");
            builder.SetMessage(message);
            builder.SetPositiveButton("OK", delegate { });
            builder.Create().Show();
        }
        public void OnScroll(AbsListView view, int firstVisibleItem, int visibleItemCount, int totalItemCount)
        {

        }
        public void OnScrollDown() 
        {
            //throw new NotImplementedException();
        }
        public void OnScrollStateChanged(AbsListView view, [GeneratedEnum] ScrollState scrollState)
        {

        }
        public void OnScrollUp() 
        {
        
        }
    }
}