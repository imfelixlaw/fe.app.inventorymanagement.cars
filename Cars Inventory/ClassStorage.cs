using System;
/*
 * act as the storage system,
 * create virtual class to store data collection
 */
namespace Cars_Inventory
{
    // for Centre Item
    public class storageItemCentreQuantity
    {
        public int Site { get; set; }
        public string CentreName { get; set; }
        public int Qty { get; set; }
        public int StockIn { get; set; }
        public int OnHand { get; set; }
    }

    // for cached Centre Item, extending storageItemCentreQuantity
    public class storageCacheItemCentreQuantity : storageItemCentreQuantity
    {
        public int Index { get; set; }
    }

    // for Centre List
    public class storageCentre
    {
        public int site { get; set; }
        public string siteName { get; set; }
        public string GroupName { get; set; }
    }

    // for Item Data
    public class storageItemData
    {
        public string serviceCode { get; set; }
        public string serviceName { get; set; }
    }

    // for cache details item data, extending storageItemData
    public class storageCacheItemData : storageItemData
    {
        public int IDItemGroup { get; set; }
    }

    // for Item Group
    public class storageItemGroup
    {
        public int IDItemGroup { get; set; }
        public string ItemGroupName { get; set; }
    }

    public class storageSellHistory
    {
        public string JobNo { get; set; }
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public DateTime Date { get; set; }
        public decimal Charge { get; set; }
    }

    public class storageStockHistory
    {
        public int IDItemStockIn { get; set; }
        public DateTime Date { get; set; }
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public int Qty { get; set; }
    }

    public class storageOrderCodeDataField
    {
        public int IndexOrder { get; set; }
        public int IDStock { get; set; }
        public string StockName { get; set; }
        public string Measure { get; set; }
    }

    public class storageOrderCodeQty
    {
        public int IDCentre { get; set; }
        public int IDStock { get; set; }
        public string MaxDate { get; set; }
        public int Qty { get; set; }
    }

    public class storageOrderRptQty : storageOrderCodeQty
    {
        public string CentreName { get; set; }
        public string StockName { get; set; }
        public string Remark { get; set; }
    }
}
