using System.Collections;
using UnityEngine;

public class MapEconomy : MonoBehaviour
{
	public AuiButton buttonClose;

	public TextMesh castleName;

	public GameObject objDetail;

	public TextMesh commandPts;

	public TextMesh buildDays;

	public TextMesh buildGold;

	public TextMesh buildGem;

	public TextMesh textBldgName;

	public TextMesh textRequires;

	public TextMesh textBenefit;

	public TextMesh labelRequires;

	public TextMesh labelBenefit;

	public AuiButton buttonBuild;

	public AuiSprite buttonBuildLabel;

	public AuiButton buttonInstantly;

	public GameObject objForConstruct;

	public GameObject objUnderConst;

	public TextMesh textUnderConst;

	public AuiSprite iconSelected;

	public AuiSpriteAnimation aniBuilding;

	public EconomyBuilding[] bldgs;

	public AuiButton[] buttonUpgrade;

	public UIMap uiMap;

	private int castleIndex;

	private int buildingIndex;

	private bool isConstructing;

	private void Start()
	{
		buttonClose.isTop = true;
		buttonBuild.isTop = true;
		buttonInstantly.isTop = true;
		buttonBuild.onButtonClick = OnBuildClick;
		buttonInstantly.onButtonClick = OnInstantlyClick;
		buttonClose.onButtonClick = OnCloseClick;
		AuiButton[] array = buttonUpgrade;
		foreach (AuiButton auiButton in array)
		{
			auiButton.isTop = true;
			auiButton.onButtonClick = OnMoveUpgradeClick;
		}
		for (int j = 0; j < 7; j++)
		{
			bldgs[j].bldgDetail.isTop = true;
			bldgs[j].bldgDetail.buttonTag = j;
			bldgs[j].bldgDetail.onButtonClick = OnBldgClick;
		}
		Vector3 position = buttonBuild.transform.position;
		position.z = objUnderConst.transform.position.z;
		objUnderConst.transform.position = position;
		labelRequires.text = StringContent.wordRequires;
		labelBenefit.text = StringContent.wordBenefit;
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	private bool CheckCanBuild()
	{
		int num = buildingIndex;
		CastleInfo castleInfo = PlayInfo.castleManager.castle[castleIndex];
		CastleInfo.BuildingAttribute buildingAttribute = CastleInfo.buildingAttribute[num];
		if (buildingAttribute.requireLevel > castleInfo.level)
		{
			ProcBase.ShowMsg(StringContent.msgRequireCastleLevel.Replace(StringContent.strValue, buildingAttribute.requireLevel.ToString()), MessageView.MsgIcon.alert);
			return false;
		}
		if (buildingAttribute.requireBuilding > -1 && castleInfo.buildingStatus[buildingAttribute.requireBuilding] != CastleInfo.BuildingStatus.activate)
		{
			ProcBase.ShowMsg(StringContent.msgRequireBuilding.Replace(StringContent.strValue, StringContent.wordBuilding[buildingAttribute.requireBuilding]), MessageView.MsgIcon.alert);
			return false;
		}
		if (PlayInfo.playerData.cmdPts < PlayInfo.gameRule.cmdPtsConstruct)
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

	private void OnMoveUpgradeClick(AuiButton sender)
	{
		Hide();
		uiMap.popupUpgrade.Show(castleIndex);
	}

	private void OnBuildClick(AuiButton sender)
	{
		int num = buildingIndex;
		CastleInfo castleInfo = PlayInfo.castleManager.castle[castleIndex];
		CastleInfo.BuildingAttribute buildingAttribute = CastleInfo.buildingAttribute[num];
		if (!CheckCanBuild())
		{
			return;
		}
		if (PlayInfo.playerData.gold < buildingAttribute.constructGold)
		{
			ShowGotoGoldShop();
			return;
		}
		PlayInfo.playerData.cmdPts -= PlayInfo.gameRule.cmdPtsConstruct;
		if (PlayInfo.playerData.cmdPts < 0)
		{
			PlayInfo.playerData.cmdPts = 0;
		}
		PlayInfo.playerData.gold -= buildingAttribute.constructGold;
		castleInfo.SetBuilding(num);
		Refresh();
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

	private void OnInstantlyClick(AuiButton sender)
	{
		int num = buildingIndex;
		CastleInfo castleInfo = PlayInfo.castleManager.castle[castleIndex];
		CastleInfo.BuildingAttribute buildingAttribute = CastleInfo.buildingAttribute[num];
		if (castleInfo.buildingStatus[num] != CastleInfo.BuildingStatus.constructing)
		{
			ProcBase.ShowMsg(StringContent.msgAlreadyConstruction, MessageView.MsgIcon.alert);
			ShowBuildingInfo(num);
		}
		else if (PlayInfo.playerData.gem < buildingAttribute.constructInstantlyGem)
		{
			ShowGotoGemShop();
		}
		else
		{
			PlayInfo.playerData.gem -= buildingAttribute.constructInstantlyGem;
			castleInfo.FinishBuilding(num);
			Refresh();
		}
	}

	private void OnCloseClick(AuiButton sender)
	{
		Hide();
	}

	private void OnBldgClick(AuiButton sender)
	{
		int buttonTag = sender.buttonTag;
		ShowBuildingInfo(buttonTag);
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
		AuiButton.topActive = false;
		StopCoroutine("RefreshBuildStatus");
	}

	public void Show(int castleIndex)
	{
		this.castleIndex = castleIndex;
		base.gameObject.SetActive(true);
		AuiButton.topActive = true;
		Refresh();
		StartCoroutine("RefreshBuildStatus");
	}

	private void Refresh()
	{
		CastleInfo castleInfo = PlayInfo.castleManager.castle[castleIndex];
		castleName.text = castleInfo.castleName;
		isConstructing = false;
		for (int i = 0; i < 7; i++)
		{
			if (castleInfo.buildingStatus[i] == CastleInfo.BuildingStatus.constructing)
			{
				isConstructing = true;
				Vector3 position = bldgs[i].bldgIcon.transform.position;
				position.z -= 1f;
				aniBuilding.visible = true;
				aniBuilding.transform.position = position;
				aniBuilding.StartAnimation(true, false);
			}
			bldgs[i].bldgIcon.SetFrame((castleInfo.buildingStatus[i] == CastleInfo.BuildingStatus.none) ? 1 : 0);
		}
		if (!isConstructing)
		{
			aniBuilding.StopAnimation(true);
		}
		ShowBuildingInfo(buildingIndex);
	}

	public void ShowBuildingInfo(int index)
	{
		buildingIndex = index;
		CastleInfo castleInfo = PlayInfo.castleManager.castle[castleIndex];
		CastleInfo.BuildingAttribute buildingAttribute = CastleInfo.buildingAttribute[index];
		iconSelected.transform.position = bldgs[index].bldgDetail.transform.position;
		if (castleInfo.buildingStatus[index] == CastleInfo.BuildingStatus.none)
		{
			objForConstruct.SetActive(true);
			objUnderConst.SetActive(false);
			commandPts.text = PlayInfo.gameRule.cmdPtsConstruct.ToString();
			int constructPeriod = CastleInfo.buildingAttribute[index].constructPeriod;
			buildDays.text = constructPeriod + " " + ((constructPeriod <= 1) ? StringContent.wordDay : StringContent.wordDays);
			buildGold.text = buildingAttribute.constructGold.ToString();
			buildGem.text = buildingAttribute.constructInstantlyGem.ToString();
			objForConstruct.SetActive(!isConstructing);
			objUnderConst.SetActive(false);
		}
		else if (castleInfo.buildingStatus[index] == CastleInfo.BuildingStatus.constructing)
		{
			objForConstruct.SetActive(false);
			objUnderConst.SetActive(true);
			int day = (int)castleInfo.constructBuildingHour[index] / 24;
			int hour = (int)castleInfo.constructBuildingHour[index] % 24;
			buildGem.text = buildingAttribute.constructInstantlyGem.ToString();
			textUnderConst.text = StringContent.msgUnderConstruction.Replace(StringContent.strValue, PlayInfo.gameTime.GetDayString(day, hour));
		}
		else
		{
			objForConstruct.SetActive(false);
			objUnderConst.SetActive(false);
		}
		if (castleInfo.buildingStatus[1] == CastleInfo.BuildingStatus.activate)
		{
			if (castleInfo.level == 1)
			{
				buttonUpgrade[0].enabled = true;
				buttonUpgrade[0].SetFrame(0);
			}
			else
			{
				buttonUpgrade[0].enabled = false;
				buttonUpgrade[0].SetFrame(3);
			}
		}
		else
		{
			buttonUpgrade[0].enabled = false;
			buttonUpgrade[0].SetFrame(2);
		}
		if (castleInfo.buildingStatus[3] == CastleInfo.BuildingStatus.activate)
		{
			if (castleInfo.level == 2)
			{
				buttonUpgrade[1].enabled = true;
				buttonUpgrade[1].SetFrame(0);
			}
			else
			{
				buttonUpgrade[1].enabled = false;
				buttonUpgrade[1].SetFrame(3);
			}
		}
		else
		{
			buttonUpgrade[1].enabled = false;
			buttonUpgrade[1].SetFrame(2);
		}
		textBldgName.text = StringContent.wordBuilding[buildingIndex];
		textRequires.text = StringContent.wordCastleLevel + buildingAttribute.requireLevel + "\n";
		if (buildingAttribute.requireBuilding > -1)
		{
			textRequires.text += StringContent.wordBuilding[buildingAttribute.requireBuilding];
		}
		if (buildingAttribute.genUnitCode != 0)
		{
			textBenefit.text = StringContent.wordRecruit + "  " + StringContent.wordSoldier[PlayInfo.humanMilitary.GetUnitIndexFromCode(buildingAttribute.genUnitCode)];
			return;
		}
		textBenefit.text = StringContent.wordTax + "  +" + buildingAttribute.incTax + "\n" + StringContent.wordResidents + "  +" + buildingAttribute.incPopulation;
	}

	private IEnumerator RefreshBuildStatus()
	{
		while (true)
		{
			if (isConstructing)
			{
				Refresh();
			}
			yield return new WaitForSeconds(0.5f);
		}
	}
}
