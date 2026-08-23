public class UnitUpgrade
{
	public bool[] unitUpgrading = new bool[5];

	public float[] unitUpgradeHour = new float[5];

	public void Init()
	{
		for (int i = 0; i < 5; i++)
		{
			unitUpgrading[i] = false;
			unitUpgradeHour[i] = 0f;
		}
	}

	public void Save()
	{
		DataRegistry.KeyData parent = DataRegistry.Set(null, "UnitUpgrade", string.Empty);
		DataRegistry.Set(parent, "unitUpgrading", unitUpgrading);
		DataRegistry.Set(parent, "unitUpgradeHour", unitUpgradeHour);
	}

	public void Load()
	{
		string keyvalue = string.Empty;
		DataRegistry.KeyData current = null;
		if (DataRegistry.Get(null, "UnitUpgrade", ref keyvalue, ref current))
		{
			DataRegistry.KeyData current2 = null;
			DataRegistry.Get(current, "unitUpgrading", ref unitUpgrading, ref current2);
			DataRegistry.Get(current, "unitUpgradeHour", ref unitUpgradeHour, ref current2);
		}
	}

	public void CheckUpgrade()
	{
		for (int i = 0; i < 5; i++)
		{
			if (unitUpgrading[i])
			{
				unitUpgradeHour[i] -= 1f;
				if (unitUpgradeHour[i] <= 0f)
				{
					FinishUpgrade(i);
				}
			}
		}
	}

	public void StartUpgrade(int index)
	{
		unitUpgrading[index] = true;
		unitUpgradeHour[index] = (float)PlayInfo.humanMilitary.GetUnitState(index).upgradeDays * 24f;
	}

	public void FinishUpgrade(int index)
	{
		if (unitUpgrading[index])
		{
			unitUpgrading[index] = false;
			unitUpgradeHour[index] = 0f;
			UnitState unitState = PlayInfo.humanMilitary.GetUnitState(index);
			if (unitState.level < 50)
			{
				unitState.level++;
			}
		}
	}
}
