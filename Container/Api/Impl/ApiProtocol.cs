using System.Threading;
using System.Threading.Tasks;

namespace Redbean
{
	public class ApiProtocol : IApiProtocol
	{
		protected object[] args;
		
		public IApiProtocol Parameter(params object[] args)
		{
			this.args = args;
			return this;
		}

		public virtual Task<object> RequestAsync(CancellationToken cancellationToken = default)
		{
			var completionSource = new TaskCompletionSource<object>();
			return completionSource.Task;
		}
	}
}