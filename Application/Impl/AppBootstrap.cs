using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Redbean
{
	public class AppBootstrap
	{
		private static readonly Dictionary<string, IAppBootstrap[]> Bootstraps = new();
		
		[RuntimeInitializeOnLoadMethod]
		public static void RuntimeBootstrap()
		{
			Application.runInBackground = true;
			Application.targetFrameRate = 60;
			
			var go = new GameObject("[Application Life Cycle]", typeof(AppLifeCycle));
			Object.DontDestroyOnLoad(go);
		}

		public static async Task BootstrapSetup(string type)
		{
			var bootstrapContexts = AppSettings.RuntimeBootstrap.Where(_ => _.BootstrapType == type).ToArray();
			var bootstraps = bootstrapContexts.Select(_ => Activator.CreateInstance(Type.GetType(_.BootstrapName)) as IAppBootstrap).ToArray();
	
			Bootstraps[type] = bootstraps;
			
			foreach (var bootstrap in bootstraps)
				await bootstrap.Setup();
		}

		public static async Task BootstrapDispose()
		{
			var bootstrapGroup = new List<IAppBootstrap>();
			foreach (var bootstraps in Bootstraps.Values)
				bootstrapGroup.AddRange(bootstraps);

			bootstrapGroup.Reverse();
			foreach (var bootstrap in bootstrapGroup)
				await bootstrap.Teardown();
		}
	}
}