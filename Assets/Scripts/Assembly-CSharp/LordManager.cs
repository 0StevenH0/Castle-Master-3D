using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LordManager
{
	public class Lord
	{
		public int castleIndex = -1;

		public LordType type;

		public int level = 1;

		public int fame = 60;

		public int loyalty = 60;

		public int atk = 1;

		public int def = 1;

		public int str = 1;

		public int inte = 1;

		public int con = 1;

		public int[] cloth;

		public int[] weapon;

		public int hp;

		public int critical;

		public float speed;

		public float splash;

		public float splashLength;

		public float nextDownDay;

		public int cost
		{
			get
			{
				return level * 5 + fame * 10;
			}
		}

		public string name
		{
			get
			{
				return "Lv." + level + " " + type.ToString();
			}
		}
	}

	public class LordDefault
	{
		public float rateFame = 100f;

		public float rateAttak = 100f;

		public float rateInte = 100f;

		public float rateHp = 100f;

		public float speed = 1f;

		public float splash;

		public float splashLength = 1f;

		public int regenTime = 10;
	}

	public enum LordType
	{
		Strategist = 0,
		Swordsman = 1,
		Tanker = 2,
		max = 3
	}

	public const int defIncLoyalty = 10;

	public List<Lord> list = new List<Lord>();

	public static LordDefault[] lordDefault;

	public void Init()
	{
		TextAsset textAsset = ResourceManager.Load("GameData", "lord_default", typeof(TextAsset)) as TextAsset;
		bool succeed = false;
		string s = DataSecurity.Decrypt(textAsset.text, "surkwjch", out succeed);
		if (!succeed)
		{
			Debug.LogError("Decrypt Error!!");
			return;
		}
		StringReader stringReader = new StringReader(s);
		List<LordDefault> list = new List<LordDefault>();
		string text;
		while ((text = stringReader.ReadLine()) != null)
		{
			if (text.Trim().Length != 0)
			{
				char[] separator = new char[1] { '\t' };
				string[] array = text.Split(separator);
				if (array.Length > 1)
				{
					LordDefault lordDefault = new LordDefault();
					lordDefault.rateFame = float.Parse(array[1]);
					lordDefault.rateAttak = float.Parse(array[2]);
					lordDefault.rateInte = float.Parse(array[3]);
					lordDefault.rateHp = float.Parse(array[4]);
					lordDefault.speed = float.Parse(array[6]);
					lordDefault.splash = float.Parse(array[7]);
					lordDefault.splashLength = float.Parse(array[8]);
					lordDefault.regenTime = int.Parse(array[9]);
					list.Add(lordDefault);
				}
			}
		}
		LordManager.lordDefault = list.ToArray();
	}

	public Lord SearchRandom()
	{
		Lord lord = new Lord();
		int num = Random.Range(0, 3);
		LordType type = (LordType)num;
		LordDefault lordDefault = LordManager.lordDefault[num];
		lord.type = type;
		lord.level = PlayInfo.heroState.level + Random.Range(-2, 3);
		if (lord.level < 1)
		{
			lord.level = 1;
		}
		if (lord.level > 99)
		{
			lord.level = 99;
		}
		int num2 = lord.level - PlayInfo.heroState.level;
		int num3 = num2 * Random.Range(0, 4);
		lord.fame = (int)((float)PlayInfo.heroState.fame * (lordDefault.rateFame + (float)num3) / 100f);
		lord.loyalty = PlayInfo.gameRule.lordDefaultLoyalty;
		num3 = num2 * Random.Range(0, 4);
		lord.atk = (int)((float)PlayInfo.heroState.sumAttack * (lordDefault.rateAttak + (float)num3) / 100f);
		lord.def = 0;
		lord.str = PlayInfo.heroState.sumStr;
		num3 = num2 * Random.Range(0, 4);
		lord.inte = (int)((float)PlayInfo.heroState.sumInt * (lordDefault.rateInte + (float)num3) / 100f);
		lord.con = PlayInfo.heroState.sumCon;
		lord.cloth = new int[3];
		lord.cloth[0] = 200 + Random.Range(0, 7);
		lord.cloth[1] = 210 + Random.Range(0, 7);
		lord.cloth[2] = 220 + Random.Range(0, 7);
		lord.weapon = new int[3];
		lord.weapon[0] = 100 + Random.Range(0, 7);
		lord.weapon[1] = 110 + Random.Range(0, 7);
		lord.weapon[2] = 120 + Random.Range(0, 7);
		num3 = num2 * Random.Range(0, 4);
		lord.hp = (int)((float)PlayInfo.heroState.sumHp * (lordDefault.rateHp + (float)num3) / 100f);
		lord.critical = PlayInfo.heroState.sumCri;
		lord.speed = lordDefault.speed;
		lord.splash = lordDefault.splash;
		lord.splashLength = lordDefault.splashLength;
		lord.nextDownDay = PlayInfo.gameRule.lordLoyaltyDownDays;
		return lord;
	}

	public void RemoveInCastle(int castleIndex)
	{
		Lord item = null;
		foreach (Lord item2 in list)
		{
			if (item2.castleIndex == castleIndex)
			{
				item = item2;
				break;
			}
		}
		list.Remove(item);
	}

	public Lord GetLordInCastle(int castleIndex)
	{
		foreach (Lord item in list)
		{
			if (item.castleIndex == castleIndex)
			{
				return item;
			}
		}
		return null;
	}

	public void ProcessDaily()
	{
		List<Lord> list = new List<Lord>();
		foreach (Lord item in this.list)
		{
			item.nextDownDay -= 1f;
			if (!(item.nextDownDay <= 0f))
			{
				continue;
			}
			item.loyalty -= PlayInfo.gameRule.lordLoyaltyDown;
			if (item.loyalty < 0)
			{
				item.loyalty = 0;
			}
			item.nextDownDay = PlayInfo.gameRule.lordLoyaltyDownDays;
			if (item.loyalty < PlayInfo.gameRule.lordFireLoyalty)
			{
				string empty = string.Empty;
				string msgLordRunAway = StringContent.msgLordRunAway;
				empty = StringContent.msgMinLoyaltyRunAwayInCastle;
				empty = empty.Replace(StringContent.strValue, item.name);
				msgLordRunAway = msgLordRunAway.Replace(StringContent.strValue, item.name);
				if (item.castleIndex > -1)
				{
					empty = empty.Replace(StringContent.strValue2, PlayInfo.castleManager.castle[item.castleIndex].castleName);
					msgLordRunAway = msgLordRunAway + " (" + PlayInfo.castleManager.castle[item.castleIndex].castleName + ")";
				}
				PlayInfo.messageManager.Add(5, PlayMessage.MessagLevel.alert, empty, msgLordRunAway);
				PlayInfo.castleManager.castle[item.castleIndex].lord = null;
				list.Add(item);
			}
			else if (item.loyalty < PlayInfo.gameRule.lordFireLoyalty + PlayInfo.gameRule.lordLoyaltyDown * 2)
			{
				string empty2 = string.Empty;
				string msgLordNeedReward = StringContent.msgLordNeedReward;
				empty2 = StringContent.msgMercyForLordLoyaltyInCastle;
				empty2 = empty2.Replace(StringContent.strValue, item.name);
				msgLordNeedReward = msgLordNeedReward.Replace(StringContent.strValue, item.name);
				if (item.castleIndex > -1)
				{
					empty2 = empty2.Replace(StringContent.strValue2, PlayInfo.castleManager.castle[item.castleIndex].castleName);
					msgLordNeedReward = msgLordNeedReward + " (" + PlayInfo.castleManager.castle[item.castleIndex].castleName + ")";
				}
				PlayInfo.messageManager.Add(9, PlayMessage.MessagLevel.warning, empty2, msgLordNeedReward);
			}
		}
		foreach (Lord item2 in list)
		{
			this.list.Remove(item2);
		}
	}

	public static int GetAppointCost()
	{
		return PlayInfo.heroState.level * 5 + PlayInfo.heroState.fame * 15;
	}

	public static int GetRewardCost(Lord ret)
	{
		return ret.level * 5 + ret.fame * 10;
	}

	public void Save()
	{
		DataRegistry.KeyData parent = DataRegistry.Set(null, "LordList", list.Count);
		int num = 0;
		foreach (Lord item in list)
		{
			DataRegistry.KeyData parent2 = DataRegistry.Set(parent, num.ToString(), string.Empty);
			DataRegistry.Set(parent2, "castleIndex", item.castleIndex);
			DataRegistry.Set(parent2, "type", (int)item.type);
			DataRegistry.Set(parent2, "level", item.level);
			DataRegistry.Set(parent2, "fame", item.fame);
			DataRegistry.Set(parent2, "loyalty", item.loyalty);
			DataRegistry.Set(parent2, "atk", item.atk);
			DataRegistry.Set(parent2, "def", item.def);
			DataRegistry.Set(parent2, "str", item.str);
			DataRegistry.Set(parent2, "inte", item.inte);
			DataRegistry.Set(parent2, "con", item.con);
			DataRegistry.Set(parent2, "cloth", item.cloth);
			DataRegistry.Set(parent2, "weapon", item.weapon);
			DataRegistry.Set(parent2, "hp", item.hp);
			DataRegistry.Set(parent2, "critical", item.critical);
			DataRegistry.Set(parent2, "speed", item.speed);
			DataRegistry.Set(parent2, "splash", item.splash);
			DataRegistry.Set(parent2, "splashLength", item.splashLength);
			DataRegistry.Set(parent2, "nextDownDay", item.nextDownDay);
			num++;
		}
	}

	public void Load()
	{
		list.Clear();
		int keyvalue = 0;
		DataRegistry.KeyData current = null;
		if (!DataRegistry.Get(null, "LordList", ref keyvalue, ref current))
		{
			return;
		}
		for (int i = 0; i < keyvalue; i++)
		{
			DataRegistry.KeyData current2 = null;
			string keyvalue2 = string.Empty;
			if (DataRegistry.Get(current, i.ToString(), ref keyvalue2, ref current2))
			{
				Lord lord = new Lord();
				lord.cloth = new int[3];
				lord.weapon = new int[3];
				DataRegistry.KeyData current3 = null;
				DataRegistry.Get(current2, "castleIndex", ref lord.castleIndex, ref current3);
				int keyvalue3 = 0;
				DataRegistry.Get(current2, "type", ref keyvalue3, ref current3);
				lord.type = (LordType)keyvalue3;
				DataRegistry.Get(current2, "level", ref lord.level, ref current3);
				DataRegistry.Get(current2, "fame", ref lord.fame, ref current3);
				DataRegistry.Get(current2, "loyalty", ref lord.loyalty, ref current3);
				DataRegistry.Get(current2, "atk", ref lord.atk, ref current3);
				DataRegistry.Get(current2, "def", ref lord.def, ref current3);
				DataRegistry.Get(current2, "str", ref lord.str, ref current3);
				DataRegistry.Get(current2, "inte", ref lord.inte, ref current3);
				DataRegistry.Get(current2, "con", ref lord.con, ref current3);
				DataRegistry.Get(current2, "cloth", ref lord.cloth, ref current3);
				DataRegistry.Get(current2, "weapon", ref lord.weapon, ref current3);
				DataRegistry.Get(current2, "hp", ref lord.hp, ref current3);
				DataRegistry.Get(current2, "critical", ref lord.critical, ref current3);
				DataRegistry.Get(current2, "speed", ref lord.speed, ref current3);
				DataRegistry.Get(current2, "splash", ref lord.splash, ref current3);
				DataRegistry.Get(current2, "splashLength", ref lord.splashLength, ref current3);
				DataRegistry.Get(current2, "nextDownDay", ref lord.nextDownDay, ref current3);
				list.Add(lord);
				if (lord.castleIndex > -1)
				{
					PlayInfo.castleManager.castle[lord.castleIndex].lord = lord;
				}
			}
		}
	}
}
