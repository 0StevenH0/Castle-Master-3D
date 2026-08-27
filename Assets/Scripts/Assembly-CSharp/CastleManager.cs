using System.IO;
using UnityEngine;

public class CastleManager
{
	private const int maxCastle = 45;

	public const int nearLen = 500;

	public CastleInfo[] castle = new CastleInfo[45];

	[System.Serializable]
	private class CastleInitialRow
	{
		public int id;
		public string castleName;
		public string side;
		public int level;
		public float posx;
		public float posy;
		public int[] monsterCode;
		public int[] unitCount;
		public int groundId;
	}

	[System.Serializable]
	private class CastleInitialRowList
	{
		public CastleInitialRow[] items;
	}

	public void LoadDefault()
	{
		TextAsset textAsset = ResourceManager.Load("GameData", "castle_initial", typeof(TextAsset)) as TextAsset;
		string json = "{\"items\":" + textAsset.text + "}";
		CastleInitialRowList castleInitialRowList = JsonUtility.FromJson<CastleInitialRowList>(json);
		int num = 0;
		CastleInitialRow[] items = castleInitialRowList.items;
		foreach (CastleInitialRow row in items)
		{
			CastleInfo castleInfo = new CastleInfo();
			castleInfo.Reset();
			castleInfo.castleName = row.castleName.Trim();
			castleInfo.side = ((row.side.Trim() == "monster") ? 1 : 0);
			castleInfo.level = row.level;
			castleInfo.posx = row.posx;
			castleInfo.posy = row.posy;
			for (int i = 0; i < 5; i++)
			{
				castleInfo.monsterCode[i] = row.monsterCode[i];
			}
			for (int j = 0; j < 5; j++)
			{
				castleInfo.unitCount[j] = row.unitCount[j];
			}
			castleInfo.groundId = row.groundId;
			castleInfo.index = num;
			castle[num] = castleInfo;
			num++;
		}
		CastleInfo[] array2 = castle;
		foreach (CastleInfo castleInfo2 in array2)
		{
			Vector2 a = new Vector2(castleInfo2.posx, castleInfo2.posy);
			CastleInfo[] array3 = castle;
			foreach (CastleInfo castleInfo3 in array3)
			{
				Vector2 b = new Vector2(castleInfo3.posx, castleInfo3.posy);
				if (Vector2.Distance(a, b) < 500f)
				{
					castleInfo2.nearCastle.Add(castleInfo3);
				}
			}
		}
	}

	public int GetCastleCount(int side)
	{
		int num = 0;
		CastleInfo[] array = castle;
		foreach (CastleInfo castleInfo in array)
		{
			if (castleInfo.side == side)
			{
				num++;
			}
		}
		return num;
	}

	public void Save()
	{
		DataRegistry.KeyData parent = DataRegistry.Set(null, "CastleInfo", string.Empty);
		for (int i = 0; i < 45; i++)
		{
			DataRegistry.KeyData parent2 = DataRegistry.Set(parent, i.ToString(), string.Empty);
			CastleInfo castleInfo = castle[i];
			DataRegistry.Set(parent2, "side", castleInfo.side);
			DataRegistry.Set(parent2, "level", castleInfo.level);
			DataRegistry.Set(parent2, "unitCount", castleInfo.unitCount);
			DataRegistry.Set(parent2, "population", castleInfo.population);
			DataRegistry.Set(parent2, "loyalty", castleInfo.loyalty);
			DataRegistry.Set(parent2, "loyaltyDownDays", castleInfo.loyaltyDownDays);
			DataRegistry.Set(parent2, "isUpgrading", castleInfo.isUpgrading);
			DataRegistry.Set(parent2, "upgradeHour", castleInfo.upgradeHour);
			DataRegistry.Set(parent2, "autoUpgradeDays", castleInfo.autoUpgradeDays);
			DataRegistry.Set(parent2, "constructBuildingHour", castleInfo.constructBuildingHour);
			int[] array = new int[castleInfo.buildingStatus.Length];
			for (int j = 0; j < castleInfo.buildingStatus.Length; j++)
			{
				array[j] = (int)castleInfo.buildingStatus[j];
			}
			DataRegistry.Set(parent2, "buildingStatus", array);
			DataRegistry.Set(parent2, "lordBaseFame", castleInfo.lordBaseFame);
			DataRegistry.Set(parent2, "monsterCode", castleInfo.monsterCode);
			DataRegistry.Set(parent2, "activeSpy", castleInfo.activeSpy);
			DataRegistry.Set(parent2, "spyHour", castleInfo.spyHour);
			DataRegistry.Set(parent2, "isRecruitSoldier", castleInfo.isRecruitSoldier);
			DataRegistry.Set(parent2, "recruitHour", castleInfo.recruitHour);
			DataRegistry.Set(parent2, "unitRecruit", castleInfo.unitRecruit);
			DataRegistry.Set(parent2, "isRedeploy", castleInfo.isRedeploy);
			DataRegistry.Set(parent2, "redeployHour", castleInfo.redeployHour);
			DataRegistry.Set(parent2, "redeployTargetIndex", castleInfo.redeployTargetIndex);
			DataRegistry.Set(parent2, "redeployUnitCount", castleInfo.redeployUnitCount);
			DataRegistry.Set(parent2, "nextTaxHour", castleInfo.nextTaxHour);
			DataRegistry.Set(parent2, "castleStartDay", castleInfo.castleStartDay);
		}
	}

	public void Load()
	{
		string keyvalue = string.Empty;
		DataRegistry.KeyData current = null;
		if (!DataRegistry.Get(null, "CastleInfo", ref keyvalue, ref current))
		{
			return;
		}
		for (int i = 0; i < 45; i++)
		{
			DataRegistry.KeyData current2 = null;
			if (DataRegistry.Get(current, i.ToString(), ref keyvalue, ref current2))
			{
				CastleInfo castleInfo = castle[i];
				DataRegistry.KeyData current3 = null;
				DataRegistry.Get(current2, "side", ref castleInfo.side, ref current3);
				DataRegistry.Get(current2, "level", ref castleInfo.level, ref current3);
				DataRegistry.Get(current2, "unitCount", ref castleInfo.unitCount, ref current3);
				DataRegistry.Get(current2, "population", ref castleInfo.population, ref current3);
				DataRegistry.Get(current2, "loyalty", ref castleInfo.loyalty, ref current3);
				DataRegistry.Get(current2, "loyaltyDownDays", ref castleInfo.loyaltyDownDays, ref current3);
				DataRegistry.Get(current2, "isUpgrading", ref castleInfo.isUpgrading, ref current3);
				DataRegistry.Get(current2, "upgradeHour", ref castleInfo.upgradeHour, ref current3);
				DataRegistry.Get(current2, "autoUpgradeDays", ref castleInfo.autoUpgradeDays, ref current3);
				DataRegistry.Get(current2, "constructBuildingHour", ref castleInfo.constructBuildingHour, ref current3);
				int[] keyvalue2 = new int[castleInfo.buildingStatus.Length];
				for (int j = 0; j < keyvalue2.Length; j++)
				{
					keyvalue2[j] = 0;
				}
				DataRegistry.Get(current2, "buildingStatus", ref keyvalue2, ref current3);
				for (int k = 0; k < castleInfo.buildingStatus.Length; k++)
				{
					castleInfo.buildingStatus[k] = (CastleInfo.BuildingStatus)keyvalue2[k];
				}
				DataRegistry.Get(current2, "lordBaseFame", ref castleInfo.lordBaseFame, ref current3);
				DataRegistry.Get(current2, "monsterCode", ref castleInfo.monsterCode, ref current3);
				DataRegistry.Get(current2, "activeSpy", ref castleInfo.activeSpy, ref current3);
				DataRegistry.Get(current2, "spyHour", ref castleInfo.spyHour, ref current3);
				DataRegistry.Get(current2, "isRecruitSoldier", ref castleInfo.isRecruitSoldier, ref current3);
				DataRegistry.Get(current2, "recruitHour", ref castleInfo.recruitHour, ref current3);
				DataRegistry.Get(current2, "unitRecruit", ref castleInfo.unitRecruit, ref current3);
				DataRegistry.Get(current2, "isRedeploy", ref castleInfo.isRedeploy, ref current3);
				DataRegistry.Get(current2, "redeployHour", ref castleInfo.redeployHour, ref current3);
				DataRegistry.Get(current2, "redeployTargetIndex", ref castleInfo.redeployTargetIndex, ref current3);
				DataRegistry.Get(current2, "redeployUnitCount", ref castleInfo.redeployUnitCount, ref current3);
				DataRegistry.Get(current2, "nextTaxHour", ref castleInfo.nextTaxHour, ref current3);
				DataRegistry.Get(current2, "castleStartDay", ref castleInfo.castleStartDay, ref current3);
				if (castleInfo.isRedeploy && castleInfo.redeployTargetIndex > -1)
				{
					castleInfo.redeployTarget = castle[castleInfo.redeployTargetIndex];
				}
			}
		}
	}
}
