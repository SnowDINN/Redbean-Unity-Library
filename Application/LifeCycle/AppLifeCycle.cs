
using System.Threading;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Redbean
{
	public class AppLifeCycle : MonoBase
	{
		public static bool IsAppChecked { get; private set; }
		public static bool IsAppReady { get; private set; }

		public static CancellationToken AppCancellationToken => cancellationTokenSource.Token;

		private static readonly CancellationTokenSource cancellationTokenSource = new();

		private async void Awake()
		{
			await AppBootstrap.BootstrapSetup(AppBootstrapType.Runtime);
			
			IsAppReady = true;
		}

		public override async void OnDestroy()
		{
			await AppBootstrap.BootstrapDispose();
			
			IsAppReady = false;
			
			cancellationTokenSource.Cancel();
			cancellationTokenSource.Dispose();
			
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