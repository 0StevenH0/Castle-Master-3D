using System.Collections;
using UnityEngine;

public class ProcCastle : ProcBase
{
	public UIGameMenu uiGameMenu;

	public UIInventory uiInventory;

	public UISkill uiSkill;

	public UINpcDialog uiNpcDialog;

	public UIStatus uiStatus;

	public UIShopBuy uiShopBuy;

	public UIShopSell uiShopSell;

	public UISkillMaster uiSkillMaster;

	public UIMap uiMap;

	public UIFortune uiFortune;

	public UIFortuneResult uiFortuneResult;

	public UILord uiLord;

	public UITraining uiTraining;

	public UIMessageNote uiMessageNote;

	public UIPlayMessage uiPlayMessage;

	public UIPlayMenu uiPlayMenu;

	public UIUserSetting uiUserSetting;

	public UITutorial uiTutorial;

	public UIPlayTutorial uiPlayTutorial;

	public UIGemShop uiGemShop;

	public UICmdPtsShop uiCmdPtsShop;

	public UIQuest uiQuest;

	public GameObject questUnit;

	public UILoveGame uiLoveGame;

	public GameObject loveUnit;

	public UINpcAction uiNpcAction;

	public InterfaceControl interControl;

	public CameraControl cameraControl;

	public bool tutorialActive;

	private bool isShowMap = true;

	private float befNpcFov = 60f;

	private int reviewAddGem;

	public AudioSource bgmMap;

	public AudioSource bgmCastle;

	public AudioSource bgmFortune;

	public override void OnStart()
	{
		PlayInfo.gameTime.pause = false;
		GameObject gameObject = null;
		GameObject gameObject2 = null;
		gameObject = ResourceManager.Load("interface/prefabs", "feb_gamemenu", typeof(GameObject)) as GameObject;
		gameObject2 = Object.Instantiate(gameObject) as GameObject;
		AuiButton.SetCameraAllChild(gameObject2.transform, interControl.uiCamera);
		uiGameMenu = gameObject2.GetComponent<UIGameMenu>();
		uiGameMenu.buttonInventory.onButtonClick = OnInventoryClick;
		uiGameMenu.buttonSkill.onButtonClick = OnSkillClick;
		uiGameMenu.buttonMap.onButtonClick = OnShowMapClick;
		uiGameMenu.buttonMenu.onButtonClick = OnMenuClick;
		uiGameMenu.buttonCastle.onButtonClick = OnShowCastleClick;
		gameObject2 = GameObject.Find("feb_inventory");
		if (gameObject2 == null)
		{
			gameObject = ResourceManager.Load("interface/prefabs", "feb_inventory", typeof(GameObject)) as GameObject;
			gameObject2 = Object.Instantiate(gameObject) as GameObject;
		}
		AuiButton.SetCameraAllChild(gameObject2.transform, interControl.uiCamera);
		uiInventory = gameObject2.GetComponent<UIInventory>();
		uiInventory.thisUnit = interControl.ctrlUnit.thisChar;
		gameObject = ResourceManager.Load("interface/prefabs", "feb_skill", typeof(GameObject)) as GameObject;
		gameObject2 = Object.Instantiate(gameObject) as GameObject;
		AuiButton.SetCameraAllChild(gameObject2.transform, interControl.uiCamera);
		uiSkill = gameObject2.GetComponent<UISkill>();
		gameObject = ResourceManager.Load("interface/prefabs", "feb_shopbuy", typeof(GameObject)) as GameObject;
		gameObject2 = Object.Instantiate(gameObject) as GameObject;
		AuiButton.SetCameraAllChild(gameObject2.transform, interControl.uiCamera);
		uiShopBuy = gameObject2.GetComponent<UIShopBuy>();
		uiShopBuy.buttonClose.onButtonClick = OnShopCloseClick;
		uiShopBuy.thisUnit = interControl.ctrlUnit.thisChar;
		uiShopBuy.procCastle = this;
		gameObject = ResourceManager.Load("interface/prefabs", "feb_shopsell", typeof(GameObject)) as GameObject;
		gameObject2 = Object.Instantiate(gameObject) as GameObject;
		AuiButton.SetCameraAllChild(gameObject2.transform, interControl.uiCamera);
		uiShopSell = gameObject2.GetComponent<UIShopSell>();
		uiShopSell.buttonClose.onButtonClick = OnShopCloseClick;
		uiShopSell.uiShopBuy = uiShopBuy;
		uiShopBuy.uiShopSell = uiShopSell;
		gameObject = ResourceManager.Load("interface/prefabs", "feb_skillmaster", typeof(GameObject)) as GameObject;
		gameObject2 = Object.Instantiate(gameObject) as GameObject;
		AuiButton.SetCameraAllChild(gameObject2.transform, interControl.uiCamera);
		uiSkillMaster = gameObject2.GetComponent<UISkillMaster>();
		uiSkillMaster.buttonClose.onButtonClick = OnSkillMasterCloseClick;
		uiSkillMaster.procCastle = this;
		gameObject = ResourceManager.Load("interface/prefabs", "feb_npcdialog", typeof(GameObject)) as GameObject;
		gameObject2 = Object.Instantiate(gameObject) as GameObject;
		AuiButton.SetCameraAllChild(gameObject2.transform, interControl.uiCamera);
		uiNpcDialog = gameObject2.GetComponent<UINpcDialog>();
		gameObject2 = GameObject.Find("feb_training");
		if (gameObject2 == null)
		{
			gameObject = ResourceManager.Load("interface/prefabs", "feb_training", typeof(GameObject)) as GameObject;
			gameObject2 = Object.Instantiate(gameObject) as GameObject;
		}
		AuiButton.SetCameraAllChild(gameObject2.transform, interControl.uiCamera);
		gameObject2.SetActiveRecursive(false);
		uiTraining = gameObject2.GetComponent<UITraining>();
		uiTraining.buttonClose.onButtonClick = OnTrainingCloseClick;
		uiTraining.procCastle = this;
		gameObject2 = GameObject.Find("feb_lordlist");
		if (gameObject2 == null)
		{
			gameObject = ResourceManager.Load("interface/prefabs", "feb_lordlist", typeof(GameObject)) as GameObject;
			gameObject2 = Object.Instantiate(gameObject) as GameObject;
		}
		AuiButton.SetCameraAllChild(gameObject2.transform, interControl.uiCamera);
		gameObject2.SetActiveRecursive(false);
		uiLord = gameObject2.GetComponent<UILord>();
		uiLord.buttonClose.onButtonClick = OnRetainerCloseClick;
		uiLord.procCastle = this;
		gameObject2 = GameObject.Find("feb_messagenote");
		if (gameObject2 == null)
		{
			gameObject = ResourceManager.Load("interface/prefabs", "feb_messagenote", typeof(GameObject)) as GameObject;
			gameObject2 = Object.Instantiate(gameObject) as GameObject;
		}
		AuiButton.SetCameraAllChild(gameObject2.transform, interControl.uiCamera);
		gameObject2.SetActiveRecursive(false);
		uiMessageNote = gameObject2.GetComponent<UIMessageNote>();
		gameObject2 = GameObject.Find("feb_fortune");
		if (gameObject2 == null)
		{
			gameObject = ResourceManager.Load("interface/prefabs", "feb_fortune", typeof(GameObject)) as GameObject;
			gameObject2 = Object.Instantiate(gameObject) as GameObject;
		}
		AuiButton.SetCameraAllChild(gameObject2.transform, interControl.uiCamera);
		gameObject2.SetActiveRecursive(false);
		uiFortune = gameObject2.GetComponent<UIFortune>();
		uiFortune.procCastle = this;
		gameObject2 = GameObject.Find("feb_fortuneresult");
		if (gameObject2 == null)
		{
			gameObject = ResourceManager.Load("interface/prefabs", "feb_fortuneresult", typeof(GameObject)) as GameObject;
			gameObject2 = Object.Instantiate(gameObject) as GameObject;
		}
		AuiButton.SetCameraAllChild(gameObject2.transform, interControl.uiCamera);
		gameObject2.SetActiveRecursive(false);
		uiFortuneResult = gameObject2.GetComponent<UIFortuneResult>();
		gameObject2 = GameObject.Find("feb_playmessage");
		if (gameObject2 == null)
		{
			gameObject = ResourceManager.Load("interface/prefabs", "feb_playmessage", typeof(GameObject)) as GameObject;
			gameObject2 = Object.Instantiate(gameObject) as GameObject;
		}
		uiPlayMessage = gameObject2.GetComponent<UIPlayMessage>();
		gameObject2 = GameObject.Find("feb_playmenu");
		if (gameObject2 == null)
		{
			gameObject = ResourceManager.Load("interface/prefabs", "feb_playmenu", typeof(GameObject)) as GameObject;
			gameObject2 = Object.Instantiate(gameObject) as GameObject;
		}
		AuiButton.SetCameraAllChild(gameObject2.transform, interControl.uiCamera);
		gameObject2.SetActiveRecursive(false);
		uiPlayMenu = gameObject2.GetComponent<UIPlayMenu>();
		uiPlayMenu.procCastle = this;
		gameObject2 = GameObject.Find("feb_usersetting");
		if (gameObject2 == null)
		{
			gameObject = ResourceManager.Load("interface/prefabs", "feb_usersetting", typeof(GameObject)) as GameObject;
			gameObject2 = Object.Instantiate(gameObject) as GameObject;
		}
		AuiButton.SetCameraAllChild(gameObject2.transform, interControl.uiCamera);
		gameObject2.SetActiveRecursive(false);
		uiUserSetting = gameObject2.GetComponent<UIUserSetting>();
		gameObject2 = GameObject.Find("feb_tutorial");
		if (gameObject2 == null)
		{
			gameObject = ResourceManager.Load("interface/prefabs", "feb_tutorial", typeof(GameObject)) as GameObject;
			gameObject2 = Object.Instantiate(gameObject) as GameObject;
		}
		AuiButton.SetCameraAllChild(gameObject2.transform, interControl.uiCamera);
		gameObject2.SetActiveRecursive(false);
		uiTutorial = gameObject2.GetComponent<UITutorial>();
		if (PlayInfo.playerData.tutorialMode)
		{
			gameObject2 = GameObject.Find("feb_playtutorial");
			if (gameObject2 == null)
			{
				gameObject = ResourceManager.Load("interface/prefabs", "feb_playtutorial", typeof(GameObject)) as GameObject;
				gameObject2 = Object.Instantiate(gameObject) as GameObject;
			}
			gameObject2.SetActiveRecursive(false);
			uiPlayTutorial = gameObject2.GetComponent<UIPlayTutorial>();
			uiPlayTutorial.uiCamera = interControl.uiCamera;
			uiPlayTutorial.interCtrl = interControl;
			uiPlayTutorial.procCastle = this;
		}
		gameObject2 = GameObject.Find("feb_status");
		if (gameObject2 == null)
		{
			gameObject = ResourceManager.Load("interface/prefabs", "feb_status", typeof(GameObject)) as GameObject;
			gameObject2 = Object.Instantiate(gameObject) as GameObject;
		}
		uiStatus = gameObject2.GetComponent<UIStatus>();
		AuiButton.SetCameraAllChild(gameObject2.transform, interControl.uiCamera);
		uiStatus.buttonGoCastle.onButtonClick = OnGoBattleCastleClick;
		gameObject2 = GameObject.Find("feb_map");
		if (gameObject2 == null)
		{
			gameObject = ResourceManager.Load("interface/prefabs", "feb_map", typeof(GameObject)) as GameObject;
			gameObject2 = Object.Instantiate(gameObject) as GameObject;
		}
		uiMap = gameObject2.GetComponent<UIMap>();
		AuiButton.SetCameraAllChild(gameObject2.transform, interControl.uiCamera);
		uiMap.procCastle = this;
		uiStatus.uiMap = uiMap;
		interControl.AddUIArea(new Rect(-470f, 170f, 150f, 138f));
		interControl.AddUIArea(new Rect(-320f, 268f, 150f, 40f));
		Bounds gameObjectBound = ProcBase.GetGameObjectBound(uiGameMenu.gameObject);
		interControl.AddUIArea(new Rect(gameObjectBound.min.x, gameObjectBound.min.y, gameObjectBound.size.x, gameObjectBound.size.y));
		interControl.onFollowFinish = OnFollowFinish;
		gameObject2 = GameObject.Find("feb_gemshop");
		if (gameObject2 == null)
		{
			gameObject = ResourceManager.Load("interface/prefabs", "feb_gemshop", typeof(GameObject)) as GameObject;
			gameObject2 = Object.Instantiate(gameObject) as GameObject;
		}
		uiGemShop = gameObject2.GetComponent<UIGemShop>();
		AuiButton.SetCameraAllChild(gameObject2.transform, interControl.uiCamera);
		gameObject2 = GameObject.Find("feb_commandshop");
		if (gameObject2 == null)
		{
			gameObject = ResourceManager.Load("interface/prefabs", "feb_commandshop", typeof(GameObject)) as GameObject;
			gameObject2 = Object.Instantiate(gameObject) as GameObject;
		}
		uiCmdPtsShop = gameObject2.GetComponent<UICmdPtsShop>();
		AuiButton.SetCameraAllChild(gameObject2.transform, interControl.uiCamera);
		uiCmdPtsShop.uiGemShop = uiGemShop;
		gameObject2 = GameObject.Find("feb_quest");
		if (gameObject2 == null)
		{
			gameObject = ResourceManager.Load("interface/prefabs", "feb_quest", typeof(GameObject)) as GameObject;
			gameObject2 = Object.Instantiate(gameObject) as GameObject;
		}
		uiQuest = gameObject2.GetComponent<UIQuest>();
		AuiButton.SetCameraAllChild(gameObject2.transform, interControl.uiCamera);
		uiQuest.buttonClose.onButtonClick = OnQuestCloseClick;
		uiQuest.Init(questUnit);
		uiQuest.heroUnit = interControl.ctrlUnit;
		gameObject2 = GameObject.Find("feb_lovegame");
		if (gameObject2 == null)
		{
			gameObject = ResourceManager.Load("interface/prefabs", "feb_lovegame", typeof(GameObject)) as GameObject;
			gameObject2 = Object.Instantiate(gameObject) as GameObject;
		}
		uiLoveGame = gameObject2.GetComponent<UILoveGame>();
		AuiButton.SetCameraAllChild(gameObject2.transform, interControl.uiCamera);
		uiLoveGame.buttonClose.onButtonClick = OnLoveCloseClick;
		uiLoveGame.InitNPC(loveUnit);
		uiLoveGame.procCastle = this;
		gameObject2 = GameObject.Find("feb_npcaction");
		if (gameObject2 == null)
		{
			gameObject = ResourceManager.Load("interface/prefabs", "feb_npcaction", typeof(GameObject)) as GameObject;
			gameObject2 = Object.Instantiate(gameObject) as GameObject;
		}
		uiNpcAction = gameObject2.GetComponent<UINpcAction>();
		AuiButton.SetCameraAllChild(gameObject2.transform, interControl.uiCamera);
		uiNpcAction.buttonClose.onButtonClick = OnNpcActionCloseClick;
		uiNpcAction.procCastle = this;
		bgmMap = base.gameObject.AddComponent<AudioSource>();
		bgmMap.clip = ResourceManager.Load("Sound/bgm", "bgm_map", typeof(AudioClip)) as AudioClip;
		bgmMap.loop = true;
		bgmMap.playOnAwake = false;
		bgmMap.volume = (float)UserSetting.volumeBgm / 100f;
		bgmCastle = base.gameObject.AddComponent<AudioSource>();
		bgmCastle.clip = ResourceManager.Load("Sound/bgm", "bgm_town", typeof(AudioClip)) as AudioClip;
		bgmCastle.loop = true;
		bgmCastle.playOnAwake = false;
		bgmCastle.volume = (float)UserSetting.volumeBgm / 100f;
		bgmFortune = base.gameObject.AddComponent<AudioSource>();
		bgmFortune.clip = ResourceManager.Load("Sound/bgm", "bgm_fortune", typeof(AudioClip)) as AudioClip;
		bgmFortune.loop = true;
		bgmFortune.playOnAwake = false;
		bgmFortune.volume = (float)UserSetting.volumeBgm / 100f;
		cameraControl.SetPitch(15f);
		ShowMap();
		uiMap.MoveToLastHumanCastle();
		uiMap.MoveToLastBattleCastle();
		if (PlayInfo.battleInfo != null)
		{
			StartCoroutine("ShowBattleResultAnimation");
		}
		PlayInfo.castleAI.Reset();
		if (PlayInfo.castleAI.attackCurrent.attackActive)
		{
			StartCoroutine("CountDownForBattle");
		}
		StartCoroutine("CoroutineIncTime");
		StartCoroutine("SwitchLoveGameQuestMode");
		if (PlayInfo.playerData.tutorialMode)
		{
			StartCoroutine("TutorialGotoCastle");
			return;
		}
		GameObject gameObject3 = GameObject.Find("AlphaAdManager");
		if (!(gameObject3 == null))
		{
			AlphaAdManager component = gameObject3.GetComponent<AlphaAdManager>();
			if (!(component == null))
			{
				component.ShowListBanner(null);
			}
		}
	}

	private IEnumerator TutorialGotoCastle()
	{
		yield return 1;
		if (isShowMap)
		{
			ShowCastle();
		}
	}

	private IEnumerator ShowBattleResultAnimation()
	{
		yield return new WaitForSeconds(0.5f);
		if (PlayInfo.battleInfo != null)
		{
			Vector3 pos = new Vector3(PlayInfo.battleInfo.defenseCastle.posx, PlayInfo.battleInfo.defenseCastle.posy, 0f);
			if (PlayInfo.battleInfo.battleWin)
			{
				pos.z = uiMap.aniBattleWin.transform.localPosition.z;
				uiMap.aniBattleWin.transform.localPosition = pos;
				uiMap.aniBattleWin.visible = true;
				uiMap.aniBattleWin.StartAnimation(0, false, true);
			}
			else
			{
				pos.z = uiMap.aniBattleLose.transform.localPosition.z;
				uiMap.aniBattleLose.transform.localPosition = pos;
				uiMap.aniBattleLose.visible = true;
				uiMap.aniBattleLose.StartAnimation(0, false, true);
			}
		}
	}

	public void OnInventoryClick(AuiButton sender)
	{
		uiInventory.Show();
	}

	public void OnShowMapClick(AuiButton sender)
	{
		if (isShowMap)
		{
			ShowCastle();
			return;
		}
		ResetCamera();
		ShowMap();
	}

	public void OnShowCastleClick(AuiButton sender)
	{
		if (isShowMap)
		{
			ShowCastle();
			return;
		}
		ResetCamera();
		ShowMap();
	}

	public void OnSkillClick(AuiButton sender)
	{
		uiSkill.Show();
	}

	public void OnMenuClick(AuiButton sender)
	{
		uiPlayMenu.Show(false);
	}

	public void OnShopCloseClick(AuiButton sender)
	{
		uiShopBuy.gameObject.SetActiveRecursive(false);
		uiShopSell.gameObject.SetActiveRecursive(false);
		ResetCamera();
	}

	public void OnSkillMasterCloseClick(AuiButton sender)
	{
		uiSkillMaster.gameObject.SetActiveRecursive(false);
		ResetCamera();
	}

	public void OnTrainingCloseClick(AuiButton sender)
	{
		uiTraining.Hide();
	}

	public void OnRetainerCloseClick(AuiButton sender)
	{
		uiLord.Hide();
	}

	public void OnQuestCloseClick(AuiButton sender)
	{
		uiQuest.Hide();
		ResetCamera();
	}

	public void OnLoveCloseClick(AuiButton sender)
	{
		uiLoveGame.Hide();
		ResetCamera();
		loveUnit.GetComponent<UnitControl>().isMovable = true;
	}

	public void OnNpcActionCloseClick(AuiButton sender)
	{
		uiNpcAction.Hide();
		ResetCamera();
	}

	public void OnNpcDialogYesClick(AuiButton sender)
	{
		ResetCamera();
		isShowMap = true;
		ShowMap();
	}

	public void OnNpcDialogNoClick(AuiButton sender)
	{
		ResetCamera();
		uiNpcDialog.Hide();
	}

	public void OnGoBattleCastleClick(AuiButton sender)
	{
		if (!isShowMap)
		{
			ShowMap();
		}
		int index = PlayInfo.castleAI.attackCurrent.attackTo.index;
		uiMap.MoveToCastle(index);
	}

	public void ResetCamera()
	{
		cameraControl.SetZoomLength(1f);
		cameraControl.SetShiftX(0f);
		cameraControl.SetShiftY(0f);
		cameraControl.SetTarget(interControl.ctrlUnit);
		cameraControl.camUnit.fieldOfView = befNpcFov;
		interControl.UserInputEnable(true);
		uiNpcDialog.Hide();
	}

	private void OnFollowFinish(UnitCharactor unitCtrl)
	{
		if (isShowMap || unitCtrl.charType != UnitCharactor.CharactorType.npc)
		{
			return;
		}
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		bool flag6 = false;
		if (unitCtrl.charIdx == 0)
		{
			flag = true;
			uiShopBuy.SetShopType(ItemManager.ItemType.weapon);
			uiNpcDialog.Show(StringContent.msgNpcDlgChooseWeapon, UINpcDialog.DialogAlign.left, null, null);
			PlayInfo.soundManager.Play(ExtSoundManager.Effect2D.efs_weapon, new Vector3(0f, 0f, 0f));
		}
		if (unitCtrl.charIdx == 1)
		{
			flag = true;
			uiShopBuy.SetShopType(ItemManager.ItemType.cloth);
			uiNpcDialog.Show(StringContent.msgNpcDlgChooseArmor, UINpcDialog.DialogAlign.left, null, null);
			PlayInfo.soundManager.Play(ExtSoundManager.Effect2D.efs_armor, new Vector3(0f, 0f, 0f));
		}
		if (unitCtrl.charIdx == 2)
		{
			flag = true;
			uiShopBuy.SetShopType(ItemManager.ItemType.misc);
			uiNpcDialog.Show(StringContent.msgNpcDlgChooseItem, UINpcDialog.DialogAlign.left, null, null);
			PlayInfo.soundManager.Play(ExtSoundManager.Effect2D.efs_accessory, new Vector3(0f, 0f, 0f));
		}
		if (unitCtrl.charIdx == 5)
		{
			flag2 = true;
			uiNpcDialog.Show(StringContent.msgNpcDlgLearnSkill, UINpcDialog.DialogAlign.left, null, null);
			PlayInfo.soundManager.Play(ExtSoundManager.Effect2D.efs_skillmaster, new Vector3(0f, 0f, 0f));
		}
		if (unitCtrl.charIdx == 3)
		{
			flag3 = true;
			uiNpcDialog.Show(StringContent.msgNpcDlgGotoWorldMap, UINpcDialog.DialogAlign.middle, OnNpcDialogYesClick, OnNpcDialogNoClick);
			PlayInfo.soundManager.Play(ExtSoundManager.Effect2D.efs_gatekeeper, new Vector3(0f, 0f, 0f));
		}
		if (unitCtrl.charIdx == 4)
		{
			flag4 = true;
			PlayInfo.soundManager.Play(ExtSoundManager.Effect2D.efs_captain, new Vector3(0f, 0f, 0f));
		}
		if (unitCtrl.charIdx == 8)
		{
			flag5 = true;
			PlayInfo.soundManager.Play(ExtSoundManager.Effect2D.efs_zd, new Vector3(0f, 0f, 0f));
		}
		if (unitCtrl.charIdx == 6)
		{
			flag6 = true;
			PlayInfo.soundManager.Play(ExtSoundManager.Effect2D.efs_priest, new Vector3(0f, 0f, 0f));
		}
		if (unitCtrl.charIdx == 7)
		{
			flag6 = true;
			PlayInfo.soundManager.Play(ExtSoundManager.Effect2D.efs_secretary, new Vector3(0f, 0f, 0f));
		}
		if (flag)
		{
			uiShopBuy.Show();
			AuiButton.SetCameraAllChild(uiShopBuy.transform, interControl.uiCamera);
		}
		else if (flag2)
		{
			uiSkillMaster.Show();
		}
		else if (flag4)
		{
			uiQuest.Show();
		}
		else if (flag5)
		{
			uiLoveGame.Show();
			unitCtrl.thisCtrl.ForcedIdle();
			unitCtrl.thisCtrl.isMovable = false;
			unitCtrl.thisCtrl.SetRotationTo(interControl.ctrlUnit.thisTrans.position);
		}
		else if (flag6)
		{
			if (unitCtrl.charIdx == 6)
			{
				uiNpcAction.Show(UINpcAction.NpcActionType.priest);
			}
			else if (unitCtrl.charIdx == 7)
			{
				uiNpcAction.Show(UINpcAction.NpcActionType.secretary);
			}
		}
		if (flag || flag2 || flag3 || flag4 || flag5 || flag6)
		{
			unitCtrl.thisCtrl.SetRotationTo(interControl.ctrlUnit.thisTrans.position);
			cameraControl.SetZoomLength(0.5f);
			bool flag7 = flag3 || flag4 || flag5 || flag6;
			bool flag8 = flag4 || flag5 || flag6;
			cameraControl.SetShiftX((!flag7) ? (-3f) : 0f);
			cameraControl.SetShiftY((!flag8) ? 3f : 4f);
			befNpcFov = cameraControl.camUnit.fieldOfView;
			cameraControl.camUnit.fieldOfView = 60f;
			cameraControl.SetTarget(unitCtrl.thisCtrl);
			interControl.UserInputEnable(false);
		}
	}

	private void HideAll()
	{
		uiInventory.Hide();
		uiShopBuy.Hide();
		uiShopSell.Hide();
		uiSkill.Hide();
		uiSkillMaster.Hide();
		uiNpcDialog.Hide();
		uiMap.Hide();
		uiFortune.Hide();
		uiFortuneResult.Hide();
		uiMessageNote.Hide();
		uiLord.Hide();
		uiGemShop.Hide();
		uiCmdPtsShop.Hide();
		uiQuest.Hide();
		uiLoveGame.Hide();
		uiNpcAction.Hide();
		uiUserSetting.Hide();
		uiTutorial.Hide();
		ResetCamera();
	}

	public void ShowMap()
	{
		HideAll();
		uiMap.Show();
		cameraControl.camUnit.enabled = false;
		interControl.UserInputEnable(false);
		uiGameMenu.buttonCastle.visible = true;
		uiGameMenu.buttonMap.visible = false;
		isShowMap = true;
		bgmMap.volume = (float)UserSetting.volumeBgm / 100.9f;
		UserSetting.currentBgmSound = bgmMap;
		bgmMap.Play();
		bgmCastle.Pause();
		bgmFortune.Pause();
		if (PlayInfo.castleAI.attackCurrent.attackActive)
		{
			uiMap.aniAttacked.visible = true;
			uiMap.aniAttacked.StartAnimation(true, false);
		}
	}

	public void ShowCastle()
	{
		HideAll();
		cameraControl.camUnit.enabled = true;
		interControl.UserInputEnable(true);
		uiGameMenu.buttonCastle.visible = false;
		uiGameMenu.buttonMap.visible = true;
		isShowMap = false;
		bgmCastle.volume = (float)UserSetting.volumeBgm / 100.9f;
		UserSetting.currentBgmSound = bgmCastle;
		bgmMap.Pause();
		bgmCastle.Play();
		bgmFortune.Pause();
	}

	private IEnumerator CoroutineIncTime()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.2f);
			if (tutorialActive)
			{
				continue;
			}
			bool dayChange = false;
			PlayInfo.gameTime.IncTime(0.2f, out dayChange);
			if (PlayInfo.fortuneSystem.takeFortuneActive)
			{
				if (!AuiButton.modalActive && !AuiButton.mostTopActive)
				{
					PlayInfo.fortuneSystem.takeFortuneActive = false;
					uiFortune.Show();
				}
			}
			else if (PlayInfo.fortuneSystem.eventFortuneActive && !AuiButton.modalActive && !AuiButton.mostTopActive)
			{
				PlayInfo.fortuneSystem.eventFortuneActive = false;
				uiFortuneResult.Show(PlayInfo.fortuneSystem.currentEventIndex, PlayInfo.fortuneSystem.currentTargetCastleIdx);
			}
			if (dayChange)
			{
				PlayInfo.monsterMilitary.CheckDayProcess();
				CastleInfo[] castle = PlayInfo.castleManager.castle;
				foreach (CastleInfo info in castle)
				{
					info.CheckDayProcess();
				}
				PlayInfo.fortuneSystem.CheckFortuneSystem();
				PlayInfo.lordManager.ProcessDaily();
				PlayInfo.castleAI.Compute();
				if (PlayInfo.castleAI.attackReserve.attackActive)
				{
					string msg = StringContent.msgAttackFromMonster.Replace(StringContent.strValue, PlayInfo.castleAI.attackReserve.attackTo.castleName);
					ProcBase.ShowMsg(msg, MessageView.MsgIcon.military);
					PlayInfo.messageManager.Add(2, PlayMessage.MessagLevel.alert, msg);
					PlayInfo.castleAI.attackCurrent.attackActive = true;
					PlayInfo.castleAI.attackCurrent.attackFrom = PlayInfo.castleAI.attackReserve.attackFrom;
					PlayInfo.castleAI.attackCurrent.attackTo = PlayInfo.castleAI.attackReserve.attackTo;
					PlayInfo.castleAI.attackCountDown = 10;
					PlayInfo.castleAI.attackReserve.Reset();
					StartCoroutine("CountDownForBattle");
				}
				PlayInfo.questManager.ProcessDaily(true);
				uiQuest.RefreshQuestIcon();
				PlayInfo.Save();
			}
			else
			{
				PlayInfo.questManager.ProcessDaily(false);
				uiQuest.RefreshQuestIcon();
			}
			if (dayChange || PlayInfo.gameTime.pause)
			{
				continue;
			}
			int idx = 0;
			if (ReviewGem.CheckHeroLevel(ref idx))
			{
				reviewAddGem = ReviewGem.rewardList[idx].gem;
				int level = PlayInfo.heroState.level;
				string strRev2 = StringContent.msgReviewGiveGem1;
				switch (idx)
				{
				case 1:
					strRev2 = StringContent.msgReviewGiveGem2;
					break;
				case 2:
					strRev2 = StringContent.msgReviewGiveGem3;
					break;
				}
				strRev2 = strRev2.Replace(StringContent.strValue, level.ToString());
				if (idx == 0)
				{
					ProcBase.ShowMsg(strRev2, MessageView.MsgIcon.question, false, new MessageView.MsgButton[2]
					{
						MessageView.MsgButton.yes,
						MessageView.MsgButton.no
					}, OnReviewMessageFirst);
				}
				else
				{
					ProcBase.ShowMsg(strRev2, MessageView.MsgIcon.question, false, new MessageView.MsgButton[2]
					{
						MessageView.MsgButton.yes,
						MessageView.MsgButton.no
					}, OnReviewMessage);
				}
				PlayInfo.gameTime.pause = true;
			}
		}
	}

	private void OnReviewMessageFirst(MessageView.MsgButton button)
	{
		PlayInfo.gameTime.pause = false;
		if (button != 0)
		{
			return;
		}
		PlayInfo.playerData.gem += reviewAddGem;
		PlayInfo.Save();
		if (PlusType.isPlus)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (UserSetting.language == Language.korean)
				{
					Application.OpenURL("http://itunes.apple.com/kr/app/castle-master-3d/id521038578?mt=8");
				}
				if (UserSetting.language == Language.japanese)
				{
					Application.OpenURL("http://itunes.apple.com/jp/app/castle-master-3d/id521038578?mt=8");
				}
				else
				{
					Application.OpenURL("http://itunes.apple.com/us/app/castle-master-3d/id521038578?mt=8");
				}
			}
			else if (Application.platform == RuntimePlatform.Android)
			{
				if (StoreType.store == StoreType.Store.android)
				{
					Application.OpenURL("market://details?id=com.alphacloud.castlemasterplus");
				}
				else if (StoreType.store == StoreType.Store.tstore)
				{
					Application.OpenURL("tstore://PRODUCT_VIEW/0000282959/0");
				}
				else if (StoreType.store == StoreType.Store.olleh)
				{
					Application.OpenURL("cstore://detail?CONTENT_TYPE=APPLICATION&P_TYPE=c&P_ID=51200009148584&N_ID=A001002&CAT_TYPE=GAME");
				}
				else if (StoreType.store == StoreType.Store.samsung)
				{
					Application.OpenURL("http://m.facebook.com/45castles");
				}
			}
		}
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if (UserSetting.language == Language.korean)
			{
				Application.OpenURL("http://itunes.apple.com/kr/app/castle-master-3d/id521038578?mt=8");
			}
			if (UserSetting.language == Language.japanese)
			{
				Application.OpenURL("http://itunes.apple.com/jp/app/castle-master-3d/id521038578?mt=8");
			}
			else
			{
				Application.OpenURL("http://itunes.apple.com/us/app/castle-master-3d/id521038578?mt=8");
			}
		}
		else if (Application.platform == RuntimePlatform.Android)
		{
			if (StoreType.store == StoreType.Store.android)
			{
				Application.OpenURL("market://details?id=com.alphacloud.castlemaster");
			}
			else if (StoreType.store == StoreType.Store.tstore)
			{
				Application.OpenURL("tstore://PRODUCT_VIEW/0000282959/0");
			}
			else if (StoreType.store == StoreType.Store.olleh)
			{
				Application.OpenURL("cstore://detail?CONTENT_TYPE=APPLICATION&P_TYPE=c&P_ID=51200009148584&N_ID=A001002&CAT_TYPE=GAME");
			}
			else if (StoreType.store == StoreType.Store.samsung)
			{
				Application.OpenURL("http://m.facebook.com/45castles");
			}
		}
	}

	private void OnReviewMessage(MessageView.MsgButton button)
	{
		PlayInfo.gameTime.pause = false;
		if (button == MessageView.MsgButton.yes)
		{
			PlayInfo.playerData.gem += reviewAddGem;
			PlayInfo.Save();
			Application.OpenURL("http://m.facebook.com/45castles");
		}
	}

	private IEnumerator CountDownForBattle()
	{
		yield return 1;
		uiStatus.BlinkAlert();
		PlayInfo.soundManager.Play(ExtSoundManager.Effect2D.efs_castle_attack, new Vector3(0f, 0f, 0f));
		uiMap.aniAttacked.visible = true;
		uiMap.aniAttacked.StartAnimation(true, false);
		Vector3 pos = new Vector3(PlayInfo.castleAI.attackCurrent.attackTo.posx, PlayInfo.castleAI.attackCurrent.attackTo.posy, uiMap.aniAttacked.transform.localPosition.z);
		pos.y += 100f;
		uiMap.aniAttacked.transform.localPosition = pos;
		uiStatus.iconCastle.SetFrame(PlayInfo.castleAI.attackCurrent.attackTo.level - 1);
		uiStatus.castleName.text = PlayInfo.castleAI.attackCurrent.attackTo.castleName;
		while (PlayInfo.castleAI.attackCountDown > 0)
		{
			if (!PlayInfo.gameTime.pause)
			{
				PlayInfo.castleAI.attackCountDown--;
			}
			string msg = StringContent.msgBattleCountdown.Replace(StringContent.strValue, PlayInfo.castleAI.attackCountDown.ToString());
			uiStatus.textAlert.text = msg;
			yield return new WaitForSeconds(1f);
			uiMap.aniAttacked.visible = uiMap.gameObject.activeInHierarchy;
		}
		while (AuiButton.modalActive || uiFortune.gameObject.activeInHierarchy || PlayInfo.gameTime.pause)
		{
			yield return new WaitForSeconds(0.2f);
			uiMap.aniAttacked.visible = uiMap.gameObject.activeInHierarchy;
		}
		PlayInfo.castleAI.SetBattleUnitCount();
		ProcMain.playStage = StageManager.StageType.battle;
		ProcBase.LoadScene("Scene Main", ProcLoading.ContentMode.battle);
	}

	private IEnumerator SwitchLoveGameQuestMode()
	{
		while (true)
		{
			uiLoveGame.RandomAllowQuest();
			yield return new WaitForSeconds(PlayInfo.gameTime.secPerDay);
		}
	}
}
