#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Redbean
{
	public class AppLifeCycle : MonoBase
	{
		public static bool IsAppChecked { get; private set; }
		public static bool IsAppReady { get; private set; }

		private async void Awake()
		{
			await AppBootstrap.BootstrapInitialize();
			
			IsAppReady = true;
		}

		public override void OnDestroy()
		{
			AppBootstrap.BootstrapDispose();
			
			IsAppReady = false;
			
#if UNITY_EDITOR
			if (EditorApplication.isPlaying)
				EditorApplication.isPlaying = false;
#endif
		}

		public static void AppCheckSuccess()
		{
			IsAppChecked = true;
		}
		
		public static void AppCheckFail()
		{
			IsAppChecked = false;
		}
	}
}