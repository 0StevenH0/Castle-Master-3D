using System.Collections;
using UnityEngine;

public class ProcPlayerList : ProcBase
{
	private class Player
	{
		public string heroName = string.Empty;

		public int level = 1;

		public int expSum;

		public int[] cloth = new int[3]
		{
			defaultCloth[0],
			defaultCloth[1],
			defaultCloth[2]
		};

		public int weapon = 100;

		public bool isNew = true;

		public bool isUpdatedRank;
	}

	private const int maxPlayer = 3;

	private const int defaultWeapon = 100;

	public AuiButton buttonBack;

	public AuiButton buttonNewGame;

	public AuiButton buttonSubmit;

	public AuiButton buttonCancel;

	public AuiButton buttonPlay;

	public AuiButton buttonDelete;

	public AuiButton buttonSkip;

	public AuiButton buttonRanking;

	public Transform modelCamera;

	public GameObject panelPlayerInfo;

	public GameObject panelEmpty;

	public GameObject panelNewGame;

	public GameObject panelSynopsis;

	public GameObject panelSceneMenu;

	public UIRanking uiRanking;

	public TextMesh textPlayerLevel;

	public TextMesh textPlayerName;

	public TextMesh textSynopsis;

	public TextMesh textInputName;

	public AuiSpriteAnimation iconCursor;

	public TextMesh labelNewHero;

	public TextMesh labelEnterHeroName;

	public UIKeyBoard uiKeyBoard;

	public GUISkin guiSkinInputText;

	public CharactorManager charactorManager;

	public WeaponManager weaponManager;

	private static int[] defaultCloth = new int[3] { 200, 210, 220 };

	private static int currentPlayer = 0;

	private int dirRotate;

	private float currentY;

	private float rotateY;

	private UnitControl[] playerChar;

	private bool isBannerActive;

	private string playerName = string.Empty;

	private Player[] players = new Player[3];

	public override void OnStart()
	{
		ProcMain.InitGame();
		currentPlayer = UserSetting.currentSaveSlot;
		buttonBack.onButtonClick = OnBack;
		buttonNewGame.onButtonClick = OnNewGame;
		buttonSubmit.onButtonClick = OnSubmit;
		buttonCancel.onButtonClick = OnCancel;
		buttonPlay.onButtonClick = OnPlay;
		buttonDelete.onButtonClick = OnDelete;
		buttonSkip.onButtonClick = OnSkip;
		buttonRanking.onButtonClick = OnRankingClick;
		AudioSource audioSource = base.gameObject.AddComponent<AudioSource>();
		audioSource.clip = ResourceManager.Load("Sound/bgm", "bgm_title", typeof(AudioClip)) as AudioClip;
		audioSource.loop = true;
		audioSource.volume = (float)UserSetting.volumeBgm / 100f;
		audioSource.Play();
		UserSetting.currentBgmSound = audioSource;
		LoadPlayerList();
		panelPlayerInfo.SetActive(false);
		panelEmpty.SetActive(true);
		panelNewGame.SetActive(false);
		panelSynopsis.SetActive(false);
		panelSceneMenu.SetActive(true);
		playerChar = new UnitControl[3];
		Vector3 vector = new Vector3(0f, 0f, 2f);
		for (int i = 0; i < 3; i++)
		{
			float num = 120 * i;
			Quaternion quaternion = Quaternion.Euler(new Vector3(0f, num, 0f));
			Vector3 vector2 = vector;
			if (i > 0)
			{
				vector2 = quaternion * vector;
			}
			playerChar[i] = charactorManager.AddHero(vector2).thisCtrl;
			playerChar[i].SetRotation(num);
			playerChar[i].SetPosition(vector2);
		}
		Vector3 eulerAngles = modelCamera.rotation.eulerAngles;
		eulerAngles.y = (float)currentPlayer * 120f;
		modelCamera.rotation = Quaternion.Euler(eulerAngles);
		ResetPlayerChar();
		ShowPlayerInfo();
		labelNewHero.text = StringContent.msgDescHeroName;
		labelEnterHeroName.text = StringContent.msgTitleHeroName;
		AuiButton.SetCameraAllChild(uiKeyBoard.transform, base.GetComponent<Camera>());
		uiKeyBoard.maxLength = 20;
		AuiButton.SetCameraAllChild(uiRanking.transform, base.GetComponent<Camera>());
		uiRanking.Hide();
		ProcBase.ChangeTextMeshLanguageAllChild(panelPlayerInfo.transform);
		ProcBase.ChangeTextMeshLanguageAllChild(panelEmpty.transform);
		ProcBase.ChangeTextMeshLanguageAllChild(panelNewGame.transform);
		ProcBase.ChangeTextMeshLanguageAllChild(panelSynopsis.transform);
		ProcBase.ChangeTextMeshLanguageAllChild(panelSceneMenu.transform);
	}

	private void ResetPlayerChar()
	{
		for (int i = 0; i < 3; i++)
		{
			playerChar[i].thisChar.heroModel.SetClothPartFromCode(HeroModel.ClothPart.head, players[i].cloth[0]);
			playerChar[i].thisChar.heroModel.SetClothPartFromCode(HeroModel.ClothPart.top, players[i].cloth[1]);
			playerChar[i].thisChar.heroModel.SetClothPartFromCode(HeroModel.ClothPart.bottom, players[i].cloth[2]);
			playerChar[i].thisChar.heroModel.SetWeapon(weaponManager, UnitCharactor.WeaponType.onehand, players[i].weapon);
		}
	}

	private void OnBack(AuiButton sender)
	{
		ProcBase.LoadScene("Scene Title", ProcLoading.ContentMode.mainmenu);
	}

	private void OnSubmit(AuiButton sender)
	{
		playerName = playerName.Trim();
		if (playerName.Length < 3)
		{
			ProcBase.ShowMsg(StringContent.msgEnterPlayerName, MessageView.MsgIcon.alert);
			return;
		}
		int num = currentPlayer;
		players[num] = new Player();
		players[num].heroName = playerName;
		DataRegistry.SetSlot(num);
		PlayInfo.Init();
		PlayInfo.playerData.heroName = playerName;
		PlayInfo.Save();
		ShowPlayerInfo();
		panelNewGame.SetActive(false);
	}

	private void OnNewGame(AuiButton sender)
	{
		panelEmpty.SetActive(false);
		panelNewGame.SetActive(true);
		uiKeyBoard.textValue = string.Empty;
		playerName = string.Empty;
		uiKeyBoard.buttonEnter.onButtonClick = OnSubmit;
	}

	private void OnCancel(AuiButton sender)
	{
		panelEmpty.SetActive(true);
		panelNewGame.SetActive(false);
	}

	private void OnPlay(AuiButton sender)
	{
		int num = currentPlayer;
		panelPlayerInfo.SetActive(false);
		panelSceneMenu.SetActive(false);
		if (UserSetting.currentSaveSlot != currentPlayer)
		{
			UserSetting.currentSaveSlot = currentPlayer;
			UserSetting.Save();
		}
		if (players[num].isNew)
		{
			panelSynopsis.SetActive(true);
			textSynopsis.text = StringContent.msgSynopsis.Replace(StringContent.strValue, players[num].heroName);
			ProcBase.ResetTextWordWarp(textSynopsis, 900f);
			StartCoroutine("RollingSynopsis");
		}
		else
		{
			StartGame();
		}
	}

	private void OnSkip(AuiButton sender)
	{
		panelSynopsis.SetActive(false);
		StartGame();
	}

	private void OnRankingClick(AuiButton sender)
	{
		if (!players[currentPlayer].isUpdatedRank && players[currentPlayer].expSum > 0)
		{
			ProcBase.ShowMsg(StringContent.msgUpdateRanking, MessageView.MsgIcon.question, new MessageView.MsgButton[2]
			{
				MessageView.MsgButton.yes,
				MessageView.MsgButton.no
			}, OnRankingUpdate);
		}
		else
		{
			uiRanking.Show(players[currentPlayer].heroName);
		}
	}

	private void OnRankingUpdate(MessageView.MsgButton button)
	{
		if (button == MessageView.MsgButton.yes)
		{
			players[currentPlayer].isUpdatedRank = true;
			buttonRanking.enabled = false;
			AppRankingSystem appRankingSystem = base.gameObject.GetComponent<AppRankingSystem>();
			if (appRankingSystem == null)
			{
				appRankingSystem = base.gameObject.AddComponent<AppRankingSystem>();
			}
			appRankingSystem.UpdateRanking("castlemaster", players[currentPlayer].heroName, players[currentPlayer].expSum, OnUpdateRankingResult);
		}
		else
		{
			uiRanking.Show(players[currentPlayer].heroName);
		}
	}

	private void OnUpdateRankingResult(bool isError)
	{
		buttonRanking.enabled = true;
		if (isError)
		{
			ProcBase.ShowMsg(StringContent.msgErrorUpdateRanking, MessageView.MsgIcon.alert);
		}
		uiRanking.Show(players[currentPlayer].heroName);
	}

	private IEnumerator RollingSynopsis()
	{
		Vector3 pos = textSynopsis.transform.localPosition;
		pos.y = (float)(-ScreenSize.Height) * 0.5f;
		textSynopsis.transform.localPosition = pos;
		float endY = textSynopsis.GetComponent<Renderer>().bounds.size.y * 1.1f + (float)ScreenSize.Height * 0.5f;
		while (pos.y < endY)
		{
			pos.y += Time.deltaTime * 50f;
			textSynopsis.transform.localPosition = pos;
			yield return 1;
		}
		StartGame();
	}

	private void StartGame()
	{
		isBannerActive = true;
		AlphaAdManager component = GetComponent<AlphaAdManager>();
		component.ShowStartAd(OnBannerClose);
	}

	private void OnBannerClose()
	{
		int slot = currentPlayer;
		DataRegistry.SetSlot(slot);
		PlayInfo.Init();
		PlayInfo.Load();
		if (PlayInfo.castleManager.GetCastleCount(0) == 0)
		{
			if (PlayInfo.gameTime.day != 0 || PlayInfo.gameTime.hour != 0)
			{
				ProcEnding.isWin = false;
				ProcBase.LoadScene("Scene Ending", ProcLoading.ContentMode.castle);
				return;
			}
		}
		else if (PlayInfo.castleManager.GetCastleCount(1) == 0)
		{
			ProcEnding.isWin = true;
			ProcBase.LoadScene("Scene Ending", ProcLoading.ContentMode.castle);
			return;
		}
		ProcMain.playStage = StageManager.StageType.castle;
		ProcBase.LoadScene("Scene Main", ProcLoading.ContentMode.castle);
	}

	private void OnDelete(AuiButton sender)
	{
		int num = currentPlayer;
		ProcBase.ShowMsg(StringContent.msgQuestDeletePlayer.Replace(StringContent.strValue, players[num].heroName), MessageView.MsgIcon.question, false, new MessageView.MsgButton[2]
		{
			MessageView.MsgButton.yes,
			MessageView.MsgButton.no
		}, OnDeleteAgree);
	}

	private void OnDeleteAgree(MessageView.MsgButton button)
	{
		if (button == MessageView.MsgButton.yes)
		{
			int num = currentPlayer;
			players[num] = new Player();
			DataRegistry.DeleteSlot(num);
			ResetPlayerChar();
			ShowPlayerInfo();
		}
	}

	private void Update()
	{
		if (uiRanking.gameObject.activeInHierarchy)
		{
			return;
		}
		if (panelNewGame.activeInHierarchy)
		{
			textInputName.text = uiKeyBoard.textValue;
			playerName = uiKeyBoard.textValue.Trim();
			uiKeyBoard.allowInput = textInputName.GetComponent<Renderer>().bounds.size.x < 320f;
			Vector3 position = iconCursor.transform.position;
			position.x = textInputName.GetComponent<Renderer>().bounds.size.x + textInputName.transform.position.x + 2f;
			if (textInputName.text.Length > 1 && textInputName.text.Substring(textInputName.text.Length - 1, 1) == " ")
			{
				position.x += 10f;
			}
			iconCursor.transform.position = position;
		}
		else
		{
			if (panelSynopsis.activeInHierarchy || isBannerActive || AuiButton.modalActive)
			{
				return;
			}
			if (dirRotate != 0)
			{
				Vector3 eulerAngles = modelCamera.rotation.eulerAngles;
				float num = 240f * Time.deltaTime;
				currentY += num * (float)dirRotate;
				if ((dirRotate > 0 && currentY < rotateY) || (dirRotate < 0 && currentY > rotateY))
				{
					eulerAngles.y = currentY;
				}
				else
				{
					currentPlayer += dirRotate;
					if (currentPlayer < 0)
					{
						currentPlayer = 2;
					}
					if (currentPlayer > 2)
					{
						currentPlayer = 0;
					}
					ShowPlayerInfo();
					eulerAngles.y = rotateY;
					dirRotate = 0;
				}
				modelCamera.rotation = Quaternion.Euler(eulerAngles);
			}
			else
			{
				if (!Input.GetMouseButtonDown(0))
				{
					return;
				}
				Vector3 vector = buttonBack.uiCamera.ScreenToWorldPoint(Input.mousePosition);
				if (vector.y < 190f && vector.y > -190f)
				{
					if (vector.x < -200f)
					{
						currentY = modelCamera.rotation.eulerAngles.y;
						rotateY = currentY + 120f;
						dirRotate = 1;
					}
					else if (vector.x > 200f)
					{
						currentY = modelCamera.rotation.eulerAngles.y;
						rotateY = currentY - 120f;
						dirRotate = -1;
					}
					if (dirRotate != 0)
					{
						panelPlayerInfo.SetActive(false);
						panelEmpty.SetActive(false);
					}
				}
			}
		}
	}

	private void ShowPlayerInfo()
	{
		int num = currentPlayer;
		if (players[num].heroName.Length == 0)
		{
			panelPlayerInfo.SetActive(false);
			panelEmpty.SetActive(true);
		}
		else
		{
			panelPlayerInfo.SetActive(true);
			panelEmpty.SetActive(false);
			textPlayerLevel.text = "Lv." + players[num].level;
			textPlayerName.text = players[num].heroName;
			ProcBase.ResetTextWidth(textPlayerName, 400f);
		}
		StopCoroutine("PlayCharSelectedAnimation");
		StartCoroutine("PlayCharSelectedAnimation", num);
		PlayInfo.soundManager.Play(ExtSoundManager.Effect2D.efs_onehand_1hit, new Vector3(0f, 0f, 0f));
	}

	private IEnumerator PlayCharSelectedAnimation(int cur)
	{
		for (int i = 0; i < playerChar.Length; i++)
		{
			playerChar[i].GetComponent<Animation>().Play((i != cur) ? "none_idle" : "onehand_attacknormal01");
		}
		float len = playerChar[cur].GetComponent<Animation>()["onehand_attacknormal01"].clip.length;
		yield return new WaitForSeconds(len / 0.25f - 0.1f);
		playerChar[cur].GetComponent<Animation>().CrossFade("none_idle");
	}

	private void LoadPlayerList()
	{
		DataRegistry.ClearAll();
		for (int i = 0; i < 3; i++)
		{
			DataRegistry.SetSlot(i);
			PlayInfo.playerData.heroName = string.Empty;
			PlayInfo.Load();
			players[i] = new Player();
			if (PlayInfo.playerData.heroName.Length <= 0)
			{
				continue;
			}
			players[i].heroName = PlayInfo.playerData.heroName;
			players[i].level = PlayInfo.heroState.level;
			players[i].expSum = PlayInfo.heroState.expSum;
			players[i].isNew = false;
			for (int j = 0; j < players[i].cloth.Length; j++)
			{
				if (PlayInfo.playerData.wearCloth[j] != null)
				{
					players[i].cloth[j] = PlayInfo.playerData.wearCloth[j].code;
				}
			}
			if (PlayInfo.playerData.equipWeapon[0] != null)
			{
				players[i].weapon = PlayInfo.playerData.equipWeapon[0].code;
			}
		}
	}
}
