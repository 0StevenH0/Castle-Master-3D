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
		if (CurAspect == OrgAspect)
		{
			return;
		}
		bool flag = false;
		Rect rect = new Rect(0f, 0f, 1f, 1f);
		if (CurAspect > OrgAspect)
		{
			float num = (CurAspect - OrgAspect) / CurAspect;
			rect.x = num * 0.5f;
			rect.width = 1f - num;
			flag = true;
		}
		else
		{
			float num2 = (OrgAspect - CurAspect) / OrgAspect;
			rect.y = num2 * 0.5f;
			rect.height = 1f - num2;
		}
		if (cam.orthographic)
		{
			Rect rect2 = cam.rect;
			rect2.x *= rect.width;
			rect2.y *= rect.height;
			rect2.width *= rect.width;
			rect2.height *= rect.height;
			rect2.x += rect.x;
			rect2.y += rect.y;
			cam.rect = rect2;
		}
		else
		{
			Rect rect3 = cam.rect;
			if (flag)
			{
				float num3 = rect3.x * rect.width;
				rect3.x = rect.x + num3;
				rect3.width *= rect.width;
			}
			else
			{
				rect3.y = cam.rect.y * rect.height + rect.y;
				rect3.height = cam.rect.height * rect.height;
			}
			cam.rect = rect3;
		}
	}

	public static void ConvertGUISize()
	{
		if (CurAspect == OrgAspect)
		{
			return;
		}
		Camera[] array = Object.FindObjectsByType<Camera>();
		Camera[] array2 = array;
		foreach (Camera camera in array2)
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
		GameObject gameObject = new GameObject("ScreenMargin Camera");
		gameObject.AddComponent<Camera>();
		gameObject.GetComponent<Camera>().cullingMask = 0;
		gameObject.GetComponent<Camera>().depth = -200f;
		gameObject.GetComponent<Camera>().backgroundColor = new Color(0f, 0f, 0f, 1f);
		gameObject.GetComponent<Camera>().clearFlags = CameraClearFlags.Color;
	}
}
