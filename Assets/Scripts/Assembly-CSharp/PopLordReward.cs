using UnityEngine;

public class PopLordReward : MonoBehaviour
{
	public AuiButton buttonSubmit;

	public AuiButton buttonClose;

	public TextMesh textCost;

	public TextMesh textIncLoyalty;

	public TextMesh textCommandPts;

	public TextMesh textMessage;

	public UILord uiLord;

	private LordManager.Lord curlord;

	private int incLoyalty = 10;

	private int cost;

	public void Show(LordManager.Lord revalue)
	{
		buttonSubmit.isTop = true;
		buttonClose.isTop = true;
		buttonSubmit.enabled = true;
		buttonClose.enabled = true;
		base.gameObject.SetActive(true);
		curlord = revalue;
		textMessage.text = StringContent.msgQuestRewardLord;
		incLoyalty = 10;
		if (revalue.loyalty + incLoyalty >= 100)
		{
			incLoyalty = 100 - revalue.loyalty;
			if (incLoyalty < 0)
			{
				incLoyalty = 0;
			}
		}
		cost = LordManager.GetRewardCost(revalue);
		textCost.text = cost.ToString();
		textIncLoyalty.text = incLoyalty.ToString();
		textCommandPts.text = PlayInfo.gameRule.cmdPtsManageLord.ToString();
		buttonSubmit.onButtonClick = OnSubmitClick;
		buttonClose.onButtonClick = OnCloseClick;
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
		uiLord.SetButtonEnableAll(true);
	}

	private void OnSubmitClick(AuiButton sender)
	{
		if (PlayInfo.lordManager.list.IndexOf(curlord) == -1)
		{
			ProcBase.ShowMsg(StringContent.msgCannotRewardLord, MessageView.MsgIcon.alert);
			uiLord.Refresh();
			Hide();
			return;
		}
		if (PlayInfo.playerData.gold < cost)
		{
			ShowGotoGoldShop();
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
		PlayInfo.playerData.gold -= cost;
		PlayInfo.playerData.cmdPts -= PlayInfo.gameRule.cmdPtsManageLord;
		if (PlayInfo.playerData.cmdPts < 0)
		{
			PlayInfo.playerData.cmdPts = 0;
		}
		curlord.loyalty += incLoyalty;
		uiLord.Refresh();
		Hide();
	}

	private void OnCommandPointMessage(MessageView.MsgButton button)
	{
		if (button == MessageView.MsgButton.yes)
		{
			uiLord.procCastle.uiCmdPtsShop.Show();
		}
	}

	private void OnCloseClick(AuiButton sender)
	{
		Hide();
	}

	private void ShowGotoGoldShop()
	{
		ProcBase.ShowMsg(StringContent.msgNotEnoughGold, MessageView.MsgIcon.alert, new MessageView.MsgButton[2]
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
			uiLord.procCastle.uiGemShop.Show();
		}
	}
}
