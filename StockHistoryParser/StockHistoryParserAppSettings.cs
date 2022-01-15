using System;
using System.IO;
using System.Text.Json.Serialization;

namespace StockHistoryParser
{
	class StockHistoryParserAppSettings
	{

		public StockHistoryParserAppSettings()
		{
			FilePath = AppContext.BaseDirectory + Path.DirectorySeparatorChar + 
				"StockHistoryParser_settings.json";
		}
		[JsonIgnore]
		public string FilePath { get; }
		public string LastStockNameFilePath { get; set; } = string.Empty;
	}
}
