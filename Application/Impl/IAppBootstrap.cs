using System;
using System.Threading.Tasks;

namespace Redbean
{
	public interface IAppBootstrap : IExtension
	{
		/// <summary>
		/// 앱 시작 시 실행되는 함수
		/// </summary>
		Task Setup();
		
		/// <summary>
		/// 앱 종료 시 실행되는 함수
		/// </summary>
		Task Teardown();
	}
}