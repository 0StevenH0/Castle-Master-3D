using UnityEngine;

public class GameRule
{
	[System.Serializable]
	private class GameRuleJson
	{
		public float SpyActiveDays = 3f;

		public int SpyCostGold = 300;

		public float RedeploySoldierDays = 10f;

		public float RecruitSoldierDays = 10f;

		public float TaxDays = 30f;

		public float FortuneDays = 30f;

		public int AutoIncCmdPts = 10;

		public int RedeployGold = 1000;

		public int RedeployInstantlyGem = 3;

		public int BattleTimeLimit = 300;

		public int RecruitInstantlyGem = 2;

		public int LordDefaultLoyalty = 60;

		public int LordFireLoyalty = 40;

		public int LordLoyaltyDownDays = 30;

		public int LordLoyaltyDown = 5;

		public int LoyaltyDown = 2;

		public int LoyaltyDownDays = 5;

		public int LoyaltyDownAlert = 20;
	}

	[System.Serializable]
	private class CommandPointEntry
	{
		public int Index;

		public string Name;

		public int Points;
	}

	[System.Serializable]
	private class CommandPointJson
	{
		public CommandPointEntry[] items;
	}

	public const int maxUnitPerBattle = 300;

	public const int maxLoyalty = 100;

	public int cmdPtsAttack = 1;

	public int cmdPtsTraining = 1;

	public int cmdPtsRedeploy = 1;

	public int cmdPtsSearchLord = 1;

	public int cmdPtsManageLord = 1;

	public int cmdPtsRecruitSoldier = 1;

	public int cmdPtsConstruct = 1;

	public int cmdPtsAppointLord = 1;

	public int cmdPtsSpy = 1;

	public int cmdPtsUpgradeCastle = 1;

	public float spyActiveDays = 3f;

	public int spyCostGold = 300;

	public float redeploySoldierDays = 10f;

	public float recruitSoldierDays = 10f;

	public float taxDays = 30f;

	public float fortuneDays = 30f;

	public int autoIncCmdPts = 10;

	public int redeployGold = 1000;

	public int redeployInstantlyGem = 3;

	public int battleTimeLimit = 300;

	public int recruitInstantlyGem = 2;

	public int lordDefaultLoyalty = 60;

	public int lordFireLoyalty = 40;

	public int lordLoyaltyDownDays = 30;

	public int lordLoyaltyDown = 5;

	public int loyaltyDown = 2;

	public int loyaltyDownDays = 5;

	public int loyaltyDownAlert = 20;

	public void LoadDefault()
	{
		TextAsset textAsset = ResourceManager.Load("GameData", "game_rule", typeof(TextAsset)) as TextAsset;
		GameRuleJson gameRuleJson = JsonUtility.FromJson<GameRuleJson>(textAsset.text);
		spyActiveDays = gameRuleJson.SpyActiveDays;
		spyCostGold = gameRuleJson.SpyCostGold;
		redeploySoldierDays = gameRuleJson.RedeploySoldierDays;
		recruitSoldierDays = gameRuleJson.RecruitSoldierDays;
		taxDays = gameRuleJson.TaxDays;
		fortuneDays = gameRuleJson.FortuneDays;
		autoIncCmdPts = gameRuleJson.AutoIncCmdPts;
		redeployGold = gameRuleJson.RedeployGold;
		redeployInstantlyGem = gameRuleJson.RedeployInstantlyGem;
		battleTimeLimit = gameRuleJson.BattleTimeLimit;
		recruitInstantlyGem = gameRuleJson.RecruitInstantlyGem;
		lordDefaultLoyalty = gameRuleJson.LordDefaultLoyalty;
		lordFireLoyalty = gameRuleJson.LordFireLoyalty;
		lordLoyaltyDownDays = gameRuleJson.LordLoyaltyDownDays;
		lordLoyaltyDown = gameRuleJson.LordLoyaltyDown;
		loyaltyDown = gameRuleJson.LoyaltyDown;
		loyaltyDownDays = gameRuleJson.LoyaltyDownDays;
		loyaltyDownAlert = gameRuleJson.LoyaltyDownAlert;
		textAsset = ResourceManager.Load("GameData", "command_point", typeof(TextAsset)) as TextAsset;
		CommandPointJson commandPointJson = JsonUtility.FromJson<CommandPointJson>("{\"items\":" + textAsset.text + "}");
		CommandPointEntry[] items = commandPointJson.items;
		foreach (CommandPointEntry commandPointEntry in items)
		{
			int points = commandPointEntry.Points;
			switch (commandPointEntry.Index)
			{
			case 0:
				cmdPtsAttack = points;
				break;
			case 1:
				cmdPtsTraining = points;
				break;
			case 2:
				cmdPtsRedeploy = points;
				break;
			case 3:
				cmdPtsSearchLord = points;
				break;
			case 4:
				cmdPtsManageLord = points;
				break;
			case 5:
				cmdPtsRecruitSoldier = points;
				break;
			case 6:
				cmdPtsConstruct = points;
				break;
			case 7:
				cmdPtsAppointLord = points;
				break;
			case 8:
				cmdPtsSpy = points;
				break;
			case 9:
				cmdPtsUpgradeCastle = points;
				break;
			}
		}
	}
}
