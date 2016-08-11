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
using Android.Views.InputMethods;
using POSMobile.CustomClass;
using Android.Telephony;
using Android.Support.V7.Widget;

namespace POSV1
{
    [Activity(Theme = "@style/AppTheme")]
    public class MainActivity : ActionBarActivity//, IScrollDirectorListener, AbsListView.IOnScrollListener
    {
        public static string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "my_store.db");
        private bool 
                isAllowedToCloseDialog = true
                , scannedState = false
                , canScan = false
                , hasAction
                , hasChoseItem = false;
        private string
            requestType
            , evaluationType
            , command
            , filterValue = ""
            , oldFilterValue = ""
            , sslfVendor = " "
            , sslfConsumer = " "
            , srfVendor = " "
            , srfConsumer = " ";
        private double? oldPrice = 0.0;
        private int? oldStock = 0;
        private int currentRetrieveItems = 0, countDialogShown, itemPosition = -1;
        private SupportToolbar mToolbar;
        private AppActionBarDrawerToggle mDrawerToggle;
        private DrawerLayout mDrawerLayout;
        private ListView mRightDrawer, mLeftDrawer, lvItems, lvCartList, lvSalesSummary, lvSalesReport;
        private IMenu menu;
        private AutoCompleteTextView actvItemName, atcvSalesItemDescription;
        private LinearLayout parentLayout;
        private FrameLayout flInvetoryList, flCartList, flMainSales, flAddEditSales, flSalesSummary, flSalesReport;
        private GridLayout glAbout;
        private ScrollView svInventory, svAddEditCart;
        private ListItemAdapter listItemAdapter, cartListAdapter, salesSummaryListAdapter;
        private SalesReportAdapter salesReportAdapter;
        private Dialog popupMenuDialog, salesPopupMenuDialog, dynamicPopupDialog, loaderDialog;
        private DateTime salesDate, salesTime, fromDate, toDate, sslfFromTime, sslfToTime, srfFrom ,srfTo;
        private ArrayAdapter aaSalesItemDescription;
        private ImageView loaderImage;
        private SaleMaster salesMasterModel;
        private SQLiteADO sqliteADO;
        private Item itemHolder = new Item();
        private List<TableItem> tableItems, tableItemOptions;
        private List<ListItem> selectedItems = new List<ListItem>();
        private List<ListItem> itemListDisplay = new List<ListItem>();
        private List<ListItem> cartItemsDisplay = new List<ListItem>();
        private List<ListItem> salesSummaryListDisplay = new List<ListItem>();
        private List<SalesReportItem> salesReportItemDisplay = new List<SalesReportItem>();
        private List<SaleDetail> itemCartList = new List<SaleDetail>();
        private List<SalesSummary> currentSales = new List<SalesSummary>();
        private List<string> aaSalesItemDescriptionList = new List<string>();
        private List<Configuration> access = new List<Configuration>();
        View filterView;
        private Animation 
            mainFab_show
            , mainFab_hide
            , childFab_show
            , childFab_hide
            , loader_show
            , loader_hide;
        private Button btnEvaluateSales
            , btnClearSales
            , btnEvaluateItem
            , btnClearItem
            , btnEvaluateSaleItem
            , btnCancelSales;
        private System.Threading.Thread 
            hideFabThread
            , hideSalesFabThread
            , hideDynamicPopupDialogThread
            , hideLoaderDialogThread
            , fabAnimationWatcher;
        private EditText
            etConsumer
            , etVendor
            , etSearchFab
            , etSearch
            , etSalesQuantity
            , etSalesRetailPrice
            , etSalesTotalPrice
            , etPurchasedPrice
            , etRetailPrice
            , etStocks
            , etAvarageCost
            , etBarcode
            , etSalesBarcode
            , etAmountReceived
            , etConsumerMobileNo;
        private TextView 
            tvSelectSalesDate
            , tvShowSales
            , tvAddSales
            , tvSalesTime
            , tvTotalAmount
            , tvIndicator
            , tvSalesDate
            , tvTotalSales
            , tvNoOfSales
            , tvNoOfItemSold
            , tvShowMore
            , tvShowDetail
            , tvEditSelected
            , tvDeleteSelected
            , tvUncheckAll
            , tvCheckAll
            , tvMainFab
            , tvChildFab1
            , tvChildFab2
            , tvChildFab3
            , tvChildFab4
            , tvChildFab5
            , tvChange;
        private com.refractored.fab.FloatingActionButton
            dMainFab
            , dSearchFab
            , dChildFab1
            , dChildFab2
            , dChildFab3
            , dChildFab4
            , dChildFab5
            , addSalesFab
            , cartListMainFab
            , showSalesDateFab
            , selectSalesDateFab
            , pMainSalesFab
            , mainSalesFab
            , searchFab
            , checkAllFab
            , uncheckAllFab
            , deleteSelectedFab
            , editSelectedFab
            , showDetailFab
            , fabSalesReport
            , mainFab
            , fabItem
            , salesSummaryMainFab;
        private FrameLayout.LayoutParams 
            dMainFabLayoutParams
            , dSearchFabLayoutParams
            , dChildFab1LayoutParams
            , dChildFab2LayoutParams
            , dChildFab3LayoutParams
            , dChildFab4LayoutParams
            , dChildFab5LayoutParams
            , loaderImageFrameLayoutParams
            , tvMainFabLayoutParams
            , etSearchFabLayoutParams
            , tvChildFab1LayoutParams
            , tvChildFab2LayoutParams
            , tvChildFab3LayoutParams
            , tvChildFab4LayoutParams
            , tvChildFab5LayoutParams
            , tvAddSalesLayoutParams
            , tvShowSalesLayoutParams
            , showSalesFabLayoutParams
            , addSalesFabLayoutParams
            , tvSelectSalesDateLayoutParams
            , selectSalesDateFabLayoutParams
            , pMainSalesFabLayoutParams
            , etSearchLayoutParams
            , searchFabLayoutParams
            , tvCheckAllLayoutParams
            , checkAllFabLayoutParams
            , uncheckAllFabLayoutParams
            , tvDeleteSelectedLayoutParams
            , tvEditSelectedLayoutParams
            , editSelectedFabLayoutParams
            , showDetailFabLayoutParams
            , tvShowMoreLayoutParams
            , mainLayoutParams
            , tvUncheckAllLayoutParams
            , deleteSelectedFabLayoutParams
            , tvShowDetailLayoutParams;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            //Toolbar widgets and drawers
            this.mToolbar = FindViewById<SupportToolbar>(Resource.Id.toolBar);
            this.mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            this.mLeftDrawer = FindViewById<ListView>(Resource.Id.left_drawer);
            this.mRightDrawer = FindViewById<ListView>(Resource.Id.right_drawer);

            //Other widgets
            this.btnEvaluateItem = FindViewById<Button>(Resource.Id.btnEvaluateItem);
            this.btnClearItem = FindViewById<Button>(Resource.Id.btnClearItem);
            this.etPurchasedPrice = FindViewById<EditText>(Resource.Id.etPurchasedPrice);
            this.etRetailPrice = FindViewById<EditText>(Resource.Id.etRetailPrice);
            this.etStocks = FindViewById<EditText>(Resource.Id.etAvailableStocks);
            this.etAvarageCost = FindViewById<EditText>(Resource.Id.etAverageCost);
            this.etBarcode = FindViewById<EditText>(Resource.Id.etBarcode);
            this.etSalesBarcode = FindViewById<EditText>(Resource.Id.etSalesBarcode);
            this.actvItemName = FindViewById<AutoCompleteTextView>(Resource.Id.atcvDescription);
            this.tvSalesDate = FindViewById<TextView>(Resource.Id.tvSalesDate);
            this.tvNoOfSales = FindViewById<TextView>(Resource.Id.tvNoOfSales);
            this.tvTotalSales = FindViewById<TextView>(Resource.Id.tvTotalSales);
            this.tvNoOfItemSold = FindViewById<TextView>(Resource.Id.tvNoOfItems);
            this.tvSalesTime = FindViewById<TextView>(Resource.Id.tvSalesTime);
            this.tvTotalAmount = FindViewById<TextView>(Resource.Id.tvTotalAmount);
            this.tvChange = FindViewById<TextView>(Resource.Id.tvChange);
            this.etConsumer = FindViewById<EditText>(Resource.Id.etConsumer);
            this.etConsumerMobileNo = FindViewById<EditText>(Resource.Id.etConsumerMobileNo);
            this.etVendor = FindViewById<EditText>(Resource.Id.etVendor);
            this.etAmountReceived = FindViewById<EditText>(Resource.Id.etAmountReceived);
            this.btnEvaluateSales = FindViewById<Button>(Resource.Id.btnEvaluateSale);
            this.btnClearSales = FindViewById<Button>(Resource.Id.btnClearSale);
            this.etSalesQuantity = FindViewById<EditText>(Resource.Id.etSalesQuantity);
            this.etSalesRetailPrice = FindViewById<EditText>(Resource.Id.etSalesRetailPrice);
            this.etSalesTotalPrice = FindViewById<EditText>(Resource.Id.etSalesTotalPrice);
            this.atcvSalesItemDescription = FindViewById<AutoCompleteTextView>(Resource.Id.atcvSalesItemDescription);
            this.btnEvaluateSaleItem = FindViewById<Button>(Resource.Id.btnEvaluateSaleItem);
            this.btnCancelSales = FindViewById<Button>(Resource.Id.btnCancelSales);

            //Fabs
            this.fabItem = FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.fabItem);
            this.mainSalesFab = FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.fabMainSales);
            this.fabSalesReport = FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.salesReportMainFab);
            this.cartListMainFab = FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.cartListMainFab);
            this.salesSummaryMainFab = FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.salesSummaryMainFab);

            //Layouts
            this.parentLayout = FindViewById<LinearLayout>(Resource.Id.parentLayout);
            this.glAbout = FindViewById<GridLayout>(Resource.Id.About);
            this.flInvetoryList = FindViewById<FrameLayout>(Resource.Id.InventoryList);
            this.svInventory = FindViewById<ScrollView>(Resource.Id.Inventory);
            this.flMainSales = FindViewById<FrameLayout>(Resource.Id.SalesMainLayout);
            this.flAddEditSales = FindViewById<FrameLayout>(Resource.Id.AddEditLayout);
            this.svAddEditCart = FindViewById<ScrollView>(Resource.Id.AddEditCart);
            this.flCartList = FindViewById<FrameLayout>(Resource.Id.CartList);
            this.flSalesSummary = FindViewById<FrameLayout>(Resource.Id.SalesSummary);
            this.flSalesReport = FindViewById<FrameLayout>(Resource.Id.SalesReport);

            //ListViews
            this.lvItems = FindViewById<ListView>(Resource.Id.lvList);
            this.lvCartList = FindViewById<ListView>(Resource.Id.lvCart);
            this.lvSalesSummary = FindViewById<ListView>(Resource.Id.lvSalesSummary);
            this.lvSalesReport = FindViewById<ListView>(Resource.Id.lvSalesReport);

            //Set Animation
            this.setAnimations();

            this.mLeftDrawer.Tag = 0;
            this.mRightDrawer.Tag = 1;

            this.lvItems.ChoiceMode = ChoiceMode.Multiple;

            //Events
            this.fabItem.Click += fabItem_Clicked;
            this.mainSalesFab.Click += this.mainSalesFab_Clicked;
            this.cartListMainFab.Click += this.cartListMainFab_Clicked;
            this.salesSummaryMainFab.Click += this.salesSummaryMainFab_Clicked;
            this.fabSalesReport.Click += this.salesReportMainFab_Clicked;
            this.mLeftDrawer.ItemClick += this.mLeftDrawer_ItemClicked;
            this.mRightDrawer.ItemClick += this.mRightDrawer_ItemClicked;
            this.atcvSalesItemDescription.ItemClick += this.atcvSalesItemDescription_Clicked;
            this.etSalesQuantity.KeyPress += this.etSalesQuantity_ItemChanged;
            this.etAmountReceived.KeyPress += this.etAmountReceived_ItemChanged;
            this.btnEvaluateSaleItem.Click += this.btnAddEditItemToCart_Clicked;
            this.btnCancelSales.Click += this.btnCancelItemInCart_Clicked;
            this.btnEvaluateSales.Click += this.btnEvaluateSales_Clicked;
            this.btnClearSales.Click += this.btnClearSales_Clicked;
            this.btnEvaluateItem.Click += this.btnEvaluateItem_Clicked;
            this.btnClearItem.Click += this.btnClearItem_Clicked;

            this.requestType = "Show Items";
            this.setTableItems();
            this.setTableItemOptions();

            this.mLeftDrawer.Adapter = new LeftDrawerAdapter(this, this.tableItems);
            this.mRightDrawer.Adapter = new RightDrawerAdapter(this, this.tableItemOptions);
            this.mDrawerToggle = new AppActionBarDrawerToggle(
                    this, //Action Bar Activity
                    this.mDrawerLayout, // Drawer Layout
                    Resource.String.LeftDrawerDescWhenOpen,
                    Resource.String.LeftDrawerDescWhenClose
                );

            this.SetSupportActionBar(mToolbar);
            //Listens if drawer is open or closed
            this.mDrawerLayout.SetDrawerListener(mDrawerToggle);
            //Displays the toggle button
            this.SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            //Enables the opening or closing of drawer
            this.SupportActionBar.SetHomeButtonEnabled(true);
            //Enables Toolbar title
            this.SupportActionBar.SetDisplayShowTitleEnabled(true);
            this.mDrawerToggle.SyncState();

            this.salesDate = DateTime.Now;
            this.initializeToolbarTitle(bundle);
            this.getItems("SELECT * FROM Item");
            this.getAccess();
        }
        public void hideLayouts()
        {
            this.scannedState = false;
            this.glAbout.Visibility = ViewStates.Gone;
            this.svInventory.Visibility = ViewStates.Gone;
            this.flInvetoryList.Visibility = ViewStates.Gone;
            this.flMainSales.Visibility = ViewStates.Gone;
            this.flCartList.Visibility = ViewStates.Gone;
            this.flAddEditSales.Visibility = ViewStates.Gone;
            this.svAddEditCart.Visibility = ViewStates.Gone;
            this.flSalesSummary.Visibility = ViewStates.Gone;
            this.flSalesReport.Visibility = ViewStates.Gone;
            this.hideOptionMenu(Resource.Id.action_more);
            this.hideOptionMenu(Resource.Id.action_refresh);
            this.hasAction = false;
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
            hasAction = false;
            if (this.loaderDialog != null && this.loaderDialog.IsShowing)
            {
                this.loaderDialog.Dismiss();
            }
            var builder = new AlertDialog.Builder(this);
            builder.SetTitle("Application Message");
            builder.SetMessage(message);
            builder.SetPositiveButton("OK", delegate { });
            builder.Create().Show();
        }
        public void showSnackBar(string message)
        {
            hasAction = false;
            if (this.loaderDialog != null && this.loaderDialog.IsShowing)
            {
                this.loaderDialog.Dismiss();
            }
            Snackbar
            .Make(this.parentLayout, message, Snackbar.LengthLong)
            .SetAction("Close", (view) => { })
            .Show(); // Don’t forget to show!});
        }
        public void showSnackBarInfinite(string message)
        {
            this.hasAction = false;
            if (this.loaderDialog != null)
            {
                this.loaderDialog.Dismiss();
            }

            Snackbar
            .Make(this.parentLayout, message, Snackbar.LengthIndefinite)
            .SetAction("Close", (view) => { })
            .Show(); // Don’t forget to show!});
        }
        private void mLeftDrawer_ItemClicked(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (hasAction)
                return;

            if (this.hideFabThread != null)
                this.hideFabThread.Abort();

            this.hideLayouts();
            switch (e.Position)
            {
                case 0:
                    this.resetSaleSummaryFilter();
                    this.getSalesForMainContent(true);
                    break;
                case 1:
                    this.invokeLoader();
                    this.resetItemList();
                    this.requestType = "Show Items";
                    this.setTableItemOptions();
                    this.mRightDrawer.Adapter = new RightDrawerAdapter(this, this.tableItemOptions);
                    this.evaluationType = "";
                    this.mDrawerToggle.setCloseDrawerDesc(Resource.String.Inventory);
                    this.flInvetoryList.Visibility = ViewStates.Visible;
                    this.showOptionMenu(Resource.Id.action_more);
                    this.showOptionMenu(Resource.Id.action_refresh);
                    break;
                case 2:
                    this.mDrawerToggle.setCloseDrawerDesc(Resource.String.LeftDrawerDescWhenClose);
                    this.glAbout.Visibility = ViewStates.Visible;
                    break;
                default:
                    this.mDrawerToggle.setCloseDrawerDesc(Resource.String.Help);
                    break;
            }
            this.mDrawerLayout.CloseDrawer(mLeftDrawer);
        }
        private void mRightDrawer_ItemClicked(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (this.hideFabThread != null)
                this.hideFabThread.Abort();

            if (this.hasAction)
                return;

            if (!this.requestType.Equals("Show Sales"))
            {
                this.hideLayouts();
                this.etBarcode.Enabled = true;
                this.etPurchasedPrice.Enabled = true;
                this.etRetailPrice.Enabled = true;
                this.etAvarageCost.Enabled = false;
                this.etStocks.Enabled = true;
                if (canScan)
                {
                    switch (e.Position)
                    {
                        case 0://Scan
                            if (this.requestType.Equals("Show Items"))
                            {
                                this.resetItem();
                                this.btnScanItem_Clicked();
                            }
                            break;
                        case 1: //Add
                            if (this.requestType.Equals("Show Items"))
                            {
                                this.resetItem();
                                this.evaluationType = "Save";
                                this.svInventory.Visibility = ViewStates.Visible;
                                this.showOptionMenu(Resource.Id.action_more);
                                this.etPurchasedPrice.KeyPress += this.etPurchasedPrice_ItemChanged;
                                this.etStocks.KeyPress += this.etPurchasedPrice_ItemChanged;
                            }
                            break;
                        default:
                            if (this.requestType.Equals("Show Items"))
                            {
                                this.evaluationType = "ShowItemList";
                                this.flInvetoryList.Visibility = ViewStates.Visible;
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
                            if (this.requestType.Equals("Show Items"))
                            {
                                this.resetItem();
                                this.evaluationType = "Save";
                                this.svInventory.Visibility = ViewStates.Visible;
                                this.showOptionMenu(Resource.Id.action_more);
                                this.etPurchasedPrice.KeyPress += this.etPurchasedPrice_ItemChanged;
                                this.etStocks.KeyPress += this.etPurchasedPrice_ItemChanged;
                            }
                            break;
                        default:
                            if (this.requestType.Equals("Show Items"))
                            {
                                this.evaluationType = "ShowItemList";
                                this.flInvetoryList.Visibility = ViewStates.Visible;
                                this.showOptionMenu(Resource.Id.action_more);
                                this.showOptionMenu(Resource.Id.action_refresh);
                            }
                            break;
                    }
                }
                this.btnEvaluateItem.Text = this.evaluationType;
            }
            else
            {
                //Reports
                if (this.flMainSales.Visibility == ViewStates.Visible 
                    || this.flSalesReport.Visibility == ViewStates.Visible 
                    || this.flSalesSummary.Visibility == ViewStates.Visible)
                {
                    if (this.hasAction)
                        return;

                    this.hasAction = true;

                    AlertDialog.Builder dialogBuilder = new AlertDialog.Builder(this);
                    this.filterView = LayoutInflater.Inflate(Resource.Layout.SaleReportFilterLayout, null);

                    if (this.salesReportItemDisplay.Count == 0)
                    {
                        this.srfFrom = DateTime.Now;
                        this.srfTo = DateTime.Now;
                        this.srfVendor = this.filterView.FindViewById<EditText>(Resource.Id.etReportVendor).Text;
                        this.srfConsumer = this.filterView.FindViewById<EditText>(Resource.Id.etReportConsumer).Text;
                    }

                    this.filterView.FindViewById<EditText>(Resource.Id.etReportFrom).Text = this.srfFrom.ToString("dddd, MMMM dd, yyyy");
                    this.filterView.FindViewById<EditText>(Resource.Id.etReportTo).Text = this.srfTo.ToString("dddd, MMMM dd, yyyy");
                    this.filterView.FindViewById<EditText>(Resource.Id.etReportVendor).Text = this.srfVendor.ToString();
                    this.filterView.FindViewById<EditText>(Resource.Id.etReportConsumer).Text = this.srfConsumer.ToString();
                    this.filterView.FindViewById<EditText>(Resource.Id.etReportFocus).RequestFocus();

                    this.filterView.FindViewById<EditText>(Resource.Id.etReportFrom).FocusChange += this.focusReportListener;
                    this.filterView.FindViewById<EditText>(Resource.Id.etReportTo).FocusChange += this.focusReportListener;

                    dialogBuilder.SetCancelable(false);
                    dialogBuilder.SetView(filterView);
                    dialogBuilder.SetNegativeButton("Cancel", delegate
                    {
                        this.srfFrom = this.srfFrom == DateTime.Now ? DateTime.Now : this.srfFrom;
                        this.srfTo = this.srfTo == DateTime.Now ? DateTime.Now : this.srfTo;
                        this.hasAction = false;
                    });

                    switch (e.Position)
                    {
                        case 0: // Sales By Date
                            dialogBuilder.SetPositiveButton("Find", delegate
                            {
                                this.hideLayouts();
                                this.salesReportItemDisplay.Clear();
                                this.getSalesReportByDate(this.srfFrom, this.srfTo, false);
                            });
                            break;
                        case 1:  // Sales By Consumer
                            this.filterView.FindViewById<EditText>(Resource.Id.etReportConsumer).Visibility = ViewStates.Visible;
                            dialogBuilder.SetPositiveButton("Find", delegate
                            {
                                this.hideLayouts();
                                this.salesReportItemDisplay.Clear();
                                this.srfConsumer = this.filterView.FindViewById<EditText>(Resource.Id.etReportConsumer).Text.Trim().ToUpper();
                                this.getSalesReportByConsumer(this.srfFrom, this.srfTo, this.srfConsumer, false);
                            });
                            break;
                        default:  // Sales By Vendor
                            this.filterView.FindViewById<EditText>(Resource.Id.etReportVendor).Visibility = ViewStates.Visible;
                            dialogBuilder.SetPositiveButton("Find", delegate
                            {
                                this.hideLayouts();
                                this.salesReportItemDisplay.Clear();
                                this.srfVendor = this.filterView.FindViewById<EditText>(Resource.Id.etReportVendor).Text.Trim().ToUpper();
                                this.getSalesReportByVendor(this.srfFrom, this.srfTo, this.srfVendor, false);
                            });
                            break;
                    }
                    dialogBuilder.Create().Show();
                }
                else
                {
                    if (canScan)
                    {
                        switch (e.Position)
                        {
                            case 0:
                                this.hideLayouts();
                                this.scanSalesItemFab_Clicked(new object(), new EventArgs());
                                break;
                            case 1:
                                this.hideLayouts();
                                this.addItemToCartFab_Clicked(new object(), new EventArgs());
                                break;
                            default:
                                if (this.itemCartList.Count > 0)
                                {
                                    this.hideLayouts();
                                    this.showCartFab_Clicked(new object(), new EventArgs());
                                }
                                else
                                {
                                    this.showMessage("No items in cart, please add one or more.");
                                }
                                break;
                        }
                    }
                    else
                    {
                        switch (e.Position)
                        {
                            case 0:
                                this.hideLayouts();
                                this.addItemToCartFab_Clicked(new object(), new EventArgs());
                                break;
                            default:
                                if (this.itemCartList.Count > 0)
                                {
                                    this.hideLayouts();
                                    this.showCartFab_Clicked(new object(), new EventArgs());
                                }
                                else
                                {
                                    this.showMessage("No items in cart, please add one or more.");
                                }
                                break;
                        }
                    }
                }
            }
            this.mDrawerLayout.CloseDrawer(this.mRightDrawer);
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
                    else if (svInventory.Visibility == ViewStates.Visible)
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
        private void getAccess() 
        {
            Response getAccess = new Response();
            Configuration configurationModel = new Configuration();

            tableItemOptions = new List<TableItem>();
            tableItemOptions.Clear();
            getAccess = configurationModel.GetAllConfiguration("my_store.db");
            if (getAccess.status.Equals("SUCCESS"))
            {
                this.access = getAccess.configurationList;
            }
            else
            {
                this.showMessage(getAccess.message);
            }
        }
        private void setTableItemOptions()
        {
            if (requestType.Equals("Show Items"))
            {
                Response getAccess = new Response();
                Configuration configurationModel = new Configuration();

                tableItemOptions = new List<TableItem>();
                tableItemOptions.Clear();
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
            else
            {
                tableItemOptions.Clear();
                if (this.flMainSales.Visibility == ViewStates.Visible)
                {
                    tableItemOptions.Add(new TableItem() { ImageResourceId = Resource.Drawable.Report, ItemLabel = "Sales By Date" });
                    tableItemOptions.Add(new TableItem() { ImageResourceId = Resource.Drawable.Report, ItemLabel = "Sales By Consumer" });
                    tableItemOptions.Add(new TableItem() { ImageResourceId = Resource.Drawable.Report, ItemLabel = "Sales By Vendor" });
                }
                else
                {
                    foreach (Configuration conf in this.access)
                    {
                        if (conf.Description.Equals("AllowScanning"))
                        {
                            if (conf.IsGranted == 1)
                            {
                                this.canScan = true;
                                tableItemOptions.Add(new TableItem() { ImageResourceId = Resource.Drawable.Scan, ItemLabel = "Scan Item" });
                                break;
                            }
                        }
                    }

                    tableItemOptions.Add(new TableItem() { ImageResourceId = Resource.Drawable.Add, ItemLabel = "Add Item To Cart" });
                    tableItemOptions.Add(new TableItem() { ImageResourceId = Resource.Drawable.Cart, ItemLabel = "Show Cart" });
                }
            }
        }
        //function that will listen if the left/right toggle menu is selected
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (hideFabThread != null)
                hideFabThread.Abort();

            if (this.hasAction)
                return true;

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
                        //Item List
                        if (requestType.Equals("Show Items"))
                        {
                            this.invokeLoader();
                            this.resetItemList();
                        }
                        //Sale Summary List
                        if (this.flSalesSummary.Visibility == ViewStates.Visible)
                        {
                            this.invokeLoader();
                            this.salesSummaryListDisplay.Clear();
                            Response getSalesSummary = new Response();
                            SalesSummary salesSummaryModel = new SalesSummary();

                            getSalesSummary = salesSummaryModel.getSalesByDateTime(this.salesDate
                                                                                    , this.sslfFromTime
                                                                                    , this.sslfToTime
                                                                                    , this.sslfConsumer
                                                                                    , this.sslfVendor
                                                                                    , "my_store.db"
                                                                                    , this.salesSummaryListDisplay.Count);
                            if (getSalesSummary.status.Equals("SUCCESS"))
                            {
                                foreach (SalesSummary ssm in getSalesSummary.salesSummary)
                                {
                                    this.salesSummaryListDisplay.Add(new ListItem()
                                    {
                                        Id = ssm.SaleMasterId
                                        , IsChecked = false
                                        , Description = string.Format("{0} - {1} {2} sold for {3}"
                                        , ssm.SalesDateTime.ToString("hh:mm:ss tt")
                                        , ssm.ItemsSold
                                        , ssm.ItemsSold > 1 ? "Items" : "Item"
                                        , this.formatCurrency(ssm.Amount.ToString()))
                                    });
                                }
                                if (this.salesSummaryListAdapter == null)
                                {
                                    this.salesSummaryListAdapter = new ListItemAdapter(this, this.salesSummaryListDisplay);
                                    this.lvSalesSummary.Adapter = this.salesSummaryListAdapter;
                                }
                                RunOnUiThread(() =>
                                {
                                    this.salesSummaryListAdapter.NotifyDataSetChanged();
                                });
                                this.hideLoader();
                            }
                            else
                            {
                                this.hideLoader();
                                this.showMessage(getSalesSummary.message);
                            }
                        }
                        //Sales Report List
                        if (this.flMainSales.Visibility == ViewStates.Visible || this.flSalesReport.Visibility == ViewStates.Visible)
                        {
                            if (this.salesReportItemDisplay[0].type == 1)
                            {
                                //Sales By Date
                                this.salesReportItemDisplay.Clear();
                                this.getSalesReportByDate(this.srfFrom, this.srfTo, true);
                            }
                            else if (this.salesReportItemDisplay[0].type == 2)
                            {
                                //Sales By Consumer
                                this.salesReportItemDisplay.Clear();
                                this.getSalesReportByConsumer(this.srfFrom, this.srfTo, this.srfConsumer, true);
                            }
                            else
                            {
                                //Sales By Vendor
                                this.salesReportItemDisplay.Clear();
                                this.getSalesReportByVendor(this.srfFrom, this.srfTo, this.srfVendor, true);
                            }
                        }
                    });
                    messageDialog.Show();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
        //function that will be invoke during creation of right menus
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            this.menu = menu;
            MenuInflater.Inflate(Resource.Menu.action_menu, menu);
            this.hideOptionMenu(Resource.Id.action_more);
            this.hideOptionMenu(Resource.Id.action_refresh);
            return base.OnCreateOptionsMenu(menu);
        }
        public void setAnimations()
        {
            //Animations
            mainFab_show = AnimationUtils.LoadAnimation(this, Resource.Layout.mainFab_show);
            mainFab_hide = AnimationUtils.LoadAnimation(this, Resource.Layout.mainFab_hide);
            childFab_show = AnimationUtils.LoadAnimation(this, Resource.Layout.childFab_show);
            childFab_hide = AnimationUtils.LoadAnimation(this, Resource.Layout.childFab_hide);
            loader_show = AnimationUtils.LoadAnimation(this, Resource.Layout.loader_show);
            loader_hide = AnimationUtils.LoadAnimation(this, Resource.Layout.loader_hide);
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
        public void invokeLoader(){
            this.hasAction = true;
            if (this.loaderDialog == null)
            {
                this.showLoader();
            }
            else
            {
                if(!this.loaderDialog.IsShowing)
                {
                    this.showLoader();
                }
            }
        }
        public void showLoader() 
        {
            var loaderMenuView = LayoutInflater.Inflate(Resource.Layout.LoaderLayout, null);
            this.loaderDialog = new Dialog(this, Android.Resource.Style.ThemeTranslucentNoTitleBar);
            this.loaderDialog.SetContentView(loaderMenuView);
            this.loaderDialog.Show();
            this.loaderDialog.SetCancelable(false);

            this.loaderImage = loaderMenuView.FindViewById<ImageView>(Resource.Id.Image);
            this.loaderImageFrameLayoutParams = (FrameLayout.LayoutParams)this.loaderImage.LayoutParameters;
            this.loaderImage.LayoutParameters = this.loaderImageFrameLayoutParams;
            this.loaderImage.Animation = this.loader_show;
            this.loaderImage.Clickable = true;
        }
        public void hideLoader() 
        {
            hideLoaderDialogThread = new System.Threading.Thread(new ThreadStart(delegate
            {
                RunOnUiThread(() =>
                {
                    this.loaderImage.Animation = this.loader_hide;
                    this.loaderImage.StartAnimation(this.loaderImage.Animation);
                });

                System.Threading.Thread.Sleep(500);
                RunOnUiThread(() =>
                {
                    loaderDialog.Dismiss();
                    this.hasAction = false;
                });
            }));
            hideLoaderDialogThread.Start();
        }
        public void hideKeyBoard() 
        {
            InputMethodManager inputManager = (InputMethodManager)this.GetSystemService(Context.InputMethodService);
            inputManager.HideSoftInputFromWindow(this.CurrentFocus.WindowToken, HideSoftInputFlags.NotAlways);
        }
        public void sendSMS(string recipient, string smsMessage)
        {
            List<string> sms = new List<string>();
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            int divisor = 0;
            int max = 160;
            builder.SetTitle("Send SMS");
            builder.SetMessage("Proceed to sending of purchased summary.");
            builder.SetNegativeButton("No", delegate { });
            builder.SetPositiveButton("Yes", delegate {
                divisor = (smsMessage.Length / max) == 0 ? 1 : (smsMessage.Length / max) + 1;
                if (divisor != 1)
                {
                    for (int i = 1; i <= divisor; i++)
                    {
                        if (i != divisor)
                        {
                            if (i == 1)
                            {
                                SmsManager.Default.SendTextMessage(recipient, null,
                                                      smsMessage.Substring(0, max), null, null);
                            }
                            else
                            {
                                SmsManager.Default.SendTextMessage(recipient, null,
                                                     smsMessage.Substring(((i - 1) * max), max), null, null);
                            }
                        }
                        else
                        {
                            SmsManager.Default.SendTextMessage(recipient, null,
                                                      smsMessage.Substring(((i - 1) * max), (smsMessage.Length - ((i - 1) * max))), null, null);
                        }
                    }
                }
                else
                {
                    SmsManager.Default.SendTextMessage(recipient, null,
                                                      smsMessage, null, null);
                }
            });
            builder.SetCancelable(false);
            builder.Create().Show();
        }
        /* ------------------------------------------SALES--------------------------------------*/
        public void getSalesReportByDate(DateTime fromDate, DateTime toDate, bool showMore)
        {
            Response getSalesReportCount = new Response();
            Response getSalesReportList = new Response();
            SalesSummary getSalesReportCountA = new SalesSummary();
            SalesSummary getSalesReportListA = new SalesSummary();

            this.invokeLoader();
            string command = string.Format("DATE(SalesDateTime) BETWEEN DATE('{0}') AND DATE('{1}')"
                , fromDate.ToString("yyyy-MM-dd ")
                , toDate.ToString("yyyy-MM-dd "));
            getSalesReportCount = getSalesReportCountA.getSalesReportCount(command, "my_store.db");
            if (getSalesReportCount.status.Equals("SUCCESS"))
            {
                if (getSalesReportCount.param1 == 0 && !showMore)
                {
                    this.showMessage("No record has been retrieved.");
                    this.flMainSales.Visibility = ViewStates.Visible;
                    this.showOptionMenu(Resource.Id.action_more);
                    this.hideLoader();
                    return;
                }
                else if(getSalesReportCount.param1 == 0 && showMore)
                {
                    this.flSalesReport.Visibility = ViewStates.Visible;
                    this.showOptionMenu(Resource.Id.action_more);
                    this.hideLoader();
                    return;
                }

                getSalesReportList = getSalesReportListA.getSalesReportByDate(fromDate, toDate, "my_store.db", this.salesReportItemDisplay.Count);
                if (getSalesReportList.status.Equals("SUCCESS"))
                {
                    foreach (SalesReportItem ssr in getSalesReportList.salesReportList)
                    {
                        this.salesReportItemDisplay.Add(new SalesReportItem()
                        {
                            type = 1
                            , salesDate = ssr.salesDate
                            , noOfSales = ssr.noOfSales
                            , salesTotalAmount = this.formatCurrency(ssr.salesTotalAmount)
                            , salesNoOfItems = ssr.salesNoOfItems
                        });
                    }

                    if (this.salesReportAdapter == null)
                    {
                        this.salesReportAdapter = new SalesReportAdapter(this, this.salesReportItemDisplay);
                        this.lvSalesReport.Adapter = this.salesReportAdapter;
                    }
                    //Update list of item in ListView Adapter
                    RunOnUiThread(() =>
                    {
                        if (this.salesReportAdapter != null)
                        {
                            this.salesReportAdapter.NotifyDataSetChanged();
                        }
                    });
                    if (!showMore)
                    {
                        this.flSalesReport.Visibility = ViewStates.Visible;
                        this.showOptionMenu(Resource.Id.action_more);
                        this.showOptionMenu(Resource.Id.action_refresh);
                        this.mDrawerLayout.OpenDrawer(mLeftDrawer);
                        this.mDrawerToggle.setCloseDrawerDesc(Resource.String.SalesByDate);
                    }
                    new System.Threading.Thread(new ThreadStart(delegate
                    {
                        System.Threading.Thread.Sleep(500);
                        RunOnUiThread(() =>
                        {
                            this.mDrawerLayout.CloseDrawer(mLeftDrawer);
                        });
                    })).Start();
                    this.hideLoader();
                }
                else
                {
                    this.flMainSales.Visibility = ViewStates.Visible;
                    this.showOptionMenu(Resource.Id.action_more);
                    this.showMessage(getSalesReportList.message);
                }
            }
            else
            {
                this.flMainSales.Visibility = ViewStates.Visible;
                this.showOptionMenu(Resource.Id.action_more);
                this.showMessage(getSalesReportCount.message);
            }
        }
        public void getSalesReportByConsumer(DateTime fromDate, DateTime toDate, string consumer, bool showMore)
        {
            Response getSalesReportCount = new Response();
            Response getSalesReportList = new Response();
            SalesSummary getSalesReportCountA = new SalesSummary();
            SalesSummary getSalesReportListA = new SalesSummary();

            this.invokeLoader();
            string command = string.Format("SoldTo LIKE '%{0}%' AND DATE(SalesDateTime) BETWEEN DATE('{1}') AND DATE('{2}')"
                , consumer
                , fromDate.ToString("yyyy-MM-dd ")
                , toDate.ToString("yyyy-MM-dd "));
            getSalesReportCount = getSalesReportCountA.getSalesReportCount(command, "my_store.db");
            if (getSalesReportCount.status.Equals("SUCCESS"))
            {
                if (getSalesReportCount.param1 == 0 && !showMore)
                {
                    this.showMessage("No record has been retrieved.");
                    this.flMainSales.Visibility = ViewStates.Visible;
                    this.showOptionMenu(Resource.Id.action_more);
                    this.hideLoader();
                    return;
                }
                else if (getSalesReportCount.param1 == 0 && showMore)
                {
                    this.flSalesReport.Visibility = ViewStates.Visible;
                    this.showOptionMenu(Resource.Id.action_more);
                    this.hideLoader();
                    return;
                }

                getSalesReportList = getSalesReportListA.getSalesReportByConsumer(fromDate, toDate, consumer, "my_store.db", this.salesReportItemDisplay.Count);
                if (getSalesReportList.status.Equals("SUCCESS"))
                {
                    foreach (SalesReportItem ssr in getSalesReportList.salesReportList)
                    {
                        this.salesReportItemDisplay.Add(new SalesReportItem()
                        {
                            type = 2
                            , salesDate = ssr.salesDate
                            , noOfSales = ssr.noOfSales
                            , salesTotalAmount = this.formatCurrency(ssr.salesTotalAmount)
                            , salesNoOfItems = ssr.salesNoOfItems
                            , salesConsumer = ssr.salesConsumer
                        });
                    }

                    if (this.salesReportAdapter == null)
                    {
                        this.salesReportAdapter = new SalesReportAdapter(this, this.salesReportItemDisplay);
                        this.lvSalesReport.Adapter = this.salesReportAdapter;
                    }
                    //Update list of item in ListView Adapter
                    RunOnUiThread(() =>
                    {
                        if (this.salesReportAdapter != null)
                        {
                            this.salesReportAdapter.NotifyDataSetChanged();
                        }
                    });
                    if (!showMore)
                    {
                        this.flSalesReport.Visibility = ViewStates.Visible;
                        this.showOptionMenu(Resource.Id.action_more);
                        this.showOptionMenu(Resource.Id.action_refresh);
                        this.mDrawerLayout.OpenDrawer(mLeftDrawer);
                        this.mDrawerToggle.setCloseDrawerDesc(Resource.String.SalesByConsumer);
                    }
                    new System.Threading.Thread(new ThreadStart(delegate
                    {
                        System.Threading.Thread.Sleep(500);
                        RunOnUiThread(() =>
                        {
                            this.mDrawerLayout.CloseDrawer(mLeftDrawer);
                        });
                    })).Start();
                    this.hideLoader();
                }
                else
                {
                    this.flMainSales.Visibility = ViewStates.Visible;
                    this.showOptionMenu(Resource.Id.action_more);
                    this.showMessage(getSalesReportList.message);
                }
            }
            else
            {
                this.flMainSales.Visibility = ViewStates.Visible;
                this.showOptionMenu(Resource.Id.action_more);
                this.showMessage(getSalesReportCount.message);
            }
        }
        public void getSalesReportByVendor(DateTime fromDate, DateTime toDate, string vendor, bool showMore)
        {
            Response getSalesReportCount = new Response();
            Response getSalesReportList = new Response();
            SalesSummary getSalesReportCountA = new SalesSummary();
            SalesSummary getSalesReportListA = new SalesSummary();

            this.invokeLoader();
            string command = string.Format("SoldBy LIKE '%{0}%' AND DATE(SalesDateTime) BETWEEN DATE('{1}') AND DATE('{2}')"
                , vendor
                , fromDate.ToString("yyyy-MM-dd ")
                , toDate.ToString("yyyy-MM-dd "));
            getSalesReportCount = getSalesReportCountA.getSalesReportCount(command, "my_store.db");
            if (getSalesReportCount.status.Equals("SUCCESS"))
            {
                if (getSalesReportCount.param1 == 0 && !showMore)
                {
                    this.showMessage("No record has been retrieved.");
                    this.flMainSales.Visibility = ViewStates.Visible;
                    this.showOptionMenu(Resource.Id.action_more);
                    this.hideLoader();
                    return;
                }
                else if (getSalesReportCount.param1 == 0 && showMore)
                {
                    this.flSalesReport.Visibility = ViewStates.Visible;
                    this.showOptionMenu(Resource.Id.action_more);
                    this.hideLoader();
                    return;
                }

                getSalesReportList = getSalesReportListA.getSalesReportByVendor(fromDate, toDate, vendor, "my_store.db", this.salesReportItemDisplay.Count);
                if (getSalesReportList.status.Equals("SUCCESS"))
                {
                    foreach (SalesReportItem ssr in getSalesReportList.salesReportList)
                    {
                        this.salesReportItemDisplay.Add(new SalesReportItem()
                        {
                            type = 3
                            , salesDate = ssr.salesDate
                            , noOfSales = ssr.noOfSales
                            , salesTotalAmount = this.formatCurrency(ssr.salesTotalAmount)
                            , salesNoOfItems = ssr.salesNoOfItems
                            , salesVendor = ssr.salesVendor
                        });
                    }

                    if (this.salesReportAdapter == null)
                    {
                        this.salesReportAdapter = new SalesReportAdapter(this, this.salesReportItemDisplay);
                        this.lvSalesReport.Adapter = this.salesReportAdapter;
                    }
                    //Update list of item in ListView Adapter
                    RunOnUiThread(() =>
                    {
                        if (this.salesReportAdapter != null)
                        {
                            this.salesReportAdapter.NotifyDataSetChanged();
                        }
                    });
                    if (!showMore)
                    {
                        this.flSalesReport.Visibility = ViewStates.Visible;
                        this.showOptionMenu(Resource.Id.action_more);
                        this.showOptionMenu(Resource.Id.action_refresh);
                        this.mDrawerLayout.OpenDrawer(mLeftDrawer);
                        this.mDrawerToggle.setCloseDrawerDesc(Resource.String.SalesByVendor);
                    }
                    new System.Threading.Thread(new ThreadStart(delegate
                    {
                        System.Threading.Thread.Sleep(500);
                        RunOnUiThread(() =>
                        {
                            this.mDrawerLayout.CloseDrawer(mLeftDrawer);
                        });
                    })).Start();
                    this.hideLoader();
                }
                else
                {
                    this.flMainSales.Visibility = ViewStates.Visible;
                    this.showOptionMenu(Resource.Id.action_more);
                    this.showMessage(getSalesReportList.message);
                }
            }
            else
            {
                this.flMainSales.Visibility = ViewStates.Visible;
                this.showOptionMenu(Resource.Id.action_more);
                this.showMessage(getSalesReportCount.message);
            }
        }
        public int searchInItemCart(int id)
        {
            for (int i = 0; i < this.itemCartList.Count; i++)
            {
                if (this.itemCartList[i].Id == id)
                {
                    return i;
                }
            }
            return -1;
        }
        public void getSalesForMainContent(bool getItems)
        {
            this.invokeLoader();
            SaleMaster saleMasterModel = new SaleMaster();
            Response getSales = new Response();
            this.currentRetrieveItems = this.aaSalesItemDescriptionList.Count;
            getSales = saleMasterModel.getSalesByDate(salesDate, "my_store.db");
            if (getSales.status.Equals("SUCCESS"))
            {
                if(getItems)
                {
                    this.recurseItemsForSale();
                }
                else
                {
                    this.hideLoader();
                    object sender = new object();
                    EventArgs e = new EventArgs();
                }
                
                if (this.hideSalesFabThread != null)
                    this.hideSalesFabThread.Abort();

                this.salesSummaryListDisplay.Clear();
                this.evaluationType = "";
                this.requestType = "Show Sales";
                this.tvSalesDate.Text = "Sales Date: " + salesDate.ToString("dddd, MMMM dd, yyyy ");
                this.tvNoOfSales.Text = "No. of Sales:" + getSales.param2.ToString();
                this.tvNoOfItemSold.Text = "No. of Items Sold: " + getSales.param1.ToString();
                this.tvTotalSales.Text = "Total Sales: " + this.formatCurrency(getSales.dblParam1.ToString());

                this.mDrawerToggle.setCloseDrawerDesc(Resource.String.Sales);
                this.flMainSales.Visibility = ViewStates.Visible;
                this.setTableItemOptions();
                this.mRightDrawer.Adapter = new RightDrawerAdapter(this, this.tableItemOptions);
                this.showOptionMenu(Resource.Id.action_more);
            }
            else
            {
                this.showMessage(getSales.message);
            }
        }
        public void atcvSalesItemDescription_Clicked(object sender, AdapterView.ItemClickEventArgs e)
        {
            Response getItem = new Response();
            Item itemModel = new Item();
            if (this.hasAction)
                return;

            this.hasAction = true;
            getItem = itemModel.GetByName(this.atcvSalesItemDescription.Text, "my_store.db");
            if(getItem.status.Equals("SUCCESS"))
            {
                //Search if item already exist in cart
                for (int i = 0; i < this.itemCartList.Count; i++)
                {
                    if (this.itemCartList[i].ItemId == getItem.itemList[0].Id)
                    {
                        this.itemPosition = i;
                        break;
                    }
                }
                if (itemPosition == -1)
                {
                    itemPosition = -1;
                    this.itemCartList.Add(new SaleDetail() { Id = 0, SaleMasterId = 0, ItemId = getItem.itemList[0].Id, Quantity = 0, ItemName = getItem.itemList[0].NAME, RetailPrice = getItem.itemList[0].RetailPrice, Barcode = getItem.itemList[0].Barcode });
                }
                this.etSalesRetailPrice.Text = this.formatCurrency(getItem.itemList[0].RetailPrice.ToString());
                this.etSalesBarcode.Text = getItem.itemList[0].Barcode.ToString();
                this.hasAction = false;
                this.hasChoseItem = true;
            }
            else
            {
                this.showMessage("Item(Select): An error has occured.");
            }
        }
        public void etSalesQuantity_ItemChanged(object sender, View.KeyEventArgs e)
        {
            e.Handled = false;
            if (!this.etSalesQuantity.Text.Trim().Equals("") && !this.etSalesQuantity.Text.Trim().Equals(".") && !this.etSalesRetailPrice.Text.Trim().Equals(""))
            {
                this.etSalesTotalPrice.Text = this.formatCurrency((Convert.ToDouble(this.etSalesRetailPrice.Text) * Convert.ToInt16(this.etSalesQuantity.Text)).ToString());
            }
        }
        public void etAmountReceived_ItemChanged(object sender, View.KeyEventArgs e)
        {
            e.Handled = false;
            if (!this.etAmountReceived.Text.Trim().Equals("") && this.salesMasterModel.Amount != 0)
            {
                this.tvChange.Text = string.Format("Change: {0}", this.formatCurrency((Convert.ToDouble(this.etAmountReceived.Text) - this.salesMasterModel.Amount).ToString()));
            }
        }
        public void btnAddEditItemToCart_Clicked(object sender, EventArgs e)
        {
            if (this.itemCartList.Count == 0)
            {
                this.showSnackBar("Please select an item.");
                return;
            }

            if (this.itemCartList[this.itemCartList.Count - 1].ItemId == 0)
            {
                this.showSnackBar("Please select an item.");
            }
            else if(this.etSalesQuantity.Text.Trim().Equals(""))
            {
                this.showSnackBar("Please input quantity.");
            }
            else if (Convert.ToInt16(this.etSalesQuantity.Text.Trim()) <= 0)
            {
                this.showSnackBar("Quantity must be greater than zero(0).");
            }
            else
            {
                if (this.hasAction)
                    return;

                this.hasAction = true;

                if (this.btnEvaluateSaleItem.Text.Equals("Add Item To Cart"))
                {
                    if (this.itemPosition == -1)
                    {
                        this.itemCartList[this.itemCartList.Count - 1].Quantity = Convert.ToInt16(this.etSalesQuantity.Text);
                    }
                    else
                    {
                        this.itemCartList[itemPosition].Quantity += Convert.ToInt16(this.etSalesQuantity.Text);
                    }

                    this.etSalesTotalPrice.Text = this.formatCurrency((Convert.ToDouble(this.etSalesRetailPrice.Text.Trim()) * Convert.ToInt16(this.etSalesQuantity.Text.Trim())).ToString());
                    this.salesMasterModel.Amount += (Convert.ToDouble(this.etSalesTotalPrice.Text.Trim()));
                    this.tvTotalAmount.Text = string.Format("Total Amount: {0}", this.formatCurrency(this.salesMasterModel.Amount.ToString()));
                    this.hideLayouts();
                    this.resetItemCartContent();
                    this.showOptionMenu(Resource.Id.action_more);
                    this.flAddEditSales.Visibility = ViewStates.Visible;
                    this.itemPosition = -1;
                    this.hasChoseItem = false;
                }
                else
                {
                    this.itemCartList[this.itemPosition].Quantity = Convert.ToInt16(this.etSalesQuantity.Text);
                    this.etSalesTotalPrice.Text = this.formatCurrency((Convert.ToDouble(this.etSalesRetailPrice.Text.Trim()) * Convert.ToInt16(this.etSalesQuantity.Text.Trim())).ToString());
                    this.salesMasterModel.Amount = 0;
                    foreach (SaleDetail saleDetail in this.itemCartList)
                    {
                        this.salesMasterModel.Amount = this.salesMasterModel.Amount + (double)(saleDetail.Quantity * saleDetail.RetailPrice);
                    }
                    this.tvTotalAmount.Text = string.Format("Total Amount: {0}", this.formatCurrency(this.salesMasterModel.Amount.ToString()));
                    this.hideLayouts();
                    this.resetItemCartContent();
                    this.showOptionMenu(Resource.Id.action_more);
                    this.flAddEditSales.Visibility = ViewStates.Visible;
                    this.itemPosition = -1;
                    this.hasChoseItem = false;
                }
            }
           
            this.hideKeyBoard();
        }      
        public void btnCancelItemInCart_Clicked(object sender, EventArgs e)
        {
            if (this.hasAction)
                return;

            this.hasAction = true;

            if (this.itemPosition == -1)
            {
                if (this.itemCartList.Count > 0)
                {
                    if (this.hasChoseItem)
                    {
                        this.itemCartList.Remove(this.itemCartList[this.itemCartList.Count - 1]);
                        this.hasChoseItem = false;
                    }
                }
            }
            this.resetItemCartContent();
            this.salesMasterModel.Amount += (Convert.ToDouble(this.etSalesTotalPrice.Text));
            this.tvTotalAmount.Text = string.Format("Total Amount: {0}", this.formatCurrency(this.salesMasterModel.Amount.ToString()));
            this.hideLayouts();
            this.showOptionMenu(Resource.Id.action_more);
            this.flAddEditSales.Visibility = ViewStates.Visible;
            this.hideKeyBoard();
        }
        public void recurseItemsForSale()
        {
            Response getItemCount = new Response();
            Response getItems = new Response();
            Item getItemCountA = new Item();
            Item getItemsA = new Item();

            this.invokeLoader();

            getItemCount = getItemCountA.GetItemCount("my_store.db");
            if (getItemCount.status.Equals("SUCCESS"))
            {

                if (getItemCount.param1 != this.aaSalesItemDescriptionList.Count)
                {
                    getItems = getItemsA.GetItems("SELECT * FROM Item", "my_store.db", this.currentRetrieveItems);
                    if (getItems.status.Equals("SUCCESS"))
                    {
                        foreach (Item itemModel in getItems.itemList)
                        {
                            this.aaSalesItemDescriptionList.Add(itemModel.NAME);
                        }

                        if (aaSalesItemDescription == null)
                        {
                            aaSalesItemDescription = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, this.aaSalesItemDescriptionList);
                            atcvSalesItemDescription.Adapter = aaSalesItemDescription;
                        }
                        //Update list of item in ListView Adapter
                        RunOnUiThread(() =>
                        {
                            if (aaSalesItemDescription != null)
                                aaSalesItemDescription.NotifyDataSetChanged();
                            this.currentRetrieveItems = this.aaSalesItemDescriptionList.Count;
                        });
                        this.recurseItemsForSale();
                    }
                    else
                    {
                        this.showMessage(getItems.message);
                    }  
                }
                else
                {
                    if (this.loaderDialog != null && this.loaderDialog.IsShowing)
                    {
                        this.loaderDialog.Dismiss();
                    }
                    hasAction = false;
                }
            }
            else
            {
                this.showMessage(getItemCount.message);
            }
        }
        public void hideSalesMainPopupMenu_Clicked(object sender, EventArgs e)
        {
            if (this.fabAnimationWatcher != null)
                this.fabAnimationWatcher = null;

            hideSalesFabThread = new System.Threading.Thread(new ThreadStart(delegate
            {
                RunOnUiThread(() =>
                {
                    if (tvIndicator != null)
                    {
                        tvIndicator.Animation = childFab_hide;
                        tvIndicator.StartAnimation(tvIndicator.Animation);
                    }
                    if (pMainSalesFab != null)
                    {
                        pMainSalesFab.Animation = mainFab_hide;
                        pMainSalesFab.StartAnimation(pMainSalesFab.Animation);
                    }
                    if (selectSalesDateFab != null)
                    {
                        selectSalesDateFab.Animation = childFab_hide;
                        selectSalesDateFab.StartAnimation(selectSalesDateFab.Animation);
                    }
                    if (tvSelectSalesDate != null)
                    {
                        tvSelectSalesDate.Animation = childFab_hide;
                        tvSelectSalesDate.StartAnimation(tvSelectSalesDate.Animation);
                    }

                    if (showSalesDateFab != null)
                    {
                        showSalesDateFab.Animation = childFab_hide;
                        showSalesDateFab.StartAnimation(showSalesDateFab.Animation);
                    }
                    if (tvShowSales != null)
                    {
                        tvShowSales.Animation = childFab_hide;
                        tvShowSales.StartAnimation(tvShowSales.Animation);
                    }
                    if (addSalesFab != null)
                    {
                        addSalesFab.Animation = childFab_hide;
                        addSalesFab.StartAnimation(addSalesFab.Animation);
                    }
                    if (tvAddSales != null)
                    {
                        tvAddSales.Animation = childFab_hide;
                        tvAddSales.StartAnimation(tvAddSales.Animation);
                    }
                }); 
                System.Threading.Thread.Sleep(500);
                RunOnUiThread(() =>
                {
                    salesPopupMenuDialog.Dismiss();
                    this.mDrawerLayout.CloseDrawer(mLeftDrawer);
                });
            }));
            hideSalesFabThread.Start();
        }
        public void hideDynamicPopupMenu_Clicked(object sender, EventArgs e)
        {
            if (this.fabAnimationWatcher != null)
                this.fabAnimationWatcher = null;

            this.hideDynamicPopupDialogThread = new System.Threading.Thread(new ThreadStart(delegate
            {
                RunOnUiThread(() =>
                {
                    if (this.tvIndicator != null)
                    {
                        this.tvIndicator.Animation = this.childFab_hide;
                        this.tvIndicator.StartAnimation(this.tvIndicator.Animation);
                    }
                    if (this.dMainFab != null)
                    {
                        this.dMainFab.Animation = this.mainFab_hide;
                        this.dMainFab.StartAnimation(this.dMainFab.Animation);
                    }
                    if (this.tvMainFab != null)
                    {
                        this.tvMainFab.Animation = this.childFab_hide;
                        this.tvMainFab.StartAnimation(this.tvMainFab.Animation);
                    }
                    if (this.dSearchFab != null)
                    {
                        this.dSearchFab.Animation = this.childFab_hide;
                        this.dSearchFab.StartAnimation(this.dSearchFab.Animation);
                        this.etSearchFab.Animation = this.childFab_hide;
                        this.etSearchFab.StartAnimation(this.etSearchFab.Animation);
                    }
                    if (this.dChildFab1 != null)
                    {
                        this.dChildFab1.Animation = this.childFab_hide;
                        this.dChildFab1.StartAnimation(this.dChildFab1.Animation);
                        this.tvChildFab1.Animation = this.childFab_hide;
                        this.tvChildFab1.StartAnimation(this.tvChildFab1.Animation);
                    }
                    if (this.dChildFab2 != null)
                    {
                        this.dChildFab2.Animation = this.childFab_hide;
                        this.dChildFab2.StartAnimation(this.dChildFab2.Animation);
                        this.tvChildFab2.Animation = this.childFab_hide;
                        this.tvChildFab2.StartAnimation(this.tvChildFab2.Animation);
                    }
                    if (this.dChildFab3 != null)
                    {
                        this.dChildFab3.Animation = this.childFab_hide;
                        this.dChildFab3.StartAnimation(this.dChildFab3.Animation);
                        this.tvChildFab3.Animation = this.childFab_hide;
                        this.tvChildFab3.StartAnimation(this.tvChildFab3.Animation);
                    }
                    if (this.dChildFab4 != null)
                    {
                        this.dChildFab4.Animation = this.childFab_hide;
                        this.dChildFab4.StartAnimation(this.dChildFab4.Animation);
                        this.tvChildFab4.Animation = this.childFab_hide;
                        this.tvChildFab4.StartAnimation(this.tvChildFab4.Animation);
                    }
                    if (this.dChildFab5 != null)
                    {
                        this.dChildFab5.Animation = this.childFab_hide;
                        this.dChildFab5.StartAnimation(this.dChildFab5.Animation);
                        this.tvChildFab5.Animation = this.childFab_hide;
                        this.tvChildFab5.StartAnimation(this.tvChildFab5.Animation);
                    }
                });

                System.Threading.Thread.Sleep(500);
                RunOnUiThread(() =>
                {
                    this.dynamicPopupDialog.Dismiss();
                    this.mDrawerLayout.CloseDrawer(mLeftDrawer);
                });
            }));
            this.hideDynamicPopupDialogThread.Start();
        }
        public void hideCartList_Clicked(object sender, EventArgs e)
        {
            if (this.fabAnimationWatcher != null)
                this.fabAnimationWatcher = null;

            this.hideLayouts();
            this.showOptionMenu(Resource.Id.action_more);
            this.flAddEditSales.Visibility = ViewStates.Visible;
            this.mDrawerLayout.OpenDrawer(mLeftDrawer);
            this.mDrawerToggle.setCloseDrawerDesc(Resource.String.Sales);
            this.hideDynamicPopupMenu_Clicked(sender, e);
        }
        public void resetSales()
        {
            this.itemPosition = -1;
            this.salesMasterModel = new SaleMaster();
            this.salesMasterModel.Amount = 0;
            this.itemCartList.Clear();
            this.salesTime = DateTime.Now;
            this.tvSalesTime.RequestFocus();
            this.tvSalesTime.Text = string.Format("Time: {0}", this.salesTime.ToString("hh:mm tt"));
            this.tvTotalAmount.Text = string.Format("Total Amount: {0}", "0.00");
            this.etAmountReceived.Text = "0.00";
            this.tvChange.Text = "Change: 0.00";
            this.etConsumer.Text = "GORGEOUS";
            this.etVendor.Text = "SMART VENDOR";
            this.etConsumerMobileNo.Text = "";
            this.resetSaleSummaryFilter();
            if (this.btnEvaluateSales.Text.Equals("Evaluate") || this.btnEvaluateSales.Text.Equals("Update Sales"))
            {
                this.btnEvaluateSales.Text = "Save Sales";
            }
        }
        public void resetItemCartContent()
        {
            this.atcvSalesItemDescription.RequestFocus();
            this.btnEvaluateSaleItem.Text = "Add Item To Cart";
            this.etSalesBarcode.Text = "";
            this.atcvSalesItemDescription.Enabled = true;
            this.atcvSalesItemDescription.Text = "";
            this.etSalesQuantity.Text = "";
            this.etSalesRetailPrice.Text = "0.00";
            this.etSalesTotalPrice.Text = "0.00";
        }
        public void btnEvaluateSales_Clicked(object sender, EventArgs e)
        {
            if (this.itemCartList.Count == 0)
            {
                this.showSnackBar("Please add item to cart.");
            }
            else if (this.etAmountReceived.Text.Trim().Equals("") || Convert.ToDouble(this.etAmountReceived.Text) <= 0.00)
            {
                this.showSnackBar("Please input amount received.");
            }
            else if (Convert.ToDouble(etAmountReceived.Text.Trim()) < this.salesMasterModel.Amount)
            {
                this.showSnackBar("Amount received should be equal/higher to total amount.");
            }
            else if(this.etConsumer.Text.Trim().Equals(""))
            {
                this.showSnackBar("Consumer is required.");
            }
            else if (this.etVendor.Text.Trim().Equals(""))
            {
                this.showSnackBar("Vendor is required.");
            }
            else
            {
                string smsMessage = "", smsItems = "";
                int count = 1;
                if (this.btnEvaluateSales.Text.Equals("Save Sales"))
                {

                    if (this.hasAction)
                        return;

                    this.hasAction = true;

                    //Save Sales
                    this.invokeLoader();
                    Response saveSales = new Response();
                    int salesItemId = new SaleMaster().getLastId("my_store.db").param1 + 1;
                    sqliteADO = new SQLiteADO();

                    command = string.Format("INSERT INTO SaleMaster(SalesDateTime, Amount, AmountReceived, SoldBy, SoldTo, SoldToMobileNo) VALUES('{0}', {1}, {2}, '{3}', '{4}', '{5}');"
                        , string.Format("{0} {1}", this.salesDate.ToString("yyyy-MM-dd"), this.salesTime.ToString("HH:mm:ss"))
                        , Convert.ToDouble(this.salesMasterModel.Amount)
                        , Convert.ToDouble(this.etAmountReceived.Text.Trim())
                        , this.etVendor.Text.ToString()
                        , this.etConsumer.Text.ToString()
                        , this.etConsumerMobileNo.Text.Trim()) + "\n";
                    foreach (SaleDetail saleDetails in this.itemCartList)
                    {
                        command += string.Format("INSERT INTO SaleDetail(SaleMasterId, ItemId, Quantity) VALUES({0}, {1}, {2});"
                            , salesItemId
                            , saleDetails.ItemId
                            , saleDetails.Quantity) + "\n";
                        foreach (Item itemModel in new Item().GetById(saleDetails.ItemId, "my_store.db").itemList)
                        {
                            command += string.Format("UPDATE Item SET AvailableStock = {0} WHERE Id = {1};"
                            , itemModel.AvailableStock - saleDetails.Quantity
                            , itemModel.Id) + "\n";
                        }
                        smsItems += string.Format("{0} {1}({2}) COST {3}{4}"
                            , saleDetails.Quantity
                            , saleDetails.ItemName
                            , this.formatCurrency(saleDetails.RetailPrice.ToString())
                            , this.formatCurrency((saleDetails.RetailPrice * saleDetails.Quantity).ToString())
                            , count == this.itemCartList.Count ? "":",") + "\n";
                        count = count + 1;
                    }
                    saveSales = sqliteADO.ExecuteNonQuery("BEGIN TRANSACTION;" + "\n" + command + "COMMIT;", dbPath);
                    if (saveSales.status.Equals("SUCCESS"))
                    {
                        if (!this.etConsumerMobileNo.Text.Trim().Equals(""))
                        {
                            smsMessage = string.Format("Hi {0},", this.etConsumer.Text.Trim()) + "\n \n";
                            smsMessage += string.Format("Purchased Date: {0}", string.Format("{0} {1}", this.salesDate.ToString("dddd, MM dd,yyyy"), this.salesTime.ToString("hh:mm:ss tt"))) +"\n";
                            smsMessage += string.Format("Total Amount: {0}", this.formatCurrency(this.salesMasterModel.Amount.ToString())) + "\n";
                            smsMessage += string.Format("Amount Received: {0}", this.formatCurrency(this.etAmountReceived.Text.Trim().ToString())) + "\n";
                            smsMessage += string.Format("Change: {0}", this.formatCurrency((Convert.ToDouble(this.etAmountReceived.Text.Trim()) - this.salesMasterModel.Amount).ToString())) + "\n";
                            smsMessage += string.Format("Vendor: {0}", this.etVendor.Text.Trim()) + "\n \n";
                            smsMessage += string.Format("Items Purchased:") + "\n";
                            smsMessage += smsItems + "\n My Store";
                            this.sendSMS(this.etConsumerMobileNo.Text.Trim(), smsMessage);
                        }
                        this.resetSales();
                        this.hideLoader();
                        this.showSnackBarInfinite("Successfully Saved.");
                    }
                    else
                    {
                        this.showMessage("Sale(Insert):" + saveSales.message);
                    }
                }
                else
                {
                    command = "";
                    int index = 0;
                    //Check if current sales found in item cart list
                    foreach (SalesSummary saleDetailModel in this.currentSales)
                    {
                        index = this.searchInItemCart(saleDetailModel.SaleDetailId);
                        if (index != -1) //if found
                        {
                            //Update Sale Detail
                            command += string.Format("UPDATE SaleDetail SET Quantity = {0} WHERE Id = {1};"
                                , saleDetailModel.SaleDetailId
                                , this.itemCartList[index].Quantity) + "\n";
                            //Update Item
                            command += string.Format("UPDATE Item SET AvailableStock = {0} WHERE Id = {1};"
                                , saleDetailModel.Quantity < this.itemCartList[index].Quantity
                                    ? saleDetailModel.AvailableStock - (this.itemCartList[index].Quantity - saleDetailModel.Quantity)
                                    : saleDetailModel.AvailableStock + (saleDetailModel.Quantity - this.itemCartList[index].Quantity)
                                , saleDetailModel.ItemId) + "\n";
                        }
                        else //if not found
                        {
                            //Delete sale detail
                            command += string.Format("DELETE FROM SaleDetail WHERE Id = {0};", saleDetailModel.SaleDetailId) + "\n";
                            //Update Item
                            command += string.Format("UPDATE Item SET AvailableStock = {0} WHERE Id = {1};"
                                , saleDetailModel.AvailableStock + saleDetailModel.Quantity
                                , saleDetailModel.ItemId) + "\n";
                        }
                    }

                    index = 0;
                    //Add sales
                    foreach (SaleDetail saleDetailModel1 in this.itemCartList)
                    {
                        if (saleDetailModel1.Id == 0)
                        {
                            //Insert Sale Detail
                            command += string.Format("INSERT INTO SaleDetail(SaleMasterId, ItemId, Quantity) VALUES({0}, {1}, {2});"
                                  , this.currentSales[0].SaleMasterId
                                  , saleDetailModel1.ItemId
                                  , saleDetailModel1.Quantity) + "\n";
                            //Update Item
                            command += string.Format("UPDATE Item SET AvailableStock = {0} WHERE Id = {1};"
                                , this.currentSales[index].AvailableStock - saleDetailModel1.Quantity
                                , saleDetailModel1.ItemId) + "\n";
                        }
                        index = index + 1;
                    }

                    //Update Sale Master
                    command += string.Format("UPDATE SaleMaster SET Amount = {0}, AmountReceived = {1}, SoldBy = '{2}', SoldTo = '{3}', SoldToMobileNo = '{4}'  WHERE Id = {5};"
                        , Convert.ToDouble(this.salesMasterModel.Amount)
                        , Convert.ToDouble(this.etAmountReceived.Text.Trim())
                        , this.etVendor.Text.ToString()
                        , this.etConsumer.Text.ToString()
                        , this.etConsumerMobileNo.Text.Trim()
                        , this.salesMasterModel.Id) + "\n";

                    foreach (SaleDetail saleDetails in this.itemCartList)
                    {
                        smsItems += string.Format("{0} {1}({2}) COST {3}{4}"
                            , saleDetails.Quantity
                            , saleDetails.ItemName
                            , this.formatCurrency(saleDetails.RetailPrice.ToString())
                            , this.formatCurrency((saleDetails.RetailPrice * saleDetails.Quantity).ToString())
                            , count == this.itemCartList.Count ? "":",") + "\n";
                        count = count + 1;
                    }

                    this.sqliteADO = new SQLiteADO();
                    Response response = new Response();
                    response = sqliteADO.ExecuteNonQuery("BEGIN TRANSACTION;" + "\n" + command + "COMMIT;", dbPath);
                    if (response.status.Equals("SUCCESS"))
                    {
                        if (!this.etConsumerMobileNo.Text.Trim().Equals(""))
                        {
                            smsMessage = string.Format("Hi {0},", this.etConsumer.Text.Trim()) + "\n \n";
                            smsMessage += string.Format("Purchased Date: {0}", string.Format("{0} {1}", this.salesDate.ToString("dddd, MM dd,yyyy"), this.salesTime.ToString("hh:mm:ss tt"))) + "\n";
                            smsMessage += string.Format("Total Amount: {0}", this.formatCurrency(this.salesMasterModel.Amount.ToString())) + "\n";
                            smsMessage += string.Format("Amount Received: {0}", this.formatCurrency(this.salesMasterModel.AmountReceived.ToString())) + "\n";
                            smsMessage += string.Format("Change: {0}", this.formatCurrency((this.salesMasterModel.AmountReceived - this.salesMasterModel.Amount).ToString())) + "\n";
                            smsMessage += string.Format("Vendor: {0}", this.etVendor.Text.Trim()) + "\n \n";
                            smsMessage += string.Format("Items Purchased:") + "\n";
                            smsMessage += smsItems + "\n My Store";
                            this.sendSMS(this.etConsumerMobileNo.Text.Trim(), smsMessage);
                        }
                        this.hideLayouts();
                        this.getSalesForMainContent(false);
                        this.showSnackBarInfinite("Successfully updated.");
                        this.hideLoader();
                    }
                    else
                    {
                        this.showMessage("Sales(Update):" + response.message);
                    }
                }
            }
            this.hasChoseItem = false;
            this.hideKeyBoard();
        }
        public void btnClearSales_Clicked(object sender, EventArgs e)
        {
            this.resetSales();
            this.hideKeyBoard();
        }
        public void resetSaleSummaryFilter()
        {

            this.sslfFromTime = Convert.ToDateTime(this.salesDate.ToString("yyyy-MM-dd") + " 00:01:00");
            this.sslfToTime = Convert.ToDateTime(this.salesDate.ToString("yyyy-MM-dd") + " 23:59:59");
            this.sslfVendor = "";
            this.sslfConsumer = "";
        }
        public void setFilterFromTime(object sender, EventArgs e)
        {
            this.hideKeyBoard();
            DateTime holder = DateTime.Now;
            TimePickerFragment frag = TimePickerFragment.NewInstance(delegate(DateTime date)
            {
                    holder = Convert.ToDateTime(this.sslfFromTime.ToString("yyyy-MM-dd") + " " + date.ToString("HH:mm") + ":00");
                    if (date > this.sslfToTime)
                    {
                        this.showMessage("From value should not be beyond To value.");
                    }
                    else
                    {
                        this.sslfFromTime = holder;
                        filterView.FindViewById<EditText>(Resource.Id.etFilterFromTime).Text = this.sslfFromTime.ToString("hh:mm:ss tt");
                    }

                    filterView.FindViewById<EditText>(Resource.Id.etFilterVendor).RequestFocus();
            }, delegate(DateTime date)
            {
               //Do nothing

                filterView.FindViewById<EditText>(Resource.Id.etFilterVendor).RequestFocus();
            });
            if (this.sslfFromTime.Hour == 0)
            {
                frag.currentDateTime = Convert.ToDateTime(this.salesDate.ToString("yyyy-MM-dd") + " 00:01:00");
            }
            else
            {
                frag.currentDateTime = Convert.ToDateTime(this.salesDate.ToString("yyyy-MM-dd") + " " + this.sslfFromTime.ToString("HH:mm:ss"));
            }
            frag.Show(FragmentManager, TimePickerFragment.TAG);
        }
        public void setFilterToTime(object sender, EventArgs e)
        {
            this.hideKeyBoard();
            int count = 0;
            DateTime holder = DateTime.Now;
            TimePickerFragment frag = TimePickerFragment.NewInstance(delegate(DateTime date)
            {
                //If user select date
                count = count + 1;
                if (count == 1)
                {
                    holder = Convert.ToDateTime(this.sslfToTime.ToString("yyyy-MM-dd") + " " + date.ToString("HH:mm") + ":59");
                    if (date < this.sslfFromTime)
                    {
                        this.showMessage("To value should be beyond From value.");
                    }
                    else
                    {
                        this.sslfToTime = holder;
                        filterView.FindViewById<EditText>(Resource.Id.etFilterToTime).Text = this.sslfToTime.ToString("hh:mm:ss tt");
                    }
                    filterView.FindViewById<EditText>(Resource.Id.etFilterVendor).RequestFocus();
                }
            }, delegate(DateTime date)
            {
                //Do nothing
                filterView.FindViewById<EditText>(Resource.Id.etFilterVendor).RequestFocus();
            });
            if (this.sslfToTime.Hour == 0)
            {
                frag.currentDateTime = Convert.ToDateTime(this.salesDate.ToString("yyyy-MM-dd") + " 23:59:59");
            }
            else
            {
                frag.currentDateTime = Convert.ToDateTime(this.salesDate.ToString("yyyy-MM-dd") + " " + this.sslfToTime.ToString("HH:mm:ss"));
            }
            frag.Show(FragmentManager, TimePickerFragment.TAG);
        }
        public void setReportFrom()
        {
            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate(DateTime date)
            {
                //If user select date
                this.srfFrom = date;
                this.filterView.FindViewById<EditText>(Resource.Id.etReportFrom).Text = this.srfFrom.ToString("dddd, MMMM dd, yyyy");
                this.filterView.FindViewById<EditText>(Resource.Id.etReportFocus).RequestFocus();
            }, delegate(DateTime date)
            {
                //if user cancel the dialog
                this.filterView.FindViewById<EditText>(Resource.Id.etReportFrom).Text = this.srfFrom.ToString("dddd, MMMM dd, yyyy");
                this.filterView.FindViewById<EditText>(Resource.Id.etReportFocus).RequestFocus();
            });
            if (this.salesReportItemDisplay.Count == 0)
            {
                frag.currentDateTime = DateTime.Now;
            }
            else
            {
                frag.currentDateTime = this.srfFrom;
            }
            frag.Show(FragmentManager, DatePickerFragment.TAG);
        }
        public void setReportTo()
        {
            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate(DateTime date)
            {
                //If user select date
                this.srfTo = date;
                this.filterView.FindViewById<EditText>(Resource.Id.etReportTo).Text = this.srfTo.ToString("dddd, MMMM dd, yyyy");
                this.filterView.FindViewById<EditText>(Resource.Id.etReportFocus).RequestFocus();
            }, delegate(DateTime date)
            {
                //if user cancel the dialog
                this.filterView.FindViewById<EditText>(Resource.Id.etReportTo).Text = this.srfTo.ToString("dddd, MMMM dd, yyyy");
                this.filterView.FindViewById<EditText>(Resource.Id.etReportFocus).RequestFocus();
            });
            if (this.salesReportItemDisplay.Count == 0)
            {
                frag.currentDateTime = DateTime.Now;
            }
            else
            {
                frag.currentDateTime = this.srfFrom;
            }
            frag.Show(FragmentManager, DatePickerFragment.TAG);
        }
        public void focusListener(object sender, EventArgs e)
        {
            if (filterView.FindViewById<EditText>(Resource.Id.etFilterFromTime).HasFocus)
            {
                this.setFilterFromTime(sender, e);
            }
            else
            {
                if (filterView.FindViewById<EditText>(Resource.Id.etFilterToTime).HasFocus)
                {
                    this.setFilterToTime(sender, e);
                }
            }
        }
        public void focusReportListener(object sender, EventArgs e)
        {
            if (filterView.FindViewById<EditText>(Resource.Id.etReportFrom).HasFocus)
            {
                this.setReportFrom();
            }
            else
            {
                if (filterView.FindViewById<EditText>(Resource.Id.etReportTo).HasFocus)
                {
                    this.setReportTo();
                }
            }
        }
        /*----------------------------------Main Sale FABS-----------------------------------------*/
        public void addSalesFab_Clicked(object sender, EventArgs e)
        {
            if (this.hasAction)
                return;

            this.hasAction = true;
            this.resetSales();
            this.hideLayouts();
            this.flAddEditSales.Visibility = ViewStates.Visible;
            this.setTableItemOptions();
            this.mRightDrawer.Adapter = new RightDrawerAdapter(this, this.tableItemOptions);
            this.showOptionMenu(Resource.Id.action_more);
            this.hideSalesMainPopupMenu_Clicked(sender, e);
        }
        public void showSalesByDateFab_Clicked(object sender, EventArgs e)
        {
            if (this.hasAction)
                return;

            int count = 0;

            Response getSalesSummary = new Response();
            SalesSummary salesSummaryModel = new SalesSummary();

            count = count + 1;
            if (count > 1)
                return;

            this.invokeLoader();
            this.salesSummaryListDisplay.Clear();
            this.resetSaleSummaryFilter();
            getSalesSummary = salesSummaryModel.getSalesByDateTime(this.salesDate
                , this.sslfFromTime
                , this.sslfToTime
                , this.sslfConsumer
                , this.sslfVendor
                , "my_store.db"
                , this.salesSummaryListDisplay.Count);
            if (getSalesSummary.status.Equals("SUCCESS"))
            {
                if (getSalesSummary.salesSummary.Count == 0)
                {
                    this.showMessage(string.Format("No sales for {0}.", this.salesDate.ToString("MM/dd/yyyy")));
                    return;
                }
                foreach(SalesSummary ssm in getSalesSummary.salesSummary)
                {
                    this.salesSummaryListDisplay.Add(new ListItem()
                    {
                        Id = ssm.SaleMasterId
                        , IsChecked = false
                        , Description = string.Format("{0} - {1} {2} sold for {3}"
                                                , ssm.SalesDateTime.ToString("hh:mm:ss tt")
                                                , ssm.ItemsSold
                                                , ssm.ItemsSold > 1 ? "Items" : "Item"
                                                , this.formatCurrency(ssm.Amount.ToString()))
                    });
                }
                if (this.salesSummaryListAdapter == null)
                {
                    this.salesSummaryListAdapter = new ListItemAdapter(this, this.salesSummaryListDisplay);
                    this.lvSalesSummary.Adapter = this.salesSummaryListAdapter;
                }
                RunOnUiThread(() =>
                {
                    this.salesSummaryListAdapter.NotifyDataSetChanged();
                });
                this.hideLoader();
                this.hideLayouts();
                this.flSalesSummary.Visibility = ViewStates.Visible;
                this.showOptionMenu(Resource.Id.action_more);
                this.showOptionMenu(Resource.Id.action_refresh);
                this.mDrawerLayout.OpenDrawer(mLeftDrawer);
                this.mDrawerToggle.setCloseDrawerDesc(Resource.String.SalesList);
                this.hideSalesMainPopupMenu_Clicked(sender, e);
                count = 0;
            }
            else
            {
                this.hideLoader();
                this.showMessage(getSalesSummary.message);
                count = 0;
            }
        }
        public void selectSalesDateFab_Clicked(object sender, EventArgs e)
        {
            if (this.hasAction)
                return;
            this.hasAction = true;
            int count = 0;
            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate(DateTime date)
            {
                //If user select date
                count = count + 1;
                if (count == 1)
                {
                    this.fromDate = date;
                    if (fromDate <= DateTime.Now)
                    {
                        this.salesDate = this.fromDate;
                        this.getSalesForMainContent(false);
                        this.resetSaleSummaryFilter();
                        this.hideSalesMainPopupMenu_Clicked(sender, e);
                    }
                    else
                    {
                        this.showMessage("Please select date not beyond today.");
                        this.hasAction = false;
                    }
                    this.salesSummaryListDisplay.Clear();
                }
            }, delegate(DateTime date) {
                //if user cancel the dialog
                this.salesDate = DateTime.Now;
                this.hasAction = false;
                this.resetSaleSummaryFilter();
                this.hideSalesMainPopupMenu_Clicked(sender, e);
            });
            if (fromDate.Year == 1)
            {
                frag.currentDateTime = DateTime.Now;
            }
            else
            {
                frag.currentDateTime = fromDate; 
            }
            frag.Show(FragmentManager, DatePickerFragment.TAG);
        }
        /*----------------------------------Add/Edit Sale FABS-------------------------------------*/
        public async void scanSalesItemFab_Clicked(object sender, EventArgs e)
        {
            if (this.hasAction)
                return;

            Item itemModel = new Item();
            Response response = new Response();
            #if __ANDROID__
            // Initialize the scanner first so it can track the current context
            MobileBarcodeScanner.Initialize(Application);
            #endif
            var scanner = new ZXing.Mobile.MobileBarcodeScanner();
            scanner.AutoFocus();
            this.resetItemCartContent();
            // Create your application here
            var result = await scanner.Scan();
            if (result != null)
            {
                this.hasAction = true;
                response = itemModel.GetByBarcode(result.ToString(), "my_store.db");
                if (response.param1 == 0)
                {
                    this.showMessage("Item doesn't exist, try to add item in inventory.");
                    this.hasAction = false;
                    this.resetSales();
                    this.hideLayouts();
                    this.setTableItemOptions();
                    this.mRightDrawer.Adapter = new RightDrawerAdapter(this, this.tableItemOptions);
                    this.showOptionMenu(Resource.Id.action_more);
                    this.flAddEditSales.Visibility = ViewStates.Visible;
                }
                else
                {
                    bool found = false;
                    this.resetItemCartContent();

                    for (int i = 0; i < this.itemCartList.Count; i++)
                    {
                        if (result.ToString().Trim().Equals(this.itemCartList[i].Barcode.Trim()))
                        {
                            this.hasAction = true;
                            this.itemPosition = i;
                            this.atcvSalesItemDescription.Text = this.itemCartList[i].ItemName;
                            this.etSalesBarcode.Text = this.itemCartList[i].Barcode.ToString();
                            this.etSalesQuantity.Text = this.itemCartList[i].Quantity.ToString();
                            this.etSalesRetailPrice.Text = this.formatCurrency(this.itemCartList[i].RetailPrice.ToString());
                            this.etSalesTotalPrice.Text = this.formatCurrency((this.itemCartList[i].RetailPrice * this.itemCartList[i].Quantity).ToString());
                            this.btnEvaluateSaleItem.Text = "Edit Item In Cart"; 
                            i = this.itemCartList.Count;
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        this.hasAction = true;
                        itemPosition = -1;
                        this.itemCartList.Add(new SaleDetail() { Id = 0
                            , SaleMasterId = 0
                            , ItemId = response.itemList[0].Id
                            , Quantity = 0
                            , ItemName = response.itemList[0].NAME
                            , RetailPrice = response.itemList[0].RetailPrice
                            , Barcode = response.itemList[0].Barcode });

                        this.hasChoseItem = true;
                        this.atcvSalesItemDescription.Text = response.itemList[0].NAME.ToString();
                        this.etSalesRetailPrice.Text = this.formatCurrency(response.itemList[0].RetailPrice.ToString());
                        this.etSalesBarcode.Text = response.itemList[0].Barcode.ToString();
                        this.hasChoseItem = true;
                    }
                    this.hideLayouts();
                    this.showOptionMenu(Resource.Id.action_more);
                    this.svAddEditCart.Visibility = ViewStates.Visible;
                    this.atcvSalesItemDescription.Enabled = false;
                    this.etSalesQuantity.RequestFocus();
                    //this.hideDynamicPopupMenu_Clicked(sender, e);
                }
            }
            else
            {
                this.hideLayouts();
                this.flAddEditSales.Visibility = ViewStates.Visible;
                this.showOptionMenu(Resource.Id.action_more);
            }
        }
        public void addItemToCartFab_Clicked(object sender, EventArgs e)
        {
            if (this.hasAction)
                return;

            this.hasAction = true;
            this.itemPosition = -1;
            this.hideLayouts();
            this.showOptionMenu(Resource.Id.action_more);
            this.hasChoseItem = false;
            this.resetItemCartContent();
            this.svAddEditCart.Visibility = ViewStates.Visible;
            //this.hideDynamicPopupMenu_Clicked(sender, e);
        }
        public void showCartFab_Clicked(object sender, EventArgs e)
        {
            
                if (this.hasAction)
                    return;

                this.hasAction = true;

                this.cartItemsDisplay.Clear();

                foreach (SaleDetail saleDetail in this.itemCartList)
                {
                    this.cartItemsDisplay.Add(new ListItem()
                    {
                        Id = saleDetail.ItemId,
                        IsChecked = false,
                        Description = string.Format("{0}({1} {2})({3})"
                        , saleDetail.ItemName
                        , saleDetail.Quantity
                        , saleDetail.Quantity > 1 ? "PCS" : "PC"
                        , this.formatCurrency((saleDetail.Quantity * saleDetail.RetailPrice).ToString())) });
                }
                this.cartListAdapter = new ListItemAdapter(this, this.cartItemsDisplay);
                this.lvCartList.Adapter = cartListAdapter;
                this.hideLayouts();
                this.flCartList.Visibility = ViewStates.Visible;
                this.showOptionMenu(Resource.Id.action_more);
                this.mDrawerLayout.OpenDrawer(mLeftDrawer);
                this.mDrawerToggle.setCloseDrawerDesc(Resource.String.CartList);
                new System.Threading.Thread(new ThreadStart(delegate
                {
                    System.Threading.Thread.Sleep(500);
                    RunOnUiThread(() =>
                    {
                        this.mDrawerLayout.CloseDrawer(mLeftDrawer);
                    });
                })).Start();
        }
        /*----------------------------------Sale Summary FABS-------------------------------------*/
        public void findSaleInSummaryFab_Clicked(object sender, EventArgs e)
        {
            if (this.hasAction)
                return;

            this.hasAction = true;
            this.isAllowedToCloseDialog = false;

            AlertDialog.Builder dialogBuilder = new AlertDialog.Builder(this);
            filterView = LayoutInflater.Inflate(Resource.Layout.SaleSummaryFilterLayout, null);

            filterView.FindViewById<EditText>(Resource.Id.etFilterFromTime).Text = this.sslfFromTime.ToString("hh:mm:ss tt");
            filterView.FindViewById<EditText>(Resource.Id.etFilterToTime).Text = this.sslfToTime.ToString("hh:mm:ss tt");
            filterView.FindViewById<EditText>(Resource.Id.etFilterConsumer).Text = this.sslfConsumer;
            filterView.FindViewById<EditText>(Resource.Id.etFilterVendor).Text = this.sslfVendor;
            filterView.FindViewById<EditText>(Resource.Id.etFilterVendor).RequestFocus();

            filterView.FindViewById<EditText>(Resource.Id.etFilterFromTime).Enabled = true;
            filterView.FindViewById<EditText>(Resource.Id.etFilterToTime).Enabled = true;
            filterView.FindViewById<EditText>(Resource.Id.etFilterFromTime).FocusChange += focusListener;
            filterView.FindViewById<EditText>(Resource.Id.etFilterToTime).FocusChange += focusListener;

            string nbLabel = "";
            if( this.sslfFromTime == Convert.ToDateTime(this.salesDate.ToString("yyyy-MM-dd") + " 00:01:00")
                && this.sslfToTime == Convert.ToDateTime(this.salesDate.ToString("yyyy-MM-dd") + " 23:59:59")
                && this.sslfVendor.Equals("")
                && this.sslfConsumer.Equals(""))
            {
                nbLabel = "Cancel";
            }
            else
            {
                nbLabel = "Clear";
            }
            dialogBuilder.SetCancelable(false);
            dialogBuilder.SetView(filterView);
            dialogBuilder.SetNegativeButton(nbLabel, delegate
            {

                this.isAllowedToCloseDialog = true;
                this.hasAction = false;
                if (nbLabel.Equals("Clear"))
                {
                    this.resetSaleSummaryFilter();
                    this.showSalesByDateFab_Clicked(new object(), new EventArgs());
                }
                this.hideDynamicPopupMenu_Clicked(sender, e);
            });
            dialogBuilder.SetPositiveButton("Find", delegate
            {
                this.isAllowedToCloseDialog = true;
                this.hasAction = false;
                this.salesSummaryListDisplay.Clear();
                this.sslfConsumer = filterView.FindViewById<EditText>(Resource.Id.etFilterConsumer).Text;
                this.sslfVendor = filterView.FindViewById<EditText>(Resource.Id.etFilterVendor).Text;
                this.showMoreSummary_Clicked(sender, e);
            });
            dialogBuilder.Create().Show();
        }
        public void unCheckAllSaleInSummaryFab_Clicked(object sender, EventArgs e)
        {
            int count = 0;
            if (this.salesSummaryListDisplay.Count > 0)
            {
                if (this.hasAction)
                    return;

                this.hasAction = true;

                RunOnUiThread(() =>
                {
                    this.tvIndicator.Visibility = ViewStates.Visible;
                    this.isAllowedToCloseDialog = false;
                    for (int i = 0; i < this.salesSummaryListDisplay.Count; i++)
                    {
                        if (this.salesSummaryListDisplay[i].IsChecked)
                        {
                            this.tvIndicator.Text = string.Format("Unchecking record {0} of {1}", i + 1, this.salesSummaryListDisplay.Count);
                            this.salesSummaryListDisplay[i].IsChecked = false;
                            count = count + 1;
                        }
                        if (i == this.salesSummaryListDisplay.Count - 1)
                        {
                            if (count != 0)
                            {
                                this.tvIndicator.Text = count.ToString() + " record/s have been unchecked.";
                            }
                            else
                            {
                                this.tvIndicator.Text = "All record/s were already unchecked.";
                            }
                            this.isAllowedToCloseDialog = true;
                            if (this.salesSummaryListAdapter != null)
                                this.salesSummaryListAdapter.NotifyDataSetChanged();
                        }
                    }
                    this.hasAction = false;
                });
            }
            else
            {
                tvIndicator.Text = "Sale list were all deleted.";
            }
        }
        public void checkAllSaleInSummaryFab_Clicked(object sender, EventArgs e)
        {
            int count = 0;
            if (this.salesSummaryListDisplay.Count > 0)
            {
                if (this.hasAction)
                    return;

                this.hasAction = true;

                RunOnUiThread(() =>
                {
                    this.tvIndicator.Visibility = ViewStates.Visible;
                    this.isAllowedToCloseDialog = false;
                    for (int i = 0; i < this.salesSummaryListDisplay.Count; i++)
                    {
                        if (!this.salesSummaryListDisplay[i].IsChecked)
                        {
                            this.tvIndicator.Text = string.Format("Unchecking record {0} of {1}", i + 1, this.salesSummaryListDisplay.Count);
                            this.salesSummaryListDisplay[i].IsChecked = true;
                            count = count + 1;
                        }
                        if (i == this.salesSummaryListDisplay.Count - 1)
                        {
                            if (count != 0)
                            {
                                this.tvIndicator.Text = count.ToString() + " record/s have been checked.";
                            }
                            else
                            {
                                this.tvIndicator.Text = "All record/s were already checked.";
                            }
                            this.isAllowedToCloseDialog = true;
                            if (this.salesSummaryListAdapter != null)
                                this.salesSummaryListAdapter.NotifyDataSetChanged();
                        }
                    }
                    this.hasAction = false;
                });
            }
            else
            {
                tvIndicator.Text = "Sale list were all deleted.";
            }
        }
        public void deleteSelectedSaleInSummaryFab_Clicked(object sender, EventArgs e)
        {
            if (this.hasAction)
                return;
            
            if (this.salesSummaryListAdapter.getSelectedItems().Count == 0)
            {
                tvIndicator.Text = "No sale selected.";
                return;
            }
            int count = 0;
            List<ListItem> saleSummaryListCopy = new List<ListItem>();
            Response getSaleDetail = new Response();
            SalesSummary saleDetail = new SalesSummary();
            Response deleteSale = new Response();

            this.hasAction = true;

            var builder = new AlertDialog.Builder(this);
            builder.SetTitle("Application Message");
            builder.SetMessage("Are you sure you want to delete selected sale/s?");
            builder.SetPositiveButton("Yes", delegate
            {
                this.invokeLoader();
                tvIndicator.Visibility = ViewStates.Visible;
                isAllowedToCloseDialog = false;
                saleSummaryListCopy = this.salesSummaryListDisplay;
                command = "";
                foreach (ListItem salesSummaryModel in this.salesSummaryListAdapter.getSelectedItems())
                {
                    count = count + 1;
                    command += string.Format("DELETE FROM SaleMaster WHERE Id = {0};", salesSummaryModel.Id) + "\n";
                    getSaleDetail = saleDetail.getSaleById(salesSummaryModel.Id, "my_store.db");
                    if (getSaleDetail.status.Equals("SUCCESS"))
                    {
                        foreach (SalesSummary saleSummary in getSaleDetail.salesSummary)
                        {
                            command += string.Format("DELETE FROM SaleDetail WHERE SaleMasterId = {0};", saleSummary.SaleMasterId) + "\n";
                            command += string.Format("UPDATE Item SET AvailableStock = {0} WHERE Id = {1};"
                                , saleSummary.AvailableStock + saleSummary.Quantity
                                , saleSummary.ItemId) + "\n";
                        }
                    }
                    else
                    {
                        this.isAllowedToCloseDialog = true;
                        this.salesSummaryListDisplay = saleSummaryListCopy;
                        this.tvIndicator.Text = "SaleDetail(Delete): " + getSaleDetail.message;
                        break;
                    }

                    this.salesSummaryListDisplay.Remove(salesSummaryModel);
                }
                sqliteADO = new SQLiteADO();

                deleteSale = sqliteADO.ExecuteNonQuery("BEGIN TRANSACTION;" + "\n" + command + "COMMIT;", dbPath);
                if (deleteSale.status.Equals("SUCCESS"))
                {
                    this.hideLoader();
                    this.hasAction = false;
                    this.tvIndicator.Text = count.ToString() + " sale/s have been deleted.";
                }
                else
                {
                    this.tvIndicator.Text = "Sale(Delete): " + deleteSale.message;
                    this.isAllowedToCloseDialog = true;
                }

                RunOnUiThread(() =>
                {
                    if (salesSummaryListAdapter != null)
                        salesSummaryListAdapter.NotifyDataSetChanged();
                });
            });
            builder.SetNegativeButton("No", delegate { this.tvIndicator.Text = "Tap blank space to go back in the list."; this.hasAction = false; });
            builder.Create().Show();

        }
        public void editSelectedSaleInSummaryFab_Clicked(object sender, EventArgs e)
        {
            if (this.hasAction)
                return;

            if (this.salesSummaryListAdapter.getSelectedItems().Count == 0)
            {
                tvIndicator.Text = "No sale selected.";
                return;
            }

            this.hasAction = true;
            this.invokeLoader();
            Response getSaleDetail = new Response();
            SalesSummary saleDetail = new SalesSummary();

            getSaleDetail = saleDetail.getSaleById(this.salesSummaryListAdapter.getSelectedItems()[0].Id, "my_store.db");
            if (getSaleDetail.status.Equals("SUCCESS"))
            {
                this.itemPosition = -1;
                this.itemCartList.Clear();
                this.tvSalesTime.RequestFocus();
                this.salesMasterModel = new SaleMaster();
                this.salesMasterModel.Id = getSaleDetail.salesSummary[0].SaleMasterId;
                this.salesMasterModel.Amount = getSaleDetail.salesSummary[0].Amount;
                this.salesMasterModel.SalesDateTime = getSaleDetail.salesSummary[0].SalesDateTime;
                this.salesMasterModel.AmountReceived = getSaleDetail.salesSummary[0].AmountReceived;
                this.salesMasterModel.SoldBy = getSaleDetail.salesSummary[0].SoldBy;
                this.salesMasterModel.SoldTo = getSaleDetail.salesSummary[0].SoldTo;
                this.salesMasterModel.SoldToMobileNo = getSaleDetail.salesSummary[0].SoldToMobileNo;
                this.salesTime = getSaleDetail.salesSummary[0].SalesDateTime;
                this.tvSalesTime.Text = string.Format("Time: {0}", this.salesTime.ToString("hh:mm tt"));
                this.tvTotalAmount.Text = string.Format("Total Amount: {0}", this.formatCurrency(this.salesMasterModel.Amount.ToString()));
                this.etAmountReceived.Text = this.formatCurrency(this.salesMasterModel.AmountReceived.ToString());
                this.tvChange.Text = string.Format("Change: {0}", this.formatCurrency((this.salesMasterModel.AmountReceived - this.salesMasterModel.Amount).ToString()));
                this.etConsumer.Text = this.salesMasterModel.SoldTo.ToString();
                this.etVendor.Text = this.salesMasterModel.SoldBy.ToString();
                this.etConsumerMobileNo.Text = this.salesMasterModel.SoldToMobileNo.ToString();
                currentSales.Clear();
                
                foreach (SalesSummary saleSummary in getSaleDetail.salesSummary)
                {
                    this.itemCartList.Add(new SaleDetail()
                    {
                        Id = saleSummary.SaleDetailId
                        , SaleMasterId = saleSummary.SaleMasterId
                        , ItemId = saleSummary.ItemId
                        , Quantity = saleSummary.Quantity
                        , ItemName = saleSummary.ItemName
                        , Barcode = saleSummary.Barcode
                        , RetailPrice = saleSummary.RetailPrice
                        , SalesDateTime = saleSummary.SalesDateTime.ToString("dddd, MMMM-dd-yyyy")
                    });

                    this.currentSales.Add(saleSummary);
                }
                this.hideLayouts();
                this.flAddEditSales.Visibility = ViewStates.Visible;
                this.setTableItemOptions();
                this.mRightDrawer.Adapter = new RightDrawerAdapter(this, this.tableItemOptions);
                this.showOptionMenu(Resource.Id.action_more);
                this.btnEvaluateSales.Text = "Update Sales";
                this.mDrawerLayout.OpenDrawer(mLeftDrawer);
                this.mDrawerToggle.setCloseDrawerDesc(Resource.String.Sales);
                this.hideLoader();
                this.hasAction = false;
                this.hideDynamicPopupMenu_Clicked(sender, e);
            }
            else
            {
                this.showMessage(getSaleDetail.message);
            }
        }
        public void displaySaleDetailInSummaryFab_Clicked(object sender, EventArgs e)
        {
            if (this.hasAction)
                return;

            if (this.salesSummaryListAdapter.getSelectedItems().Count == 0)
            {
                tvIndicator.Text = "No sale selected.";
                return;
            }

            this.hasAction = true;
            this.invokeLoader();
            Response getSaleDetail = new Response();
            SalesSummary saleDetail = new SalesSummary();
            var view = LayoutInflater.Inflate(Resource.Layout.AddEditCartDetail, null);
            AlertDialog.Builder dialogBuilder = new AlertDialog.Builder(this);
            dialogBuilder.SetCancelable(false);

            isAllowedToCloseDialog = false;
            dialogBuilder.SetView(view);
            dialogBuilder.SetPositiveButton("Close", delegate { isAllowedToCloseDialog = true; });

            getSaleDetail = saleDetail.getSaleById(this.salesSummaryListAdapter.getSelectedItems()[0].Id, "my_store.db");
            if (getSaleDetail.status.Equals("SUCCESS"))
            {
                
                view.FindViewById<TextView>(Resource.Id.tvSalesTime).Text = string.Format("Time: {0}", getSaleDetail.salesSummary[0].SalesDateTime.ToString("hh:mm tt"));
                view.FindViewById<TextView>(Resource.Id.tvTotalAmount).Text = string.Format("Total Amount: {0}", this.formatCurrency(getSaleDetail.salesSummary[0].Amount.ToString()));
                view.FindViewById<EditText>(Resource.Id.etAmountReceived).Text = this.formatCurrency(getSaleDetail.salesSummary[0].AmountReceived.ToString());
                view.FindViewById<EditText>(Resource.Id.etConsumer).Text = getSaleDetail.salesSummary[0].SoldTo.ToString();
                view.FindViewById<EditText>(Resource.Id.etVendor).Text = getSaleDetail.salesSummary[0].SoldBy.ToString();
                view.FindViewById<TextView>(Resource.Id.tvChange).Text = string.Format("Change: {0}", this.formatCurrency((getSaleDetail.salesSummary[0].AmountReceived - getSaleDetail.salesSummary[0].Amount).ToString()));
                this.hideLoader();
                dialogBuilder.Create().Show();
                this.hasAction = false;
            }
            else
            {
                this.hideLoader();
                tvIndicator.Text = "Item(Selected): An error has occured.";
                isAllowedToCloseDialog = true;
                this.hasAction = false;
            }
        }
        public void showMoreSummary_Clicked(object sender, EventArgs e)
        {
            if (this.hasAction)
                return;

            this.hasAction = true;

            int count = 0;

            Response getSalesSummary = new Response();
            SalesSummary salesSummaryModel = new SalesSummary();

            count = count + 1;
            if (count > 1)
                return;

            this.invokeLoader();
            getSalesSummary = salesSummaryModel.getSalesByDateTime(this.salesDate
                , this.sslfFromTime
                , this.sslfToTime
                , this.sslfConsumer
                , this.sslfVendor
                , "my_store.db"
                , this.salesSummaryListDisplay.Count);
            if (getSalesSummary.status.Equals("SUCCESS"))
            {
                foreach (SalesSummary ssm in getSalesSummary.salesSummary)
                {
                    this.salesSummaryListDisplay.Add(new ListItem()
                    {
                        Id = ssm.SaleMasterId
                        , IsChecked = false
                        , Description = string.Format("{0} - {1} {2} sold for {3}"
                                              , ssm.SalesDateTime.ToString("hh:mm:ss tt")
                                              , ssm.ItemsSold
                                              , ssm.ItemsSold > 1 ? "Items" : "Item"
                                              , this.formatCurrency(ssm.Amount.ToString()))
                    });
                }
                if (this.salesSummaryListAdapter == null)
                {
                    this.salesSummaryListAdapter = new ListItemAdapter(this, this.salesSummaryListDisplay);
                    this.lvSalesSummary.Adapter = this.salesSummaryListAdapter;
                }
                RunOnUiThread(() =>
                {
                    this.salesSummaryListAdapter.NotifyDataSetChanged();
                });
                this.hideLoader();
                this.hideLayouts();
                this.flSalesSummary.Visibility = ViewStates.Visible;
                this.showOptionMenu(Resource.Id.action_more);
                this.mDrawerLayout.OpenDrawer(mLeftDrawer);
                this.mDrawerToggle.setCloseDrawerDesc(Resource.String.SalesList);
                this.hideDynamicPopupMenu_Clicked(sender, e);
                count = 0;
            }
            else
            {
                this.hideLoader();
                this.showMessage(getSalesSummary.message);
                count = 0;
            }
        }
        /*----------------------------------Item Cart FABS----------------------------------------*/
        public void displaySaleDetailInCartFab_Clicked(object sender, EventArgs e)
        {
            if (this.hasAction)
                return;

            if (this.cartListAdapter.getSelectedItems().Count == 0)
            {
                tvIndicator.Text = "No item selected.";
                return;
            }

            this.hasAction = true;
            Item itemModel = new Item();
            Response response = new Response();
            var view = LayoutInflater.Inflate(Resource.Layout.CartItemDetail, null);
            AlertDialog.Builder dialogBuilder = new AlertDialog.Builder(this);
            dialogBuilder.SetCancelable(false);

            isAllowedToCloseDialog = false;
            dialogBuilder.SetView(view);
            dialogBuilder.SetPositiveButton("Close", delegate { isAllowedToCloseDialog = true; this.hasAction = false; });

            foreach (SaleDetail saleDetail in this.itemCartList)
            {
                if (saleDetail.ItemId == this.cartListAdapter.getSelectedItems()[0].Id)
                {
                    view.FindViewById<AutoCompleteTextView>(Resource.Id.atcvSalesItemDescription).Text =  saleDetail.ItemName;
                    view.FindViewById<EditText>(Resource.Id.etSalesBarcode).Text = saleDetail.Barcode.ToString();
                    view.FindViewById<EditText>(Resource.Id.etSalesQuantity).Text = saleDetail.Quantity.ToString();
                    view.FindViewById<EditText>(Resource.Id.etSalesRetailPrice).Text = this.formatCurrency(saleDetail.RetailPrice.ToString());
                    view.FindViewById<EditText>(Resource.Id.etSalesTotalPrice).Text = this.formatCurrency( (saleDetail.RetailPrice * saleDetail.Quantity).ToString());
                    dialogBuilder.Create().Show();
                    break;
                }
            }
            this.hasAction = false;
        }
        public void editSelectedSaleInCartFab_Clicked(object sender, EventArgs e)
        {
            if (this.hasAction)
                return;

            if (this.cartListAdapter.getSelectedItems().Count == 0)
            {
                tvIndicator.Text = "No item selected.";
                return;
            }

            this.hasAction = true;

            for(int i = 0; i< this.itemCartList.Count; i++)
            {
                if (this.cartListAdapter.getSelectedItems()[0].Id == this.itemCartList[i].ItemId)
                {
                    this.itemPosition = i;
                    this.atcvSalesItemDescription.Text = this.itemCartList[i].ItemName;
                    this.etSalesBarcode.Text = this.itemCartList[i].Barcode.ToString();
                    this.etSalesQuantity.Text = this.itemCartList[i].Quantity.ToString();
                    this.etSalesRetailPrice.Text = this.formatCurrency(this.itemCartList[i].RetailPrice.ToString());
                    this.etSalesTotalPrice.Text = this.formatCurrency((this.itemCartList[i].RetailPrice * this.itemCartList[i].Quantity).ToString());
                    this.hideLayouts();
                    this.showOptionMenu(Resource.Id.action_more);
                    this.svAddEditCart.Visibility = ViewStates.Visible;
                    this.hideDynamicPopupMenu_Clicked(sender, e);
                    this.btnEvaluateSaleItem.Text = "Edit Item In Cart";
                    this.atcvSalesItemDescription.Enabled = false;
                    this.etSalesQuantity.RequestFocus();
                    i = this.itemCartList.Count;
                }
            }
        }
        public void deleteSelectedSaleInCartFab_Clicked(object sender, EventArgs e)
        {
            if (this.hasAction)
                return;

            if (this.cartListAdapter.getSelectedItems().Count == 0)
            {
                tvIndicator.Text = "No item selected.";
                return;
            }

            int count = 0;
            string firstItem = "";
            tvIndicator.Text = "Preparing for delete...";
            if (this.cartListAdapter.getSelectedItems().Count > 0)
            {
                if (this.hasAction)
                    return;

                this.hasAction = true;

                var builder = new AlertDialog.Builder(this);
                builder.SetTitle("Application Message");
                builder.SetMessage("Are you sure you want to delete selected item/s?");
                builder.SetPositiveButton("Yes", delegate
                {
                    tvIndicator.Visibility = ViewStates.Visible;
                    isAllowedToCloseDialog = false;
                    foreach (ListItem listItem in cartListAdapter.getSelectedItems())
                    {
                        tvIndicator.Text = string.Format("Deleting {0}...", listItem.Description.Split('(')[0]);
                        firstItem = listItem.Description.Split('(')[0];
                        foreach(SaleDetail saleDetail in this.itemCartList)
                        {
                            if(saleDetail.ItemId == listItem.Id)
                            {
                                this.salesMasterModel.Amount = this.salesMasterModel.Amount - (double)(saleDetail.Quantity * saleDetail.RetailPrice);
                                this.cartItemsDisplay.Remove(listItem);
                                this.itemCartList.Remove(saleDetail);
                                count = count + 1;
                                break;
                            }
                        }
                    }
                    this.tvTotalAmount.Text = string.Format("Total Amount: {0}", this.formatCurrency(this.salesMasterModel.Amount.ToString()));
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
                        if (this.cartListAdapter != null)
                            this.cartListAdapter.NotifyDataSetChanged();
                    });
                    isAllowedToCloseDialog = true;
                    this.hasAction = false;
                });
                builder.SetNegativeButton("No", delegate { tvIndicator.Text = "Tap blank space to go back in the list."; this.hasAction = false; });
                builder.Create().Show();
            }
            else
            {
                tvIndicator.Text = "No item selected.";
            }
        }
        public void unCheckAllSaleInCartFab_Clicked(object sender, EventArgs e)
        {
            int count = 0;
            if (this.cartItemsDisplay.Count > 0)
            {
                if (this.hasAction)
                    return;

                this.hasAction = true;

                RunOnUiThread(() =>
                {
                    this.tvIndicator.Visibility = ViewStates.Visible;
                    this.isAllowedToCloseDialog = false;
                    for (int i = 0; i < this.cartItemsDisplay.Count; i++)
                    {
                        if (this.cartItemsDisplay[i].IsChecked)
                        {
                            this.tvIndicator.Text = string.Format("Unchecking record {0} of {1}", i + 1, this.cartItemsDisplay.Count);
                            this.cartItemsDisplay[i].IsChecked = false;
                            count = count + 1;
                        }
                        if (i == this.cartItemsDisplay.Count - 1)
                        {
                            if (count != 0)
                            {
                                this.tvIndicator.Text = count.ToString() + " record/s have been unchecked.";
                            }
                            else
                            {
                                this.tvIndicator.Text = "All record/s were already unchecked.";
                            }
                            this.isAllowedToCloseDialog = true;
                            if (this.cartListAdapter != null)
                                this.cartListAdapter.NotifyDataSetChanged();
                        }
                    }
                    this.hasAction = false;
                });
            }
            else
            {
                tvIndicator.Text = "Items in cart were all deleted.";
            }
        }
        public void checkAllSaleInCartFab_Clicked(object sender, EventArgs e)
        {
            int count = 0;
            if (this.cartItemsDisplay.Count > 0)
            {
                if (this.hasAction)
                    return;

                this.hasAction = true;
                RunOnUiThread(() =>
                {
                    this.tvIndicator.Visibility = ViewStates.Visible;
                    this.isAllowedToCloseDialog = false;
                    for (int i = 0; i < this.cartItemsDisplay.Count; i++)
                    {
                        if (!this.cartItemsDisplay[i].IsChecked)
                        {
                            this.tvIndicator.Text = string.Format("Checking record {0} of {1}", i + 1, this.cartItemsDisplay.Count);
                            this.cartItemsDisplay[i].IsChecked = true;
                            count = count + 1;
                        }
                        if (i == this.cartItemsDisplay.Count - 1)
                        {
                            if (count != 0)
                            {
                                this.tvIndicator.Text = count.ToString() + " record/s have been checked.";
                            }
                            else
                            {
                                this.tvIndicator.Text = "All record/s were already checked.";
                            }
                            this.isAllowedToCloseDialog = true;
                            if (this.cartListAdapter != null)
                                this.cartListAdapter.NotifyDataSetChanged();
                        }
                    }
                    this.hasAction = false;
                });
            }
            else
            {
                tvIndicator.Text = "Items in cart were all deleted.";
            }
        }
        /*----------------------------------Main FABS----------------------------------------*/
        public void fabMainAddEditSales_Clicked(object sender, EventArgs e)
        {
            if (this.hasAction)
                return;

            this.hasAction = true;

            var dynamicPopupMenuView = LayoutInflater.Inflate(Resource.Layout.DynamicPopupMenu, null);
            this.dynamicPopupDialog = new Dialog(this, Android.Resource.Style.AnimationTranslucent);
            this.dynamicPopupDialog.SetContentView(dynamicPopupMenuView);
            this.dynamicPopupDialog.Show();
            this.dynamicPopupDialog.SetCancelable(false);

            if (this.hideDynamicPopupDialogThread != null)
                this.hideDynamicPopupDialogThread.Abort();

            //Hide Search Fab Edit Text
            this.etSearchFab = dynamicPopupMenuView.FindViewById<EditText>(Resource.Id.etSearchFab);
            this.etSearchFab.Visibility = ViewStates.Gone;

            this.tvIndicator = dynamicPopupMenuView.FindViewById<TextView>(Resource.Id.tvIndicator);
            this.tvIndicator.Visibility = ViewStates.Visible;
            this.tvIndicator.Text = "Tap blank space to go back.";

            this.setAnimations();

            this.dMainFab = dynamicPopupMenuView.FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.mainFab);
            this.dMainFabLayoutParams = (FrameLayout.LayoutParams)this.dMainFab.LayoutParameters;
            this.dMainFab.LayoutParameters = this.dMainFabLayoutParams;
            this.dMainFab.Animation = this.mainFab_show;
            this.dMainFab.Clickable = true;

            this.dChildFab1 = dynamicPopupMenuView.FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.childFab1);
            this.dChildFab1LayoutParams = (FrameLayout.LayoutParams)this.dChildFab1.LayoutParameters;
            this.dChildFab1LayoutParams.RightMargin = this.dChildFab1LayoutParams.RightMargin + (int)(this.dChildFab1LayoutParams.RightMargin * .60);
            this.dChildFab1LayoutParams.BottomMargin = this.dChildFab1LayoutParams.BottomMargin * 5;
            this.dChildFab1.LayoutParameters = this.dChildFab1LayoutParams;
            this.dChildFab1.SetImageResource(Resource.Drawable.ShowDetail);
            this.dChildFab1.Size = FabSize.Mini;
            this.dChildFab1.Animation = this.childFab_show;
            this.dChildFab1.Clickable = true;

            this.tvChildFab1 = dynamicPopupMenuView.FindViewById<TextView>(Resource.Id.tvChildFab1);
            this.tvChildFab1LayoutParams = (FrameLayout.LayoutParams)this.tvChildFab1.LayoutParameters;
            this.tvChildFab1LayoutParams.RightMargin = this.tvChildFab1LayoutParams.RightMargin + (int)(tvChildFab1LayoutParams.RightMargin * 4);
            this.tvChildFab1LayoutParams.BottomMargin = this.tvChildFab1LayoutParams.BottomMargin * 5 + (int)(tvChildFab1LayoutParams.BottomMargin * .4);
            this.tvChildFab1.LayoutParameters = this.tvChildFab1LayoutParams;
            this.tvChildFab1.Text = "Show Cart";
            this.tvChildFab1.Animation = this.childFab_show;
            this.tvChildFab1.Clickable = true;

            this.dChildFab2 = dynamicPopupMenuView.FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.childFab2);
            this.dChildFab2LayoutParams = (FrameLayout.LayoutParams)this.dChildFab2.LayoutParameters;
            this.dChildFab2LayoutParams.RightMargin = this.dChildFab2LayoutParams.RightMargin + (int)(this.dChildFab2LayoutParams.RightMargin * .60);
            this.dChildFab2LayoutParams.BottomMargin = this.dChildFab2LayoutParams.BottomMargin * 8;
            this.dChildFab2.LayoutParameters = this.dChildFab2LayoutParams;
            this.dChildFab2.SetImageResource(Resource.Drawable.AddWhite);
            this.dChildFab2.Size = FabSize.Mini;
            this.dChildFab2.Animation = this.childFab_show;
            this.dChildFab2.Clickable = true;

            this.tvChildFab2 = dynamicPopupMenuView.FindViewById<TextView>(Resource.Id.tvChildFab2);
            this.tvChildFab2LayoutParams = (FrameLayout.LayoutParams)this.tvChildFab2.LayoutParameters;
            this.tvChildFab2LayoutParams.RightMargin = this.tvChildFab2LayoutParams.RightMargin + (int)(tvChildFab2LayoutParams.RightMargin * 4);
            this.tvChildFab2LayoutParams.BottomMargin = this.tvChildFab2LayoutParams.BottomMargin * 8 + (int)(tvChildFab2LayoutParams.BottomMargin * .4);
            this.tvChildFab2.LayoutParameters = this.tvChildFab2LayoutParams;
            this.tvChildFab2.Text = "Add Item To Cart";
            this.tvChildFab2.Animation = this.childFab_show;
            this.tvChildFab2.Clickable = true;

            foreach (Configuration conf in this.access)
            {
                if (conf.Description.Equals("AllowScanning"))
                {
                    if (conf.IsGranted == 1)
                    {
                        tableItemOptions.Add(new TableItem() { ImageResourceId = Resource.Drawable.Scan, ItemLabel = "Scan Item" });
                        canScan = true;
                        this.dChildFab3 = dynamicPopupMenuView.FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.childFab3);
                        this.dChildFab3LayoutParams = (FrameLayout.LayoutParams)this.dChildFab3.LayoutParameters;
                        this.dChildFab3LayoutParams.RightMargin = this.dChildFab3LayoutParams.RightMargin + (int)(this.dChildFab3LayoutParams.RightMargin * .60);
                        this.dChildFab3LayoutParams.BottomMargin = this.dChildFab3LayoutParams.BottomMargin * 11;
                        this.dChildFab3.LayoutParameters = this.dChildFab3LayoutParams;
                        this.dChildFab3.SetImageResource(Resource.Drawable.ScanWhite);
                        this.dChildFab3.Size = FabSize.Mini;
                        this.dChildFab3.Animation = this.childFab_show;
                        this.dChildFab3.Clickable = true;

                        this.tvChildFab3 = dynamicPopupMenuView.FindViewById<TextView>(Resource.Id.tvChildFab3);
                        this.tvChildFab3LayoutParams = (FrameLayout.LayoutParams)this.tvChildFab3.LayoutParameters;
                        this.tvChildFab3LayoutParams.RightMargin = this.tvChildFab3LayoutParams.RightMargin + (int)(tvChildFab3LayoutParams.RightMargin * 4);
                        this.tvChildFab3LayoutParams.BottomMargin = this.tvChildFab3LayoutParams.BottomMargin * 11 + (int)(tvChildFab3LayoutParams.BottomMargin * .4);
                        this.tvChildFab3.LayoutParameters = this.tvChildFab3LayoutParams;
                        this.tvChildFab3.Text = "Scan Item";
                        this.tvChildFab3.Animation = this.childFab_show;
                        this.tvChildFab3.Clickable = true;

                        this.dChildFab3.Click += this.scanSalesItemFab_Clicked;
                        break;
                    }
                }
            }
            

            this.fabAnimationWatcher = new System.Threading.Thread(new ThreadStart(delegate
            {
                System.Threading.Thread.Sleep(350);
                this.hasAction = false;
            }));
            this.fabAnimationWatcher.Start();

            this.dChildFab1.Click += this.showCartFab_Clicked;
            this.dChildFab2.Click += this.addItemToCartFab_Clicked;
            dynamicPopupMenuView.Click += this.hideDynamicPopupMenu_Clicked;
        }
        public void mainSalesFab_Clicked(object sender, EventArgs e)
        {
            if (this.hasAction)
                return;

            this.hasAction = true;

            var popupMenuViewSales = LayoutInflater.Inflate(Resource.Layout.SalesMainPopupMenu, null);
            salesPopupMenuDialog = new Dialog(this, Android.Resource.Style.AnimationTranslucent);
            salesPopupMenuDialog.SetContentView(popupMenuViewSales);
            salesPopupMenuDialog.Show();
            salesPopupMenuDialog.SetCancelable(false);

            if (hideSalesFabThread != null)
                hideSalesFabThread.Abort();

            tvIndicator = popupMenuViewSales.FindViewById<TextView>(Resource.Id.tvIndicator);
            tvIndicator.Visibility = ViewStates.Visible;
            tvIndicator.Text = "Tap blank space to go back.";
            tvIndicator.Click += hideSalesMainPopupMenu_Clicked;

            //Animations
            mainFab_show = AnimationUtils.LoadAnimation(this, Resource.Layout.mainFab_show);
            mainFab_hide = AnimationUtils.LoadAnimation(this, Resource.Layout.mainFab_hide);
            childFab_show = AnimationUtils.LoadAnimation(this, Resource.Layout.childFab_show);
            childFab_hide = AnimationUtils.LoadAnimation(this, Resource.Layout.childFab_hide);

            pMainSalesFab = popupMenuViewSales.FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.mainSalesFab);
            pMainSalesFabLayoutParams = (FrameLayout.LayoutParams)pMainSalesFab.LayoutParameters;
            pMainSalesFab.LayoutParameters = pMainSalesFabLayoutParams;
            pMainSalesFab.Animation = mainFab_show;
            pMainSalesFab.Clickable = true;

            selectSalesDateFab = popupMenuViewSales.FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.selectSalesDateFab);
            selectSalesDateFabLayoutParams = (FrameLayout.LayoutParams)selectSalesDateFab.LayoutParameters;

            tvSelectSalesDate = popupMenuViewSales.FindViewById<TextView>(Resource.Id.tvSelectSalesDate);
            tvSelectSalesDateLayoutParams = (FrameLayout.LayoutParams)tvSelectSalesDate.LayoutParameters;

            showSalesDateFab = popupMenuViewSales.FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.showSalesFab);
            showSalesFabLayoutParams = (FrameLayout.LayoutParams)showSalesDateFab.LayoutParameters;

            tvShowSales = popupMenuViewSales.FindViewById<TextView>(Resource.Id.tvShowSales);
            tvShowSalesLayoutParams = (FrameLayout.LayoutParams)tvShowSales.LayoutParameters;

            addSalesFab = popupMenuViewSales.FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.addSalesFab);
            addSalesFabLayoutParams = (FrameLayout.LayoutParams)addSalesFab.LayoutParameters;

            tvAddSales = popupMenuViewSales.FindViewById<TextView>(Resource.Id.tvAddSales);
            tvAddSalesLayoutParams = (FrameLayout.LayoutParams)tvAddSales.LayoutParameters;

            selectSalesDateFabLayoutParams.RightMargin = selectSalesDateFabLayoutParams.RightMargin + (int)(selectSalesDateFabLayoutParams.RightMargin * .60);
            selectSalesDateFabLayoutParams.BottomMargin = selectSalesDateFabLayoutParams.BottomMargin * 5;
            selectSalesDateFab.LayoutParameters = selectSalesDateFabLayoutParams;
            selectSalesDateFab.Size = FabSize.Mini;
            selectSalesDateFab.Animation = childFab_show;
            selectSalesDateFab.Clickable = true;

            tvSelectSalesDateLayoutParams.RightMargin = tvSelectSalesDateLayoutParams.RightMargin + (int)(tvSelectSalesDateLayoutParams.RightMargin * 4);
            tvSelectSalesDateLayoutParams.BottomMargin = tvSelectSalesDateLayoutParams.BottomMargin * 5 + (int)(tvSelectSalesDateLayoutParams.BottomMargin * .4);
            tvSelectSalesDate.LayoutParameters = tvSelectSalesDateLayoutParams;
            tvSelectSalesDate.Animation = childFab_show;
            tvSelectSalesDate.Clickable = true;

            showSalesFabLayoutParams.RightMargin = showSalesFabLayoutParams.RightMargin + (int)(showSalesFabLayoutParams.RightMargin * .60);
            showSalesFabLayoutParams.BottomMargin = showSalesFabLayoutParams.BottomMargin * 8;
            showSalesDateFab.LayoutParameters = showSalesFabLayoutParams;
            showSalesDateFab.Size = FabSize.Mini;
            showSalesDateFab.Animation = childFab_show;
            showSalesDateFab.Clickable = true;

            tvShowSalesLayoutParams.RightMargin = tvShowSalesLayoutParams.RightMargin + (int)(tvShowSalesLayoutParams.RightMargin * 4);
            tvShowSalesLayoutParams.BottomMargin = tvShowSalesLayoutParams.BottomMargin * 8 + (int)(tvShowSalesLayoutParams.BottomMargin * .4);
            tvShowSales.LayoutParameters = tvShowSalesLayoutParams;
            tvShowSales.Animation = childFab_show;
            tvShowSales.Clickable = true;

            addSalesFabLayoutParams.RightMargin = addSalesFabLayoutParams.RightMargin + (int)(addSalesFabLayoutParams.RightMargin * .60);
            addSalesFabLayoutParams.BottomMargin = addSalesFabLayoutParams.BottomMargin * 11;
            addSalesFab.LayoutParameters = addSalesFabLayoutParams;
            addSalesFab.Size = FabSize.Mini;
            addSalesFab.Animation = childFab_show;
            addSalesFab.Clickable = true;

            tvAddSalesLayoutParams.RightMargin = tvAddSalesLayoutParams.RightMargin + (int)(tvAddSalesLayoutParams.RightMargin * 4);
            tvAddSalesLayoutParams.BottomMargin = tvAddSalesLayoutParams.BottomMargin * 11 + (int)(tvAddSalesLayoutParams.BottomMargin * .4);
            tvAddSales.LayoutParameters = tvAddSalesLayoutParams;
            tvAddSales.Animation = childFab_show;
            tvAddSales.Clickable = true;

            this.fabAnimationWatcher = new System.Threading.Thread(new ThreadStart(delegate
            {
                System.Threading.Thread.Sleep(350);
                this.hasAction = false;
            }));
            this.fabAnimationWatcher.Start();

            popupMenuViewSales.Click += hideSalesMainPopupMenu_Clicked;
            addSalesFab.Click += addSalesFab_Clicked;
            showSalesDateFab.Click += showSalesByDateFab_Clicked;
            selectSalesDateFab.Click += selectSalesDateFab_Clicked;

        }
        public void cartListMainFab_Clicked(object sender, EventArgs e)
        {
            if (this.hasAction)
                return;

            this.hasAction = true;

            var dynamicPopupMenuView = LayoutInflater.Inflate(Resource.Layout.DynamicPopupMenu, null);
            this.dynamicPopupDialog = new Dialog(this, Android.Resource.Style.AnimationTranslucent);
            this.dynamicPopupDialog.SetContentView(dynamicPopupMenuView);
            this.dynamicPopupDialog.Show();
            this.dynamicPopupDialog.SetCancelable(false);

            if (this.hideDynamicPopupDialogThread != null)
                this.hideDynamicPopupDialogThread.Abort();

            //Hide Search Fab Edit Text
            this.etSearchFab = dynamicPopupMenuView.FindViewById<EditText>(Resource.Id.etSearchFab);
            this.etSearchFab.Visibility = ViewStates.Gone;

            this.tvIndicator = dynamicPopupMenuView.FindViewById<TextView>(Resource.Id.tvIndicator);
            this.tvIndicator.Visibility = ViewStates.Visible;
            this.tvIndicator.Text = "Tap blank space to go back in the list.";

            this.setAnimations();

            this.dMainFab = dynamicPopupMenuView.FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.mainFab);
            this.dMainFabLayoutParams = (FrameLayout.LayoutParams)this.dMainFab.LayoutParameters;
            this.dMainFab.LayoutParameters = this.dMainFabLayoutParams;
            this.dMainFab.Animation = this.mainFab_show;
            this.dMainFab.Clickable = true;

            this.tvMainFab = dynamicPopupMenuView.FindViewById<TextView>(Resource.Id.tvMainFab);
            this.tvMainFabLayoutParams = (FrameLayout.LayoutParams)this.tvMainFab.LayoutParameters;
            this.tvMainFab.Text = "Go back to sales";
            this.tvMainFabLayoutParams.RightMargin = this.tvMainFabLayoutParams.RightMargin + (int)(this.tvMainFabLayoutParams.RightMargin * 4);
            this.tvMainFabLayoutParams.BottomMargin = this.tvMainFabLayoutParams.BottomMargin * 2;
            this.tvMainFab.LayoutParameters = this.tvMainFabLayoutParams;
            this.tvMainFab.Animation = childFab_show;
            this.tvMainFab.Clickable = true;

            this.dChildFab1 = dynamicPopupMenuView.FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.childFab1);
            this.dChildFab1LayoutParams = (FrameLayout.LayoutParams)this.dChildFab1.LayoutParameters;
            this.dChildFab1.SetImageResource(Resource.Drawable.ShowDetail);
            this.dChildFab1.Size = FabSize.Mini;

            this.tvChildFab1 = dynamicPopupMenuView.FindViewById<TextView>(Resource.Id.tvChildFab1);
            this.tvChildFab1LayoutParams = (FrameLayout.LayoutParams)this.tvChildFab1.LayoutParameters;
            this.tvChildFab1.Text = "Display Item Details";

            this.dChildFab2 = dynamicPopupMenuView.FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.childFab2);
            this.dChildFab2LayoutParams = (FrameLayout.LayoutParams)this.dChildFab2.LayoutParameters;
            this.dChildFab2.SetImageResource(Resource.Drawable.Edit);
            this.dChildFab2.Size = FabSize.Mini;

            this.tvChildFab2 = dynamicPopupMenuView.FindViewById<TextView>(Resource.Id.tvChildFab2);
            this.tvChildFab2LayoutParams = (FrameLayout.LayoutParams)this.tvChildFab2.LayoutParameters;
            this.tvChildFab2.Text = "Edit Selected Item";

            this.dChildFab3 = dynamicPopupMenuView.FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.childFab3);
            this.dChildFab3LayoutParams = (FrameLayout.LayoutParams)this.dChildFab3.LayoutParameters;
            this.dChildFab3.SetImageResource(Resource.Drawable.Delete);
            this.dChildFab3.Size = FabSize.Mini;

            this.tvChildFab3 = dynamicPopupMenuView.FindViewById<TextView>(Resource.Id.tvChildFab3);
            this.tvChildFab3LayoutParams = (FrameLayout.LayoutParams)this.tvChildFab3.LayoutParameters;
            this.tvChildFab3.Text = "Delete Selected Item/s";

            this.dChildFab4 = dynamicPopupMenuView.FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.childFab4);
            this.dChildFab4LayoutParams = (FrameLayout.LayoutParams)this.dChildFab4.LayoutParameters;
            this.dChildFab4.SetImageResource(Resource.Drawable.Uncheck);
            this.dChildFab4.Size = FabSize.Mini;

            this.tvChildFab4 = dynamicPopupMenuView.FindViewById<TextView>(Resource.Id.tvChildFab4);
            this.tvChildFab4LayoutParams = (FrameLayout.LayoutParams)this.tvChildFab4.LayoutParameters;
            this.tvChildFab4.Text = "Uncheck-All Item";

            this.dChildFab5 = dynamicPopupMenuView.FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.childFab5);
            this.dChildFab5LayoutParams = (FrameLayout.LayoutParams)this.dChildFab5.LayoutParameters;
            this.dChildFab5.SetImageResource(Resource.Drawable.Check);
            this.dChildFab5.Size = FabSize.Mini;

            this.tvChildFab5 = dynamicPopupMenuView.FindViewById<TextView>(Resource.Id.tvChildFab5);
            this.tvChildFab5LayoutParams = (FrameLayout.LayoutParams)this.tvChildFab5.LayoutParameters;
            this.tvChildFab5.Text = "Check-All Item";

            if (this.cartListAdapter.getSelectedItems().Count == 0 || this.cartListAdapter.getSelectedItems().Count != 1)
            {
                this.dChildFab1 = null;
                this.dChildFab2 = null;
                this.tvChildFab1 = null;
                this.tvChildFab2 = null;

                this.dChildFab3LayoutParams.RightMargin = this.dChildFab3LayoutParams.RightMargin + (int)(this.dChildFab3LayoutParams.RightMargin * .60);
                this.dChildFab3LayoutParams.BottomMargin = this.dChildFab3LayoutParams.BottomMargin * 5;
                this.dChildFab3.LayoutParameters = this.dChildFab3LayoutParams;
                this.dChildFab3.Animation = this.childFab_show;
                this.dChildFab3.Clickable = true;

                this.tvChildFab3LayoutParams.RightMargin = this.tvChildFab3LayoutParams.RightMargin + (int)(tvChildFab3LayoutParams.RightMargin * 4);
                this.tvChildFab3LayoutParams.BottomMargin = this.tvChildFab3LayoutParams.BottomMargin * 5 + (int)(tvChildFab3LayoutParams.BottomMargin * .4);
                this.tvChildFab3.LayoutParameters = this.tvChildFab3LayoutParams;
                this.tvChildFab3.Animation = this.childFab_show;
                this.tvChildFab3.Clickable = true;

                this.dChildFab4LayoutParams.RightMargin = this.dChildFab4LayoutParams.RightMargin + (int)(this.dChildFab4LayoutParams.RightMargin * .60);
                this.dChildFab4LayoutParams.BottomMargin = this.dChildFab4LayoutParams.BottomMargin * 8;
                this.dChildFab4.LayoutParameters = this.dChildFab4LayoutParams;
                this.dChildFab4.Animation = this.childFab_show;
                this.dChildFab4.Clickable = true;

                this.tvChildFab4LayoutParams.RightMargin = this.tvChildFab4LayoutParams.RightMargin + (int)(tvChildFab4LayoutParams.RightMargin * 4);
                this.tvChildFab4LayoutParams.BottomMargin = this.tvChildFab4LayoutParams.BottomMargin * 8 + (int)(tvChildFab4LayoutParams.BottomMargin * .4);
                this.tvChildFab4.LayoutParameters = this.tvChildFab4LayoutParams;
                this.tvChildFab4.Animation = this.childFab_show;
                this.tvChildFab4.Clickable = true;

                this.dChildFab5LayoutParams.RightMargin = this.dChildFab5LayoutParams.RightMargin + (int)(this.dChildFab5LayoutParams.RightMargin * .60);
                this.dChildFab5LayoutParams.BottomMargin = this.dChildFab5LayoutParams.BottomMargin * 11;
                this.dChildFab5.LayoutParameters = this.dChildFab5LayoutParams;
                this.dChildFab5.Animation = this.childFab_show;
                this.dChildFab5.Clickable = true;

                this.tvChildFab5LayoutParams.RightMargin = this.tvChildFab5LayoutParams.RightMargin + (int)(tvChildFab5LayoutParams.RightMargin * 4);
                this.tvChildFab5LayoutParams.BottomMargin = this.tvChildFab5LayoutParams.BottomMargin * 11 + (int)(tvChildFab5LayoutParams.BottomMargin * .4);
                this.tvChildFab5.LayoutParameters = this.tvChildFab5LayoutParams;
                this.tvChildFab5.Animation = this.childFab_show;
                this.tvChildFab5.Clickable = true;

                this.fabAnimationWatcher = new System.Threading.Thread(new ThreadStart(delegate
                {
                    System.Threading.Thread.Sleep(350);
                    this.hasAction = false;
                }));
                this.fabAnimationWatcher.Start();

                this.dChildFab3.Click += this.deleteSelectedSaleInCartFab_Clicked;
                this.dChildFab4.Click += this.unCheckAllSaleInCartFab_Clicked;
                this.dChildFab5.Click += this.checkAllSaleInCartFab_Clicked;
            }
            else
            {
                this.dChildFab1LayoutParams.RightMargin = this.dChildFab1LayoutParams.RightMargin + (int)(this.dChildFab1LayoutParams.RightMargin * .60);
                this.dChildFab1LayoutParams.BottomMargin = this.dChildFab1LayoutParams.BottomMargin * 5;
                this.dChildFab1.LayoutParameters = this.dChildFab1LayoutParams;
                this.dChildFab1.Animation = this.childFab_show;
                this.dChildFab1.Clickable = true;

                this.tvChildFab1LayoutParams.RightMargin = this.tvChildFab1LayoutParams.RightMargin + (int)(tvChildFab1LayoutParams.RightMargin * 4);
                this.tvChildFab1LayoutParams.BottomMargin = this.tvChildFab1LayoutParams.BottomMargin * 5 + (int)(tvChildFab1LayoutParams.BottomMargin * .4);
                this.tvChildFab1.LayoutParameters = this.tvChildFab1LayoutParams;
                this.tvChildFab1.Animation = this.childFab_show;
                this.tvChildFab1.Clickable = true;

                this.dChildFab2LayoutParams.RightMargin = this.dChildFab2LayoutParams.RightMargin + (int)(this.dChildFab2LayoutParams.RightMargin * .60);
                this.dChildFab2LayoutParams.BottomMargin = this.dChildFab2LayoutParams.BottomMargin * 8;
                this.dChildFab2.LayoutParameters = this.dChildFab2LayoutParams;
                this.dChildFab2.Animation = this.childFab_show;
                this.dChildFab2.Clickable = true;

                this.tvChildFab2LayoutParams.RightMargin = this.tvChildFab2LayoutParams.RightMargin + (int)(tvChildFab2LayoutParams.RightMargin * 4);
                this.tvChildFab2LayoutParams.BottomMargin = this.tvChildFab2LayoutParams.BottomMargin * 8 + (int)(tvChildFab2LayoutParams.BottomMargin * .4);
                this.tvChildFab2.LayoutParameters = this.tvChildFab2LayoutParams;
                this.tvChildFab2.Animation = this.childFab_show;
                this.tvChildFab2.Clickable = true;

                this.dChildFab3LayoutParams.RightMargin = this.dChildFab3LayoutParams.RightMargin + (int)(this.dChildFab3LayoutParams.RightMargin * .60);
                this.dChildFab3LayoutParams.BottomMargin = this.dChildFab3LayoutParams.BottomMargin * 11;
                this.dChildFab3.LayoutParameters = this.dChildFab3LayoutParams;
                this.dChildFab3.Animation = this.childFab_show;
                this.dChildFab3.Clickable = true;

                this.tvChildFab3LayoutParams.RightMargin = this.tvChildFab3LayoutParams.RightMargin + (int)(tvChildFab3LayoutParams.RightMargin * 4);
                this.tvChildFab3LayoutParams.BottomMargin = this.tvChildFab3LayoutParams.BottomMargin * 11 + (int)(tvChildFab3LayoutParams.BottomMargin * .4);
                this.tvChildFab3.LayoutParameters = this.tvChildFab3LayoutParams;
                this.tvChildFab3.Animation = this.childFab_show;
                this.tvChildFab3.Clickable = true;

                this.dChildFab4LayoutParams.RightMargin = this.dChildFab4LayoutParams.RightMargin + (int)(this.dChildFab4LayoutParams.RightMargin * .60);
                this.dChildFab4LayoutParams.BottomMargin = this.dChildFab4LayoutParams.BottomMargin * 14;
                this.dChildFab4.LayoutParameters = this.dChildFab4LayoutParams;
                this.dChildFab4.Animation = this.childFab_show;
                this.dChildFab4.Clickable = true;

                this.tvChildFab4LayoutParams.RightMargin = this.tvChildFab4LayoutParams.RightMargin + (int)(tvChildFab4LayoutParams.RightMargin * 4);
                this.tvChildFab4LayoutParams.BottomMargin = this.tvChildFab4LayoutParams.BottomMargin * 14 + (int)(tvChildFab4LayoutParams.BottomMargin * .4);
                this.tvChildFab4.LayoutParameters = this.tvChildFab4LayoutParams;
                this.tvChildFab4.Animation = this.childFab_show;
                this.tvChildFab4.Clickable = true;

                this.dChildFab5LayoutParams.RightMargin = this.dChildFab5LayoutParams.RightMargin + (int)(this.dChildFab5LayoutParams.RightMargin * .60);
                this.dChildFab5LayoutParams.BottomMargin = this.dChildFab5LayoutParams.BottomMargin * 17;
                this.dChildFab5.LayoutParameters = this.dChildFab5LayoutParams;
                this.dChildFab5.Animation = this.childFab_show;
                this.dChildFab5.Clickable = true;

                this.tvChildFab5LayoutParams.RightMargin = this.tvChildFab5LayoutParams.RightMargin + (int)(tvChildFab5LayoutParams.RightMargin * 4);
                this.tvChildFab5LayoutParams.BottomMargin = this.tvChildFab5LayoutParams.BottomMargin * 17 + (int)(tvChildFab5LayoutParams.BottomMargin * .4);
                this.tvChildFab5.LayoutParameters = this.tvChildFab5LayoutParams;
                this.tvChildFab5.Animation = this.childFab_show;
                this.tvChildFab5.Clickable = true;

                this.fabAnimationWatcher = new System.Threading.Thread(new ThreadStart(delegate
                {
                    System.Threading.Thread.Sleep(350);
                    this.hasAction = false;
                }));
                this.fabAnimationWatcher.Start();

                this.dChildFab1.Click += this.displaySaleDetailInCartFab_Clicked;
                this.dChildFab2.Click += this.editSelectedSaleInCartFab_Clicked;
                this.dChildFab3.Click += this.deleteSelectedSaleInCartFab_Clicked;
                this.dChildFab4.Click += this.unCheckAllSaleInCartFab_Clicked;
                this.dChildFab5.Click += this.checkAllSaleInCartFab_Clicked;
            }

            this.dMainFab.Click += this.hideCartList_Clicked;
            dynamicPopupMenuView.Click += this.hideDynamicPopupMenu_Clicked;
        }
        public void salesSummaryMainFab_Clicked(object sender, EventArgs e)
        {
            if (this.hasAction)
                return;

            this.hasAction = true;

            var dynamicPopupMenuView = LayoutInflater.Inflate(Resource.Layout.DynamicPopupMenu, null);
            this.dynamicPopupDialog = new Dialog(this, Android.Resource.Style.AnimationTranslucent);
            this.dynamicPopupDialog.SetContentView(dynamicPopupMenuView);
            this.dynamicPopupDialog.Show();
            this.dynamicPopupDialog.SetCancelable(false);

            if (this.hideDynamicPopupDialogThread != null)
                this.hideDynamicPopupDialogThread.Abort();

            this.tvIndicator = dynamicPopupMenuView.FindViewById<TextView>(Resource.Id.tvIndicator);
            this.tvIndicator.Visibility = ViewStates.Visible;
            this.tvIndicator.Text = "Tap blank space to go back in the list.";

            this.setAnimations();

            this.dMainFab = dynamicPopupMenuView.FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.mainFab);
            this.dMainFabLayoutParams = (FrameLayout.LayoutParams)this.dMainFab.LayoutParameters;
            this.dMainFab.LayoutParameters = this.dMainFabLayoutParams;
            this.dMainFab.Animation = this.mainFab_show;
            this.dMainFab.Clickable = true;

            this.tvMainFab = dynamicPopupMenuView.FindViewById<TextView>(Resource.Id.tvMainFab);
            this.tvMainFabLayoutParams = (FrameLayout.LayoutParams)this.tvMainFab.LayoutParameters;
            this.tvMainFab.Text = "Show more sales";
            this.tvMainFabLayoutParams.RightMargin = this.tvMainFabLayoutParams.RightMargin + (int)(this.tvMainFabLayoutParams.RightMargin * 4);
            this.tvMainFabLayoutParams.BottomMargin = this.tvMainFabLayoutParams.BottomMargin * 2;
            this.tvMainFab.LayoutParameters = this.tvMainFabLayoutParams;
            this.tvMainFab.Animation = childFab_show;
            this.tvMainFab.Clickable = true;

            this.dChildFab1 = dynamicPopupMenuView.FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.childFab1);
            this.dChildFab1LayoutParams = (FrameLayout.LayoutParams)this.dChildFab1.LayoutParameters;
            this.dChildFab1.SetImageResource(Resource.Drawable.ShowDetail);
            this.dChildFab1.Size = FabSize.Mini;

            this.tvChildFab1 = dynamicPopupMenuView.FindViewById<TextView>(Resource.Id.tvChildFab1);
            this.tvChildFab1LayoutParams = (FrameLayout.LayoutParams)this.tvChildFab1.LayoutParameters;
            this.tvChildFab1.Text = "Display Sale Details";

            this.dChildFab2 = dynamicPopupMenuView.FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.childFab2);
            this.dChildFab2LayoutParams = (FrameLayout.LayoutParams)this.dChildFab2.LayoutParameters;
            this.dChildFab2.SetImageResource(Resource.Drawable.Edit);
            this.dChildFab2.Size = FabSize.Mini;

            this.tvChildFab2 = dynamicPopupMenuView.FindViewById<TextView>(Resource.Id.tvChildFab2);
            this.tvChildFab2LayoutParams = (FrameLayout.LayoutParams)this.tvChildFab2.LayoutParameters;
            this.tvChildFab2.Text = "Edit Selected Sale";

            this.dChildFab3 = dynamicPopupMenuView.FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.childFab3);
            this.dChildFab3LayoutParams = (FrameLayout.LayoutParams)this.dChildFab3.LayoutParameters;
            this.dChildFab3.SetImageResource(Resource.Drawable.Delete);
            this.dChildFab3.Size = FabSize.Mini;

            this.tvChildFab3 = dynamicPopupMenuView.FindViewById<TextView>(Resource.Id.tvChildFab3);
            this.tvChildFab3LayoutParams = (FrameLayout.LayoutParams)this.tvChildFab3.LayoutParameters;
            this.tvChildFab3.Text = "Delete Selected Sale/s";

            this.dChildFab4 = dynamicPopupMenuView.FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.childFab4);
            this.dChildFab4LayoutParams = (FrameLayout.LayoutParams)this.dChildFab4.LayoutParameters;
            this.dChildFab4.SetImageResource(Resource.Drawable.Uncheck);
            this.dChildFab4.Size = FabSize.Mini;

            this.tvChildFab4 = dynamicPopupMenuView.FindViewById<TextView>(Resource.Id.tvChildFab4);
            this.tvChildFab4LayoutParams = (FrameLayout.LayoutParams)this.tvChildFab4.LayoutParameters;
            this.tvChildFab4.Text = "Uncheck-All Sales";

            this.dChildFab5 = dynamicPopupMenuView.FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.childFab5);
            this.dChildFab5LayoutParams = (FrameLayout.LayoutParams)this.dChildFab5.LayoutParameters;
            this.dChildFab5.SetImageResource(Resource.Drawable.Check);
            this.dChildFab5.Size = FabSize.Mini;

            this.tvChildFab5 = dynamicPopupMenuView.FindViewById<TextView>(Resource.Id.tvChildFab5);
            this.tvChildFab5LayoutParams = (FrameLayout.LayoutParams)this.tvChildFab5.LayoutParameters;
            this.tvChildFab5.Text = "Check-All Sales";

            this.dSearchFab = dynamicPopupMenuView.FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.searchFab);
            this.dSearchFabLayoutParams = (FrameLayout.LayoutParams)this.dSearchFab.LayoutParameters;
            this.dSearchFab.SetImageResource(Resource.Drawable.Search);
            this.dSearchFab.Size = FabSize.Mini;

            this.etSearchFab = dynamicPopupMenuView.FindViewById<EditText>(Resource.Id.etSearchFab);
            this.etSearchFabLayoutParams = (FrameLayout.LayoutParams)this.etSearchFab.LayoutParameters;
            this.etSearchFab.Text = "Find sales";
            this.etSearchFab.Enabled = false;


            if (this.salesSummaryListAdapter.getSelectedItems().Count == 0 || this.salesSummaryListAdapter.getSelectedItems().Count != 1)
            {
                this.dChildFab1 = null;
                this.dChildFab2 = null;
                this.tvChildFab1 = null;
                this.tvChildFab2 = null;

                this.dChildFab3LayoutParams.RightMargin = this.dChildFab3LayoutParams.RightMargin + (int)(this.dChildFab3LayoutParams.RightMargin * .60);
                this.dChildFab3LayoutParams.BottomMargin = this.dChildFab3LayoutParams.BottomMargin * 5;
                this.dChildFab3.LayoutParameters = this.dChildFab3LayoutParams;
                this.dChildFab3.Animation = this.childFab_show;
                this.dChildFab3.Clickable = true;

                this.tvChildFab3LayoutParams.RightMargin = this.tvChildFab3LayoutParams.RightMargin + (int)(tvChildFab3LayoutParams.RightMargin * 4);
                this.tvChildFab3LayoutParams.BottomMargin = this.tvChildFab3LayoutParams.BottomMargin * 5 + (int)(tvChildFab3LayoutParams.BottomMargin * .4);
                this.tvChildFab3.LayoutParameters = this.tvChildFab3LayoutParams;
                this.tvChildFab3.Animation = this.childFab_show;
                this.tvChildFab3.Clickable = true;

                this.dChildFab4LayoutParams.RightMargin = this.dChildFab4LayoutParams.RightMargin + (int)(this.dChildFab4LayoutParams.RightMargin * .60);
                this.dChildFab4LayoutParams.BottomMargin = this.dChildFab4LayoutParams.BottomMargin * 8;
                this.dChildFab4.LayoutParameters = this.dChildFab4LayoutParams;
                this.dChildFab4.Animation = this.childFab_show;
                this.dChildFab4.Clickable = true;

                this.tvChildFab4LayoutParams.RightMargin = this.tvChildFab4LayoutParams.RightMargin + (int)(tvChildFab4LayoutParams.RightMargin * 4);
                this.tvChildFab4LayoutParams.BottomMargin = this.tvChildFab4LayoutParams.BottomMargin * 8 + (int)(tvChildFab4LayoutParams.BottomMargin * .4);
                this.tvChildFab4.LayoutParameters = this.tvChildFab4LayoutParams;
                this.tvChildFab4.Animation = this.childFab_show;
                this.tvChildFab4.Clickable = true;

                this.dChildFab5LayoutParams.RightMargin = this.dChildFab5LayoutParams.RightMargin + (int)(this.dChildFab5LayoutParams.RightMargin * .60);
                this.dChildFab5LayoutParams.BottomMargin = this.dChildFab5LayoutParams.BottomMargin * 11;
                this.dChildFab5.LayoutParameters = this.dChildFab5LayoutParams;
                this.dChildFab5.Animation = this.childFab_show;
                this.dChildFab5.Clickable = true;

                this.tvChildFab5LayoutParams.RightMargin = this.tvChildFab5LayoutParams.RightMargin + (int)(tvChildFab5LayoutParams.RightMargin * 4);
                this.tvChildFab5LayoutParams.BottomMargin = this.tvChildFab5LayoutParams.BottomMargin * 11 + (int)(tvChildFab5LayoutParams.BottomMargin * .4);
                this.tvChildFab5.LayoutParameters = this.tvChildFab5LayoutParams;
                this.tvChildFab5.Animation = this.childFab_show;
                this.tvChildFab5.Clickable = true;

                this.dSearchFabLayoutParams.RightMargin = this.dSearchFabLayoutParams.RightMargin + (int)(this.dSearchFabLayoutParams.RightMargin * .60);
                this.dSearchFabLayoutParams.BottomMargin = this.dSearchFabLayoutParams.BottomMargin * 14;
                this.dSearchFab.LayoutParameters = this.dSearchFabLayoutParams;
                this.dSearchFab.Animation = this.childFab_show;
                this.dSearchFab.Clickable = true;

                this.etSearchFabLayoutParams.RightMargin = this.etSearchFabLayoutParams.RightMargin + (int)(etSearchFabLayoutParams.RightMargin * 4);
                this.etSearchFabLayoutParams.BottomMargin = this.etSearchFabLayoutParams.BottomMargin * 14 + (int)(etSearchFabLayoutParams.BottomMargin * .4);
                this.etSearchFab.LayoutParameters = this.etSearchFabLayoutParams;
                this.etSearchFab.Animation = this.childFab_show;
                this.etSearchFab.Clickable = true;

                this.fabAnimationWatcher = new System.Threading.Thread(new ThreadStart(delegate
                {
                    System.Threading.Thread.Sleep(350);
                    this.hasAction = false;
                }));
                this.fabAnimationWatcher.Start();

                this.dChildFab3.Click += this.deleteSelectedSaleInSummaryFab_Clicked;
                this.dChildFab4.Click += this.unCheckAllSaleInSummaryFab_Clicked;
                this.dChildFab5.Click += this.checkAllSaleInSummaryFab_Clicked;
                this.dSearchFab.Click += this.findSaleInSummaryFab_Clicked;
            }
            else
            {
                this.dChildFab1LayoutParams.RightMargin = this.dChildFab1LayoutParams.RightMargin + (int)(this.dChildFab1LayoutParams.RightMargin * .60);
                this.dChildFab1LayoutParams.BottomMargin = this.dChildFab1LayoutParams.BottomMargin * 5;
                this.dChildFab1.LayoutParameters = this.dChildFab1LayoutParams;
                this.dChildFab1.Animation = this.childFab_show;
                this.dChildFab1.Clickable = true;

                this.tvChildFab1LayoutParams.RightMargin = this.tvChildFab1LayoutParams.RightMargin + (int)(tvChildFab1LayoutParams.RightMargin * 4);
                this.tvChildFab1LayoutParams.BottomMargin = this.tvChildFab1LayoutParams.BottomMargin * 5 + (int)(tvChildFab1LayoutParams.BottomMargin * .4);
                this.tvChildFab1.LayoutParameters = this.tvChildFab1LayoutParams;
                this.tvChildFab1.Animation = this.childFab_show;
                this.tvChildFab1.Clickable = true;

                this.dChildFab2LayoutParams.RightMargin = this.dChildFab2LayoutParams.RightMargin + (int)(this.dChildFab2LayoutParams.RightMargin * .60);
                this.dChildFab2LayoutParams.BottomMargin = this.dChildFab2LayoutParams.BottomMargin * 8;
                this.dChildFab2.LayoutParameters = this.dChildFab2LayoutParams;
                this.dChildFab2.Animation = this.childFab_show;
                this.dChildFab2.Clickable = true;

                this.tvChildFab2LayoutParams.RightMargin = this.tvChildFab2LayoutParams.RightMargin + (int)(tvChildFab2LayoutParams.RightMargin * 4);
                this.tvChildFab2LayoutParams.BottomMargin = this.tvChildFab2LayoutParams.BottomMargin * 8 + (int)(tvChildFab2LayoutParams.BottomMargin * .4);
                this.tvChildFab2.LayoutParameters = this.tvChildFab2LayoutParams;
                this.tvChildFab2.Animation = this.childFab_show;
                this.tvChildFab2.Clickable = true;

                this.dChildFab3LayoutParams.RightMargin = this.dChildFab3LayoutParams.RightMargin + (int)(this.dChildFab3LayoutParams.RightMargin * .60);
                this.dChildFab3LayoutParams.BottomMargin = this.dChildFab3LayoutParams.BottomMargin * 11;
                this.dChildFab3.LayoutParameters = this.dChildFab3LayoutParams;
                this.dChildFab3.Animation = this.childFab_show;
                this.dChildFab3.Clickable = true;

                this.tvChildFab3LayoutParams.RightMargin = this.tvChildFab3LayoutParams.RightMargin + (int)(tvChildFab3LayoutParams.RightMargin * 4);
                this.tvChildFab3LayoutParams.BottomMargin = this.tvChildFab3LayoutParams.BottomMargin * 11 + (int)(tvChildFab3LayoutParams.BottomMargin * .4);
                this.tvChildFab3.LayoutParameters = this.tvChildFab3LayoutParams;
                this.tvChildFab3.Animation = this.childFab_show;
                this.tvChildFab3.Clickable = true;

                this.dChildFab4LayoutParams.RightMargin = this.dChildFab4LayoutParams.RightMargin + (int)(this.dChildFab4LayoutParams.RightMargin * .60);
                this.dChildFab4LayoutParams.BottomMargin = this.dChildFab4LayoutParams.BottomMargin * 14;
                this.dChildFab4.LayoutParameters = this.dChildFab4LayoutParams;
                this.dChildFab4.Animation = this.childFab_show;
                this.dChildFab4.Clickable = true;

                this.tvChildFab4LayoutParams.RightMargin = this.tvChildFab4LayoutParams.RightMargin + (int)(tvChildFab4LayoutParams.RightMargin * 4);
                this.tvChildFab4LayoutParams.BottomMargin = this.tvChildFab4LayoutParams.BottomMargin * 14 + (int)(tvChildFab4LayoutParams.BottomMargin * .4);
                this.tvChildFab4.LayoutParameters = this.tvChildFab4LayoutParams;
                this.tvChildFab4.Animation = this.childFab_show;
                this.tvChildFab4.Clickable = true;

                this.dChildFab5LayoutParams.RightMargin = this.dChildFab5LayoutParams.RightMargin + (int)(this.dChildFab5LayoutParams.RightMargin * .60);
                this.dChildFab5LayoutParams.BottomMargin = this.dChildFab5LayoutParams.BottomMargin * 17;
                this.dChildFab5.LayoutParameters = this.dChildFab5LayoutParams;
                this.dChildFab5.Animation = this.childFab_show;
                this.dChildFab5.Clickable = true;

                this.tvChildFab5LayoutParams.RightMargin = this.tvChildFab5LayoutParams.RightMargin + (int)(tvChildFab5LayoutParams.RightMargin * 4);
                this.tvChildFab5LayoutParams.BottomMargin = this.tvChildFab5LayoutParams.BottomMargin * 17 + (int)(tvChildFab5LayoutParams.BottomMargin * .4);
                this.tvChildFab5.LayoutParameters = this.tvChildFab5LayoutParams;
                this.tvChildFab5.Animation = this.childFab_show;
                this.tvChildFab5.Clickable = true;

                this.dSearchFabLayoutParams.RightMargin = this.dSearchFabLayoutParams.RightMargin + (int)(this.dSearchFabLayoutParams.RightMargin * .60);
                this.dSearchFabLayoutParams.BottomMargin = this.dSearchFabLayoutParams.BottomMargin * 20;
                this.dSearchFab.LayoutParameters = this.dSearchFabLayoutParams;
                this.dSearchFab.Animation = this.childFab_show;
                this.dSearchFab.Clickable = true;

                this.etSearchFabLayoutParams.RightMargin = this.etSearchFabLayoutParams.RightMargin + (int)(etSearchFabLayoutParams.RightMargin * 4);
                this.etSearchFabLayoutParams.BottomMargin = this.etSearchFabLayoutParams.BottomMargin * 20 + (int)(etSearchFabLayoutParams.BottomMargin * .4);
                this.etSearchFab.LayoutParameters = this.etSearchFabLayoutParams;
                this.etSearchFab.Animation = this.childFab_show;
                this.etSearchFab.Clickable = true;

                this.fabAnimationWatcher = new System.Threading.Thread(new ThreadStart(delegate
                {
                    System.Threading.Thread.Sleep(350);
                    this.hasAction = false;
                }));
                this.fabAnimationWatcher.Start();

                this.dChildFab1.Click += this.displaySaleDetailInSummaryFab_Clicked;
                this.dChildFab2.Click += this.editSelectedSaleInSummaryFab_Clicked;
                this.dChildFab3.Click += this.deleteSelectedSaleInSummaryFab_Clicked;
                this.dChildFab4.Click += this.unCheckAllSaleInSummaryFab_Clicked;
                this.dChildFab5.Click += this.checkAllSaleInSummaryFab_Clicked;
                this.dSearchFab.Click += this.findSaleInSummaryFab_Clicked;
            }

            this.dMainFab.Click += this.showMoreSummary_Clicked;
            dynamicPopupMenuView.Click += this.hideDynamicPopupMenu_Clicked;
        }
        public void salesReportMainFab_Clicked(object sender, EventArgs e)
        {
            if(this.salesReportItemDisplay[0].type == 1)
            {
                //Sales By Date
                this.getSalesReportByDate(this.srfFrom, this.srfTo, true);
            }
            else if(this.salesReportItemDisplay[0].type == 2)
            {
                //Sales By Consumer
                this.getSalesReportByConsumer(this.srfFrom, this.srfTo, this.srfConsumer, true);
            }
            else
            {
                //Sales By Vendor
                this.getSalesReportByVendor(this.srfFrom, this.srfTo, this.srfVendor, true);
            }
        }
        /* --------------------------------------END OF SALES-----------------------------------*/
        /* ------------------------------------------INVENTORY----------------------------------*/
        public void evaluateFilterCriteria()
        {
            //Clear item list display if oldfilter value is not equal to new filtered value
            if (!etSearch.Text.Trim().ToString().Equals(this.oldFilterValue.ToUpper()))
            {
                this.itemListDisplay.Clear();
                this.listItemAdapter.resetInstance();
                currentRetrieveItems = 0;
                if (etSearch.Text.Trim().ToString().Equals(""))
                {
                    this.filterValue = "";
                    this.oldFilterValue = "";
                }
                else
                {
                    this.filterValue = etSearch.Text.Trim().ToString().ToUpper();
                    this.oldFilterValue = this.filterValue.ToUpper();
                }
            }
            else
            {
                this.filterValue = etSearch.Text.Trim().ToString().ToUpper();
                this.oldFilterValue = this.filterValue.ToUpper();
            }
            
        }
        public void getItems(string command)
        {
            Response getItemCount = new Response();
            Response getItems = new Response();
            Item getItemCountA = new Item();
            Item getItemsA = new Item();

            this.invokeLoader();

            getItemCount = getItemCountA.GetItemCount("my_store.db");
            if (getItemCount.status.Equals("SUCCESS"))
            {
               
                if (getItemCount.param1 == this.itemListDisplay.Count)
                {
                    if (tvIndicator != null)
                    {
                        this.isAllowedToCloseDialog = true;
                        object sender = new object();
                        EventArgs e = new EventArgs();
                        this.hidePopupMenuView_Clicked(sender, e);
                        this.showSnackBar(string.Format("No item has been retrieved."));
                        this.hideLoader();
                    }
                    else
                    {
                        this.hideLoader();
                        return;
                    }
                }

                getItems = getItemsA.GetItems(command, "my_store.db", currentRetrieveItems);
                if (getItems.status.Equals("SUCCESS"))
                {
                    if (tvIndicator != null)
                    {
                        this.isAllowedToCloseDialog = true;
                        object sender = new object();
                        EventArgs e = new EventArgs();
                        this.hidePopupMenuView_Clicked(sender, e);
                        this.showSnackBar(string.Format("{0} item/s have been retrieved.", getItems.itemList.Count));
                    }
                    foreach (Item itemModel in getItems.itemList)
                    {
                        this.itemListDisplay.Add(new ListItem() { Id = itemModel.Id, IsChecked = false, Description = itemModel.NAME.ToUpper() + "(" + itemModel.AvailableStock.ToString() + (itemModel.AvailableStock > 1 ? " PCS)(" : " PC)(") + this.formatCurrency(itemModel.RetailPrice.ToString()) + ")" });
                    }

                    if (listItemAdapter == null)
                    {
                        listItemAdapter = new ListItemAdapter(this, this.itemListDisplay);
                        lvItems.Adapter = listItemAdapter;
                    }
                    //Update list of item in ListView Adapter
                    RunOnUiThread(() =>
                    {
                        if (listItemAdapter != null)
                            listItemAdapter.NotifyDataSetChanged();
                        currentRetrieveItems = this.itemListDisplay.Count;
                    });
                    this.hideLoader();
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
        public void resetItemList()
        {
            this.itemListDisplay.Clear();

            this.RunOnUiThread(() =>
            {
                if (this.listItemAdapter != null)
                {
                    this.listItemAdapter.resetInstance();
                    this.listItemAdapter.NotifyDataSetChanged();
                }
                this.currentRetrieveItems = 0;
            });

            if (this.filterValue.Trim().Equals(""))
            {
                this.getItems("SELECT * FROM Item");
            }
            else
            {
                this.getItems(string.Format("SELECT * FROM Item WHERE NAME LIKE '%{0}%' OR BARCODE LIKE '%{1}%'", this.filterValue, this.filterValue));
            }
        }
        public void hidePopupMenuView_Clicked(object sender, EventArgs e)
        {
            if (this.fabAnimationWatcher != null)
                this.fabAnimationWatcher = null;

            if (this.popupMenuDialog != null)
            {
                if (this.popupMenuDialog.IsShowing)
                {
                    if (isAllowedToCloseDialog)
                    {
                        if (!this.oldFilterValue.Trim().Equals("") && this.etSearch.Text.Trim().Equals(""))
                        {
                            if (this.hasAction)
                                return;

                            this.hasAction = true;

                            this.itemListDisplay.Clear();
                            this.oldFilterValue = "";
                            this.RunOnUiThread(() =>
                            {
                                if (listItemAdapter != null)
                                {
                                    listItemAdapter.resetInstance();
                                    listItemAdapter.NotifyDataSetChanged();
                                }
                                this.currentRetrieveItems = 0;
                            });
                            this.getItems("SELECT * FROM Item");
                        }

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
                                if ((int)etSearch.Tag == 1)
                                {
                                    etSearch.Animation = childFab_hide;
                                    etSearch.StartAnimation(etSearch.Animation);
                                    searchFab.Animation = childFab_hide;
                                    searchFab.StartAnimation(searchFab.Animation);
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
                                etSearch.Tag = 0;
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
            }
        }
        public void searchFab_Clicked(object sender, EventArgs e)
        {
            if (this.hasAction)
                return;

            this.hasAction = true;

            this.evaluateFilterCriteria();

            if (requestType.Equals("Show Items"))
            {
                if (this.filterValue.Trim().Equals(""))
                {
                    this.getItems("SELECT * FROM Item");
                }
                else
                {
                    this.getItems(string.Format("SELECT * FROM Item WHERE NAME LIKE '{0}%' OR BARCODE LIKE '{1}%'", this.filterValue.ToUpper(), this.filterValue.ToUpper()));
                }
            }
        }
        public void uncheckAllFab_Clicked(object sender, EventArgs e)
        {
            int count = 0;
            if (this.itemListDisplay.Count > 0)
            {
                if (this.hasAction)
                    return;

                this.hasAction = true;

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
                            if (listItemAdapter != null)
                                listItemAdapter.NotifyDataSetChanged();
                        }
                    }
                    this.hasAction = false;
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
                if (this.hasAction)
                    return;

                this.hasAction = true;

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
                            if (listItemAdapter != null)
                                listItemAdapter.NotifyDataSetChanged();
                        }
                    }
                    this.hasAction = false;
                });
            }
            else
            {
                tvIndicator.Text = "Item list were all deleted.";
            }
        }
        public void deleteSelectedFab_Clicked(object sender, EventArgs e)
        {
            if (this.hasAction)
                return;

            if (listItemAdapter.getSelectedItems().Count == 0)
                return;

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
                    this.hasAction = true;
                    tvIndicator.Visibility = ViewStates.Visible;
                    isAllowedToCloseDialog = false;
                    foreach (ListItem listItem in listItemAdapter.getSelectedItems())
                    {
                        tvIndicator.Text = string.Format("Preparing in deleting {0}...", listItem.Description.Split('(')[0]);
                        command += string.Format("DELETE FROM Item WHERE Id = {0} AND (SELECT COUNT(Id) FROM SaleMaster) = 0;", listItem.Id);
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
                            if (listItemAdapter != null)
                                listItemAdapter.NotifyDataSetChanged();
                        });
                        this.hasAction = false;
                    }
                    else
                    {
                        this.itemListDisplay.AddRange(deletedItems);
                        this.tvIndicator.Text = "Item(Delete): Error has occured.";
                        this.hasAction = false;
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
                this.hasAction = false;
            }
        }
        public void editSelectedFab_Clicked(object sender, EventArgs e)
        {
            if (this.hasAction)
                return;

            if (listItemAdapter.getSelectedItems().Count == 0)
                return;

            this.hasAction = true;

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
                svInventory.Visibility = ViewStates.Visible;
                this.showOptionMenu(Resource.Id.action_more);
                this.hasAction = false;
            }
            else
            {
                tvIndicator.Text = "Item(Selected): An error has occured.";
                isAllowedToCloseDialog = true;
                this.hasAction = false;
            }

        }
        public void showDetailFab_Clicked(object sender, EventArgs e)
        {
            if (this.hasAction)
                return;

            if (listItemAdapter.getSelectedItems().Count == 0)
                return;

            this.hasAction = true;

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
                this.hasAction = false;
            }
            else
            {
                tvIndicator.Text = "Item(Selected): An error has occured.";
                isAllowedToCloseDialog = true;
                this.hasAction = false;
            }
        }
        public void mainFab_Clicked(object sender, EventArgs e)
        {
            if (this.hasAction)
                return;

            this.hasAction = true;

            this.evaluateFilterCriteria();

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
            if (this.hasAction)
                return;

            Item itemModel = new Item();
            if (itemModel.GetItemCount("my_store.db").param1 > 0)
            {
                this.hasAction = true;
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

                searchFab = popupMenuView.FindViewById<com.refractored.fab.FloatingActionButton>(Resource.Id.searchFab);
                searchFabLayoutParams = (FrameLayout.LayoutParams)searchFab.LayoutParameters;

                etSearch = popupMenuView.FindViewById<EditText>(Resource.Id.etSearch);
                etSearchLayoutParams = (FrameLayout.LayoutParams)etSearch.LayoutParameters;
                etSearch.SetHintTextColor(Android.Graphics.Color.ParseColor("#616161"));
                tvShowMoreLayoutParams.RightMargin = tvShowMoreLayoutParams.RightMargin + (int)(tvShowMoreLayoutParams.RightMargin * 4);
                tvShowMoreLayoutParams.BottomMargin = tvShowMoreLayoutParams.BottomMargin * 2;
                tvShowMore.Animation = childFab_show;
                tvShowMore.Clickable = true;
                etSearch.Text = this.oldFilterValue;

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

                    searchFabLayoutParams.RightMargin = searchFabLayoutParams.RightMargin + (int)(searchFabLayoutParams.RightMargin * .60);
                    searchFabLayoutParams.BottomMargin = searchFabLayoutParams.BottomMargin * 14;
                    searchFab.LayoutParameters = searchFabLayoutParams;
                    searchFab.Size = FabSize.Mini;
                    searchFab.Animation = childFab_show;
                    searchFab.Clickable = true;

                    etSearchLayoutParams.RightMargin = etSearchLayoutParams.RightMargin + (int)(etSearchLayoutParams.RightMargin * 4);
                    etSearchLayoutParams.BottomMargin = etSearchLayoutParams.BottomMargin * 14 + (int)(etSearchLayoutParams.BottomMargin * .4);
                    etSearch.LayoutParameters = etSearchLayoutParams;
                    etSearch.Animation = childFab_show;

                    tvDeleteSelected.Tag = 1;
                    tvCheckAll.Tag = 1;
                    tvUncheckAll.Tag = 1;
                    tvShowMore.Tag = 1;
                    etSearch.Tag = 1;
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

                    searchFabLayoutParams.RightMargin = searchFabLayoutParams.RightMargin + (int)(searchFabLayoutParams.RightMargin * .60);
                    searchFabLayoutParams.BottomMargin = searchFabLayoutParams.BottomMargin * 20;
                    searchFab.LayoutParameters = searchFabLayoutParams;
                    searchFab.Size = FabSize.Mini;
                    searchFab.Animation = childFab_show;
                    searchFab.Clickable = true;

                    etSearchLayoutParams.RightMargin = etSearchLayoutParams.RightMargin + (int)(etSearchLayoutParams.RightMargin * 4);
                    etSearchLayoutParams.BottomMargin = etSearchLayoutParams.BottomMargin * 20 + (int)(etSearchLayoutParams.BottomMargin * .4);
                    etSearch.LayoutParameters = etSearchLayoutParams;
                    etSearch.Animation = childFab_show;

                    tvDeleteSelected.Tag = 1;
                    tvEditSelected.Tag = 1;
                    tvCheckAll.Tag = 1;
                    tvUncheckAll.Tag = 1;
                    tvShowDetail.Tag = 1;
                    tvShowMore.Tag = 1;
                    etSearch.Tag = 1;
                }

                this.fabAnimationWatcher = new System.Threading.Thread(new ThreadStart(delegate
                {
                    System.Threading.Thread.Sleep(350);
                    this.hasAction = false;
                }));
                this.fabAnimationWatcher.Start();

                popupMenuView.Click += this.hidePopupMenuView_Clicked;
                this.searchFab.Click += this.searchFab_Clicked;
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
                if (oldStock <= 0)
                {
                    return ((oldPrice * 0) + ((newStock - 0) * newPrice)) / newStock;
                }
                else
                {
                    return ((oldPrice * oldStock) + ((newStock - oldStock) * newPrice)) / newStock;
                }
            }
            else if (oldStock == Convert.ToInt16(etStocks.Text))
            {
                return newPrice;
            }
            else
            {
                return ((oldPrice * oldStock) + ((oldStock - newStock) * newPrice)) / newStock;
            }
        }
        public void etPurchasedPrice_ItemChanged(object sender, View.KeyEventArgs e)
        {
            e.Handled = false;
            if (this.evaluationType.Equals("Save"))
            {
                if (!this.etPurchasedPrice.Text.Equals("") && (!this.etStocks.Text.Equals("")))
                    this.etAvarageCost.Text = this.etPurchasedPrice.Text;
            }
            else
            {
                if (!etPurchasedPrice.Text.Equals("") && (!etStocks.Text.Equals("")))
                {
                    this.etAvarageCost.Text = this.getAverageCost(this.oldStock, Convert.ToInt16(etStocks.Text), this.oldPrice, Convert.ToDouble(this.etPurchasedPrice.Text)).ToString();
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
            this.hideKeyBoard();
        }
        public void btnEvaluateItem_Clicked(object sender, EventArgs e)
        {
            if (this.hasAction)
                return;

            string validate = checkIfValid(actvItemName.Text, etPurchasedPrice.Text, etRetailPrice.Text, etStocks.Text);
            if (validate.Equals(""))
            {
                if (evaluationType.Equals("Save"))
                {
                    this.invokeLoader();
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
                        if (!etPurchasedPrice.Text.Equals("") && (!etStocks.Text.Equals("")))
                            etAvarageCost.Text = etPurchasedPrice.Text;

                        //Save item
                        command = string.Format("INSERT INTO Item(Barcode, NAME, PurchasedPrice, RetailPrice, AvailableStock, AverageCost) VALUES('{0}', '{1}', {2}, {3}, {4}, {5});"
                            , etBarcode.Text.Trim()
                            , actvItemName.Text.Trim().Replace("'", "/").ToUpper()
                            , Convert.ToDouble(etPurchasedPrice.Text.Trim())
                            , Convert.ToDouble(etRetailPrice.Text.Trim())
                            , Convert.ToInt16(etStocks.Text.Trim())
                            , Convert.ToDouble(etAvarageCost.Text.Trim())) + "\n";
                        saveItem = sqliteADO.ExecuteNonQuery("BEGIN TRANSACTION;" + "\n" + command + "COMMIT;", dbPath);
                        if (saveItem.status.Equals("SUCCESS"))
                        {
                            this.showSnackBarInfinite("Successfully Saved.");
                            getId = itemModel.GetLastId("my_store.db");
                            if (getId.status.Equals("SUCCESS"))
                            {
                                this.itemListDisplay.Add(new ListItem() { Id = getId.param1, IsChecked = false, Description = actvItemName.Text.ToUpper() + "(" + this.etStocks.Text.ToString() + (Convert.ToInt16(this.etStocks.Text) > 1 ? " PCS)(" : " PC)(") + this.formatCurrency(this.etRetailPrice.Text.ToString()) + ")" });
                                this.hideLayouts();
                                currentRetrieveItems = this.itemListDisplay.Count;
                                evaluationType = "ShowItemList";
                                flInvetoryList.Visibility = ViewStates.Visible;
                                this.showOptionMenu(Resource.Id.action_more);
                                this.showOptionMenu(Resource.Id.action_refresh);
                                //Update list of item in ListView Adapter
                                RunOnUiThread(() =>
                                {
                                    if (listItemAdapter == null)
                                    {
                                        listItemAdapter = new ListItemAdapter(this, this.itemListDisplay);
                                        lvItems.Adapter = listItemAdapter;
                                        this.resetItem();
                                    }
                                    else
                                    {
                                        listItemAdapter.NotifyDataSetChanged();
                                        this.resetItem();
                                    }
                                });
                                this.hideLoader();
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
                    messageDialog.SetMessage("Are you sure you want to update Item " + actvItemName.Text.ToUpper() + "?");
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
                        this.invokeLoader();
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
                            if (!etPurchasedPrice.Text.Equals("") && (!etStocks.Text.Equals("")))
                            {
                                this.etAvarageCost.Text = this.getAverageCost(this.oldStock, Convert.ToInt16(etStocks.Text), this.oldPrice, Convert.ToDouble(etPurchasedPrice.Text)).ToString();     
                            }

                            if (scannedState)
                            {
                                for(int i = 0; i < this.itemListDisplay.Count; i++)
                                {
                                    if(this.itemListDisplay[i].Id == this.itemHolder.Id)
                                    {
                                        position = i;
                                        command = string.Format("UPDATE Item SET Barcode = '{0}', NAME = '{1}', PurchasedPrice = {2} , RetailPrice = {3}, AvailableStock = {4}, AverageCost = {5} WHERE Id = {6};"
                                            , etBarcode.Text.Trim()
                                            , actvItemName.Text.Trim().Replace("'", "/").ToUpper()
                                            , Convert.ToDouble(etPurchasedPrice.Text.Trim())
                                            , Convert.ToDouble(etRetailPrice.Text.Trim())
                                            , Convert.ToInt16(etStocks.Text.Trim())
                                            , Convert.ToDouble(etAvarageCost.Text.Trim())
                                            , this.itemListDisplay[i].Id) + "\n";
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                command = string.Format("UPDATE Item SET Barcode = '{0}', NAME = '{1}', PurchasedPrice = {2} , RetailPrice = {3}, AvailableStock = {4}, AverageCost = {5} WHERE Id = {6};"
                                    , etBarcode.Text.Trim()
                                    , actvItemName.Text.Trim().Replace("'", "/")
                                    , Convert.ToDouble(etPurchasedPrice.Text.Trim())
                                    , Convert.ToDouble(etRetailPrice.Text.Trim())
                                    , Convert.ToInt16(etStocks.Text.Trim())
                                    , Convert.ToDouble(etAvarageCost.Text.Trim())
                                    , this.listItemAdapter.getSelectedItems()[0].Id) + "\n";
                            }

                            //Update item
                            saveItem = sqliteADO.ExecuteNonQuery("BEGIN TRANSACTION;" + "\n" + command + "COMMIT;", dbPath);
                            if (saveItem.status.Equals("SUCCESS"))
                            {
                                if (scannedState)
                                {
                                    if (position != -1)
                                    {
                                        this.itemListDisplay[position].Description = actvItemName.Text.ToUpper() + "(" + this.etStocks.Text.ToString() + (Convert.ToInt16(this.etStocks.Text) > 1 ? " PCS)(" : " PC)(") + this.formatCurrency(this.etRetailPrice.Text.ToString()) + ")";
                                    }
                                }
                                else
                                {
                                    this.itemListDisplay[listItemAdapter.indexOfSelecteItems()[0]].Description = actvItemName.Text + "(" + this.etStocks.Text.ToString() + (Convert.ToInt16(this.etStocks.Text) > 1 ? " PCS)(" : " PC)(") + this.formatCurrency(this.etRetailPrice.Text.ToString()) + ")";
                                }

                                this.showSnackBar("Successfully Updated.");
                                currentRetrieveItems = this.itemListDisplay.Count;
                                this.hideLayouts();
                                evaluationType = "ShowItemList";
                                flInvetoryList.Visibility = ViewStates.Visible;
                                this.showOptionMenu(Resource.Id.action_more);
                                this.showOptionMenu(Resource.Id.action_refresh);
                                //Update list of item in ListView Adapter
                                RunOnUiThread(() =>
                                {
                                    if (listItemAdapter != null)
                                        listItemAdapter.NotifyDataSetChanged();
                                });
                                this.hideLoader();
                                this.resetItem();
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
            this.hideKeyBoard();
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
                    svInventory.Visibility = ViewStates.Visible;
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
                    svInventory.Visibility = ViewStates.Visible;
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
            actvItemName.RequestFocus();
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
    }
}

