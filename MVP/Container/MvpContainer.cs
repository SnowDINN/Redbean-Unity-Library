using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Redbean.MVP;

namespace Redbean.Singleton
{
	public class MvpContainer
	{
		private static readonly Dictionary<Type, IModel> models = new();

		/// <summary>
		/// 모델 전부 제거
		/// </summary>
		public void Clear() => models.Clear();
		public void Dispose() => models.Clear();

		/// <summary>
		/// 모델 호출
		/// </summary>
		public static T GetModel<T>() where T : class, IModel
		{
			if (!models.ContainsKey(typeof(T)))
				models[typeof(T)] = Activator.CreateInstance<T>();

			return (T)models[typeof(T)];
		}

		/// <summary>
		/// 모델 호출
		/// </summary>
		public static IModel GetModel(Type type)
		{
			if (!models.ContainsKey(type))
				models[type] = Activator.CreateInstance(type) as IModel;

			return models[type];
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