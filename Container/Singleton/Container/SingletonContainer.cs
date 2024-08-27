using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Redbean
{
	public class SingletonContainer : IAppBootstrap
	{
		private static readonly Dictionary<Type, ISingletonContainer> singletonGroup = new();
		private GameObject parent;
		
		public Task Setup()
		{
#region Native

			var nativeSingletons = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(x => x.GetTypes())
				.Where(x => x.FullName != null
				            && typeof(ISingletonContainer).IsAssignableFrom(x)
				            && !typeof(MonoBehaviour).IsAssignableFrom(x)
				            && !x.IsInterface
				            && !x.IsAbstract)
				.Select(x => Activator.CreateInstance(x) as ISingletonContainer)
				.ToArray();

			foreach (var nativeSingleton in nativeSingletons)
				singletonGroup.TryAdd(nativeSingleton.GetType(), nativeSingleton);

#endregion

#region Mono

			var monoSingletons = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(x => x.GetTypes())
				.Where(x => x.FullName != null
				            && typeof(ISingletonContainer).IsAssignableFrom(x)
				            && typeof(MonoBehaviour).IsAssignableFrom(x)
				            && !x.IsInterface
				            && !x.IsAbstract)
				.ToArray();
			
			if (monoSingletons.Any())
			{
				parent = new GameObject("[Singleton Group]");
				Object.DontDestroyOnLoad(parent);
			}

			foreach (var monoSingleton in monoSingletons)
				singletonGroup.TryAdd(monoSingleton, parent.AddComponent(monoSingleton) as ISingletonContainer);

#endregion
			return Task.CompletedTask;
		}

		public Task Teardown()
		{
			foreach (var singleton in singletonGroup.Values)
				singleton.Dispose();
			
			return Task.CompletedTask;
		}

		/// <summary>
		/// 싱글톤 전부 제거
		/// </summary>
		public static void Clear() => singletonGroup.Clear();
		
		/// <summary>
		/// 싱글톤 호출
		/// </summary>
		public static ISingletonContainer GetSingleton(Type type) => singletonGroup[type];
		
		/// <summary>
		/// 싱글톤 호출
		/// </summary>
		public static T GetSingleton<T>() where T : ISingletonContainer => (T)singletonGroup[typeof(T)];
	}
}