using System;
using System.Threading;
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
		private static T GetModel<T>() where T : IModel => SingletonContainer.GetSingleton<MvpContainer>().GetModel<T>();
		
		/// <summary>
		/// 모델 호출
		/// </summary>
		private static object GetModel(Type type) => SingletonContainer.GetSingleton<MvpContainer>().GetModel(type);
		
		/// <summary>
		/// API 호출
		/// </summary>
		private static T GetProtocol<T>() where T : ApiProtocol => SingletonContainer.GetSingleton<ApiContainer>().GetProtocol<T>();
		
		/// <summary>
		/// API 호출
		/// </summary>
		private static object GetProtocol(Type type) => SingletonContainer.GetSingleton<ApiContainer>().GetProtocol(type);
		
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
		
		/// <summary>
		/// API 호출
		/// </summary>
		public static T GetProtocol<T>(this IExtension extension) where T : ApiProtocol => GetProtocol<T>();
		
		/// <summary>
		/// API 호출
		/// </summary>
		public static object GetProtocol(this IExtension extension, Type type) => GetProtocol(type);
		
#if UNITY_EDITOR
		/// <summary>
		/// API 호출
		/// </summary>
		public static T EditorGetApi<T>(this IExtension extension) where T : ApiProtocol => GetProtocol<T>();
#endif
	}
}