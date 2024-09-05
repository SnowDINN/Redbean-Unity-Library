using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Redbean.Table
{
	public partial class TableContainer
	{
		private static Dictionary<string, string> table;

		public static void SetTable(Dictionary<string, string> table) => TableContainer.table = table;
	
		public static Task Setup()
		{
			foreach (var sheet in table)
			{
				var tsv = $"{sheet.Value}".Split("\r\n");
				
				// Skip Name and Type Rows
				var skipRows = tsv.Skip(2);
				foreach (var item in skipRows)
				{
					var type = Type.GetType($"{nameof(Redbean)}.Table.T{sheet.Key}");
					if (Activator.CreateInstance(type) is ITable instance)
						instance.Apply(item);
				}
			}

			Log.Success("TABLE", $"Success to load to the table. [ Sheet : {table.Count} ]");
			return Task.CompletedTask;
		}
	}
}