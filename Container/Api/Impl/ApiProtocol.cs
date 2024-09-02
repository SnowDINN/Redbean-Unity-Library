using System.Threading;
using System.Threading.Tasks;
using Redbean.Api;
using Redbean.Rx;
using Redbean.Utility;

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

		public async Task<ApiResponse> RequestAsync(CancellationToken cancellationToken = default)
		{
			var response = new ApiResponse();
			
			RxApiBinder.OnRequestPublish(GetType());

			using (new DisableInteraction())
				response = await Request(cancellationToken) as ApiResponse;
			
			RxApiBinder.OnResponsePublish(GetType(), response);

			return response;
		}

		protected virtual Task<IApiResponse> Request(CancellationToken cancellationToken = default)
		{
			return default;
		}
	}
}