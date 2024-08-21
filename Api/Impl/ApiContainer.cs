using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Redbean
{
	public class ApiContainer : IAppBootstrap
	{
		private static readonly Dictionary<Type, IApiContainer> apiGroup = new();
		public static readonly HttpClient Http = new(new HttpClientHandler
		{
			UseProxy = false,
		})
		{
			BaseAddress = new Uri("https://localhost:44395"),
			DefaultRequestHeaders =
			{
				{ "Version", AppSettings.Version },
			},
			Timeout = TimeSpan.FromSeconds(60),
		};
		
		public Task Setup()
		{
			var apis = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(x => x.GetTypes())
				.Where(_ => _.FullName != null
				            && typeof(IApiContainer).IsAssignableFrom(_)
				            && !_.IsInterface
				            && !_.IsAbstract)
				.Select(_ => Activator.CreateInstance(_) as IApiContainer)
				.ToArray();

			foreach (var api in apis)
				apiGroup.TryAdd(api.GetType(), api);
			
			Log.System("Api has been bind.");
			
			return Task.CompletedTask;
		}

		public void Dispose()
		{
			apiGroup.Clear();
			
			Log.System("Api has been terminated.");
		}

		public static async Task<object> RequestApi(Type type, params object[] args) => 
			await apiGroup[type].Request(args);

		public static async Task<object> RequestApi<T>(params object[] args) where T : IApiContainer =>
			await apiGroup[typeof(T)].Request(args);
	}
}