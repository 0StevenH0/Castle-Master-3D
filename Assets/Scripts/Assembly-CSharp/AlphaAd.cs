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

	public bool RequestList(int slot, AdPos pos, OnResultList proc)
	{
		return false;
	}

	public bool LinkClick(int slot, string code, OnResultLinkClick proc)
	{
		return false;
	}

	public void StopRequest()
	{
		StopAllCoroutines();
	}
}
