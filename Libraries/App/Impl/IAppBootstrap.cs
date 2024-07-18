using System;
using System.Threading.Tasks;

namespace Redbean
{
	public interface IAppBootstrap : IExtension, IDisposable
	{
		/// <summary>
		/// 실행 타입
		/// </summary>
		AppBootstrapType ExecutionType { get; }
		
		/// <summary>
		/// 실행 순서
		/// </summary>
		int ExecutionOrder { get; }

		/// <summary>
		/// 메모리 해제 순서
		/// </summary>
		int Order => (int)ExecutionType + ExecutionOrder;
		
		/// <summary>
		/// 앱 시작 시 실행되는 함수
		/// </summary>
		Task Setup();
	}
}