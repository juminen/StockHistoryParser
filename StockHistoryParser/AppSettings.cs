//using System;
//using System.IO;
//using System.Text.Json;

//namespace StockHistoryParser
//{
//	class AppSettingsSerializer
//	{
//		public void Save(string filePath)
//		{
//			string s = JsonSerializer.Serialize(this);
//			File.WriteAllText(filePath, s);
//		}

//		public StockHistoryParserAppSettings Load()
//		{
//			StockHistoryParserAppSettings settings = new();
//			if (File.Exists(settings.FilePath))
//			{
//				try
//				{
//					StockHistoryParserAppSettings? s = 
//						JsonSerializer.Deserialize<StockHistoryParserAppSettings>
//						(File.ReadAllText(settings.FilePath));
//					if (s != null)
//					{
//						settings = s;
//					}
//				}
//				catch (Exception)
//				{

//				}
//			}
//			return settings;
//		}
//	}
//}
