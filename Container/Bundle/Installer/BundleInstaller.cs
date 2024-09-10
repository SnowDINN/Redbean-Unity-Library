using System;
using Redbean.Base;
using UnityEngine;

namespace Redbean.Bundle
{
	[CreateAssetMenu(fileName = "Bundle", menuName = "Redbean/Library/Bundle")]
	public class BundleInstaller : ScriptableObject
	{
		[Header("Get addressable information during runtime")]
		public string[] Labels;
	}

	public class AddressableSettings : SettingsBase<BundleInstaller>
	{
		public static string GetPopupPath(Type type) => $"Popup/{type.Name}.prefab";
		
		public static string[] Labels
		{
			get => Installer.Labels;
			set
			{
				Installer.Labels = value;
				
#if UNITY_EDITOR
				Save();
#endif
			}
		}
	}
}