using UnityEngine;

public class LayerManager
{
	public static int layerDefault = 1;

	public static int layerUI = 8;

	public static int layerCharactor = 10;

	public static int layerBackground = 11;

	public static int layerTerrain = 12;

	public static int layerPreviewInven = 14;

	public static int layerPreviewShop = 15;

	public static void SetLayerAllChild(Transform trans, int layer)
	{
		Transform[] componentsInChildren = trans.GetComponentsInChildren<Transform>();
		trans.gameObject.layer = layer;
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			transform.gameObject.layer = layer;
		}
	}
}
