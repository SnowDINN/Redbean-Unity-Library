using System;
using UnityEngine;

namespace Redbean
{
	public class SingletonContainer : Container<Type, ISingleton>
	{
		private static GameObject go;
		
		/// <summary>
		/// 싱글톤 호출
		/// </summary>
		public static T GetSingleton<T>() where T : class, ISingleton
		{
			if (!container.ContainsKey(typeof(T)))
				container[typeof(T)] = Activator.CreateInstance<T>();

			return container[typeof(T)] as T;
		}

		/// <summary>
		/// 싱글톤 호출
		/// </summary>
		public static ISingleton GetSingleton(Type type)
		{
			if (!container.ContainsKey(type))
				container[type] = Activator.CreateInstance(type) as ISingleton;

			return container[type];
		}
	}
}