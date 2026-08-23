using UnityEngine;

public class MapMercy : MonoBehaviour
{
	public AuiButton buttonClose;

	public AuiButton[] buttonRaise;

	public TextMesh[] textRaiseCost;

	public TextMesh textCastleName;

	public TextMesh textDesc;

	public MapCastleDetail castleDetail;

	private CastleInfo info;

	private void Start()
	{
		buttonClose.isTop = true;
		buttonClose.onButtonClick = OnCloseClick;
		for (int i = 0; i < buttonRaise.Length; i++)
		{
			buttonRaise[i].isTop = true;
			buttonRaise[i].onButtonClick = OnRaiseMercyClick;
			buttonRaise[i].buttonTag = i;
		}
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	private void OnCloseClick(AuiButton sender)
	{
		Hide();
	}

	private void OnRaiseMercyClick(AuiButton sender)
	{
		int buttonTag = sender.buttonTag;
		float num = PlayInfo.mercyTable[buttonTag].raiseRate;
		int costGold = PlayInfo.mercyTable[buttonTag].costGold;
		if (info.loyalty >= 100)
		{
			ProcBase.ShowMsg(StringContent.msgMaxLimitLoyalty, MessageView.MsgIcon.alert);
			return;
		}
		if (PlayInfo.playerData.gold < costGold)
		{
			ShowGotoGoldShop();
			return;
		}
		info.loyalty += (int)num;
		PlayInfo.playerData.gold -= costGold;
		castleDetail.Refresh();
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
			castleDetail.procCastle.uiGemShop.Show();
		}
	}

	public void Show(int castleIndex)
	{
		base.gameObject.SetActiveRecursively(true);
		AuiButton.topActive = true;
		textDesc.text = StringContent.msgMercyCitizenLoyalty;
		info = PlayInfo.castleManager.castle[castleIndex];
		textCastleName.text = info.castleName;
		for (int i = 0; i < 3; i++)
		{
			textRaiseCost[i].text = PlayInfo.mercyTable[i].costGold.ToString();
		}
	}

	public void Hide()
	{
		base.gameObject.SetActiveRecursively(false);
		AuiButton.topActive = false;
	}
}
