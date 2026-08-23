[System.Serializable]
public class UnitState
{
	public int side;

	public int code;

	public string name = string.Empty;

	public int level = 1;

	public int statsPoint;

	public int rankAttack = 1;

	public int rankHp = 1;

	public int fame;

	public int loyalty;

	public float attack;

	public float defense;

	public float strength;

	public float intellectual;

	public float constitution;

	public float critical;

	public float speed;

	public float baseHp;

	public float baseMp;

	public float curHp;

	public float curMp;

	public float exp;

	public int expSum;

	public int basePrice;

	public int autoBorn;

	public int autoUptime;

	public int upgradeDays;

	public int upgradeInstantlyGem;

	public bool splashAttack;

	public int splashRange;

	public float splashLength;

	public int requiredBldg = -1;

	public int curWeaponSlot;

	[System.NonSerialized]
	public PlayerData playerData;

	public int price
	{
		get
		{
			return basePrice + level;
		}
	}

	public int sumStr
	{
		get
		{
			if (playerData != null)
			{
				float num = 0f;
				if (playerData.equipWeapon[curWeaponSlot] != null)
				{
					num += playerData.equipWeapon[curWeaponSlot].ability.strength;
				}
				UnitItem[] wearCloth = playerData.wearCloth;
				foreach (UnitItem unitItem in wearCloth)
				{
					if (unitItem != null)
					{
						num += unitItem.ability.strength;
					}
				}
				if (playerData.slotRing != null)
				{
					num += playerData.slotRing.ability.strength;
				}
				return (int)(strength + num);
			}
			return (int)strength;
		}
	}

	public int sumAttack
	{
		get
		{
			if (playerData != null)
			{
				float num = 0f;
				if (playerData.equipWeapon[curWeaponSlot] != null)
				{
					num += playerData.equipWeapon[curWeaponSlot].ability.attack;
				}
				UnitItem[] wearCloth = playerData.wearCloth;
				foreach (UnitItem unitItem in wearCloth)
				{
					if (unitItem != null)
					{
						num += unitItem.ability.attack;
					}
				}
				if (playerData.slotRing != null)
				{
					num += playerData.slotRing.ability.attack;
				}
				return (int)(attack + num + (float)sumStr / 2f);
			}
			return (int)(attack + (float)(level * rankAttack) / 5f + (float)level / 2f);
		}
	}

	public int sumDefense
	{
		get
		{
			if (playerData != null)
			{
				float num = 0f;
				if (playerData.equipWeapon[curWeaponSlot] != null)
				{
					num += playerData.equipWeapon[curWeaponSlot].ability.defense;
				}
				UnitItem[] wearCloth = playerData.wearCloth;
				foreach (UnitItem unitItem in wearCloth)
				{
					if (unitItem != null)
					{
						num += unitItem.ability.defense;
					}
				}
				if (playerData.slotRing != null)
				{
					num += playerData.slotRing.ability.defense;
				}
				return (int)(defense + num + (float)sumCon / 4f);
			}
			return (int)defense;
		}
	}

	public int sumHp
	{
		get
		{
			if (playerData != null)
			{
				float num = 0f;
				if (playerData.equipWeapon[curWeaponSlot] != null)
				{
					num += playerData.equipWeapon[curWeaponSlot].ability.hp;
				}
				UnitItem[] wearCloth = playerData.wearCloth;
				foreach (UnitItem unitItem in wearCloth)
				{
					if (unitItem != null)
					{
						num += unitItem.ability.hp;
					}
				}
				if (playerData.slotRing != null)
				{
					num += playerData.slotRing.ability.hp;
				}
				return (int)(baseHp + num + (float)(sumCon * 5));
			}
			return (int)(baseHp + (float)(level * rankHp / 2) + (float)(level / 2));
		}
	}

	public int sumMp
	{
		get
		{
			if (playerData != null)
			{
				float num = 0f;
				if (playerData.equipWeapon[curWeaponSlot] != null)
				{
					num += playerData.equipWeapon[curWeaponSlot].ability.mp;
				}
				UnitItem[] wearCloth = playerData.wearCloth;
				foreach (UnitItem unitItem in wearCloth)
				{
					if (unitItem != null)
					{
						num += unitItem.ability.mp;
					}
				}
				if (playerData.slotRing != null)
				{
					num += playerData.slotRing.ability.mp;
				}
				return (int)(baseMp + num + (float)sumInt);
			}
			return (int)baseMp;
		}
	}

	public int sumCri
	{
		get
		{
			if (playerData != null)
			{
				float num = 0f;
				if (playerData.equipWeapon[curWeaponSlot] != null)
				{
					num += playerData.equipWeapon[curWeaponSlot].ability.critical;
				}
				UnitItem[] wearCloth = playerData.wearCloth;
				foreach (UnitItem unitItem in wearCloth)
				{
					if (unitItem != null)
					{
						num += unitItem.ability.critical;
					}
				}
				if (playerData.slotRing != null)
				{
					num += playerData.slotRing.ability.critical;
				}
				return (int)(critical + num);
			}
			return (int)critical;
		}
	}

	public int sumInt
	{
		get
		{
			if (playerData != null)
			{
				float num = 0f;
				if (playerData.equipWeapon[curWeaponSlot] != null)
				{
					num += playerData.equipWeapon[curWeaponSlot].ability.intellectual;
				}
				UnitItem[] wearCloth = playerData.wearCloth;
				foreach (UnitItem unitItem in wearCloth)
				{
					if (unitItem != null)
					{
						num += unitItem.ability.intellectual;
					}
				}
				if (playerData.slotRing != null)
				{
					num += playerData.slotRing.ability.intellectual;
				}
				return (int)(intellectual + num);
			}
			return (int)intellectual;
		}
	}

	public int sumCon
	{
		get
		{
			if (playerData != null)
			{
				float num = 0f;
				if (playerData.equipWeapon[curWeaponSlot] != null)
				{
					num += playerData.equipWeapon[curWeaponSlot].ability.constitution;
				}
				UnitItem[] wearCloth = playerData.wearCloth;
				foreach (UnitItem unitItem in wearCloth)
				{
					if (unitItem != null)
					{
						num += unitItem.ability.constitution;
					}
				}
				if (playerData.slotRing != null)
				{
					num += playerData.slotRing.ability.constitution;
				}
				return (int)(constitution + num);
			}
			return (int)constitution;
		}
	}

	public UnitState Clone()
	{
		UnitState unitState = new UnitState();
		unitState.side = side;
		unitState.code = code;
		unitState.name = name;
		unitState.level = level;
		unitState.statsPoint = statsPoint;
		unitState.fame = fame;
		unitState.rankAttack = rankAttack;
		unitState.rankHp = rankHp;
		unitState.loyalty = loyalty;
		unitState.attack = attack;
		unitState.defense = defense;
		unitState.strength = strength;
		unitState.intellectual = intellectual;
		unitState.constitution = constitution;
		unitState.critical = critical;
		unitState.speed = speed;
		unitState.baseHp = baseHp;
		unitState.baseMp = baseMp;
		unitState.curHp = curHp;
		unitState.curMp = curMp;
		unitState.exp = exp;
		unitState.expSum = expSum;
		unitState.basePrice = basePrice;
		unitState.autoBorn = autoBorn;
		unitState.autoUptime = autoUptime;
		unitState.upgradeDays = upgradeDays;
		unitState.upgradeInstantlyGem = upgradeInstantlyGem;
		unitState.splashAttack = splashAttack;
		unitState.splashRange = splashRange;
		unitState.splashLength = splashLength;
		unitState.requiredBldg = requiredBldg;
		return unitState;
	}
}
