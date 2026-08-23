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

	public void LoadDefault(int side)
	{
		TextAsset textAsset = ResourceManager.Load("GameData", "unit_default", typeof(TextAsset)) as TextAsset;
		bool succeed = false;
		string s = DataSecurity.Decrypt(textAsset.text, "surkwjch", out succeed);
		if (!succeed)
		{
			Debug.LogError("Decrypt Error!!");
			return;
		}
		StringReader stringReader = new StringReader(s);
		string text;
		while ((text = stringReader.ReadLine()) != null)
		{
			if (text.Trim().Length == 0)
			{
				continue;
			}
			char[] separator = new char[1] { '\t' };
			string[] array = text.Split(separator);
			if (array.Length > 1)
			{
				int num = ((!array[0].Equals("human")) ? 1 : 0);
				if (num == side)
				{
					UnitState unitState = new UnitState();
					unitState.side = num;
					unitState.code = int.Parse(array[1]);
					unitState.name = array[2];
					unitState.basePrice = int.Parse(array[3]);
					unitState.autoBorn = int.Parse(array[4]);
					unitState.autoUptime = int.Parse(array[5]);
					unitState.rankAttack = int.Parse(array[6]);
					unitState.rankHp = int.Parse(array[7]);
					unitState.baseHp = float.Parse(array[8]);
					unitState.attack = float.Parse(array[9]);
					unitState.defense = float.Parse(array[10]);
					unitState.speed = float.Parse(array[11]);
					unitState.requiredBldg = int.Parse(array[12]);
					unitState.splashAttack = int.Parse(array[13]) == 1;
					unitState.splashRange = int.Parse(array[14]);
					unitState.splashLength = float.Parse(array[15]);
					unitState.upgradeDays = int.Parse(array[16]);
					unitState.upgradeInstantlyGem = int.Parse(array[17]);
					unitState.curHp = unitState.sumHp;
					unitStates.Add(unitState);
				}
			}
		}
		LoadUnitLevelCost();
	}

	private void LoadUnitLevelCost()
	{
		unitLevelCost = new UnitLevelCost[5, 50];
		TextAsset textAsset = ResourceManager.Load("GameData", "unit_leveltable", typeof(TextAsset)) as TextAsset;
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
			if (text.Trim().Length == 0)
			{
				continue;
			}
			char[] separator = new char[1] { '\t' };
			string[] array = text.Split(separator);
			if (array.Length > 1)
			{
				for (int i = 0; i < 5; i++)
				{
					unitLevelCost[i, num] = new UnitLevelCost();
					unitLevelCost[i, num].costGold = int.Parse(array[1 + i]);
				}
				num++;
				if (num >= 50)
				{
					break;
				}
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
