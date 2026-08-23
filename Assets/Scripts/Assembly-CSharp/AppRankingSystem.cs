using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class AppRankingSystem : MonoBehaviour
{
	public delegate void OnUpdateRankingResult(bool isError);

	public delegate void OnRankingList(bool isError, int count, int[] row, int[] rank, string[] uid, string[] nickname, string[] lang, int[] score, string[] date, float[] percent);

	public const string gameCode = "castlemaster";

	private const string htmlHeader = "[RANKING_SYSTEM_V_2]";

	private const string updateUrl = "http://m.castlemaster.co.kr/ranking/updaterank.aspx";

	private const string rankingUrl = "http://m.castlemaster.co.kr/ranking/ranklist.aspx";

	public const int resultSuccess = 0;

	public const int resultError = 1;

	public const int resultErrorDB = 2;

	public const int resultErrorParam = 3;

	public const char splitLine = '\n';

	public const char splitTab = '\t';

	private OnUpdateRankingResult procUpdateResult;

	private OnRankingList procRankingList;

	public void UpdateRanking(string code, string userName, int score, OnUpdateRankingResult proc)
	{
		procUpdateResult = proc;
		StartCoroutine(ProcUpdateRanking(code, userName, score));
	}

	public void RankingList(string code, string userName, int listStart, int listCount, OnRankingList proc)
	{
		procRankingList = proc;
		StartCoroutine(ProcRankingList(code, userName, listStart, listCount));
	}

	public void StopRequest()
	{
		StopAllCoroutines();
	}

	private IEnumerator ProcUpdateRanking(string code, string userName, int score)
	{
		string uniqueNum = SystemInfo.deviceUniqueIdentifier;
		SystemLanguage lang = Application.systemLanguage;
		string sendmsg6 = string.Empty;
		string text = sendmsg6;
		sendmsg6 = text + "uid" + '\t' + uniqueNum + '\n';
		text = sendmsg6;
		sendmsg6 = text + "nickname" + '\t' + userName + '\n';
		text = sendmsg6;
		sendmsg6 = text + "gamecode" + '\t' + code + '\n';
		text = sendmsg6;
		sendmsg6 = string.Concat(text, "langtype", '\t', lang, '\n');
		text = sendmsg6;
		sendmsg6 = text + "score" + '\t' + score + '\n';
		WWWForm webForm = new WWWForm();
		webForm.AddField("data", Base64Hash(sendmsg6));
		UnityWebRequest web = UnityWebRequest.Post("http://m.castlemaster.co.kr/ranking/updaterank.aspx", webForm);
		yield return web.SendWebRequest();
		if (web.result == UnityWebRequest.Result.Success)
		{
			string recvtext = Base64Decode(web.downloadHandler.text);
			string[] lines = recvtext.Split('\n');
			int num = 0;
			bool isHeader = FindHeader(lines, ref num);
			int result = 1;
			if (isHeader)
			{
				while (num < lines.Length)
				{
					string line = lines[num];
					num++;
					string[] fields = line.Split('\t');
					if (fields.Length < 2 || !fields[0].Equals("result"))
					{
						continue;
					}
					int a;
					if (int.TryParse(fields[1], out a))
					{
						result = a;
					}
					break;
				}
			}
			if (procUpdateResult != null)
			{
				procUpdateResult(result != 0);
			}
		}
		else if (procUpdateResult != null)
		{
			procUpdateResult(true);
		}
	}

	private IEnumerator ProcRankingList(string code, string userName, int listStart, int listCount)
	{
		string uniqueNum = SystemInfo.deviceUniqueIdentifier;
		string sendmsg = string.Empty;
		string text = sendmsg;
		sendmsg = text + "uid" + '\t' + uniqueNum + '\n';
		text = sendmsg;
		sendmsg = text + "nickname" + '\t' + userName + '\n';
		text = sendmsg;
		sendmsg = text + "gamecode" + '\t' + code + '\n';
		text = sendmsg;
		sendmsg = text + "liststart" + '\t' + listStart + '\n';
		text = sendmsg;
		sendmsg = text + "listcount" + '\t' + listCount + '\n';
		WWWForm webForm = new WWWForm();
		webForm.AddField("data", Base64Hash(sendmsg));
		UnityWebRequest web = UnityWebRequest.Post("http://m.castlemaster.co.kr/ranking/ranklist.aspx", webForm);
		yield return web.SendWebRequest();
		if (web.result == UnityWebRequest.Result.Success && procRankingList != null)
		{
			try
			{
				string recvtext = Base64Decode(web.downloadHandler.text);
				string[] lines = recvtext.Split('\n');
				int num = 0;
				bool isHeader = FindHeader(lines, ref num);
				int result = 1;
				if (isHeader)
				{
					while (num < lines.Length)
					{
						string line2 = lines[num];
						num++;
						string[] fields2 = line2.Split('\t');
						if (fields2.Length < 2 || !fields2[0].Equals("result"))
						{
							continue;
						}
						int a;
						if (int.TryParse(fields2[1], out a))
						{
							result = a;
						}
						break;
					}
					if (result != 0)
					{
						if (procRankingList != null)
						{
							procRankingList(true, 0, null, null, null, null, null, null, null, null);
						}
						yield break;
					}
					List<int> rowList = new List<int>();
					List<int> rankList = new List<int>();
					List<string> uidList = new List<string>();
					List<string> nickList = new List<string>();
					List<string> langList = new List<string>();
					List<int> scoreList = new List<int>();
					List<string> dateList = new List<string>();
					List<float> percentList = new List<float>();
					while (num < lines.Length)
					{
						string line = lines[num];
						num++;
						if (line.Length != 0)
						{
							string[] fields = line.Split('\t');
							if (fields.Length != 0)
							{
								int rownum = int.Parse(fields[0].Trim());
								int rank = int.Parse(fields[1].Trim());
								string uid = fields[2].Trim();
								string nickname = fields[3].Trim();
								string lang = fields[4].Trim();
								int score = int.Parse(fields[5].Trim());
								string date = fields[6].Trim();
								float percent = float.Parse(fields[7].Trim());
								rowList.Add(rownum);
								rankList.Add(rank);
								uidList.Add(uid);
								nickList.Add(nickname);
								langList.Add(lang);
								scoreList.Add(score);
								dateList.Add(date);
								percentList.Add(percent);
							}
						}
					}
					procRankingList(false, rankList.Count, rowList.ToArray(), rankList.ToArray(), uidList.ToArray(), nickList.ToArray(), langList.ToArray(), scoreList.ToArray(), dateList.ToArray(), percentList.ToArray());
					yield break;
				}
			}
			finally
			{
			}
		}
		if (procRankingList != null)
		{
			procRankingList(true, 0, null, null, null, null, null, null, null, null);
		}
	}

	private bool FindHeader(string[] strdata, ref int lineNum)
	{
		while (lineNum < strdata.Length)
		{
			string text = strdata[lineNum];
			lineNum++;
			if (text.Equals("[RANKING_SYSTEM_V_2]"))
			{
				return true;
			}
		}
		return false;
	}

	public static string Base64Hash(string Data)
	{
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		byte[] bytes = uTF8Encoding.GetBytes(Data);
		return Convert.ToBase64String(bytes);
	}

	public static string Base64Decode(string Data)
	{
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		try
		{
			byte[] bytes = Convert.FromBase64String(Data);
			return uTF8Encoding.GetString(bytes);
		}
		catch
		{
			return string.Empty;
		}
	}
}
