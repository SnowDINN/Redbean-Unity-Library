using System;
using UnityEngine;

namespace Redbean
{
	public class Indicator : IDisposable
	{
		public Indicator()
		{
			if (IndicatorMono.Indicator)
				IndicatorMono.Indicator.ActiveGameObject(true);
		}
		
		public void Dispose()
		{
			if (IndicatorMono.Indicator)
				IndicatorMono.Indicator.ActiveGameObject(false);
		}
	}
	
	public class IndicatorMono : MonoBehaviour
	{
		public static IndicatorMono Indicator;

		public static void OnInitialize()
		{
			var resource = Resources.Load<GameObject>("Indicator");
			var go = Instantiate(resource, AppLifeCycle.Transform);
			go.name = "[Indicator System]";
				
			Indicator = go.AddComponent<IndicatorMono>();
			Indicator.ActiveGameObject(false);
		}
	}
}