using System;
using System.Collections.Generic;
using UnityEngine;

namespace Redbean
{
	public class SingletonContainer
	{
		private static readonly Dictionary<Type, ISingleton> singletons = new();
		private static GameObject go;
		
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