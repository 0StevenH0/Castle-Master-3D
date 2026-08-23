using System.Collections;
using UnityEngine;

public class ProcTitle : ProcBase
{
	public GameObject bgiLogo;

	public AuiButton buttonStart;

	public AuiButton buttonOption;

	public AuiButton buttonTutorial;

	public AuiButton buttonExit;

	public GameObject panelMenu;

	public UIUserSetting uiUserSetting;

	public UITutorial uiTutorial;

	public GameObject iconGrade;

	public GameObject iconPlus;

	private static bool isFirstLoad = true;

	private StoreLib storeLib;

	public override void OnStart()
	{
		RenderSettings.fog = false;
		ProcMain.InitGame();
		buttonStart.onButtonClick = OnStartClick;
		buttonOption.onButtonClick = OnOptionClick;
		buttonTutorial.onButtonClick = OnTutorialClick;
		buttonExit.onButtonClick = OnExitClick;
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			buttonExit.visible = false;
		}
		AudioSource audioSource = base.gameObject.AddComponent<AudioSource>();
		audioSource.clip = ResourceManager.Load("Sound/bgm", "bgm_title", typeof(AudioClip)) as AudioClip;
		audioSource.loop = true;
		audioSource.volume = (float)UserSetting.volumeBgm / 100f;
		audioSource.Play();
		UserSetting.currentBgmSound = audioSource;
		AuiButton.SetCameraAllChild(uiUserSetting.transform, buttonStart.uiCamera);
		uiUserSetting.Hide();
		AuiButton.SetCameraAllChild(uiTutorial.transform, buttonStart.uiCamera);
		uiTutorial.Hide();
		storeLib = StoreLib.LoadStoreLib();
		if (isFirstLoad)
		{
			isFirstLoad = false;
			StartCoroutine("OverlapLogo");
			storeLib.InitAd();
		}
		else
		{
			bgiLogo.active = false;
			iconPlus.gameObject.active = PlusType.isPlus;
		}
		StartCoroutine("WaitForStartAd");
	}

	private IEnumerator WaitForStartAd()
	{
		yield return new WaitForSeconds(0.5f);
		if (UserSetting.tabletMode)
		{
			storeLib.ShowAd(StoreLib.AdPos.centertop);
			Vector3 pos2 = iconGrade.transform.localPosition;
			pos2.y = 234f;
			iconGrade.transform.localPosition = pos2;
		}
		else
		{
			storeLib.ShowAd(StoreLib.AdPos.righttop);
			Vector3 pos = iconGrade.transform.localPosition;
			pos.y = 134f;
			iconGrade.transform.localPosition = pos;
		}
	}

	private IEnumerator OverlapLogo()
	{
		panelMenu.gameObject.SetActiveRecursively(false);
		Color colorLogo = new Color(1f, 1f, 1f, 1f);
		Material mtrLogo = bgiLogo.GetComponent<Renderer>().material;
		bgiLogo.active = true;
		mtrLogo.SetColor("_Color", colorLogo);
		yield return new WaitForSeconds(0.5f);
		while (colorLogo.a > 0f)
		{
			yield return 1;
			colorLogo.a -= Time.deltaTime;
			if (colorLogo.a < 0f)
			{
				colorLogo.a = 0f;
			}
			mtrLogo.SetColor("_Color", colorLogo);
		}
		bgiLogo.active = false;
		panelMenu.gameObject.SetActiveRecursively(true);
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			buttonExit.visible = false;
		}
		iconPlus.gameObject.active = PlusType.isPlus;
		if (StoreType.store != StoreType.Store.tstore && StoreType.store != StoreType.Store.olleh)
		{
			iconGrade.active = false;
		}
		ProcBase.ChangeTextMeshLanguage();
	}

	private void OnStartClick(AuiButton sender)
	{
		buttonStart.enabled = false;
		buttonOption.enabled = false;
		buttonExit.enabled = false;
		ProcBase.LoadScene("Scene PlayerList", ProcLoading.ContentMode.mainmenu);
	}

	private void OnOptionClick(AuiButton sender)
	{
		storeLib.HideAd();
		uiUserSetting.Show();
		StartCoroutine("WaitForTutorialClose");
	}

	private void OnTutorialClick(AuiButton sender)
	{
		storeLib.HideAd();
		uiTutorial.Show();
		StartCoroutine("WaitForTutorialClose");
	}

	private IEnumerator WaitForTutorialClose()
	{
		while (uiTutorial.gameObject.active || uiUserSetting.gameObject.active)
		{
			yield return new WaitForSeconds(0.1f);
		}
		if (UserSetting.tabletMode)
		{
			storeLib.ShowAd(StoreLib.AdPos.centertop);
		}
		else
		{
			storeLib.ShowAd(StoreLib.AdPos.righttop);
		}
	}

	private void OnExitClick(AuiButton sender)
	{
		buttonStart.enabled = false;
		buttonOption.enabled = false;
		buttonExit.enabled = false;
		StartCoroutine("WaitForQuit");
	}

	private IEnumerator WaitForQuit()
	{
		yield return new WaitForSeconds(0.5f);
		if (storeLib != null && !Application.isEditor)
		{
			storeLib.AppStop();
		}
		Application.Quit();
	}
}
