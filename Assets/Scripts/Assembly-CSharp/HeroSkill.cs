using UnityEngine;

public class HeroSkill
{
	public enum SkillType
	{
		skillShieldSmash = 0,
		skillHealing = 1,
		skillSmack = 2,
		skillDefenseUp = 3,
		skillSmackDown = 4,
		skillSlashx2 = 5,
		skillRush = 6,
		skillSlashx3 = 7,
		skillSlashx5 = 8,
		skillBlast = 9,
		skillRagingBlow = 10,
		skillWhirlStrike = 11,
		skillGiantSwing = 12,
		skillShockwave = 13,
		skillBladeStorm = 14
	}

	public class SkillLevelSpec
	{
		public string name = string.Empty;

		public int skillLevel = 1;

		public int requireLevel = 1;

		public int attack;

		public int hp;

		public int defense;

		public float length;

		public bool splash;

		public float range;

		public bool knockdown;

		public bool knockback;

		public bool stun;

		public int mp;

		public float colldown;

		public float duration;

		public int currency;

		public int price;
	}

	[System.Serializable]
	private class SkillListRow
	{
		public string skillType;

		public string name;

		public int skillLevel;

		public int requireLevel;

		public int attack;

		public int hp;

		public int defense;

		public float length;

		public bool splash;

		public float range;

		public bool knockdown;

		public bool knockback;

		public bool stun;

		public int mp;

		public float colldown;

		public float duration;

		public string currency;

		public int price;
	}

	[System.Serializable]
	private class SkillListRowWrapper
	{
		public SkillListRow[] items;
	}

	public const int maxSkillSlot = 3;

	public const int maxSkillKind = 3;

	public const int maxSkillPerKind = 5;

	public const int maxSkill = 15;

	public const int maxSkillLevel = 5;

	public static int[][] skillClass = new int[3][]
	{
		new int[5] { 0, 1, 2, 3, 4 },
		new int[5] { 5, 6, 7, 8, 9 },
		new int[5] { 10, 11, 12, 13, 14 }
	};

	public static SkillLevelSpec[] levelSpec = new SkillLevelSpec[75];

	public int[] skillLevel = new int[15];

	public HeroSkill()
	{
		for (int i = 0; i < skillLevel.Length; i++)
		{
			skillLevel[i] = 0;
		}
	}

	public static SkillLevelSpec GetSkillLevelSpec(int skillIndex, int skillLevel)
	{
		return levelSpec[skillIndex * 5 + skillLevel];
	}

	public static void LoadDefault()
	{
		TextAsset textAsset = ResourceManager.Load("GameData", "skill_list", typeof(TextAsset)) as TextAsset;
		string json = "{\"items\":" + textAsset.text + "}";
		SkillListRowWrapper skillListRowWrapper = JsonUtility.FromJson<SkillListRowWrapper>(json);
		int num = 0;
		SkillListRow[] items = skillListRowWrapper.items;
		foreach (SkillListRow skillListRow in items)
		{
			SkillLevelSpec skillLevelSpec = new SkillLevelSpec();
			skillLevelSpec.name = skillListRow.name;
			skillLevelSpec.skillLevel = skillListRow.skillLevel;
			skillLevelSpec.requireLevel = skillListRow.requireLevel;
			skillLevelSpec.attack = skillListRow.attack;
			skillLevelSpec.hp = skillListRow.hp;
			skillLevelSpec.defense = skillListRow.defense;
			skillLevelSpec.length = skillListRow.length;
			skillLevelSpec.splash = skillListRow.splash;
			skillLevelSpec.range = skillListRow.range;
			skillLevelSpec.knockdown = skillListRow.knockdown;
			skillLevelSpec.knockback = skillListRow.knockback;
			skillLevelSpec.stun = skillListRow.stun;
			skillLevelSpec.mp = skillListRow.mp;
			skillLevelSpec.colldown = skillListRow.colldown;
			skillLevelSpec.duration = skillListRow.duration;
			skillLevelSpec.currency = ((!skillListRow.currency.Equals("gold")) ? 1 : 0);
			skillLevelSpec.price = skillListRow.price;
			levelSpec[num] = skillLevelSpec;
			num++;
		}
	}
}
