using System.Collections.Generic;

namespace Redbean
{
	public class Container<Key, Value> : IContainer
	{
		private static Dictionary<Key, Value> m_container;
		protected static Dictionary<Key, Value> container
		{
			get
			{
				if (m_container is not null)
					return m_container;

				AppLifeCycle.OnAppExit += OnAppExit;
				
				m_container = new Dictionary<Key, Value>();
				return m_container;
			}
			
			set => m_container = value;
		}

		private static void OnAppExit()
		{
			AppLifeCycle.OnAppExit -= OnAppExit;
			
			container.Clear();
		}
	}
}