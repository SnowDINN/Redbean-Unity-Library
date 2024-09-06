using System.Threading;
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
		public static bool IsAppChecked { get; private set; }
		public static bool IsAppReady { get; private set; }
		
		public static GameObject AudioSystem { get; private set; }
		public static GameObject EventSystem { get; private set; }
		
		public delegate void onAppExit();
		public static event onAppExit OnAppExit;

		private void Awake()
		{
			AudioSystem = new GameObject("[Audio System]", typeof(AudioSource), typeof(AudioSource), typeof(AudioSource), typeof(AudioSource));
			AudioSystem.transform.SetParent(transform);
			
			EventSystem = new GameObject("[Event System]", typeof(EventSystem), typeof(StandaloneInputModule));
			EventSystem.transform.SetParent(transform);
			
			AppSettings.BootstrapSetup(BootstrapKey.RUNTIME);
			
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