using System.Collections;
using UnityEngine;

public class MapUpgrade : MonoBehaviour
{
	public TextMesh castleName;

	public AuiSprite sprLevelFrom;

	public TextMesh levelFrom;

	public AuiSprite sprLevelTo;

	public TextMesh levelTo;

	public TextMesh fromMaxSoldiers;

	public TextMesh fromMaxPopulation;

	public TextMesh fromCastleDefense;

	public TextMesh fromBattleUnits;

	public TextMesh toMaxSoldiers;

	public TextMesh toMaxPopulation;

	public TextMesh toCastleDefense;

	public TextMesh toBattleUnits;

	public TextMesh labelMaxSoldiers;

	public TextMesh labelMaxPopulation;

	public TextMesh labelCastleDefense;

	public TextMesh labelBattleUnits;

	public TextMesh commandPtr;

	public TextMesh costGem;

	public TextMesh costGold;

	public TextMesh period;

	public AuiButton buttonUpgrade;

	public AuiButton buttonInstantly;

	public AuiButton buttonCancel;

	public GameObject objForUpgrade;

	public GameObject objUpgrading;

	public GameObject objInstant;

	public TextMesh textUpgrading;

	public AuiSpriteAnimation aniUpgrading;

	private int castleIndex;

	private CastleInfo info;

	private void Start()
	{
		buttonUpgrade.isTop = true;
		buttonInstantly.isTop = true;
		buttonCancel.isTop = true;
		buttonUpgrade.onButtonClick = OnUpgradeClick;
		buttonInstantly.onButtonClick = OnInstantlyClick;
		buttonCancel.onButtonClick = OnCancelClick;
		Vector3 position = buttonUpgrade.transform.position;
		position.z = objInstant.transform.position.z;
		objInstant.transform.position = position;
		labelMaxSoldiers.text = StringContent.wordMaxSoldiers;
		labelMaxPopulation.text = StringContent.wordMaxCitizen;
		labelCastleDefense.text = StringContent.wordCastleDefense;
		labelBattleUnits.text = StringContent.wordBattleUnits;
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	private bool CheckCanUpgrade()
	{
		if (info.isUpgrading)
		{
			ProcBase.ShowMsg(StringContent.msgUpgradingAlready, MessageView.MsgIcon.alert);
			return false;
		}
		if (info.level >= 3)
		{
			ProcBase.ShowMsg(StringContent.msgAlreadyTopLevel, MessageView.MsgIcon.alert);
			return false;
		}
		bool flag = true;
		string text = string.Empty;
		for (int i = 0; i < 7; i++)
		{
			if (CastleInfo.buildingAttribute[i].requireLevel == info.level && info.buildingStatus[i] != CastleInfo.BuildingStatus.activate)
			{
				if (!flag)
				{
					text += ",";
				}
				text += StringContent.wordBuilding[i];
				flag = false;
			}
		}
		if (!flag)
		{
			ProcBase.ShowMsg(StringContent.msgRequireBuildingForUpgrade.Replace(StringContent.strValue, text), MessageView.MsgIcon.alert);
			return false;
		}
		if (PlayInfo.playerData.cmdPts < PlayInfo.gameRule.cmdPtsUpgradeCastle)
		{
			ProcBase.ShowMsg(StringContent.msgNotEnoughCmdPts, MessageView.MsgIcon.question, false, new MessageView.MsgButton[2]
			{
				MessageView.MsgButton.yes,
				MessageView.MsgButton.no
			}, OnCommandPointMessage);
			return false;
		}
		return true;
	}

	private void OnCommandPointMessage(MessageView.MsgButton button)
	{
		if (button == MessageView.MsgButton.yes)
		{
			UIMap component = base.transform.parent.gameObject.GetComponent<UIMap>();
			component.OnCommandPointClick(null);
		}
	}

	private void OnUpgradeClick(AuiButton sender)
	{
		if (!CheckCanUpgrade())
		{
			return;
		}
		if (PlayInfo.playerData.gold < CastleInfo.levelDefault[info.level].upgradeGold)
		{
			ShowGotoGoldShop();
			return;
		}
		PlayInfo.playerData.cmdPts -= PlayInfo.gameRule.cmdPtsUpgradeCastle;
		if (PlayInfo.playerData.cmdPts < 0)
		{
			PlayInfo.playerData.cmdPts = 0;
		}
		PlayInfo.playerData.gold -= CastleInfo.levelDefault[info.level].upgradeGold;
		info.SetUpgrading();
		Refresh();
	}

	private void OnInstantlyClick(AuiButton sender)
	{
		if (!info.isUpgrading)
		{
			ProcBase.ShowMsg(StringContent.msgAlreadyUpgraded, MessageView.MsgIcon.alert);
			Hide();
		}
		else if (PlayInfo.playerData.gem < CastleInfo.levelDefault[info.level].upgradeInstantlyGem)
		{
			ShowGotoGemShop();
		}
		else
		{
			PlayInfo.playerData.gem -= CastleInfo.levelDefault[info.level].upgradeInstantlyGem;
			info.FinishUpgrade();
			Refresh();
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
		StopCoroutine("RefreshUpgradeStatus");
	}

	public void Show(int castleIndex)
	{
		this.castleIndex = castleIndex;
		base.gameObject.SetActiveRecursively(true);
		AuiButton.topActive = true;
		Refresh();
		StartCoroutine("RefreshUpgradeStatus");
	}

	private void Refresh()
	{
		info = PlayInfo.castleManager.castle[castleIndex];
		castleName.text = info.castleName;
		bool flag = info.level + 1 > 3;
		if (flag)
		{
			Hide();
			return;
		}
		levelFrom.text = "Lv." + info.level;
		if (flag)
		{
			levelTo.text = string.Empty;
		}
		else
		{
			levelTo.text = "Lv." + (info.level + 1);
		}
		sprLevelFrom.SetFrame(info.level - 1);
		if (flag)
		{
			sprLevelTo.visible = false;
		}
		else
		{
			sprLevelTo.visible = true;
			sprLevelTo.SetFrame(info.level);
		}
		CastleInfo.CastleLevelDefault castleLevelDefault = CastleInfo.levelDefault[info.level - 1];
		CastleInfo.CastleLevelDefault castleLevelDefault2 = ((!flag) ? CastleInfo.levelDefault[info.level] : null);
		fromMaxSoldiers.text = castleLevelDefault.maxUnit.ToString();
		fromMaxPopulation.text = castleLevelDefault.maxPopulation.ToString();
		fromCastleDefense.text = castleLevelDefault.defense.ToString();
		fromBattleUnits.text = castleLevelDefault.battleUnit.ToString();
		if (flag)
		{
			toMaxSoldiers.text = string.Empty;
			toMaxPopulation.text = string.Empty;
			toCastleDefense.text = string.Empty;
			toBattleUnits.text = string.Empty;
		}
		else
		{
			toMaxSoldiers.text = castleLevelDefault2.maxUnit.ToString();
			toMaxPopulation.text = castleLevelDefault2.maxPopulation.ToString();
			toCastleDefense.text = castleLevelDefault2.defense.ToString();
			toBattleUnits.text = castleLevelDefault2.battleUnit.ToString();
		}
		commandPtr.text = PlayInfo.gameRule.cmdPtsUpgradeCastle.ToString();
		if (flag)
		{
			costGem.text = string.Empty;
			costGold.text = string.Empty;
			period.text = string.Empty;
		}
		else
		{
			costGem.text = castleLevelDefault2.upgradeInstantlyGem.ToString();
			costGold.text = castleLevelDefault2.upgradeGold.ToString();
			period.text = castleLevelDefault2.upgradePeriod + " " + ((castleLevelDefault2.upgradePeriod <= 1) ? StringContent.wordDay : StringContent.wordDays);
		}
		if (flag)
		{
			objForUpgrade.SetActiveRecursively(false);
			objUpgrading.SetActiveRecursively(false);
			aniUpgrading.StopAnimation(true);
		}
		else if (info.isUpgrading)
		{
			int day = (int)info.upgradeHour / 24;
			int hour = (int)info.upgradeHour % 24;
			textUpgrading.text = StringContent.msgUpgradingNow.Replace(StringContent.strValue, PlayInfo.gameTime.GetDayString(day, hour));
			costGem.text = CastleInfo.levelDefault[info.level].upgradeInstantlyGem.ToString();
			objForUpgrade.SetActiveRecursively(false);
			objUpgrading.SetActiveRecursively(true);
			aniUpgrading.visible = true;
			aniUpgrading.StartAnimation(0, true, false);
		}
		else
		{
			objForUpgrade.SetActiveRecursively(true);
			objUpgrading.SetActiveRecursively(false);
			aniUpgrading.StopAnimation(true);
		}
	}

	private IEnumerator RefreshUpgradeStatus()
	{
		while (true)
		{
			info = PlayInfo.castleManager.castle[castleIndex];
			if (info.isUpgrading)
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
