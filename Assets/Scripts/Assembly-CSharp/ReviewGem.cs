using System.Collections.Generic;
using UnityEngine;

public class ReviewGem
{
	public class RewardGem
	{
		public int heroLevel;

		public int gem;
	}

	[System.Serializable]
	private class RewardGemRow
	{
		public int id;

		public int heroLevel;

		public int gem;
	}

	[System.Serializable]
	private class RewardGemRowWrapper
	{
		public RewardGemRow[] items;
	}

	public static RewardGem[] rewardList;

	public static void LoadDefault()
	{
		TextAsset textAsset = ResourceManager.Load("GameData", "review_gem", typeof(TextAsset)) as TextAsset;
		string json = "{\"items\":" + textAsset.text + "}";
		RewardGemRowWrapper rewardGemRowWrapper = JsonUtility.FromJson<RewardGemRowWrapper>(json);
		List<RewardGem> list = new List<RewardGem>();
		RewardGemRow[] items = rewardGemRowWrapper.items;
		foreach (RewardGemRow rewardGemRow in items)
		{
			RewardGem rewardGem = new RewardGem();
			rewardGem.heroLevel = rewardGemRow.heroLevel;
			rewardGem.gem = rewardGemRow.gem;
			list.Add(rewardGem);
		}
		rewardList = list.ToArray();
	}

	public static bool CheckHeroLevel(ref int idx)
	{
		if (PlayInfo.playerData.reviewIndex >= rewardList.Length)
		{
			return false;
		}
		int reviewIndex = PlayInfo.playerData.reviewIndex;
		if (PlayInfo.heroState.level >= rewardList[reviewIndex].heroLevel)
		{
			PlayInfo.playerData.reviewIndex = reviewIndex + 1;
			idx = reviewIndex;
			PlayInfo.Save();
			return true;
		}
		return false;
	}
}
