using Redbean.Base;
using UnityEngine;

namespace Redbean
{
	[CreateAssetMenu(fileName = "Application", menuName = "Redbean/Application")]
	public class AppInstaller : ScriptableObject
	{
		[Header("Get application information during runtime")]
		public string Version;
	}

	public class AppSettings : SettingsBase<AppInstaller>
	{
		public const string ApiUri = "https://localhost:44395";

		public static string Version =>
			string.IsNullOrEmpty(Installer.Version) ? Application.version : Installer.Version;
	}
}