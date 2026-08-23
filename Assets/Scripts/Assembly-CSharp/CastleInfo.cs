using System.Collections.Generic;
using System.IO;
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

	public static void LoadDefault()
	{
		TextAsset textAsset = ResourceManager.Load("GameData", "castle_level", typeof(TextAsset)) as TextAsset;
		bool succeed = false;
		string s = DataSecurity.Decrypt(textAsset.text, "surkwjch", out succeed);
		if (!succeed)
		{
			Debug.LogError("Decrypt Error!!");
			return;
		}
		StringReader stringReader = new StringReader(s);
		int num = 0;
		string text;
		while ((text = stringReader.ReadLine()) != null)
		{
			if (text.Trim().Length != 0)
			{
				char[] separator = new char[1] { '\t' };
				string[] array = text.Split(separator);
				if (array.Length > 1)
				{
					CastleLevelDefault castleLevelDefault = new CastleLevelDefault();
					castleLevelDefault.upgradeGold = int.Parse(array[1]);
					castleLevelDefault.upgradeInstantlyGem = int.Parse(array[2]);
					castleLevelDefault.upgradePeriod = int.Parse(array[3]);
					castleLevelDefault.maxUnit = int.Parse(array[4]);
					castleLevelDefault.maxPopulation = int.Parse(array[5]);
					castleLevelDefault.defense = int.Parse(array[6]);
					castleLevelDefault.battleUnit = int.Parse(array[7]);
					castleLevelDefault.buildingActivate = int.Parse(array[8]);
					castleLevelDefault.autoUpgradeDays = int.Parse(array[9]);
					castleLevelDefault.battleUnitMon = int.Parse(array[10]);
					levelDefault[num] = castleLevelDefault;
					num++;
				}
			}
		}
		textAsset = ResourceManager.Load("GameData", "building_attribute", typeof(TextAsset)) as TextAsset;
		succeed = false;
		s = DataSecurity.Decrypt(textAsset.text, "surkwjch", out succeed);
		if (!succeed)
		{
			Debug.LogError("Decrypt Error!!");
			return;
		}
		stringReader = new StringReader(s);
		num = 0;
		while ((text = stringReader.ReadLine()) != null)
		{
			if (text.Trim().Length == 0)
			{
				continue;
			}
			char[] separator2 = new char[1] { '\t' };
			string[] array2 = text.Split(separator2);
			if (array2.Length > 1)
			{
				BuildingAttribute buildingAttribute = new BuildingAttribute();
				buildingAttribute.name = array2[1];
				buildingAttribute.requireLevel = int.Parse(array2[2]);
				if (array2[3].Equals(string.Empty))
				{
					buildingAttribute.requireBuilding = -1;
				}
				else
				{
					buildingAttribute.requireBuilding = int.Parse(array2[3]);
				}
				buildingAttribute.incTax = int.Parse(array2[4]);
				buildingAttribute.incPopulation = int.Parse(array2[5]);
				buildingAttribute.constructPeriod = int.Parse(array2[6]);
				buildingAttribute.constructGold = int.Parse(array2[7]);
				buildingAttribute.constructInstantlyGem = int.Parse(array2[8]);
				buildingAttribute.genUnitCode = int.Parse(array2[9]);
				CastleInfo.buildingAttribute[num] = buildingAttribute;
				num++;
			}
		}
		textAsset = ResourceManager.Load("GameData", "castle_default", typeof(TextAsset)) as TextAsset;
		succeed = false;
		s = DataSecurity.Decrypt(textAsset.text, "surkwjch", out succeed);
		if (!succeed)
		{
			Debug.LogError("Decrypt Error!!");
			return;
		}
		stringReader = new StringReader(s);
		castleDefault = new CastleInfo();
		while ((text = stringReader.ReadLine()) != null)
		{
			if (text.Trim().Length == 0)
			{
				continue;
			}
			char[] separator3 = new char[1] { '\t' };
			string[] array3 = text.Split(separator3);
			if (array3.Length <= 1)
			{
				continue;
			}
			string text2 = array3[0].ToLower().Trim();
			string s2 = array3[1];
			if (text2.Equals("level"))
			{
				castleDefault.level = int.Parse(s2);
			}
			else if (text2.Equals("units"))
			{
				for (int i = 0; i < 5; i++)
				{
					if (array3.Length > i + 1)
					{
						castleDefault.unitCount[i] = int.Parse(array3[i + 1]);
					}
					else
					{
						castleDefault.unitCount[i] = 0;
					}
				}
			}
			else if (text2.Equals("population"))
			{
				castleDefault.population = int.Parse(s2);
			}
			else if (text2.Equals("loyalty"))
			{
				castleDefault.loyalty = int.Parse(s2);
			}
			else if (text2.Equals("lordfame"))
			{
				castleDefault.lordBaseFame = int.Parse(s2);
			}
		}
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
