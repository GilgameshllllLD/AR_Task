using UnityEngine;

public static class TransformExtensions
{
	public static Transform FindDeepChild(this Transform root, string name) {
		var result = root.Find(name);
		if (result != null)
			return result;

		foreach(Transform child in root) {
			result = child.FindDeepChild(name);
			if (result != null)
				return result;
		}

		return null;
	}

	public static void SetLayerRecursively(this Transform trans, int layer) {
		trans.gameObject.layer = layer;
		foreach (Transform child in trans)
			SetLayerRecursively (child, layer);
	}
}
