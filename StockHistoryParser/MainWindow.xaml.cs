using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace StockHistoryParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            StockNameFileTextBox.Text = Properties.Settings.Default.LastStockNameFilePath;
            WindowState = WindowState.Maximized;
        }

        private Dictionary<string, string> StockNames;
        private string[] files;
        private List<StockData> stocks;
        StringBuilder sb = new StringBuilder();

        private void SelectFilesButton_Click(object sender, RoutedEventArgs e)
        {
            sb.Clear();
            stocks = new List<StockData>();

            if (!System.IO.File.Exists(StockNameFileTextBox.Text))
            {
                StockDataTextBox.Text = "Stock name file not found.";
                return;
            }

            if (!OpenFileSelectDialog())
            {
                StockDataTextBox.Text = "File selection canceled/failed.";
                return;
            }

            ReadStockNameFile(StockNameFileTextBox.Text);

            foreach (var item in files)
            {
                ReadHistoryFile(item);
            }

            WriteStockData();
        }

        private bool OpenFileSelectDialog()
        {
            OpenFileDialog dlg = new OpenFileDialog()
            {
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = "csv files (*.csv)| *.csv",
                AddExtension = true,
                Multiselect = true,
                Title = "Select files"
            };

            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return false;
            }

            files = dlg.FileNames;
            return true;
        }

        private void ReadStockNameFile(string filepath)
        {
            StockNames = new Dictionary<string, string>();
            string line = "";
            using (var file = new System.IO.StreamReader(filepath))
            {
                while ((line = file.ReadLine()) != null)
                {
                    string[] name = line.Split(';');
                    StockNames.Add(name[0], name[1]);
                }
            }
        }

        private void ReadHistoryFile(string filepath)
        {
            string stockName = GetStockNameFromFileName(filepath);
            string line = "";
            using (var file = new System.IO.StreamReader(filepath))
            {
                //First line contains separator, second line headers and rest are stock data.
                //sep=;
                //Date; Bid; Ask; Opening price; High price; Low price; Closing price; Average price; Total volume; Turnover; Trades;
                //2021-06-04; 8,67; 8,68; 8,70; 8,71; 8,61; 8,67; 8,659; 22623; 195908,6; 217;

                //Separator
                line = file.ReadLine();
                string[] name = line.Split('=');
                string separator = name[1];
                //Skip headers
                line = file.ReadLine();
                //Values
                while ((line = file.ReadLine()) != null)
                {
                    string[] values = line.Split(new string[] { separator }, StringSplitOptions.None);
                    //Date is column 1 and Closing price is column 7.
                    StockData sd = new StockData(stockName, values[0], values[6]);
                    stocks.Add(sd);
                }
            }
        }

        private string GetStockNameFromFileName(string filepath)
        {
            string filename = System.IO.Path.GetFileName(filepath);
            string[] name = filename.Split('-');
            if (StockNames.ContainsKey(name[0]))
            {
                return StockNames[name[0]];
            }
            return name[0];
        }

        private void WriteStockData()
        {
            foreach (var item in stocks.OrderBy(s => s.CompanyName).ThenBy(s => s.Date))
            {
                sb.AppendLine(item.ToString());
            }
            StockDataTextBox.Text = sb.ToString();
            System.Windows.Clipboard.SetText(StockDataTextBox.Text);
        }

        private class StockData
        {
            public string CompanyName { get; private set; }
            public DateTime Date { get; private set; }
            public decimal ClosingPrice { get; private set; }

            public StockData(string name, string date, string closingPrice)
            {
                CompanyName = name;
                Date = Convert.ToDateTime(date);
                ClosingPrice = Convert.ToDecimal(closingPrice);
            }

            public override string ToString()
            {
                return $"{CompanyName}\t{Date.ToString("dd.MM.yyyy")}\t{ClosingPrice}";
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!StockNameFileTextBox.Text.Equals(Properties.Settings.Default.LastStockNameFilePath) &&
                System.IO.File.Exists(StockNameFileTextBox.Text))
            {
                Properties.Settings.Default.LastStockNameFilePath = StockNameFileTextBox.Text;
                Properties.Settings.Default.Save();
            }
        }
    }
}
