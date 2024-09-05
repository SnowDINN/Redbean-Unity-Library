using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Redbean.Bundle
{
	public class BundleContainer
	{
		private static Dictionary<string, BundleAsset> assets = new();
		
		public static async Task Setup()
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
			Log.Success("BUNDLE", $"Success to load to the bundle. [ {convert.value}{convert.type} ]");
		}

		public static T LoadAsset<T>(string key, Transform parent = null) where T : Object
		{
			var bundle = new BundleAsset();
			
			if (assets.TryGetValue(key, out var assetBundle))
				bundle = assetBundle;
			else
			{
				bundle = LoadBundle<T>(key);
				assets[key] = bundle;
			}
			
			var asset = Object.Instantiate(bundle.Asset as T, parent);
			assets[key].References[asset.GetInstanceID()] = asset;
			
			return asset;
		}

		public static void Release(string key, int instanceId)
		{
#region Try Get Asset

			if (!assets.TryGetValue(key, out var assetBundle))
				return;

			if (assetBundle.References.Remove(instanceId, out var go))
				Object.Destroy(go);

#endregion

#region Check Use Reference

			if (assetBundle.References.Any())
				return;

			if (assets.Remove(key, out var removeBundle))
				removeBundle.Release();

#endregion
		}

		public static void AutoRelease()
		{
			var assetsArray = assets.ToList();
			for (var i = 0; i < assetsArray.Count; i++)
			{
				var referenceArray = assetsArray[i].Value.References.ToList();
				for (var j = 0; j < referenceArray.Count; j++)
				{
					if (!referenceArray[j].Value)
						referenceArray.RemoveAt(j);
				}
				
				assetsArray[i].Value.References = referenceArray.ToDictionary(_ => _.Key, _ => _.Value);
				if (!assetsArray[i].Value.References.Any())
				{
					assetsArray[i].Value.Release();
					assetsArray.RemoveAt(i);
				}
			}
			
			assets = assetsArray.ToDictionary(_ => _.Key, _ => _.Value);
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

		private static BundleAsset LoadBundle<T>(string key) where T : Object
		{
			var value = Addressables.LoadAssetAsync<T>(key).WaitForCompletion();
			var bundle = new BundleAsset
			{
				Asset = value,
			};

			return bundle;
		}
	}
}