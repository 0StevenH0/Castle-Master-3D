using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayInfo
{
	public class BattleInfo
	{
		public bool isAttack;

		public CastleInfo attackCastle;

		public CastleInfo defenseCastle;

		public int[] attackUnits;

		public int[] defenseUnits;

		public bool battleFinish;

		public bool battleWin;
	}

	public class BattleReward
	{
		public int fame;

		public int gem;

		public float xp_a;

		public float xp_b;

		public float xp_inc;

		public float gold_a;

		public float gold_b;

		public float gold_inc;
	}

	public class HeroLevelTable
	{
		public int level;

		public int exp;

		public int incStats;
	}

	public class MercyTable
	{
		public int raiseRate;

		public int costGold;
	}

	public class MonsterBorn
	{
		public int bornRate;
	}

	public class MonsterAttackInterval
	{
		public int heroLevel;

		public int days;
	}

	public const int maxHeroLevelTable = 99;

	public const int maxMercyTable = 3;

	public static GameTime gameTime = new GameTime();

	public static GameRule gameRule = new GameRule();

	public static ItemManager itemManager = new ItemManager();

	public static InventoryManager inventory = new InventoryManager();

	public static CastleManager castleManager = new CastleManager();

	public static PlayerData playerData = new PlayerData();

	public static MilitaryManager humanMilitary = new MilitaryManager();

	public static MilitaryManager monsterMilitary = new MilitaryManager();

	public static UnitState heroState = new UnitState();

	public static LordManager lordManager = new LordManager();

	public static UnitUpgrade unitUpgrade = new UnitUpgrade();

	public static QuestManager questManager = new QuestManager();

	public static MessageManager messageManager = new MessageManager();

	public static BattleInfo battleInfo;

	public static BattleReward battleRewardVictory = new BattleReward();

	public static BattleReward battleRewardDepeat = new BattleReward();

	public static HeroLevelTable[] heroLevelTable = new HeroLevelTable[99];

	public static MercyTable[] mercyTable = new MercyTable[3];

	public static FortuneSystem fortuneSystem = new FortuneSystem();

	public static CastleAI castleAI = new CastleAI();

	public static List<MonsterBorn> monsterBorn = new List<MonsterBorn>();

	public static List<MonsterAttackInterval> monsterAttackInterval = new List<MonsterAttackInterval>();

	public static ExtSoundManager soundManager = new ExtSoundManager();

	public static void Init()
	{
		gameTime = new GameTime();
		gameRule = new GameRule();
		itemManager = new ItemManager();
		inventory = new InventoryManager();
		castleManager = new CastleManager();
		playerData = new PlayerData();
		humanMilitary = new MilitaryManager();
		monsterMilitary = new MilitaryManager();
		heroState = new UnitState();
		lordManager = new LordManager();
		unitUpgrade = new UnitUpgrade();
		questManager = new QuestManager();
		messageManager = new MessageManager();
		fortuneSystem = new FortuneSystem();
		castleAI = new CastleAI();
		gameRule.LoadDefault();
		gameTime.LoadDefault();
		ReviewGem.LoadDefault();
		humanMilitary.LoadDefault(0);
		monsterMilitary.LoadDefault(1);
		itemManager.LoadDefault();
		QuestManager.LoadDefault();
		questManager.Init();
		CastleInfo.LoadDefault();
		castleManager.LoadDefault();
		inventory.SetDefault();
		playerData.LoadDefault();
		lordManager.Init();
		LoadHeroDefault();
		LoadBattleReward();
		LoadHeroLevelTable();
		LoadMercyTable();
		LoadMonsterBorn();
		LoadMonsterAttackInterval();
		unitUpgrade.Init();
		heroState.playerData = playerData;
		FortuneSystem.Init();
		castleAI.Init();
		UILoveGame.Init();
		UINpcAction.Init();
		CastleInfo[] castle = castleManager.castle;
		foreach (CastleInfo castleInfo in castle)
		{
			castleInfo.SetAutoUpgrade();
		}
		soundManager.Init();
	}

	public static void Reinit()
	{
		gameTime = new GameTime();
		castleManager = new CastleManager();
		humanMilitary = new MilitaryManager();
		monsterMilitary = new MilitaryManager();
		lordManager = new LordManager();
		unitUpgrade = new UnitUpgrade();
		messageManager = new MessageManager();
		fortuneSystem = new FortuneSystem();
		castleAI = new CastleAI();
		gameTime.LoadDefault();
		humanMilitary.LoadDefault(0);
		monsterMilitary.LoadDefault(1);
		CastleInfo.LoadDefault();
		castleManager.LoadDefault();
		lordManager.Init();
		unitUpgrade.Init();
		FortuneSystem.Init();
		castleAI.Init();
		playerData.startLoveGame = false;
		playerData.loveProgress = 0f;
		playerData.countHeart = 0;
		CastleInfo[] castle = castleManager.castle;
		foreach (CastleInfo castleInfo in castle)
		{
			castleInfo.SetAutoUpgrade();
		}
	}

	public static void Save()
	{
		gameTime.Save();
		inventory.Save();
		castleManager.Save();
		playerData.Save();
		lordManager.Save();
		humanMilitary.Save();
		monsterMilitary.Save();
		SaveHeroState();
		messageManager.Save();
		castleAI.Save();
		unitUpgrade.Save();
		questManager.Save();
		fortuneSystem.Save();
		DataRegistry.Save();
	}

	public static void Load()
	{
		DataRegistry.Load();
		gameTime.Load();
		inventory.Load();
		castleManager.Load();
		playerData.Load();
		lordManager.Load();
		humanMilitary.Load();
		monsterMilitary.Load();
		LoadHeroState();
		messageManager.Load();
		castleAI.Load();
		unitUpgrade.Load();
		questManager.Load();
		fortuneSystem.Load();
	}

	private static void LoadHeroDefault()
	{
		TextAsset textAsset = ResourceManager.Load("GameData", "hero_default", typeof(TextAsset)) as TextAsset;
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
			if (text.Trim().Length != 0)
			{
				char[] separator = new char[1] { '\t' };
				string[] array = text.Split(separator);
				if (array.Length > 1)
				{
					heroState.level = int.Parse(array[0]);
					heroState.fame = int.Parse(array[1]);
					heroState.strength = int.Parse(array[2]);
					heroState.intellectual = int.Parse(array[3]);
					heroState.constitution = int.Parse(array[4]);
					heroState.baseHp = int.Parse(array[5]);
					heroState.baseMp = int.Parse(array[6]);
					heroState.critical = int.Parse(array[7]);
					heroState.speed = int.Parse(array[8]);
				}
			}
		}
	}

	private static void LoadBattleReward()
	{
		TextAsset textAsset = ResourceManager.Load("GameData", "battle_reward", typeof(TextAsset)) as TextAsset;
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
				string text2 = array[0];
				if (text2.Equals("victory"))
				{
					battleRewardVictory.fame = int.Parse(array[1]);
					battleRewardVictory.gem = int.Parse(array[2]);
					battleRewardVictory.xp_a = float.Parse(array[3]);
					battleRewardVictory.xp_b = float.Parse(array[4]);
					battleRewardVictory.xp_inc = float.Parse(array[5]);
					battleRewardVictory.gold_a = float.Parse(array[6]);
					battleRewardVictory.gold_b = float.Parse(array[7]);
					battleRewardVictory.gold_inc = float.Parse(array[8]);
				}
				else if (text2.Equals("defeat"))
				{
					battleRewardDepeat.fame = int.Parse(array[1]);
					battleRewardDepeat.gem = int.Parse(array[2]);
					battleRewardDepeat.xp_a = float.Parse(array[3]);
					battleRewardDepeat.xp_b = float.Parse(array[4]);
					battleRewardDepeat.xp_inc = float.Parse(array[5]);
					battleRewardDepeat.gold_a = float.Parse(array[6]);
					battleRewardDepeat.gold_b = float.Parse(array[7]);
					battleRewardDepeat.gold_inc = float.Parse(array[8]);
				}
			}
		}
	}

	private static void LoadHeroLevelTable()
	{
		TextAsset textAsset = ResourceManager.Load("GameData", "hero_leveltable", typeof(TextAsset)) as TextAsset;
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
					heroLevelTable[num] = new HeroLevelTable();
					heroLevelTable[num].level = int.Parse(array[0]);
					heroLevelTable[num].exp = int.Parse(array[1]);
					heroLevelTable[num].incStats = int.Parse(array[2]);
					num++;
				}
			}
		}
	}

	private static void LoadMercyTable()
	{
		TextAsset textAsset = ResourceManager.Load("GameData", "mercy_list", typeof(TextAsset)) as TextAsset;
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
					mercyTable[num] = new MercyTable();
					mercyTable[num].raiseRate = int.Parse(array[0]);
					mercyTable[num].costGold = int.Parse(array[1]);
					num++;
				}
			}
		}
	}

	private static void LoadMonsterBorn()
	{
		TextAsset textAsset = ResourceManager.Load("GameData", "monster_born", typeof(TextAsset)) as TextAsset;
		bool succeed = false;
		string s = DataSecurity.Decrypt(textAsset.text, "surkwjch", out succeed);
		if (!succeed)
		{
			Debug.LogError("Decrypt Error!!");
			return;
		}
		StringReader stringReader = new StringReader(s);
		PlayInfo.monsterBorn.Clear();
		string text;
		while ((text = stringReader.ReadLine()) != null)
		{
			if (text.Trim().Length != 0)
			{
				char[] separator = new char[1] { '\t' };
				string[] array = text.Split(separator);
				if (array.Length > 1)
				{
					MonsterBorn monsterBorn = new MonsterBorn();
					monsterBorn.bornRate = int.Parse(array[1]);
					PlayInfo.monsterBorn.Add(monsterBorn);
				}
			}
		}
	}

	private static void LoadMonsterAttackInterval()
	{
		TextAsset textAsset = ResourceManager.Load("GameData", "monster_interval", typeof(TextAsset)) as TextAsset;
		bool succeed = false;
		string s = DataSecurity.Decrypt(textAsset.text, "surkwjch", out succeed);
		if (!succeed)
		{
			Debug.LogError("Decrypt Error!!");
			return;
		}
		StringReader stringReader = new StringReader(s);
		PlayInfo.monsterAttackInterval.Clear();
		string text;
		while ((text = stringReader.ReadLine()) != null)
		{
			if (text.Trim().Length != 0)
			{
				char[] separator = new char[1] { '\t' };
				string[] array = text.Split(separator);
				if (array.Length > 1)
				{
					MonsterAttackInterval monsterAttackInterval = new MonsterAttackInterval();
					monsterAttackInterval.heroLevel = int.Parse(array[0]);
					monsterAttackInterval.days = int.Parse(array[1]);
					PlayInfo.monsterAttackInterval.Add(monsterAttackInterval);
				}
			}
		}
	}

	private static void SaveHeroState()
	{
		DataRegistry.KeyData parent = DataRegistry.Set(null, "HeroState", string.Empty);
		DataRegistry.Set(parent, "level", heroState.level);
		DataRegistry.Set(parent, "statsPoint", heroState.statsPoint);
		DataRegistry.Set(parent, "strength", heroState.strength);
		DataRegistry.Set(parent, "exp", heroState.exp);
		DataRegistry.Set(parent, "expSum", heroState.expSum);
		DataRegistry.Set(parent, "fame", heroState.fame);
		DataRegistry.Set(parent, "intellectual", heroState.intellectual);
		DataRegistry.Set(parent, "constitution", heroState.constitution);
	}

	private static void LoadHeroState()
	{
		string keyvalue = string.Empty;
		DataRegistry.KeyData current = null;
		if (DataRegistry.Get(null, "HeroState", ref keyvalue, ref current))
		{
			DataRegistry.KeyData current2 = null;
			DataRegistry.Get(current, "level", ref heroState.level, ref current2);
			DataRegistry.Get(current, "statsPoint", ref heroState.statsPoint, ref current2);
			DataRegistry.Get(current, "strength", ref heroState.strength, ref current2);
			DataRegistry.Get(current, "exp", ref heroState.exp, ref current2);
			DataRegistry.Get(current, "expSum", ref heroState.expSum, ref current2);
			DataRegistry.Get(current, "fame", ref heroState.fame, ref current2);
			DataRegistry.Get(current, "intellectual", ref heroState.intellectual, ref current2);
			DataRegistry.Get(current, "constitution", ref heroState.constitution, ref current2);
		}
	}

	public static void IncUnitExp(UnitState unitState, float exp, out bool isLevelUp)
	{
		unitState.exp += exp;
		unitState.expSum = (int)((float)unitState.expSum + exp);
		isLevelUp = false;
		while (CheckLevelUp(unitState))
		{
			isLevelUp = true;
		}
	}

	private static bool CheckLevelUp(UnitState unitState)
	{
		if (unitState.level == 99)
		{
			return false;
		}
		float nextExp = GetNextExp(unitState);
		if (unitState.exp >= nextExp)
		{
			unitState.level++;
			unitState.exp -= nextExp;
			int num = unitState.level - 1;
			if (num < 0)
			{
				num = 0;
			}
			if (num >= 99)
			{
				num = 98;
			}
			unitState.statsPoint += heroLevelTable[num].incStats;
			return true;
		}
		return false;
	}

	public static float GetNextExp(UnitState unitState)
	{
		int num = unitState.level;
		if (num < 1)
		{
			num = 1;
		}
		if (num >= 99)
		{
			num = 98;
		}
		return heroLevelTable[num].exp;
	}
}
