using System;
using System.Collections.Generic;
using System.Linq;

namespace Redbean.Table
{
	public partial class TableContainer : Container<string, string>
	{
		public static void SetTable(Dictionary<string, string> data) => container = data;
	
		public static void Setup()
		{
			foreach (var page in container)
			{
				var tsv = $"{page.Value}".Split("\r\n");
				
				// Skip Name and Type Rows
				var skipRows = tsv.Skip(2);
				foreach (var item in skipRows)
				{
					var type = Type.GetType($"{nameof(Redbean)}.Table.T{page.Key}");
					if (Activator.CreateInstance(type) is ITable instance)
						instance.Apply(item);
				}
			}

			Log.Success("TABLE", $"Success to load to the table. [ Sheet : {container.Count} ]");
		}

		public static void Setup(string key)
		{
			if (key.StartsWith('T'))
				key = key[1..];

			if (container.ContainsKey(key))
			{
				var page = container.FirstOrDefault(_ => _.Key == key);
				var tsv = $"{page.Value}".Split("\r\n");
				
				// Skip Name and Type Rows
				var skipRows = tsv.Skip(2);
				foreach (var item in skipRows)
				{
					var type = Type.GetType($"{nameof(Redbean)}.Table.T{page.Key}");
					if (Activator.CreateInstance(type) is ITable instance)
						instance.Apply(item);
				}
			}
			else
				Log.Fail("TABLE", $"Fail to load to the table. [ Sheet : {key} ]");
		}
	}
}