using System.Collections.Generic;
using Newtonsoft.Json;
using Redbean.Security;
using UnityEngine;

namespace Redbean.Utility
{
	public class LocalDatabase
	{
		public const string PLAYER_PREFS_KEY = "PLAYER_PREFS_DATA_GROUP";
		
		private static Dictionary<string, string> playerPrefsGroup = new();

		public static void Setup()
		{
			if (!PlayerPrefs.HasKey(PLAYER_PREFS_KEY))
				return;

			var dataDecrypt = PlayerPrefs.GetString(PLAYER_PREFS_KEY).Decryption();
			var dataGroups = JsonConvert.DeserializeObject<Dictionary<string, string>>(dataDecrypt);

			playerPrefsGroup = dataGroups;
		}
		
		/// <summary>
		/// 로컬 데이터 저장 및 퍼블리싱
		/// </summary>
		public static T Save<T>(string key, T value)
		{
			playerPrefsGroup[key] = JsonConvert.SerializeObject(value);
			
			var encryptValue = JsonConvert.SerializeObject(playerPrefsGroup).Encryption();
			PlayerPrefs.SetString(PLAYER_PREFS_KEY, encryptValue);
			
			return value;
		}
		
		/// <summary>
		/// 로컬 데이터 호출
		/// </summary>
		public static T Load<T>(string key)
		{
			return playerPrefsGroup.TryGetValue(key, out var value) 
				? JsonConvert.DeserializeObject<T>(value) 
				: default;
		}
	}
}