using System;
using System.Threading;
using System.Threading.Tasks;
using Redbean.MVP;
using Redbean.Singleton;

namespace Redbean
{
	public static partial class Extension
	{
		/// <summary>
		/// 토큰 취소 및 할당 해제
		/// </summary>
		public static void CancelAndDispose(this CancellationTokenSource cancellationTokenSource)
		{
			if (!cancellationTokenSource.IsCancellationRequested)
				cancellationTokenSource.Cancel();
		
			cancellationTokenSource.Dispose();
		}
		
		public static async Task WaitWhile(this IExtension extension, Func<bool> condition, int frequency = 25, int timeout = -1)
		{
			var waitTask = Task.Run(async () =>
			{
				while (condition()) await Task.Delay(frequency);
			});

			if(waitTask != await Task.WhenAny(waitTask, Task.Delay(timeout)))
				throw new TimeoutException();
		}
		
		public static async Task WaitUntil(this IExtension extension, Func<bool> condition, int frequency = 25, int timeout = -1)
		{
			var waitTask = Task.Run(async () =>
			{
				while (!condition()) await Task.Delay(frequency);
			});

			if (waitTask != await Task.WhenAny(waitTask, Task.Delay(timeout))) 
				throw new TimeoutException();
		}
		
		/// <summary>
		/// 싱글톤 호출
		/// </summary>
		private static T GetSingleton<T>() where T : ISingletonContainer => SingletonContainer.GetSingleton<T>();
		
		/// <summary>
		/// 싱글톤 호출
		/// </summary>
		private static object GetSingleton(Type type) => SingletonContainer.GetSingleton(type);
		
		/// <summary>
		/// 모델 호출
		/// </summary>
		private static T GetModel<T>() where T : IModel => SingletonContainer.GetSingleton<MvpSingleton>().GetModel<T>();
		
		/// <summary>
		/// 모델 호출
		/// </summary>
		private static object GetModel(Type type) => SingletonContainer.GetSingleton<MvpSingleton>().GetModel(type);
		
		/// <summary>
		/// 모델 호출
		/// </summary>
		public static T GetModel<T>(this IExtension extension) where T : IModel => GetModel<T>();
		
		/// <summary>
		/// 모델 호출
		/// </summary>
		public static object GetModel(this IExtension extension, Type type) => GetModel(type);
		
		/// <summary>
		/// 싱글톤 호출
		/// </summary>
		public static T GetSingleton<T>(this IExtension extension) where T : ISingletonContainer => GetSingleton<T>();
		
		/// <summary>
		/// 싱글톤 호출
		/// </summary>
		public static object GetSingleton(this IExtension extension, Type type) => GetSingleton(type);
	}
}