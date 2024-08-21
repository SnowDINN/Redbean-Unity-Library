using System.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace Redbean.Bundle
{
	public class BundleBootstrap : IAppBootstrap
	{
		public async Task Setup()
		{
			var size = 0L;
			foreach (var label in AddressableSettings.Labels)
				size += await Addressables.GetDownloadSizeAsync(label).Task;

			if (size > 0)
			{
				foreach (var label in AddressableSettings.Labels)
				{
					var download = await Addressables.DownloadDependenciesAsync(label).Task;
					Addressables.Release(download);
				}
			}

			var convert = ConvertDownloadSize(size);
			Log.Success("Bundle", $"Success to load to the bundles. [ {convert.value}{convert.type} ]");
		}

		private static (string value, string type) ConvertDownloadSize(long size)
		{
			var value = (double)size;
			value /= 1024;
			if (value < 1024)
				return ($"{value:F1}", "KB");
			
			value /= 1024;
			if (value < 1024)
				return ($"{value:F1}", "MB");
			
			value /= 1024;
			return ($"{value:F1}", "GB");
		}

		public void Dispose()
		{
		}
	}
}