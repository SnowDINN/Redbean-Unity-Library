using System;

namespace Redbean.Utility
{
	public class DisableInteraction : IDisposable
	{
		public DisableInteraction()
		{
			if (AppLifeCycle.EventSystem)
				AppLifeCycle.EventSystem.SetActive(false);
		}
		
		public void Dispose()
		{
			if (AppLifeCycle.EventSystem)
				AppLifeCycle.EventSystem.SetActive(true);
		}
	}
}