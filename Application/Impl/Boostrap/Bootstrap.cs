using System.Threading;
using System.Threading.Tasks;

namespace Redbean
{
	public class Bootstrap : IBootstrap
	{
		private CancellationTokenSource source;
		protected CancellationToken cancellationToken => source.Token;
		
		public async Task Start()
		{
			AppLifeCycle.OnAppExit += OnAppExit;

			source = new CancellationTokenSource();
			await Setup();
		}
		
		private async void OnAppExit()
		{
			AppLifeCycle.OnAppExit -= OnAppExit;

			source?.Cancel();
			await Teardown();
		}

		protected virtual Task Setup() => Task.CompletedTask;
		protected virtual Task Teardown() => Task.CompletedTask;
	}
}