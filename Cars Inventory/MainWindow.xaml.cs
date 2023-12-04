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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;

namespace Cars_Inventory
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                myConn = new MySqlConnection(m.Setting); // Create MySQL Connection
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

        private void menuItemItemListByItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (WindowItemListByItem wILi = new WindowItemListByItem()) // create windows and dispose resource after using
                {
                    wILi.ShowDialog();
                    wILi.Dispose();
                    wILi.Owner = this;
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

        private void MenuItemProductGroup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (WindowProductGroup wPG = new WindowProductGroup())
                {
                    wPG.ShowDialog();
                    wPG.Dispose();
                    wPG.Owner = this;
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

        private void menuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(); // shut down entire application and disposed all resources
        }

        private void menuItemItemStockIn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (WindowCentreStockIn wcsi = new WindowCentreStockIn())
                {
                    wcsi.ShowDialog();
                    wcsi.Dispose();
                    wcsi.Owner = this;
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

        private void buttonSettingProductGroup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (WindowProductGroup wPG = new WindowProductGroup())
                {
                    wPG.ShowDialog();
                    wPG.Dispose();
                    wPG.Owner = this;
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

        private void buttonViewItemList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (WindowItemListByItem wILi = new WindowItemListByItem()) // create windows and dispose resource after using
                {
                    wILi.ShowDialog();
                    wILi.Dispose();
                    wILi.Owner = this;
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

        private void buttonAddStockIn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (WindowCentreStockIn wcsi = new WindowCentreStockIn())
                {
                    wcsi.ShowDialog();
                    wcsi.Dispose();
                    wcsi.Owner = this;
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

        private void buttonRptStockTake_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (WindowRptStockTake wrst = new WindowRptStockTake())
                {
                    wrst.ShowDialog();
                    wrst.Dispose();
                    wrst.Owner = this;
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

        private void buttonRptStockOrdering_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (WindowRptStockOrder wrso = new WindowRptStockOrder())
                {
                    wrso.ShowDialog();
                    wrso.Dispose();
                    wrso.Owner = this;
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
