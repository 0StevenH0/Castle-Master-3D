using UnityEngine;

public class ProcMain : MonoBehaviour
{
	public CharactorManager charactorManager;

	public WeaponManager weaponManager;

	public static StageManager.StageType playStage;

	public static bool alreadyInit;

	public static StageManager stageManager;

	private void Start()
	{
		RenderSettings.fog = false;
		InitGame();
		charactorManager.Init();
		weaponManager.Init();
		if (playStage == StageManager.StageType.castle && PlayInfo.castleManager.GetCastleCount(0) == 0)
		{
			PlayInfo.fortuneSystem.CheckFortuneSystem();
			playStage = StageManager.StageType.battle;
			PlayInfo.battleInfo = new PlayInfo.BattleInfo();
			PlayInfo.battleInfo.isAttack = true;
			PlayInfo.battleInfo.attackCastle = PlayInfo.castleManager.castle[0];
			PlayInfo.battleInfo.defenseCastle = PlayInfo.castleManager.castle[0];
			PlayInfo.battleInfo.attackUnits = new int[5];
			PlayInfo.battleInfo.defenseUnits = new int[5];
			PlayInfo.battleInfo.battleFinish = false;
			PlayInfo.battleInfo.battleWin = false;
			for (int i = 0; i < 5; i++)
			{
				PlayInfo.battleInfo.attackUnits[i] = PlayInfo.battleInfo.attackCastle.unitCount[i];
				PlayInfo.battleInfo.defenseUnits[i] = PlayInfo.battleInfo.attackCastle.unitCount[i];
			}
		}
		stageManager = base.gameObject.AddComponent<StageManager>();
		stageManager.charactorManager = charactorManager;
		stageManager.weaponManager = weaponManager;
		stageManager.GenStage(playStage);
	}

	public static void InitGame()
	{
		UserSetting.Init();
		if (!alreadyInit)
		{
			Application.runInBackground = true;
			InitString();
			BoobyTrap.LoadDefault();
			CastleTrap.LoadDefault();
			PlayInfo.Init();
			PlayInfo.Load();
			alreadyInit = true;
		}
	}

	public static void InitString()
	{
		StringContent.Init(UserSetting.language);
		LoadingContent.Init(UserSetting.language);
		UILoveGame.Init();
	}
}
