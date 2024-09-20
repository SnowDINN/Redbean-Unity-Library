using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Redbean
{
	public class Interaction : IDisposable
	{
		public Interaction()
		{
			if (InteractionMono.Interaction)
				InteractionMono.Interaction.ActiveGameObject(true);
		}
		
		public void Dispose()
		{
			if (InteractionMono.Interaction)
				InteractionMono.Interaction.ActiveGameObject(false);
		}
	}
	
	public class InteractionMono : MonoBehaviour
	{
		public static InteractionMono Interaction;

		public static void OnInitialize()
		{
			var go = new GameObject("[Interaction System]", 
			                        typeof(EventSystem),
			                        typeof(StandaloneInputModule),
			                        typeof(InteractionMono));
			go.transform.SetParent(AppLifeCycle.Transform);
				
			Interaction = go.GetComponent<InteractionMono>();
		}
	}
}