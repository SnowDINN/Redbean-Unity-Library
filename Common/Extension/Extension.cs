using System;
using System.Threading;
using Redbean.MVP;

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
		private static T ExtensionGetSingleton<T>() where T : class, ISingleton => SingletonContainer.GetSingleton<T>();
		
		/// <summary>
		/// 싱글톤 호출
		/// </summary>
		private static object ExtensionGetSingleton(Type type) => SingletonContainer.GetSingleton(type);
		
		/// <summary>
		/// 모델 호출
		/// </summary>
		private static T ExtensionGetModel<T>() where T : class, IModel => MvpContainer.GetModel<T>();
		
		/// <summary>
		/// 모델 호출
		/// </summary>
		private static object ExtensionGetModel(Type type) => MvpContainer.GetModel(type);
		
		/// <summary>
		/// API 호출
		/// </summary>
		private static T ExtensionGetProtocol<T>() where T : class, IApiProtocol => ApiContainer.GetProtocol<T>();
		
		/// <summary>
		/// API 호출
		/// </summary>
		private static object ExtensionGetProtocol(Type type) => ApiContainer.GetProtocol(type);
		
		/// <summary>
		/// 싱글톤 호출
		/// </summary>
		public static T GetSingleton<T>(this IExtension extension) where T : class, ISingleton => ExtensionGetSingleton<T>();
		
		/// <summary>
		/// 싱글톤 호출
		/// </summary>
		public static object GetSingleton(this IExtension extension, Type type) => ExtensionGetSingleton(type);
		
		/// <summary>
		/// 모델 호출
		/// </summary>
		public static T GetModel<T>(this IExtension extension) where T : class, IModel => ExtensionGetModel<T>();
		
		/// <summary>
		/// 모델 호출
		/// </summary>
		public static object GetModel(this IExtension extension, Type type) => ExtensionGetModel(type);
		
		/// <summary>
		/// API 호출
		/// </summary>
		public static T GetProtocol<T>(this IExtension extension) where T : class, IApiProtocol => ExtensionGetProtocol<T>();
		
		/// <summary>
		/// API 호출
		/// </summary>
		public static object GetProtocol(this IExtension extension, Type type) => ExtensionGetProtocol(type);
		
#if UNITY_EDITOR
		/// <summary>
		/// API 호출
		/// </summary>
		public static T EditorGetApi<T>(this IExtension extension) where T : class, IApiProtocol => ExtensionGetProtocol<T>();
#endif
	}
}