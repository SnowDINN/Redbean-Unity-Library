using System.Threading;
using System.Threading.Tasks;
using Redbean.Api;
using Redbean.Utility;

namespace Redbean
{
	public class ApiProtocol : IApiProtocol
	{
		protected object[] args;
		
		public ApiProtocol Parameter(params object[] args)
		{
			this.args = args;
			return this;
		}

		public async Task<ApiResponse> RequestAsync(CancellationToken cancellationToken = default)
		{
			ApiContainer.OnRequestPublish(GetType());

			var response = new ApiResponse();
			using (new DisableInteraction())
				response = await Request(cancellationToken);
			
			ApiContainer.OnResponsePublish(GetType(), response);

			return response;
		}

		protected virtual Task<ApiResponse> Request(CancellationToken cancellationToken = default)
		{
			return default;
		}
	}
}