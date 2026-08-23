using System;
using System.Collections;
using UnityEngine;

public class AppUtcTime : MonoBehaviour
{
	public delegate void OnUpdateUtcTime(bool isSucceed);

	private const string timeUrl = "http://m.castlemaster.co.kr/ranking/globalticks.aspx";

	public static DateTime utcNow = DateTime.UtcNow;

	private OnUpdateUtcTime procUpdateUtcTime;

	public void UpdateUtcTime(OnUpdateUtcTime rst)
	{
		procUpdateUtcTime = rst;
		StartCoroutine("ProcUpdateUtcTime");
	}

	private IEnumerator ProcUpdateUtcTime()
	{
		WWW web = new WWW("http://m.castlemaster.co.kr/ranking/globalticks.aspx");
		yield return web;
		if (web.error == null)
		{
			string recvtext = AppRankingSystem.Base64Decode(web.text);
			if (recvtext.Length == 0)
			{
				procUpdateUtcTime(false);
				yield break;
			}
			utcNow = DateTime.Parse(recvtext);
			procUpdateUtcTime(true);
		}
		else
		{
			procUpdateUtcTime(false);
		}
	}
}
