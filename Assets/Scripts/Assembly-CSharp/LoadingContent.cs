using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadingContent
{
	private static List<string> strCastle = new List<string>();

	private static List<string> strBattle = new List<string>();

	public static void Init(Language lang)
	{
		TextAsset textAsset = ResourceManager.Load("GameData", "loading_" + lang, typeof(TextAsset)) as TextAsset;
		bool succeed = false;
		string s = DataSecurity.Decrypt(textAsset.text, "surkwjch", out succeed);
		if (!succeed)
		{
			Debug.LogError("Decrypt Error!!");
			return;
		}
		StringReader stringReader = new StringReader(s);
		strCastle.Clear();
		strBattle.Clear();
		string text;
		while ((text = stringReader.ReadLine()) != null && text.Trim().Length != 0)
		{
			char[] separator = new char[1] { '\t' };
			string[] array = text.Split(separator);
			if (array.Length > 1)
			{
				int num = int.Parse(array[0].Trim());
				string text2 = array[1].Trim();
				text2 = text2.Replace("\\n", "\n");
				if (num == 0)
				{
					strCastle.Add(text2);
				}
				else
				{
					strBattle.Add(text2);
				}
			}
		}
	}

	public static string GetRandomStringCastle()
	{
		int index = Random.Range(0, strCastle.Count);
		return strCastle[index];
	}

	public static string GetRandomStringBattle()
	{
		int index = Random.Range(0, strBattle.Count);
		return strBattle[index];
	}
}
