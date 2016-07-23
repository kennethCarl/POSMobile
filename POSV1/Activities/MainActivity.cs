using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using Android.Support.V4.Widget;
using System.Collections.Generic;
using POSV1.CustomeClasses;
using POSV1;
using System.Drawing;
using Android.Graphics.Drawables;
using ZXing.Mobile;
using System.IO;
using Android.Support.Design.Widget;
using System.Threading.Tasks;
using POSV1.Model;
using Java.Lang;
using com.refractored.fab;
using Android.Views.Animations;
using System.Threading;

namespace POSV1
{
    [Activity(Theme = "@style/AppTheme")]
    public class MainActivity : ActionBarActivity//, IScrollDirectorListener, AbsListView.IOnScrollListener
    {
        public static string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "my_store.db");
        private SupportToolbar mToolbar;
        private AppActionBarDrawerToggle mDrawerToggle;
        private DrawerLayout mDrawerLayout;
        private ListView mRightDrawer, mLeftDrawer, lvItems;
        private List<TableItem> tableItems, tableItemOptions;
        private GridLayout glAbout;
        private ScrollView glInventory;
        private Button btnEvaluateItem, btnClearItem;
        private IMenu menu;
        private ProgressDialog progressBar;
        private string requestType, evaluationType, command;
        private EditText etPurchasedPrice, etRetailPrice, etStocks, etAvarageCost, etBarcode;
        private AutoCompleteTextView actvItemName;
        private LinearLayout parentLayout;
        private SQLiteADO sqliteADO;
        private double? oldPrice = 0.0;
        private int? oldStock = 0;
        private int currentRetrieveItems = 0;
        private List<ListItem> itemListDisplay = new List<ListItem>();
        private FrameLayout flInvetoryList;
        private com.refractored.fab.FloatingActionButton fabItem;
        private AlertDialog.Builder popupBuilder;
        private AlertDialog popupDialog;
        private ListItemAdapter listItemAdapter;
        private string filterValue = "";
        private List<ListItem> selectedItems = new List<ListItem>();
        private System.Threading.Thread hideFabThread;
        private Animation mainFab_show;
        private Animation mainFab_hide;
        private Animation childFab_show;
        private Animation childFab_hide;
        private com.refractored.fab.FloatingActionButton mainFab;
        private FrameLayout.LayoutParams mainLayoutParams;
        private TextView tvShowMore;
        private FrameLayout.LayoutParams tvShowMoreLayoutParams;
        private com.refractored.fab.FloatingActionButton showDetailFab;
        private FrameLayout.LayoutParams showDetailFabLayoutParams;
        private TextView tvShowDetail;
        private FrameLayout.LayoutParams tvShowDetailLayoutParams;
        private com.refractored.fab.FloatingActionButton editSelectedFab;
        private FrameLayout.LayoutParams editSelectedFabLayoutParams;
        private TextView tvEditSelected;
        private FrameLayout.LayoutParams tvEditSelectedLayoutParams;
        private com.refractored.fab.FloatingActionButton deleteSelectedFab;
        private FrameLayout.LayoutParams deleteSelectedFabLayoutParams;
        private TextView tvDeleteSelected;
        private FrameLayout.LayoutParams tvDeleteSelectedLayoutParams;
        private com.refractored.fab.FloatingActionButton uncheckAllFab;
        private FrameLayout.LayoutParams uncheckAllFabLayoutParams;
        private TextView tvUncheckAll;
        private FrameLayout.LayoutParams tvUncheckAllLayoutParams;
        private com.refractored.fab.FloatingActionButton checkAllFab;
        private FrameLayout.LayoutParams checkAllFabLayoutParams;
        private TextView tvCheckAll;
        private FrameLayout.LayoutParams tvCheckAllLayoutParams;
        private TextView tvSearch;
        private FrameLayout.LayoutParams etSearchLayoutParams;
        private Dialog popupMenuDialog;
        private TextView tvIndicator;
        private bool isAllowedToCloseDialog = true;
        private int countDialogShown;
        private Item itemHolder = new Item();
        private bool scannedState = false;
        private bool canScan = false;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            mToolbar = FindViewById<SupportToolbar>(Resource.Id.toolBar);
            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            mLeftDrawer = FindViewById<ListView>(Resource.Id.left_drawer);
            mRightDrawer = FindViewById<ListView>(Resource.Id.right_drawer);
            btnEvaluateItem = FindViewById<Button>(Resource.Id.btnEvaluateItem);
            btnClearItem = FindViewById<Button>(Resource.Id.btnClearItem);
            etPurchasedPrice = FindViewById<EditText>(Resource.Id.etPurchasedPrice);
            etRetailPrice = FindViewById<EditText>(Resource.Id.etRetailPrice);
            etStocks = FindViewById<EditText>(Resource.Id.etAvailableStocks);
            etAvarageCost = FindViewById<EditText>(Resource.Id.etAverageCost);
            etBarcode = FindViewById<EditText>(Resource.Id.etBarcode);
            actvItemName = FindViewById<AutoCompleteTextView>(Resource.Id.atcvDescription);
            fabItem = FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.fabItem);
            parentLayout = FindViewById<LinearLayout>(Resource.Id.parentLayout);
            glAbout = FindViewById<GridLayout>(Resource.Id.About);
            flInvetoryList = FindViewById<FrameLayout>(Resource.Id.InventoryList);
            glInventory = FindViewById<ScrollView>(Resource.Id.Inventory);
            lvItems = FindViewById<ListView>(Resource.Id.lvList);

            //Animations
            mainFab_show = AnimationUtils.LoadAnimation(this, Resource.Layout.mainFab_show);
            mainFab_hide = AnimationUtils.LoadAnimation(this, Resource.Layout.mainFab_hide);
            childFab_show = AnimationUtils.LoadAnimation(this, Resource.Layout.childFab_show);
            childFab_hide = AnimationUtils.LoadAnimation(this, Resource.Layout.childFab_hide);

            mLeftDrawer.Tag = 0;
            mRightDrawer.Tag = 1;

            lvItems.ChoiceMode = ChoiceMode.Multiple;

            fabItem.Click += fabItem_Clicked;
            mLeftDrawer.ItemClick += mLeftDrawer_ItemClicked;
            mRightDrawer.ItemClick += mRightDrawer_ItemClicked;

            this.setTableItems();
            this.setTableItemOptions();

            mLeftDrawer.Adapter = new LeftDrawerAdapter(this, tableItems);
            mRightDrawer.Adapter = new RightDrawerAdapter(this, tableItemOptions);
            mDrawerToggle = new AppActionBarDrawerToggle(
                    this, //Action Bar Activity
                    mDrawerLayout, // Drawer Layout
                    Resource.String.LeftDrawerDescWhenOpen,
                    Resource.String.LeftDrawerDescWhenClose
                );

            SetSupportActionBar(mToolbar);
            //Listens if drawer is open or closed
            mDrawerLayout.SetDrawerListener(mDrawerToggle);
            //Displays the toggle button
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            //Enables the opening or closing of drawer
            SupportActionBar.SetHomeButtonEnabled(true);
            //Enables Toolbar title
            SupportActionBar.SetDisplayShowTitleEnabled(true);
            mDrawerToggle.SyncState();

            this.initializeToolbarTitle(bundle);
            this.getItems("SELECT * FROM Item");
        }
        public void hideLayouts()
        {
            scannedState = false;
            glAbout.Visibility = ViewStates.Gone;
            glInventory.Visibility = ViewStates.Gone;
            flInvetoryList.Visibility = ViewStates.Gone;
            this.hideOptionMenu(Resource.Id.action_more);
            this.hideOptionMenu(Resource.Id.action_refresh);
        }
        public string formatCurrency(string value)
        {
            string[] valueHolder = value.Split('.');
            string leftDecimal = "", leftDecimalFinal = "", rightDecimal = "";
            int counter = 0;
            valueHolder[0] = valueHolder[0].Replace(",", "");
            //Initialize left decimal value
            for (int i = valueHolder[0].Length - 1; i >= 0; i--)
            {
                if (counter == 3)
                {
                    leftDecimal = leftDecimal + ",";
                    leftDecimal = leftDecimal + valueHolder[0][i];
                    counter = 0;
                }
                else
                {
                    leftDecimal = leftDecimal + valueHolder[0][i];
                    counter = counter + 1;
                }
            }

            for (int i = leftDecimal.Length - 1; i >= 0; i--)
                leftDecimalFinal = leftDecimalFinal + leftDecimal[i];

            if (valueHolder.Length == 1)
                rightDecimal = ".00";
            else
            {
                rightDecimal = ".";
                for (int i = 0; i < (valueHolder[1].Length >= 2 ? 2 : valueHolder[1].Length); i++)
                    rightDecimal = rightDecimal + valueHolder[1][i];
            }

            return leftDecimalFinal + rightDecimal;
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
            .Make(this.parentLayout, message, Snackbar.LengthShort)
            .SetAction("Close", (view) => { })
            .Show(); // Don’t forget to show!});
        }
        public void showSnackBarInfinite(string message)
        {
            Snackbar
            .Make(this.parentLayout, message, Snackbar.LengthIndefinite)
            .SetAction("Close", (view) => { })
            .Show(); // Don’t forget to show!});
        }
        private void mLeftDrawer_ItemClicked(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (hideFabThread != null)
                hideFabThread.Abort();

            this.hideLayouts();

            switch (e.Position)
            {
                case 0:
                    requestType = "Show Sales";
                    evaluationType = "";
                    break;
                case 1:
                    requestType = "Show Items";
                    evaluationType = "";
                    mDrawerToggle.setCloseDrawerDesc(Resource.String.Inventory);
                    flInvetoryList.Visibility = ViewStates.Visible;
                    this.showOptionMenu(Resource.Id.action_more);
                    this.showOptionMenu(Resource.Id.action_refresh);
                    break;
                case 2:
                    mDrawerToggle.setCloseDrawerDesc(Resource.String.LeftDrawerDescWhenClose);
                    glAbout.Visibility = ViewStates.Visible;
                    break;
                default:
                    mDrawerToggle.setCloseDrawerDesc(Resource.String.Help);
                    break;
            }
            mDrawerLayout.CloseDrawer(mLeftDrawer);
        }
        private void mRightDrawer_ItemClicked(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (hideFabThread != null)
                hideFabThread.Abort();

            this.hideLayouts();

            etBarcode.Enabled = true;
            etPurchasedPrice.Enabled = true;
            etRetailPrice.Enabled = true;
            etAvarageCost.Enabled = false;
            etStocks.Enabled = true;
            if (canScan)
            {
                switch (e.Position)
                {
                    case 0://Scan
                        if (requestType.Equals("Show Items"))
                        {
                            this.btnScanItem_Clicked();
                        }
                        break;
                    case 1: //Add
                        if (requestType.Equals("Show Items"))
                        {
                            evaluationType = "Save";
                            glInventory.Visibility = ViewStates.Visible;
                            this.showOptionMenu(Resource.Id.action_more);
                            etPurchasedPrice.KeyPress += etPurchasedPrice_ItemChanged;
                            etStocks.KeyPress += etPurchasedPrice_ItemChanged;
                        }
                        break;
                    default:
                        if (requestType.Equals("Show Items"))
                        {
                            evaluationType = "ShowItemList";
                            flInvetoryList.Visibility = ViewStates.Visible;
                            this.showOptionMenu(Resource.Id.action_more);
                            this.showOptionMenu(Resource.Id.action_refresh);
                        }
                        break;
                }
            }
            else
            {
                switch (e.Position)
                {
                    case 0: //Add
                        if (requestType.Equals("Show Items"))
                        {
                            evaluationType = "Save";
                            glInventory.Visibility = ViewStates.Visible;
                            this.showOptionMenu(Resource.Id.action_more);
                            etPurchasedPrice.KeyPress += etPurchasedPrice_ItemChanged;
                            etStocks.KeyPress += etPurchasedPrice_ItemChanged;
                        }
                        break;
                    default:
                        if (requestType.Equals("Show Items"))
                        {
                            evaluationType = "ShowItemList";
                            flInvetoryList.Visibility = ViewStates.Visible;
                            this.showOptionMenu(Resource.Id.action_more);
                            this.showOptionMenu(Resource.Id.action_refresh);
                        }
                        break;
                }
            }
            btnEvaluateItem.Text = evaluationType;
            btnEvaluateItem.Click += btnEvaluateItem_Clicked;
            btnClearItem.Click += btnClearItem_Clicked;
            mDrawerLayout.CloseDrawer(mRightDrawer);
        }
        private void initializeToolbarTitle(Bundle bundleState)
        {
            if (bundleState != null)
            {
                if (bundleState.GetString("DrawerState").Equals("OpenState"))
                {
                    SupportActionBar.SetTitle(Resource.String.LeftDrawerDescWhenOpen);
                    mDrawerLayout.OpenDrawer(mLeftDrawer);
                }
                else
                {
                    if (glAbout.Visibility == ViewStates.Visible)
                    {
                        SupportActionBar.SetTitle(Resource.String.LeftDrawerDescWhenClose);
                    }
                    else if (glInventory.Visibility == ViewStates.Visible)
                    {
                        SupportActionBar.SetTitle(Resource.String.Inventory);
                    }
                    mDrawerLayout.CloseDrawer(mLeftDrawer);
                }
            }
            else
            {
                SupportActionBar.SetTitle(Resource.String.LeftDrawerDescWhenClose);
            }
        }
        private void setTableItems()
        {
            tableItems = new List<TableItem>();

            tableItems.Add(new TableItem() { ImageResourceId = Resource.Drawable.Sales, ItemLabel = "Sales" });
            tableItems.Add(new TableItem() { ImageResourceId = Resource.Drawable.Inventory, ItemLabel = "Inventory" });
            tableItems.Add(new TableItem() { ImageResourceId = Resource.Drawable.Information, ItemLabel = "About" });
            tableItems.Add(new TableItem() { ImageResourceId = Resource.Drawable.Help, ItemLabel = "Help" });
        }
        private void setTableItemOptions()
        {
            Response getAccess = new Response();
            Configuration configurationModel = new Configuration();

            tableItemOptions = new List<TableItem>();

            getAccess = configurationModel.GetAllConfiguration("my_store.db");
            if (getAccess.status.Equals("SUCCESS"))
            {
                foreach (Configuration conf in getAccess.configurationList)
                {
                    if (conf.Description.Equals("AllowScanning"))
                    {
                        if (conf.IsGranted == 1)
                        {
                            tableItemOptions.Add(new TableItem() { ImageResourceId = Resource.Drawable.Scan, ItemLabel = "Scan Item" });
                            canScan = true;
                        }
                    }
                }
            }
            else
            {
                this.showMessage(getAccess.message);
            }

            tableItemOptions.Add(new TableItem() { ImageResourceId = Resource.Drawable.Add, ItemLabel = "Add Item" });
            tableItemOptions.Add(new TableItem() { ImageResourceId = Resource.Drawable.List, ItemLabel = "Show Items" });
        }
        //function that will listen if the left toggle menu is selected
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (hideFabThread != null)
                hideFabThread.Abort();

            switch (item.ItemId)
            {
                //Left toggle resource id
                case Android.Resource.Id.Home:
                    mDrawerLayout.CloseDrawer(mRightDrawer);
                    mDrawerToggle.OnOptionsItemSelected(item);
                    return true;
                case Resource.Id.action_more:
                    if (mDrawerLayout.IsDrawerOpen(mRightDrawer))
                    {
                        mDrawerLayout.CloseDrawer(mRightDrawer);
                    }
                    else
                    {
                        if (glAbout.Visibility != ViewStates.Visible)
                        {
                            mDrawerLayout.OpenDrawer(mRightDrawer);
                            mDrawerLayout.CloseDrawer(mLeftDrawer);
                        }
                    }
                    return true;
                case Resource.Id.action_refresh:
                    var messageDialog = new AlertDialog.Builder(this);
                    messageDialog.SetMessage("Application Message");
                    messageDialog.SetMessage("Are you sure you want to refresh list?");
                    messageDialog.SetNegativeButton("No", delegate { });
                    messageDialog.SetPositiveButton("Yes", delegate
                    {
                        if (requestType.Equals("Show Items"))
                        {
                            this.itemListDisplay.Clear();

                            RunOnUiThread(() =>
                            {
                                listItemAdapter.resetInstance();
                                listItemAdapter.NotifyDataSetChanged();
                                this.currentRetrieveItems = 0;
                            });

                            if (this.filterValue.Trim().Equals(""))
                            {
                                this.getItems("SELECT * FROM Item");
                            }
                            else
                            {
                                this.getItems(string.Format("SELECT * FROM Item WHERE NAME LIKE '{0}%' OR BARCODE LIKE '{1}%'", this.filterValue, this.filterValue));
                            }
                        }
                    });
                    messageDialog.Show();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
        //function that will listen if right menus are selected
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            this.menu = menu;
            MenuInflater.Inflate(Resource.Menu.action_menu, menu);
            this.hideOptionMenu(Resource.Id.action_more);
            this.hideOptionMenu(Resource.Id.action_refresh);
            return base.OnCreateOptionsMenu(menu);
        }
        private void hideOptionMenu(int id)
        {
            IMenuItem item = menu.FindItem(id);
            item.SetVisible(false);
        }
        private void showOptionMenu(int id)
        {
            IMenuItem item = menu.FindItem(id);
            item.SetVisible(true);
        }
        /* ------------------------------------------INVENTORY----------------------------------*/
        public void getItems(string command)
        {
            Response getItemCount = new Response();
            Response getItems = new Response();
            Item getItemCountA = new Item();
            Item getItemsA = new Item();

            getItemCount = getItemCountA.GetItemCount("my_store.db");
            if (getItemCount.status.Equals("SUCCESS"))
            {
                if (getItemCount.param1 == this.itemListDisplay.Count)
                    return;

                getItems = getItemsA.GetItems(command, "my_store.db", currentRetrieveItems);
                if (getItems.status.Equals("SUCCESS"))
                {
                    foreach (Item itemModel in getItems.itemList)
                    {
                        this.itemListDisplay.Add(new ListItem() { Id = itemModel.Id, IsChecked = false, Description = itemModel.NAME + "(" + itemModel.AvailableStock.ToString() + " PCS)(" + this.formatCurrency(itemModel.RetailPrice.ToString()) + ")" });
                    }

                    if (listItemAdapter == null)
                    {
                        listItemAdapter = new ListItemAdapter(this, this.itemListDisplay);
                        lvItems.Adapter = listItemAdapter;
                    }
                    //Update list of item in ListView Adapter
                    RunOnUiThread(() =>
                    {
                        listItemAdapter.NotifyDataSetChanged();
                        currentRetrieveItems = this.itemListDisplay.Count;
                    });
                }
                else
                {
                    this.showMessage(getItems.message);
                }
            }
            else
            {
                this.showMessage(getItemCount.message);
            }
        }
        public void hidePopupMenuView_Clicked(object sender, EventArgs e)
        {
            if (isAllowedToCloseDialog)
            {
                hideFabThread = new System.Threading.Thread(new ThreadStart(delegate
                {
                    RunOnUiThread(() =>
                    {
                        if (tvIndicator.Visibility == ViewStates.Visible)
                        {
                            tvIndicator.Animation = childFab_hide;
                            tvIndicator.StartAnimation(tvIndicator.Animation);
                        }
                        if ((int)tvDeleteSelected.Tag == 1)
                        {
                            tvDeleteSelected.Animation = childFab_hide;
                            tvDeleteSelected.StartAnimation(tvDeleteSelected.Animation);
                            deleteSelectedFab.Animation = childFab_hide;
                            deleteSelectedFab.StartAnimation(deleteSelectedFab.Animation);
                        }
                        if ((int)tvEditSelected.Tag == 1)
                        {
                            tvEditSelected.Animation = childFab_hide;
                            tvEditSelected.StartAnimation(tvEditSelected.Animation);
                            editSelectedFab.Animation = childFab_hide;
                            editSelectedFab.StartAnimation(editSelectedFab.Animation);
                        }
                        if ((int)tvCheckAll.Tag == 1)
                        {
                            tvCheckAll.Animation = childFab_hide;
                            tvCheckAll.StartAnimation(tvCheckAll.Animation);
                            checkAllFab.Animation = childFab_hide;
                            checkAllFab.StartAnimation(checkAllFab.Animation);
                        }
                        if ((int)tvUncheckAll.Tag == 1)
                        {
                            tvUncheckAll.Animation = childFab_hide;
                            tvUncheckAll.StartAnimation(tvUncheckAll.Animation);
                            uncheckAllFab.Animation = childFab_hide;
                            uncheckAllFab.StartAnimation(uncheckAllFab.Animation);
                        }
                        if ((int)tvShowDetail.Tag == 1)
                        {
                            tvShowDetail.Animation = childFab_hide;
                            tvShowDetail.StartAnimation(tvShowDetail.Animation);
                            showDetailFab.Animation = childFab_hide;
                            showDetailFab.StartAnimation(showDetailFab.Animation);
                        }
                        if ((int)tvShowMore.Tag == 1)
                        {
                            tvShowMore.Animation = childFab_hide;
                            tvShowMore.StartAnimation(tvShowMore.Animation);
                            mainFab.Animation = mainFab_hide;
                            mainFab.StartAnimation(mainFab.Animation);
                        }
                        tvDeleteSelected.Tag = 0;
                        tvEditSelected.Tag = 0;
                        tvCheckAll.Tag = 0;
                        tvUncheckAll.Tag = 0;
                        tvShowDetail.Tag = 0;
                        tvShowMore.Tag = 0;
                    });

                    System.Threading.Thread.Sleep(500);
                    RunOnUiThread(() =>
                    {
                        popupMenuDialog.Dismiss(); ;
                    });
                }));
                hideFabThread.Start();
            }
        }
        public void uncheckAllFab_Clicked(object sender, EventArgs e)
        {
            int count = 0;
            if (this.itemListDisplay.Count > 0)
            {
                RunOnUiThread(() =>
                {
                    tvIndicator.Visibility = ViewStates.Visible;
                    isAllowedToCloseDialog = false;

                    for (int i = 0; i < this.itemListDisplay.Count; i++)
                    {
                        if (this.itemListDisplay[i].IsChecked)
                        {
                            tvIndicator.Text = string.Format("Unchecking record {0} of {1}", i + 1, this.itemListDisplay.Count);
                            this.itemListDisplay[i].IsChecked = false;
                            count = count + 1;
                        }
                        if (i == this.itemListDisplay.Count - 1)
                        {
                            if (count != 0)
                            {
                                tvIndicator.Text = count.ToString() + " record/s have been unchecked.";
                            }
                            else
                            {
                                tvIndicator.Text = "All record/s were already unchecked.";
                            }
                            isAllowedToCloseDialog = true;
                            listItemAdapter.NotifyDataSetChanged();
                        }
                    }
                });
            }
            else
            {
                tvIndicator.Text = "Item list were all deleted.";
            }
        }
        public void checkAllFab_Clicked(object sender, EventArgs e)
        {
            int count = 0;
            if (this.itemListDisplay.Count > 0)
            {
                RunOnUiThread(() =>
                {
                    tvIndicator.Visibility = ViewStates.Visible;
                    isAllowedToCloseDialog = false;
                    for (int i = 0; i < this.itemListDisplay.Count; i++)
                    {
                        if (!this.itemListDisplay[i].IsChecked)
                        {
                            tvIndicator.Text = string.Format("Checking record {0} of {1}", i + 1, this.itemListDisplay.Count);
                            this.itemListDisplay[i].IsChecked = true;
                            count = count + 1;
                        }
                        if (i == this.itemListDisplay.Count - 1)
                        {
                            if (count != 0)
                            {
                                tvIndicator.Text = count.ToString() + " record/s have been checked.";
                            }
                            else
                            {
                                tvIndicator.Text = "All record/s were already checked.";
                            }
                            isAllowedToCloseDialog = true;
                            listItemAdapter.NotifyDataSetChanged();
                        }
                    }
                });
            }
            else
            {
                tvIndicator.Text = "Item list were all deleted.";
            }
        }
        public void deleteSelectedFab_Clicked(object sender, EventArgs e)
        {
            int count = 0;
            string firstItem = "";
            string command = "";

            List<ListItem> itemListDisplayCopy = new List<ListItem>();
            List<ListItem> deletedItems = new List<ListItem>();
            tvIndicator.Text = "Preparing for delete...";
            if (listItemAdapter.getSelectedItems().Count > 0)
            {
                var builder = new AlertDialog.Builder(this);
                builder.SetTitle("Application Message");
                builder.SetMessage("Are you sure you want to delete selected item/s?");
                builder.SetPositiveButton("Yes", delegate
                {
                    tvIndicator.Visibility = ViewStates.Visible;
                    isAllowedToCloseDialog = false;
                    foreach (ListItem listItem in listItemAdapter.getSelectedItems())
                    {
                        tvIndicator.Text = string.Format("Preparing in deleting {0}...", listItem.Description.Split('(')[0]);
                        command += string.Format("DELETE FROM Item WHERE Id = {0};", listItem.Id);
                        firstItem = listItem.Description.Split('(')[0];
                        deletedItems.Add(listItem);
                        this.itemListDisplay.Remove(listItem);
                        count = count + 1;
                    }

                    tvIndicator.Text = string.Format("Deleting item/s...");
                    sqliteADO = new SQLiteADO();
                    if (sqliteADO.ExecuteNonQuery(string.Format("BEGIN TRANSACTION; {0} COMMIT;", command), dbPath).status.Equals("SUCCESS"))
                    {
                        if (count == 1)
                        {
                            tvIndicator.Text = firstItem + " have been deleted.";
                        }
                        else
                        {
                            tvIndicator.Text = count.ToString() + " item/s have been deleted.";
                        }
                        RunOnUiThread(() =>
                        {
                            listItemAdapter.NotifyDataSetChanged();
                        });
                    }
                    else
                    {
                        this.itemListDisplay.AddRange(deletedItems);
                        this.tvIndicator.Text = "Item(Delete): Error has occured.";
                    }
                    deletedItems.Clear();
                    isAllowedToCloseDialog = true;
                });
                builder.SetNegativeButton("No", delegate { tvIndicator.Text = "Tap blank space to go back."; });
                builder.Create().Show();
            }
            else
            {
                tvIndicator.Text = "No item selected.";
            }
        }
        public void editSelectedFab_Clicked(object sender, EventArgs e)
        {
            Item itemModel = new Item();
            Response response = new Response();
            countDialogShown = 0;
            isAllowedToCloseDialog = false;
            response = itemModel.GetItem(listItemAdapter.getSelectedItems()[0].Id, "my_store.db");
            if (response.status.Equals("SUCCESS"))
            {
                evaluationType = "Update";
                btnEvaluateItem.Text = "Update";
                this.hideLayouts();
                this.btnEvaluateItem.Click += btnEvaluateItem_Clicked;
                itemHolder = response.itemList[0];
                etPurchasedPrice.KeyPress += etPurchasedPrice_ItemChanged;
                etStocks.KeyPress += etPurchasedPrice_ItemChanged;
                oldPrice = response.itemList[0].PurchasedPrice;
                oldStock = response.itemList[0].AvailableStock;
                this.actvItemName.Text = response.itemList[0].NAME.ToString();
                this.etBarcode.Text = response.itemList[0].Barcode.ToString().Trim().Equals("") ? " " : response.itemList[0].Barcode.ToString();
                this.etPurchasedPrice.Text = this.formatCurrency(response.itemList[0].PurchasedPrice.ToString());
                this.etAvarageCost.Text = this.formatCurrency(response.itemList[0].AverageCost.ToString());
                this.etRetailPrice.Text = this.formatCurrency(response.itemList[0].RetailPrice.ToString());
                this.etStocks.Text = response.itemList[0].AvailableStock.ToString();
                isAllowedToCloseDialog = true;
                this.hidePopupMenuView_Clicked(sender, e);
                glInventory.Visibility = ViewStates.Visible;
                this.showOptionMenu(Resource.Id.action_more);
            }
            else
            {
                tvIndicator.Text = "Item(Selected): An error has occured.";
                isAllowedToCloseDialog = true;
            }

        }
        public void showDetailFab_Clicked(object sender, EventArgs e)
        {
            Item itemModel = new Item();
            Response response = new Response();
            var view = LayoutInflater.Inflate(Resource.Layout.ItemDetails, null);
            AlertDialog.Builder dialogBuilder = new AlertDialog.Builder(this);
            dialogBuilder.SetCancelable(false);

            isAllowedToCloseDialog = false;
            dialogBuilder.SetView(view);
            dialogBuilder.SetPositiveButton("Close", delegate { isAllowedToCloseDialog = true; });

            response = itemModel.GetItem(listItemAdapter.getSelectedItems()[0].Id, "my_store.db");
            if (response.status.Equals("SUCCESS"))
            {
                view.FindViewById<EditText>(Resource.Id.etDescription).Text = response.itemList[0].NAME.ToString();
                view.FindViewById<EditText>(Resource.Id.etBarcode).Text = response.itemList[0].Barcode.ToString().Trim().Equals("") ? " " : response.itemList[0].Barcode.ToString();
                view.FindViewById<EditText>(Resource.Id.etPurchasedPrice).Text = this.formatCurrency(response.itemList[0].PurchasedPrice.ToString());
                view.FindViewById<EditText>(Resource.Id.etAverageCost).Text = this.formatCurrency(response.itemList[0].AverageCost.ToString());
                view.FindViewById<EditText>(Resource.Id.etRetailPrice).Text = this.formatCurrency(response.itemList[0].RetailPrice.ToString());
                view.FindViewById<EditText>(Resource.Id.etAvailableStocks).Text = response.itemList[0].AvailableStock.ToString();
                dialogBuilder.Create().Show();
            }
            else
            {
                tvIndicator.Text = "Item(Selected): An error has occured.";
                isAllowedToCloseDialog = true;
            }
        }
        public void mainFab_Clicked(object sender, EventArgs e)
        {
            if (this.filterValue.Trim().Equals(""))
            {
                this.getItems("SELECT * FROM Item");
                this.hidePopupMenuView_Clicked(sender, e);
            }
            else
            {
                this.getItems(string.Format("SELECT * FROM Item WHERE NAME LIKE '{0}%' OR BARCODE LIKE '{1}%'", this.filterValue, this.filterValue));
                this.hidePopupMenuView_Clicked(sender, e);
            }
        }
        public void fabItem_Clicked(object sender, EventArgs e)
        {
            if (this.itemListDisplay.Count > 0)
            {
                var popupMenuView = LayoutInflater.Inflate(Resource.Layout.PopupMenu, null);
                popupMenuDialog = new Dialog(this, Android.Resource.Style.AnimationTranslucent);
                popupMenuDialog.SetContentView(popupMenuView);
                popupMenuDialog.Show();
                popupMenuDialog.SetCancelable(false);

                if (hideFabThread != null)
                    hideFabThread.Abort();

                tvIndicator = popupMenuView.FindViewById<TextView>(Resource.Id.tvIndicator);
                tvIndicator.Visibility = ViewStates.Visible;
                tvIndicator.Text = "Tap blank space to go back.";
                tvIndicator.Click += hidePopupMenuView_Clicked;

                //Animations
                mainFab_show = AnimationUtils.LoadAnimation(this, Resource.Layout.mainFab_show);
                mainFab_hide = AnimationUtils.LoadAnimation(this, Resource.Layout.mainFab_hide);
                childFab_show = AnimationUtils.LoadAnimation(this, Resource.Layout.childFab_show);
                childFab_hide = AnimationUtils.LoadAnimation(this, Resource.Layout.childFab_hide);

                mainFab = popupMenuView.FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.mainFab);
                mainLayoutParams = (FrameLayout.LayoutParams)mainFab.LayoutParameters;
                mainFab.LayoutParameters = mainLayoutParams;
                mainFab.Animation = mainFab_show;
                mainFab.Clickable = true;

                tvShowMore = popupMenuView.FindViewById<TextView>(Resource.Id.tvShowMore);
                tvShowMoreLayoutParams = (FrameLayout.LayoutParams)tvShowMore.LayoutParameters;

                showDetailFab = popupMenuView.FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.showDetailFab);
                showDetailFabLayoutParams = (FrameLayout.LayoutParams)showDetailFab.LayoutParameters;

                tvShowDetail = popupMenuView.FindViewById<TextView>(Resource.Id.tvShowDetail);
                tvShowDetailLayoutParams = (FrameLayout.LayoutParams)tvShowDetail.LayoutParameters;

                editSelectedFab = popupMenuView.FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.editSelectedFab);
                editSelectedFabLayoutParams = (FrameLayout.LayoutParams)editSelectedFab.LayoutParameters;

                tvEditSelected = popupMenuView.FindViewById<TextView>(Resource.Id.tvEditSelected);
                tvEditSelectedLayoutParams = (FrameLayout.LayoutParams)tvEditSelected.LayoutParameters;

                deleteSelectedFab = popupMenuView.FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.deleteSelectedFab);
                deleteSelectedFabLayoutParams = (FrameLayout.LayoutParams)deleteSelectedFab.LayoutParameters;

                tvDeleteSelected = popupMenuView.FindViewById<TextView>(Resource.Id.tvDeleteSelected);
                tvDeleteSelectedLayoutParams = (FrameLayout.LayoutParams)tvDeleteSelected.LayoutParameters;

                uncheckAllFab = popupMenuView.FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.unCheckAllFab);
                uncheckAllFabLayoutParams = (FrameLayout.LayoutParams)uncheckAllFab.LayoutParameters;

                tvUncheckAll = popupMenuView.FindViewById<TextView>(Resource.Id.tvUncheckAll);
                tvUncheckAllLayoutParams = (FrameLayout.LayoutParams)tvUncheckAll.LayoutParameters;

                checkAllFab = popupMenuView.FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.checkAllFab);
                checkAllFabLayoutParams = (FrameLayout.LayoutParams)checkAllFab.LayoutParameters;

                tvCheckAll = popupMenuView.FindViewById<TextView>(Resource.Id.tvCheckAll);
                tvCheckAllLayoutParams = (FrameLayout.LayoutParams)tvCheckAll.LayoutParameters;

                tvShowMoreLayoutParams.RightMargin = tvShowMoreLayoutParams.RightMargin + (int)(tvShowMoreLayoutParams.RightMargin * 4);
                tvShowMoreLayoutParams.BottomMargin = tvShowMoreLayoutParams.BottomMargin * 2;
                tvShowMore.Animation = childFab_show;
                tvShowMore.Clickable = true;

                if (this.listItemAdapter.getSelectedItems().Count == 0 || this.listItemAdapter.getSelectedItems().Count != 1)
                {
                    deleteSelectedFabLayoutParams.RightMargin = deleteSelectedFabLayoutParams.RightMargin + (int)(deleteSelectedFabLayoutParams.RightMargin * .60);
                    deleteSelectedFabLayoutParams.BottomMargin = deleteSelectedFabLayoutParams.BottomMargin * 5;
                    deleteSelectedFab.LayoutParameters = deleteSelectedFabLayoutParams;
                    deleteSelectedFab.Size = FabSize.Mini;
                    deleteSelectedFab.Animation = childFab_show;
                    deleteSelectedFab.Clickable = true;

                    tvDeleteSelectedLayoutParams.RightMargin = tvDeleteSelectedLayoutParams.RightMargin + (int)(tvDeleteSelectedLayoutParams.RightMargin * 4);
                    tvDeleteSelectedLayoutParams.BottomMargin = tvDeleteSelectedLayoutParams.BottomMargin * 5 + (int)(tvDeleteSelectedLayoutParams.BottomMargin * .4);
                    tvDeleteSelected.LayoutParameters = tvDeleteSelectedLayoutParams;
                    tvDeleteSelected.Animation = childFab_show;
                    tvDeleteSelected.Clickable = true;

                    uncheckAllFabLayoutParams.RightMargin = uncheckAllFabLayoutParams.RightMargin + (int)(uncheckAllFabLayoutParams.RightMargin * .60);
                    uncheckAllFabLayoutParams.BottomMargin = uncheckAllFabLayoutParams.BottomMargin * 8;
                    uncheckAllFab.LayoutParameters = uncheckAllFabLayoutParams;
                    uncheckAllFab.Size = FabSize.Mini;
                    uncheckAllFab.Animation = childFab_show;
                    uncheckAllFab.Clickable = true;

                    tvUncheckAllLayoutParams.RightMargin = tvUncheckAllLayoutParams.RightMargin + (int)(tvUncheckAllLayoutParams.RightMargin * 4);
                    tvUncheckAllLayoutParams.BottomMargin = tvUncheckAllLayoutParams.BottomMargin * 8 + (int)(tvUncheckAllLayoutParams.BottomMargin * .4);
                    tvUncheckAll.LayoutParameters = tvUncheckAllLayoutParams;
                    tvUncheckAll.Animation = childFab_show;
                    tvUncheckAll.Clickable = true;

                    checkAllFabLayoutParams.RightMargin = checkAllFabLayoutParams.RightMargin + (int)(checkAllFabLayoutParams.RightMargin * .60);
                    checkAllFabLayoutParams.BottomMargin = checkAllFabLayoutParams.BottomMargin * 11;
                    checkAllFab.LayoutParameters = checkAllFabLayoutParams;
                    checkAllFab.Size = FabSize.Mini;
                    checkAllFab.Animation = childFab_show;
                    checkAllFab.Clickable = true;

                    tvCheckAllLayoutParams.RightMargin = tvCheckAllLayoutParams.RightMargin + (int)(tvCheckAllLayoutParams.RightMargin * 4);
                    tvCheckAllLayoutParams.BottomMargin = tvCheckAllLayoutParams.BottomMargin * 11 + (int)(tvCheckAllLayoutParams.BottomMargin * .4);
                    tvCheckAll.LayoutParameters = tvCheckAllLayoutParams;
                    tvCheckAll.Animation = childFab_show;
                    tvCheckAll.Clickable = true;

                    tvDeleteSelected.Tag = 1;
                    tvCheckAll.Tag = 1;
                    tvUncheckAll.Tag = 1;
                    tvShowMore.Tag = 1;
                }
                else// if (this.listItemAdapter.getSelectedItems().Count == 1)
                {
                    showDetailFabLayoutParams.RightMargin = showDetailFabLayoutParams.RightMargin + (int)(showDetailFabLayoutParams.RightMargin * .60);
                    showDetailFabLayoutParams.BottomMargin = showDetailFabLayoutParams.BottomMargin * 5;
                    showDetailFab.LayoutParameters = showDetailFabLayoutParams;
                    showDetailFab.Size = FabSize.Mini;
                    showDetailFab.Animation = childFab_show;
                    showDetailFab.Clickable = true;

                    tvShowDetailLayoutParams.RightMargin = tvShowDetailLayoutParams.RightMargin + (int)(tvShowDetailLayoutParams.RightMargin * 4);
                    tvShowDetailLayoutParams.BottomMargin = tvShowDetailLayoutParams.BottomMargin * 5 + (int)(tvShowDetailLayoutParams.BottomMargin * .4);
                    tvShowDetail.LayoutParameters = tvShowDetailLayoutParams;
                    tvShowDetail.Animation = childFab_show;
                    tvShowDetail.Clickable = true;

                    editSelectedFabLayoutParams.RightMargin = editSelectedFabLayoutParams.RightMargin + (int)(editSelectedFabLayoutParams.RightMargin * .60);
                    editSelectedFabLayoutParams.BottomMargin = editSelectedFabLayoutParams.BottomMargin * 8;
                    editSelectedFab.LayoutParameters = editSelectedFabLayoutParams;
                    editSelectedFab.Size = FabSize.Mini;
                    editSelectedFab.Animation = childFab_show;
                    showDetailFab.Clickable = true;

                    tvEditSelectedLayoutParams.RightMargin = tvEditSelectedLayoutParams.RightMargin + (int)(tvEditSelectedLayoutParams.RightMargin * 4);
                    tvEditSelectedLayoutParams.BottomMargin = tvEditSelectedLayoutParams.BottomMargin * 8 + (int)(tvEditSelectedLayoutParams.BottomMargin * .4);
                    tvEditSelected.LayoutParameters = tvEditSelectedLayoutParams;
                    tvEditSelected.Animation = childFab_show;
                    tvEditSelected.Clickable = true;

                    deleteSelectedFabLayoutParams.RightMargin = deleteSelectedFabLayoutParams.RightMargin + (int)(deleteSelectedFabLayoutParams.RightMargin * .60);
                    deleteSelectedFabLayoutParams.BottomMargin = deleteSelectedFabLayoutParams.BottomMargin * 11;
                    deleteSelectedFab.LayoutParameters = deleteSelectedFabLayoutParams;
                    deleteSelectedFab.Size = FabSize.Mini;
                    deleteSelectedFab.Animation = childFab_show;
                    deleteSelectedFab.Clickable = true;

                    tvDeleteSelectedLayoutParams.RightMargin = tvDeleteSelectedLayoutParams.RightMargin + (int)(tvDeleteSelectedLayoutParams.RightMargin * 4);
                    tvDeleteSelectedLayoutParams.BottomMargin = tvDeleteSelectedLayoutParams.BottomMargin * 11 + (int)(tvDeleteSelectedLayoutParams.BottomMargin * .4);
                    tvDeleteSelected.LayoutParameters = tvDeleteSelectedLayoutParams;
                    tvDeleteSelected.Animation = childFab_show;
                    tvDeleteSelected.Clickable = true;

                    uncheckAllFabLayoutParams.RightMargin = uncheckAllFabLayoutParams.RightMargin + (int)(uncheckAllFabLayoutParams.RightMargin * .60);
                    uncheckAllFabLayoutParams.BottomMargin = uncheckAllFabLayoutParams.BottomMargin * 14;
                    uncheckAllFab.LayoutParameters = uncheckAllFabLayoutParams;
                    uncheckAllFab.Size = FabSize.Mini;
                    uncheckAllFab.Animation = childFab_show;
                    uncheckAllFab.Clickable = true;

                    tvUncheckAllLayoutParams.RightMargin = tvUncheckAllLayoutParams.RightMargin + (int)(tvUncheckAllLayoutParams.RightMargin * 4);
                    tvUncheckAllLayoutParams.BottomMargin = tvUncheckAllLayoutParams.BottomMargin * 14 + (int)(tvUncheckAllLayoutParams.BottomMargin * .4);
                    tvUncheckAll.LayoutParameters = tvUncheckAllLayoutParams;
                    tvUncheckAll.Animation = childFab_show;
                    tvUncheckAll.Clickable = true;

                    checkAllFabLayoutParams.RightMargin = checkAllFabLayoutParams.RightMargin + (int)(checkAllFabLayoutParams.RightMargin * .60);
                    checkAllFabLayoutParams.BottomMargin = checkAllFabLayoutParams.BottomMargin * 17;
                    checkAllFab.LayoutParameters = checkAllFabLayoutParams;
                    checkAllFab.Size = FabSize.Mini;
                    checkAllFab.Animation = childFab_show;
                    checkAllFab.Clickable = true;

                    tvCheckAllLayoutParams.RightMargin = tvCheckAllLayoutParams.RightMargin + (int)(tvCheckAllLayoutParams.RightMargin * 4);
                    tvCheckAllLayoutParams.BottomMargin = tvCheckAllLayoutParams.BottomMargin * 17 + (int)(tvCheckAllLayoutParams.BottomMargin * .4);
                    tvCheckAll.LayoutParameters = tvCheckAllLayoutParams;
                    tvCheckAll.Animation = childFab_show;
                    tvCheckAll.Clickable = true;

                    tvDeleteSelected.Tag = 1;
                    tvEditSelected.Tag = 1;
                    tvCheckAll.Tag = 1;
                    tvUncheckAll.Tag = 1;
                    tvShowDetail.Tag = 1;
                    tvShowMore.Tag = 1;
                }

                popupMenuView.Click += this.hidePopupMenuView_Clicked;
                this.uncheckAllFab.Click += this.uncheckAllFab_Clicked;
                this.checkAllFab.Click += this.checkAllFab_Clicked;
                this.deleteSelectedFab.Click += this.deleteSelectedFab_Clicked;
                this.editSelectedFab.Click += this.editSelectedFab_Clicked;
                this.showDetailFab.Click += this.showDetailFab_Clicked;
                this.mainFab.Click += this.mainFab_Clicked;
            }
        }
        private double? getAverageCost(int? oldStock, int newStock, double? oldOrice, double newPrice)
        {
            if (oldStock < Convert.ToInt16(etStocks.Text))
            {
                return ((oldPrice * oldStock) + (newStock - oldStock) * newPrice) / newStock;
            }
            else
            {
                return ((oldPrice * oldStock) + (oldStock - newStock) * newPrice) / newStock;
            }
        }
        public void etPurchasedPrice_ItemChanged(object sender, View.KeyEventArgs e)
        {
            e.Handled = false;
            if (evaluationType.Equals("Save"))
            {
                if (!etPurchasedPrice.Text.Equals("") && (!etStocks.Text.Equals("")))
                    etAvarageCost.Text = etPurchasedPrice.Text;
            }
            else
            {
                if (!etPurchasedPrice.Text.Equals("") && (!etStocks.Text.Equals("")))
                {
                    if (Convert.ToDouble(etPurchasedPrice.Text) != oldPrice)
                        this.getAverageCost(this.oldStock, Convert.ToInt16(etStocks.Text), this.oldPrice, Convert.ToDouble(etPurchasedPrice.Text));
                }
            }
        }
        public string checkIfValid(string name, string purchasedPrice, string retailPrice, string availableStocks)
        {
            string message = "";
            if (name.Trim().Equals("") || name == null)
                message = "Item description is required.";
            else if (purchasedPrice.Trim().Equals("") || purchasedPrice == null)
                message = "Cost is required.";
            else if (retailPrice.Trim().Equals("") || retailPrice == null)
                message = "Retail Price is required.";
            else if (availableStocks.Trim().Equals("") || availableStocks == null)
                message = "Available Stock(s) is required.";

            return message;
        }
        public void btnClearItem_Clicked(object sender, EventArgs e)
        {
            this.resetItem();
        }
        public void btnEvaluateItem_Clicked(object sender, EventArgs e)
        {
            string validate = checkIfValid(actvItemName.Text, etPurchasedPrice.Text, etRetailPrice.Text, etStocks.Text);
            if (validate.Equals(""))
            {
                if (evaluationType.Equals("Save"))
                {
                    Response saveItem = new Response();
                    Response getId = new Response();
                    Item itemModel = new Item();
                    sqliteADO = new SQLiteADO();
                    //Check if Item Name and Barcode doesn't exist
                    if (itemModel.GetByName(actvItemName.Text, "my_store.db").param1 == 1)
                    {
                        this.showSnackBar("Item already exist.");
                    }
                    else if ((this.etBarcode.Text.Trim().Equals("") ? 0 : itemModel.GetByBarcode(this.etBarcode.Text.Trim(), "my_store.db").param1) == 1)
                    {
                        this.showSnackBar("Barcode already exist.");
                    }
                    else
                    {
                        //Save item
                        command = string.Format("INSERT INTO Item(Barcode, NAME, PurchasedPrice, RetailPrice, AvailableStock, AverageCost) VALUES('{0}', '{1}', {2}, {3}, {4}, {5});", etBarcode.Text, actvItemName.Text.Replace("'", "/"), Convert.ToDouble(etPurchasedPrice.Text), Convert.ToDouble(etRetailPrice.Text), Convert.ToInt16(etStocks.Text), Convert.ToDouble(etAvarageCost.Text)) + "\n";
                        saveItem = sqliteADO.ExecuteNonQuery("BEGIN TRANSACTION;" + "\n" + command + "COMMIT;", dbPath);
                        if (saveItem.status.Equals("SUCCESS"))
                        {
                            this.showSnackBarInfinite("Successfully Saved.");
                            getId = itemModel.GetLastId("my_store.db");
                            if (getId.status.Equals("SUCCESS"))
                            {
                                this.resetItem();
                                this.itemListDisplay.Add(new ListItem() { Id = getId.param1, IsChecked = false, Description = actvItemName.Text + "(" + this.etStocks.Text.ToString() + " PCS)(" + this.formatCurrency(this.etRetailPrice.Text.ToString()) + ")" });
                                //Update list of item in ListView Adapter
                                RunOnUiThread(() =>
                                {
                                    listItemAdapter.NotifyDataSetChanged();
                                    currentRetrieveItems = this.itemListDisplay.Count;
                                });
                            }
                        }
                        else
                        {
                            this.showMessage("Item(Insert): " + saveItem.message);
                        }
                    }
                }
                else //Update
                {
                    countDialogShown = countDialogShown + 1;
                    var messageDialog = new AlertDialog.Builder(this);
                    messageDialog.SetMessage("Application Message");
                    messageDialog.SetMessage("Are you sure you want to update Item " + actvItemName.Text + "?");
                    messageDialog.SetNegativeButton("No", delegate
                    {
                        this.resetItem();
                        this.hideLayouts();
                        evaluationType = "ShowItemList";
                        flInvetoryList.Visibility = ViewStates.Visible;
                        this.showOptionMenu(Resource.Id.action_more);
                        this.showOptionMenu(Resource.Id.action_refresh);
                    });
                    messageDialog.SetPositiveButton("Yes", delegate
                    {
                        Response saveItem = new Response();
                        Item itemModel = new Item();
                        sqliteADO = new SQLiteADO();
                        bool valid = true;
                        int position = -1;

                        if (!this.itemHolder.NAME.Equals(actvItemName.Text))
                        {
                            //Check if Item Name doesn't exist
                            if (itemModel.GetByName(actvItemName.Text, "my_store.db").param1 == 1)
                            {
                                this.showSnackBar("Item already exist.");
                                valid = false;
                                countDialogShown = 0;
                            }
                        }
                        else if (!this.itemHolder.Barcode.Equals(this.etBarcode.Text))
                        {
                            if ((this.etBarcode.Text.Trim().Equals("") ? 0 : itemModel.GetByBarcode(this.etBarcode.Text.Trim(), "my_store.db").param1) == 1)
                            {
                                this.showSnackBar("Barcode already exist.");
                                valid = false;
                                countDialogShown = 0;
                            }
                        }
                        if (valid)
                        {
                            //Update item
                            if (scannedState)
                            {
                                for(int i = 0; i < this.itemListDisplay.Count; i++)
                                {
                                    if(this.itemListDisplay[i].Id == this.itemHolder.Id)
                                    {
                                        position = i;
                                        command = string.Format("UPDATE Item SET Barcode = '{0}', NAME = '{1}', PurchasedPrice = {2} , RetailPrice = {3}, AvailableStock = {4}, AverageCost = {5} WHERE Id = {6};", etBarcode.Text, actvItemName.Text.Replace("'", "/"), Convert.ToDouble(etPurchasedPrice.Text), Convert.ToDouble(etRetailPrice.Text), Convert.ToInt16(etStocks.Text), Convert.ToDouble(etAvarageCost.Text), this.itemListDisplay[i].Id) + "\n";
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                command = string.Format("UPDATE Item SET Barcode = '{0}', NAME = '{1}', PurchasedPrice = {2} , RetailPrice = {3}, AvailableStock = {4}, AverageCost = {5} WHERE Id = {6};", etBarcode.Text, actvItemName.Text.Replace("'", "/"), Convert.ToDouble(etPurchasedPrice.Text), Convert.ToDouble(etRetailPrice.Text), Convert.ToInt16(etStocks.Text), Convert.ToDouble(etAvarageCost.Text), this.listItemAdapter.getSelectedItems()[0].Id) + "\n";
                            }
                            
                            saveItem = sqliteADO.ExecuteNonQuery("BEGIN TRANSACTION;" + "\n" + command + "COMMIT;", dbPath);
                            if (saveItem.status.Equals("SUCCESS"))
                            {
                                if (scannedState)
                                {
                                    if (position != -1)
                                    {
                                        this.itemListDisplay[position].Description = actvItemName.Text + "(" + this.etStocks.Text.ToString() + " PCS)(" + this.formatCurrency(this.etRetailPrice.Text.ToString()) + ")";
                                    }
                                }
                                else
                                {
                                    this.itemListDisplay[listItemAdapter.indexOfSelecteItems()[0]].Description = actvItemName.Text + "(" + this.etStocks.Text.ToString() + " PCS)(" + this.formatCurrency(this.etRetailPrice.Text.ToString()) + ")";
                                }

                                this.showSnackBar("Successfully Updated.");
                                currentRetrieveItems = this.itemListDisplay.Count;
                                this.resetItem();
                                this.hideLayouts();
                                evaluationType = "ShowItemList";
                                flInvetoryList.Visibility = ViewStates.Visible;
                                this.showOptionMenu(Resource.Id.action_more);
                                this.showOptionMenu(Resource.Id.action_refresh);
                                //Update list of item in ListView Adapter
                                RunOnUiThread(() =>
                                {
                                    listItemAdapter.NotifyDataSetChanged();
                                });
                            }
                            else
                            {
                                this.showMessage("Item(Update): " + saveItem.message);
                            }
                        }
                    });
                    if (countDialogShown == 1)//To prevent showing the dialog more than once
                        messageDialog.Show();
                }
            }
            else
                this.showSnackBar(validate);
        }
        public async void btnScanItem_Clicked()
        {
            Item itemModel = new Item();
            Response response = new Response();
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
                response = itemModel.GetByBarcode(result.ToString(), "my_store.db");
                if (response.param1 == 0)
                {
                    this.hideLayouts();
                    evaluationType = "Save";
                    btnEvaluateItem.Text = "Save";
                    etBarcode.Text = result.ToString();
                    glInventory.Visibility = ViewStates.Visible;
                    this.showOptionMenu(Resource.Id.action_more);
                    etPurchasedPrice.KeyPress += etPurchasedPrice_ItemChanged;
                    etStocks.KeyPress += etPurchasedPrice_ItemChanged;
                }
                else
                {
                    countDialogShown = 0;
                    evaluationType = "Update";
                    btnEvaluateItem.Text = "Update";
                    this.hideLayouts();
                    this.btnEvaluateItem.Click += btnEvaluateItem_Clicked;
                    itemHolder = response.itemList[0];
                    etPurchasedPrice.KeyPress += etPurchasedPrice_ItemChanged;
                    etStocks.KeyPress += etPurchasedPrice_ItemChanged;
                    oldPrice = response.itemList[0].PurchasedPrice;
                    oldStock = response.itemList[0].AvailableStock;
                    this.actvItemName.Text = response.itemList[0].NAME.ToString();
                    this.etBarcode.Text = response.itemList[0].Barcode.ToString().Trim().Equals("") ? " " : response.itemList[0].Barcode.ToString();
                    this.etPurchasedPrice.Text = this.formatCurrency(response.itemList[0].PurchasedPrice.ToString());
                    this.etAvarageCost.Text = this.formatCurrency(response.itemList[0].AverageCost.ToString());
                    this.etRetailPrice.Text = this.formatCurrency(response.itemList[0].RetailPrice.ToString());
                    this.etStocks.Text = response.itemList[0].AvailableStock.ToString();
                    isAllowedToCloseDialog = true;
                    glInventory.Visibility = ViewStates.Visible;
                    this.showOptionMenu(Resource.Id.action_more);
                }
                mDrawerLayout.CloseDrawer(mRightDrawer);
                scannedState = true;
            }
            else
            {

                evaluationType = "ShowItemList";
                flInvetoryList.Visibility = ViewStates.Visible;
                this.showOptionMenu(Resource.Id.action_more);
                this.showOptionMenu(Resource.Id.action_refresh);
            }
        }
        /* ------------------------------------------END OF INVENTORY----------------------------------*/
        protected override void OnSaveInstanceState(Bundle outState)
        {
            if (mDrawerLayout.IsDrawerOpen((int)GravityFlags.Left))
            {
                outState.PutString("DrawerState", "OpenState");
            }
            else
            {
                outState.PutString("DrawerState", "CloseState");
            }
        }
        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            mDrawerToggle.SyncState();
        }
        public override void OnBackPressed()
        {
            if (mDrawerLayout.IsDrawerOpen(mLeftDrawer))
            {
                mDrawerLayout.CloseDrawer(mLeftDrawer);
            }
            else if (mDrawerLayout.IsDrawerOpen(mRightDrawer))
            {
                mDrawerLayout.CloseDrawer(mRightDrawer);
            }
            else
            {
                //Display AlertBox
                var builder = new AlertDialog.Builder(this);

                builder.SetTitle("Application Message")
                       .SetMessage("Are you sure you want to exit?")
                       .SetPositiveButton("Yes", delegate { this.FinishAffinity(); })
                       .SetNegativeButton("No", delegate { });
                builder.Create().Show();
            }
        }
        private void resetItem()
        {
            actvItemName.Text = "";
            etBarcode.Text = "";
            etPurchasedPrice.Text = "";
            etAvarageCost.Text = "0.00";
            etRetailPrice.Text = "";
            etStocks.Text = "";
            oldPrice = 0.00;
            oldStock = 0;
        }
    }
}

