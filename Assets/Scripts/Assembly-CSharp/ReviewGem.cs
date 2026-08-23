using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ReviewGem
{
	public class RewardGem
	{
		public int heroLevel;

		public int gem;
	}

	public static RewardGem[] rewardList;

	public static void LoadDefault()
	{
		TextAsset textAsset = ResourceManager.Load("GameData", "review_gem", typeof(TextAsset)) as TextAsset;
		bool succeed = false;
		string s = DataSecurity.Decrypt(textAsset.text, "surkwjch", out succeed);
		if (!succeed)
		{
			Debug.LogError("Decrypt Error!!");
			return;
		}
		StringReader stringReader = new StringReader(s);
		List<RewardGem> list = new List<RewardGem>();
		string empty = string.Empty;
		while ((empty = stringReader.ReadLine()) != null)
		{
			if (empty.Trim().Length != 0)
			{
				char[] separator = new char[1] { '\t' };
				string[] array = empty.Split(separator);
				if (array.Length > 1)
				{
					RewardGem rewardGem = new RewardGem();
					rewardGem.heroLevel = int.Parse(array[1]);
					rewardGem.gem = int.Parse(array[2]);
					list.Add(rewardGem);
				}
			}
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
