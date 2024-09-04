using System;
using System.Collections.Generic;
using System.Net.Http;
using Redbean.Api;

namespace Redbean
{
	public class ApiContainer
	{
		private static readonly Dictionary<Type, ApiProtocol> apiProtocols = new();
		
		public delegate void onRequest(Type Type);
		public static event onRequest OnRequest;
		
		public delegate void onResponse(Type Type, ApiResponse response);
		public static event onResponse OnResponse;
		
		public static readonly HttpClient Http = new(new HttpClientHandler
		{
			UseProxy = false,
		})
		{
			BaseAddress = new Uri(AppSettings.ApiUri),
			DefaultRequestHeaders =
			{
				{ "Version", AppSettings.Version },
			},
			Timeout = TimeSpan.FromSeconds(60),
		};

		public static void AddProtocol(Type type, ApiProtocol apiProtocol) => apiProtocols[type] = apiProtocol;

		public static void OnRequestPublish(Type type) => OnRequest?.Invoke(type);

		public static void OnResponsePublish(Type type, ApiResponse response) => OnResponse?.Invoke(type, response);

		public static T GetProtocol<T>() where T : class, IApiProtocol
		{
			if (!apiProtocols.ContainsKey(typeof(T)))
				Activator.CreateInstance<T>();

			return apiProtocols[typeof(T)] as T;
		}

		public static object GetProtocol(Type type)
		{
			if (!apiProtocols.ContainsKey(type))
				Activator.CreateInstance(type);

			return apiProtocols[type];
		}
	}
}