using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace StockHistoryParser
{
	class HistoryParser
	{
		#region constructors
		#endregion

		#region properties
		private readonly Dictionary<string, string> stockNames = new();
		private readonly List<string> files = new();
		private readonly List<StockData> stocks = new();
		private readonly StringBuilder sb = new();
		#endregion

		#region methods
		public string ProcessFiles(string stockSettingsFilePath, IEnumerable<string> csvFiles)
		{
			sb.Clear();
			stocks.Clear();

			if (!System.IO.File.Exists(stockSettingsFilePath))
			{
				return "Stock name file not found.";
			}

			AddFilesNamesToList(csvFiles);

			ReadStockNameFile(stockSettingsFilePath);

			foreach (var item in files)
			{
				ReadHistoryFile(item);
			}

			return GetStockData();
		}

		private void AddFilesNamesToList(IEnumerable<string> fileNames)
		{
			files.Clear();
			foreach (var item in fileNames)
			{
				if (item.EndsWith(".csv"))
				{
					files.Add(item);
				}
			}
		}

		private void ReadStockNameFile(string filepath)
		{
			stockNames.Clear();
			using var file = new System.IO.StreamReader(filepath);
			while (file.Peek() > -1)
			{
				string? line = file.ReadLine();
				if (line != null)
				{
					string[] name = line.Split(';');
					stockNames.Add(name[0], name[1]);
				}
			}
		}

		private void ReadHistoryFile(string filepath)
		{
			string stockName = GetStockNameFromFileName(filepath);
			string line = "";
			using var file = new System.IO.StreamReader(filepath);
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
				StockData sd = new(stockName, values[0], values[6]);
				stocks.Add(sd);
			}
		}
		
		private string GetStockNameFromFileName(string filepath)
		{
			string filename = System.IO.Path.GetFileName(filepath);
			string[] name = filename.Split('_');
			if (stockNames.ContainsKey(name[0].ToUpper()))
			{
				return stockNames[name[0].ToUpper()];
			}
			return name[0];
		}

		private string GetStockData()
		{
			foreach (var item in stocks.OrderBy(s => s.CompanyName).ThenBy(s => s.Date))
			{
				sb.AppendLine(item.ToString());
			}
			string result = sb.ToString();
			Clipboard.SetText(result);
			return result;
		}
		#endregion

		#region events
		#endregion

		#region event handlers
		#endregion

		private class StockData
		{
			public string CompanyName { get; private set; }
			public DateTime Date { get; private set; }
			public decimal ClosingPrice { get; private set; }

			public StockData(string name, string date, string closingPrice)
			{
				CompanyName = name;
				//2025-05-23
				Date = DateTime.Parse(date);
				ClosingPrice = decimal.Parse(closingPrice, System.Globalization.CultureInfo.InvariantCulture);
			}

			public override string ToString()
			{
				return $"{CompanyName}\t{Date:dd.MM.yyyy}\t{ClosingPrice}";
			}
		}

	}
}
