using System;
using UnityEngine;

public class ResourceManager
{
	private static AssetBundle bundle;

	public static UnityEngine.Object Load(string path, string filename, Type type)
	{
		UnityEngine.Object @object = null;
		if (bundle != null)
		{
			@object = bundle.LoadAsset(filename, type);
		}
		if (@object == null)
		{
			@object = Resources.Load(path + "/" + filename, type);
		}
		return @object;
	}
}
