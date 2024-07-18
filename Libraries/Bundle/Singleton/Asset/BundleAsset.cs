using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Redbean.Bundle
{
	public class BundleAsset
	{
		public Dictionary<int, Object> References = new();
		public Object Asset;

		public void Release()
		{
			foreach (var reference in References.Values)
				Object.Destroy(reference);
			
			Addressables.Release(Asset);
		}
	}
}