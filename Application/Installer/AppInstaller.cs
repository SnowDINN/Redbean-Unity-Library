using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Redbean.Base;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace Redbean
{
	[Serializable]
	public class BootstrapContext
	{
		public string BootstrapName;
		public string BootstrapType;
	}
	
	[CreateAssetMenu(fileName = "App", menuName = "Redbean/App")]
	public class AppInstaller : ScriptableObject
	{
		[HideInInspector]
		public List<BootstrapContext> RuntimeBootstrap = new();
		public List<string> BootstrapTypes = new() { "RUNTIME" };
		
		[Header("Get application information during runtime")]
		public string Version;
	}

#if UNITY_EDITOR
#region UNITY EDITOR
	
	[CustomEditor(typeof(AppInstaller), true)]
	public class AppInstallerEditor : Editor
	{
		private AppInstaller app => target as AppInstaller;
		
		private List<string> bootstrapArray;
		private ReorderableList runtimeBootstrapRecorder;

		private void OnEnable()
		{
			bootstrapArray = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(x => x.GetTypes())
				.Where(x => typeof(IBootstrap).IsAssignableFrom(x)
				            && x.Name != nameof(Bootstrap)
				            && !x.IsInterface
				            && !x.IsAbstract)
				.Select(x => x.FullName)
				.ToList();

			runtimeBootstrapRecorder = new ReorderableList(app.RuntimeBootstrap,
			                                               typeof(BootstrapContext),
			                                               true, true, true, true);
			
			runtimeBootstrapRecorder.drawHeaderCallback += OnRuntimeBootstrapDrawHeaderCallback;
			runtimeBootstrapRecorder.drawElementCallback += OnRuntimeBootstrapDrawHeaderCallback;
			runtimeBootstrapRecorder.onChangedCallback += OnChangedCallback;
		}

		private void OnDisable()
		{
			runtimeBootstrapRecorder.drawHeaderCallback -= OnRuntimeBootstrapDrawHeaderCallback;
			runtimeBootstrapRecorder.drawElementCallback -= OnRuntimeBootstrapDrawHeaderCallback;
			runtimeBootstrapRecorder.onChangedCallback -= OnChangedCallback;
		}

		private void OnRuntimeBootstrapDrawHeaderCallback(Rect rect)
		{
			EditorGUI.LabelField(rect, "Runtime Bootstrap List");
		}
		
		private void OnRuntimeBootstrapDrawHeaderCallback(Rect rect, int index, bool isactive, bool isfocused)
		{
			EditorGUI.BeginChangeCheck();
			{
				var indexOfName = bootstrapArray.IndexOf(app.RuntimeBootstrap[index].BootstrapName);
				if (indexOfName < 0)
					indexOfName = 0;
				indexOfName = EditorGUI.Popup(new Rect(rect.x, rect.y + 2.5f, rect.width * 0.75f - 5, rect.height), indexOfName, bootstrapArray.ToArray());
				app.RuntimeBootstrap[index].BootstrapName = bootstrapArray[indexOfName];
				
				var indexOfType = app.BootstrapTypes.IndexOf(app.RuntimeBootstrap[index].BootstrapType);
				if (indexOfType < 0)
					indexOfType = 0;
				indexOfType = EditorGUI.Popup(new Rect(rect.x + rect.width * 0.75f, rect.y + 2.5f, rect.width  * 0.25f, rect.height), indexOfType, app.BootstrapTypes.ToArray());
				app.RuntimeBootstrap[index].BootstrapType = app.BootstrapTypes[indexOfType];
			}
			if (!EditorGUI.EndChangeCheck())
				return;

			EditorUtility.SetDirty(app);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
		
		private void OnChangedCallback(ReorderableList list)
		{
			EditorUtility.SetDirty(app);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		public override void OnInspectorGUI()
		{
			EditorGUILayout.LabelField("Sorting Execution Bootstrap", EditorStyles.boldLabel);
			
			serializedObject.Update();
			{
				runtimeBootstrapRecorder.DoLayoutList();
				DrawPropertiesExcluding(serializedObject, "m_Script");
			}
			serializedObject.ApplyModifiedProperties();
		}
	}
	
#endregion
#endif

	public class AppSettings : SettingsBase<AppInstaller>
	{
		public static string Version =>
			string.IsNullOrEmpty(Installer.Version) ? Application.version : Installer.Version;
		
		public static void BootstrapSetup(string type)
		{
			var bootstrapContexts = Installer.RuntimeBootstrap.Where(_ => _.BootstrapType == type).ToArray();
			var bootstraps = bootstrapContexts.Select(_ => Activator.CreateInstance(Type.GetType(_.BootstrapName)) as Bootstrap).ToArray();
			
			foreach (var bootstrap in bootstraps)
				bootstrap.Start();
		}
	}
}