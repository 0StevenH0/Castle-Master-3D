using System.Collections;
using UnityEngine;

public class UIUserSetting : MonoBehaviour
{
	private const float volumeMinX = -240f;

	private const float volumeMaxX = -4f;

	private const float volumeFxY = 118f;

	private const float volumeBgmY = 39f;

	private const float volumeHeight = 32f;

	public AuiButton buttonSubmit;

	public AuiButton buttonClose;

	public AuiSprite iconSoundFx;

	public AuiSprite iconSoundBgm;

	public AuiButton buttonChgLangPrev;

	public AuiButton buttonChgLangNext;

	public AuiButton[] buttonQuality;

	public AuiButton[] buttonDevice;

	public AuiSprite iconLang;

	public AuiSprite[] labelQuality;

	public AuiSprite[] labelDevice;

	private bool tabletMode = true;

	private int volumeEffect = 100;

	private int volumeBgm = 100;

	private UserSetting.GraphicsQuality quality = UserSetting.GraphicsQuality.beautiful;

	private Language language;

	private void Start()
	{
		Camera camera = GameObject.Find("UICamera").GetComponent<Camera>();
		if (camera != null)
		{
			AuiButton.SetCameraAllChild(base.transform, camera);
		}
		buttonSubmit.onButtonClick = OnSubmitClick;
		buttonClose.onButtonClick = OnCloseClick;
		buttonChgLangPrev.onButtonClick = OnChangeLangClick;
		buttonChgLangNext.onButtonClick = OnChangeLangClick;
		AuiButton[] array = buttonQuality;
		foreach (AuiButton auiButton in array)
		{
			auiButton.onButtonClick = OnQualityClick;
		}
		AuiButton[] array2 = buttonDevice;
		foreach (AuiButton auiButton2 in array2)
		{
			auiButton2.onButtonClick = OnDeviceClick;
		}
		Refresh();
		StartCoroutine("FirstRefresh");
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	private IEnumerator FirstRefresh()
	{
		yield return 1;
		Refresh();
	}

	public void Show()
	{
		AuiButton.modalActive = true;
		base.gameObject.SetActive(true);
		tabletMode = UserSetting.tabletMode;
		volumeEffect = UserSetting.volumeEffect;
		volumeBgm = UserSetting.volumeBgm;
		quality = UserSetting.quality;
		language = UserSetting.language;
		Refresh();
	}

	public void Hide()
	{
		AuiButton.modalActive = false;
		base.gameObject.SetActive(false);
	}

	private void Refresh()
	{
		float num = 236f;
		Vector3 position = iconSoundFx.transform.position;
		position.x = -240f + (float)volumeEffect / 100f * num;
		iconSoundFx.transform.position = position;
		position = iconSoundBgm.transform.position;
		position.x = -240f + (float)volumeBgm / 100f * num;
		iconSoundBgm.transform.position = position;
		iconLang.SetFrame((int)language);
		for (int i = 0; i < buttonQuality.Length; i++)
		{
			bool flag = i == (int)quality;
			buttonQuality[i].SetFrame(flag ? 1 : 0);
			labelQuality[i].SetFrame(flag ? 1 : 0);
		}
		for (int j = 0; j < buttonDevice.Length; j++)
		{
			bool flag2 = j == (tabletMode ? 1 : 0);
			buttonDevice[j].SetFrame(flag2 ? 1 : 0);
			labelDevice[j].SetFrame(flag2 ? 1 : 0);
		}
	}

	private void Update()
	{
		if (!Input.GetMouseButton(0))
		{
			return;
		}
		Vector3 mousePosition = Input.mousePosition;
		mousePosition = buttonSubmit.uiCamera.ScreenToWorldPoint(mousePosition);
		if (mousePosition.x >= -240f && mousePosition.x <= -4f)
		{
			float num = 236f;
			int num2 = (int)((mousePosition.x - -240f) / num * 100f);
			if (num2 < 5)
			{
				num2 = 0;
			}
			if (num2 > 95)
			{
				num2 = 100;
			}
			if (mousePosition.y >= 86f && mousePosition.y <= 150f)
			{
				volumeEffect = num2;
				UserSetting.volumeEffect = volumeEffect;
				Refresh();
				ResetVolume();
			}
			else if (mousePosition.y >= 7f && mousePosition.y <= 71f)
			{
				volumeBgm = num2;
				UserSetting.volumeBgm = volumeBgm;
				Refresh();
				ResetVolume();
			}
		}
	}

	private void OnChangeLangClick(AuiButton sender)
	{
		int num = (int)language;
		num += sender.buttonTag;
		if (num < 0)
		{
			num = 2;
		}
		if (num >= 3)
		{
			num = 0;
		}
		language = (Language)num;
		Refresh();
	}

	private void OnQualityClick(AuiButton sender)
	{
		int buttonTag = sender.buttonTag;
		quality = (UserSetting.GraphicsQuality)buttonTag;
		Refresh();
	}

	private void OnDeviceClick(AuiButton sender)
	{
		int buttonTag = sender.buttonTag;
		tabletMode = buttonTag == 1;
		Refresh();
	}

	private void ResetVolume()
	{
		try
		{
			if (UserSetting.currentBgmSound != null)
			{
				UserSetting.currentBgmSound.volume = (float)volumeBgm / 100f;
			}
		}
		catch
		{
			UserSetting.currentBgmSound = null;
		}
	}

	private void OnSubmitClick(AuiButton sender)
	{
		bool flag = UserSetting.language != language;
		UserSetting.tabletMode = tabletMode;
		UserSetting.volumeEffect = volumeEffect;
		UserSetting.volumeBgm = volumeBgm;
		UserSetting.quality = quality;
		UserSetting.language = language;
		ResetVolume();
		UserSetting.Save();
		Hide();
		if (flag)
		{
			ProcMain.InitString();
		}
		ProcBase.ShowMsg(StringContent.msgSettingChanged, MessageView.MsgIcon.alert);
	}

	private void OnCloseClick(AuiButton sender)
	{
		Hide();
	}
}
