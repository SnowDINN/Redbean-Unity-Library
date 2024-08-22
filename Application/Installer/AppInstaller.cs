using System.Collections.Generic;
using Redbean.Base;
using UnityEngine;

#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
#endif

namespace Redbean
{
	[Serializable]
	public class BootstrapContext
	{
		public string BootstrapName;
		public AppBootstrapType BootstrapType;
	}
	
	[CreateAssetMenu(fileName = "Application", menuName = "Redbean/Application")]
	public class AppInstaller : ScriptableObject
	{
		[HideInInspector]
		public List<BootstrapContext> RuntimeBootstrap = new();
		
		[Header("Get application information during runtime")]
		public string Version;
	}

#if UNITY_EDITOR
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
				.Where(x => typeof(IAppBootstrap).IsAssignableFrom(x)
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
				var indexOf = bootstrapArray.IndexOf(app.RuntimeBootstrap[index].BootstrapName);
				if (indexOf < 0)
					indexOf = 0;
				indexOf = EditorGUI.Popup(new Rect(rect.x, rect.y + 2.5f, rect.width * 0.75f - 5, rect.height), indexOf, bootstrapArray.ToArray());
				app.RuntimeBootstrap[index].BootstrapName = bootstrapArray[indexOf];
				app.RuntimeBootstrap[index].BootstrapType =
					(AppBootstrapType)EditorGUI.EnumPopup(new Rect(rect.x + rect.width * 0.75f, rect.y + 2.5f, rect.width  * 0.25f, rect.height),
					                                      app.RuntimeBootstrap[index].BootstrapType);
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
#endif

	public class AppSettings : SettingsBase<AppInstaller>
	{
		public static List<BootstrapContext> RuntimeBootstrap => Installer.RuntimeBootstrap;
		
		public const string ApiUri = "https://localhost:44395";

		public static string Version =>
			string.IsNullOrEmpty(Installer.Version) ? Application.version : Installer.Version;
	}
}