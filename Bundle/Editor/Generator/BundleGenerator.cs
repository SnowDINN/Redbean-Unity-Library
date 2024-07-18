using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Redbean.Bundle
{
	public class BundleGenerator
	{
		private const string bundleDataAsset = "Assets/AddressableAssetsData/DataBuilders/BuildScriptPackedMode.asset";
		
		public static AddressableAssetBuildResult TryBuildBundle()
		{
			if (AssetDatabase.LoadAssetAtPath<ScriptableObject>(bundleDataAsset) is not IDataBuilder builderScript)
				return default;
			
			var assetAtPath = AddressableAssetSettingsDefaultObject.Settings;
			if (assetAtPath)
			{
				var id = assetAtPath.profileSettings.GetProfileId("Assets");
				if (!string.IsNullOrEmpty(id))
					assetAtPath.activeProfileId = id;
				
				var index = assetAtPath.DataBuilders.IndexOf((ScriptableObject)builderScript);
				if (index > 0)
					assetAtPath.ActivePlayerDataBuilderIndex = index;
			}
			
			AddressableAssetSettings.BuildPlayerContent(out var result);
			return result;
		}
	}
}