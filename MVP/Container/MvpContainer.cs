using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Redbean.MVP;

namespace Redbean.Singleton
{
	public class MvpContainer : ISingletonContainer
	{
		private readonly Dictionary<Type, IModel> modelGroup = new();

		public MvpContainer()
		{
			var models = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(x => x.GetTypes())
				.Where(x => typeof(IModel).IsAssignableFrom(x)
				            && !typeof(IRxModel).IsAssignableFrom(x)
				            && !x.IsInterface
				            && !x.IsAbstract)
				.Select(x => Activator.CreateInstance(x) as IModel)
				.ToArray();

			foreach (var model in models)
				modelGroup.TryAdd(model.GetType(), model);
		}

		public void Dispose()
		{
			modelGroup.Clear();
		}

		/// <summary>
		/// 모델 전부 제거
		/// </summary>
		public void Clear() => modelGroup.Clear();

		/// <summary>
		/// 모델 호출
		/// </summary>
		public T GetModel<T>() where T : IModel => (T)modelGroup[typeof(T)];

		/// <summary>
		/// 모델 호출
		/// </summary>
		public IModel GetModel(Type type) => modelGroup[type];

		/// <summary>
		/// 모델 재정의
		/// </summary>
		public T Override<T>(T value) where T : IModel
		{
			var targetFields = modelGroup[value.GetType()].GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).ToArray();
			var copyFields = value.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).ToArray();
			
			for (var i = 0; i < targetFields.Length; i++)
				targetFields[i].SetValue(modelGroup[value.GetType()], copyFields[i].GetValue(value));
			
			return value;
		}
	}
}