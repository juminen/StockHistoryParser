using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace StockHistoryParser
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		#region constructors
		public MainWindow()
		{
			InitializeComponent();
			LoadSettings();
			StockNameFileTextBox.Text = appSettings.LastStockNameFilePath;
			WindowState = WindowState.Maximized;
		}
		#endregion

		#region properties
		private StockHistoryParserAppSettings appSettings = new();
		#endregion

		#region methods
		public void SaveSettings()
		{
			JsonSerializerOptions options = new JsonSerializerOptions() { WriteIndented = true };
			string s = JsonSerializer.Serialize(appSettings, options);
			File.WriteAllText(appSettings.FilePath, s);
		}

		public void LoadSettings()
		{
			if (File.Exists(appSettings.FilePath))
			{
				try
				{
					StockHistoryParserAppSettings? s =
						JsonSerializer.Deserialize<StockHistoryParserAppSettings>
						(File.ReadAllText(appSettings.FilePath));
					if (s != null)
					{
						appSettings = s;
					}
				}
				catch (Exception)
				{

				}
			}
		}
		#endregion

		#region events
		#endregion

		#region event handlers
		private void SelectFilesButton_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog dlg = new()
			{
				CheckFileExists = true,
				CheckPathExists = true,
				Filter = "csv files (*.csv)| *.csv",
				AddExtension = true,
				Multiselect = true,
				Title = "Select files"
			};

			if (dlg.ShowDialog() == false)
			{
				StockDataTextBox.Text = "File selection canceled/failed.";
				return;
			}
			HistoryParser parser = new();
			StockDataTextBox.Text = parser.ProcessFiles(StockNameFileTextBox.Text, dlg.FileNames);
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (!StockNameFileTextBox.Text.Equals(appSettings.LastStockNameFilePath) &&
				File.Exists(StockNameFileTextBox.Text))
			{
				appSettings.LastStockNameFilePath = StockNameFileTextBox.Text;
				SaveSettings();
			}
		}		
		
		private void StockDataTextBox_Drop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
				HistoryParser parser = new();
				StockDataTextBox.Text = parser.ProcessFiles(StockNameFileTextBox.Text, files);
			}
		}

		private void StockDataTextBox_PreviewDragOver(object sender, DragEventArgs e)
		{
			e.Effects = DragDropEffects.All;
			e.Handled = true;
		}

		private void ClearListButton_Click(object sender, RoutedEventArgs e)
		{
			StockDataTextBox.Clear();
		}
		#endregion
	}
}
