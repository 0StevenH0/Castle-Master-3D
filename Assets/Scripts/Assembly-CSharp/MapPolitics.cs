using System.Collections;
using UnityEngine;

public class MapPolitics : MonoBehaviour
{
	private const int defIncLoyalty = 10;

	public TextMesh textCastleName;

	public GameObject panelLord;

	public GameObject panelAppoint;

	public GameObject panelCurrent;

	public GameObject panelSearch;

	public GameObject panelSearching;

	public AuiButton buttonClose;

	public AuiButton buttonAppoint;

	public AuiButton buttonSearch;

	public AuiButton buttonSearchRetry;

	public AuiButton buttonDismiss;

	public AuiButton buttonReward;

	public AuiSprite iconLord;

	public TextMesh textLordLevel;

	public TextMesh textLordName;

	public TextMesh textLordLoyalty;

	public TextMesh textLordFame;

	public TextMesh textLordAtk;

	public TextMesh textLordInt;

	public TextMesh textLordHp;

	public TextMesh textRewardCmdPts;

	public TextMesh textRewardLoyalty;

	public TextMesh textRewardCost;

	public TextMesh textAppointCmdPts;

	public TextMesh textAppointCost;

	public TextMesh textAppointMsg;

	public TextMesh textSearchMsg;

	public TextMesh textSearchingMsg;

	public TextMesh textRewardMsg;

	public UIMap uiMap;

	public MapCastleDetail popupCastleDetail;

	private CastleInfo info;

	private LordManager.Lord searchedLord;

	private int cost;

	private void Start()
	{
		buttonClose.isTop = true;
		buttonAppoint.isTop = true;
		buttonSearch.isTop = true;
		buttonDismiss.isTop = true;
		buttonReward.isTop = true;
		buttonSearchRetry.isTop = true;
		buttonClose.onButtonClick = OnCloseClick;
		buttonAppoint.onButtonClick = OnAppointClick;
		buttonSearch.onButtonClick = OnSearchClick;
		buttonSearchRetry.onButtonClick = OnSearchClick;
		buttonDismiss.onButtonClick = OnDismissClick;
		buttonReward.onButtonClick = OnRewardClick;
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	private void OnSearchClick(AuiButton sender)
	{
		ShowPanel(panelSearching);
		StartCoroutine("SearchLord");
	}

	private void OnCloseClick(AuiButton sender)
	{
		Hide();
	}

	private void OnAppointClick(AuiButton sender)
	{
		if (PlayInfo.playerData.cmdPts < PlayInfo.gameRule.cmdPtsAppointLord)
		{
			ProcBase.ShowMsg(StringContent.msgNotEnoughCmdPts, MessageView.MsgIcon.question, false, new MessageView.MsgButton[2]
			{
				MessageView.MsgButton.yes,
				MessageView.MsgButton.no
			}, OnCommandPointMessage);
			return;
		}
		if (PlayInfo.playerData.gold < cost)
		{
			ShowGotoGoldShop();
			return;
		}
		PlayInfo.playerData.cmdPts -= PlayInfo.gameRule.cmdPtsAppointLord;
		if (PlayInfo.playerData.cmdPts < 0)
		{
			PlayInfo.playerData.cmdPts = 0;
		}
		PlayInfo.playerData.gold -= cost;
		if (PlayInfo.playerData.gold < 0)
		{
			PlayInfo.playerData.gold = 0;
		}
		info.lord = searchedLord;
		info.lord.castleIndex = info.index;
		PlayInfo.lordManager.list.Add(searchedLord);
		Refresh();
		popupCastleDetail.Refresh();
	}

	private void ShowGotoGoldShop()
	{
		ProcBase.ShowMsg(StringContent.msgNotEnoughGold, MessageView.MsgIcon.alert, new MessageView.MsgButton[2]
		{
			MessageView.MsgButton.yes,
			MessageView.MsgButton.no
		}, OnMessageGotoGemShop);
	}

	private void ShowGotoGemShop()
	{
		ProcBase.ShowMsg(StringContent.msgNotEnoughGem, MessageView.MsgIcon.alert, new MessageView.MsgButton[2]
		{
			MessageView.MsgButton.yes,
			MessageView.MsgButton.no
		}, OnMessageGotoGemShop);
	}

	private void OnMessageGotoGemShop(MessageView.MsgButton button)
	{
		if (button == MessageView.MsgButton.yes)
		{
			Hide();
			uiMap.procCastle.uiGemShop.Show();
		}
	}

	private void OnCommandPointMessage(MessageView.MsgButton button)
	{
		if (button == MessageView.MsgButton.yes)
		{
			uiMap.OnCommandPointClick(null);
		}
	}

	private void OnDismissClick(AuiButton sender)
	{
		ProcBase.ShowMsg(StringContent.msgQuestFireLord, MessageView.MsgIcon.question, false, new MessageView.MsgButton[2]
		{
			MessageView.MsgButton.yes,
			MessageView.MsgButton.no
		}, OnDismissMessage);
	}

	private void OnDismissMessage(MessageView.MsgButton button)
	{
		if (button == MessageView.MsgButton.yes)
		{
			PlayInfo.lordManager.RemoveInCastle(info.index);
			info.lord = null;
			Refresh();
			popupCastleDetail.Refresh();
		}
	}

	private void OnRewardClick(AuiButton sender)
	{
		if (info.lord == null)
		{
			return;
		}
		if (info.lord.loyalty >= 100)
		{
			ProcBase.ShowMsg(StringContent.msgAlreadyMaxLoyalty, MessageView.MsgIcon.alert);
			return;
		}
		if (PlayInfo.playerData.cmdPts < PlayInfo.gameRule.cmdPtsManageLord)
		{
			ProcBase.ShowMsg(StringContent.msgNotEnoughCmdPts, MessageView.MsgIcon.question, false, new MessageView.MsgButton[2]
			{
				MessageView.MsgButton.yes,
				MessageView.MsgButton.no
			}, OnCommandPointMessage);
			return;
		}
		if (PlayInfo.playerData.gold < cost)
		{
			ShowGotoGoldShop();
			return;
		}
		PlayInfo.playerData.cmdPts -= PlayInfo.gameRule.cmdPtsManageLord;
		if (PlayInfo.playerData.cmdPts < 0)
		{
			PlayInfo.playerData.cmdPts = 0;
		}
		PlayInfo.playerData.gold -= cost;
		if (PlayInfo.playerData.gold < 0)
		{
			PlayInfo.playerData.gold = 0;
		}
		info.lord.loyalty += 10;
		Refresh();
		popupCastleDetail.Refresh();
	}

	private void ShowPanel(GameObject panel)
	{
		panelAppoint.SetActive(panelAppoint == panel);
		panelCurrent.SetActive(panelCurrent == panel);
		panelSearch.SetActive(panelSearch == panel);
		panelSearching.SetActive(panelSearching == panel);
		panelLord.SetActive(panelAppoint == panel || panelCurrent == panel);
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
		AuiButton.topActive = false;
	}

	public void Show(CastleInfo info)
	{
		base.gameObject.SetActive(true);
		AuiButton.topActive = true;
		this.info = info;
		textRewardCmdPts.text = PlayInfo.gameRule.cmdPtsManageLord.ToString();
		textAppointCmdPts.text = PlayInfo.gameRule.cmdPtsAppointLord.ToString();
		textAppointMsg.text = StringContent.msgQuestAppointLord;
		textSearchMsg.text = StringContent.msgDescNeedToSearchLord;
		textSearchingMsg.text = StringContent.msgNowSearchLord;
		textRewardMsg.text = StringContent.msgDescMercyForLordLoyalty;
		textCastleName.text = info.castleName;
		Refresh();
	}

	private void RefreshLord(LordManager.Lord lord)
	{
		iconLord.SetFrame((int)lord.type);
		textLordLevel.text = lord.level.ToString();
		textLordName.text = lord.type.ToString();
		textLordLoyalty.text = lord.loyalty.ToString();
		textLordFame.text = lord.fame.ToString();
		textLordAtk.text = lord.atk.ToString();
		textLordInt.text = lord.inte.ToString();
		textLordHp.text = lord.hp.ToString();
	}

	public void Refresh()
	{
		StopCoroutine("CheckDismissLord");
		if (info.lord == null)
		{
			ShowPanel(panelSearch);
			return;
		}
		ShowPanel(panelCurrent);
		RefreshLord(info.lord);
		cost = LordManager.GetRewardCost(info.lord);
		textRewardCost.text = cost.ToString();
		textRewardLoyalty.text = "+" + 10;
		StartCoroutine("CheckDismissLord");
	}

	private IEnumerator CheckDismissLord()
	{
		do
		{
			yield return new WaitForSeconds(0.5f);
		}
		while (info.lord != null);
		ShowPanel(panelSearch);
	}

	private IEnumerator SearchLord()
	{
		yield return new WaitForSeconds(1f);
		ShowPanel(panelAppoint);
		searchedLord = PlayInfo.lordManager.SearchRandom();
		RefreshLord(searchedLord);
		cost = LordManager.GetAppointCost();
		textAppointCost.text = cost.ToString();
	}
}
