using UnityEngine;

public class FrameCheck : MonoBehaviour
{
	private float frameTime;

	private float fps;

	private float fcount;

	private Matrix4x4 mtx = Matrix4x4.Scale(new Vector3(2f, 2f, 1f));

	private void OnGUI()
	{
		frameTime += Time.deltaTime;
		fcount += 1f;
		if (frameTime >= 1f)
		{
			fps = fcount;
			fcount = 0f;
			frameTime = 0f;
		}
		GUI.matrix = mtx;
		GUI.Label(new Rect(30f, 5f, 100f, 30f), fps.ToString());
	}
}
