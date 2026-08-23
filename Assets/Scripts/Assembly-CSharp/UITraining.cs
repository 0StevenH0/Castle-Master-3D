using System.Collections;
using UnityEngine;

public class UITraining : MonoBehaviour
{
	public AuiButton buttonClose;

	public AuiButton buttonTran;

	public AuiButton buttonInstant;

	public GameObject panelTrain;

	public GameObject panelInstant;

	public AuiSprite costCurrency;

	public TextMesh commandPts;

	public TextMesh trainCost;

	public TextMesh instantGem;

	public TextMesh textTraining;

	public TextMesh textPeriod;

	public TextMesh textUnitName;

	public TextMesh textHp;

	public TextMesh textAtk;

	public TextMesh textLevel;

	public TextMesh textIncHp;

	public TextMesh textIncAtk;

	public TextMesh textIncLevel;

	public TextMesh textRequires;

	public TextMesh labelRequires;

	public AuiSprite iconSelected;

	public TrainingUnit[] units;

	public ProcCastle procCastle;

	private int trainingIndex;

	private void Start()
	{
		buttonClose.isTop = true;
		buttonTran.isTop = true;
		buttonInstant.isTop = true;
		buttonTran.onButtonClick = OnTranClick;
		buttonInstant.onButtonClick = OnInstantClick;
		panelInstant.transform.position = panelTrain.transform.position;
		labelRequires.text = StringContent.wordRequires;
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	private void OnTranClick(AuiButton sender)
	{
		int num = trainingIndex;
		UnitState unitState = PlayInfo.humanMilitary.GetUnitState(num);
		if (PlayInfo.playerData.cmdPts < PlayInfo.gameRule.cmdPtsTraining)
		{
			ProcBase.ShowMsg(StringContent.msgNotEnoughCmdPts, MessageView.MsgIcon.question, false, new MessageView.MsgButton[2]
			{
				MessageView.MsgButton.yes,
				MessageView.MsgButton.no
			}, OnCommandPointMessage);
			return;
		}
		int level = unitState.level;
		int costGold = 0;
		PlayInfo.humanMilitary.GetUnitLevelCost(num, level, ref costGold);
		if (PlayInfo.playerData.gold < costGold)
		{
			ShowGotoGoldShop();
			return;
		}
		PlayInfo.playerData.cmdPts -= PlayInfo.gameRule.cmdPtsTraining;
		if (PlayInfo.playerData.cmdPts < 0)
		{
			PlayInfo.playerData.cmdPts = 0;
		}
		PlayInfo.playerData.gold -= costGold;
		PlayInfo.unitUpgrade.StartUpgrade(num);
		Refresh();
	}

	private void OnCommandPointMessage(MessageView.MsgButton button)
	{
		if (button == MessageView.MsgButton.yes)
		{
			procCastle.uiCmdPtsShop.Show();
		}
	}

	private void OnInstantClick(AuiButton sender)
	{
		int num = trainingIndex;
		int upgradeInstantlyGem = PlayInfo.humanMilitary.GetUnitState(num).upgradeInstantlyGem;
		if (PlayInfo.playerData.gem < upgradeInstantlyGem)
		{
			ShowGotoGemShop();
			return;
		}
		PlayInfo.playerData.gem -= upgradeInstantlyGem;
		PlayInfo.unitUpgrade.FinishUpgrade(num);
		Refresh();
	}

	private void OnUnitClick(AuiButton sender)
	{
		trainingIndex = sender.buttonTag;
		ShowTraningInfo();
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
		AuiButton.topActive = false;
		StopCoroutine("RefreshTrainingStatus");
	}

	public void Show()
	{
		base.gameObject.SetActive(true);
		AuiButton.topActive = true;
		Refresh();
		StartCoroutine("RefreshTrainingStatus");
	}

	private void Refresh()
	{
		for (int i = 0; i < 5; i++)
		{
			units[i].unitDetail.isTop = true;
			units[i].unitDetail.buttonTag = i;
			units[i].unitDetail.onButtonClick = OnUnitClick;
			UnitState unitState = PlayInfo.humanMilitary.GetUnitState(i);
			units[i].unitIcon.SetFrame(i);
			units[i].unitLevel.text = unitState.level.ToString();
			units[i].aniTraining.visible = PlayInfo.unitUpgrade.unitUpgrading[i];
			if (PlayInfo.unitUpgrade.unitUpgrading[i])
			{
				units[i].aniTraining.StartAnimation(true, false);
			}
		}
		ShowTraningInfo();
	}

	public void ShowTraningInfo()
	{
		int num = trainingIndex;
		iconSelected.transform.position = units[num].unitDetail.transform.position;
		UnitState unitState = PlayInfo.humanMilitary.GetUnitState(num);
		textUnitName.text = StringContent.wordSoldier[num];
		textHp.text = unitState.sumHp.ToString();
		textAtk.text = unitState.sumAttack.ToString();
		textLevel.text = "Lv. " + unitState.level;
		UnitState unitState2 = unitState.Clone();
		unitState2.level++;
		textIncHp.text = "+" + (unitState2.sumHp - unitState.sumHp);
		textIncAtk.text = "+" + (unitState2.sumAttack - unitState.sumAttack);
		textIncLevel.text = "Lv. " + unitState2.level;
		string wordCastleLevel = StringContent.wordCastleLevel;
		int requiredBldg = unitState.requiredBldg;
		if (requiredBldg < 0)
		{
			wordCastleLevel += "1";
		}
		else
		{
			wordCastleLevel += CastleInfo.buildingAttribute[requiredBldg].requireLevel;
			wordCastleLevel = wordCastleLevel + "\n" + StringContent.wordBuilding[requiredBldg];
		}
		textRequires.text = wordCastleLevel;
		if (PlayInfo.unitUpgrade.unitUpgrading[num])
		{
			panelTrain.SetActive(false);
			panelInstant.SetActive(true);
			int upgradeInstantlyGem = PlayInfo.humanMilitary.GetUnitState(num).upgradeInstantlyGem;
			float num2 = PlayInfo.unitUpgrade.unitUpgradeHour[num];
			textTraining.text = StringContent.msgNowTraining.Replace(StringContent.strValue, PlayInfo.gameTime.GetDayString((int)num2 / 24, (int)num2 % 24));
			units[num].aniTraining.visible = true;
			units[num].aniTraining.StartAnimation(true, false);
			instantGem.text = upgradeInstantlyGem.ToString();
			buttonInstant.SetFrame(0);
			buttonInstant.visible = true;
			return;
		}
		bool flag = unitState.level >= 50;
		panelTrain.SetActive(!flag);
		panelInstant.SetActive(false);
		if (!flag)
		{
			int level = unitState.level;
			int costGold = 0;
			PlayInfo.humanMilitary.GetUnitLevelCost(num, level, ref costGold);
			trainCost.text = costGold.ToString();
			commandPts.text = PlayInfo.gameRule.cmdPtsTraining.ToString();
			int upgradeDays = PlayInfo.humanMilitary.GetUnitState(num).upgradeDays;
			textPeriod.text = upgradeDays + " " + ((upgradeDays <= 1) ? StringContent.wordDay : StringContent.wordDays);
			buttonTran.SetFrame(0);
			buttonTran.visible = true;
		}
		units[num].aniTraining.StopAnimation(true);
	}

	private IEnumerator RefreshTrainingStatus()
	{
		while (true)
		{
			Refresh();
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
			procCastle.uiGemShop.Show();
		}
	}
}
