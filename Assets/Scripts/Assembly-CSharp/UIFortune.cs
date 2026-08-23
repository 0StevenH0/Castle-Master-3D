using System.Collections;
using UnityEngine;

public class UIFortune : MonoBehaviour
{
	private const int maxCard = 4;

	public AuiButton buttonSubmit;

	public AuiSprite buttonSubmitLabel;

	public AuiSprite cardBoard;

	public AuiSprite[] sprCard;

	public AuiSprite sprSorcerer;

	public AuiSpriteAnimation aniAfterSelect;

	public AuiSpriteAnimation aniBeforeSelect;

	public AuiSprite iconSelected;

	public TextMesh textTalk;

	public ProcCastle procCastle;

	private bool isStarted;

	private float marbleAlpha;

	private bool marbleLight;

	private Material matMarble;

	private void Start()
	{
		buttonSubmit.isMostTop = true;
		buttonSubmit.onButtonClick = OnSubmitClick;
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	private void OnSubmitClick(AuiButton sender)
	{
		Hide();
		if (PlayInfo.playerData.tutorialMode)
		{
			procCastle.tutorialActive = true;
			procCastle.uiPlayTutorial.Show();
		}
	}

	private void Update()
	{
		if (isStarted)
		{
			return;
		}
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 mousePosition = Input.mousePosition;
			if (buttonSubmit.uiCamera != null)
			{
				Vector3 vector = buttonSubmit.uiCamera.ScreenToWorldPoint(mousePosition);
				if (vector.x >= -75f && vector.x <= 70f && vector.y >= -140f && vector.y <= 38f)
				{
					StartCoroutine("StartMarbleAnimation");
				}
			}
		}
		if (matMarble != null)
		{
			marbleAlpha += (float)((!marbleLight) ? 1 : (-1)) * Time.deltaTime;
			if (marbleAlpha < 0f)
			{
				marbleLight = false;
				marbleAlpha = 0f;
			}
			if (marbleAlpha > 1f)
			{
				marbleLight = true;
				marbleAlpha = 1f;
			}
			Color color = new Color(1f, 1f, 1f, marbleAlpha);
			iconSelected.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_Color", color);
		}
	}

	private IEnumerator StartMarbleAnimation()
	{
		isStarted = true;
		aniBeforeSelect.visible = false;
		aniAfterSelect.visible = true;
		aniAfterSelect.StartAnimation(0, false, true);
		iconSelected.visible = false;
		yield return new WaitForSeconds(0.5f);
		StartTakeFortune();
	}

	private void StartTakeFortune()
	{
		textTalk.text = string.Empty;
		TransformAnimation component = cardBoard.gameObject.GetComponent<TransformAnimation>();
		component.moveFrom = cardBoard.transform.localPosition;
		component.moveTo = cardBoard.transform.localPosition;
		component.moveFrom.x += 1000f;
		cardBoard.transform.localPosition = component.moveFrom;
		component.speed = 8000f;
		component.StartMove();
		cardBoard.visible = true;
		StartCoroutine("CardAnimation");
	}

	private IEnumerator CardAnimation()
	{
		yield return new WaitForSeconds(0.5f);
		for (int i = 0; i < 4; i++)
		{
			TransformAnimation transAni = sprCard[i].gameObject.GetComponent<TransformAnimation>();
			transAni.moveFrom = sprCard[i].transform.localPosition;
			transAni.moveTo = sprCard[i].transform.localPosition;
			transAni.moveFrom.x = 0f;
			transAni.moveFrom.y += 400f;
			sprCard[i].transform.localPosition = transAni.moveFrom;
			transAni.speed = 6000f;
			transAni.delay = 0.3f * (float)i;
			transAni.StartMove();
			sprCard[i].visible = true;
			sprCard[i].SetFrame(0);
		}
		yield return new WaitForSeconds(1f);
		int maxCardMix = FortuneSystem.fortuneCardMix.Length;
		float[] rate = new float[maxCardMix];
		for (int i2 = 0; i2 < maxCardMix; i2++)
		{
			for (int j2 = 0; j2 < 3; j2++)
			{
				if (PlayInfo.heroState.level <= FortuneSystem.fortuneCardMix[i2].stepLevel[j2])
				{
					rate[i2] = FortuneSystem.fortuneCardMix[i2].stepRate[j2];
					break;
				}
			}
		}
		float randValue = Random.Range(0, 100);
		int cardMixIdx = maxCardMix - 1;
		for (int n = 0; n < maxCardMix; n++)
		{
			randValue -= rate[n];
			if (randValue <= 0f)
			{
				cardMixIdx = n;
				break;
			}
		}
		PlayInfo.fortuneSystem.SetCardMix(cardMixIdx);
		int[] cardResult = new int[4];
		for (int m = 0; m < 4; m++)
		{
			cardResult[m] = 1;
		}
		for (int l = 0; l < cardMixIdx + 1; l++)
		{
			cardResult[l] = 2;
		}
		for (int k = 0; k < 4; k++)
		{
			int chg = Random.Range(0, 4);
			if (chg != k)
			{
				int k2 = cardResult[k];
				cardResult[k] = cardResult[chg];
				cardResult[chg] = k2;
			}
		}
		for (int j = 0; j < 4; j++)
		{
			float rotY2 = 0f;
			for (int j4 = 0; j4 < 3; j4++)
			{
				yield return new WaitForSeconds(0.03f);
				rotY2 += 30f;
				Quaternion quot = Quaternion.Euler(0f, rotY2, 0f);
				sprCard[j].transform.localRotation = quot;
			}
			rotY2 += 180f;
			sprCard[j].SetFrame(cardResult[j]);
			for (int j3 = 0; j3 < 3; j3++)
			{
				yield return new WaitForSeconds(0.03f);
				rotY2 += 30f;
				Quaternion quot2 = Quaternion.Euler(0f, rotY2, 0f);
				sprCard[j].transform.localRotation = quot2;
			}
			sprCard[j].transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		}
		buttonSubmit.visible = true;
		buttonSubmitLabel.visible = true;
		switch (cardMixIdx)
		{
		case 0:
			textTalk.text = StringContent.msgFortuneResultGood;
			break;
		case 1:
			textTalk.text = StringContent.msgFortuneResultNormal;
			break;
		case 2:
			textTalk.text = StringContent.msgFortuneResultBad;
			break;
		case 3:
			textTalk.text = StringContent.msgFortuneResultTerrible;
			break;
		}
		sprSorcerer.visible = true;
	}

	public void Hide()
	{
		base.gameObject.SetActiveRecursively(false);
		AuiButton.mostTopActive = false;
		if (procCastle.uiMap.gameObject.active)
		{
			procCastle.bgmMap.volume = (float)UserSetting.volumeBgm / 100.9f;
			UserSetting.currentBgmSound = procCastle.bgmMap;
			procCastle.bgmMap.Play();
		}
		else
		{
			procCastle.bgmCastle.volume = (float)UserSetting.volumeBgm / 100.9f;
			UserSetting.currentBgmSound = procCastle.bgmCastle;
			procCastle.bgmCastle.Play();
		}
		procCastle.bgmFortune.Pause();
		PlayInfo.gameTime.pause = false;
	}

	public void Show()
	{
		base.gameObject.SetActiveRecursively(true);
		AuiButton.mostTopActive = true;
		buttonSubmit.visible = false;
		buttonSubmitLabel.visible = false;
		cardBoard.visible = false;
		AuiSprite[] array = sprCard;
		foreach (AuiSprite auiSprite in array)
		{
			auiSprite.visible = false;
		}
		sprSorcerer.visible = false;
		aniAfterSelect.visible = false;
		iconSelected.visible = true;
		aniBeforeSelect.visible = true;
		textTalk.text = StringContent.msgFortuneStart;
		isStarted = false;
		marbleAlpha = 1f;
		marbleLight = true;
		matMarble = iconSelected.transform.GetChild(0).GetComponent<Renderer>().material;
		procCastle.bgmFortune.volume = (float)UserSetting.volumeBgm / 100.9f;
		UserSetting.currentBgmSound = procCastle.bgmFortune;
		procCastle.bgmMap.Pause();
		procCastle.bgmCastle.Pause();
		procCastle.bgmFortune.Play();
		PlayInfo.gameTime.pause = true;
		if (PlayInfo.playerData.tutorialMode)
		{
			procCastle.interControl.UserInputEnable(false);
		}
	}
}
