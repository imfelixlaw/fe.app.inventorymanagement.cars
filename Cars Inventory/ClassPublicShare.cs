using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.Win32;

namespace Cars_Inventory
{
    public static class PublicShare
    {
        private static myConn m = new myConn();
        private static MySqlConnection myConn = new MySqlConnection(m.Setting); // Mysql Connection
        private static MySqlCommand myCmd; // MySql Command
        private static MySqlDataReader myDr; // MySql Data Reader

        public static bool ForceItemRefresh = false;

        public static List<storageItemGroup> LoadItemGroupList()
        {
            List<storageItemGroup> IG = new List<storageItemGroup>();
            try
            {
                if (myConn.State.Equals(ConnectionState.Closed)) { myConn.Open(); }
                using (myCmd = new MySqlCommand(@"SELECT `IDItemGroup`, `ItemGroupName` FROM `cars_itemgroup` WHERE `Status` = 'Y';", myConn))
                {
                    myCmd.CommandTimeout = 0;
                    using (myDr = myCmd.ExecuteReader())
                    {
                        while (myDr.Read())
                        {
                            IG.Add(new storageItemGroup { IDItemGroup = myDr.GetInt32(0), ItemGroupName = myDr.GetString(1) });
                        }
                    }
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            finally { if (myConn.State.Equals(ConnectionState.Open)) { myConn.Close(); } }
            return IG.OrderBy(x => x.ItemGroupName).ToList(); // order by ItemGroupName asc
        }

        public static List<string> LoadGroupName()
        {
            List<string> GroupList = new List<string>();
            try
            {
                string sql = @" SELECT `GroupName` FROM `tblregcentre`
                    WHERE `GroupName` <> 'X'
                    GROUP BY `GroupName`
                    ORDER BY `GroupName`;";
                if (myConn.State.Equals(ConnectionState.Closed)) { myConn.Open(); }
                using (myCmd = new MySqlCommand(sql, myConn))
                {
                    myCmd.CommandTimeout = 0;
                    using (myDr = myCmd.ExecuteReader())
                    {
                        while (myDr.Read())
                        {
                            GroupList.Add(myDr.GetValue(0).ToString());
                        }
                    }
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            finally { if (myConn.State.Equals(ConnectionState.Open)) { myConn.Close(); } }
            return GroupList;


           
        }

        public static List<storageCentre> LoadCentreList()
        {
            List<storageCentre> centreList = new List<storageCentre>();
            try
            {
                string sql = @"SELECT `Site`, `RegCentre`, `GroupName` FROM `tblregcentre` WHERE `GroupName` <> 'X';";
                if (myConn.State.Equals(ConnectionState.Closed)) { myConn.Open(); }
                using (myCmd = new MySqlCommand(sql, myConn))
                {
                    myCmd.CommandTimeout = 0;
                    using (myDr = myCmd.ExecuteReader())
                    {
                        while (myDr.Read())
                        {
                            centreList.Add(new storageCentre { site = myDr.GetInt32(0), siteName = myDr.GetString(1), GroupName = myDr.GetValue(2).ToString() });
                        }
                    }
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            finally { if (myConn.State.Equals(ConnectionState.Open)) { myConn.Close(); } }
            return centreList;
        }

        public static List<storageItemData> LoadUsedItemList(List<storageItemData> FullItemList)
        {
            List<storageItemData> UsedItemList = new List<storageItemData>();
            try
            {
                if (myConn.State.Equals(ConnectionState.Closed)) { myConn.Open(); }
                using (myCmd = new MySqlCommand(@"SELECT `FKIDServiceCode` FROM `cars_itemgroup_item` WHERE `Status` = 'Y';", myConn))
                {
                    myCmd.CommandTimeout = 0;
                    using (myDr = myCmd.ExecuteReader())
                    {
                        while (myDr.Read())
                        {
                            UsedItemList.Add(new storageItemData { serviceCode = myDr.GetString(0), serviceName = string.Empty});
                        }
                    }
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            finally { if (myConn.State.Equals(ConnectionState.Open)) { myConn.Close(); } }
            return (from i1 in UsedItemList // take from UsedItemList
                    from i2 in FullItemList // take from FullItemList
                    where i1.serviceCode.Equals(i2.serviceCode) // only select when UsedItemList's serviceCode is equal to FullItemList's serviceCode
                    select new storageItemData { serviceCode = i1.serviceCode, serviceName = i2.serviceName }).ToList(); // select data to list
        }

        public static List<storageItemData> LoadFullItemList()
        {
            List<storageItemData> FullItemList = new List<storageItemData>();
            try
            {
                if (myConn.State.Equals(ConnectionState.Closed)) { myConn.Open(); }
                using (myCmd = new MySqlCommand(@"SELECT `ServiceCode`, `ServiceName` FROM `tblservicecode`;", myConn))
                {
                    myCmd.CommandTimeout = 0; // set to maximum execution time, generally no required but extra protection
                    using (myDr = myCmd.ExecuteReader()) // Read from tblservicecode
                    {
                        while (myDr.Read())
                        {
                            FullItemList.Add(new storageItemData { serviceCode = myDr.GetString(0), serviceName = myDr.GetString(1) });
                        }
                    }
                }
                using (myCmd = new MySqlCommand(@"SELECT `ServiceCode`, `ServiceType` FROM `tblvoucher`", myConn))
                {
                    myCmd.CommandTimeout = 0;
                    using (myDr = myCmd.ExecuteReader()) // Read from tblvoucher
                    {
                        while (myDr.Read())
                        {
                            FullItemList.Add(new storageItemData { serviceCode = myDr.GetString(0), serviceName = myDr.GetString(1) });
                        }
                    }
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            finally { if (myConn.State.Equals(ConnectionState.Open)) { myConn.Close(); } }
            return FullItemList.OrderBy(x => x.serviceName).ToList(); // Return order asc serviceName
        }

        public static List<storageCacheItemData> LoadingCacheItemData()
        {
            List<storageCacheItemData> sDid = new List<storageCacheItemData>();
            try
            {
                if (myConn.State.Equals(ConnectionState.Closed)) { myConn.Open(); }
                using (myCmd = new MySqlCommand(@"SELECT `FKIDItemGroup`, `FKIDServiceCode` FROM `cars_itemgroup_item` WHERE `Status` = 'Y';", myConn))
                {
                    myCmd.CommandTimeout = 0; // set to maximum execution time, generally no required but extra protection
                    using (myDr = myCmd.ExecuteReader()) // Read from tblservicecode
                    {
                        while (myDr.Read())
                        {
                            sDid.Add(new storageCacheItemData { IDItemGroup = myDr.GetInt32(0), serviceCode = myDr.GetString(1), serviceName = string.Empty });
                        }
                    }
                }
                sDid = (from i1 in sDid
                        from i2 in LoadFullItemList()
                        where i1.serviceCode.Equals(i2.serviceCode)
                        select new storageCacheItemData { IDItemGroup = i1.IDItemGroup, serviceCode = i1.serviceCode, serviceName = i2.serviceName + " (" + i1.serviceCode + ")" }).ToList();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            finally { if (myConn.State.Equals(ConnectionState.Open)) { myConn.Close(); } }
            return sDid;
        }

        public static void UpdateItemGroupRename(int IDItemGroup, string newname)
        {
            try
            {
                if (myConn.State.Equals(ConnectionState.Closed)) { myConn.Open(); }
                using (myCmd = new MySqlCommand(string.Format(@"SELECT * FROM `cars_itemgroup` WHERE `ItemGroupName` = '{0}' AND `Status` = 'Y';", newname), myConn))
                {
                    myCmd.CommandTimeout = 0;
                    using (myDr = myCmd.ExecuteReader())
                    {
                        if (myDr.Read()) { throw new Exception("This name is in used, please choose unique"); } // if got result return mean got this name already
                    }
                }
                using (myCmd = new MySqlCommand(string.Format(@"UPDATE `cars_itemgroup` SET `ItemGroupName` = '{0}' WHERE `IDItemGroup` = {1};", newname, IDItemGroup), myConn))
                {
                    myCmd.CommandTimeout = 0;
                    myCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            finally { if (myConn.State.Equals(ConnectionState.Open)) { myConn.Close(); } }
        }

        public static List<storageStockHistory> FetchStockInHistory(string ServiceCodeGroup, int Site)
        {
            List<storageStockHistory> lssh = new List<storageStockHistory>();
            try
            {
                if (myConn.State.Equals(ConnectionState.Closed)) { myConn.Open(); }
                string sql = string.Format(@"SELECT `IDItemStockIn`, `FKIDServiceCode`, `Qty`, `StockDT`
                    FROM `cars_item_stockin`
                    WHERE `Active` = 'Y' AND `FKIDSite` = '{0}' AND `FKIDServiceCode` IN ({1});", Site, ServiceCodeGroup);
                using (myCmd = new MySqlCommand(sql, myConn))
                {
                    myCmd.CommandTimeout = 0; // set to maximum execution time, generally no required but extra protection
                    using (myDr = myCmd.ExecuteReader()) // Read from tblservicecode
                    {
                        while (myDr.Read())
                        {
                            lssh.Add(new storageStockHistory { IDItemStockIn = myDr.GetInt32(0), ServiceCode = myDr.GetString(1), Qty = myDr.GetInt32(2), Date = myDr.GetDateTime(3), ServiceName = string.Empty });
                        }
                    }
                }
                lssh = (from i1 in lssh
                        from i2 in LoadFullItemList()
                        where i1.ServiceCode.Equals(i2.serviceCode)
                        select new storageStockHistory { IDItemStockIn = i1.IDItemStockIn, Date = i1.Date, ServiceCode = i1.ServiceCode, ServiceName = i2.serviceName, Qty = i1.Qty }).ToList();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            finally { if (myConn.State.Equals(ConnectionState.Open)) { myConn.Close(); } }
            return lssh;
        }

        public static List<storageSellHistory> FetchSellingHistory(string ServiceCodeGroup, int Site)
        {
            List<storageSellHistory> lssh = new List<storageSellHistory>();
            try
            {
                if (myConn.State.Equals(ConnectionState.Closed)) { myConn.Open(); }
                string sql = string.Format(@"SELECT `JobNo`, `ServiceCode`, `DateCreated`, `Charge` FROM `tbljobsheetservice`
                    WHERE `ServiceCode` IN ({0}) AND `Site` = '{1}' ORDER BY `JobNo`;", ServiceCodeGroup, Site);
                using (myCmd = new MySqlCommand(sql, myConn))
                {
                    myCmd.CommandTimeout = 0; // set to maximum execution time, generally no required but extra protection
                    using (myDr = myCmd.ExecuteReader()) // Read from tblservicecode
                    {
                        while (myDr.Read())
                        {
                            lssh.Add(new storageSellHistory { JobNo = myDr.GetString(0), ServiceCode = myDr.GetString(1), ServiceName = string.Empty, Date = myDr.GetDateTime(2), Charge = myDr.GetDecimal(3) });
                        }
                    }
                }
                lssh = (from i1 in lssh
                        from i2 in LoadFullItemList()
                        where i1.ServiceCode.Equals(i2.serviceCode)
                        select new storageSellHistory { JobNo = i1.JobNo, ServiceCode = i1.ServiceCode, ServiceName = i2.serviceName, Date = i1.Date, Charge = i1.Charge }).ToList();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            finally { if (myConn.State.Equals(ConnectionState.Open)) { myConn.Close(); } }
            return lssh;
        }

        public static void UpdateItemGroupNew(string newname)
        {
            try
            {
                if (myConn.State.Equals(ConnectionState.Closed)) { myConn.Open(); }
                using (myCmd = new MySqlCommand(string.Format(@"SELECT * FROM `cars_itemgroup` WHERE `ItemGroupName` = '{0}' AND `Status` = 'Y';", newname), myConn))
                {
                    myCmd.CommandTimeout = 0;
                    using (myDr = myCmd.ExecuteReader())
                    {
                        if (myDr.Read()) { throw new Exception("This name is in used, please choose unique"); }
                    }
                }
                using (myCmd = new MySqlCommand(string.Format(@"INSERT INTO `cars_itemgroup` VALUES (NULL, '{0}', 'Y');", newname), myConn))
                {
                    myCmd.CommandTimeout = 0;
                    myCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            finally { if (myConn.State.Equals(ConnectionState.Open)) { myConn.Close(); } }
        }

        public static void DeleteStockInHistory(int IDItemStockIn)
        {
            try
            {
                if (myConn.State.Equals(ConnectionState.Closed)) { myConn.Open(); }
                string sql = string.Format(@"UPDATE `cars_item_stockin` SET `Active` = 'N' WHERE `IDItemStockIn` = {0}", IDItemStockIn);
                using (myCmd = new MySqlCommand(sql, myConn))
                {
                    myCmd.CommandTimeout = 0;
                    myCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            finally { if (myConn.State.Equals(ConnectionState.Open)) { myConn.Close(); } }
        }

        public static string SaveFilename()
        {
            try
            {
                SaveFileDialog sFD = new SaveFileDialog();
                sFD.Title = "Inventory Report";
                sFD.AddExtension = true; // auto add extention .xls
                sFD.DefaultExt = "xls"; // default as excel file
                sFD.Filter = "Excel files |*.xls|All files (*.*)|*.*";
                sFD.FilterIndex = 1;
                sFD.RestoreDirectory = true;
                sFD.FileName = "InventoryReport.xls"; // put default file name here
                return (sFD.ShowDialog().Equals(true)) ? sFD.FileName : string.Empty; // Getting if ShowDialog Save is Press, then return the FileName or return empty string
            }
            catch { return string.Empty; } // Dismiss Error
        }

        public static string SaveFilenameRpt(string defaultName)
        {
            try
            {
                SaveFileDialog sFD = new SaveFileDialog();
                sFD.Title = "Save as Excel";
                sFD.AddExtension = true; // auto add extention .xls
                sFD.DefaultExt = "xls"; // default as excel file
                sFD.Filter = "Excel files |*.xls|All files (*.*)|*.*";
                sFD.FilterIndex = 1;
                sFD.RestoreDirectory = true;
                sFD.FileName = defaultName + ".xls"; // put default file name here
                return (sFD.ShowDialog().Equals(true)) ? sFD.FileName : string.Empty; // Getting if ShowDialog Save is Press, then return the FileName or return empty string
            }
            catch { return string.Empty; } // Dismiss Error
        }

        public static string GetServiceCodeFromProductGroup(int IDProductGroup)
        {
            string output = string.Empty;
            try
            {
                if (myConn.State.Equals(ConnectionState.Closed)) { myConn.Open(); }
                using (myCmd = new MySqlCommand(string.Format(@"SELECT `FKIDServiceCode` FROM `cars_itemgroup_item` WHERE `FKIDItemGroup` = {0};", IDProductGroup), myConn))
                {
                    myCmd.CommandTimeout = 0;
                    using (myDr = myCmd.ExecuteReader())
                    {
                        for (int Counter = 0; myDr.Read(); Counter++)
                        {
                            if (output.Equals(string.Empty).Equals(false)) { output += ", "; } // add , as separator if is not the 1st element
                            output += string.Format("'{0}'", myDr.GetString(0)); // appending it
                        }
                    }
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            finally { if (myConn.State.Equals(ConnectionState.Open)) { myConn.Close(); } }
            return output;
        }

        public static List<storageOrderCodeQty> GetOrderCodeQty(string startdate, string enddate)
        {
            List<storageOrderCodeQty> tmpocq = new List<storageOrderCodeQty>();
            try
            {
                if (myConn.State.Equals(ConnectionState.Closed)) { myConn.Open(); }
                string sql = string.Format(@"SELECT `Site`, `StockNo`, MAX(`OrderDate`) AS `MaxDate` , `StockBalance`
FROM `tblstockordering`
WHERE `StockNo` IN (12, 14, 8, 7, 6, 1, 5, 10, 9, 13, 7701, 7702, 7703, 7704, 7705, 11, 2, 3, 4, 7501, 7502, 7503, 7504, 7505, 7506, 7507, 7508, 7509, 7801, 7802)
AND `OrderDate` BETWEEN '{0} 00:00:00' AND '{1} 23:59:59'
GROUP BY `Site`, `StockNo`
ORDER BY `Site`, `StockNo`, `OrderDate`;", startdate, enddate);
                using (myCmd = new MySqlCommand(sql, myConn))
                {
                    myCmd.CommandTimeout = 0;
                    using (myDr = myCmd.ExecuteReader())
                    {
                        while (myDr.Read())
                        {
                            tmpocq.Add(new storageOrderCodeQty {IDCentre = myDr.GetInt32(0), IDStock = myDr.GetInt32(1), MaxDate = myDr.GetValue(2).ToString(), Qty = Convert.ToInt32(myDr.GetValue(3).ToString()) });
                        }
                    }
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            finally { if (myConn.State.Equals(ConnectionState.Open)) { myConn.Close(); } }
            return tmpocq;
        }

        public static List<storageOrderRptQty> GetOrderingRptQty(string startdate, string enddate, string scode)
        {
            List<storageOrderRptQty> tmpocq = new List<storageOrderRptQty>();
            try
            {
                if (myConn.State.Equals(ConnectionState.Closed)) { myConn.Open(); }
                string sql = string.Format(@"SELECT rc.`RegCentre` AS `Centre`, DATE_FORMAT(so.`OrderDate`, '%d/%m/%Y') AS `Date Order`, so.`StockNo` AS `Stock No`, so.`Name`, so.`Qty` AS `Quantity`, so.`Remark`
FROM `tblstockordering` AS so
INNER JOIN `tblregcentre` AS rc ON rc.`Site` = so.`Site`
WHERE so.`OrderDate` BETWEEN '{0} 00:00:00' AND '{1} 23:59:59'
AND so.`Site` IN ({2})
ORDER BY rc.`RegCentre`, so.`OrderDate`, so.`StockNo`, so.`Name`;", startdate, enddate, scode);
                using (myCmd = new MySqlCommand(sql, myConn))
                {
                    myCmd.CommandTimeout = 0;
                    using (myDr = myCmd.ExecuteReader())
                    {
                        while (myDr.Read())
                        {
                            tmpocq.Add(new storageOrderRptQty { CentreName = myDr.GetValue(0).ToString(), MaxDate = myDr.GetValue(1).ToString(), IDStock = myDr.GetInt32(2), StockName = myDr.GetString(3), Qty = Convert.ToInt32(myDr.GetValue(4).ToString()), Remark = myDr.GetValue(5).ToString() });
                        }
                    }
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            finally { if (myConn.State.Equals(ConnectionState.Open)) { myConn.Close(); } }
            return tmpocq;
        }
    }
}
