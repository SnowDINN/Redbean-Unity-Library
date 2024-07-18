using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Redbean.Base
{
	public class SettingsBase<T> where T : ScriptableObject
	{
		private static readonly string resourceLocation = $"Settings/{typeof(T).Name.Replace("Installer", "")}";
		
		private static T installer;
		protected static T Installer
		{
			get
			{
				if (!installer)
					installer = Resources.Load<T>(resourceLocation);

				return installer;
			}
		}
		

		public static void Save()
		{
#if UNITY_EDITOR
			EditorUtility.SetDirty(Installer);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
#endif
		}
	}
}