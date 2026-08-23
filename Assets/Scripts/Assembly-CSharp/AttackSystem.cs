public class AttackSystem
{
	private const int weaponMax = 3;

	private const int heroAttackMax = 7;

	private const int soldierMax = 5;

	private const int soldierAttackMax = 2;

	private const int monsterMax = 17;

	private const int monsterAttackMax = 4;

	private const int lordMax = 3;

	private const int lordAttackMax = 2;

	public const int heroNormalAttackMax = 2;

	public const int heroSkillAttackMax = 5;

	public const int monsterNormalAttackMax = 2;

	public const int monsterSpecialAttackMax = 2;

	private static float[][][] heroAttackTimingFrame = new float[3][][]
	{
		new float[7][]
		{
			new float[1] { 0.25f },
			new float[1] { 0.25f },
			new float[1] { 0.6f },
			new float[1] { 0.625f },
			new float[1] { 1f },
			new float[1] { 0.625f },
			new float[3] { 1f, 1.5f, 2f }
		},
		new float[7][]
		{
			new float[2] { 0.375f, 0.5f },
			new float[2] { 0.375f, 0.5f },
			new float[2] { 0.25f, 0.375f },
			new float[1] { 0.125f },
			new float[3] { 0.25f, 0.375f, 0.75f },
			new float[5] { 0.25f, 0.375f, 0.75f, 1.125f, 1.375f },
			new float[1] { 1f }
		},
		new float[7][]
		{
			new float[1] { 0.75f },
			new float[1] { 0.75f },
			new float[1] { 0.875f },
			new float[1] { 1.25f },
			new float[1] { 0.75f },
			new float[1] { 0.75f },
			new float[3] { 1f, 1.375f, 2f }
		}
	};

	private static float[][] soldierAttackTimingFrame = new float[5][]
	{
		new float[2] { 0.75f, 0.75f },
		new float[2] { 0.375f, 0.375f },
		new float[2] { 0.75f, 0.75f },
		new float[2] { 1.375f, 1.375f },
		new float[2] { 0.875f, 0.875f }
	};

	private static float[][] monsterAttackTimingFrame = new float[17][]
	{
		new float[4] { 0.625f, 0.625f, 0.625f, 0.625f },
		new float[4] { 0.375f, 0.375f, 0.375f, 0.375f },
		new float[4] { 0.625f, 0.625f, 0.625f, 0.625f },
		new float[4] { 0.75f, 0.75f, 0.75f, 0.75f },
		new float[4] { 0.625f, 0.625f, 0.5f, 0.625f },
		new float[4] { 0.875f, 0.875f, 0.75f, 0.875f },
		new float[4] { 1f, 1f, 0.75f, 1f },
		new float[4] { 0.5f, 0.5f, 0.5f, 0.5f },
		new float[4] { 0.375f, 0.375f, 0.375f, 0.375f },
		new float[4] { 1f, 1f, 1f, 1f },
		new float[4] { 0.75f, 0.75f, 1f, 1f },
		new float[4] { 1.625f, 1.625f, 1.75f, 1.625f },
		new float[4] { 0.5f, 0.5f, 0.625f, 0.5f },
		new float[4] { 1.875f, 1.875f, 1.875f, 1.875f },
		new float[4] { 0.5f, 0.5f, 1.375f, 1.375f },
		new float[4] { 0.875f, 0.875f, 1f, 0.75f },
		new float[4] { 0.75f, 0.75f, 1f, 1f }
	};

	private static float[][] lordAttackTimingFrame = new float[3][]
	{
		new float[1] { 0.25f },
		new float[2] { 0.37f, 0.38f },
		new float[1] { 0.75f }
	};

	public static float[] GetAttackTimingSec(UnitCharactor.CharactorType charType, int charIdx, UnitCharactor.WeaponType weapon, UnitCharactor.AttackMode attackMode, int attackIdx)
	{
		float[] array = new float[1];
		int num = (int)(weapon - 1);
		switch (charType)
		{
		case UnitCharactor.CharactorType.hero:
		{
			int num3 = attackIdx;
			if (attackMode == UnitCharactor.AttackMode.skill)
			{
				num3 += 2;
			}
			array = heroAttackTimingFrame[num][num3].Clone() as float[];
			break;
		}
		case UnitCharactor.CharactorType.soldier:
			array[0] = soldierAttackTimingFrame[charIdx][attackIdx];
			break;
		case UnitCharactor.CharactorType.lord:
			array = lordAttackTimingFrame[charIdx];
			break;
		case UnitCharactor.CharactorType.monster:
		{
			int num2 = attackIdx;
			if (attackMode == UnitCharactor.AttackMode.special)
			{
				num2 += 2;
			}
			array[0] = monsterAttackTimingFrame[charIdx][num2];
			break;
		}
		}
		return array;
	}
}
