using UnityEngine;

public class ProcBattle : ProcBase
{
	public UIHeroInfo uiHeroInfo;

	public UIGameMenu uiGameMenu;

	public UIInventory uiInventory;

	public UISkill uiSkill;

	public UIGameSlot uiGameSlot;

	public UIBattleInfo uiBattleInfo;

	public UIResult uiResult;

	public UIPlayMenu uiPlayMenu;

	public UIIngameView uiIngameView;

	public UIUserSetting uiUserSetting;

	public UIGameHelp uiGameHelp;

	public InterfaceControl interControl;

	public CameraControl cameraControl;

	public StageManager battleStage;

	public override void OnStart()
	{
		GameObject gameObject = null;
		GameObject gameObject2 = null;
		gameObject = ResourceManager.Load("interface/prefabs", "feb_heroinfo", typeof(GameObject)) as GameObject;
		gameObject2 = Object.Instantiate(gameObject) as GameObject;
		AuiButton.SetCameraAllChild(gameObject2.transform, interControl.uiCamera);
		uiHeroInfo = gameObject2.GetComponent<UIHeroInfo>();
		gameObject = ResourceManager.Load("interface/prefabs", "feb_gamemenu", typeof(GameObject)) as GameObject;
		gameObject2 = Object.Instantiate(gameObject) as GameObject;
		AuiButton.SetCameraAllChild(gameObject2.transform, interControl.uiCamera);
		uiGameMenu = gameObject2.GetComponent<UIGameMenu>();
		uiGameMenu.buttonInventory.onButtonClick = OnInventoryClick;
		uiGameMenu.buttonSkill.onButtonClick = OnSkillClick;
		uiGameMenu.buttonMenu.onButtonClick = OnMenuClick;
		uiGameMenu.buttonCastle.visible = false;
		uiGameMenu.buttonMap.visible = false;
		uiGameMenu.buttonInventory.visible = false;
		uiGameMenu.buttonSkill.visible = false;
		gameObject = ResourceManager.Load("interface/prefabs", "feb_gameslot", typeof(GameObject)) as GameObject;
		gameObject2 = Object.Instantiate(gameObject) as GameObject;
		AuiButton.SetCameraAllChild(gameObject2.transform, interControl.uiCamera);
		uiGameSlot = gameObject2.GetComponent<UIGameSlot>();
		uiGameSlot.heroUnit = interControl.ctrlUnit;
		gameObject = ResourceManager.Load("interface/prefabs", "feb_inventory", typeof(GameObject)) as GameObject;
		gameObject2 = Object.Instantiate(gameObject) as GameObject;
		AuiButton.SetCameraAllChild(gameObject2.transform, interControl.uiCamera);
		uiInventory = gameObject2.GetComponent<UIInventory>();
		uiInventory.thisUnit = interControl.ctrlUnit.thisChar;
		uiInventory.buttonClose.onButtonClick = OnInventoryClick;
		gameObject = ResourceManager.Load("interface/prefabs", "feb_skill", typeof(GameObject)) as GameObject;
		gameObject2 = Object.Instantiate(gameObject) as GameObject;
		AuiButton.SetCameraAllChild(gameObject2.transform, interControl.uiCamera);
		uiSkill = gameObject2.GetComponent<UISkill>();
		uiSkill.buttonClose.onButtonClick = OnSkillClick;
		gameObject = ResourceManager.Load("interface/prefabs", "feb_battleinfo", typeof(GameObject)) as GameObject;
		gameObject2 = Object.Instantiate(gameObject) as GameObject;
		AuiButton.SetCameraAllChild(gameObject2.transform, interControl.uiCamera);
		uiBattleInfo = gameObject2.GetComponent<UIBattleInfo>();
		uiBattleInfo.battleStage = battleStage;
		gameObject = ResourceManager.Load("interface/prefabs", "feb_result", typeof(GameObject)) as GameObject;
		gameObject2 = Object.Instantiate(gameObject) as GameObject;
		AuiButton.SetCameraAllChild(gameObject2.transform, interControl.uiCamera);
		uiResult = gameObject2.GetComponent<UIResult>();
		battleStage.uiResult = uiResult;
		gameObject2 = GameObject.Find("feb_playmenu");
		if (gameObject2 == null)
		{
			gameObject = ResourceManager.Load("interface/prefabs", "feb_playmenu", typeof(GameObject)) as GameObject;
			gameObject2 = Object.Instantiate(gameObject) as GameObject;
		}
		AuiButton.SetCameraAllChild(gameObject2.transform, interControl.uiCamera);
		gameObject2.SetActive(false);
		uiPlayMenu = gameObject2.GetComponent<UIPlayMenu>();
		uiPlayMenu.procBattle = this;
		gameObject2 = GameObject.Find("feb_usersetting");
		if (gameObject2 == null)
		{
			gameObject = ResourceManager.Load("interface/prefabs", "feb_usersetting", typeof(GameObject)) as GameObject;
			gameObject2 = Object.Instantiate(gameObject) as GameObject;
		}
		AuiButton.SetCameraAllChild(gameObject2.transform, interControl.uiCamera);
		gameObject2.SetActive(false);
		uiUserSetting = gameObject2.GetComponent<UIUserSetting>();
		gameObject2 = GameObject.Find("feb_gamehelp");
		if (gameObject2 == null)
		{
			gameObject = ResourceManager.Load("interface/prefabs", "feb_gamehelp", typeof(GameObject)) as GameObject;
			gameObject2 = Object.Instantiate(gameObject) as GameObject;
		}
		AuiButton.SetCameraAllChild(gameObject2.transform, interControl.uiCamera);
		gameObject2.SetActive(false);
		uiGameHelp = gameObject2.GetComponent<UIGameHelp>();
		gameObject = ResourceManager.Load("interface/prefabs", "feb_ingameview", typeof(GameObject)) as GameObject;
		gameObject2 = Object.Instantiate(gameObject) as GameObject;
		AuiButton.SetCameraAllChild(gameObject2.transform, interControl.uiCamera);
		uiIngameView = gameObject2.GetComponent<UIIngameView>();
		uiIngameView.uiCamera = interControl.uiCamera;
		uiIngameView.gameCamera = cameraControl.camUnit;
		gameObject2 = GameObject.Find("feb_gamestart");
		if (gameObject2 == null)
		{
			gameObject = ResourceManager.Load("interface/prefabs", "feb_gamestart", typeof(GameObject)) as GameObject;
			gameObject2 = Object.Instantiate(gameObject) as GameObject;
		}
		AudioSource audioSource = base.gameObject.AddComponent<AudioSource>();
		audioSource.clip = ResourceManager.Load("Sound/bgm", "bgm_battle" + Random.Range(1, 9), typeof(AudioClip)) as AudioClip;
		audioSource.loop = true;
		audioSource.volume = (float)UserSetting.volumeBgm / 100f;
		audioSource.Play();
		UserSetting.currentBgmSound = audioSource;
		uiInventory.Hide();
		uiSkill.Hide();
		uiResult.Hide();
		uiUserSetting.Hide();
		if (UserSetting.viewGameHelp)
		{
			uiGameHelp.Show();
		}
		else
		{
			uiGameHelp.Hide();
		}
		uiHeroInfo.Show(true);
		uiGameSlot.Show();
		interControl.AddUIArea(new Rect(-470f, 170f, 150f, 138f));
		interControl.AddUIArea(new Rect(-320f, 268f, 150f, 40f));
		interControl.AddUIArea(new Rect(180f, 238f, 250f, 70f));
		if (UserSetting.tabletMode)
		{
			interControl.AddUIArea(new Rect(270f, -312f, 205f, 115f));
			interControl.AddUIArea(new Rect(-470f, -312f, 180f, 85f));
		}
		else
		{
			interControl.AddUIArea(new Rect(210f, -312f, 260f, 150f));
			interControl.AddUIArea(new Rect(-470f, -312f, 234f, 110f));
		}
		cameraControl.SetRotate((!PlayInfo.battleInfo.isAttack) ? (-90) : 90);
		cameraControl.SetPitch(25f);
		interControl.ctrlUnit.SetRotation((!PlayInfo.battleInfo.isAttack) ? 90 : (-90));
		interControl.buttonController = base.gameObject.GetComponent<AuiButtonController>();
		PlayInfo.gameTime.pause = true;
	}

	public void OnInventoryClick(AuiButton sender)
	{
		uiInventory.Show();
	}

	public void OnSkillClick(AuiButton sender)
	{
		uiSkill.Show();
	}

	public void OnMenuClick(AuiButton sender)
	{
		uiPlayMenu.Show(true);
	}
}
