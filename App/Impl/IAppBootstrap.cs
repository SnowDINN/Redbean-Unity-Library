using System;
using System.Threading.Tasks;

namespace Redbean
{
	public interface IAppBootstrap : IExtension, IDisposable
	{
		/// <summary>
		/// 앱 시작 시 실행되는 함수
		/// </summary>
		Task Setup();
	}
}