using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Redbean
{
	public class AppStart
	{
		[RuntimeInitializeOnLoadMethod]
		public static void Start()
		{
			Application.runInBackground = true;
			Application.targetFrameRate = 60;
			
			var go = new GameObject("[Application Life Cycle]", typeof(AppLifeCycle));
			Object.DontDestroyOnLoad(go);
		}
	}
	
	public class AppLifeCycle : MonoBase
	{
		public delegate void onAppExit();
		public static event onAppExit OnAppExit;

		public static Transform Transform { get; private set; }
		public static bool IsAppChecked { get; private set; }
		public static bool IsAppReady { get; private set; }

		private async void Awake()
		{
			Transform = transform;
			
			await AppSettings.BootstrapSetup<OnSystemBootstrap>();
			await AppSettings.BootstrapSetup<OnValidationBootstrap>();
			
			IsAppReady = true;
		}

		public override void OnDestroy()
		{
			IsAppReady = false;
			OnAppExit?.Invoke();
			
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