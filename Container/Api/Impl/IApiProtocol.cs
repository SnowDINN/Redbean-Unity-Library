using System.Threading;
using System.Threading.Tasks;
using Redbean.Api;

namespace Redbean
{
	public interface IApiProtocol : IExtension
	{
		Task<ApiResponse> RequestAsync(CancellationToken cancellationToken = default);
	}
}