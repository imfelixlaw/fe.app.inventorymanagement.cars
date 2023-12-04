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

namespace Cars_Inventory
{
    /// <summary>
    /// Interaction logic for WindowItemDataDetails.xaml
    /// </summary>
    public partial class WindowItemDataDetails : Window
    {
        private int _IDItemGroup = 0, _IDSite = 0, _SellQty = 0, _StockInQty = 0;
        public WindowItemDataDetails(int IDItemGroup, int IDSite, int SellQty, int StockInQty)
        {
            InitializeComponent();
            _IDItemGroup = IDItemGroup;
            _IDSite = IDSite;
            _SellQty = SellQty;
            _StockInQty = StockInQty;
            StartUpInit();
        }

        private void StartUpInit()
        {
            try
            {
                string SCGroup = PublicShare.GetServiceCodeFromProductGroup(_IDItemGroup);
                if (_SellQty.Equals(0))
                {
                    textBlockNoSelling.Visibility = Visibility.Visible;
                }
                else
                {
                    textBlockNoSelling.Visibility = Visibility.Hidden;
                    dataGridSellingHistory.ItemsSource = PublicShare.FetchSellingHistory(SCGroup, _IDSite);
                    dataGridSellingHistory.Items.Refresh();
                }

                if (_StockInQty.Equals(0))
                {
                    textBlockNoStockInHistory.Visibility = Visibility.Visible;
                    buttonDelete.IsEnabled = false;
                }
                else
                {
                    textBlockNoStockInHistory.Visibility = Visibility.Hidden;
                    buttonDelete.IsEnabled = true;
                    dataGridStockInHistory.ItemsSource = PublicShare.FetchStockInHistory(SCGroup, _IDSite);
                    dataGridStockInHistory.Items.Refresh();
                }
                textBoxSales.Text = _SellQty.ToString();
                textBoxStockIn.Text = _StockInQty.ToString();
                textBoxOnHand.Text = (_StockInQty - _SellQty).ToString();
                if (dataGridStockInHistory.Items.Count.Equals(0)) { textBlockNoStockInHistory.Visibility = Visibility.Visible; }
                if (dataGridSellingHistory.Items.Count.Equals(0)) { textBlockNoSelling.Visibility = Visibility.Visible; }
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

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGridStockInHistory.SelectedIndex.Equals(-1)) { throw new Exception("Please select a stock in history to delete"); }
                if (MessageBox.Show("This delete the selected history, are you sure to proceed?", "Confirmation", MessageBoxButton.YesNo).Equals(MessageBoxResult.Yes))
                {
                    storageStockHistory ssh = dataGridStockInHistory.SelectedItem as storageStockHistory;
                    PublicShare.DeleteStockInHistory(ssh.IDItemStockIn);
                    StartUpInit();
                    PublicShare.ForceItemRefresh = true;
                    MessageBox.Show("Data is expired, will regenerate the data after this close.");
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
    }
}
