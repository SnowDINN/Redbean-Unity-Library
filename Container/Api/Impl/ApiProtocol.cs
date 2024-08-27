using System.Threading;
using System.Threading.Tasks;
using Redbean.Rx;

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

		public async Task<object> RequestAsync(CancellationToken cancellationToken = default)
		{
			var response = new object();
			
			RxApiBinder.OnRequestPublish(GetType());

			using (new DisableInteraction())
				response = await Request(cancellationToken);
			
			RxApiBinder.OnResponsePublish(GetType(), response);

			return response;
		}

		protected virtual Task<object> Request(CancellationToken cancellationToken = default)
		{
			return Task.FromResult(new object());
		}
	}
}