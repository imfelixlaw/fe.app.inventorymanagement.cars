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
    /// Interaction logic for WindowProductGroup.xaml
    /// </summary>
    public partial class WindowProductGroup : Window
    {
        private myConn m = new myConn();
        private MySqlConnection myConn; // Mysql Connection
        private MySqlCommand myCmd; // MySql Command
        private MySqlDataReader myDr; // MySql Data Reader

        private List<storageItemData> FullItemList = new List<storageItemData>();
        private List<storageItemData> UsedItemList = new List<storageItemData>();
        private List<storageItemData> SelectedItemList = new List<storageItemData>();

        public WindowProductGroup()
        {
            InitializeComponent();
            try
            {
                myConn = new MySqlConnection(m.Setting); // Create MySQL Connection
                FullItemList = PublicShare.LoadFullItemList();
                LoadItemList(FullItemList);
                LoadItemGroupList();
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

        private void LoadItemGroupList()
        {
            try
            {
                listBoxProductGroup.DataContext = PublicShare.LoadItemGroupList(); // Loading the ListBox
                listBoxProductGroup.DisplayMemberPath = "ItemGroupName"; // set the display column
                listBoxProductGroup.Items.Refresh(); // Update the listbox
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


        private void LoadItemList(List<storageItemData> FullList)
        {
            try
            {
                UsedItemList = PublicShare.LoadUsedItemList(FullItemList);
                listBoxAllItem.DataContext = null; // Release the listbox
                listBoxAllItem.Items.Refresh(); // Update the listbox
                listBoxAllItem.DataContext = FullItemList.Except((from i1 in UsedItemList
                                                                  from i2 in FullItemList
                                                                  where i1.serviceCode.Equals(i2.serviceCode)
                                                                  select i2).ToList()).OrderBy(x => x.serviceName);
                listBoxAllItem.DisplayMemberPath = "serviceName";
                listBoxAllItem.Items.Refresh();
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

        private void listBoxProductGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxProductGroup.SelectedIndex != -1)
            {
                RefreshItemListData();
                textBoxItemServiceCode.Clear();
            }
        }

        private void RefreshItemListData()
        {
            try
            {
                if (listBoxProductGroup.SelectedIndex.Equals(-1).Equals(false))
                {
                    textBlockListofItemTips.Visibility = Visibility.Hidden;
                    buttonAddToGroup.IsEnabled = true;
                    buttonRemoveFromGroup.IsEnabled = true;
                    buttonGroupRemove.IsEnabled = true;
                    buttonGroupRename.IsEnabled = true;
                    SelectedItemList.Clear();
                    listBoxItemInGroup.DataContext = null;
                    storageItemGroup IG = listBoxProductGroup.SelectedItem as storageItemGroup;
                    if (myConn.State.Equals(ConnectionState.Closed)) { myConn.Open(); }
                    using (myCmd = new MySqlCommand(string.Format(@"SELECT `FKIDServiceCode`
                        FROM `cars_itemgroup_item`
                        WHERE `Status` = 'Y' AND `FKIDItemGroup` = {0};", IG.IDItemGroup), myConn))
                    {
                        myCmd.CommandTimeout = 0;
                        using (myDr = myCmd.ExecuteReader())
                        {
                            while (myDr.Read())
                            {
                                SelectedItemList.Add(new storageItemData { serviceCode = myDr.GetString(0), serviceName = string.Empty });
                            }
                        }
                    }
                    SelectedItemList = (from i1 in SelectedItemList
                                        from i2 in FullItemList
                                        where i1.serviceCode.Equals(i2.serviceCode)
                                        select new storageItemData { serviceCode = i1.serviceCode, serviceName = i2.serviceName }).ToList();
                    if (textBoxSearchItem.Text.Length == 0)
                    {
                        LoadItemList(FullItemList.Except(SelectedItemList).ToList());
                    }
                    else
                    {
                        SearchFilterAllItemList();
                    }
                    listBoxItemInGroup.DataContext = SelectedItemList.OrderBy(x => x.serviceName);
                    listBoxItemInGroup.DisplayMemberPath = "serviceName";
                    buttonGroupRemove.IsEnabled = (listBoxItemInGroup.Items.Count > 0) ? false : true;
                }
                else // technically not required but for protection
                {
                    listBoxItemInGroup.DataContext = null;
                    listBoxItemInGroup.Items.Clear();
                }
                listBoxItemInGroup.Items.Refresh();
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

        private void buttonGroupRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (listBoxProductGroup.SelectedIndex.Equals(-1)) { throw new Exception("Please select one item from the list"); }
                if (MessageBox.Show("This will remove the selected group, are you sure to proceed?", "Confirmation", MessageBoxButton.YesNo).Equals(MessageBoxResult.Yes))
                {
                    storageItemGroup Temp = listBoxProductGroup.SelectedItem as storageItemGroup;
                    // Check if any item belong to group
                    if (myConn.State.Equals(ConnectionState.Closed)) { myConn.Open(); }
                    int TotalExist = 0;
                    using (myCmd = new MySqlCommand(string.Format(@"SELECT COUNT(*) FROM `cars_itemgroup_item` WHERE `FKIDItemGroup` = {0};", Temp.IDItemGroup), myConn))
                    {
                        myCmd.CommandTimeout = 0;
                        using (myDr = myCmd.ExecuteReader())
                        {
                            if (myDr.Read()) { TotalExist = myDr.GetInt32(0); }
                        }
                    }
                    if (TotalExist > 0) { throw new Exception("This group still containing service code, please remove all of them before remove this group."); }
                    // End Check, if no exist any item within the group continue
                    using (myCmd = new MySqlCommand(string.Format(@"UPDATE `cars_itemgroup` SET `Status` = 'N' WHERE `IDItemGroup` = {0};", Temp.IDItemGroup), myConn))
                    {
                        myCmd.CommandTimeout = 0;
                        myCmd.ExecuteNonQuery();
                    }
                    LoadItemGroupList();
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

        private void buttonGroupRename_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (listBoxProductGroup.SelectedIndex.Equals(-1)) { throw new Exception("Please select one item from the list"); }
                storageItemGroup tmpsig = listBoxProductGroup.SelectedItem as storageItemGroup;
                using (WindowProductGroup_GroupRelated wpggr = new WindowProductGroup_GroupRelated(1, tmpsig.IDItemGroup, tmpsig.ItemGroupName))
                {
                    wpggr.ShowDialog();
                    wpggr.Dispose(); // destroy all created resourses
                    wpggr.Owner = this;
                }
                int SelectedIndex = listBoxProductGroup.SelectedIndex;
                LoadItemGroupList();
                listBoxProductGroup.SelectedIndex = SelectedIndex;
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

        private void listBoxAllItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (listBoxAllItem.SelectedIndex > -1)
                {
                    storageItemData temp = listBoxAllItem.SelectedItem as storageItemData;
                    textBoxItemServiceCode.Text = temp.serviceCode;
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

        private void textBoxSearchItem_GotFocus(object sender, RoutedEventArgs e)
        {
            textBlockSearchBoxTips.Visibility = Visibility.Hidden;
        }

        private void textBoxSearchItem_LostFocus(object sender, RoutedEventArgs e)
        {
            textBlockSearchBoxTips.Visibility = (textBoxSearchItem.Text.Length > 0) ? Visibility.Hidden : Visibility.Visible;
        }

        private void buttonAddToGroup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (listBoxAllItem.SelectedIndex.Equals(-1)) { throw new Exception("Please select an item from the list beside this button to proceed"); }
                if (listBoxProductGroup.SelectedIndex.Equals(-1)) { throw new Exception("Please select an item from the product group list to proceed"); }
                int selectIndex = listBoxAllItem.SelectedIndex;
                storageItemData tmpSid = listBoxAllItem.SelectedItem as storageItemData;
                storageItemGroup tmpSig = listBoxProductGroup.SelectedItem as storageItemGroup;
                if (myConn.State.Equals(ConnectionState.Closed)) { myConn.Open(); }
                using (myCmd = new MySqlCommand(string.Format(@"REPLACE INTO `cars_itemgroup_item` (`FKIDItemGroup`, `FKIDServiceCode`, `Status`) VALUES ({0}, '{1}', 'Y')", tmpSig.IDItemGroup, tmpSid.serviceCode), myConn))
                {
                    myCmd.CommandTimeout = 0;
                    myCmd.ExecuteNonQuery();
                }
                RefreshItemListData();
                listBoxAllItem.SelectedIndex = selectIndex;
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

        private void listBoxItemInGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (listBoxItemInGroup.SelectedIndex > -1)
                {
                    storageItemData temp = listBoxItemInGroup.SelectedItem as storageItemData;
                    textBoxItemServiceCode.Text = temp.serviceCode;
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

        private void buttonRemoveFromGroup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (listBoxItemInGroup.SelectedIndex.Equals(-1)) { throw new Exception("Please select an item from the list beside this button to proceed"); }
                if (listBoxProductGroup.SelectedIndex.Equals(-1)) { throw new Exception("Please select an item from the product group list to proceed"); }
                int selectIndex = listBoxItemInGroup.SelectedIndex;
                storageItemData tmpSid = listBoxItemInGroup.SelectedItem as storageItemData;
                storageItemGroup tmpSig = listBoxProductGroup.SelectedItem as storageItemGroup;
                if (myConn.State.Equals(ConnectionState.Closed)) { myConn.Open(); }
                using (myCmd = new MySqlCommand(string.Format(@"REPLACE INTO `cars_itemgroup_item` (`FKIDItemGroup`, `FKIDServiceCode`, `Status`) VALUES ({0}, '{1}', 'N')", tmpSig.IDItemGroup, tmpSid.serviceCode), myConn))
                {
                    myCmd.CommandTimeout = 0;
                    myCmd.ExecuteNonQuery();
                }
                RefreshItemListData();
                listBoxItemInGroup.SelectedIndex = selectIndex;
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

        private void SearchFilterAllItemList()
        {
            try
            {
                if (textBoxSearchItem.Text.Length > 0)
                {
                    UsedItemList = PublicShare.LoadUsedItemList(FullItemList);
                    listBoxAllItem.DataContext = null; // Release the listbox
                    listBoxAllItem.Items.Refresh(); // Update the listbox
                    listBoxAllItem.DataContext = (from fil in FullItemList.Except((from i1 in UsedItemList
                                                                                   from i2 in FullItemList
                                                                                   where i1.serviceCode.Equals(i2.serviceCode)
                                                                                   select i2)).ToList()
                                                  where fil.serviceName.ToLower().Contains(textBoxSearchItem.Text.ToLower())
                                                  select fil).ToList().OrderBy(x => x.serviceName);
                    listBoxAllItem.DisplayMemberPath = "serviceName";
                    listBoxAllItem.Items.Refresh();
                }
                else
                {
                    RefreshItemListData();
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

        private void textBoxSearchItem_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchFilterAllItemList();
        }

        private void buttonGroupAddNew_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (WindowProductGroup_GroupRelated wpggr = new WindowProductGroup_GroupRelated(2))
                {
                    wpggr.ShowDialog();
                    wpggr.Dispose();
                    wpggr.Owner = this;
                }
                LoadItemGroupList();
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
