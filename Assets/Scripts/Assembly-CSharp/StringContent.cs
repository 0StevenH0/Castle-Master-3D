using System.IO;
using UnityEngine;

public class StringContent
{
	private static Language _lang = Language.korean;

	public static string strValue = "{value}";

	public static string strValue2 = "{value2}";

	public static string strValue3 = "{value3}";

	public static string msgConnecting = string.Empty;

	public static string msgDisconnect = string.Empty;

	public static string msgConnectFail = string.Empty;

	public static string msgInvalidPasskey = string.Empty;

	public static string msgMaxUserConnect = string.Empty;

	public static string msgMultiConnect = string.Empty;

	public static string msgNetworkSendError = string.Empty;

	public static string msgChangeLanguage = string.Empty;

	public static string msgWaitForDownload = string.Empty;

	public static string msgDownloadError = string.Empty;

	public static string msgUpdateRanking = string.Empty;

	public static string msgErrorUpdateRanking = string.Empty;

	public static string msgQuitGame = string.Empty;

	public static string msgAgreeNewGame = string.Empty;

	public static string msgComingSoon = string.Empty;

	public static string msgQuestDeletePlayer = string.Empty;

	public static string msgEnterPlayerName = string.Empty;

	public static string msgTitleHeroName = string.Empty;

	public static string msgDescHeroName = string.Empty;

	public static string msgNotEnoughInventory = string.Empty;

	public static string msgSettingChanged = string.Empty;

	public static string msgDownloadRank = string.Empty;

	public static string msgCanNotPurchase = string.Empty;

	public static string msgPaymentWait = string.Empty;

	public static string msgPaymentReGemGold = string.Empty;

	public static string msgPaymentReCmdPts = string.Empty;

	public static string msgPaymentReMaxCmdPts = string.Empty;

	public static string msgAlphaAdTitle = string.Empty;

	public static string msgAlphaAdNotify = string.Empty;

	public static string msgAlphaAdError = string.Empty;

	public static string msgAlphaAdAlready = string.Empty;

	public static string msgAlphaAdNoMore = string.Empty;

	public static string msgSynopsis = string.Empty;

	public static string msgCannotAttack = string.Empty;

	public static string msgNotEnoughGold = string.Empty;

	public static string msgNotEnoughCmdPts = string.Empty;

	public static string msgRecruitExceedMaxLimit = string.Empty;

	public static string msgAlreadyRecruit = string.Empty;

	public static string msgRequireCastleLevel = string.Empty;

	public static string msgRequireBuilding = string.Empty;

	public static string msgNotEnoughGem = string.Empty;

	public static string msgAlreadyTopLevel = string.Empty;

	public static string msgUpgradingAlready = string.Empty;

	public static string msgCollectTax = string.Empty;

	public static string msgRequireHeroLevel = string.Empty;

	public static string msgAlreadyBoughtItem = string.Empty;

	public static string msgRecruitingNow = string.Empty;

	public static string msgUpgradingNow = string.Empty;

	public static string msgAlreadyUpgraded = string.Empty;

	public static string msgUnderConstruction = string.Empty;

	public static string msgAlreadyConstruction = string.Empty;

	public static string msgRedeployExceedMaxLimit = string.Empty;

	public static string msgRedeployingNow = string.Empty;

	public static string msgCannotRedeploy = string.Empty;

	public static string msgRedeployingNowTo = string.Empty;

	public static string msgNotSelectRedeployUnits = string.Empty;

	public static string msgAlreadyRedeploy = string.Empty;

	public static string msgNoSelectAttackUnit = string.Empty;

	public static string msgAttackExceedMaxLimit = string.Empty;

	public static string msgSelectCastleForAttack = string.Empty;

	public static string msgSelectCastleForRedploy = string.Empty;

	public static string msgFortuneStart = string.Empty;

	public static string msgFortuneResultGood = string.Empty;

	public static string msgFortuneResultNormal = string.Empty;

	public static string msgFortuneResultBad = string.Empty;

	public static string msgFortuneResultTerrible = string.Empty;

	public static string msgFortuneEvent0 = string.Empty;

	public static string msgFortuneEvent1 = string.Empty;

	public static string msgFortuneEvent2 = string.Empty;

	public static string msgFortuneEvent3 = string.Empty;

	public static string msgFortuneEvent4 = string.Empty;

	public static string msgFortuneEvent5 = string.Empty;

	public static string msgFortuneEvent6 = string.Empty;

	public static string msgFortuneEvent7 = string.Empty;

	public static string msgFortuneEvent8 = string.Empty;

	public static string msgFortuneEvent9 = string.Empty;

	public static string msgFortuneEvent10 = string.Empty;

	public static string msgFortuneEvent11 = string.Empty;

	public static string msgFortuneEvent12 = string.Empty;

	public static string msgRequiresHeroLevel = string.Empty;

	public static string msgSpyActivate = string.Empty;

	public static string msgRecruitingSoldier = string.Empty;

	public static string msgUpgradingCastle = string.Empty;

	public static string msgBuildingBase = string.Empty;

	public static string msgNotSelectRecruitUnits = string.Empty;

	public static string msgMercyCitizenLoyalty = string.Empty;

	public static string msgCanNotDeleteConsumableItem = string.Empty;

	public static string msgCanNotDeleteEquipItem = string.Empty;

	public static string msgQuestionItemDelete = string.Empty;

	public static string msgRequireBuildingForUpgrade = string.Empty;

	public static string msgRequireCitizenLoyalty = string.Empty;

	public static string msgDoNotEquip = string.Empty;

	public static string msgQuestRewardLord = string.Empty;

	public static string msgNowTraining = string.Empty;

	public static string msgNowSearchLord = string.Empty;

	public static string msgQuestFireLord = string.Empty;

	public static string msgNotAppoint = string.Empty;

	public static string msgLordReward = string.Empty;

	public static string msgQuestAppointLord = string.Empty;

	public static string msgAppointLord = string.Empty;

	public static string msgMaxLimitLoyalty = string.Empty;

	public static string msgMinLoyaltyRunAwayInCastle = string.Empty;

	public static string msgMercyForLordLoyaltyInCastle = string.Empty;

	public static string msgCannotAppointLord = string.Empty;

	public static string msgCannotRewardLord = string.Empty;

	public static string msgDescMercyForLordLoyalty = string.Empty;

	public static string msgDescNeedToSearchLord = string.Empty;

	public static string msgAlreadyMaxLoyalty = string.Empty;

	public static string msgNpcDlgChooseWeapon = string.Empty;

	public static string msgNpcDlgChooseArmor = string.Empty;

	public static string msgNpcDlgChooseItem = string.Empty;

	public static string msgNpcDlgGotoWorldMap = string.Empty;

	public static string msgNpcDlgAchievement = string.Empty;

	public static string msgNpcDlgLearnSkill = string.Empty;

	public static string msgBattleStartDefense = string.Empty;

	public static string msgBattleStartAttack = string.Empty;

	public static string msgBattleDestroyedGate = string.Empty;

	public static string msgBattleTimesUp = string.Empty;

	public static string msgBattleEnemyCleared = string.Empty;

	public static string msgBattleHeroKilled = string.Empty;

	public static string msgLordRunAway = string.Empty;

	public static string msgLordNeedReward = string.Empty;

	public static string msgExceedUnitPerCastleLevel = string.Empty;

	public static string msgCmdPtsShop = string.Empty;

	public static string msgCmdPtsRecharge100 = string.Empty;

	public static string msgCmdPtsUpgrade100 = string.Empty;

	public static string msgCmdPtsRechargeDesc = string.Empty;

	public static string msgCmdPtsUpgradeDesc = string.Empty;

	public static string msgCmdPtsQuestRecharge = string.Empty;

	public static string msgCmdPtsQuestUpgrade = string.Empty;

	public static string msgCmdPtsResultCannotExtand = string.Empty;

	public static string msgCmdPtsResultAlready100 = string.Empty;

	public static string msgGemShop = string.Empty;

	public static string msgGemShopFinishBuyGem = string.Empty;

	public static string msgGemShopFinishBuyGold = string.Empty;

	public static string msgGemShopResultCannotBuy = string.Empty;

	public static string msgCmdPtsFinishRecharge = string.Empty;

	public static string msgCmdPtsFinishUpgrade = string.Empty;

	public static string msgQuestItemSell = string.Empty;

	public static string msgCanNotSellEquipItem = string.Empty;

	public static string msgFinishRecruit = string.Empty;

	public static string msgFinishRedeploy = string.Empty;

	public static string msgFinishBuilding = string.Empty;

	public static string msgFinishTraining = string.Empty;

	public static string msgAttackFromMonster = string.Empty;

	public static string msgBattleCountdown = string.Empty;

	public static string msgFinishCastleUpgrade = string.Empty;

	public static string msgFirstNeedDefense = string.Empty;

	public static string msgLoveGameFirstClick = string.Empty;

	public static string msgLoveGameGoodAnswer = string.Empty;

	public static string msgLoveGameBadAnswer = string.Empty;

	public static string msgLoveGameGetHeart = string.Empty;

	public static string msgLoveGameNoQuest = string.Empty;

	public static string msgLoveGameSuccess = string.Empty;

	public static string msgLoveGameNoMore = string.Empty;

	public static string msgLoveGameGiveGem = string.Empty;

	public static string msgNpcQuestCitizenLoyalty = string.Empty;

	public static string msgNpcRiseCitizenLoyalty = string.Empty;

	public static string msgNpcQuestLordLoyalty = string.Empty;

	public static string msgNpcRiseLordLoyalty = string.Empty;

	public static string msgEndingSuccess1 = string.Empty;

	public static string msgEndingSuccess2 = string.Empty;

	public static string msgEndingSuccess3 = string.Empty;

	public static string msgEndingSuccess4 = string.Empty;

	public static string msgEndingFail1 = string.Empty;

	public static string msgEndingFail2 = string.Empty;

	public static string msgSendSpy = string.Empty;

	public static string msgChooseStats = string.Empty;

	public static string msgQuestSysNone = string.Empty;

	public static string msgQuestSysExcute = string.Empty;

	public static string msgQuestSysSucceed = string.Empty;

	public static string msgQuestSysFailed = string.Empty;

	public static string msgQuestSysAppointLord = string.Empty;

	public static string msgQuestSysUpgradeCastle = string.Empty;

	public static string msgQuestSysLoyaltyCastle = string.Empty;

	public static string msgQuestSysHeroStat = string.Empty;

	public static string msgQuestSysBuyItem = string.Empty;

	public static string msgQuestSysTrainingUnit = string.Empty;

	public static string msgQuestSysCaptureCastle = string.Empty;

	public static string msgQuestSysMasterSkill = string.Empty;

	public static string msgQuestSysTotalLord = string.Empty;

	public static string msgQuestSysHeroLevel = string.Empty;

	public static string msgQuestSysHeartDaughter = string.Empty;

	public static string msgQuestSysNotifySucceed = string.Empty;

	public static string msgReviewGiveGem1 = string.Empty;

	public static string msgReviewGiveGem2 = string.Empty;

	public static string msgReviewGiveGem3 = string.Empty;

	public static string msgFreeChargePoint = string.Empty;

	public static string msgFreeChargeNoPoint = string.Empty;

	public static string msgFreeChargeSucceed = string.Empty;

	public static string msgFreeChargeFailed = string.Empty;

	public static string wordLoyalty = string.Empty;

	public static string wordFame = string.Empty;

	public static string wordGold = string.Empty;

	public static string wordGem = string.Empty;

	public static string wordResidents = string.Empty;

	public static string wordHours = string.Empty;

	public static string wordHour = string.Empty;

	public static string wordDays = string.Empty;

	public static string wordDay = string.Empty;

	public static string wordYears = string.Empty;

	public static string wordYear = string.Empty;

	public static string wordWeaponTrader = string.Empty;

	public static string wordArmorTrader = string.Empty;

	public static string wordItemTrader = string.Empty;

	public static string wordGuard = string.Empty;

	public static string wordCaption = string.Empty;

	public static string wordSkillMaster = string.Empty;

	public static string wordCurrent = string.Empty;

	public static string wordPriest = string.Empty;

	public static string wordSecretary = string.Empty;

	public static string wordDaughter = string.Empty;

	public static string wordRunaway = string.Empty;

	public static string wordPropose = string.Empty;

	public static string wordLater = string.Empty;

	public static string wordClose = string.Empty;

	public static string wordOK = string.Empty;

	public static string wordLove = string.Empty;

	public static string wordYes = string.Empty;

	public static string wordNo = string.Empty;

	public static string wordCost = string.Empty;

	public static string wordMaxUnits = "Max";

	public static string wordCurrentUnits = "Current";

	public static string wordLord = "Lord";

	public static string wordTax = "Tax";

	public static string wordArmy = "Army";

	public static string wordDefense = "Defense";

	public static string wordRequires = "Requires";

	public static string wordBenefit = "Benefit";

	public static string wordRecruit = "Recruit";

	public static string wordCastleLevel = "Castle Lv.";

	public static string wordCmdPts = "Command Point";

	public static string wordMaxSoldiers = "Max Soldiers";

	public static string wordMaxCitizen = "Max Citizen";

	public static string wordCastleDefense = "Castle Defense";

	public static string wordBattleUnits = "Battle Units";

	public static string wordStatsPoint = string.Empty;

	public static string wordAttack = string.Empty;

	public static string wordMaxMP = string.Empty;

	public static string wordMaxHP = string.Empty;

	public static string wordSkillAtk = string.Empty;

	public static string wordDefenseUp = string.Empty;

	public static string wordTimeLimit = string.Empty;

	public static string wordRewards = string.Empty;

	public static string[] wordMonth = new string[12]
	{
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty
	};

	public static string[] wordBuilding = new string[7]
	{
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty
	};

	public static string[] wordSoldier = new string[5]
	{
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty
	};

	public static string helpControl = string.Empty;

	public static string helpCastleDefense = string.Empty;

	public static string helpAllyUnits = string.Empty;

	public static string helpEnemyUnits = string.Empty;

	public static string helpTimeLimit = string.Empty;

	public static string helpChangeWeapon = string.Empty;

	public static string helpSkillAttack = string.Empty;

	public static string helpHPPotion = string.Empty;

	public static string helpMPPotion = string.Empty;

	public static string signDollar = string.Empty;

	public static string signWon = string.Empty;

	public static string[] tutorialDesc = new string[7];

	public static string tutorialGotoMap = string.Empty;

	public static string tutorialGotoTown = string.Empty;

	public static void Init(Language lang)
	{
		_lang = lang;
		TextAsset textAsset = ResourceManager.Load("GameData", "string_" + _lang, typeof(TextAsset)) as TextAsset;
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
			if (array != null && array.Length >= 2)
			{
				string text2 = array[0].Trim();
				string text3 = array[1].Trim();
				text3 = text3.Replace("\\n", "\n");
				switch (text2)
				{
				case "101":
					msgConnecting = text3;
					break;
				case "102":
					msgDisconnect = text3;
					break;
				case "103":
					msgConnectFail = text3;
					break;
				case "104":
					msgInvalidPasskey = text3;
					break;
				case "105":
					msgMaxUserConnect = text3;
					break;
				case "106":
					msgMultiConnect = text3;
					break;
				case "107":
					msgNetworkSendError = text3;
					break;
				case "108":
					msgChangeLanguage = text3;
					break;
				case "109":
					msgWaitForDownload = text3;
					break;
				case "110":
					msgDownloadError = text3;
					break;
				case "111":
					msgUpdateRanking = text3;
					break;
				case "112":
					msgErrorUpdateRanking = text3;
					break;
				case "113":
					msgQuitGame = text3;
					break;
				case "114":
					msgAgreeNewGame = text3;
					break;
				case "115":
					msgComingSoon = text3;
					break;
				case "116":
					msgQuestDeletePlayer = text3;
					break;
				case "117":
					msgEnterPlayerName = text3;
					break;
				case "118":
					msgTitleHeroName = text3;
					break;
				case "119":
					msgDescHeroName = text3;
					break;
				case "120":
					msgNotEnoughInventory = text3;
					break;
				case "121":
					msgSettingChanged = text3;
					break;
				case "122":
					msgDownloadRank = text3;
					break;
				case "123":
					msgCanNotPurchase = text3;
					break;
				case "124":
					msgPaymentWait = text3;
					break;
				case "125":
					msgPaymentReGemGold = text3;
					break;
				case "126":
					msgPaymentReCmdPts = text3;
					break;
				case "127":
					msgPaymentReMaxCmdPts = text3;
					break;
				case "130":
					msgAlphaAdTitle = text3;
					break;
				case "131":
					msgAlphaAdNotify = text3;
					break;
				case "132":
					msgAlphaAdError = text3;
					break;
				case "133":
					msgAlphaAdAlready = text3;
					break;
				case "134":
					msgAlphaAdNoMore = text3;
					break;
				case "150":
					msgSynopsis = text3;
					break;
				case "201":
					msgCannotAttack = text3;
					break;
				case "202":
					msgNotEnoughGold = text3;
					break;
				case "203":
					msgNotEnoughCmdPts = text3;
					break;
				case "204":
					msgRecruitExceedMaxLimit = text3;
					break;
				case "205":
					msgAlreadyRecruit = text3;
					break;
				case "206":
					msgRequireCastleLevel = text3;
					break;
				case "207":
					msgRequireBuilding = text3;
					break;
				case "208":
					msgNotEnoughGem = text3;
					break;
				case "209":
					msgAlreadyTopLevel = text3;
					break;
				case "210":
					msgUpgradingAlready = text3;
					break;
				case "211":
					msgCollectTax = text3;
					break;
				case "212":
					msgRequireHeroLevel = text3;
					break;
				case "213":
					msgAlreadyBoughtItem = text3;
					break;
				case "214":
					msgRecruitingNow = text3;
					break;
				case "215":
					msgUpgradingNow = text3;
					break;
				case "216":
					msgAlreadyUpgraded = text3;
					break;
				case "217":
					msgUnderConstruction = text3;
					break;
				case "218":
					msgAlreadyConstruction = text3;
					break;
				case "219":
					msgRedeployExceedMaxLimit = text3;
					break;
				case "220":
					msgRedeployingNow = text3;
					break;
				case "221":
					msgCannotRedeploy = text3;
					break;
				case "222":
					msgRedeployingNowTo = text3;
					break;
				case "223":
					msgNotSelectRedeployUnits = text3;
					break;
				case "224":
					msgAlreadyRedeploy = text3;
					break;
				case "225":
					msgNoSelectAttackUnit = text3;
					break;
				case "226":
					msgAttackExceedMaxLimit = text3;
					break;
				case "227":
					msgSelectCastleForAttack = text3;
					break;
				case "228":
					msgSelectCastleForRedploy = text3;
					break;
				case "229":
					msgFortuneStart = text3;
					break;
				case "230":
					msgFortuneResultGood = text3;
					break;
				case "231":
					msgFortuneResultNormal = text3;
					break;
				case "232":
					msgFortuneResultBad = text3;
					break;
				case "233":
					msgFortuneResultTerrible = text3;
					break;
				case "250":
					msgFortuneEvent0 = text3;
					break;
				case "251":
					msgFortuneEvent1 = text3;
					break;
				case "252":
					msgFortuneEvent2 = text3;
					break;
				case "253":
					msgFortuneEvent3 = text3;
					break;
				case "254":
					msgFortuneEvent4 = text3;
					break;
				case "255":
					msgFortuneEvent5 = text3;
					break;
				case "256":
					msgFortuneEvent6 = text3;
					break;
				case "257":
					msgFortuneEvent7 = text3;
					break;
				case "258":
					msgFortuneEvent8 = text3;
					break;
				case "259":
					msgFortuneEvent9 = text3;
					break;
				case "260":
					msgFortuneEvent10 = text3;
					break;
				case "261":
					msgFortuneEvent11 = text3;
					break;
				case "262":
					msgFortuneEvent12 = text3;
					break;
				case "263":
					msgRequiresHeroLevel = text3;
					break;
				case "264":
					msgSpyActivate = text3;
					break;
				case "265":
					msgRecruitingSoldier = text3;
					break;
				case "266":
					msgUpgradingCastle = text3;
					break;
				case "267":
					msgBuildingBase = text3;
					break;
				case "268":
					msgNotSelectRecruitUnits = text3;
					break;
				case "269":
					msgMercyCitizenLoyalty = text3;
					break;
				case "270":
					msgCanNotDeleteConsumableItem = text3;
					break;
				case "271":
					msgCanNotDeleteEquipItem = text3;
					break;
				case "272":
					msgQuestionItemDelete = text3;
					break;
				case "273":
					msgRequireBuildingForUpgrade = text3;
					break;
				case "274":
					msgRequireCitizenLoyalty = text3;
					break;
				case "275":
					msgDoNotEquip = text3;
					break;
				case "282":
					msgQuestRewardLord = text3;
					break;
				case "283":
					msgNowTraining = text3;
					break;
				case "284":
					msgNowSearchLord = text3;
					break;
				case "287":
					msgQuestFireLord = text3;
					break;
				case "288":
					msgNotAppoint = text3;
					break;
				case "289":
					msgLordReward = text3;
					break;
				case "290":
					msgQuestAppointLord = text3;
					break;
				case "291":
					msgAppointLord = text3;
					break;
				case "292":
					msgMaxLimitLoyalty = text3;
					break;
				case "295":
					msgMinLoyaltyRunAwayInCastle = text3;
					break;
				case "296":
					msgMercyForLordLoyaltyInCastle = text3;
					break;
				case "297":
					msgCannotAppointLord = text3;
					break;
				case "298":
					msgCannotRewardLord = text3;
					break;
				case "300":
					msgDescMercyForLordLoyalty = text3;
					break;
				case "301":
					msgDescNeedToSearchLord = text3;
					break;
				case "302":
					msgAlreadyMaxLoyalty = text3;
					break;
				case "340":
					msgNpcDlgChooseWeapon = text3;
					break;
				case "341":
					msgNpcDlgChooseArmor = text3;
					break;
				case "342":
					msgNpcDlgChooseItem = text3;
					break;
				case "343":
					msgNpcDlgGotoWorldMap = text3;
					break;
				case "344":
					msgNpcDlgAchievement = text3;
					break;
				case "345":
					msgNpcDlgLearnSkill = text3;
					break;
				case "350":
					msgBattleStartDefense = text3;
					break;
				case "351":
					msgBattleStartAttack = text3;
					break;
				case "352":
					msgBattleDestroyedGate = text3;
					break;
				case "353":
					msgBattleTimesUp = text3;
					break;
				case "354":
					msgBattleEnemyCleared = text3;
					break;
				case "355":
					msgBattleHeroKilled = text3;
					break;
				case "360":
					msgLordRunAway = text3;
					break;
				case "361":
					msgLordNeedReward = text3;
					break;
				case "362":
					msgExceedUnitPerCastleLevel = text3;
					break;
				case "363":
					msgCmdPtsShop = text3;
					break;
				case "365":
					msgCmdPtsRecharge100 = text3;
					break;
				case "366":
					msgCmdPtsUpgrade100 = text3;
					break;
				case "367":
					msgCmdPtsRechargeDesc = text3;
					break;
				case "368":
					msgCmdPtsUpgradeDesc = text3;
					break;
				case "369":
					msgCmdPtsQuestRecharge = text3;
					break;
				case "370":
					msgCmdPtsQuestUpgrade = text3;
					break;
				case "371":
					msgCmdPtsResultCannotExtand = text3;
					break;
				case "372":
					msgCmdPtsResultAlready100 = text3;
					break;
				case "373":
					msgGemShop = text3;
					break;
				case "374":
					msgGemShopFinishBuyGem = text3;
					break;
				case "375":
					msgGemShopFinishBuyGold = text3;
					break;
				case "376":
					msgGemShopResultCannotBuy = text3;
					break;
				case "377":
					msgCmdPtsFinishRecharge = text3;
					break;
				case "378":
					msgCmdPtsFinishUpgrade = text3;
					break;
				case "380":
					msgQuestItemSell = text3;
					break;
				case "381":
					msgCanNotSellEquipItem = text3;
					break;
				case "401":
					msgFinishRecruit = text3;
					break;
				case "402":
					msgFinishRedeploy = text3;
					break;
				case "403":
					msgFinishBuilding = text3;
					break;
				case "404":
					msgFinishTraining = text3;
					break;
				case "405":
					msgAttackFromMonster = text3;
					break;
				case "406":
					msgBattleCountdown = text3;
					break;
				case "407":
					msgFinishCastleUpgrade = text3;
					break;
				case "420":
					msgFirstNeedDefense = text3;
					break;
				case "431":
					msgLoveGameFirstClick = text3;
					break;
				case "432":
					msgLoveGameGoodAnswer = text3;
					break;
				case "433":
					msgLoveGameBadAnswer = text3;
					break;
				case "434":
					msgLoveGameGetHeart = text3;
					break;
				case "435":
					msgLoveGameNoQuest = text3;
					break;
				case "436":
					msgLoveGameSuccess = text3;
					break;
				case "437":
					msgLoveGameNoMore = text3;
					break;
				case "438":
					msgLoveGameGiveGem = text3;
					break;
				case "440":
					msgNpcQuestCitizenLoyalty = text3;
					break;
				case "441":
					msgNpcRiseCitizenLoyalty = text3;
					break;
				case "442":
					msgNpcQuestLordLoyalty = text3;
					break;
				case "443":
					msgNpcRiseLordLoyalty = text3;
					break;
				case "450":
					msgEndingSuccess1 = text3;
					break;
				case "451":
					msgEndingSuccess2 = text3;
					break;
				case "452":
					msgEndingSuccess3 = text3;
					break;
				case "453":
					msgEndingSuccess4 = text3;
					break;
				case "454":
					msgEndingFail1 = text3;
					break;
				case "455":
					msgEndingFail2 = text3;
					break;
				case "460":
					msgSendSpy = text3;
					break;
				case "461":
					msgChooseStats = text3;
					break;
				case "462":
					msgQuestSysNone = text3;
					break;
				case "463":
					msgQuestSysExcute = text3;
					break;
				case "464":
					msgQuestSysSucceed = text3;
					break;
				case "465":
					msgQuestSysFailed = text3;
					break;
				case "470":
					msgQuestSysAppointLord = text3;
					break;
				case "471":
					msgQuestSysUpgradeCastle = text3;
					break;
				case "472":
					msgQuestSysLoyaltyCastle = text3;
					break;
				case "473":
					msgQuestSysHeroStat = text3;
					break;
				case "474":
					msgQuestSysBuyItem = text3;
					break;
				case "475":
					msgQuestSysTrainingUnit = text3;
					break;
				case "476":
					msgQuestSysCaptureCastle = text3;
					break;
				case "477":
					msgQuestSysMasterSkill = text3;
					break;
				case "478":
					msgQuestSysTotalLord = text3;
					break;
				case "479":
					msgQuestSysHeroLevel = text3;
					break;
				case "480":
					msgQuestSysHeartDaughter = text3;
					break;
				case "481":
					msgQuestSysNotifySucceed = text3;
					break;
				case "482":
					msgReviewGiveGem1 = text3;
					break;
				case "483":
					msgReviewGiveGem2 = text3;
					break;
				case "484":
					msgReviewGiveGem3 = text3;
					break;
				case "490":
					msgFreeChargePoint = text3;
					break;
				case "491":
					msgFreeChargeNoPoint = text3;
					break;
				case "492":
					msgFreeChargeSucceed = text3;
					break;
				case "493":
					msgFreeChargeFailed = text3;
					break;
				case "500":
					wordLoyalty = text3;
					break;
				case "501":
					wordFame = text3;
					break;
				case "502":
					wordGold = text3;
					break;
				case "503":
					wordGem = text3;
					break;
				case "504":
					wordResidents = text3;
					break;
				case "505":
					wordHours = text3;
					break;
				case "506":
					wordHour = text3;
					break;
				case "507":
					wordDays = text3;
					break;
				case "508":
					wordDay = text3;
					break;
				case "509":
					wordYears = text3;
					break;
				case "510":
					wordYear = text3;
					break;
				case "520":
					wordWeaponTrader = text3;
					break;
				case "521":
					wordArmorTrader = text3;
					break;
				case "522":
					wordItemTrader = text3;
					break;
				case "523":
					wordGuard = text3;
					break;
				case "524":
					wordCaption = text3;
					break;
				case "525":
					wordSkillMaster = text3;
					break;
				case "526":
					wordCurrent = text3;
					break;
				case "527":
					wordPriest = text3;
					break;
				case "528":
					wordSecretary = text3;
					break;
				case "529":
					wordDaughter = text3;
					break;
				case "540":
					wordRunaway = text3;
					break;
				case "545":
					wordPropose = text3;
					break;
				case "546":
					wordLater = text3;
					break;
				case "547":
					wordClose = text3;
					break;
				case "548":
					wordOK = text3;
					break;
				case "549":
					wordLove = text3;
					break;
				case "550":
					wordYes = text3;
					break;
				case "551":
					wordNo = text3;
					break;
				case "552":
					wordCost = text3;
					break;
				case "570":
					wordMaxUnits = text3;
					break;
				case "571":
					wordCurrentUnits = text3;
					break;
				case "572":
					wordLord = text3;
					break;
				case "573":
					wordTax = text3;
					break;
				case "574":
					wordArmy = text3;
					break;
				case "575":
					wordDefense = text3;
					break;
				case "576":
					wordRequires = text3;
					break;
				case "577":
					wordBenefit = text3;
					break;
				case "578":
					wordRecruit = text3;
					break;
				case "579":
					wordCastleLevel = text3;
					break;
				case "580":
					wordCmdPts = text3;
					break;
				case "581":
					wordMaxSoldiers = text3;
					break;
				case "582":
					wordMaxCitizen = text3;
					break;
				case "583":
					wordCastleDefense = text3;
					break;
				case "584":
					wordBattleUnits = text3;
					break;
				case "585":
					wordStatsPoint = text3;
					break;
				case "586":
					wordAttack = text3;
					break;
				case "587":
					wordMaxMP = text3;
					break;
				case "588":
					wordMaxHP = text3;
					break;
				case "589":
					wordSkillAtk = text3;
					break;
				case "590":
					wordDefenseUp = text3;
					break;
				case "591":
					wordTimeLimit = text3;
					break;
				case "592":
					wordRewards = text3;
					break;
				case "593":
					signDollar = text3;
					break;
				case "594":
					signWon = text3;
					break;
				case "600":
					wordMonth[0] = text3;
					break;
				case "601":
					wordMonth[1] = text3;
					break;
				case "602":
					wordMonth[2] = text3;
					break;
				case "603":
					wordMonth[3] = text3;
					break;
				case "604":
					wordMonth[4] = text3;
					break;
				case "605":
					wordMonth[5] = text3;
					break;
				case "606":
					wordMonth[6] = text3;
					break;
				case "607":
					wordMonth[7] = text3;
					break;
				case "608":
					wordMonth[8] = text3;
					break;
				case "609":
					wordMonth[9] = text3;
					break;
				case "610":
					wordMonth[10] = text3;
					break;
				case "611":
					wordMonth[11] = text3;
					break;
				case "620":
					wordBuilding[0] = text3;
					break;
				case "621":
					wordBuilding[1] = text3;
					break;
				case "622":
					wordBuilding[2] = text3;
					break;
				case "623":
					wordBuilding[3] = text3;
					break;
				case "624":
					wordBuilding[4] = text3;
					break;
				case "625":
					wordBuilding[5] = text3;
					break;
				case "626":
					wordBuilding[6] = text3;
					break;
				case "630":
					wordSoldier[0] = text3;
					break;
				case "631":
					wordSoldier[1] = text3;
					break;
				case "632":
					wordSoldier[2] = text3;
					break;
				case "633":
					wordSoldier[3] = text3;
					break;
				case "634":
					wordSoldier[4] = text3;
					break;
				case "800":
					helpControl = text3;
					break;
				case "801":
					helpCastleDefense = text3;
					break;
				case "802":
					helpAllyUnits = text3;
					break;
				case "803":
					helpEnemyUnits = text3;
					break;
				case "804":
					helpTimeLimit = text3;
					break;
				case "805":
					helpChangeWeapon = text3;
					break;
				case "806":
					helpSkillAttack = text3;
					break;
				case "807":
					helpHPPotion = text3;
					break;
				case "808":
					helpMPPotion = text3;
					break;
				case "821":
					tutorialDesc[0] = text3;
					break;
				case "822":
					tutorialDesc[1] = text3;
					break;
				case "823":
					tutorialDesc[2] = text3;
					break;
				case "824":
					tutorialDesc[3] = text3;
					break;
				case "825":
					tutorialDesc[4] = text3;
					break;
				case "826":
					tutorialDesc[5] = text3;
					break;
				case "827":
					tutorialDesc[6] = text3;
					break;
				case "830":
					tutorialGotoMap = text3;
					break;
				case "831":
					tutorialGotoTown = text3;
					break;
				}
			}
		}
	}
}
