using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Redbean.Security;
using Redbean.MVP;
using UnityEngine;

namespace Redbean.Singleton
{
	public class MvpSingleton : ISingletonContainer
	{
		public const string PLAYER_PREFS_KEY = "PLAYER_PREFS__DATA_GROUP";
		
		private readonly Dictionary<string, string> playerPrefsGroup = new();
		private readonly Dictionary<Type, IModel> modelGroup = new();

		public MvpSingleton()
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

#region PlayerPrefs

			if (PlayerPrefs.HasKey(PLAYER_PREFS_KEY))
			{
				var dataDecrypt = PlayerPrefs.GetString(PLAYER_PREFS_KEY).Decryption();
				var dataGroups = JsonConvert.DeserializeObject<Dictionary<string, string>>(dataDecrypt);
				foreach (var dataGroup in dataGroups)
				{
					var key = Assembly.Load("Assembly-CSharp").GetTypes().FirstOrDefault(_ => _.FullName == dataGroup.Key);
					var value = JsonConvert.DeserializeObject(dataGroup.Value, key);

					if (value is IModel model)
						modelGroup[key] = model;
				}

				playerPrefsGroup = dataGroups;
			}

#endregion
		}

		public void Dispose()
		{
			modelGroup.Clear();
			playerPrefsGroup.Clear();
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
		public T Override<T>(T value, bool isPlayerPrefs = false) where T : IModel
		{
			var targetFields = modelGroup[value.GetType()].GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).ToArray();
			var copyFields = value.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).ToArray();
			
			for (var i = 0; i < targetFields.Length; i++)
				targetFields[i].SetValue(modelGroup[value.GetType()], copyFields[i].GetValue(value));

			if (isPlayerPrefs)
				Save(value);
			
			return value;
		}
		
#region PlayerPrefs
		
		/// <summary>
		/// 로컬 데이터 저장 및 퍼블리싱
		/// </summary>
		public T Save<T>(T value)
		{
			playerPrefsGroup[typeof(T).FullName] = JsonConvert.SerializeObject(value);
			
			var encryptValue = JsonConvert.SerializeObject(playerPrefsGroup).Encryption();
			PlayerPrefs.SetString(PLAYER_PREFS_KEY, encryptValue);
			
			return value;
		}
		
		/// <summary>
		/// 로컬 데이터 호출
		/// </summary>
		public T Load<T>()
		{
			return playerPrefsGroup.TryGetValue(typeof(T).FullName, out var value) 
				? JsonConvert.DeserializeObject<T>(value) 
				: default;
		}
		
#endregion
	}
}