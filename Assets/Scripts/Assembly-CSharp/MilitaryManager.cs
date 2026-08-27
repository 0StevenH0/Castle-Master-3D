using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MilitaryManager
{
	public class UnitLevelCost
	{
		public int costGold;
	}

	public const int maxUnitLevel = 50;

	public const int codeBossMonster = 217;

	private List<UnitState> unitStates = new List<UnitState>();

	private UnitLevelCost[,] unitLevelCost;

	[System.Serializable]
	private class UnitDefaultEntry
	{
		public string side;
		public int code;
		public string name;
		public int basePrice;
		public int autoBorn;
		public int autoUptime;
		public int rankAttack;
		public int rankHp;
		public float baseHp;
		public float attack;
		public float defense;
		public float speed;
		public int requiredBldg;
		public bool splashAttack;
		public int splashRange;
		public float splashLength;
		public int upgradeDays;
		public int upgradeInstantlyGem;
	}

	[System.Serializable]
	private class UnitDefaultList
	{
		public UnitDefaultEntry[] items;
	}

	public void LoadDefault(int side)
	{
		TextAsset textAsset = ResourceManager.Load("GameData", "unit_default", typeof(TextAsset)) as TextAsset;
		string json = "{\"items\":" + textAsset.text + "}";
		UnitDefaultList unitDefaultList = JsonUtility.FromJson<UnitDefaultList>(json);
		UnitDefaultEntry[] items = unitDefaultList.items;
		foreach (UnitDefaultEntry unitDefaultEntry in items)
		{
			int num = ((!unitDefaultEntry.side.Equals("human")) ? 1 : 0);
			if (num == side)
			{
				UnitState unitState = new UnitState();
				unitState.side = num;
				unitState.code = unitDefaultEntry.code;
				unitState.name = unitDefaultEntry.name;
				unitState.basePrice = unitDefaultEntry.basePrice;
				unitState.autoBorn = unitDefaultEntry.autoBorn;
				unitState.autoUptime = unitDefaultEntry.autoUptime;
				unitState.rankAttack = unitDefaultEntry.rankAttack;
				unitState.rankHp = unitDefaultEntry.rankHp;
				unitState.baseHp = unitDefaultEntry.baseHp;
				unitState.attack = unitDefaultEntry.attack;
				unitState.defense = unitDefaultEntry.defense;
				unitState.speed = unitDefaultEntry.speed;
				unitState.requiredBldg = unitDefaultEntry.requiredBldg;
				unitState.splashAttack = unitDefaultEntry.splashAttack;
				unitState.splashRange = unitDefaultEntry.splashRange;
				unitState.splashLength = unitDefaultEntry.splashLength;
				unitState.upgradeDays = unitDefaultEntry.upgradeDays;
				unitState.upgradeInstantlyGem = unitDefaultEntry.upgradeInstantlyGem;
				unitState.curHp = unitState.sumHp;
				unitStates.Add(unitState);
			}
		}
		LoadUnitLevelCost();
	}

	[System.Serializable]
	private class UnitLevelTableEntry
	{
		public int level;
		public int[] costGold;
	}

	[System.Serializable]
	private class UnitLevelTableList
	{
		public UnitLevelTableEntry[] items;
	}

	private void LoadUnitLevelCost()
	{
		unitLevelCost = new UnitLevelCost[5, 50];
		TextAsset textAsset = ResourceManager.Load("GameData", "unit_leveltable", typeof(TextAsset)) as TextAsset;
		string json = "{\"items\":" + textAsset.text + "}";
		UnitLevelTableList unitLevelTableList = JsonUtility.FromJson<UnitLevelTableList>(json);
		UnitLevelTableEntry[] items = unitLevelTableList.items;
		int num = 0;
		foreach (UnitLevelTableEntry unitLevelTableEntry in items)
		{
			for (int i = 0; i < 5; i++)
			{
				unitLevelCost[i, num] = new UnitLevelCost();
				unitLevelCost[i, num].costGold = unitLevelTableEntry.costGold[i];
			}
			num++;
			if (num >= 50)
			{
				break;
			}
		}
	}

	public void GetUnitLevelCost(int unitIndex, int level, ref int costGold)
	{
		if (level >= 50)
		{
			costGold = 0;
		}
		else
		{
			costGold = unitLevelCost[unitIndex, level].costGold;
		}
	}

	public UnitState GetUnitState(int idx)
	{
		return unitStates[idx];
	}

	public UnitState GetUnitStateFromCode(int code)
	{
		foreach (UnitState unitState in unitStates)
		{
			if (unitState.code == code)
			{
				return unitState;
			}
		}
		return null;
	}

	public int GetUnitIndexFromCode(int code)
	{
		int num = 0;
		foreach (UnitState unitState in unitStates)
		{
			if (unitState.code == code)
			{
				return num;
			}
			num++;
		}
		return 0;
	}

	public void CheckDayProcess()
	{
		if (PlayInfo.gameTime.day <= 0)
		{
			return;
		}
		int num = 0;
		int num2 = PlayInfo.castleManager.GetCastleCount(0);
		if (num2 > PlayInfo.monsterBorn.Count - 1)
		{
			num2 = PlayInfo.monsterBorn.Count - 1;
		}
		foreach (UnitState unitState in unitStates)
		{
			if (unitState.side == 1 && unitState != null && unitState.level < 50 && unitState.autoUptime > 0)
			{
				float num3 = (float)(unitState.autoUptime * PlayInfo.monsterBorn[num2].bornRate) / 100f;
				if (num3 < 1f)
				{
					num3 = 1f;
				}
				if (PlayInfo.gameTime.day % (int)num3 == 0)
				{
					unitState.level++;
				}
			}
			num++;
		}
	}

	public void Save()
	{
		DataRegistry.KeyData parent = DataRegistry.Set(null, "UnitState" + unitStates[0].side, unitStates.Count);
		int num = 0;
		foreach (UnitState unitState in unitStates)
		{
			DataRegistry.KeyData parent2 = DataRegistry.Set(parent, num.ToString(), string.Empty);
			DataRegistry.Set(parent2, "level", unitState.level);
			DataRegistry.Set(parent2, "fame", unitState.fame);
			DataRegistry.Set(parent2, "loyalty", unitState.loyalty);
			num++;
		}
	}

	public void Load()
	{
		int keyvalue = 0;
		DataRegistry.KeyData current = null;
		if (!DataRegistry.Get(null, "UnitState" + unitStates[0].side, ref keyvalue, ref current))
		{
			return;
		}
		if (keyvalue > unitStates.Count)
		{
			keyvalue = unitStates.Count;
		}
		for (int i = 0; i < keyvalue; i++)
		{
			DataRegistry.KeyData current2 = null;
			string keyvalue2 = string.Empty;
			if (DataRegistry.Get(current, i.ToString(), ref keyvalue2, ref current2))
			{
				DataRegistry.KeyData current3 = null;
				DataRegistry.Get(current2, "level", ref unitStates[i].level, ref current3);
				DataRegistry.Get(current2, "fame", ref unitStates[i].fame, ref current3);
				DataRegistry.Get(current2, "loyalty", ref unitStates[i].loyalty, ref current3);
			}
		}
	}
}
