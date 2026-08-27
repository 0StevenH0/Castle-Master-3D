using System.Collections;
using UnityEngine;

public class MapRedeploy : MonoBehaviour
{
	private const int maxScroll = 214;

	private const int minScroll = 35;

	private const int sizeScroll = 179;

	public AuiButton buttonSubmit;

	public AuiSprite buttonSubmitLabel;

	public AuiButton buttonInstant;

	public GameObject ObjectInstant;

	public AuiButton buttonClose;

	public TextMesh castleNameFrom;

	public TextMesh castleNameTo;

	public TextMesh commandPtr;

	public TextMesh periodDays;

	public TextMesh unitMax;

	public TextMesh unitCur;

	public TextMesh redeployCostGold;

	public TextMesh instantlyGem;

	public TextMesh textRedeploying;

	public TextMesh labelMax;

	public TextMesh labelCurrent;

	public RedeployUnit[] units;

	public GameObject objBefRedeploy;

	public GameObject objRedeploying;

	public UIMap uiMap;

	private int castleIndex;

	private int targetIndex;

	private int maxRedeployCount = 100;

	private int selectUnit = -1;

	private bool isScrolling;

	private Vector3 scrollStartPos;

	private Vector3 scrollObjectPos;

	private int[] redeployUnitCount = new int[5];

	private bool alreadyRedeploy;

	private void Start()
	{
		buttonSubmit.isTop = true;
		buttonClose.isTop = true;
		buttonInstant.isTop = true;
		buttonSubmit.onButtonClick = OnSubmitClick;
		buttonInstant.onButtonClick = OnInstantClick;
		buttonClose.onButtonClick = OnCloseClick;
		for (int i = 0; i < 5; i++)
		{
			units[i].buttonCount.isTop = true;
			units[i].buttonCount.buttonTag = i;
			units[i].buttonCount.onButtonClick = OnUnitScrollClick;
			redeployUnitCount[i] = 0;
		}
		labelMax.text = StringContent.wordMaxUnits;
		labelCurrent.text = StringContent.wordCurrentUnits;
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	private void OnSubmitClick(AuiButton sender)
	{
		CastleInfo castleInfo = PlayInfo.castleManager.castle[castleIndex];
		int num = 0;
		int redeployGold = PlayInfo.gameRule.redeployGold;
		int[] array = redeployUnitCount;
		foreach (int num2 in array)
		{
			num += num2;
		}
		if (num <= 0)
		{
			ProcBase.ShowMsg(StringContent.msgNotSelectRedeployUnits, MessageView.MsgIcon.alert);
			return;
		}
		if (num > maxRedeployCount)
		{
			ProcBase.ShowMsg(StringContent.msgRedeployExceedMaxLimit, MessageView.MsgIcon.alert);
			return;
		}
		if (redeployGold > PlayInfo.playerData.gold)
		{
			ShowGotoGoldShop();
			return;
		}
		if (PlayInfo.gameRule.cmdPtsRedeploy > PlayInfo.playerData.cmdPts)
		{
			ProcBase.ShowMsg(StringContent.msgNotEnoughCmdPts, MessageView.MsgIcon.question, false, new MessageView.MsgButton[2]
			{
				MessageView.MsgButton.yes,
				MessageView.MsgButton.no
			}, OnCommandPointMessage);
			return;
		}
		PlayInfo.playerData.gold -= redeployGold;
		PlayInfo.playerData.cmdPts -= PlayInfo.gameRule.cmdPtsRedeploy;
		if (PlayInfo.playerData.cmdPts < 0)
		{
			PlayInfo.playerData.cmdPts = 0;
		}
		castleInfo.SetRedeploy(redeployUnitCount, targetIndex, PlayInfo.castleManager.castle[targetIndex]);
		Refresh();
	}

	private void OnCommandPointMessage(MessageView.MsgButton button)
	{
		if (button == MessageView.MsgButton.yes)
		{
			UIMap component = base.transform.parent.gameObject.GetComponent<UIMap>();
			component.OnCommandPointClick(null);
		}
	}

	private void OnInstantClick(AuiButton sender)
	{
		CastleInfo castleInfo = PlayInfo.castleManager.castle[castleIndex];
		if (!castleInfo.isRedeploy)
		{
			ProcBase.ShowMsg(StringContent.msgAlreadyRedeploy, MessageView.MsgIcon.alert);
			Hide();
		}
		else if (castleInfo.redeployTarget.side != 0)
		{
			ProcBase.ShowMsg(StringContent.msgCannotRedeploy, MessageView.MsgIcon.alert);
			Hide();
		}
		else if (PlayInfo.playerData.gem < PlayInfo.gameRule.redeployInstantlyGem)
		{
			ShowGotoGemShop();
		}
		else
		{
			PlayInfo.playerData.gem -= PlayInfo.gameRule.redeployInstantlyGem;
			castleInfo.FinishRedeploy();
			Hide();
		}
	}

	private void OnCloseClick(AuiButton sender)
	{
		Hide();
	}

	public void Hide()
	{
		base.gameObject.SetActiveRecursive(false);
		AuiButton.topActive = false;
		uiMap.DisableCastleTargetMode();
		StopCoroutine("RefreshRedeployStatus");
	}

	private void OnUnitScrollClick(AuiButton sender)
	{
		selectUnit = sender.buttonTag;
		scrollObjectPos = units[selectUnit].buttonCount.transform.localPosition;
	}

	private void Update()
	{
		if (alreadyRedeploy || AuiButton.modalActive || AuiButton.mostTopActive)
		{
			return;
		}
		if (Input.GetMouseButtonDown(0))
		{
			isScrolling = true;
			Vector3 mousePosition = Input.mousePosition;
			scrollStartPos = buttonSubmit.uiCamera.ScreenToWorldPoint(mousePosition);
		}
		else if (Input.GetMouseButtonUp(0))
		{
			isScrolling = false;
			selectUnit = -1;
		}
		if (!isScrolling || selectUnit <= -1)
		{
			return;
		}
		Vector3 mousePosition2 = Input.mousePosition;
		mousePosition2 = buttonSubmit.uiCamera.ScreenToWorldPoint(mousePosition2);
		if (mousePosition2.x != scrollStartPos.x)
		{
			float num = mousePosition2.x - scrollStartPos.x;
			Vector3 localPosition = scrollObjectPos;
			localPosition.x += num;
			if (localPosition.x > 214f)
			{
				localPosition.x = 214f;
			}
			if (localPosition.x < 35f)
			{
				localPosition.x = 35f;
			}
			CastleInfo castleInfo = PlayInfo.castleManager.castle[castleIndex];
			int num2 = (int)((localPosition.x - 35f) / 179f * (float)(castleInfo.unitCount[selectUnit] + 1));
			if (num2 > castleInfo.unitCount[selectUnit])
			{
				num2 = castleInfo.unitCount[selectUnit];
			}
			redeployUnitCount[selectUnit] = num2;
			units[selectUnit].addCount.text = num2.ToString();
			int num3 = 0;
			for (int i = 0; i < redeployUnitCount.Length; i++)
			{
				num3 += redeployUnitCount[i];
			}
			unitCur.text = num3.ToString();
			units[selectUnit].buttonCount.transform.localPosition = localPosition;
		}
	}

	public void Show(int castleIndex, int targetIndex)
	{
		this.castleIndex = castleIndex;
		CastleInfo castleInfo = PlayInfo.castleManager.castle[castleIndex];
		if (targetIndex == -1)
		{
			if (!castleInfo.isRedeploy)
			{
				return;
			}
			targetIndex = castleInfo.redeployTargetIndex;
		}
		this.targetIndex = targetIndex;
		uiMap.DisableCastleTargetMode();
		base.gameObject.SetActiveRecursive(true);
		AuiButton.topActive = true;
		Refresh();
		StartCoroutine("RefreshRedeployStatus");
	}

	private void Refresh()
	{
		CastleInfo castleInfo = PlayInfo.castleManager.castle[castleIndex];
		CastleInfo castleInfo2 = PlayInfo.castleManager.castle[targetIndex];
		alreadyRedeploy = castleInfo.isRedeploy;
		castleNameFrom.text = castleInfo.castleName;
		castleNameTo.text = castleInfo2.castleName;
		commandPtr.text = PlayInfo.gameRule.cmdPtsRedeploy.ToString();
		int num = (int)PlayInfo.gameRule.redeploySoldierDays;
		periodDays.text = num + ((num <= 1) ? StringContent.wordDay : StringContent.wordDays);
		maxRedeployCount = 300;
		redeployCostGold.text = PlayInfo.gameRule.redeployGold.ToString();
		unitMax.text = maxRedeployCount.ToString();
		unitCur.text = "0";
		for (int i = 0; i < 5; i++)
		{
			redeployUnitCount[i] = 0;
		}
		for (int j = 0; j < 5; j++)
		{
			bool flag = true;
			units[j].gameObject.SetActiveRecursive(flag);
			units[j].unitBlank.SetFrame(j);
			units[j].unitBlank.visible = castleInfo.unitCount[j] == 0;
			if (flag)
			{
				units[j].unitIcon.SetFrame(j);
				units[j].unitIcon.visible = castleInfo.unitCount[j] > 0;
				units[j].unitLevel.text = PlayInfo.humanMilitary.GetUnitState(j).level.ToString();
				units[j].unitCount.text = castleInfo.unitCount[j].ToString();
				units[j].addCount.text = "0";
				Vector3 localPosition = units[j].buttonCount.transform.localPosition;
				localPosition.x = 35f;
				units[j].buttonCount.transform.localPosition = localPosition;
			}
			units[j].buttonCount.visible = castleInfo.unitCount[j] > 0;
		}
		if (alreadyRedeploy)
		{
			buttonSubmit.visible = false;
			buttonSubmitLabel.visible = false;
			textRedeploying.gameObject.SetActive(true);
			int day = (int)castleInfo.redeployHour / 24;
			int hour = (int)castleInfo.redeployHour % 24;
			textRedeploying.text = StringContent.msgRedeployingNow.Replace(StringContent.strValue, PlayInfo.gameTime.GetDayString(day, hour));
			int num2 = 0;
			for (int k = 0; k < 5; k++)
			{
				units[k].buttonCount.visible = false;
				int num3 = castleInfo.redeployUnitCount[k];
				num2 += num3;
				units[k].addCount.text = num3.ToString();
			}
			unitCur.text = num2.ToString();
			instantlyGem.text = PlayInfo.gameRule.redeployInstantlyGem.ToString();
			ObjectInstant.transform.position = buttonSubmit.transform.position;
			objBefRedeploy.SetActiveRecursive(false);
			objRedeploying.SetActiveRecursive(true);
		}
		else
		{
			buttonSubmit.visible = true;
			buttonSubmitLabel.visible = true;
			objBefRedeploy.SetActiveRecursive(true);
			objRedeploying.SetActiveRecursive(false);
		}
	}

	private IEnumerator RefreshRedeployStatus()
	{
		while (true)
		{
			if (alreadyRedeploy)
			{
				Refresh();
			}
			yield return new WaitForSeconds(0.5f);
		}
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
}
