using System.Threading;
using System.Threading.Tasks;

namespace Redbean
{
	public interface IApiProtocol : IExtension
	{
		Task<object> RequestAsync(CancellationToken cancellationToken = default);
	}
}