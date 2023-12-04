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
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.Data;

namespace Cars_Inventory
{
    /// <summary>
    /// Interaction logic for WindowCentreStockIn.xaml
    /// </summary>
    public partial class WindowCentreStockIn : Window
    {
        private myConn m = new myConn();
        private MySqlConnection myConn; // Mysql Connection
        private MySqlCommand myCmd; // MySql Command
        
        public WindowCentreStockIn()
        {
            InitializeComponent();
            try
            {
                myConn = new MySqlConnection(m.Setting); // Create MySQL Connection
                StartUpInit();
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

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void StartUpInit()
        {
            try
            {
                comboBoxCentre.DataContext = PublicShare.LoadCentreList().OrderBy(x => x.siteName);
                comboBoxCentre.DisplayMemberPath = "siteName";
                comboBoxCentre.Items.Refresh();
                comboBoxProductGroup.DataContext = PublicShare.LoadItemGroupList().OrderBy(x => x.ItemGroupName);
                comboBoxProductGroup.DisplayMemberPath = "ItemGroupName";
                comboBoxProductGroup.Items.Refresh();
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

        private void comboBoxProductGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                List<storageCacheItemData> cachedItemData = new List<storageCacheItemData>();
                if (comboBoxProductGroup.SelectedIndex > -1)
                {
                    textBlockProductGroupTips.Visibility = Visibility.Hidden;
                    storageItemGroup sig = comboBoxProductGroup.SelectedItem as storageItemGroup;
                    cachedItemData = PublicShare.LoadingCacheItemData();
                    comboBoxServiceCode.DataContext = (from x in cachedItemData // get from cached data
                                                       where x.IDItemGroup.Equals(sig.IDItemGroup) // select all data that match selected index
                                                       select new storageCacheItemData { IDItemGroup = x.IDItemGroup, serviceCode = x.serviceCode, serviceName = x.serviceName }).ToList().OrderBy(x => x.serviceName); // select to list
                    comboBoxServiceCode.DisplayMemberPath = "serviceName";
                    comboBoxServiceCode.Items.Refresh();
                }
                else
                {
                    textBlockProductGroupTips.Visibility = Visibility.Visible;
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

        private void comboBoxCentre_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                textBlockCentreTips.Visibility = comboBoxCentre.SelectedIndex.Equals(-1) ? Visibility.Visible : Visibility.Hidden;
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

        private void comboBoxServiceCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                textBlockServiceCodeTips.Visibility = comboBoxServiceCode.SelectedIndex.Equals(-1) ? Visibility.Visible : Visibility.Hidden;
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

        private void textBoxStockInQty_GotFocus(object sender, RoutedEventArgs e)
        {
            textBlockStockInQtyTips.Visibility = Visibility.Hidden;
        }

        private void textBoxStockInQty_LostFocus(object sender, RoutedEventArgs e)
        {
            textBlockStockInQtyTips.Visibility = (textBoxStockInQty.Text.Length.Equals(0)) ? Visibility.Visible : Visibility.Hidden;
        }

        private void textBoxStockInQty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
                char c = e.Text.ToCharArray().First();
                e.Handled = !(char.IsNumber(c) || char.IsControl(c));
            }
            catch { } // do not need report error
        }

        private void buttonAccept_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(comboBoxCentre.SelectedIndex.Equals(-1)) { throw new Exception("Please select a centre to continue"); } // check centre for empty
                if(comboBoxProductGroup.SelectedIndex.Equals(-1)) { throw new Exception("Please select a product group to continue"); } // check product group for empty
                if(comboBoxServiceCode.SelectedIndex.Equals(-1)) { throw new Exception("Please select a item to continue"); } // check item for empty
                if(textBoxStockInQty.Text.Length.Equals(0) || Convert.ToInt32(textBoxStockInQty.Text).Equals(0)) { throw new Exception("Quantity cannot be empty or zero"); } // check qty for empty or zero
                if(datePickerStockInDate.Text.Equals(string.Empty)) { throw new Exception("Date cannot be empty"); } // check date for empty
                storageCentre sc = comboBoxCentre.SelectedItem as storageCentre;
                storageCacheItemData scid = comboBoxServiceCode.SelectedItem as storageCacheItemData;
                if (myConn.State.Equals(ConnectionState.Closed)) { myConn.Open(); }
                using (myCmd = new MySqlCommand(string.Format(@"INSERT INTO `cars_item_stockin` VALUES (NULL, {0}, '{1}', {2}, '{3}', 1, 'Y');", sc.site, scid.serviceCode, textBoxStockInQty.Text, ((DateTime)datePickerStockInDate.SelectedDate).ToString("yyyy-MM-dd")), myConn))
                {
                    myCmd.CommandTimeout = 0;
                    myCmd.ExecuteNonQuery();
                }
                using (WindowDisplayMessage wdm = new WindowDisplayMessage("The information is added successfully"))
                {
                    wdm.ShowDialog();
                    wdm.Dispose();
                    wdm.Owner = this;
                    textBoxStockInQty.Clear();
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
        }
    }
}
