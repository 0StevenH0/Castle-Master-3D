using UnityEngine;

public class MapSpy : MonoBehaviour
{
	public AuiButton buttonSubmit;

	public AuiButton buttonCancel;

	public TextMesh costGold;

	public TextMesh commandPtr;

	public MapCastleDetail castleDetail;

	public TextMesh labelSpy;

	public TextMesh labelCost;

	private int castleIndex;

	private void Start()
	{
		buttonSubmit.isTop = true;
		buttonCancel.isTop = true;
		buttonSubmit.onButtonClick = OnSubmitClick;
		buttonCancel.onButtonClick = OnCancelClick;
		labelCost.text = StringContent.wordCost;
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	private void OnSubmitClick(AuiButton sender)
	{
		Hide();
		if (PlayInfo.playerData.gold < PlayInfo.gameRule.spyCostGold)
		{
			ShowGotoGoldShop();
			return;
		}
		if (PlayInfo.playerData.cmdPts < PlayInfo.gameRule.cmdPtsSpy)
		{
			ProcBase.ShowMsg(StringContent.msgNotEnoughCmdPts, MessageView.MsgIcon.question, false, new MessageView.MsgButton[2]
			{
				MessageView.MsgButton.yes,
				MessageView.MsgButton.no
			}, OnCommandPointMessage);
			return;
		}
		PlayInfo.playerData.gold -= PlayInfo.gameRule.spyCostGold;
		PlayInfo.playerData.cmdPts -= PlayInfo.gameRule.cmdPtsSpy;
		if (PlayInfo.playerData.cmdPts < 0)
		{
			PlayInfo.playerData.cmdPts = 0;
		}
		PlayInfo.castleManager.castle[castleIndex].ActiveSpy();
		castleDetail.Show(castleIndex);
	}

	private void OnCommandPointMessage(MessageView.MsgButton button)
	{
		if (button == MessageView.MsgButton.yes)
		{
			UIMap component = base.transform.parent.gameObject.GetComponent<UIMap>();
			component.OnCommandPointClick(null);
		}
	}

	private void OnCancelClick(AuiButton sender)
	{
		Hide();
	}

	public void Hide()
	{
		base.gameObject.SetActiveRecursively(false);
		AuiButton.topActive = false;
	}

	public void Show(int castleIndex)
	{
		this.castleIndex = castleIndex;
		labelSpy.text = StringContent.msgSendSpy.Replace(StringContent.strValue, PlayInfo.castleManager.castle[castleIndex].castleName);
		costGold.text = PlayInfo.gameRule.spyCostGold.ToString();
		commandPtr.text = PlayInfo.gameRule.cmdPtsSpy.ToString();
		base.gameObject.SetActiveRecursively(true);
		AuiButton.topActive = true;
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
			UIMap component = base.transform.parent.gameObject.GetComponent<UIMap>();
			component.procCastle.uiGemShop.Show();
		}
	}
}
