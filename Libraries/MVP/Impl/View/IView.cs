using UnityEngine;

namespace Redbean.MVP
{
	public interface IView : IExtension
	{
		GameObject GetGameObject();
	}
}