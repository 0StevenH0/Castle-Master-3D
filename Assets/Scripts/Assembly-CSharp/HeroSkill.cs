using System.IO;
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
					SkillLevelSpec skillLevelSpec = new SkillLevelSpec();
					skillLevelSpec.name = array[1];
					skillLevelSpec.skillLevel = int.Parse(array[2]);
					skillLevelSpec.requireLevel = int.Parse(array[3]);
					skillLevelSpec.attack = int.Parse(array[4]);
					skillLevelSpec.hp = int.Parse(array[5]);
					skillLevelSpec.defense = int.Parse(array[6]);
					skillLevelSpec.length = float.Parse(array[7]);
					skillLevelSpec.splash = array[8].Equals("1");
					skillLevelSpec.range = float.Parse(array[9]);
					skillLevelSpec.knockdown = array[10].Equals("1");
					skillLevelSpec.knockback = array[11].Equals("1");
					skillLevelSpec.stun = array[12].Equals("1");
					skillLevelSpec.mp = int.Parse(array[13]);
					skillLevelSpec.colldown = float.Parse(array[14]);
					skillLevelSpec.duration = float.Parse(array[15]);
					skillLevelSpec.currency = ((!array[16].Equals("gold")) ? 1 : 0);
					skillLevelSpec.price = int.Parse(array[17]);
					levelSpec[num] = skillLevelSpec;
					num++;
				}
			}
		}
	}
}
