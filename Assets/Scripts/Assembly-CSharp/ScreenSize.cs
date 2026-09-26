using UnityEngine;

public class ScreenSize
{
	private static int iOrgWidth = 960;

	private static int iOrgHeight = 640;

	public static int Width
	{
		get
		{
			return iOrgWidth;
		}
	}

	public static int Height
	{
		get
		{
			return iOrgHeight;
		}
	}

	public static float OrgAspect
	{
		get
		{
			return (float)iOrgWidth / (float)iOrgHeight;
		}
	}

	public static float CurAspect
	{
		get
		{
			return (float)Screen.width / (float)Screen.height;
		}
	}

	public static float Scale
	{
		get
		{
			float num = Screen.width;
			return num / (float)iOrgWidth;
		}
	}

	public static bool IsTablet
	{
		get
		{
			if (Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer)
			{
				return true;
			}
			if (SystemInfo.deviceModel.Equals("iPad"))
			{
				return true;
			}
			return Screen.width >= 1024;
		}
	}

	public static Rect GetRect(int left, int top, int width, int height)
	{
		float scale = Scale;
		if (Scale == 1f)
		{
			return new Rect(left, top, width, height);
		}
		return new Rect((float)left * scale, (float)top * scale, (float)width * scale, (float)height * scale);
	}

	public static Rect GetRect(float left, float top, float width, float height)
	{
		float scale = Scale;
		if (Scale == 1f)
		{
			return new Rect(left, top, width, height);
		}
		return new Rect(left * scale, top * scale, width * scale, height * scale);
	}

	public static void AdjustCameraRect(Camera cam)
	{
		// Preview render textures have their own composition and must not be resized.
		if (cam == null || cam.targetTexture != null)
		{
			return;
		}
		// The legacy UI and its hit targets are authored in a fixed 960 x 640 space.
		// Fill the display with that entire composition instead of pillarboxing it.
		// Setting the projection aspect also keeps ScreenToWorldPoint input aligned.
		cam.rect = new Rect(0f, 0f, 1f, 1f);
		if (cam.orthographic)
		{
			cam.aspect = OrgAspect;
		}
		else
		{
			cam.ResetAspect();
		}
	}

	public static void ConvertGUISize()
	{
		foreach (Camera camera in Object.FindObjectsByType<Camera>(
			FindObjectsInactive.Include, FindObjectsSortMode.None))
		{
			if (camera.name.Contains("UI"))
			{
				AdjustCameraRect(camera);
			}
		}
		ClearScreenMargin();
	}

	public static void ClearScreenMargin()
	{
		// UI-only scenes still need a clear pass. Reuse it if initialization repeats.
		GameObject gameObject = GameObject.Find("ScreenMargin Camera");
		if (gameObject == null)
		{
			gameObject = new GameObject("ScreenMargin Camera");
			gameObject.AddComponent<Camera>();
		}
		Camera camera = gameObject.GetComponent<Camera>();
		camera.cullingMask = 0;
		camera.depth = -200f;
		camera.backgroundColor = Color.black;
		camera.clearFlags = CameraClearFlags.Color;
	}
}
