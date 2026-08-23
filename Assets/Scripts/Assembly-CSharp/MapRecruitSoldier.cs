using System.Collections;
using UnityEngine;

public class MapRecruitSoldier : MonoBehaviour
{
	private const int maxScroll = 214;

	private const int minScroll = 35;

	private const int sizeScroll = 179;

	public AuiButton buttonSubmit;

	public AuiButton buttonInstant;

	public AuiButton buttonCancel;

	public TextMesh castleName;

	public TextMesh commandPtr;

	public TextMesh recruitDays;

	public TextMesh recruitMax;

	public TextMesh recruitSoldiers;

	public TextMesh recruitSum;

	public TextMesh recruitSumTop;

	public TextMesh textRecruiting;

	public TextMesh textInstantGem;

	public TextMesh labelMax;

	public TextMesh labelCurrent;

	public TextMesh labelCost;

	public RecruitUnit[] units;

	public GameObject objTerms;

	public GameObject panelRecruit;

	public GameObject panelInstant;

	private int castleIndex;

	private int maxRecruitCount = 100;

	private int selectUnit = -1;

	private bool isScrolling;

	private Vector3 scrollStartPos;

	private Vector3 scrollObjectPos;

	private int[] recruitUnitCount = new int[5];

	private int[] recruitUnitPrice = new int[5];

	private bool alreadyRecruit;

	private void Start()
	{
		buttonSubmit.isTop = true;
		buttonInstant.isTop = true;
		buttonCancel.isTop = true;
		buttonSubmit.onButtonClick = OnSubmitClick;
		buttonInstant.onButtonClick = OnInstantClick;
		buttonCancel.onButtonClick = OnCancelClick;
		for (int i = 0; i < 5; i++)
		{
			units[i].buttonCount.isTop = true;
			units[i].buttonCount.buttonTag = i;
			units[i].buttonCount.onButtonClick = OnUnitScrollClick;
			recruitUnitCount[i] = 0;
			recruitUnitPrice[i] = 0;
		}
		panelInstant.transform.position = panelRecruit.transform.position;
		labelMax.text = StringContent.wordMaxUnits;
		labelCurrent.text = StringContent.wordCurrentUnits;
		labelCost.text = StringContent.wordCost;
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	private void OnSubmitClick(AuiButton sender)
	{
		CastleInfo castleInfo = PlayInfo.castleManager.castle[castleIndex];
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int[] array = recruitUnitCount;
		foreach (int num4 in array)
		{
			num += num4;
			num2 += PlayInfo.humanMilitary.GetUnitState(num3).price * num4;
			num3++;
		}
		if (num <= 0)
		{
			ProcBase.ShowMsg(StringContent.msgNotSelectRecruitUnits, MessageView.MsgIcon.alert);
			return;
		}
		if (num > maxRecruitCount)
		{
			ProcBase.ShowMsg(StringContent.msgRecruitExceedMaxLimit, MessageView.MsgIcon.alert);
			return;
		}
		if (num2 > PlayInfo.playerData.gold)
		{
			ShowGotoGoldShop();
			return;
		}
		if (PlayInfo.gameRule.cmdPtsRecruitSoldier > PlayInfo.playerData.cmdPts)
		{
			ProcBase.ShowMsg(StringContent.msgNotEnoughCmdPts, MessageView.MsgIcon.question, false, new MessageView.MsgButton[2]
			{
				MessageView.MsgButton.yes,
				MessageView.MsgButton.no
			}, OnCommandPointMessage);
			return;
		}
		PlayInfo.playerData.gold -= num2;
		PlayInfo.playerData.cmdPts -= PlayInfo.gameRule.cmdPtsRecruitSoldier;
		if (PlayInfo.playerData.cmdPts < 0)
		{
			PlayInfo.playerData.cmdPts = 0;
		}
		castleInfo.SetRecruit(recruitUnitCount);
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
		int recruitInstantlyGem = PlayInfo.gameRule.recruitInstantlyGem;
		if (recruitInstantlyGem > PlayInfo.playerData.gem)
		{
			ShowGotoGemShop();
			return;
		}
		PlayInfo.playerData.gem -= recruitInstantlyGem;
		CastleInfo castleInfo = PlayInfo.castleManager.castle[castleIndex];
		castleInfo.FinishRecruit();
		Refresh();
	}

	private void OnCancelClick(AuiButton sender)
	{
		Hide();
	}

	public void Hide()
	{
		base.gameObject.SetActiveRecursively(false);
		AuiButton.topActive = false;
		StopCoroutine("RefreshRecruitStatus");
	}

	private void OnUnitScrollClick(AuiButton sender)
	{
		selectUnit = sender.buttonTag;
		scrollObjectPos = units[selectUnit].buttonCount.transform.localPosition;
	}

	private void Update()
	{
		if (alreadyRecruit || AuiButton.modalActive || AuiButton.mostTopActive)
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
			int num2 = (int)((localPosition.x - 35f) / 179f * (float)maxRecruitCount);
			recruitUnitCount[selectUnit] = num2;
			recruitUnitPrice[selectUnit] = num2 * PlayInfo.humanMilitary.GetUnitState(selectUnit).price;
			units[selectUnit].addCount.text = ((num2 <= 0) ? string.Empty : "+") + num2;
			int num3 = 0;
			int num4 = 0;
			for (int i = 0; i < recruitUnitCount.Length; i++)
			{
				num3 += recruitUnitCount[i];
				num4 += recruitUnitPrice[i];
			}
			recruitSoldiers.text = num3.ToString();
			recruitSum.text = num4.ToString();
			recruitSumTop.text = num4.ToString();
			units[selectUnit].buttonCount.transform.localPosition = localPosition;
		}
	}

	public void Show(int castleIndex)
	{
		this.castleIndex = castleIndex;
		base.gameObject.SetActiveRecursively(true);
		AuiButton.topActive = true;
		Refresh();
		CastleInfo castleInfo = PlayInfo.castleManager.castle[castleIndex];
		if (castleInfo.GetCurrentUnitTotal() >= CastleInfo.levelDefault[castleInfo.level - 1].maxUnit)
		{
			ProcBase.ShowMsg(StringContent.msgExceedUnitPerCastleLevel, MessageView.MsgIcon.alert);
		}
		StartCoroutine("RefreshRecruitStatus");
	}

	private void Refresh()
	{
		CastleInfo castleInfo = PlayInfo.castleManager.castle[castleIndex];
		alreadyRecruit = castleInfo.isRecruitSoldier;
		castleName.text = castleInfo.castleName;
		commandPtr.text = PlayInfo.gameRule.cmdPtsRecruitSoldier.ToString();
		recruitDays.text = PlayInfo.gameRule.recruitSoldierDays + " " + ((!(PlayInfo.gameRule.recruitSoldierDays > 1f)) ? StringContent.wordDay : StringContent.wordDays);
		maxRecruitCount = castleInfo.GetMaxRecruitSoldiers();
		recruitMax.text = maxRecruitCount.ToString();
		recruitSoldiers.text = "0";
		recruitSum.text = "0";
		recruitSumTop.text = "0";
		for (int i = 0; i < 5; i++)
		{
			bool flag = true;
			int requiredBldg = PlayInfo.humanMilitary.GetUnitState(i).requiredBldg;
			bool flag2 = false;
			if (requiredBldg > -1 && castleInfo.buildingStatus[requiredBldg] != CastleInfo.BuildingStatus.activate)
			{
				flag2 = true;
			}
			units[i].gameObject.SetActiveRecursively(flag);
			units[i].unitBlank.SetFrame(i);
			units[i].unitBlank.visible = flag2;
			if (flag)
			{
				units[i].unitIcon.SetFrame(i);
				units[i].unitLevel.text = PlayInfo.humanMilitary.GetUnitState(i).level.ToString();
				units[i].unitCount.text = castleInfo.unitCount[i].ToString();
				units[i].addCount.text = "0";
				units[i].unitIcon.visible = !flag2;
				units[i].buttonCount.visible = !flag2;
				if (units[i].iconRequired != null)
				{
					units[i].iconRequired.visible = flag2;
				}
				Vector3 localPosition = units[i].buttonCount.transform.localPosition;
				localPosition.x = 35f;
				units[i].buttonCount.transform.localPosition = localPosition;
			}
		}
		panelRecruit.gameObject.SetActiveRecursively(!alreadyRecruit);
		panelInstant.gameObject.SetActiveRecursively(alreadyRecruit);
		if (alreadyRecruit)
		{
			textRecruiting.gameObject.active = true;
			int day = (int)castleInfo.recruitHour / 24;
			int hour = (int)castleInfo.recruitHour % 24;
			textRecruiting.text = StringContent.msgRecruitingNow.Replace(StringContent.strValue, PlayInfo.gameTime.GetDayString(day, hour));
			int num = 0;
			for (int j = 0; j < 5; j++)
			{
				units[j].buttonCount.visible = false;
				int num2 = castleInfo.unitRecruit[j];
				num += num2;
				if (castleInfo.unitRecruit[j] > 0)
				{
					units[j].addCount.text = "+" + num2;
				}
				else
				{
					units[j].addCount.text = "0";
				}
			}
			recruitSoldiers.text = num.ToString();
			textInstantGem.text = PlayInfo.gameRule.recruitInstantlyGem.ToString();
		}
		else
		{
			for (int k = 0; k < 5; k++)
			{
				recruitUnitCount[k] = 0;
				recruitUnitPrice[k] = 0;
			}
			objTerms.SetActiveRecursively(true);
			textRecruiting.gameObject.active = false;
		}
	}

	private IEnumerator RefreshRecruitStatus()
	{
		while (true)
		{
			if (alreadyRecruit)
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
			UIMap component = base.transform.parent.gameObject.GetComponent<UIMap>();
			component.procCastle.uiGemShop.Show();
		}
	}
}
