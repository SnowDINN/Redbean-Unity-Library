using System;
using System.Threading.Tasks;
using Redbean.Base;
using UnityEngine;

namespace Redbean
{
	[Serializable]
	public class BootstrapContext
	{
		public string BootstrapName;
		public string BootstrapType;
	}
	
	[CreateAssetMenu(fileName = "App", menuName = "Redbean/Library/App")]
	public class AppInstaller : ScriptableObject
	{
		[Header("Get application information during runtime")]
		public string Version;
	}

	public class AppSettings : SettingsBase<AppInstaller>
	{
		public static string Version =>
			string.IsNullOrEmpty(Installer.Version) ? Application.version : Installer.Version;
		
		public static async Task BootstrapSetup<T>() where T : Bootstrap =>
			await Activator.CreateInstance<T>().Start();
	}
}