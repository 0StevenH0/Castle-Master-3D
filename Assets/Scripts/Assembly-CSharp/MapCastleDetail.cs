using System.Collections;
using UnityEngine;

public class MapCastleDetail : MonoBehaviour
{
	public const int cmdMsgRecruit = 0;

	public const int cmdMsgSpy = 0;

	public const int cmdMsgUpgrade = 1;

	public const int cmdMsgBuild = 2;

	public AuiButton buttonClose;

	public TextMesh castleName;

	public TextMesh castleLevel;

	public TextMesh castleLord;

	public TextMesh castleFame;

	public TextMesh castleLoyalty;

	public TextMesh castleTax;

	public TextMesh castlePopulation;

	public TextMesh castleUnitTotal;

	public TextMesh castleDefense;

	public TextMesh[] castleCommandMessage;

	public AuiSprite[] unitIcon;

	public AuiSprite[] unitIconBlank;

	public AuiSprite[] unitIconUnknown;

	public TextMesh[] unitLevel;

	public TextMesh[] unitCount;

	public TextMesh labelLord;

	public TextMesh labelFame;

	public TextMesh labelLoyalty;

	public TextMesh labelArmy;

	public TextMesh labelTax;

	public TextMesh labelCitizen;

	public TextMesh labelDefense;

	public AuiButton buttonArmy;

	public AuiButton buttonEconomy;

	public AuiButton buttonPolitics;

	public AuiButton buttonMove;

	public AuiButton buttonMercy;

	public AuiButton buttonTraining;

	public AuiButton buttonAttack;

	public AuiButton buttonSpy;

	public AuiSprite buttonArmyLabel;

	public AuiSprite buttonEconomyLabel;

	public AuiSprite buttonPoliticsLabel;

	public AuiSprite buttonMoveLabel;

	public AuiSprite buttonMercyLabel;

	public AuiSprite buttonTrainingLabel;

	public AuiSprite buttonAttackLabel;

	public AuiSprite buttonSpyLabel;

	public MapRedeploy popupRedeploy;

	public MapSpy popupSpy;

	public MapRecruitSoldier popupRecruitSoldier;

	public MapEconomy popupEconomy;

	public MapUpgrade popupUpgrade;

	public MapMercy popupMercy;

	public MapPolitics popupPolitics;

	public ProcCastle procCastle;

	public CastleInterface[] castles;

	public int castleIndex = -1;

	private void Start()
	{
		buttonClose.onButtonClick = OnCloseClick;
		buttonArmy.onButtonClick = OnArmyClick;
		buttonEconomy.onButtonClick = OnEconomyClick;
		buttonPolitics.onButtonClick = OnPoliticsClick;
		buttonMove.onButtonClick = OnRedeployClick;
		buttonTraining.onButtonClick = OnTrainingClick;
		buttonAttack.onButtonClick = OnAttackClick;
		buttonSpy.onButtonClick = OnSpyClick;
		buttonMercy.onButtonClick = OnMercyClick;
		labelLord.text = StringContent.wordLord;
		labelFame.text = StringContent.wordFame;
		labelLoyalty.text = StringContent.wordLoyalty;
		labelArmy.text = StringContent.wordArmy;
		labelTax.text = StringContent.wordTax;
		labelCitizen.text = StringContent.wordResidents;
		labelDefense.text = StringContent.wordDefense;
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	private void OnCloseClick(AuiButton sender)
	{
		Hide();
	}

	private void OnArmyClick(AuiButton sender)
	{
		popupRecruitSoldier.Show(castleIndex);
	}

	private void OnEconomyClick(AuiButton sender)
	{
		popupEconomy.Show(castleIndex);
	}

	private void OnPoliticsClick(AuiButton sender)
	{
		popupPolitics.Show(PlayInfo.castleManager.castle[castleIndex]);
	}

	private void OnRedeployClick(AuiButton sender)
	{
		CastleInfo castleInfo = PlayInfo.castleManager.castle[castleIndex];
		if (castleInfo.isRedeploy)
		{
			popupRedeploy.Show(castleIndex, -1);
			return;
		}
		int num = PlayInfo.castleManager.castle.Length;
		int[] array = new int[num];
		int num2 = 0;
		foreach (CastleInfo item in castleInfo.nearCastle)
		{
			if (item.side == 0 && castleInfo != item && (!item.isRedeploy || item.redeployTargetIndex != castleIndex))
			{
				castles[item.index].SetRedeployTarget(castleIndex);
				array[num2] = item.index;
				num2++;
			}
		}
		int[] array2 = new int[num2];
		for (int i = 0; i < num2; i++)
		{
			array2[i] = array[i];
		}
		if (num2 > 0)
		{
			castles[castleIndex].SetRedeploySource(array2);
			Hide();
		}
		else
		{
			ProcBase.ShowMsg(StringContent.msgCannotRedeploy, MessageView.MsgIcon.alert);
		}
	}

	private void OnTrainingClick(AuiButton sender)
	{
		procCastle.uiTraining.Show();
	}

	private void OnAttackClick(AuiButton sender)
	{
		CastleInfo castleInfo = PlayInfo.castleManager.castle[castleIndex];
		if (PlayInfo.castleAI.attackCurrent.attackActive)
		{
			ProcBase.ShowMsg(StringContent.msgFirstNeedDefense.Replace(StringContent.strValue, PlayInfo.castleAI.attackCurrent.attackTo.castleName), MessageView.MsgIcon.military);
			return;
		}
		bool flag = false;
		foreach (CastleInfo item in castleInfo.nearCastle)
		{
			if (item.side == 0 && castleInfo != item)
			{
				castles[item.index].SetAttackTarget(castleIndex);
				flag = true;
			}
		}
		if (flag)
		{
			Hide();
		}
		else
		{
			ProcBase.ShowMsg(StringContent.msgCannotAttack, MessageView.MsgIcon.alert);
		}
	}

	private void OnSpyClick(AuiButton sender)
	{
		popupSpy.Show(castleIndex);
	}

	private void OnMercyClick(AuiButton sender)
	{
		popupMercy.Show(castleIndex);
	}

	public void Hide()
	{
		base.gameObject.SetActiveRecursive(false);
	}

	public void Show(int castleIndex)
	{
		base.gameObject.SetActiveRecursive(true);
		this.castleIndex = castleIndex;
		Refresh();
		for (int i = 0; i < PlayInfo.castleManager.castle.Length; i++)
		{
			castles[i].HideAttackTarget();
		}
		StopCoroutine("CoroutineRefreshCommandMessage");
		StartCoroutine("CoroutineRefreshCommandMessage");
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	public void Refresh()
	{
		CastleInfo castleInfo = PlayInfo.castleManager.castle[castleIndex];
		TextMesh[] array = castleCommandMessage;
		foreach (TextMesh textMesh in array)
		{
			textMesh.text = string.Empty;
		}
		castleLord.text = string.Empty;
		castleName.text = castleInfo.castleName;
		castleLevel.text = "Lv " + castleInfo.level;
		if (!castleInfo.activeSpy || castleInfo.side == 1)
		{
			for (int j = 0; j < 5; j++)
			{
				unitIconUnknown[j].visible = true;
				unitIconBlank[j].visible = false;
				unitIcon[j].visible = false;
			}
		}
		if (castleInfo.activeSpy || castleInfo.side == 0)
		{
			castlePopulation.text = castleInfo.population.ToString();
			int num = 0;
			for (int k = 0; k < 5; k++)
			{
				bool flag = castleInfo.unitCount[k] > 0;
				unitIconUnknown[k].visible = false;
				if (castleInfo.side == 0)
				{
					unitIconBlank[k].visible = !flag;
					if (!flag)
					{
						unitIconBlank[k].SetFrame(k);
					}
				}
				else
				{
					unitIconBlank[k].visible = false;
				}
				unitIcon[k].visible = flag;
				unitLevel[k].gameObject.SetActive(flag);
				unitCount[k].gameObject.SetActive(flag);
				if (flag)
				{
					if (castleInfo.side == 0)
					{
						unitIcon[k].SetFrame(k);
						unitLevel[k].text = PlayInfo.humanMilitary.GetUnitState(k).level.ToString();
					}
					else
					{
						unitIcon[k].SetFrame(5 + castleInfo.monsterCode[k] - 201);
						unitLevel[k].text = PlayInfo.monsterMilitary.GetUnitStateFromCode(castleInfo.monsterCode[k]).level.ToString();
					}
					unitCount[k].text = castleInfo.unitCount[k].ToString();
					num += castleInfo.unitCount[k];
				}
			}
			castleUnitTotal.text = num.ToString();
			castleDefense.text = castleInfo.GetCastleDefense().ToString();
			if (castleInfo.side == 0)
			{
				castleLord.text = ((castleInfo.lord != null) ? castleInfo.lord.name : string.Empty);
				if (castleInfo.lord != null)
				{
					ProcBase.ResetTextWidth(castleLord, 185f);
				}
				castleTax.text = castleInfo.GetTax().ToString();
				castleLoyalty.text = castleInfo.GetTotalLoyalty().ToString();
				castleFame.text = castleInfo.GetTotalLoadFame().ToString();
			}
			else
			{
				castleLord.text = string.Empty;
				castleFame.text = "-";
				castleTax.text = "-";
				castleLoyalty.text = "-";
			}
		}
		else
		{
			castleLoyalty.text = "??";
			castlePopulation.text = "??";
			for (int l = 0; l < 5; l++)
			{
				bool visible = false;
				unitIcon[l].visible = visible;
				unitLevel[l].gameObject.SetActive(visible);
				unitCount[l].gameObject.SetActive(visible);
			}
			castleUnitTotal.text = "??";
			castleDefense.text = "??";
			castleTax.text = "??";
			castleLoyalty.text = "??";
			castleFame.text = "??";
		}
		buttonArmy.visible = castleInfo.side == 0;
		buttonEconomy.visible = castleInfo.side == 0;
		buttonPolitics.visible = castleInfo.side == 0;
		buttonMove.visible = castleInfo.side == 0;
		buttonMercy.visible = castleInfo.side == 0;
		buttonTraining.visible = castleInfo.side == 0;
		buttonArmyLabel.visible = castleInfo.side == 0;
		buttonEconomyLabel.visible = castleInfo.side == 0;
		buttonPoliticsLabel.visible = castleInfo.side == 0;
		buttonMoveLabel.visible = castleInfo.side == 0;
		buttonMercyLabel.visible = castleInfo.side == 0;
		buttonTrainingLabel.visible = castleInfo.side == 0;
		Vector3 localPosition = buttonPolitics.transform.localPosition;
		localPosition.x -= 100f;
		buttonAttack.visible = castleInfo.side == 1;
		buttonAttack.transform.localPosition = localPosition;
		localPosition.z -= 1f;
		buttonAttackLabel.visible = castleInfo.side == 1;
		buttonAttackLabel.transform.localPosition = localPosition;
		localPosition = buttonPolitics.transform.localPosition;
		localPosition.x += 100f;
		buttonSpy.visible = castleInfo.side == 1;
		buttonSpy.transform.localPosition = localPosition;
		localPosition.z -= 1f;
		buttonSpyLabel.visible = castleInfo.side == 1;
		buttonSpyLabel.transform.localPosition = localPosition;
	}

	private IEnumerator CoroutineRefreshCommandMessage()
	{
		CastleInfo info = PlayInfo.castleManager.castle[castleIndex];
		bool bef_activeSpy = info.activeSpy;
		bool bef_isRecruitSoldier = info.isRecruitSoldier;
		bool bef_isUpgrading = info.isUpgrading;
		bool bef_isBuilding = false;
		int bef_day = PlayInfo.gameTime.day;
		CastleInfo.BuildingStatus[] buildingStatus = info.buildingStatus;
		foreach (CastleInfo.BuildingStatus status2 in buildingStatus)
		{
			if (status2 == CastleInfo.BuildingStatus.constructing)
			{
				bef_isBuilding = true;
				break;
			}
		}
		while (true)
		{
			string msg5 = string.Empty;
			TextMesh[] array = castleCommandMessage;
			foreach (TextMesh msgText in array)
			{
				msgText.text = string.Empty;
			}
			if (info.activeSpy)
			{
				int restDay4 = (int)(info.spyHour / 24f);
				int restHour4 = (int)(info.spyHour - (float)(restDay4 * 24));
				msg5 = StringContent.msgSpyActivate + " : " + PlayInfo.gameTime.GetDayString(restDay4, restHour4) + "\n";
				castleCommandMessage[0].text = msg5;
				bef_activeSpy = true;
			}
			if (info.isRecruitSoldier)
			{
				int restDay3 = (int)(info.recruitHour / 24f);
				int restHour3 = (int)(info.recruitHour - (float)(restDay3 * 24));
				msg5 = StringContent.msgRecruitingSoldier + " : " + PlayInfo.gameTime.GetDayString(restDay3, restHour3) + "\n";
				castleCommandMessage[0].text = msg5;
				bef_isRecruitSoldier = true;
			}
			if (info.isUpgrading)
			{
				int restDay2 = (int)(info.upgradeHour / 24f);
				int restHour2 = (int)(info.upgradeHour - (float)(restDay2 * 24));
				msg5 = StringContent.msgUpgradingCastle + " : " + PlayInfo.gameTime.GetDayString(restDay2, restHour2) + "\n";
				castleCommandMessage[1].text = msg5;
				bef_isUpgrading = true;
			}
			bool isBuilding = false;
			int buildIndex = 0;
			CastleInfo.BuildingStatus[] buildingStatus2 = info.buildingStatus;
			foreach (CastleInfo.BuildingStatus status in buildingStatus2)
			{
				if (status == CastleInfo.BuildingStatus.constructing)
				{
					isBuilding = true;
					break;
				}
				buildIndex++;
			}
			if (isBuilding)
			{
				float buildHour = info.constructBuildingHour[buildIndex];
				int restDay = (int)(buildHour / 24f);
				int restHour = (int)(buildHour - (float)(restDay * 24));
				msg5 = StringContent.msgBuildingBase.Replace(StringContent.strValue, StringContent.wordBuilding[buildIndex]) + " : " + PlayInfo.gameTime.GetDayString(restDay, restHour) + "\n";
				castleCommandMessage[2].text = msg5;
				bef_isUpgrading = true;
			}
			yield return new WaitForSeconds(0.2f);
			if (bef_activeSpy && !info.activeSpy)
			{
				bef_activeSpy = false;
				Refresh();
			}
			else if (bef_isRecruitSoldier && !info.isRecruitSoldier)
			{
				bef_isRecruitSoldier = false;
				Refresh();
			}
			else if (bef_isUpgrading && !info.isUpgrading)
			{
				bef_isUpgrading = false;
				Refresh();
			}
			else if (bef_isBuilding && !isBuilding)
			{
				bef_isBuilding = false;
				Refresh();
			}
			else if (bef_day != PlayInfo.gameTime.day)
			{
				bef_day = PlayInfo.gameTime.day;
				Refresh();
			}
		}
	}
}
