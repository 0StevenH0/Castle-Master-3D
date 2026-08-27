using System.Collections.Generic;
using UnityEngine;

public class QuestManager
{
	public enum QuestType
	{
		appoint_lord = 0,
		upgrade_castle = 1,
		loyalty_castle = 2,
		hero_stat = 3,
		buy_item = 4,
		training_unit = 5,
		capture_castle = 6,
		master_skill = 7,
		total_lord = 8,
		hero_level = 9,
		heart_daughter = 10,
		max = 11
	}

	public enum StatKind
	{
		STR = 0,
		INT = 1,
		CON = 2,
		max = 3
	}

	public class Quest
	{
		public QuestType type;

		public int heroLevel = 1;

		public int conA;

		public int conB;

		public int period;

		public int rewardXp;

		public int rewardGem;

		public int rewardGold;
	}

	public const int resultNone = 0;

	public const int resultWaiting = 1;

	public const int resultPlaying = 2;

	public const int resultSucceed = 3;

	public static Quest[] list;

	private int lastIndex = -1;

	private int curIndex = -1;

	public int remaining;

	public Quest curQuest;

	public int result;

	[System.Serializable]
	private class QuestRow
	{
		public string type;

		public int heroLevel;

		public int conA;

		public int conB;

		public int period;

		public int rewardXp;

		public int rewardGem;

		public int rewardGold;
	}

	[System.Serializable]
	private class QuestRowList
	{
		public QuestRow[] items;
	}

	public static void LoadDefault()
	{
		TextAsset textAsset = ResourceManager.Load("GameData", "quest_list", typeof(TextAsset)) as TextAsset;
		if (textAsset == null || string.IsNullOrEmpty(textAsset.text))
		{
			Debug.LogError("Decrypt Error!!");
			return;
		}
		QuestRowList questRowList = JsonUtility.FromJson<QuestRowList>("{\"items\":" + textAsset.text + "}");
		List<Quest> list = new List<Quest>();
		QuestRow[] items = questRowList.items;
		foreach (QuestRow row in items)
		{
			Quest quest = new Quest();
			for (int i = 0; i < 11; i++)
			{
				if (row.type.Equals(((QuestType)i).ToString()))
				{
					quest.type = (QuestType)i;
					break;
				}
			}
			quest.heroLevel = row.heroLevel;
			quest.conA = row.conA;
			quest.conB = row.conB;
			quest.period = row.period;
			quest.rewardXp = row.rewardXp;
			quest.rewardGem = row.rewardGem;
			quest.rewardGold = row.rewardGold;
			list.Add(quest);
		}
		QuestManager.list = list.ToArray();
	}

	public void Init()
	{
		lastIndex = -1;
		curIndex = -1;
		curQuest = null;
		remaining = 0;
		result = 0;
	}

	public void Save()
	{
		DataRegistry.KeyData parent = DataRegistry.Set(null, "Quest", string.Empty);
		DataRegistry.Set(parent, "lastIndex", lastIndex);
		DataRegistry.Set(parent, "curIndex", curIndex);
		DataRegistry.Set(parent, "result", result);
		DataRegistry.Set(parent, "remaining", remaining);
	}

	public void Load()
	{
		DataRegistry.KeyData current = null;
		string keyvalue = string.Empty;
		if (DataRegistry.Get(null, "Quest", ref keyvalue, ref current))
		{
			DataRegistry.KeyData current2 = null;
			DataRegistry.Get(current, "lastIndex", ref lastIndex, ref current2);
			DataRegistry.Get(current, "curIndex", ref curIndex, ref current2);
			DataRegistry.Get(current, "result", ref result, ref current2);
			DataRegistry.Get(current, "remaining", ref remaining, ref current2);
			if (curIndex >= 0)
			{
				curQuest = list[curIndex];
			}
			else
			{
				curQuest = null;
			}
		}
	}

	public bool CheckAchieve(Quest quest)
	{
		if (quest == null)
		{
			return false;
		}
		switch (quest.type)
		{
		case QuestType.appoint_lord:
			if (quest.conA >= 0 && quest.conA < PlayInfo.castleManager.castle.Length)
			{
				CastleInfo castleInfo3 = PlayInfo.castleManager.castle[quest.conA];
				if (castleInfo3.side == 0 && castleInfo3.lord != null)
				{
					return true;
				}
			}
			break;
		case QuestType.upgrade_castle:
			if (quest.conA >= 0 && quest.conA < PlayInfo.castleManager.castle.Length)
			{
				CastleInfo castleInfo2 = PlayInfo.castleManager.castle[quest.conA];
				if (castleInfo2.side == 0 && castleInfo2.level >= quest.conB)
				{
					return true;
				}
			}
			break;
		case QuestType.loyalty_castle:
			if (quest.conA >= 0 && quest.conA < PlayInfo.castleManager.castle.Length)
			{
				CastleInfo castleInfo = PlayInfo.castleManager.castle[quest.conA];
				if (castleInfo.side == 0 && castleInfo.GetTotalLoyalty() >= quest.conB)
				{
					return true;
				}
			}
			break;
		case QuestType.hero_stat:
		{
			int conA = quest.conA;
			if (conA < 0 || conA >= 3)
			{
				break;
			}
			StatKind statKind = (StatKind)conA;
			int conB = quest.conB;
			switch (statKind)
			{
			case StatKind.STR:
				if (PlayInfo.heroState.sumStr >= conB)
				{
					return true;
				}
				break;
			case StatKind.INT:
				if (PlayInfo.heroState.sumInt >= conB)
				{
					return true;
				}
				break;
			case StatKind.CON:
				if (PlayInfo.heroState.sumCon >= conB)
				{
					return true;
				}
				break;
			}
			break;
		}
		case QuestType.buy_item:
		{
			UnitItem unitItem = PlayInfo.inventory.FindItem(quest.conA);
			if (unitItem != null)
			{
				return true;
			}
			break;
		}
		case QuestType.training_unit:
		{
			UnitState unitStateFromCode = PlayInfo.humanMilitary.GetUnitStateFromCode(quest.conA);
			if (unitStateFromCode != null && unitStateFromCode.level >= quest.conB)
			{
				return true;
			}
			break;
		}
		case QuestType.capture_castle:
			if (quest.conA >= 0 && quest.conA < PlayInfo.castleManager.castle.Length)
			{
				CastleInfo castleInfo4 = PlayInfo.castleManager.castle[quest.conA];
				if (castleInfo4.side == 0)
				{
					return true;
				}
			}
			break;
		case QuestType.master_skill:
			if (quest.conA >= 0 && quest.conA < PlayInfo.playerData.skill.skillLevel.Length && PlayInfo.playerData.skill.skillLevel[quest.conA] >= quest.conB)
			{
				return true;
			}
			break;
		case QuestType.total_lord:
			if (PlayInfo.lordManager.list.Count >= quest.conA)
			{
				return true;
			}
			break;
		case QuestType.heart_daughter:
			if (PlayInfo.playerData.countHeart >= quest.conA)
			{
				return true;
			}
			break;
		case QuestType.hero_level:
			if (PlayInfo.heroState.level >= quest.conA)
			{
				return true;
			}
			break;
		}
		return false;
	}

	private void SearchNextQuest()
	{
		int level = PlayInfo.heroState.level;
		while (true)
		{
			int num = lastIndex + 1;
			if (num < list.Length && level >= list[num].heroLevel)
			{
				bool flag = CheckAchieve(list[num]);
				lastIndex = num;
				if (!flag)
				{
					curIndex = num;
					curQuest = list[curIndex];
					remaining = curQuest.period;
					result = 1;
					break;
				}
				continue;
			}
			break;
		}
	}

	public void ProcessDaily(bool incDay)
	{
		if (result == 0)
		{
			SearchNextQuest();
			return;
		}
		bool flag = false;
		if (curQuest != null)
		{
			flag = CheckAchieve(curQuest);
		}
		if (result == 1)
		{
			if (flag)
			{
				SearchNextQuest();
			}
		}
		else if (result == 2)
		{
			if (incDay && remaining > 0)
			{
				remaining--;
			}
			if (flag)
			{
				result = 3;
				PlayInfo.messageManager.Add(0, PlayMessage.MessagLevel.warning, StringContent.msgQuestSysNotifySucceed);
			}
			else if (remaining <= 0)
			{
				ClearQuest();
				PlayInfo.messageManager.Add(0, PlayMessage.MessagLevel.warning, StringContent.msgQuestSysFailed);
			}
		}
	}

	private void ClearQuest()
	{
		curQuest = null;
		curIndex = -1;
		result = 0;
	}

	public void DenyCurrentQuest()
	{
		ClearQuest();
	}

	public bool AcceptCurrentQuest()
	{
		if (curQuest == null)
		{
			return false;
		}
		if (result != 1)
		{
			return false;
		}
		if (curIndex < 0)
		{
			return false;
		}
		remaining = curQuest.period;
		result = 2;
		return true;
	}

	public bool RewardCurrentQuest(out bool isLevelUp)
	{
		isLevelUp = false;
		if (curQuest == null)
		{
			return false;
		}
		if (result != 3)
		{
			return false;
		}
		bool isLevelUp2 = false;
		PlayInfo.IncUnitExp(PlayInfo.heroState, curQuest.rewardXp, out isLevelUp2);
		PlayInfo.playerData.gem += curQuest.rewardGem;
		PlayInfo.playerData.gold += curQuest.rewardGold;
		ClearQuest();
		isLevelUp = isLevelUp2;
		return true;
	}
}
