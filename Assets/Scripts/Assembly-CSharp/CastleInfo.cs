using System.Collections.Generic;
using UnityEngine;

public class CastleInfo
{
	public enum BuildingStatus
	{
		none = 0,
		constructing = 1,
		activate = 2
	}

	public class CastleLevelDefault
	{
		public int upgradeGold;

		public int upgradeInstantlyGem;

		public int upgradePeriod;

		public int maxUnit;

		public int maxPopulation;

		public int defense;

		public int battleUnit;

		public int buildingActivate;

		public int autoUpgradeDays;

		public int battleUnitMon;
	}

	public class BuildingAttribute
	{
		public string name = string.Empty;

		public int requireLevel;

		public int requireBuilding;

		public int incTax;

		public int incPopulation;

		public int constructPeriod;

		public int constructGold;

		public int constructInstantlyGem;

		public int genUnitCode;
	}

	public const int sideHuman = 0;

	public const int sideMonster = 1;

	public const int maxBattleUnit = 5;

	public const int maxCastleLevel = 3;

	public const int maxBuilding = 7;

	public const int buildingFarm = 0;

	public const int buildingBarracks = 1;

	public const int buildingMarket = 2;

	public const int buildingBlacksmith = 3;

	public const int buildingArmory = 4;

	public const int buildingStorage = 5;

	public const int buildingAcademy = 6;

	public static CastleLevelDefault[] levelDefault = new CastleLevelDefault[3];

	public static BuildingAttribute[] buildingAttribute = new BuildingAttribute[7];

	public static CastleInfo castleDefault;

	public int index;

	public string castleName = string.Empty;

	public int side;

	public int level = 1;

	public int groundId;

	public int[] unitCount = new int[5];

	public int population;

	public int loyalty;

	public int loyaltyDownDays = 10;

	public bool isUpgrading;

	public float upgradeHour;

	public int autoUpgradeDays;

	public float[] constructBuildingHour = new float[7];

	public BuildingStatus[] buildingStatus = new BuildingStatus[7];

	public float posx;

	public float posy;

	public int lordBaseFame;

	public LordManager.Lord lord;

	public int[] monsterCode = new int[5];

	public bool activeSpy;

	public float spyHour;

	public bool isRecruitSoldier;

	public float recruitHour;

	public int[] unitRecruit = new int[5];

	public bool isRedeploy;

	public float redeployHour;

	public int redeployTargetIndex;

	public CastleInfo redeployTarget;

	public int[] redeployUnitCount = new int[5];

	public float nextTaxHour;

	public int castleStartDay;

	public List<CastleInfo> nearCastle = new List<CastleInfo>();

	[System.Serializable]
	private class CastleLevelDefaultRow
	{
		public int upgradeGold;

		public int upgradeInstantlyGem;

		public int upgradePeriod;

		public int maxUnit;

		public int maxPopulation;

		public int defense;

		public int battleUnit;

		public int buildingActivate;

		public int autoUpgradeDays;

		public int battleUnitMon;
	}

	[System.Serializable]
	private class CastleLevelDefaultRowList
	{
		public CastleLevelDefaultRow[] items;
	}

	[System.Serializable]
	private class BuildingAttributeRow
	{
		public string name;

		public int requireLevel;

		public int requireBuilding;

		public int incTax;

		public int incPopulation;

		public int constructPeriod;

		public int constructGold;

		public int constructInstantlyGem;

		public int genUnitCode;
	}

	[System.Serializable]
	private class BuildingAttributeRowList
	{
		public BuildingAttributeRow[] items;
	}

	[System.Serializable]
	private class CastleDefaultRow
	{
		public int level;

		public int[] unitCount;

		public int population;

		public int loyalty;

		public int lordBaseFame;
	}

	public static void LoadDefault()
	{
		TextAsset textAsset = ResourceManager.Load("GameData", "castle_level", typeof(TextAsset)) as TextAsset;
		string json = "{\"items\":" + textAsset.text + "}";
		CastleLevelDefaultRowList castleLevelDefaultRowList = JsonUtility.FromJson<CastleLevelDefaultRowList>(json);
		int num = 0;
		CastleLevelDefaultRow[] items = castleLevelDefaultRowList.items;
		foreach (CastleLevelDefaultRow row in items)
		{
			CastleLevelDefault castleLevelDefault = new CastleLevelDefault();
			castleLevelDefault.upgradeGold = row.upgradeGold;
			castleLevelDefault.upgradeInstantlyGem = row.upgradeInstantlyGem;
			castleLevelDefault.upgradePeriod = row.upgradePeriod;
			castleLevelDefault.maxUnit = row.maxUnit;
			castleLevelDefault.maxPopulation = row.maxPopulation;
			castleLevelDefault.defense = row.defense;
			castleLevelDefault.battleUnit = row.battleUnit;
			castleLevelDefault.buildingActivate = row.buildingActivate;
			castleLevelDefault.autoUpgradeDays = row.autoUpgradeDays;
			castleLevelDefault.battleUnitMon = row.battleUnitMon;
			levelDefault[num] = castleLevelDefault;
			num++;
		}
		textAsset = ResourceManager.Load("GameData", "building_attribute", typeof(TextAsset)) as TextAsset;
		json = "{\"items\":" + textAsset.text + "}";
		BuildingAttributeRowList buildingAttributeRowList = JsonUtility.FromJson<BuildingAttributeRowList>(json);
		num = 0;
		BuildingAttributeRow[] items2 = buildingAttributeRowList.items;
		foreach (BuildingAttributeRow row2 in items2)
		{
			BuildingAttribute buildingAttribute = new BuildingAttribute();
			buildingAttribute.name = row2.name;
			buildingAttribute.requireLevel = row2.requireLevel;
			buildingAttribute.requireBuilding = row2.requireBuilding;
			buildingAttribute.incTax = row2.incTax;
			buildingAttribute.incPopulation = row2.incPopulation;
			buildingAttribute.constructPeriod = row2.constructPeriod;
			buildingAttribute.constructGold = row2.constructGold;
			buildingAttribute.constructInstantlyGem = row2.constructInstantlyGem;
			buildingAttribute.genUnitCode = row2.genUnitCode;
			CastleInfo.buildingAttribute[num] = buildingAttribute;
			num++;
		}
		textAsset = ResourceManager.Load("GameData", "castle_default", typeof(TextAsset)) as TextAsset;
		CastleDefaultRow castleDefaultRow = JsonUtility.FromJson<CastleDefaultRow>(textAsset.text);
		castleDefault = new CastleInfo();
		castleDefault.level = castleDefaultRow.level;
		for (int i = 0; i < 5; i++)
		{
			castleDefault.unitCount[i] = (castleDefaultRow.unitCount != null && castleDefaultRow.unitCount.Length > i) ? castleDefaultRow.unitCount[i] : 0;
		}
		castleDefault.population = castleDefaultRow.population;
		castleDefault.loyalty = castleDefaultRow.loyalty;
		castleDefault.lordBaseFame = castleDefaultRow.lordBaseFame;
	}

	public void Reset()
	{
		level = castleDefault.level;
		for (int i = 0; i < 5; i++)
		{
			unitCount[i] = castleDefault.unitCount[i];
		}
		population = castleDefault.population;
		loyalty = castleDefault.loyalty;
		lordBaseFame = castleDefault.lordBaseFame;
		loyaltyDownDays = PlayInfo.gameRule.loyaltyDownDays;
		upgradeHour = 0f;
		for (int j = 0; j < constructBuildingHour.Length; j++)
		{
			constructBuildingHour[j] = 0f;
		}
		for (int k = 0; k < buildingStatus.Length; k++)
		{
			buildingStatus[k] = BuildingStatus.none;
		}
		lord = null;
		activeSpy = false;
		isRedeploy = false;
		isUpgrading = false;
		isRecruitSoldier = false;
		nextTaxHour = PlayInfo.gameRule.taxDays * 24f;
	}

	public void SetAutoUpgrade()
	{
		if (side != 0 && level < 3)
		{
			int num = PlayInfo.castleManager.GetCastleCount(0) - 1;
			if (num < 0)
			{
				num = 0;
			}
			if (num >= PlayInfo.monsterBorn.Count)
			{
				num = PlayInfo.monsterBorn.Count - 1;
			}
			int num2 = (int)((float)(levelDefault[level].autoUpgradeDays * PlayInfo.monsterBorn[num].bornRate) / 100f);
			autoUpgradeDays = num2;
		}
	}

	public int GetTotalLoadFame()
	{
		float num = lordBaseFame;
		if (lord != null)
		{
			num = lord.fame;
		}
		return (int)num;
	}

	public int GetTotalLoyalty()
	{
		return (int)((float)loyalty + (float)GetTotalLoadFame() / 5f);
	}

	public int GetMaxRecruitSoldiers()
	{
		int num = 0;
		int[] array = unitCount;
		foreach (int num2 in array)
		{
			num += num2;
		}
		int num3 = levelDefault[level - 1].maxUnit - num;
		if (num3 <= 0)
		{
			return 0;
		}
		int num4 = GetTotalLoyalty() / 3;
		if (lord != null)
		{
			num4 += lord.fame;
		}
		if (num4 > num3)
		{
			num4 = num3;
		}
		return num4;
	}

	public int GetTax()
	{
		float num = (float)population * 3f;
		float num2 = 0f;
		int num3 = 0;
		BuildingStatus[] array = this.buildingStatus;
		foreach (BuildingStatus buildingStatus in array)
		{
			if (buildingStatus == BuildingStatus.activate)
			{
				num2 += (float)buildingAttribute[num3].incTax;
			}
		}
		float num4 = 0f;
		if (lord != null)
		{
			num4 = lord.inte * 30;
		}
		return (int)((num + num2 + num4) * ((float)GetTotalLoyalty() / 100f));
	}

	public int GetCastleDefense()
	{
		int num = levelDefault[level - 1].defense;
		if (lord != null)
		{
			num += lord.inte * 20;
		}
		return num;
	}

	public int GetCurrentUnitTotal()
	{
		int num = 0;
		int[] array = unitCount;
		foreach (int num2 in array)
		{
			num += num2;
		}
		return num;
	}

	public void ActiveSpy()
	{
		activeSpy = true;
		spyHour = PlayInfo.gameRule.spyActiveDays * 24f;
	}

	public void CheckTimeProcess()
	{
		if (side == 0)
		{
			if (isRecruitSoldier)
			{
				recruitHour -= 1f;
				if (recruitHour <= 0f)
				{
					FinishRecruit();
				}
			}
			for (int i = 0; i < 7; i++)
			{
				if (buildingStatus[i] == BuildingStatus.constructing)
				{
					constructBuildingHour[i] -= 1f;
					if (constructBuildingHour[i] < 0f)
					{
						FinishBuilding(i);
					}
				}
			}
			if (isUpgrading)
			{
				upgradeHour -= 1f;
				if (upgradeHour <= 0f)
				{
					FinishUpgrade();
				}
			}
			if (isRedeploy)
			{
				redeployHour -= 1f;
				if (redeployTarget.side != 0)
				{
					isRedeploy = false;
					for (int j = 0; j < 5; j++)
					{
						unitCount[j] += redeployUnitCount[j];
					}
				}
				else if (redeployHour <= 0f)
				{
					FinishRedeploy();
				}
			}
			nextTaxHour -= 1f;
			if (nextTaxHour < 0f)
			{
				CollectTax();
				nextTaxHour = PlayInfo.gameRule.taxDays * 24f;
			}
		}
		else if (activeSpy)
		{
			spyHour -= 1f;
			if (spyHour <= 0f)
			{
				activeSpy = false;
			}
		}
	}

	public void CheckDayProcess()
	{
		if (side == 1)
		{
			int num = PlayInfo.gameTime.day - castleStartDay;
			if (level < 3)
			{
				autoUpgradeDays--;
				if (autoUpgradeDays <= 0)
				{
					level++;
					SetAutoUpgrade();
				}
			}
			int num2 = 0;
			for (int i = 0; i < 5; i++)
			{
				if (unitCount[i] < 0)
				{
					unitCount[i] = 0;
				}
				num2 += unitCount[i];
			}
			if (num2 >= 300)
			{
				return;
			}
			int num3 = PlayInfo.castleManager.GetCastleCount(0) - 1;
			if (num3 < 0)
			{
				num3 = 0;
			}
			if (num3 >= PlayInfo.monsterBorn.Count)
			{
				num3 = PlayInfo.monsterBorn.Count - 1;
			}
			int num4 = 5;
			if (index == PlayInfo.castleManager.castle.Length - 1)
			{
				num4--;
			}
			for (int j = 0; j < num4; j++)
			{
				UnitState unitStateFromCode = PlayInfo.monsterMilitary.GetUnitStateFromCode(monsterCode[j]);
				if (unitStateFromCode == null || unitStateFromCode.autoBorn <= 0)
				{
					continue;
				}
				float num5 = (float)(unitStateFromCode.autoBorn * PlayInfo.monsterBorn[num3].bornRate) / 100f;
				int num6 = 1;
				int num7 = 1;
				if (num5 < 1f)
				{
					if (num5 < 0f)
					{
						num5 = 0.1f;
					}
					num6 = (int)(1f / num5);
					num7 = 1;
				}
				else
				{
					num7 = (int)num5;
				}
				if (num % num7 == 0)
				{
					num2 += num6;
					if (num2 > 300)
					{
						num6 -= num2 - 300;
					}
					unitCount[j] += num6;
				}
			}
			return;
		}
		loyaltyDownDays--;
		if (loyaltyDownDays <= 0)
		{
			loyaltyDownDays = PlayInfo.gameRule.loyaltyDownDays;
			loyalty -= PlayInfo.gameRule.loyaltyDown;
			if (loyalty < 0)
			{
				loyalty = 0;
			}
			if (loyalty <= PlayInfo.gameRule.loyaltyDownAlert)
			{
				PlayInfo.messageManager.Add(0, PlayMessage.MessagLevel.warning, StringContent.msgRequireCitizenLoyalty.Replace(StringContent.strValue, castleName));
			}
		}
	}

	public void SetRecruit(int[] cnts)
	{
		if (!isRecruitSoldier)
		{
			isRecruitSoldier = true;
			recruitHour = PlayInfo.gameRule.recruitSoldierDays * 24f;
			for (int i = 0; i < 5; i++)
			{
				unitRecruit[i] = cnts[i];
			}
		}
	}

	public void SetBuilding(int index)
	{
		if (buildingStatus[index] == BuildingStatus.none)
		{
			buildingStatus[index] = BuildingStatus.constructing;
			constructBuildingHour[index] = (float)buildingAttribute[index].constructPeriod * 24f;
		}
	}

	public void SetUpgrading()
	{
		if (level < 3)
		{
			isUpgrading = true;
			upgradeHour = (float)levelDefault[level].upgradePeriod * 24f;
		}
	}

	public void SetRedeploy(int[] cnts, int targetIdx, CastleInfo targetCastle)
	{
		if (isRedeploy)
		{
			return;
		}
		isRedeploy = true;
		redeployHour = PlayInfo.gameRule.redeploySoldierDays * 24f;
		for (int i = 0; i < 5; i++)
		{
			redeployUnitCount[i] = cnts[i];
			unitCount[i] -= cnts[i];
			if (unitCount[i] < 0)
			{
				redeployUnitCount[i] += unitCount[i];
				unitCount[i] = 0;
			}
		}
		redeployTargetIndex = targetIdx;
		redeployTarget = targetCastle;
	}

	private void CollectTax()
	{
		if (side == 0)
		{
			int num = (int)((float)GetTax() * (PlayInfo.gameRule.taxDays / 30f));
			PlayInfo.playerData.gold += num;
			string msgCollectTax = StringContent.msgCollectTax;
			msgCollectTax = msgCollectTax.Replace(StringContent.strValue, castleName);
			msgCollectTax = msgCollectTax.Replace(StringContent.strValue2, num.ToString());
			PlayInfo.messageManager.Add(1, PlayMessage.MessagLevel.normal, msgCollectTax);
		}
	}

	public void FinishRecruit()
	{
		if (isRecruitSoldier)
		{
			isRecruitSoldier = false;
			for (int i = 0; i < 5; i++)
			{
				unitCount[i] += unitRecruit[i];
			}
			string msgFinishRecruit = StringContent.msgFinishRecruit;
			msgFinishRecruit = msgFinishRecruit.Replace(StringContent.strValue, castleName);
			PlayInfo.messageManager.Add(4, PlayMessage.MessagLevel.normal, msgFinishRecruit);
		}
	}

	public void FinishBuilding(int index)
	{
		if (buildingStatus[index] != BuildingStatus.constructing)
		{
			return;
		}
		buildingStatus[index] = BuildingStatus.activate;
		if (buildingAttribute[index].incPopulation > 0)
		{
			population += buildingAttribute[index].incPopulation;
			if (population > levelDefault[level - 1].maxPopulation)
			{
				population = levelDefault[level - 1].maxPopulation;
			}
		}
		string msgFinishBuilding = StringContent.msgFinishBuilding;
		msgFinishBuilding = msgFinishBuilding.Replace(StringContent.strValue, StringContent.wordBuilding[index]);
		msgFinishBuilding = msgFinishBuilding.Replace(StringContent.strValue2, castleName);
		PlayInfo.messageManager.Add(0, PlayMessage.MessagLevel.normal, msgFinishBuilding);
	}

	public void FinishRedeploy()
	{
		if (isRedeploy)
		{
			isRedeploy = false;
			for (int i = 0; i < 5; i++)
			{
				redeployTarget.unitCount[i] += redeployUnitCount[i];
			}
			if (side == 0)
			{
				string msgFinishRedeploy = StringContent.msgFinishRedeploy;
				msgFinishRedeploy = msgFinishRedeploy.Replace(StringContent.strValue, castleName);
				msgFinishRedeploy = msgFinishRedeploy.Replace(StringContent.strValue2, redeployTarget.castleName);
				PlayInfo.messageManager.Add(3, PlayMessage.MessagLevel.normal, msgFinishRedeploy);
			}
		}
	}

	public void FinishUpgrade()
	{
		if (isUpgrading)
		{
			isUpgrading = false;
			if (level < 3)
			{
				level++;
			}
			string msgFinishCastleUpgrade = StringContent.msgFinishCastleUpgrade;
			msgFinishCastleUpgrade = msgFinishCastleUpgrade.Replace(StringContent.strValue, castleName);
			msgFinishCastleUpgrade = msgFinishCastleUpgrade.Replace(StringContent.strValue2, level.ToString());
			PlayInfo.messageManager.Add(8, PlayMessage.MessagLevel.normal, msgFinishCastleUpgrade);
		}
	}
}
