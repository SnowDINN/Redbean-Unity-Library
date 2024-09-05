using System.Threading;
using System.Threading.Tasks;

namespace Redbean
{
	public class Bootstrap : IBootstrap
	{
		private readonly CancellationTokenSource source = new();
		protected CancellationToken cancellationToken => source.Token;
		
		public virtual Task Setup()
		{
			AppLifeCycle.OnAppExit += OnAppExit;
			
			return Task.CompletedTask;
		}

		public virtual void Teardown()
		{
			source?.Cancel();
		}

		private void OnAppExit()
		{
			AppLifeCycle.OnAppExit -= OnAppExit;

			Teardown();
		}
	}
}