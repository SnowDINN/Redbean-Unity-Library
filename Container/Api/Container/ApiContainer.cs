using System;
using System.Net.Http;
using Redbean.Api;

namespace Redbean
{
	public class ApiContainer : Container<Type, IApiProtocol>
	{
		public delegate void onRequest(Type Type);
		public static event onRequest OnRequest;
		
		public delegate void onResponse(Type Type, ApiResponse response);
		public static event onResponse OnResponse;
		
		public static readonly HttpClient Http = new(new HttpClientHandler
		{
			UseProxy = false,
		})
		{
			BaseAddress = new Uri(ApiSettings.ApiUri),
			DefaultRequestHeaders =
			{
				{ "Version", AppSettings.Version },
			},
			Timeout = TimeSpan.FromSeconds(60),
		};

		public static void AddProtocol(Type type, ApiProtocol apiProtocol) => container[type] = apiProtocol;

		public static void OnRequestPublish(Type type) => OnRequest?.Invoke(type);

		public static void OnResponsePublish(Type type, ApiResponse response) => OnResponse?.Invoke(type, response);

		public static T GetProtocol<T>() where T : class, IApiProtocol
		{
			if (!container.ContainsKey(typeof(T)))
				container[typeof(T)] = Activator.CreateInstance<T>();

			return container[typeof(T)] as T;
		}

		public static object GetProtocol(Type type)
		{
			if (!container.ContainsKey(type))
				container[type] = Activator.CreateInstance(type) as IApiProtocol;

			return container[type];
		}
	}
}