using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FortuneSystem
{
	public class FortuneCardMix
	{
		public float goodRate;

		public int dayFrom;

		public int dayTo;

		public int[] stepLevel = new int[3];

		public float[] stepRate = new float[3];
	}

	public enum FortuneType
	{
		good = 0,
		bad = 1
	}

	public enum FortuneCastle
	{
		all = 0,
		random = 1,
		none = 2
	}

	public class FortuneEvent
	{
		public FortuneType fortuneType;

		public string fortuneName;

		public float rate;

		public FortuneCastle fortuneCastle;

		public int rewardLoyalty;

		public int rewardGold;

		public int rewardGem;

		public int rewardFame;

		public int rewardResidents;
	}

	public const int evBumperyear = 0;

	public const int evMonument = 1;

	public const int evDonation = 2;

	public const int evMigrant = 3;

	public const int evGems = 4;

	public const int evDrought = 5;

	public const int evThief = 6;

	public const int evTyphoon = 7;

	public const int evForestfire = 8;

	public const int evEarthquake = 9;

	public const int evInfectiousdisease = 10;

	public const int evRiot = 11;

	public const int evEscape = 12;

	public const int maxFortuneCardMixStep = 3;

	public bool takeFortuneActive;

	public bool processFortuneActive;

	public bool eventFortuneActive;

	public float nextEventDays;

	public FortuneEvent currentEventType;

	public FortuneCardMix currentCardMixType;

	public int currentEventIndex;

	public int currentCardMixIndex;

	public int currentTargetCastleIdx;

	public static FortuneCardMix[] fortuneCardMix;

	public static FortuneEvent[] fortuneEvent;

	public static void Init()
	{
		LoadFortuneCardMix();
		LoadFortuneEvent();
	}

	private static void LoadFortuneCardMix()
	{
		TextAsset textAsset = ResourceManager.Load("GameData", "fortune_cardmix", typeof(TextAsset)) as TextAsset;
		bool succeed = false;
		string s = DataSecurity.Decrypt(textAsset.text, "surkwjch", out succeed);
		if (!succeed)
		{
			Debug.LogError("Decrypt Error!!");
			return;
		}
		StringReader stringReader = new StringReader(s);
		List<FortuneCardMix> list = new List<FortuneCardMix>();
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
				FortuneCardMix fortuneCardMix = new FortuneCardMix();
				fortuneCardMix.goodRate = float.Parse(array[1]);
				fortuneCardMix.dayFrom = int.Parse(array[2]);
				fortuneCardMix.dayTo = int.Parse(array[3]);
				for (int i = 0; i < 3; i++)
				{
					fortuneCardMix.stepLevel[i] = int.Parse(array[4 + i * 2]);
					fortuneCardMix.stepRate[i] = float.Parse(array[5 + i * 2]);
				}
				list.Add(fortuneCardMix);
			}
		}
		FortuneSystem.fortuneCardMix = list.ToArray();
	}

	private static void LoadFortuneEvent()
	{
		TextAsset textAsset = ResourceManager.Load("GameData", "fortune_event", typeof(TextAsset)) as TextAsset;
		bool succeed = false;
		string s = DataSecurity.Decrypt(textAsset.text, "surkwjch", out succeed);
		if (!succeed)
		{
			Debug.LogError("Decrypt Error!!");
			return;
		}
		StringReader stringReader = new StringReader(s);
		List<FortuneEvent> list = new List<FortuneEvent>();
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
				FortuneEvent fortuneEvent = new FortuneEvent();
				fortuneEvent.fortuneType = ((!array[0].Trim().Equals("good")) ? FortuneType.bad : FortuneType.good);
				fortuneEvent.fortuneName = array[1];
				fortuneEvent.rate = float.Parse(array[2]);
				string text2 = array[3].Trim();
				if (text2.Equals("all"))
				{
					fortuneEvent.fortuneCastle = FortuneCastle.all;
				}
				else if (text2.Equals("random"))
				{
					fortuneEvent.fortuneCastle = FortuneCastle.random;
				}
				else if (text2.Equals("none"))
				{
					fortuneEvent.fortuneCastle = FortuneCastle.none;
				}
				fortuneEvent.rewardLoyalty = int.Parse(array[4]);
				fortuneEvent.rewardGold = int.Parse(array[5]);
				fortuneEvent.rewardGem = int.Parse(array[6]);
				fortuneEvent.rewardFame = int.Parse(array[7]);
				fortuneEvent.rewardResidents = int.Parse(array[8]);
				list.Add(fortuneEvent);
			}
		}
		FortuneSystem.fortuneEvent = list.ToArray();
	}

	public void Save()
	{
		DataRegistry.KeyData parent = DataRegistry.Set(null, "FortuneSystem", string.Empty);
		DataRegistry.Set(parent, "takeFortuneActive", takeFortuneActive);
		DataRegistry.Set(parent, "processFortuneActive", processFortuneActive);
		DataRegistry.Set(parent, "eventFortuneActive", eventFortuneActive);
		DataRegistry.Set(parent, "nextEventDays", nextEventDays);
		DataRegistry.Set(parent, "currentEventIndex", currentEventIndex);
		DataRegistry.Set(parent, "currentCardMixIndex", currentCardMixIndex);
	}

	public void Load()
	{
		string keyvalue = string.Empty;
		DataRegistry.KeyData current = null;
		if (DataRegistry.Get(null, "FortuneSystem", ref keyvalue, ref current))
		{
			DataRegistry.KeyData current2 = null;
			DataRegistry.Get(current, "takeFortuneActive", ref takeFortuneActive, ref current2);
			DataRegistry.Get(current, "processFortuneActive", ref processFortuneActive, ref current2);
			DataRegistry.Get(current, "eventFortuneActive", ref eventFortuneActive, ref current2);
			DataRegistry.Get(current, "nextEventDays", ref nextEventDays, ref current2);
			DataRegistry.Get(current, "currentEventIndex", ref currentEventIndex, ref current2);
			DataRegistry.Get(current, "currentCardMixIndex", ref currentCardMixIndex, ref current2);
			currentEventType = fortuneEvent[currentEventIndex];
			currentCardMixType = fortuneCardMix[currentCardMixIndex];
		}
	}

	public void SetCardMix(int cardMixIndex)
	{
		currentCardMixType = fortuneCardMix[cardMixIndex];
		currentCardMixIndex = cardMixIndex;
		processFortuneActive = true;
		eventFortuneActive = false;
		AppointNextEventHours();
	}

	public void CheckFortuneSystem()
	{
		if ((float)PlayInfo.gameTime.day % PlayInfo.gameRule.fortuneDays == 0f)
		{
			takeFortuneActive = true;
		}
		if (!processFortuneActive)
		{
			return;
		}
		nextEventDays -= 1f;
		if (!(nextEventDays <= 0f))
		{
			return;
		}
		eventFortuneActive = true;
		int num = Random.Range(0, 100);
		FortuneType fortuneType = FortuneType.bad;
		if ((float)num < currentCardMixType.goodRate)
		{
			fortuneType = FortuneType.good;
		}
		List<int> list = new List<int>();
		List<float> list2 = new List<float>();
		for (int i = 0; i < fortuneEvent.Length; i++)
		{
			if (fortuneEvent[i].fortuneType == fortuneType)
			{
				float rate = fortuneEvent[i].rate;
				int item = i;
				list.Add(item);
				list2.Add(rate);
			}
		}
		float num2 = Random.Range(0, 100);
		int num3 = list[0];
		for (int j = 0; j < list.Count; j++)
		{
			num2 -= list2[j];
			if (num2 <= 0f)
			{
				num3 = list[j];
				break;
			}
		}
		currentEventIndex = num3;
		currentEventType = fortuneEvent[num3];
		PlayInfo.heroState.fame += currentEventType.rewardFame;
		PlayInfo.playerData.gold += currentEventType.rewardGold;
		PlayInfo.playerData.gem += currentEventType.rewardGem;
		if (PlayInfo.heroState.fame < 0)
		{
			PlayInfo.heroState.fame = 0;
		}
		if (PlayInfo.playerData.gold < 0)
		{
			PlayInfo.playerData.gold = 0;
		}
		if (PlayInfo.playerData.gem < 0)
		{
			PlayInfo.playerData.gem = 0;
		}
		int num4 = -1;
		string newValue = string.Empty;
		if (currentEventType.fortuneCastle == FortuneCastle.random)
		{
			int num5 = 0;
			CastleInfo[] castle = PlayInfo.castleManager.castle;
			foreach (CastleInfo castleInfo in castle)
			{
				if (castleInfo.side == 0)
				{
					num5++;
				}
			}
			if (num5 <= 0)
			{
				return;
			}
			int num6 = Random.Range(0, num5);
			int num7 = 0;
			CastleInfo[] castle2 = PlayInfo.castleManager.castle;
			foreach (CastleInfo castleInfo2 in castle2)
			{
				if (castleInfo2.side == 0)
				{
					if (num6 <= 0)
					{
						num4 = num7;
						break;
					}
					num6--;
				}
				num7++;
			}
			if (num4 < 0)
			{
				return;
			}
			currentTargetCastleIdx = num4;
			newValue = PlayInfo.castleManager.castle[num4].castleName;
			PlayInfo.castleManager.castle[num4].loyalty += currentEventType.rewardLoyalty;
			PlayInfo.castleManager.castle[num4].population += currentEventType.rewardResidents;
		}
		else if (currentEventType.fortuneCastle == FortuneCastle.all)
		{
			for (int m = 0; m < PlayInfo.castleManager.castle.Length; m++)
			{
				PlayInfo.castleManager.castle[m].loyalty += currentEventType.rewardLoyalty;
				PlayInfo.castleManager.castle[m].population += currentEventType.rewardResidents;
			}
		}
		string text = string.Empty;
		bool flag = currentEventType.fortuneType == FortuneType.good;
		switch (num3)
		{
		case 0:
			text = StringContent.msgFortuneEvent0;
			break;
		case 1:
			text = StringContent.msgFortuneEvent1.Replace(StringContent.strValue, newValue);
			break;
		case 2:
			text = StringContent.msgFortuneEvent2.Replace(StringContent.strValue, newValue);
			break;
		case 3:
			text = StringContent.msgFortuneEvent3.Replace(StringContent.strValue, newValue);
			break;
		case 4:
			text = StringContent.msgFortuneEvent4;
			break;
		case 5:
			text = StringContent.msgFortuneEvent5.Replace(StringContent.strValue, newValue);
			break;
		case 6:
			text = StringContent.msgFortuneEvent6.Replace(StringContent.strValue, newValue);
			break;
		case 7:
			text = StringContent.msgFortuneEvent7.Replace(StringContent.strValue, newValue);
			break;
		case 8:
			text = StringContent.msgFortuneEvent8.Replace(StringContent.strValue, newValue);
			break;
		case 9:
			text = StringContent.msgFortuneEvent9.Replace(StringContent.strValue, newValue);
			break;
		case 10:
			text = StringContent.msgFortuneEvent10.Replace(StringContent.strValue, newValue);
			break;
		case 11:
			text = StringContent.msgFortuneEvent11.Replace(StringContent.strValue, newValue);
			break;
		case 12:
			text = StringContent.msgFortuneEvent12.Replace(StringContent.strValue, newValue);
			break;
		}
		text = text.Replace("\n", " ");
		PlayInfo.messageManager.Add((!flag) ? 7 : 6, PlayMessage.MessagLevel.warning, text);
		AppointNextEventHours();
	}

	private void AppointNextEventHours()
	{
		nextEventDays = Random.Range(currentCardMixType.dayFrom, currentCardMixType.dayTo + 1);
	}
}
