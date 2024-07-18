using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Redbean.MVP
{
	public class Presenter : IPresenter
	{
		private GameObject GameObject;

		public GameObject GetGameObject() => GameObject;

		public void BindView(IView view)
		{
			GameObject = view.GetGameObject();
				
			foreach (var field in GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
			{
				var attributes = field.GetCustomAttributes(false);
				if (!attributes.Any())
					continue;

				foreach (var attribute in attributes)
					switch (attribute)
					{
						case ModelAttribute:
							field.SetValue(this, this.GetModel(field.FieldType));
							break;
						
						case ViewAttribute:
							field.SetValue(this, view);
							break;

						case SingletonAttribute:
							field.SetValue(this, this.GetSingleton(field.FieldType));
							break;
					}
			}
		}
		
		public virtual void Setup() { }
		
		public virtual void Teardown() { }
	}
}