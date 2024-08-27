using System;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Redbean
{
	public class AppLifeCycle : MonoBase
	{
		public static bool IsAppChecked { get; private set; }
		public static bool IsAppReady { get; private set; }
		
		public static GameObject AudioSystem { get; private set; }
		public static GameObject EventSystem { get; private set; }

		public static CancellationToken AppCancellationToken => cancellationTokenSource.Token;

		private static readonly CancellationTokenSource cancellationTokenSource = new();

		private async void Awake()
		{
			AudioSystem = new GameObject("[Audio System]", typeof(AudioSource), typeof(AudioSource), typeof(AudioSource), typeof(AudioSource));
			AudioSystem.transform.SetParent(transform);
			
			EventSystem = new GameObject("[Event System]", typeof(EventSystem), typeof(StandaloneInputModule));
			EventSystem.transform.SetParent(transform);

			
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

	public class DisableInteraction : IDisposable
	{
		public DisableInteraction()
		{
			AppLifeCycle.EventSystem.SetActive(false);
		}
		
		public void Dispose()
		{
			AppLifeCycle.EventSystem.SetActive(true);
		}
	}
}