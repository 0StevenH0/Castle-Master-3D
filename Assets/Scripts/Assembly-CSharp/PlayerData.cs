using System;
using System.IO;
using UnityEngine;

public class PlayerData
{
	public int gem;

	public int gold;

	public int cmdPts;

	public int maxCmdPts;

	public UnitItem[] equipWeapon;

	public UnitItem[] wearCloth;

	public UnitItem slotHp;

	public UnitItem slotMp;

	public UnitItem slotRing;

	public HeroSkill skill = new HeroSkill();

	public int[][] equipSkill;

	public string heroName = string.Empty;

	public bool tutorialMode = true;

	public int reviewIndex;

	public bool startLoveGame;

	public int countHeart;

	public float loveProgress;

	private DateTime timeBefCmdPtsInc = DateTime.UtcNow;

	public void LoadDefault()
	{
		equipWeapon = new UnitItem[3];
		wearCloth = new UnitItem[3];
		equipSkill = new int[3][];
		for (int i = 0; i < 3; i++)
		{
			equipSkill[i] = new int[3];
			for (int j = 0; j < 3; j++)
			{
				equipSkill[i][j] = -1;
			}
		}
		HeroSkill.LoadDefault();
		heroName = string.Empty;
		tutorialMode = true;
		reviewIndex = 0;
		TextAsset textAsset = ResourceManager.Load("GameData", "player_data", typeof(TextAsset)) as TextAsset;
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
			if (array.Length <= 1)
			{
				continue;
			}
			string text2 = array[0].ToLower().Trim();
			string s2 = array[1];
			if (text2.Equals("gem"))
			{
				gem = int.Parse(s2);
			}
			else if (text2.Equals("gold"))
			{
				gold = int.Parse(s2);
			}
			else if (text2.Equals("commandpts"))
			{
				cmdPts = int.Parse(s2);
				maxCmdPts = cmdPts;
			}
			else if (text2.Equals("weapon"))
			{
				UnitItem unitItem = PlayInfo.inventory.FindItem(int.Parse(s2));
				if (unitItem != null)
				{
					equipWeapon[(int)(unitItem.weaponType - 1)] = unitItem;
				}
			}
			else if (text2.Equals("cloth"))
			{
				UnitItem unitItem2 = PlayInfo.inventory.FindItem(int.Parse(s2));
				if (unitItem2 != null)
				{
					wearCloth[(int)unitItem2.clothPart] = unitItem2;
				}
			}
			else if (text2.Equals("itemhp"))
			{
				UnitItem unitItem3 = PlayInfo.inventory.FindItem(int.Parse(s2));
				if (unitItem3 != null)
				{
					slotHp = unitItem3;
				}
			}
			else if (text2.Equals("itemmp"))
			{
				UnitItem unitItem4 = PlayInfo.inventory.FindItem(int.Parse(s2));
				if (unitItem4 != null)
				{
					slotMp = unitItem4;
				}
			}
			else
			{
				if (!text2.Equals("defaultskill"))
				{
					continue;
				}
				int num = int.Parse(s2);
				int num2 = num / 5;
				skill.skillLevel[num] = 1;
				for (int k = 0; k < 3; k++)
				{
					if (equipSkill[num2][k] == -1)
					{
						equipSkill[num2][k] = num;
						break;
					}
				}
			}
		}
		int addCmdPts = 0;
		int addGem = 0;
		int addGold = 0;
		if (LoadPlusInfo(ref addCmdPts, ref addGem, ref addGold))
		{
			cmdPts += addCmdPts;
			maxCmdPts += addCmdPts;
			gem += addGem;
			gold += addGold;
		}
	}

	public bool LoadPlusInfo(ref int addCmdPts, ref int addGem, ref int addGold)
	{
		if (!PlusType.isPlus)
		{
			return false;
		}
		TextAsset textAsset = ResourceManager.Load("GameData", "plus_info", typeof(TextAsset)) as TextAsset;
		bool succeed = false;
		string s = DataSecurity.Decrypt(textAsset.text, "surkwjch", out succeed);
		if (!succeed)
		{
			Debug.LogError("Decrypt Error!!");
			return false;
		}
		StringReader stringReader = new StringReader(s);
		string text;
		while ((text = stringReader.ReadLine()) != null)
		{
			if (text.Trim().Length != 0)
			{
				char[] separator = new char[1] { '\t' };
				string[] array = text.Split(separator);
				if (array.Length > 1)
				{
					addCmdPts = int.Parse(array[0]);
					addGem = int.Parse(array[1]);
					addGold = int.Parse(array[2]);
					return true;
				}
			}
		}
		return false;
	}

	public int GetCommandPointInc()
	{
		TimeSpan timeSpan = DateTime.UtcNow.Subtract(timeBefCmdPtsInc);
		int autoIncCmdPts = PlayInfo.gameRule.autoIncCmdPts;
		int num = (int)timeSpan.TotalMinutes / autoIncCmdPts;
		if (num < 5)
		{
			return 0;
		}
		if (num > 0)
		{
			int num2 = cmdPts + num;
			if (num2 > maxCmdPts)
			{
				return maxCmdPts - cmdPts;
			}
			return num;
		}
		return 0;
	}

	public int CheckCommandPointInc()
	{
		DateTime utcNow = AppUtcTime.utcNow;
		TimeSpan timeSpan = utcNow.Subtract(timeBefCmdPtsInc);
		int autoIncCmdPts = PlayInfo.gameRule.autoIncCmdPts;
		int num = (int)timeSpan.TotalMinutes / autoIncCmdPts;
		if (num > 0)
		{
			int num2 = num;
			int num3 = cmdPts + num;
			if (num3 > maxCmdPts)
			{
				num2 = maxCmdPts - cmdPts;
			}
			cmdPts += num2;
			timeBefCmdPtsInc = utcNow;
			Save();
			PlayInfo.Save();
			return num2;
		}
		return 0;
	}

	public void Save()
	{
		DataRegistry.KeyData parent = DataRegistry.Set(null, "PlayerData", string.Empty);
		DataRegistry.Set(parent, "heroName", heroName);
		DataRegistry.Set(parent, "tutorialMode", tutorialMode);
		DataRegistry.Set(parent, "reviewIndex", reviewIndex);
		DataRegistry.Set(parent, "gem", gem);
		DataRegistry.Set(parent, "gold", gold);
		DataRegistry.Set(parent, "cmdPts", cmdPts);
		DataRegistry.Set(parent, "maxCmdPts", maxCmdPts);
		DataRegistry.Set(parent, "startLoveGame", startLoveGame);
		DataRegistry.Set(parent, "countHeart", countHeart);
		DataRegistry.Set(parent, "loveProgress", loveProgress);
		string keyvalue = timeBefCmdPtsInc.Ticks.ToString();
		DataRegistry.Set(parent, "timeBefCmdPtsIncUTC", keyvalue);
		int num = 0;
		UnitItem[] array = equipWeapon;
		foreach (UnitItem unitItem in array)
		{
			DataRegistry.Set(parent, "equipWeapon" + num, (unitItem != null) ? unitItem.code : (-1));
			num++;
		}
		num = 0;
		UnitItem[] array2 = wearCloth;
		foreach (UnitItem unitItem2 in array2)
		{
			DataRegistry.Set(parent, "wearCloth" + num, (unitItem2 != null) ? unitItem2.code : (-1));
			num++;
		}
		DataRegistry.Set(parent, "slotHp", (slotHp != null) ? slotHp.code : (-1));
		DataRegistry.Set(parent, "slotMp", (slotMp != null) ? slotMp.code : (-1));
		DataRegistry.Set(parent, "slotRing", (slotRing != null) ? slotRing.code : (-1));
		DataRegistry.Set(parent, "skillLevel", skill.skillLevel);
		for (int k = 0; k < 3; k++)
		{
			DataRegistry.Set(parent, "skillEquip" + k, equipSkill[k]);
		}
	}

	public void Load()
	{
		string keyvalue = string.Empty;
		DataRegistry.KeyData current = null;
		if (!DataRegistry.Get(null, "PlayerData", ref keyvalue, ref current))
		{
			return;
		}
		DataRegistry.KeyData current2 = null;
		DataRegistry.Get(current, "heroName", ref heroName, ref current2);
		DataRegistry.Get(current, "tutorialMode", ref tutorialMode, ref current2);
		DataRegistry.Get(current, "reviewIndex", ref reviewIndex, ref current2);
		DataRegistry.Get(current, "gem", ref gem, ref current2);
		DataRegistry.Get(current, "gold", ref gold, ref current2);
		DataRegistry.Get(current, "cmdPts", ref cmdPts, ref current2);
		DataRegistry.Get(current, "maxCmdPts", ref maxCmdPts, ref current2);
		DataRegistry.Get(current, "startLoveGame", ref startLoveGame, ref current2);
		DataRegistry.Get(current, "countHeart", ref countHeart, ref current2);
		DataRegistry.Get(current, "loveProgress", ref loveProgress, ref current2);
		string keyvalue2 = string.Empty;
		DataRegistry.Get(current, "timeBefCmdPtsIncUTC", ref keyvalue2, ref current2);
		long result = 0L;
		if (long.TryParse(keyvalue2, out result))
		{
			timeBefCmdPtsInc = DateTime.MinValue.AddTicks(result);
		}
		for (int i = 0; i < equipWeapon.Length; i++)
		{
			int keyvalue3 = -1;
			DataRegistry.Get(current, "equipWeapon" + i, ref keyvalue3, ref current2);
			if (keyvalue3 > -1)
			{
				UnitItem unitItem = PlayInfo.inventory.FindItem(keyvalue3);
				if (unitItem != null)
				{
					equipWeapon[i] = unitItem;
				}
			}
		}
		for (int j = 0; j < wearCloth.Length; j++)
		{
			int keyvalue4 = -1;
			DataRegistry.Get(current, "wearCloth" + j, ref keyvalue4, ref current2);
			if (keyvalue4 > -1)
			{
				UnitItem unitItem2 = PlayInfo.inventory.FindItem(keyvalue4);
				if (unitItem2 != null)
				{
					wearCloth[j] = unitItem2;
				}
			}
		}
		int keyvalue5 = -1;
		if (DataRegistry.Get(current, "slotHp", ref keyvalue5, ref current2))
		{
			if (keyvalue5 > -1)
			{
				UnitItem unitItem3 = PlayInfo.inventory.FindItem(keyvalue5);
				if (unitItem3 != null)
				{
					slotHp = unitItem3;
				}
			}
			else
			{
				slotHp = null;
			}
		}
		keyvalue5 = -1;
		if (DataRegistry.Get(current, "slotMp", ref keyvalue5, ref current2))
		{
			if (keyvalue5 > -1)
			{
				UnitItem unitItem4 = PlayInfo.inventory.FindItem(keyvalue5);
				if (unitItem4 != null)
				{
					slotMp = unitItem4;
				}
			}
			else
			{
				slotMp = null;
			}
		}
		keyvalue5 = -1;
		if (DataRegistry.Get(current, "slotRing", ref keyvalue5, ref current2))
		{
			if (keyvalue5 > -1)
			{
				UnitItem unitItem5 = PlayInfo.inventory.FindItem(keyvalue5);
				if (unitItem5 != null)
				{
					slotRing = unitItem5;
				}
			}
			else
			{
				slotRing = null;
			}
		}
		DataRegistry.Get(current, "skillLevel", ref skill.skillLevel, ref current2);
		for (int k = 0; k < 3; k++)
		{
			DataRegistry.Get(current, "skillEquip" + k, ref equipSkill[k], ref current2);
		}
	}
}
