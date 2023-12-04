using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Data;
using MySql.Data.MySqlClient;
using System.Threading;
using System.Windows.Threading;

namespace Cars_Inventory
{
    /// <summary>
    /// Interaction logic for WindowItemList.xaml
    /// </summary>
    public partial class WindowItemListByItem : Window
    {
        public static bool ForceRefresh = false;
        private myConn m = new myConn();
        private MySqlConnection myConn; // Mysql Connection
        private MySqlCommand myCmd; // MySql Command
        private MySqlDataReader myDr; // MySql Data Reader
        private List<storageItemData> FullItemList = new List<storageItemData>();
        private List<storageCentre> FullCentreList = new List<storageCentre>();
        private List<storageCacheItemCentreQuantity> CachedItemCentreQuantity = new List<storageCacheItemCentreQuantity>();

        public WindowItemListByItem()
        {
            InitializeComponent();
            try
            {
                myConn = new MySqlConnection(m.Setting); // Create MySQL Connection
                LoadProductGroup(); // display the product group
                FullCentreList = PublicShare.LoadCentreList();
                loadingLabel.Visibility = Visibility.Hidden;
                buttonExportExcel.IsEnabled = false;
                buttonDetailsData.IsEnabled = false;
            }
            catch (Exception ex)
            {
                using (WindowDisplayMessage wdm = new WindowDisplayMessage(ex.Message, 2))
                {
                    wdm.ShowDialog();
                    wdm.Dispose();
                }
            }
        }

        private void LoadProductGroup()
        {
            try
            {
                listBoxItemGroup.DataContext = PublicShare.LoadItemGroupList();
                listBoxItemGroup.DisplayMemberPath = "ItemGroupName";
                listBoxItemGroup.Items.Refresh();
            }
            catch (Exception ex)
            {
                using (WindowDisplayMessage wdm = new WindowDisplayMessage(ex.Message, 2))
                {
                    wdm.ShowDialog();
                    wdm.Dispose();
                    wdm.Owner = this;
                }
            }
        }
        
        private void listBoxItemGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listBoxItemGroupChanging();
        }

        private void listBoxItemGroupChanging()
        {
            try
            {
                if (listBoxItemGroup.SelectedIndex > -1)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback((obj) =>
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            listBoxItemGroup.IsEnabled = false;
                            dataGridSiteQuantityList.IsEnabled = false;
                            loadingLabel.Visibility = Visibility.Visible;
                            buttonExportExcel.IsEnabled = false;
                            buttonDetailsData.IsEnabled = false;
                            buttonRemoveCache.IsEnabled = false;
                        }));
                        Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                        {
                            LoadingQuantityList();
                            loadingLabel.Visibility = Visibility.Hidden;
                            listBoxItemGroup.IsEnabled = true;
                            dataGridSiteQuantityList.IsEnabled = true;
                            buttonExportExcel.IsEnabled = true;
                            buttonDetailsData.IsEnabled = true;
                            buttonRemoveCache.IsEnabled = true;
                        }));
                    }));
                }
            }
            catch (Exception ex)
            {
                using (WindowDisplayMessage wdm = new WindowDisplayMessage(ex.Message, 2))
                {
                    wdm.ShowDialog();
                    wdm.Dispose();
                    wdm.Owner = this;
                }
            }
        }

        private void LoadingQuantityList()
        {
            try
            {
                dataGridSiteQuantityList.ItemsSource = null; // generally no required
                dataGridSiteQuantityList.Items.Clear(); // generally no required
                dataGridSiteQuantityList.Items.Refresh(); // generally no required
                storageItemGroup SelectedItemGroup = listBoxItemGroup.SelectedItem as storageItemGroup;
                if ((from c in CachedItemCentreQuantity // take from CachedItemCentreQuantity
                     where c.Index.Equals(listBoxItemGroup.SelectedIndex) // validate if the selectedindex is cached
                     select c).Count().Equals(0)) // and return its cached data quantity
                {
                    if (SelectedItemGroup.Equals(null).Equals(false))
                    {
                        List<storageItemCentreQuantity> sICQ = new List<storageItemCentreQuantity>();
                        string ProductServiceCode = PublicShare.GetServiceCodeFromProductGroup(SelectedItemGroup.IDItemGroup);
                        foreach (storageCentre Temp in FullCentreList.OrderBy(x => x.siteName))
                        {
                            int Quantity = getItemQuantity(ProductServiceCode, Temp.site.ToString()),
                                StockInQuantity = getItemStockInQuantity(ProductServiceCode, Temp.site.ToString()),
                                OnHandQuantity = StockInQuantity - Quantity;
                            sICQ.Add(new storageItemCentreQuantity { Site = Temp.site, CentreName = Temp.siteName, Qty = Quantity, StockIn = StockInQuantity, OnHand = OnHandQuantity });
                            CachedItemCentreQuantity.Add(new storageCacheItemCentreQuantity { Index = listBoxItemGroup.SelectedIndex, Site = Temp.site, CentreName = Temp.siteName, Qty = Quantity, StockIn = StockInQuantity, OnHand = OnHandQuantity }); // make it as cached data
                        }
                        dataGridSiteQuantityList.ItemsSource = sICQ; // Order by centre name asc
                        dataGridSiteQuantityList.Items.Refresh();
                    }
                }
                else // Reading from cache since is cached
                {
                    dataGridSiteQuantityList.ItemsSource = (from x in CachedItemCentreQuantity // get from cached data
                                                            where x.Index.Equals(listBoxItemGroup.SelectedIndex) // select all data that match selected index
                                                            select new storageItemCentreQuantity { Site = x.Site, CentreName = x.CentreName, Qty = x.Qty, StockIn = x.StockIn, OnHand = x.OnHand }).ToList(); // select to list
                    dataGridSiteQuantityList.Items.Refresh();
                }
                dataGridSiteQuantityList.Columns[0].Visibility = Visibility.Hidden; // hide 1st row, site id
                dataGridSiteQuantityList.Columns[1].Header = "Centre";
                dataGridSiteQuantityList.Columns[2].Header = "Qty Sold";
                dataGridSiteQuantityList.Columns[3].Header = "Qty Stock-In";
                dataGridSiteQuantityList.Columns[4].Header = "Qty On-Hand";
            }
            catch (Exception ex)
            {
                using (WindowDisplayMessage wdm = new WindowDisplayMessage(ex.Message, 2))
                {
                    wdm.ShowDialog();
                    wdm.Dispose();
                    wdm.Owner = this;
                }
            }
        }

        private int getItemStockInQuantity(string ServiceCode, string Site)
        {
            int iQuantity = 0;
            try
            {
                if (myConn.State.Equals(ConnectionState.Closed)) { myConn.Open(); }
                using (myCmd = new MySqlCommand(string.Format(@"SELECT COALESCE(SUM(`Qty`), 0) FROM `cars_item_stockin` WHERE `Active` = 'Y' AND `FKIDSite` = '{0}' AND `FKIDServiceCode` IN ({1});", Site, ServiceCode), myConn))
                {
                    myCmd.CommandTimeout = 0;
                    using (myDr = myCmd.ExecuteReader())
                    {
                        if (myDr.Read()) { iQuantity = myDr.GetInt32(0); }
                    }
                }
            }
            catch (Exception ex)
            {
                using (WindowDisplayMessage wdm = new WindowDisplayMessage(ex.Message, 2))
                {
                    wdm.ShowDialog();
                    wdm.Dispose();
                    wdm.Owner = this;
                }
            }
            finally { if (myConn.State.Equals(ConnectionState.Open)) { myConn.Close(); } }
            return iQuantity;
        }
        
        private int getItemQuantity(string ServiceCode, string Site)
        {
            int iQuantity = 0;
            try
            {
                if (myConn.State.Equals(ConnectionState.Closed)) { myConn.Open(); }
                using (myCmd = new MySqlCommand(string.Format(@"SELECT COALESCE(COUNT(*), 0) FROM `tbljobsheetservice` WHERE `ServiceCode` IN ({0}) AND `Site` = '{1}';", ServiceCode, Site), myConn))
                {
                    myCmd.CommandTimeout = 0;
                    using (myDr = myCmd.ExecuteReader())
                    {
                        iQuantity = (myDr.Read()) ? myDr.GetInt32(0) : 0;
                    }
                }
            }
            catch (Exception ex)
            {
                using (WindowDisplayMessage wdm = new WindowDisplayMessage(ex.Message, 2))
                {
                    wdm.ShowDialog();
                    wdm.Dispose();
                    wdm.Owner = this;
                }
            }
            finally { if (myConn.State.Equals(ConnectionState.Open)) { myConn.Close(); } }
            return iQuantity;
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        
        private void buttonDetailsData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (listBoxItemGroup.SelectedIndex.Equals(-1) || dataGridSiteQuantityList.SelectedIndex.Equals(-1)) { throw new Exception("Please select item to generate data"); }
                storageItemGroup sig = listBoxItemGroup.SelectedItem as storageItemGroup;
                storageItemCentreQuantity sicq = dataGridSiteQuantityList.SelectedItem as storageItemCentreQuantity;
                using (WindowItemDataDetails widd = new WindowItemDataDetails(sig.IDItemGroup, sicq.Site, sicq.Qty, sicq.StockIn))
                {
                    widd.ShowDialog();
                    widd.Dispose();
                    widd.Owner = this;
                }
                if (PublicShare.ForceItemRefresh.Equals(true))
                {
                    CachedItemCentreQuantity = new List<storageCacheItemCentreQuantity>();
                    PublicShare.ForceItemRefresh = false;
                    listBoxItemGroupChanging();
                }
            }
            catch (Exception ex)
            {
                using (WindowDisplayMessage wdm = new WindowDisplayMessage(ex.Message, 3))
                {
                    wdm.ShowDialog();
                    wdm.Dispose();
                    wdm.Owner = this;
                }
            }
        }

        private void buttonExportExcel_Click(object sender, RoutedEventArgs e)
        {
            Excel._Application appExcel = new Excel.Application(); // creating Excel Application
            try
            {
                if (listBoxItemGroup.SelectedIndex.Equals(-1) || dataGridSiteQuantityList.Items.Count.Equals(0)) { throw new Exception("Please generate data before proceed"); } // generally no required, but for protection
                string FileName = PublicShare.SaveFilename();
                if (string.IsNullOrEmpty(FileName)) { throw new Exception("Please specific file name and location you want to save the report"); }
                int ExcelRow = 0;
                Excel._Workbook workbook = appExcel.Workbooks.Add(Type.Missing); // creating new WorkBook within Excel application
                Excel._Worksheet worksheet = null; // creating new Excelsheet in workbook
                appExcel.Visible = false; // Hiding Excel
                // get the reference of first sheet.
                worksheet = (Excel.Worksheet)workbook.Sheets["Sheet1"]; //  By default its name is Sheet1.
                worksheet = (Excel.Worksheet)workbook.ActiveSheet; // Active this Sheet
                worksheet.Name = "Inventory Report"; // changing the name of active sheet
                ExcelRow++;
                worksheet.Cells[ExcelRow++, 1].Value = "Inventory Report";
                worksheet.Cells[ExcelRow, 1].Value = "Product : ";
                storageItemGroup sig = listBoxItemGroup.SelectedItem as storageItemGroup;
                worksheet.Cells[ExcelRow++, 2].Value = sig.ItemGroupName;
                ExcelRow++;
                worksheet.Cells[ExcelRow, 1].EntireRow.Font.Bold = true; // Make Font Bold
                worksheet.Cells[ExcelRow, 1] = "Centre Name";
                worksheet.Cells[ExcelRow, 2] = "Qty Sold";
                worksheet.Cells[ExcelRow, 3] = "Qty Stock In";
                worksheet.Cells[ExcelRow, 4] = "Qty OnHand";
                worksheet.Cells[ExcelRow, 1].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                worksheet.Cells[ExcelRow, 2].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                worksheet.Cells[ExcelRow, 3].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                worksheet.Cells[ExcelRow, 4].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                ((Excel.Range)worksheet.Columns["A", Type.Missing]).ColumnWidth = 12;
                ((Excel.Range)worksheet.Columns["B", Type.Missing]).ColumnWidth = 14;
                ((Excel.Range)worksheet.Columns["C", Type.Missing]).ColumnWidth = 14;
                ((Excel.Range)worksheet.Columns["D", Type.Missing]).ColumnWidth = 14;
                ExcelRow++;
                for (int i = 0; i < dataGridSiteQuantityList.Items.Count; i++, ExcelRow++)
                {
                    storageItemCentreQuantity sicq = dataGridSiteQuantityList.Items[i] as storageItemCentreQuantity;
                    worksheet.Cells[ExcelRow, 1] = sicq.CentreName;
                    worksheet.Cells[ExcelRow, 2] = sicq.Qty;
                    worksheet.Cells[ExcelRow, 3] = sicq.StockIn;
                    worksheet.Cells[ExcelRow, 4] = sicq.OnHand;
                }
                workbook.SaveAs(FileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                MessageBox.Show("Excel file is successfully created");
            }
            catch (Exception ex)
            {
                using (WindowDisplayMessage wdm = new WindowDisplayMessage(ex.Message, 3))
                {
                    wdm.ShowDialog();
                    wdm.Dispose();
                    wdm.Owner = this;
                }
            }
            finally { appExcel.Quit(); }
        }

        private void buttonRemoveCache_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CachedItemCentreQuantity = new List<storageCacheItemCentreQuantity>();
                using (WindowDisplayMessage wdm = new WindowDisplayMessage("All cached data is removed..."))
                {
                    wdm.ShowDialog();
                    wdm.Dispose();
                    wdm.Owner = this;
                }
            }
            catch { }
        }

    }
}
