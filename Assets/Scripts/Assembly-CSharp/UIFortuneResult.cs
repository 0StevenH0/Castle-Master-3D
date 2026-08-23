using UnityEngine;

public class UIFortuneResult : MonoBehaviour
{
	public AuiButton buttonClose;

	public AuiSprite[] iconRewardCurrency;

	public TextMesh[] textRewardName;

	public TextMesh[] textRewardValue;

	public TextMesh textMessage;

	private void Start()
	{
		buttonClose.isMostTop = true;
		buttonClose.onButtonClick = OnCloseClick;
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	private void OnCloseClick(AuiButton sender)
	{
		Hide();
	}

	public void Hide()
	{
		base.gameObject.SetActiveRecursively(false);
		AuiButton.mostTopActive = false;
	}

	public void Show(int fortuneEventIndex, int targetCastle)
	{
		base.gameObject.SetActiveRecursively(true);
		AuiButton.mostTopActive = true;
		FortuneSystem.FortuneEvent fortuneEvent = FortuneSystem.fortuneEvent[fortuneEventIndex];
		string text = string.Empty;
		switch (fortuneEventIndex)
		{
		case 0:
			text = StringContent.msgFortuneEvent0;
			break;
		case 1:
			text = StringContent.msgFortuneEvent1;
			break;
		case 2:
			text = StringContent.msgFortuneEvent2;
			break;
		case 3:
			text = StringContent.msgFortuneEvent3;
			break;
		case 4:
			text = StringContent.msgFortuneEvent4;
			break;
		case 5:
			text = StringContent.msgFortuneEvent5;
			break;
		case 6:
			text = StringContent.msgFortuneEvent6;
			break;
		case 7:
			text = StringContent.msgFortuneEvent7;
			break;
		case 8:
			text = StringContent.msgFortuneEvent8;
			break;
		case 9:
			text = StringContent.msgFortuneEvent9;
			break;
		case 10:
			text = StringContent.msgFortuneEvent10;
			break;
		case 11:
			text = StringContent.msgFortuneEvent11;
			break;
		case 12:
			text = StringContent.msgFortuneEvent12;
			break;
		}
		if (fortuneEvent.fortuneCastle == FortuneSystem.FortuneCastle.random)
		{
			text = text.Replace(StringContent.strValue, PlayInfo.castleManager.castle[targetCastle].castleName);
		}
		textMessage.text = text;
		int num = 3;
		int num2 = 0;
		if (fortuneEvent.rewardGold != 0 && num2 < num)
		{
			iconRewardCurrency[num2].visible = true;
			iconRewardCurrency[num2].SetFrame(0);
			textRewardName[num2].text = StringContent.wordGold;
			textRewardValue[num2].text = ((fortuneEvent.rewardGold <= 0) ? string.Empty : "+") + fortuneEvent.rewardGold;
			num2++;
		}
		if (fortuneEvent.rewardGem != 0 && num2 < num)
		{
			iconRewardCurrency[num2].visible = true;
			iconRewardCurrency[num2].SetFrame(1);
			textRewardName[num2].text = StringContent.wordGem;
			textRewardValue[num2].text = ((fortuneEvent.rewardGem <= 0) ? string.Empty : "+") + fortuneEvent.rewardFame;
			num2++;
		}
		if (fortuneEvent.rewardLoyalty != 0 && num2 < num)
		{
			iconRewardCurrency[num2].visible = false;
			textRewardName[num2].text = StringContent.wordLoyalty;
			textRewardValue[num2].text = ((fortuneEvent.rewardLoyalty <= 0) ? string.Empty : "+") + fortuneEvent.rewardLoyalty;
			num2++;
		}
		if (fortuneEvent.rewardResidents != 0 && num2 < num)
		{
			iconRewardCurrency[num2].visible = false;
			textRewardName[num2].text = StringContent.wordResidents;
			textRewardValue[num2].text = ((fortuneEvent.rewardResidents <= 0) ? string.Empty : "+") + fortuneEvent.rewardResidents;
			num2++;
		}
		if (fortuneEvent.rewardFame != 0 && num2 < num)
		{
			iconRewardCurrency[num2].visible = false;
			textRewardName[num2].text = StringContent.wordFame;
			textRewardValue[num2].text = ((fortuneEvent.rewardFame <= 0) ? string.Empty : "+") + fortuneEvent.rewardFame;
			num2++;
		}
		for (int i = num2; i < num; i++)
		{
			iconRewardCurrency[num2].visible = false;
			textRewardName[num2].text = string.Empty;
			textRewardValue[num2].text = string.Empty;
		}
	}
}
