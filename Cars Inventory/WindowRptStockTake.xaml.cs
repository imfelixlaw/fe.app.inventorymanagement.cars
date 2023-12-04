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
    /// Interaction logic for WindowRptStockTake.xaml
    /// </summary>
    public partial class WindowRptStockTake : Window
    {
        private List<storageCentre> allcentre = new List<storageCentre>();
        private List<storageCentre> selectcentre = new List<storageCentre>();

        public WindowRptStockTake()
        {
            InitializeComponent();
            allcentre = PublicShare.LoadCentreList().OrderBy(x => x.siteName).ToList();
            datePickerToDate.SelectedDate = DateTime.Now;
            LoadingCentreList();
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
                string FileName = PublicShare.SaveFilenameRpt("Rpt_StockTake");
                if (string.IsNullOrEmpty(FileName)) { throw new Exception("File name cannot be empty"); }

                List<storageOrderCodeDataField> ordercodelist = new List<storageOrderCodeDataField>();
                ordercodelist.Add(new storageOrderCodeDataField { IndexOrder = 0, IDStock = 12, StockName = "PERFECT CUT", Measure = "btl" });
                ordercodelist.Add(new storageOrderCodeDataField { IndexOrder = 1, IDStock = 14, StockName = "Magic Clay 150g (No.5) - (Plastercine)", Measure = "pcs" });
                ordercodelist.Add(new storageOrderCodeDataField { IndexOrder = 2, IDStock = 8, StockName = "Nano One (CX200/1-Polish & Wax V2-1kg)", Measure = "btl" });
                ordercodelist.Add(new storageOrderCodeDataField { IndexOrder = 3, IDStock = 7, StockName = "TYRE SHINE", Measure = "4 lit btl" });
                ordercodelist.Add(new storageOrderCodeDataField { IndexOrder = 4, IDStock = 6, StockName = "Radiant Polish (13) 4lit/Jar", Measure = "4 lit btl" });
                ordercodelist.Add(new storageOrderCodeDataField { IndexOrder = 5, IDStock = 1, StockName = "CAR SHAMPOO", Measure = "4 lit btl" });
                ordercodelist.Add(new storageOrderCodeDataField { IndexOrder = 6, IDStock = 5, StockName = "ENGINE DEGREASER", Measure = "4 lit btl" });
                ordercodelist.Add(new storageOrderCodeDataField { IndexOrder = 7, IDStock = 10, StockName = "Leather Soft (4 Lit Btl : 13 Tin)", Measure = "tin" });
                ordercodelist.Add(new storageOrderCodeDataField { IndexOrder = 8, IDStock = 9, StockName = "Nano Mist - 62ml", Measure = "btl" });
                ordercodelist.Add(new storageOrderCodeDataField { IndexOrder = 9, IDStock = 13, StockName = "GP Coat", Measure = "set" });
                ordercodelist.Add(new storageOrderCodeDataField { IndexOrder = 10, IDStock = 7701, StockName = "GP Coat No.1", Measure = "btl" });
                ordercodelist.Add(new storageOrderCodeDataField { IndexOrder = 11, IDStock = 7702, StockName = "GP Coat No.2", Measure = "btl" });
                ordercodelist.Add(new storageOrderCodeDataField { IndexOrder = 12, IDStock = 7703, StockName = "GP Coat No.3", Measure = "btl" });
                ordercodelist.Add(new storageOrderCodeDataField { IndexOrder = 13, IDStock = 7704, StockName = "GP Coat No.4", Measure = "btl" });
                ordercodelist.Add(new storageOrderCodeDataField { IndexOrder = 14, IDStock = 7705, StockName = "GP Coat No.5", Measure = "btl" });
                ordercodelist.Add(new storageOrderCodeDataField { IndexOrder = 15, IDStock = 11, StockName = "Water Mark Remover CX303 - 1Lit", Measure = "btl" });
                ordercodelist.Add(new storageOrderCodeDataField { IndexOrder = 16, IDStock = 2, StockName = "Leather Cleaner - 4lit/Jar", Measure = "4 lit btl" });
                ordercodelist.Add(new storageOrderCodeDataField { IndexOrder = 17, IDStock = 3, StockName = "Fabric Cleaner - 4lit/Jar", Measure = "4 lit btl" });
                ordercodelist.Add(new storageOrderCodeDataField { IndexOrder = 18, IDStock = 4, StockName = "Tar Sport Remover - 4lit/Jar", Measure = "4 lit btl" });
                ordercodelist.Add(new storageOrderCodeDataField { IndexOrder = 19, IDStock = 0, StockName = "NewLine", Measure = "NewLine" });
                ordercodelist.Add(new storageOrderCodeDataField { IndexOrder = 20, IDStock = 7501, StockName = "14\" BLACK PACK - RUBBER WIPER", Measure = "pcs" });
                ordercodelist.Add(new storageOrderCodeDataField { IndexOrder = 21, IDStock = 7502, StockName = "16\" BLACK PACK - RUBBER WIPER", Measure = "pcs" });
                ordercodelist.Add(new storageOrderCodeDataField { IndexOrder = 22, IDStock = 7503, StockName = "18\" BLACK PACK - RUBBER WIPER", Measure = "pcs" });
                ordercodelist.Add(new storageOrderCodeDataField { IndexOrder = 23, IDStock = 7504, StockName = "19\" BLACK PACK - RUBBER WIPER", Measure = "pcs" });
                ordercodelist.Add(new storageOrderCodeDataField { IndexOrder = 24, IDStock = 7505, StockName = "20\" BLACK PACK - RUBBER WIPER", Measure = "pcs" });
                ordercodelist.Add(new storageOrderCodeDataField { IndexOrder = 25, IDStock = 7506, StockName = "21\" BLACK PACK - RUBBER WIPER", Measure = "pcs" });
                ordercodelist.Add(new storageOrderCodeDataField { IndexOrder = 26, IDStock = 7507, StockName = "22\" BLACK PACK - RUBBER WIPER", Measure = "pcs" });
                ordercodelist.Add(new storageOrderCodeDataField { IndexOrder = 27, IDStock = 7508, StockName = "24\" BLACK PACK - RUBBER WIPER", Measure = "pcs" });
                ordercodelist.Add(new storageOrderCodeDataField { IndexOrder = 28, IDStock = 7509, StockName = "26\" BLACK PACK - RUBBER WIPER", Measure = "pcs" });
                ordercodelist.Add(new storageOrderCodeDataField { IndexOrder = 29, IDStock = 0, StockName = "NewLine", Measure = "NewLine" });
                ordercodelist.Add(new storageOrderCodeDataField { IndexOrder = 30, IDStock = 7801, StockName = "Charcoal Deodorizer", Measure = "pcs" });
                ordercodelist.Add(new storageOrderCodeDataField { IndexOrder = 31, IDStock = 7802, StockName = "AG+ Deodorizer", Measure = "pcs" });

                int ExcelRow = 0;
                Excel._Workbook workbook = appExcel.Workbooks.Add(Type.Missing); // creating new WorkBook within Excel application
                Excel._Worksheet worksheet = null; // creating new Excelsheet in workbook
                appExcel.Visible = false; // Hiding Excel
                // get the reference of first sheet.
                worksheet = (Excel.Worksheet)workbook.Sheets["Sheet1"]; //  By default its name is Sheet1.
                worksheet = (Excel.Worksheet)workbook.ActiveSheet; // Active this Sheet
                worksheet.Name = "Stock Take Report"; // changing the name of active sheet
                ExcelRow++;

                //int[] ordercode = { 12, 14, 8, 7, 6, 1, 5, 10, 9, 13, 7701, 7702, 7703, 7704, 7705, 11, 2, 3, 4, 7501, 7502, 7503, 7504, 7505, 7506, 7507, 7508, 7509, 7801, 7802 };
                DateTime dtStartdate = (DateTime)datePickerFromDate.SelectedDate, dtEnddate = (DateTime)datePickerToDate.SelectedDate;
                List<storageOrderCodeQty> AllSoCQ = PublicShare.GetOrderCodeQty(dtStartdate.ToString("yyyy-MM-dd"), dtEnddate.ToString("yyyy-MM-dd"));
                foreach (storageCentre tmpsc in selectcentre.OrderBy(k => k.siteName))
                {
                    worksheet.Cells[ExcelRow, 1].EntireRow.Font.Size = "18"; // font size
                    worksheet.Cells[ExcelRow++, 1].Value = "Comprehensive Auto Restoration Service S/B";
                    worksheet.Cells[ExcelRow, 1].EntireRow.Font.Size = "14"; // font size
                    worksheet.Cells[ExcelRow++, 1].Value = "Stock Take Report For " + dtStartdate.ToString("dd-MM-yyyy") + " to " + dtEnddate.ToString("dd-MM-yyyy");
                    worksheet.Cells[ExcelRow, 1].EntireRow.Font.Size = "14"; // font size
                    worksheet.Cells[ExcelRow++, 1].Value = "Centre " + tmpsc.siteName;
                    ExcelRow += 2;
                    worksheet.Cells[ExcelRow, 1].EntireRow.Font.Bold = true; // Make Font Bold
                    worksheet.Cells[ExcelRow, 1] = "POS";
                    worksheet.Cells[ExcelRow, 2] = "Description";
                    worksheet.Cells[ExcelRow, 3] = "Measure";
                    worksheet.Cells[ExcelRow, 4] = "Qty";
                    worksheet.Cells[ExcelRow, 5] = "Remark";
                    worksheet.Cells[ExcelRow, 1].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    worksheet.Cells[ExcelRow, 2].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    worksheet.Cells[ExcelRow, 3].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    worksheet.Cells[ExcelRow, 4].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    worksheet.Cells[ExcelRow, 5].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    ((Excel.Range)worksheet.Columns["A", Type.Missing]).ColumnWidth = 8;
                    ((Excel.Range)worksheet.Columns["B", Type.Missing]).ColumnWidth = 37;
                    ((Excel.Range)worksheet.Columns["C", Type.Missing]).ColumnWidth = 8;
                    ((Excel.Range)worksheet.Columns["D", Type.Missing]).ColumnWidth = 4;
                    ((Excel.Range)worksheet.Columns["E", Type.Missing]).ColumnWidth = 27;
                    ExcelRow++;
                    List<storageOrderCodeQty> NewSoCQ = (from k in AllSoCQ
                                                         where k.IDCentre.Equals(tmpsc.site)
                                                         select k).ToList();
                    foreach (storageOrderCodeDataField tmporder in ordercodelist.OrderBy(x => x.IndexOrder))
                    {
                        if (tmporder.IDStock.Equals(0))
                        {
                            worksheet.Cells[ExcelRow, 1].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            worksheet.Cells[ExcelRow, 2].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            worksheet.Cells[ExcelRow, 3].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            worksheet.Cells[ExcelRow, 4].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            worksheet.Cells[ExcelRow, 5].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            ExcelRow++;
                        }
                        else
                        {
                            storageOrderCodeQty socq = NewSoCQ.FirstOrDefault(k => k.IDStock.Equals(tmporder.IDStock));
                            worksheet.Cells[ExcelRow, 1] = tmporder.IDStock.ToString();
                            worksheet.Cells[ExcelRow, 2] = tmporder.StockName.ToString();
                            worksheet.Cells[ExcelRow, 3] = tmporder.Measure.ToString();
                            if (socq != null)
                            {
                                worksheet.Cells[ExcelRow, 4] = socq.Qty.ToString();
                                worksheet.Cells[ExcelRow, 5] = socq.MaxDate.ToString();
                            }
                            else
                            {
                                worksheet.Cells[ExcelRow, 4] = "0"; // if no found in cache mean is no found in database
                            }
                            worksheet.Cells[ExcelRow, 1].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            worksheet.Cells[ExcelRow, 2].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            worksheet.Cells[ExcelRow, 3].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            worksheet.Cells[ExcelRow, 4].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            worksheet.Cells[ExcelRow, 5].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                            ExcelRow++;
                        }
                    }
                    ExcelRow += 3;
                    worksheet.Cells[ExcelRow, 2] = "Stock Take Date";
                    worksheet.Cells[ExcelRow, 3] = "____________________";
                    ExcelRow += 3;
                    worksheet.Cells[ExcelRow, 2] = "Process By (Supervisor)";
                    worksheet.Cells[ExcelRow, 3] = "____________________";
                    ExcelRow += 3;
                    worksheet.Cells[ExcelRow, 2] = "Certifify By (Operation";
                    worksheet.Cells[ExcelRow, 3] = "____________________";
                    ExcelRow += 6;
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
    }
}
