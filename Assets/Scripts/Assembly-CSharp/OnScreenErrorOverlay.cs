using System.Collections.Generic;
using UnityEngine;

public class OnScreenErrorOverlay : MonoBehaviour
{
	private static List<string> errors = new List<string>();

	private Vector2 scrollPos;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Install()
	{
		GameObject gameObject = new GameObject("OnScreenErrorOverlay");
		gameObject.AddComponent<OnScreenErrorOverlay>();
		Object.DontDestroyOnLoad(gameObject);
		Application.logMessageReceived += HandleLog;
	}

	private static void HandleLog(string condition, string stackTrace, LogType type)
	{
		if (type == LogType.Exception || type == LogType.Error)
		{
			errors.Add(condition + "\n" + stackTrace);
		}
	}

	private void OnGUI()
	{
		if (errors.Count == 0)
		{
			return;
		}
		GUI.Box(new Rect(10, 10, Screen.width - 20, Screen.height - 20), string.Empty);
		Rect viewRect = new Rect(20, 20, Screen.width - 40, Screen.height - 40);
		GUILayout.BeginArea(viewRect);
		scrollPos = GUILayout.BeginScrollView(scrollPos);
		GUIStyle style = new GUIStyle(GUI.skin.label);
		style.fontSize = 24;
		style.wordWrap = true;
		style.normal.textColor = Color.red;
		for (int i = 0; i < errors.Count; i++)
		{
			GUILayout.Label(errors[i], style);
			GUILayout.Space(20f);
		}
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}
}
