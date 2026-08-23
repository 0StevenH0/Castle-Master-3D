using UnityEngine;

public class GroundManager : MonoBehaviour
{
	public enum GroundType
	{
		castle = 0,
		battle = 1,
		max = 2
	}

	private const string pathBackground = "Background/Prefebs";

	private const string pathLightmap = "Background/Lightmap";

	public static GameObject GenGround(GroundType ground, int idx, int castleSide, int castleLevel)
	{
		string filename = "feb_background_" + ground.ToString() + string.Format("{0:00}", idx);
		GameObject result = Object.Instantiate(ResourceManager.Load("Background/Prefebs", filename, typeof(GameObject))) as GameObject;
		LightmapData[] lightmaps = LightmapSettings.lightmaps;
		if (lightmaps.Length < 1)
		{
			lightmaps = new LightmapData[1];
		}
		for (int i = 0; i < 1; i++)
		{
			LightmapData lightmapData = new LightmapData();
			filename = "tex_lightmap_" + ground.ToString() + string.Format("{0:00}", idx) + "_" + string.Format("{0:00}", i);
			Texture2D lightmapNear = (lightmapData.lightmapColor = ResourceManager.Load("Background/Lightmap", filename, typeof(Texture2D)) as Texture2D);
			lightmapData.lightmapDir = lightmapNear;
			lightmaps[i] = lightmapData;
		}
		if (ground == GroundType.battle && UserSetting.quality == UserSetting.GraphicsQuality.beautiful)
		{
			string filename2 = "feb_background_" + ground.ToString() + string.Format("{0:00}", idx) + "_effect";
			Object.Instantiate(ResourceManager.Load("Background/Prefebs", filename2, typeof(GameObject)));
		}
		if (ground == GroundType.battle)
		{
			int num = 1;
			filename = "feb_battle_castle" + string.Format("{0:00}_{1:00}", castleSide, castleLevel - 1);
			GameObject gameObject = Object.Instantiate(ResourceManager.Load("Background/Prefebs", filename, typeof(GameObject))) as GameObject;
			if (gameObject != null)
			{
				Animation[] componentsInChildren = gameObject.transform.GetComponentsInChildren<Animation>();
				foreach (Animation animation in componentsInChildren)
				{
					animation.gameObject.AddComponent<AdjustAnimationSpeed>();
				}
				MeshRenderer[] componentsInChildren2 = gameObject.GetComponentsInChildren<MeshRenderer>();
				foreach (MeshRenderer meshRenderer in componentsInChildren2)
				{
					meshRenderer.lightmapIndex = num;
				}
			}
			LightmapData lightmapData2 = new LightmapData();
			string text = string.Empty;
			switch (castleSide)
			{
			case 0:
				text = "ally";
				break;
			case 1:
				text = "enemy";
				break;
			}
			filename = "tex_lightmap_castle_" + text + string.Format("{0:00}_{1:00}", 0, castleLevel - 1);
			Texture2D lightmapNear2 = (lightmapData2.lightmapColor = ResourceManager.Load("Background/Lightmap", filename, typeof(Texture2D)) as Texture2D);
			lightmapData2.lightmapDir = lightmapNear2;
			if (lightmaps.Length <= num)
			{
				System.Array.Resize(ref lightmaps, num + 1);
			}
			lightmaps[num] = lightmapData2;
		}
		LightmapSettings.lightmaps = lightmaps;
		return result;
	}

	public static Collider GetGroundCollider(GameObject obj)
	{
		Transform transform = obj.transform.Find("collider_ground");
		if (transform == null)
		{
			return null;
		}
		return transform.GetComponent<Collider>();
	}
}
