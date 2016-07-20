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
    public class MainActivity : ActionBarActivity, IScrollDirectorListener, AbsListView.IOnScrollListener
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
        private double oldPrice = 0.0;
        private int oldStock = 0;
        private int currentRetrieveItems = 0;
        private List<ListItem> itemListDisplay = new List<ListItem>();
        private FrameLayout flInvetoryList;
        private com.refractored.fab.FloatingActionButton fabItem;
        private AlertDialog.Builder popupBuilder;
        private AlertDialog popupDialog;
        private ListItemAdapter listItemAdapter;
        private string filterValue = "";
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
            glAbout.Visibility = ViewStates.Gone;
            glInventory.Visibility = ViewStates.Gone;
            flInvetoryList.Visibility = ViewStates.Gone;
            this.hideOptionMenu(Resource.Id.action_more);
            this.hideOptionMenu(Resource.Id.action_refresh);

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
            glAbout.Visibility = ViewStates.Gone;
            glInventory.Visibility = ViewStates.Gone;
            flInvetoryList.Visibility = ViewStates.Gone;

            etBarcode.Enabled = true;
            etPurchasedPrice.Enabled = true;
            etRetailPrice.Enabled = true;
            etAvarageCost.Enabled = false;
            etStocks.Enabled = true;
            switch (e.Position)
            {
                case 0://Scan
                    this.hideOptionMenu(Resource.Id.action_refresh);
                    if (requestType.Equals("Show Items"))
                    {
                        this.btnScanItem_Clicked();
                        evaluationType = "Scan";
                    }
                    break;
                case 1: //Add
                    this.hideOptionMenu(Resource.Id.action_refresh);
                    if (requestType.Equals("Show Items"))
                    {
                        evaluationType = "Save";
                        glInventory.Visibility = ViewStates.Visible;

                        etPurchasedPrice.KeyPress += etPurchasedPrice_ItemChanged;
                        etStocks.KeyPress += etPurchasedPrice_ItemChanged;
                    }
                    break;
                //case 2: //Edit
                //    if (requestType.Equals("Show Items"))
                //    {
                //        evaluationType = "Update";
                //        glInventory.Visibility = ViewStates.Visible;
                //    }
                //    break;
                //case 3://Delete
                //    if (requestType.Equals("Show Items"))
                //    {
                //        evaluationType = "Delete";
                //        glInventory.Visibility = ViewStates.Visible;
                //        etPurchasedPrice.Enabled = false;
                //        etRetailPrice.Enabled = false;
                //        etAvarageCost.Enabled = false;
                //        etBarcode.Enabled = false;
                //        etStocks.Enabled = false;
                //    }
                //    break;
                default:
                    if (requestType.Equals("Show Items"))
                    {
                        evaluationType = "ShowItemList";
                        flInvetoryList.Visibility = ViewStates.Visible;
                        this.showOptionMenu(Resource.Id.action_refresh);
                    }
                    break;
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
            tableItemOptions = new List<TableItem>();

            tableItemOptions.Add(new TableItem() { ImageResourceId = Resource.Drawable.Scan, ItemLabel = "Scan Item" });
            tableItemOptions.Add(new TableItem() { ImageResourceId = Resource.Drawable.Add, ItemLabel = "Add Item" });
            //tableItemOptions.Add(new TableItem() { ImageResourceId = Resource.Drawable.Edit, ItemLabel = "Edit Item" });
            //tableItemOptions.Add(new TableItem() { ImageResourceId = Resource.Drawable.Delete, ItemLabel = "Delete Item" });
            tableItemOptions.Add(new TableItem() { ImageResourceId = Resource.Drawable.List, ItemLabel = "Show Items" });
        }
        //function that will listen if the left toggle menu is selected
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
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
                    if (requestType.Equals("Show Items"))
                    {
                        this.itemListDisplay.Clear();
                        this.currentRetrieveItems = 0;
                        this.updateItemListAdapter();

                        if (this.filterValue.Equals(""))
                        {
                            this.getItems("SELECT * FROM Item");
                        }
                        else
                        {
                            this.getItems(string.Format("SELECT * FROM Item WHERE NAME LIKE '{0}%' OR BARCODE LIKE '{1}%'", this.filterValue, this.filterValue));
                        }
                    }
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
                    this.updateItemListAdapter();
                    currentRetrieveItems = this.itemListDisplay.Count;
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
        public void fabItem_Clicked(object sender, EventArgs e)
        {
            if (this.itemListDisplay.Count > 0)
            {
                var popupMenuView = LayoutInflater.Inflate(Resource.Layout.PopupMenu, null);
                Dialog popupMenuDialog = new Dialog(this, Android.Resource.Style.AnimationTranslucent);
                popupMenuDialog.SetContentView(popupMenuView);
                popupMenuDialog.Show();
                popupMenuDialog.SetCancelable(false);

                //Animations
                Animation mainFab_show = AnimationUtils.LoadAnimation(this, Resource.Layout.mainFab_show);
                Animation mainFab_hide = AnimationUtils.LoadAnimation(this, Resource.Layout.mainFab_hide);
                Animation childFab_show = AnimationUtils.LoadAnimation(this, Resource.Layout.childFab_show);
                Animation childFab_hide = AnimationUtils.LoadAnimation(this, Resource.Layout.childFab_hide);

                com.refractored.fab.FloatingActionButton mainFab = popupMenuView.FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.mainFab);
                FrameLayout.LayoutParams mainLayoutParams = (FrameLayout.LayoutParams)mainFab.LayoutParameters;
                mainFab.LayoutParameters = mainLayoutParams;
                mainFab.Animation = mainFab_show;
                mainFab.Clickable = true;

                com.refractored.fab.FloatingActionButton deleteSelectedFab = popupMenuView.FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.deleteSelectedFab);
                FrameLayout.LayoutParams deleteSelectedFabLayoutParams = (FrameLayout.LayoutParams)deleteSelectedFab.LayoutParameters;
                deleteSelectedFabLayoutParams.RightMargin = deleteSelectedFabLayoutParams.RightMargin + (int)(deleteSelectedFabLayoutParams.RightMargin * .60);
                deleteSelectedFabLayoutParams.BottomMargin = deleteSelectedFabLayoutParams.BottomMargin * 5;
                deleteSelectedFab.LayoutParameters = deleteSelectedFabLayoutParams;
                deleteSelectedFab.Size = FabSize.Mini;
                deleteSelectedFab.Animation = childFab_show;
                deleteSelectedFab.Clickable = true;

                TextView tvDeleteSelected = popupMenuView.FindViewById<TextView>(Resource.Id.tvDeleteSelected);
                FrameLayout.LayoutParams tvDeleteSelectedLayoutParams = (FrameLayout.LayoutParams)tvDeleteSelected.LayoutParameters;
                tvDeleteSelectedLayoutParams.RightMargin = tvDeleteSelectedLayoutParams.RightMargin + (int)(tvDeleteSelectedLayoutParams.RightMargin * 4);
                tvDeleteSelectedLayoutParams.BottomMargin = tvDeleteSelectedLayoutParams.BottomMargin * 5  +(int)(tvDeleteSelectedLayoutParams.BottomMargin * .4);
                tvDeleteSelected.LayoutParameters = tvDeleteSelectedLayoutParams;
                tvDeleteSelected.Animation = childFab_show;
                tvDeleteSelected.Clickable = true;

                com.refractored.fab.FloatingActionButton editSelectedFab = popupMenuView.FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.editSelectedFab);
                FrameLayout.LayoutParams editSelectedFabLayoutParams = (FrameLayout.LayoutParams)editSelectedFab.LayoutParameters;
                editSelectedFabLayoutParams.RightMargin = editSelectedFabLayoutParams.RightMargin + (int)(editSelectedFabLayoutParams.RightMargin * .60);
                editSelectedFabLayoutParams.BottomMargin = editSelectedFabLayoutParams.BottomMargin * 8;
                editSelectedFab.LayoutParameters = editSelectedFabLayoutParams;
                editSelectedFab.Size = FabSize.Mini;
                editSelectedFab.Animation = childFab_show;
                editSelectedFab.Clickable = true;

                TextView tvEditSelected = popupMenuView.FindViewById<TextView>(Resource.Id.tvEditSelected);
                FrameLayout.LayoutParams tvEditSelectedLayoutParams = (FrameLayout.LayoutParams)tvEditSelected.LayoutParameters;
                tvEditSelectedLayoutParams.RightMargin = tvEditSelectedLayoutParams.RightMargin + (int)(tvEditSelectedLayoutParams.RightMargin * 4);
                tvEditSelectedLayoutParams.BottomMargin = tvEditSelectedLayoutParams.BottomMargin * 8 + (int)(tvEditSelectedLayoutParams.BottomMargin * .4);
                tvEditSelected.LayoutParameters = tvEditSelectedLayoutParams;
                tvEditSelected.Animation = childFab_show;
                tvEditSelected.Clickable = true;

                com.refractored.fab.FloatingActionButton uncheckAllFab = popupMenuView.FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.unCheckAllFab);
                FrameLayout.LayoutParams uncheckAllFabLayoutParams = (FrameLayout.LayoutParams)uncheckAllFab.LayoutParameters;
                uncheckAllFabLayoutParams.RightMargin = uncheckAllFabLayoutParams.RightMargin + (int)(uncheckAllFabLayoutParams.RightMargin * .60);
                uncheckAllFabLayoutParams.BottomMargin = uncheckAllFabLayoutParams.BottomMargin * 11;
                uncheckAllFab.LayoutParameters = uncheckAllFabLayoutParams;
                uncheckAllFab.Size = FabSize.Mini;
                uncheckAllFab.Animation = childFab_show;
                uncheckAllFab.Clickable = true;

                TextView tvUncheckAll = popupMenuView.FindViewById<TextView>(Resource.Id.tvUncheckAll);
                FrameLayout.LayoutParams tvUncheckAllLayoutParams = (FrameLayout.LayoutParams)tvUncheckAll.LayoutParameters;
                tvUncheckAllLayoutParams.RightMargin = tvUncheckAllLayoutParams.RightMargin + (int)(tvUncheckAllLayoutParams.RightMargin * 4);
                tvUncheckAllLayoutParams.BottomMargin = tvUncheckAllLayoutParams.BottomMargin * 11 + (int)(tvUncheckAllLayoutParams.BottomMargin * .4);
                tvUncheckAll.LayoutParameters = tvUncheckAllLayoutParams;
                tvUncheckAll.Animation = childFab_show;
                tvUncheckAll.Clickable = true;

                com.refractored.fab.FloatingActionButton checkAllFab = popupMenuView.FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.checkAllFab);
                FrameLayout.LayoutParams checkAllFabLayoutParams = (FrameLayout.LayoutParams)checkAllFab.LayoutParameters;
                checkAllFabLayoutParams.RightMargin = checkAllFabLayoutParams.RightMargin + (int)(checkAllFabLayoutParams.RightMargin * .60);
                checkAllFabLayoutParams.BottomMargin = checkAllFabLayoutParams.BottomMargin * 14;
                checkAllFab.LayoutParameters = checkAllFabLayoutParams;
                checkAllFab.Size = FabSize.Mini;
                checkAllFab.Animation = childFab_show;
                checkAllFab.Clickable = true;

                TextView tvCheckAll = popupMenuView.FindViewById<TextView>(Resource.Id.tvCheckAll);
                FrameLayout.LayoutParams tvCheckAllLayoutParams = (FrameLayout.LayoutParams)tvCheckAll.LayoutParameters;
                tvCheckAllLayoutParams.RightMargin = tvCheckAllLayoutParams.RightMargin + (int)(tvCheckAllLayoutParams.RightMargin * 4);
                tvCheckAllLayoutParams.BottomMargin = tvCheckAllLayoutParams.BottomMargin * 14 + (int)(tvCheckAllLayoutParams.BottomMargin * .4);
                tvCheckAll.LayoutParameters = tvCheckAllLayoutParams;
                tvCheckAll.Animation = childFab_show;
                tvCheckAll.Clickable = true;

                com.refractored.fab.FloatingActionButton searchFab = popupMenuView.FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.searchFab);
                FrameLayout.LayoutParams searchFabLayoutParams = (FrameLayout.LayoutParams)searchFab.LayoutParameters;
                searchFabLayoutParams.RightMargin = searchFabLayoutParams.RightMargin + (int)(searchFabLayoutParams.RightMargin * .60);
                searchFabLayoutParams.BottomMargin = searchFabLayoutParams.BottomMargin * 17;
                searchFab.LayoutParameters = searchFabLayoutParams;
                searchFab.Size = FabSize.Mini;
                searchFab.Animation = childFab_show;
                searchFab.Clickable = true;

                popupMenuView.Click += (object sender1, EventArgs e1) =>
                {
                    new System.Threading.Thread(new ThreadStart(delegate
                    {
                        RunOnUiThread(() =>
                        {
                            //Set tvs hide animation
                            tvDeleteSelected.Animation = childFab_hide;
                            tvEditSelected.Animation = childFab_hide;
                            tvCheckAll.Animation = childFab_hide;
                            tvUncheckAll.Animation = childFab_hide;

                            //Start tvs hide animation
                            tvDeleteSelected.StartAnimation(tvDeleteSelected.Animation);
                            tvEditSelected.StartAnimation(tvEditSelected.Animation);
                            tvCheckAll.StartAnimation(tvCheckAll.Animation);
                            tvUncheckAll.StartAnimation(tvUncheckAll.Animation);

                            //Set fabs hide animation
                            mainFab.Animation = mainFab_hide;
                            deleteSelectedFab.Animation = childFab_hide;
                            editSelectedFab.Animation= childFab_hide;
                            checkAllFab.Animation = childFab_hide;
                            uncheckAllFab.Animation = childFab_hide;
                            searchFab.Animation = childFab_hide;

                            //Start fabs hide animation
                            mainFab.StartAnimation(mainFab.Animation);
                            deleteSelectedFab.StartAnimation(deleteSelectedFab.Animation);
                            editSelectedFab.StartAnimation(editSelectedFab.Animation);
                            checkAllFab.StartAnimation(checkAllFab.Animation);
                            uncheckAllFab.StartAnimation(uncheckAllFab.Animation);
                            searchFab.StartAnimation(searchFab.Animation);
                        });

                        System.Threading.Thread.Sleep(500);

                        RunOnUiThread(() =>
                        {
                            popupMenuDialog.Dismiss();
                        });
                    })).Start();
                };
            }
        }
        public void etPurchasedPrice_ItemChanged(object sender, View.KeyEventArgs e)
        {
            e.Handled = false;
            if (!etPurchasedPrice.Text.Equals("") && (!etStocks.Text.Equals("")))
                etAvarageCost.Text = etPurchasedPrice.Text;
        }
        public string checkIfValid(string name, string purchasedPrice, string retailPrice, string availableStocks)
        {
            string message = "";
            if (name.Trim().Equals("") || name == null)
                message = "Item description is required.";
            else if (purchasedPrice.Trim().Equals("") || purchasedPrice == null)
                message = "Purchased Price is required.";
            else if (retailPrice.Trim().Equals("") || retailPrice == null)
                message = "Retail Price is required.";
            else if (availableStocks.Trim().Equals("") || availableStocks == null)
                message = "Available Stock(s) is required.";

            return message;
        }
        public void btnClearItem_Clicked(object sender, EventArgs e)
        {
            this.reset();
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
                    //Save item
                    command = string.Format("INSERT INTO Item(Barcode, NAME, PurchasedPrice, RetailPrice, AvailableStock, AverageCost) VALUES('{0}', '{1}', {2}, {3}, {4}, {5});", etBarcode.Text, actvItemName.Text.Replace("'", "/"), Convert.ToDouble(etPurchasedPrice.Text), Convert.ToDouble(etRetailPrice.Text), Convert.ToInt16(etStocks.Text), Convert.ToDouble(etAvarageCost.Text)) + "\n";
                    saveItem = sqliteADO.ExecuteNonQuery("BEGIN TRANSACTION;" + "\n" + command + "COMMIT;", dbPath);
                    if (saveItem.status.Equals("SUCCESS"))
                    {
                        this.showSnackBarInfinite("Successfully Saved.");
                        getId = itemModel.GetLastId("my_store.db");
                        if (getId.status.Equals("SUCCESS"))
                        {
                            this.itemListDisplay.Add(new ListItem() { Id = getId.param1, IsChecked = false, Description = actvItemName.Text + "(" + this.etStocks.Text.ToString() + " PCS)(" + this.formatCurrency(this.etRetailPrice.Text.ToString()) + ")" });
                            currentRetrieveItems = this.itemListDisplay.Count;
                            this.reset();
                            //Update list of item in ListView Adapter
                            this.updateItemListAdapter();
                        }
                    }
                    else
                    {
                        this.showMessage("Item(Insert): " + saveItem.message);
                    }
                }
            }
            else
                this.showSnackBar(validate);
        }
        public void updateItemListAdapter()
        {
            RunOnUiThread(() =>
            {
                listItemAdapter.NotifyDataSetChanged();
            });
        }
        public async void btnScanItem_Clicked()
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
                mDrawerLayout.CloseDrawer(mRightDrawer);
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
        private void reset()
        {
            actvItemName.Text = "";
            etBarcode.Text = "";
            etPurchasedPrice.Text = "";
            etAvarageCost.Text = "";
            etRetailPrice.Text = "";
            etStocks.Text = "";
            oldPrice = 0.00;
            oldStock = 0;
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

