using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Redbean.Table
{
	public partial class TableContainer : IAppBootstrap
	{
		public static Dictionary<string, string> RawTable = new();

		public Task Setup()
		{
			if (RawTable.Any())
				Parse(RawTable);
			
			return Task.CompletedTask;
		}

		public void Dispose()
		{
			RawTable.Clear();
		}

		private void Parse(Dictionary<string, string> tables)
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