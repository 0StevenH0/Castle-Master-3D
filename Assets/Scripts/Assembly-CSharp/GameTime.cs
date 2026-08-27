using UnityEngine;

public class GameTime
{
	[System.Serializable]
	private class GameRuleJson
	{
		public float SecondPerDay = 30f;
	}

	public const float hourPerDay = 24f;

	public int day;

	public int hour;

	public float secPerDay = 30f;

	public float secPerHour = 1.25f;

	public bool pause;

	private float passTime;

	public void Init()
	{
		day = 0;
		hour = 0;
		passTime = 0f;
	}

	public void LoadDefault()
	{
		Init();
		TextAsset textAsset = ResourceManager.Load("GameData", "game_rule", typeof(TextAsset)) as TextAsset;
		GameRuleJson gameRuleJson = JsonUtility.FromJson<GameRuleJson>(textAsset.text);
		secPerDay = gameRuleJson.SecondPerDay;
		secPerHour = secPerDay / 24f;
	}

	public void IncTime(float tick, out bool dayChange)
	{
		dayChange = false;
		if (pause)
		{
			return;
		}
		passTime += tick;
		if (passTime >= secPerHour)
		{
			hour++;
			passTime -= secPerHour;
			if ((float)hour >= 24f)
			{
				hour = 0;
				day++;
				dayChange = true;
			}
			CastleInfo[] castle = PlayInfo.castleManager.castle;
			foreach (CastleInfo castleInfo in castle)
			{
				castleInfo.CheckTimeProcess();
			}
			PlayInfo.unitUpgrade.CheckUpgrade();
		}
	}

	public float GetPassDay(int fromDay, int fromHour)
	{
		float num = (float)day + (float)hour / 24f;
		float num2 = (float)fromDay + (float)fromHour / 24f;
		return num - num2;
	}

	public float GetPassHour(int fromDay, int fromHour)
	{
		float num = (float)day + (float)hour / 24f;
		float num2 = (float)fromDay + (float)fromHour / 24f;
		return (num - num2) * 24f;
	}

	public string GetDayString(int day, int hour)
	{
		string text = string.Empty;
		if (day > 1)
		{
			string text2 = text;
			text = text2 + day + StringContent.wordDays + " ";
		}
		else if (day > 0)
		{
			string text2 = text;
			text = text2 + day + StringContent.wordDay + " ";
		}
		if (hour > 1)
		{
			string text2 = text;
			text = text2 + hour + StringContent.wordHours + " ";
		}
		else if (hour > 0)
		{
			string text2 = text;
			text = text2 + hour + StringContent.wordHour + " ";
		}
		return text;
	}

	public string GetDateString(float days, int hour)
	{
		int num = (int)(days / 360f);
		int num2 = (int)((days - (float)(num * 360)) / 30f);
		int num3 = (int)(days - (float)(num * 360) - (float)(num2 * 30));
		num++;
		num2++;
		num3++;
		return string.Format("{0:0}/{1:0}/{2:0} {3:00}:00", num2, num3, num, hour);
	}

	public void Save()
	{
		DataRegistry.KeyData parent = DataRegistry.Set(null, "GameTime", string.Empty);
		DataRegistry.Set(parent, "day", day);
		DataRegistry.Set(parent, "hour", hour);
	}

	public void Load()
	{
		string keyvalue = string.Empty;
		DataRegistry.KeyData current = null;
		if (DataRegistry.Get(null, "GameTime", ref keyvalue, ref current))
		{
			DataRegistry.KeyData current2 = null;
			DataRegistry.Get(current, "day", ref day, ref current2);
			DataRegistry.Get(current, "hour", ref hour, ref current2);
		}
	}
}
