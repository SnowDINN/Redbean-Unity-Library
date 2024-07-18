using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Redbean
{
	public enum AppBootstrapType
	{
		Runtime = 0,
		Login = 100,
	}
	
	public class AppBootstrap
	{
		private static readonly Dictionary<AppBootstrapType, IAppBootstrap[]> Bootstraps = new();
		
		[RuntimeInitializeOnLoadMethod]
		public static void RuntimeBootstrap()
		{
			Application.runInBackground = true;
			Application.targetFrameRate = 60;
			
			var go = new GameObject("[Application Life Cycle]");
			go.AddComponent<AppLifeCycle>();
			
			Object.DontDestroyOnLoad(go);
		}

		public static async Task BootstrapInitialize()
		{
			var bootstraps = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(_ => _.GetTypes())
				.Where(_ => typeof(IAppBootstrap).IsAssignableFrom(_)
				            && !_.IsInterface
				            && !_.IsAbstract)
				.Select(_ => Activator.CreateInstance(Type.GetType(_.FullName)) as IAppBootstrap)
				.ToArray();

			var flags = Enum.GetNames(typeof(AppBootstrapType));
			foreach (var flag in flags)
			{
				var type = Enum.Parse<AppBootstrapType>(flag);
				Bootstraps[type] = bootstraps.Where(_ => _.ExecutionType == type).ToArray();
			}

			await BootstrapSetup(AppBootstrapType.Runtime);
		}

		public static async Task BootstrapSetup(AppBootstrapType type)
		{
			var orderBy = Bootstraps[type].OrderBy(_ => _.Order).ToArray();
			foreach (var bootstrap in orderBy)
				await bootstrap.Setup();
		}

		public static void BootstrapDispose()
		{
			var bootstrapGroup = new List<IAppBootstrap>();
			foreach (var bootstraps in Bootstraps.Values)
				bootstrapGroup.AddRange(bootstraps);

			var orderByDescending = bootstrapGroup.OrderByDescending(_ => _.Order).ToArray();
			foreach (var bootstrap in orderByDescending)
				bootstrap.Dispose();
		}
	}
}