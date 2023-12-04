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
    /// Interaction logic for WindowRptStockOrder.xaml
    /// </summary>
    public partial class WindowRptStockOrder : Window
    {
        private List<storageCentre> allcentre = new List<storageCentre>();
        private List<storageCentre> selectcentre = new List<storageCentre>();

        public WindowRptStockOrder()
        {
            InitializeComponent();
            allcentre = PublicShare.LoadCentreList().OrderBy(x => x.siteName).ToList();
            datePickerToDate.SelectedDate = DateTime.Now;
            LoadingCentreList();
            comboBoxGroupList.DataContext = PublicShare.LoadGroupName();
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void LoadingCentreList()
        {
            try
            {
                listBoxAllCentre.DataContext = allcentre;
                listBoxAllCentre.DisplayMemberPath = "siteName";
                listBoxAllCentre.Items.Refresh();
                if (listBoxAllCentre.Items.Count > 0) { listBoxAllCentre.SelectedIndex = 0; }
                if (listBoxAllCentre.SelectedIndex.Equals(-1)) { buttonAddToList.IsEnabled = false; buttonRemoveFromList.IsEnabled = false; }
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

        private void listBoxAllCentre_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxAllCentre.SelectedIndex.Equals(-1)) { buttonAddToList.IsEnabled = false; } else { buttonAddToList.IsEnabled = true; }
        }

        private void listBoxSelectedCentre_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxSelectedCentre.SelectedIndex.Equals(-1)) { buttonRemoveFromList.IsEnabled = false; } else { buttonRemoveFromList.IsEnabled = true; }
        }

        private void buttonAddToList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (listBoxAllCentre.SelectedIndex.Equals(-1).Equals(false))
                {
                    int selIndex = listBoxAllCentre.SelectedIndex;
                    storageCentre scTmp = listBoxAllCentre.SelectedItem as storageCentre;
                    selectcentre.Add(scTmp);
                    listBoxSelectedCentre.DataContext = selectcentre.OrderBy(x => x.siteName).ToList();
                    listBoxSelectedCentre.DisplayMemberPath = "siteName";
                    listBoxSelectedCentre.Items.Refresh();
                    listBoxAllCentre.DataContext = allcentre.Except(selectcentre);
                    listBoxAllCentre.DisplayMemberPath = "siteName";
                    listBoxAllCentre.Items.Refresh();
                    listBoxAllCentre.SelectedIndex = selIndex;
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

        private void buttonRemoveFromList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (listBoxSelectedCentre.SelectedIndex.Equals(-1).Equals(false))
                {
                    int selIndex = listBoxSelectedCentre.SelectedIndex;
                    storageCentre scTmp = listBoxSelectedCentre.SelectedItem as storageCentre;
                    selectcentre = selectcentre.Except(selectcentre.Where(x => x.siteName == scTmp.siteName)).ToList();
                    listBoxSelectedCentre.DataContext = selectcentre.OrderBy(x => x.siteName).ToList();
                    listBoxSelectedCentre.DisplayMemberPath = "siteName";
                    listBoxSelectedCentre.Items.Refresh();
                    listBoxSelectedCentre.SelectedIndex = selIndex;
                    listBoxAllCentre.DataContext = allcentre.Except(selectcentre);
                    listBoxAllCentre.DisplayMemberPath = "siteName";
                    listBoxAllCentre.Items.Refresh();
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

        private void buttonGenerate_Click(object sender, RoutedEventArgs e)
        {
            Excel._Application appExcel = new Excel.Application(); // creating Excel Application
            try
            {
                if (datePickerFromDate.SelectedDate.Equals(null) || datePickerToDate.SelectedDate.Equals(null)) { throw new Exception("Please select date to generate report"); }
                if (datePickerToDate.SelectedDate.Value.Date < datePickerFromDate.SelectedDate.Value.Date) { throw new Exception("Something wrong with the date you selected, to date must greater than from date"); }
                if (listBoxSelectedCentre.Items.Count.Equals(0)) { throw new Exception("Please select centre to generate report"); }
                string FileName = PublicShare.SaveFilenameRpt("Rpt_StockOrdering");
                if (string.IsNullOrEmpty(FileName)) { throw new Exception("File name cannot be empty"); }
                DateTime dtStartdate = (DateTime)datePickerFromDate.SelectedDate, dtEnddate = (DateTime)datePickerToDate.SelectedDate;
                int ExcelRow = 0;
                Excel._Workbook workbook = appExcel.Workbooks.Add(Type.Missing); // creating new WorkBook within Excel application
                Excel._Worksheet worksheet = null; // creating new Excelsheet in workbook
                appExcel.Visible = false; // Hiding Excel
                // get the reference of first sheet.
                worksheet = (Excel.Worksheet)workbook.Sheets["Sheet1"]; //  By default its name is Sheet1.
                worksheet = (Excel.Worksheet)workbook.ActiveSheet; // Active this Sheet
                worksheet.Name = "Stock Order Report"; // changing the name of active sheet
                ExcelRow++;
                worksheet.Cells[ExcelRow, 1].EntireRow.Font.Size = "18"; // font size
                worksheet.Cells[ExcelRow++, 1].Value = "Comprehensive Auto Restoration Service S/B";
                worksheet.Cells[ExcelRow, 1].EntireRow.Font.Size = "14"; // font size
                worksheet.Cells[ExcelRow++, 1].Value = "Stock Ordering Report For " + dtStartdate.ToString("dd-MM-yyyy") + " to " + dtEnddate.ToString("dd-MM-yyyy");
                ExcelRow += 2;
                ((Excel.Range)worksheet.Columns["A", Type.Missing]).ColumnWidth = 7;
                ((Excel.Range)worksheet.Columns["B", Type.Missing]).ColumnWidth = 11;
                ((Excel.Range)worksheet.Columns["C", Type.Missing]).ColumnWidth = 9;
                ((Excel.Range)worksheet.Columns["D", Type.Missing]).ColumnWidth = 30;
                ((Excel.Range)worksheet.Columns["E", Type.Missing]).ColumnWidth = 5;
                ((Excel.Range)worksheet.Columns["F", Type.Missing]).ColumnWidth = 21;
                worksheet.Cells[ExcelRow, 1].EntireRow.Font.Bold = true; // Make Font Bold
                worksheet.Cells[ExcelRow, 1] = "Centre";
                worksheet.Cells[ExcelRow, 2] = "Date Order";
                worksheet.Cells[ExcelRow, 3] = "Stock No";
                worksheet.Cells[ExcelRow, 4] = "Name";
                worksheet.Cells[ExcelRow, 5] = "Qty";
                worksheet.Cells[ExcelRow, 6] = "Remark";
                worksheet.Cells[ExcelRow, 1].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                worksheet.Cells[ExcelRow, 2].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                worksheet.Cells[ExcelRow, 3].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                worksheet.Cells[ExcelRow, 4].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                worksheet.Cells[ExcelRow, 5].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                worksheet.Cells[ExcelRow, 6].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                ExcelRow++;

                string scode = string.Empty;
                foreach (storageCentre sc in selectcentre)
                {
                    if(!string.IsNullOrEmpty(scode))
                    {
                        scode += " ,";
                    }
                    scode += "'" + sc.site + "'";
                }

                List<storageOrderRptQty> AllSoRQ = PublicShare.GetOrderingRptQty(dtStartdate.ToString("yyyy-MM-dd"), dtEnddate.ToString("yyyy-MM-dd"), scode);

                foreach (storageOrderRptQty orq in AllSoRQ)
                {
                    if (orq != null)
                    {
                        worksheet.Cells[ExcelRow, 1] = orq.CentreName;
                        worksheet.Cells[ExcelRow, 2] = orq.MaxDate;
                        worksheet.Cells[ExcelRow, 3] = orq.IDStock;
                        worksheet.Cells[ExcelRow, 4] = orq.StockName;
                        worksheet.Cells[ExcelRow, 5] = orq.Qty;
                        worksheet.Cells[ExcelRow, 6] = orq.Remark;
                        worksheet.Cells[ExcelRow, 1].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        worksheet.Cells[ExcelRow, 2].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        worksheet.Cells[ExcelRow, 3].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        worksheet.Cells[ExcelRow, 4].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        worksheet.Cells[ExcelRow, 5].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        worksheet.Cells[ExcelRow, 6].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                        ExcelRow++;
                    }
                }
                workbook.SaveAs(FileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                MessageBox.Show("Excel file is successfully created");
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
            finally { appExcel.Quit(); }
            appExcel.Quit();
        }

        private void datePickerFromDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (datePickerFromDate.SelectedDate.Value > DateTime.Now) { datePickerFromDate.SelectedDate = DateTime.Now; throw new Exception("Cannot select date greater than today"); }
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

        private void datePickerToDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (datePickerToDate.SelectedDate.Value > DateTime.Now) { datePickerToDate.SelectedDate = DateTime.Now; throw new Exception("Cannot select date greater than today"); }
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

        private void buttonAddAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                selectcentre.Clear();
                foreach (storageCentre sc in allcentre)
                {
                    selectcentre.Add(sc);
                }
                listBoxSelectedCentre.DataContext = selectcentre.OrderBy(x => x.siteName).ToList();
                listBoxSelectedCentre.DisplayMemberPath = "siteName";
                listBoxSelectedCentre.Items.Refresh();
                listBoxAllCentre.DataContext = allcentre.Except(selectcentre);
                listBoxAllCentre.DisplayMemberPath = "siteName";
                listBoxAllCentre.Items.Refresh();
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

        private void buttonRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                selectcentre.Clear();
                listBoxSelectedCentre.DataContext = selectcentre.OrderBy(x => x.siteName).ToList();
                listBoxSelectedCentre.DisplayMemberPath = "siteName";
                listBoxSelectedCentre.Items.Refresh();
                listBoxAllCentre.DataContext = allcentre.Except(selectcentre);
                listBoxAllCentre.DisplayMemberPath = "siteName";
                listBoxAllCentre.Items.Refresh();
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

        private void buttonAddByGroup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (comboBoxGroupList.SelectedIndex.Equals(-1)) { throw new Exception("No Group is selected..."); }
                string GroupName = comboBoxGroupList.SelectedItem as string;
                foreach (storageCentre sc in allcentre)
                {
                    if (sc.GroupName.Equals(GroupName))
                    {
                        if ((from item in selectcentre
                             where item.siteName == sc.siteName
                             select item).Count().Equals(0)) // see if the item already add or not
                        {
                            selectcentre.Add(sc);
                        }
                    }
                    listBoxSelectedCentre.DataContext = selectcentre.OrderBy(x => x.siteName).ToList();
                    listBoxSelectedCentre.DisplayMemberPath = "siteName";
                    listBoxSelectedCentre.Items.Refresh();
                    listBoxAllCentre.DataContext = allcentre.Except(selectcentre);
                    listBoxAllCentre.DisplayMemberPath = "siteName";
                    listBoxAllCentre.Items.Refresh();
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
