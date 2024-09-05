using System;
using System.Linq;
using System.Reflection;

namespace Redbean.MVP
{
	public class MvpContainer : Container<Type, IModel>
	{
		/// <summary>
		/// 모델 호출
		/// </summary>
		public static T GetModel<T>() where T : class, IModel
		{
			if (!container.ContainsKey(typeof(T)))
				container[typeof(T)] = Activator.CreateInstance<T>();

			return container[typeof(T)] as T;
		}

		/// <summary>
		/// 모델 호출
		/// </summary>
		public static IModel GetModel(Type type)
		{
			if (!container.ContainsKey(type))
				container[type] = Activator.CreateInstance(type) as IModel;

			return container[type];
		}

		/// <summary>
		/// 모델 재정의
		/// </summary>
		public static T Override<T>(T value) where T : class, IModel
		{
			var model = GetModel<T>();
			
			var targetFields = model.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).ToArray();
			var copyFields = value.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).ToArray();
			
			for (var i = 0; i < targetFields.Length; i++)
				targetFields[i].SetValue(model, copyFields[i].GetValue(value));
			
			return value;
		}
	}
}