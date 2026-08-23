using System.IO;
using UnityEngine;

public class GameRule
{
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
		bool succeed = false;
		string s = DataSecurity.Decrypt(textAsset.text, "surkwjch", out succeed);
		if (!succeed)
		{
			Debug.LogError("Decrypt Error!!");
			return;
		}
		StringReader stringReader = new StringReader(s);
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
				string text2 = array[0].ToLower().Trim();
				string s2 = array[1];
				if (text2.Equals("spyactivedays"))
				{
					spyActiveDays = float.Parse(s2);
				}
				else if (text2.Equals("spycostgold"))
				{
					spyCostGold = int.Parse(s2);
				}
				else if (text2.Equals("redeploysoldierdays"))
				{
					redeploySoldierDays = int.Parse(s2);
				}
				else if (text2.Equals("recruitsoldierdays"))
				{
					recruitSoldierDays = int.Parse(s2);
				}
				else if (text2.Equals("taxdays"))
				{
					taxDays = int.Parse(s2);
				}
				else if (text2.Equals("fortunedays"))
				{
					fortuneDays = int.Parse(s2);
				}
				else if (text2.Equals("autoinccmdpts"))
				{
					autoIncCmdPts = int.Parse(s2);
				}
				else if (text2.Equals("redeploygold"))
				{
					redeployGold = int.Parse(s2);
				}
				else if (text2.Equals("redeployinstantlygem"))
				{
					redeployInstantlyGem = int.Parse(s2);
				}
				else if (text2.Equals("battletimelimit"))
				{
					battleTimeLimit = int.Parse(s2);
				}
				else if (text2.Equals("recruitinstantlygem"))
				{
					recruitInstantlyGem = int.Parse(s2);
				}
				else if (text2.Equals("lorddefaultloyalty"))
				{
					lordDefaultLoyalty = int.Parse(s2);
				}
				else if (text2.Equals("lordfireloyalty"))
				{
					lordFireLoyalty = int.Parse(s2);
				}
				else if (text2.Equals("lordloyaltydowndays"))
				{
					lordLoyaltyDownDays = int.Parse(s2);
				}
				else if (text2.Equals("lordloyaltydown"))
				{
					lordLoyaltyDown = int.Parse(s2);
				}
				else if (text2.Equals("loyaltydown"))
				{
					loyaltyDown = int.Parse(s2);
				}
				else if (text2.Equals("loyaltydowndays"))
				{
					loyaltyDownDays = int.Parse(s2);
				}
				else if (text2.Equals("loyaltydownalert"))
				{
					loyaltyDownAlert = int.Parse(s2);
				}
			}
		}
		textAsset = ResourceManager.Load("GameData", "command_point", typeof(TextAsset)) as TextAsset;
		succeed = false;
		s = DataSecurity.Decrypt(textAsset.text, "surkwjch", out succeed);
		if (!succeed)
		{
			Debug.LogError("Decrypt Error!!");
			return;
		}
		stringReader = new StringReader(s);
		while ((text = stringReader.ReadLine()) != null)
		{
			if (text.Trim().Length == 0)
			{
				continue;
			}
			char[] separator2 = new char[1] { '\t' };
			string[] array2 = text.Split(separator2);
			if (array2.Length > 1)
			{
				int num = int.Parse(array2[0]);
				int num2 = int.Parse(array2[2]);
				switch (num)
				{
				case 0:
					cmdPtsAttack = num2;
					break;
				case 1:
					cmdPtsTraining = num2;
					break;
				case 2:
					cmdPtsRedeploy = num2;
					break;
				case 3:
					cmdPtsSearchLord = num2;
					break;
				case 4:
					cmdPtsManageLord = num2;
					break;
				case 5:
					cmdPtsRecruitSoldier = num2;
					break;
				case 6:
					cmdPtsConstruct = num2;
					break;
				case 7:
					cmdPtsAppointLord = num2;
					break;
				case 8:
					cmdPtsSpy = num2;
					break;
				case 9:
					cmdPtsUpgradeCastle = num2;
					break;
				}
			}
		}
	}
}
