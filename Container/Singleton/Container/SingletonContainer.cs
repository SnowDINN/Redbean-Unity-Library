using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Redbean
{
	public interface ISingleton : IExtension, IDisposable
	{
	}	
	
	public class SingletonContainer
	{
		private static readonly Dictionary<Type, ISingleton> singletons = new();
		private static GameObject go;

		/// <summary>
		/// 싱글톤 전부 제거
		/// </summary>
		public static void Clear() => singletons.Clear();
		
		/// <summary>
		/// 싱글톤 호출
		/// </summary>
		public static T GetSingleton<T>() where T : class, ISingleton
		{
			if (!singletons.ContainsKey(typeof(T)))
				singletons[typeof(T)] = Activator.CreateInstance<T>();

			return singletons[typeof(T)] as T;
		}

		/// <summary>
		/// 싱글톤 호출
		/// </summary>
		public static ISingleton GetSingleton(Type type)
		{
			if (!singletons.ContainsKey(type))
				singletons[type] = Activator.CreateInstance(type) as ISingleton;

			return singletons[type];
		}
	}
}