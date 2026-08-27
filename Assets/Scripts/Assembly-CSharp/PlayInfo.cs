using System.Collections.Generic;
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

	[System.Serializable]
	private class HeroDefaultRow
	{
		public int level;

		public int fame;

		public int strength;

		public int intellectual;

		public int constitution;

		public int baseHp;

		public int baseMp;

		public int critical;

		public int speed;
	}

	[System.Serializable]
	private class HeroDefaultRowList
	{
		public HeroDefaultRow[] items;
	}

	private static void LoadHeroDefault()
	{
		TextAsset textAsset = ResourceManager.Load("GameData", "hero_default", typeof(TextAsset)) as TextAsset;
		if (textAsset == null || string.IsNullOrEmpty(textAsset.text))
		{
			Debug.LogError("Decrypt Error!!");
			return;
		}
		HeroDefaultRowList heroDefaultRowList = JsonUtility.FromJson<HeroDefaultRowList>("{\"items\":" + textAsset.text + "}");
		foreach (HeroDefaultRow item in heroDefaultRowList.items)
		{
			heroState.level = item.level;
			heroState.fame = item.fame;
			heroState.strength = item.strength;
			heroState.intellectual = item.intellectual;
			heroState.constitution = item.constitution;
			heroState.baseHp = item.baseHp;
			heroState.baseMp = item.baseMp;
			heroState.critical = item.critical;
			heroState.speed = item.speed;
		}
	}

	[System.Serializable]
	private class BattleRewardRow
	{
		public string type;

		public int fame;

		public int gem;

		public float xp_a;

		public float xp_b;

		public float xp_inc;

		public float gold_a;

		public float gold_b;

		public float gold_inc;
	}

	[System.Serializable]
	private class BattleRewardRowList
	{
		public BattleRewardRow[] items;
	}

	private static void LoadBattleReward()
	{
		TextAsset textAsset = ResourceManager.Load("GameData", "battle_reward", typeof(TextAsset)) as TextAsset;
		if (textAsset == null || string.IsNullOrEmpty(textAsset.text))
		{
			Debug.LogError("Decrypt Error!!");
			return;
		}
		BattleRewardRowList battleRewardRowList = JsonUtility.FromJson<BattleRewardRowList>("{\"items\":" + textAsset.text + "}");
		foreach (BattleRewardRow item in battleRewardRowList.items)
		{
			if (item.type.Equals("victory"))
			{
				battleRewardVictory.fame = item.fame;
				battleRewardVictory.gem = item.gem;
				battleRewardVictory.xp_a = item.xp_a;
				battleRewardVictory.xp_b = item.xp_b;
				battleRewardVictory.xp_inc = item.xp_inc;
				battleRewardVictory.gold_a = item.gold_a;
				battleRewardVictory.gold_b = item.gold_b;
				battleRewardVictory.gold_inc = item.gold_inc;
			}
			else if (item.type.Equals("defeat"))
			{
				battleRewardDepeat.fame = item.fame;
				battleRewardDepeat.gem = item.gem;
				battleRewardDepeat.xp_a = item.xp_a;
				battleRewardDepeat.xp_b = item.xp_b;
				battleRewardDepeat.xp_inc = item.xp_inc;
				battleRewardDepeat.gold_a = item.gold_a;
				battleRewardDepeat.gold_b = item.gold_b;
				battleRewardDepeat.gold_inc = item.gold_inc;
			}
		}
	}

	[System.Serializable]
	private class HeroLevelTableRow
	{
		public int level;

		public int exp;

		public int incStats;
	}

	[System.Serializable]
	private class HeroLevelTableRowList
	{
		public HeroLevelTableRow[] items;
	}

	private static void LoadHeroLevelTable()
	{
		TextAsset textAsset = ResourceManager.Load("GameData", "hero_leveltable", typeof(TextAsset)) as TextAsset;
		if (textAsset == null || string.IsNullOrEmpty(textAsset.text))
		{
			Debug.LogError("Decrypt Error!!");
			return;
		}
		HeroLevelTableRowList heroLevelTableRowList = JsonUtility.FromJson<HeroLevelTableRowList>("{\"items\":" + textAsset.text + "}");
		int num = 0;
		HeroLevelTableRow[] items = heroLevelTableRowList.items;
		foreach (HeroLevelTableRow item in items)
		{
			heroLevelTable[num] = new HeroLevelTable();
			heroLevelTable[num].level = item.level;
			heroLevelTable[num].exp = item.exp;
			heroLevelTable[num].incStats = item.incStats;
			num++;
		}
	}

	[System.Serializable]
	private class MercyTableRow
	{
		public int raiseRate;

		public int costGold;
	}

	[System.Serializable]
	private class MercyTableRowList
	{
		public MercyTableRow[] items;
	}

	private static void LoadMercyTable()
	{
		TextAsset textAsset = ResourceManager.Load("GameData", "mercy_list", typeof(TextAsset)) as TextAsset;
		if (textAsset == null || string.IsNullOrEmpty(textAsset.text))
		{
			Debug.LogError("Decrypt Error!!");
			return;
		}
		MercyTableRowList mercyTableRowList = JsonUtility.FromJson<MercyTableRowList>("{\"items\":" + textAsset.text + "}");
		int num = 0;
		MercyTableRow[] items = mercyTableRowList.items;
		foreach (MercyTableRow item in items)
		{
			mercyTable[num] = new MercyTable();
			mercyTable[num].raiseRate = item.raiseRate;
			mercyTable[num].costGold = item.costGold;
			num++;
		}
	}

	[System.Serializable]
	private class MonsterBornRow
	{
		public int level;

		public int bornRate;
	}

	[System.Serializable]
	private class MonsterBornRowList
	{
		public MonsterBornRow[] items;
	}

	private static void LoadMonsterBorn()
	{
		TextAsset textAsset = ResourceManager.Load("GameData", "monster_born", typeof(TextAsset)) as TextAsset;
		if (textAsset == null || string.IsNullOrEmpty(textAsset.text))
		{
			Debug.LogError("Decrypt Error!!");
			return;
		}
		MonsterBornRowList monsterBornRowList = JsonUtility.FromJson<MonsterBornRowList>("{\"items\":" + textAsset.text + "}");
		PlayInfo.monsterBorn.Clear();
		MonsterBornRow[] items = monsterBornRowList.items;
		foreach (MonsterBornRow item in items)
		{
			MonsterBorn monsterBorn = new MonsterBorn();
			monsterBorn.bornRate = item.bornRate;
			PlayInfo.monsterBorn.Add(monsterBorn);
		}
	}

	[System.Serializable]
	private class MonsterAttackIntervalRow
	{
		public int heroLevel;

		public int days;
	}

	[System.Serializable]
	private class MonsterAttackIntervalRowList
	{
		public MonsterAttackIntervalRow[] items;
	}

	private static void LoadMonsterAttackInterval()
	{
		TextAsset textAsset = ResourceManager.Load("GameData", "monster_interval", typeof(TextAsset)) as TextAsset;
		if (textAsset == null || string.IsNullOrEmpty(textAsset.text))
		{
			Debug.LogError("Decrypt Error!!");
			return;
		}
		MonsterAttackIntervalRowList monsterAttackIntervalRowList = JsonUtility.FromJson<MonsterAttackIntervalRowList>("{\"items\":" + textAsset.text + "}");
		PlayInfo.monsterAttackInterval.Clear();
		MonsterAttackIntervalRow[] items = monsterAttackIntervalRowList.items;
		foreach (MonsterAttackIntervalRow item in items)
		{
			MonsterAttackInterval monsterAttackInterval = new MonsterAttackInterval();
			monsterAttackInterval.heroLevel = item.heroLevel;
			monsterAttackInterval.days = item.days;
			PlayInfo.monsterAttackInterval.Add(monsterAttackInterval);
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
