using System.Collections;
using UnityEngine;

public class UIRanking : MonoBehaviour
{
	public class RankLine
	{
		public GameObject panel;

		public TextMesh textRank;

		public TextMesh textUserName;

		public TextMesh textTotalExp;

		public TextMesh textNationality;
	}

	public class Ranking
	{
		public int row;

		public int rank;

		public string userName;

		public int totalExp;

		public string nationality;
	}

	private const int maxLinePerPage = 10;

	private const float lineHeight = 39f;

	public AuiButton buttonClose;

	public AuiButton buttonPrev;

	public AuiButton buttonNext;

	public AuiButton buttonTop;

	public AuiButton buttonMyRank;

	public AuiSprite labelTop;

	public AuiSprite labelMyRank;

	public Material mtrFontNormal;

	public Material mtrFontMyRank;

	public TextMesh textMsg;

	public GameObject panelLine;

	private RankLine[] rankList;

	private Ranking[] ranking;

	private int myRow;

	private int curFirst;

	private string curUserName = string.Empty;

	private AppRankingSystem appRanking;

	private void Start()
	{
		buttonClose.isTop = true;
		buttonPrev.isTop = true;
		buttonNext.isTop = true;
		buttonTop.isTop = true;
		buttonMyRank.isTop = true;
		buttonClose.onButtonClick = OnCloseClick;
		buttonPrev.onButtonClick = OnPrevClick;
		buttonNext.onButtonClick = OnNextClick;
		buttonTop.onButtonClick = OnTopClick;
		buttonMyRank.onButtonClick = OnMyRankClick;
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	private void OnCloseClick(AuiButton sender)
	{
		Hide();
	}

	private void OnPrevClick(AuiButton sender)
	{
		if (curFirst > 0)
		{
			curFirst -= 10;
			if (curFirst < 0)
			{
				curFirst = 0;
			}
			StartCoroutine("ReadRankFromServer");
		}
	}

	private void OnNextClick(AuiButton sender)
	{
		curFirst += 10;
		StartCoroutine("ReadRankFromServer");
	}

	private void OnTopClick(AuiButton sender)
	{
		curFirst = 0;
		StartCoroutine("ReadRankFromServer");
	}

	private void OnMyRankClick(AuiButton sender)
	{
		curFirst = (myRow - 1) / 10 * 10;
		if (curFirst < 0)
		{
			curFirst = 0;
		}
		StartCoroutine("ReadRankFromServer");
	}

	private void RefreshList()
	{
		if (rankList == null)
		{
			rankList = new RankLine[10];
			Vector3 localPosition = panelLine.transform.localPosition;
			for (int i = 0; i < 10; i++)
			{
				GameObject gameObject = panelLine;
				if (i > 0)
				{
					gameObject = Object.Instantiate(panelLine) as GameObject;
					localPosition.y -= 39f;
					gameObject.transform.parent = panelLine.transform.parent;
					gameObject.transform.localPosition = localPosition;
				}
				rankList[i] = new RankLine();
				rankList[i].panel = gameObject;
				rankList[i].textRank = gameObject.transform.Find("Text Rank").GetComponent<TextMesh>();
				rankList[i].textUserName = gameObject.transform.Find("Text UserName").GetComponent<TextMesh>();
				rankList[i].textTotalExp = gameObject.transform.Find("Text TotalExp").GetComponent<TextMesh>();
				rankList[i].textNationality = gameObject.transform.Find("Text Nationality").GetComponent<TextMesh>();
			}
		}
		int num = 0;
		if (ranking != null)
		{
			num = ranking.Length;
			for (int j = 0; j < num; j++)
			{
				rankList[j].panel.SetActive(true);
				rankList[j].textRank.text = ranking[j].rank.ToString();
				rankList[j].textUserName.text = ranking[j].userName;
				rankList[j].textTotalExp.text = ranking[j].totalExp.ToString();
				rankList[j].textNationality.text = ranking[j].nationality;
				rankList[j].textRank.GetComponent<Renderer>().material = ((ranking[j].row != myRow) ? mtrFontNormal : mtrFontMyRank);
				rankList[j].textUserName.GetComponent<Renderer>().material = ((ranking[j].row != myRow) ? mtrFontNormal : mtrFontMyRank);
				rankList[j].textTotalExp.GetComponent<Renderer>().material = ((ranking[j].row != myRow) ? mtrFontNormal : mtrFontMyRank);
				rankList[j].textNationality.GetComponent<Renderer>().material = ((ranking[j].row != myRow) ? mtrFontNormal : mtrFontMyRank);
			}
		}
		for (int k = num; k < 10; k++)
		{
			rankList[k].panel.SetActive(false);
		}
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	public void Show(string userName)
	{
		base.gameObject.SetActive(true);
		AuiButton.topActive = true;
		curUserName = userName;
		buttonMyRank.visible = curUserName.Length > 0;
		buttonTop.visible = curUserName.Length > 0;
		labelMyRank.visible = curUserName.Length > 0;
		labelTop.visible = curUserName.Length > 0;
		RefreshList();
		StartCoroutine("ReadRankFromServer");
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
		AuiButton.topActive = false;
	}

	private IEnumerator ReadRankFromServer()
	{
		buttonPrev.enabled = false;
		buttonNext.enabled = false;
		buttonTop.enabled = false;
		buttonMyRank.enabled = false;
		textMsg.text = StringContent.msgDownloadRank;
		myRow = 0;
		ranking = null;
		RefreshList();
		yield return 1;
		if (appRanking == null)
		{
			appRanking = base.gameObject.AddComponent<AppRankingSystem>();
		}
		appRanking.RankingList("castlemaster", curUserName, curFirst, 10, OnRankingList);
	}

	private void OnRankingList(bool isError, int count, int[] row, int[] rank, string[] uid, string[] nickname, string[] lang, int[] score, string[] date, float[] percent)
	{
		buttonPrev.enabled = true;
		buttonNext.enabled = true;
		buttonTop.enabled = true;
		buttonMyRank.enabled = true;
		if (isError)
		{
			textMsg.text = StringContent.msgDownloadError;
			return;
		}
		if (count > 0)
		{
			myRow = row[0];
		}
		if (count > 1)
		{
			ranking = new Ranking[count - 1];
			for (int i = 1; i < count; i++)
			{
				int num = i - 1;
				string text = nickname[i];
				ranking[num] = new Ranking();
				ranking[num].row = row[i];
				ranking[num].rank = rank[i];
				ranking[num].userName = "Lv." + GetLevelFromTotalExp(score[i]) + " " + text;
				ranking[num].totalExp = score[i];
				ranking[num].nationality = lang[i];
			}
		}
		textMsg.text = string.Empty;
		RefreshList();
		if (curFirst > 0 && count < 2)
		{
			curFirst -= 10;
			if (curFirst < 0)
			{
				curFirst = 0;
			}
			StartCoroutine("ReadRankFromServer");
		}
	}

	private int GetLevelFromTotalExp(int totalExp)
	{
		int num = totalExp;
		int num2 = 1;
		while (num > 0)
		{
			num -= PlayInfo.heroLevelTable[num2].exp;
			if (num < 0)
			{
				break;
			}
			num2++;
			if (num2 == 99)
			{
				break;
			}
		}
		return num2;
	}
}
