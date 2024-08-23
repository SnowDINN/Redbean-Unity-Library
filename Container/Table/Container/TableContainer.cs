using System;
using System.Collections.Generic;
using System.Linq;

namespace Redbean.Table
{
	public partial class TableContainer
	{
		public static void StartParsing(Dictionary<string, string> data)
		{
			if (data.Any())
				Parse(data);
		}

		private static void Parse(Dictionary<string, string> tables)
		{
			foreach (var table in tables)
			{
				var tsv = $"{table.Value}".Split("\r\n");
				
				// Skip Name and Type Rows
				var skipRows = tsv.Skip(2);
				foreach (var item in skipRows)
				{
					var type = Type.GetType($"{nameof(Redbean)}.Table.T{table.Key}");
					if (Activator.CreateInstance(type) is ITableContainer instance)
						instance.Apply(item);
				}
			}
		}
	}
}