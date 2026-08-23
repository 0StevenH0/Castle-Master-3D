using System.Collections.Generic;
using UnityEngine;

public class ProcBase : MonoBehaviour
{
	private static MessageView messageView;

	private static Language curFontLang = Language.max;

	private static List<Material> mtrFont = new List<Material>();

	private static Font langFont = null;

	public static void LoadMessageBox()
	{
		GameObject gameObject = Object.Instantiate(ResourceManager.Load("interface/prefabs", "feb_message", typeof(GameObject))) as GameObject;
		gameObject.name = "MessageBox";
		messageView = gameObject.GetComponent<MessageView>();
	}

	public void Start()
	{
		ScreenSize.ConvertGUISize();
		LoadMessageBox();
		OnStart();
		ChangeTextMeshLanguage();
	}

	public virtual void OnStart()
	{
	}

	public static void LoadScene(string sceneName, ProcLoading.ContentMode mode)
	{
		AuiButton.modalActive = false;
		AuiButton.topActive = false;
		AuiButton.mostTopActive = false;
		GameObject gameObject = null;
		GameObject gameObject2 = null;
		gameObject = ResourceManager.Load("interface/prefabs", "feb_loading", typeof(GameObject)) as GameObject;
		gameObject2 = Object.Instantiate(gameObject) as GameObject;
		ProcLoading component = gameObject2.GetComponent<ProcLoading>();
		component.Show(mode);
		Application.LoadLevelAsync(sceneName);
	}

	public static void ShowMsg(string st, MessageView.MsgIcon icon, bool autoHide, MessageView.MsgButton[] buttons, bool alertSound, MessageView.OnMessageClickDelegate procResult)
	{
		if (messageView != null)
		{
			messageView.ShowMsgEx(st, icon, autoHide, buttons, alertSound, procResult);
		}
	}

	public static void ShowMsg(string st, MessageView.MsgIcon icon)
	{
		ShowMsg(st, icon, true, null, false, null);
	}

	public static void ShowMsg(string st, MessageView.MsgIcon icon, bool autoHide)
	{
		ShowMsg(st, icon, autoHide, null, false, null);
	}

	public static void ShowMsg(string st, MessageView.MsgIcon icon, MessageView.MsgButton[] buttons, MessageView.OnMessageClickDelegate procResult)
	{
		ShowMsg(st, icon, false, buttons, false, procResult);
	}

	public static void ShowMsg(string st, MessageView.MsgIcon icon, bool autoHide, MessageView.MsgButton[] buttons, MessageView.OnMessageClickDelegate procResult)
	{
		ShowMsg(st, icon, autoHide, buttons, false, procResult);
	}

	public static void HideMsgView()
	{
		if (messageView != null)
		{
			messageView.HideMsg();
		}
	}

	public static Bounds GetGameObjectBound(GameObject obj)
	{
		Transform[] componentsInChildren = obj.GetComponentsInChildren<Transform>();
		Bounds result = new Bounds(obj.transform.position, obj.transform.lossyScale);
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			if (transform.GetComponent<Renderer>() != null)
			{
				Bounds bounds = new Bounds(transform.position, transform.lossyScale);
				Vector3 min = result.min;
				Vector3 max = result.max;
				if (bounds.min.x < min.x)
				{
					min.x = bounds.min.x;
				}
				if (bounds.min.y < min.y)
				{
					min.y = bounds.min.y;
				}
				if (bounds.max.x > max.x)
				{
					max.x = bounds.max.x;
				}
				if (bounds.max.y > max.y)
				{
					max.y = bounds.max.y;
				}
				result.SetMinMax(min, max);
			}
		}
		return result;
	}

	public static Rect GetGameObjectRect(GameObject obj)
	{
		Bounds gameObjectBound = GetGameObjectBound(obj);
		return new Rect(gameObjectBound.min.x, gameObjectBound.min.y, gameObjectBound.size.x, gameObjectBound.size.y);
	}

	public static void ResetTextWidth(TextMesh text, float sizeX)
	{
		Vector3 localScale = new Vector3(1f, 1f, 1f);
		text.transform.localScale = localScale;
		float x = text.GetComponent<Renderer>().bounds.size.x;
		if (x > sizeX)
		{
			localScale.x = sizeX / x;
			text.transform.localScale = localScale;
		}
	}

	public static void ResetTextWordWarp(TextMesh text, float sizeX)
	{
		Vector3 localScale = new Vector3(1f, 1f, 1f);
		text.transform.localScale = localScale;
		float x = text.GetComponent<Renderer>().bounds.size.x;
		if (x < sizeX)
		{
			return;
		}
		string text2 = string.Empty;
		string[] array = text.text.Split('\n');
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(' ');
			for (int j = 0; j < array2.Length; j++)
			{
				string text3 = text2 + array2[j];
				text.text = text3;
				x = text.GetComponent<Renderer>().bounds.size.x;
				text2 = ((!(x > sizeX)) ? (text2 + array2[j] + " ") : (text2 + "\n" + array2[j] + " "));
			}
			text2 += "\n";
		}
		text.text = text2;
	}

	public static void ResetTextMeshMaterial()
	{
		if (UserSetting.language == Language.english || UserSetting.language == Language.korean || curFontLang == UserSetting.language)
		{
			return;
		}
		string path = "interface/fontdata/font_eng";
		if (UserSetting.language == Language.japanese)
		{
			path = "interface/fontdata/japanese/font_japanese";
		}
		Font font = Resources.Load(path, typeof(Font)) as Font;
		langFont = font;
		mtrFont.Clear();
		string[] array = new string[20]
		{
			"black", "calerta", "calertb", "calertc", "clevel", "cmdpts", "darkblue", "darkdesc", "darkgray", "darkred",
			"desc", "green", "lightblue", "lightgray", "msg", "red", "skill", "title", "white", "whitegray"
		};
		string[] array2 = array;
		foreach (string text in array2)
		{
			string path2 = "interface/fontdata/font_material_" + text;
			if (UserSetting.language == Language.japanese)
			{
				path2 = "interface/fontdata/japanese/font_material_" + text;
			}
			mtrFont.Add(Resources.Load(path2, typeof(Material)) as Material);
		}
		curFontLang = UserSetting.language;
	}

	private static void ChangeTextMesh(TextMesh textObj)
	{
		textObj.font = langFont;
		foreach (Material item in mtrFont)
		{
			if (item.name.Equals(textObj.GetComponent<Renderer>().material.name.Substring(0, item.name.Length)))
			{
				textObj.GetComponent<Renderer>().material = item;
				break;
			}
		}
	}

	public static void ChangeTextMeshLanguage()
	{
		if (UserSetting.language != 0 && UserSetting.language != Language.korean)
		{
			ResetTextMeshMaterial();
			TextMesh[] array = Object.FindObjectsOfType(typeof(TextMesh)) as TextMesh[];
			TextMesh[] array2 = array;
			foreach (TextMesh textObj in array2)
			{
				ChangeTextMesh(textObj);
			}
		}
	}

	private static void ChangeTextMeshLanguageChild(Transform trans)
	{
		foreach (Transform tran in trans)
		{
			TextMesh component = tran.GetComponent<TextMesh>();
			if (component != null)
			{
				ChangeTextMesh(component);
			}
			if (tran.childCount > 0)
			{
				ChangeTextMeshLanguageChild(tran);
			}
		}
	}

	public static void ChangeTextMeshLanguageAllChild(Transform trans)
	{
		if (UserSetting.language != 0 && UserSetting.language != Language.korean)
		{
			ResetTextMeshMaterial();
			ChangeTextMeshLanguageChild(trans);
		}
	}
}
