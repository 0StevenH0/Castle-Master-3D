using System.Collections;
using System.Collections.Generic;
using System.Text;
using CryptoSample;
using UnityEngine;

public class AlphaAd : MonoBehaviour
{
	public enum AdPos
	{
		start = 1,
		finish = 2,
		click = 3,
		list = 99
	}

	public class AdList
	{
		public string game_icon = string.Empty;

		public string code = string.Empty;

		public string adv_img = string.Empty;

		public string item_code = string.Empty;

		public string item_name = string.Empty;

		public string item_cnt = string.Empty;

		public string explain = string.Empty;

		public string link = string.Empty;

		public string fill_type = string.Empty;
	}

	public delegate void OnResultList(bool isSucceed, int count, AdList[] adList);

	public delegate void OnResultLinkClick(bool isSucceed);

	public const string game_code = "CAMST";

	public const string game_code_plus = "CAMAP";

	private const string htmlHeader = "[RANKING_SYSTEM_V_2]";

	private const string requestUrl = "http://adv.alphagames.co.kr/app/advertise_list.aspx";

	private const string chargeUrl = "http://adv.alphagames.co.kr/app/fill_result.aspx";

	public const char splitLine = '\n';

	public const char splitField = '\r';

	public const char splitCol = '\t';

	private const string cryptoKey = "B3176780-1673-4F02-9DB8-1D7BA951C8DB";

	private OnResultList procResultList;

	private OnResultLinkClick procResultLinkClick;

	public bool RequestList(int slot, AdPos pos, OnResultList proc)
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			return false;
		}
		procResultList = proc;
		StartCoroutine(ProcRequestList(slot, pos));
		StartCoroutine("WaitRequestList");
		return true;
	}

	public bool LinkClick(int slot, string code, OnResultLinkClick proc)
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			return false;
		}
		procResultLinkClick = proc;
		StartCoroutine(ProcRequestLinkClick(slot, code));
		StartCoroutine("WaitRequestLinkClick");
		return true;
	}

	public void StopRequest()
	{
		StopAllCoroutines();
	}

	private IEnumerator WaitRequestList()
	{
		yield return new WaitForSeconds(10f);
		if (procResultList != null)
		{
			procResultList(false, 0, null);
		}
	}

	private IEnumerator WaitRequestLinkClick()
	{
		yield return new WaitForSeconds(30f);
		if (procResultLinkClick != null)
		{
			procResultLinkClick(false);
		}
	}

	private IEnumerator ProcRequestList(int slot, AdPos pos)
	{
		string user_no = SystemInfo.deviceUniqueIdentifier;
		string market = string.Empty;
		switch (StoreType.store)
		{
		case StoreType.Store.appstore:
			market = "1";
			break;
		case StoreType.Store.tstore:
			market = "2";
			break;
		case StoreType.Store.olleh:
			market = "3";
			break;
		case StoreType.Store.android:
			market = "4";
			break;
		case StoreType.Store.samsung:
			market = "5";
			break;
		}
		string lang = "ENG";
		switch (UserSetting.language)
		{
		case Language.korean:
			lang = "KOR";
			break;
		case Language.japanese:
			lang = "JPN";
			break;
		}
		string char_name = slot.ToString();
		int num = (int)pos;
		string position = num.ToString();
		StringBuilder sb = new StringBuilder();
		if (PlusType.isPlus)
		{
			sb.Append("game_code" + '\t' + "CAMAP" + '\r');
		}
		else
		{
			sb.Append("game_code" + '\t' + "CAMST" + '\r');
		}
		sb.Append("market" + '\t' + market + '\r');
		sb.Append("lang" + '\t' + lang + '\r');
		sb.Append("user_no" + '\t' + user_no + '\r');
		sb.Append("char_name" + '\t' + char_name + '\r');
		sb.Append("position" + '\t' + position + '\r');
		string sendmsg = sb.ToString();
		sendmsg = Crypto.Encode("B3176780-1673-4F02-9DB8-1D7BA951C8DB", sendmsg);
		WWWForm webForm = new WWWForm();
		webForm.AddField("data", sendmsg);
		WWW web = new WWW("http://adv.alphagames.co.kr/app/advertise_list.aspx", webForm);
		yield return web;
		StopCoroutine("WaitRequestList");
		if (web.error == null)
		{
			try
			{
				string recvtext = Crypto.Decode("B3176780-1673-4F02-9DB8-1D7BA951C8DB", web.text);
				string[] lines = recvtext.Split('\n');
				List<AdList> adList = new List<AdList>();
				string[] array = lines;
				foreach (string line in array)
				{
					string[] fields = line.Split('\r');
					if (fields.Length < 9)
					{
						continue;
					}
					AdList item = new AdList();
					string[] array2 = fields;
					foreach (string field in array2)
					{
						string[] cols = field.Split('\t');
						if (cols.Length == 2)
						{
							if (cols[0].Equals("game_icon"))
							{
								item.game_icon = cols[1];
							}
							else if (cols[0].Equals("code"))
							{
								item.code = cols[1];
							}
							else if (cols[0].Equals("adv_img"))
							{
								item.adv_img = cols[1];
							}
							else if (cols[0].Equals("item_code"))
							{
								item.item_code = cols[1];
							}
							else if (cols[0].Equals("item_name"))
							{
								item.item_name = cols[1];
							}
							else if (cols[0].Equals("item_cnt"))
							{
								item.item_cnt = cols[1];
							}
							else if (cols[0].Equals("explain"))
							{
								item.explain = cols[1];
							}
							else if (cols[0].Equals("link"))
							{
								item.link = cols[1];
							}
							else if (cols[0].Equals("fill_type"))
							{
								item.fill_type = cols[1];
							}
						}
					}
					if (item.code != null && item.code.Length > 0)
					{
						adList.Add(item);
					}
				}
				if (adList.Count > 0)
				{
					procResultList(true, adList.Count, adList.ToArray());
					yield break;
				}
			}
			finally
			{
			}
		}
		if (procResultList != null)
		{
			procResultList(false, 0, null);
		}
	}

	private IEnumerator ProcRequestLinkClick(int slot, string code)
	{
		string user_no = SystemInfo.deviceUniqueIdentifier;
		string char_name = slot.ToString();
		StringBuilder sb = new StringBuilder();
		sb.Append("user_no" + '\t' + user_no + '\r');
		sb.Append("code" + '\t' + code + '\r');
		sb.Append("char_name" + '\t' + char_name + '\r');
		string sendmsg2 = sb.ToString();
		sendmsg2 = Crypto.Encode("B3176780-1673-4F02-9DB8-1D7BA951C8DB", sendmsg2);
		WWWForm webForm = new WWWForm();
		webForm.AddField("data", sendmsg2);
		WWW web = new WWW("http://adv.alphagames.co.kr/app/fill_result.aspx", webForm);
		yield return web;
		StopCoroutine("WaitRequestLinkClick");
		if (web.error == null)
		{
			try
			{
				string recvtext = Crypto.Decode("B3176780-1673-4F02-9DB8-1D7BA951C8DB", web.text);
				if (recvtext.Equals("SUCCESS"))
				{
					if (procResultLinkClick != null)
					{
						procResultLinkClick(true);
					}
					yield break;
				}
			}
			finally
			{
			}
		}
		if (procResultLinkClick != null)
		{
			procResultLinkClick(false);
		}
	}
}
