using UnityEngine;

public class UserSetting
{
	public enum GraphicsQuality
	{
		fast = 0,
		good = 1,
		beautiful = 2
	}

	public static AudioSource currentBgmSound;

	public static bool isInit;

	public static bool tabletMode = true;

	public static int volumeEffect = 100;

	public static int volumeBgm = 100;

	public static GraphicsQuality quality = GraphicsQuality.beautiful;

	public static Language language;

	public static int currentSaveSlot;

	public static bool viewGameHelp = true;

	public static void Init()
	{
		if (isInit)
		{
			return;
		}
		isInit = true;
		tabletMode = ScreenSize.IsTablet;
		switch (Application.systemLanguage)
		{
		case SystemLanguage.Korean:
			language = Language.korean;
			break;
		case SystemLanguage.Japanese:
			language = Language.japanese;
			break;
		default:
			language = Language.english;
			break;
		}
		if (SystemInfo.processorCount > 1)
		{
			quality = GraphicsQuality.beautiful;
		}
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if (SystemInfo.deviceName.Equals("iPad") || SystemInfo.deviceName.Equals("iPod Touch"))
			{
				quality = GraphicsQuality.good;
			}
			else if (Screen.width <= 480)
			{
				quality = GraphicsQuality.fast;
			}
			else
			{
				quality = GraphicsQuality.good;
			}
		}
		else if (ScreenSize.Height <= 480)
		{
			quality = GraphicsQuality.fast;
		}
		else
		{
			quality = GraphicsQuality.good;
		}
		Load();
		if (!Application.isEditor)
		{
			if (quality == GraphicsQuality.fast)
			{
				QualitySettings.SetQualityLevel(0, true);
			}
			else if (quality == GraphicsQuality.good)
			{
				QualitySettings.SetQualityLevel(1, true);
			}
			else if (quality == GraphicsQuality.beautiful)
			{
				QualitySettings.SetQualityLevel(3, true);
			}
		}
	}

	public static void Save()
	{
		PlayerPrefs.SetInt("tabletMode", tabletMode ? 1 : 0);
		PlayerPrefs.SetInt("volumeEffect", volumeEffect);
		PlayerPrefs.SetInt("volumeBgm", volumeBgm);
		PlayerPrefs.SetInt("quality", (int)quality);
		PlayerPrefs.SetInt("language", (int)language);
		PlayerPrefs.SetInt("currentSaveSlot", currentSaveSlot);
		PlayerPrefs.SetInt("viewGameHelp", viewGameHelp ? 1 : 0);
	}

	public static void Load()
	{
		if (PlayerPrefs.HasKey("tabletMode"))
		{
			tabletMode = PlayerPrefs.GetInt("tabletMode") == 1;
		}
		if (PlayerPrefs.HasKey("volumeEffect"))
		{
			volumeEffect = PlayerPrefs.GetInt("volumeEffect");
		}
		if (PlayerPrefs.HasKey("volumeBgm"))
		{
			volumeBgm = PlayerPrefs.GetInt("volumeBgm");
		}
		if (PlayerPrefs.HasKey("quality"))
		{
			quality = (GraphicsQuality)PlayerPrefs.GetInt("quality");
		}
		if (PlayerPrefs.HasKey("language"))
		{
			language = (Language)PlayerPrefs.GetInt("language");
		}
		if (PlayerPrefs.HasKey("currentSaveSlot"))
		{
			currentSaveSlot = PlayerPrefs.GetInt("currentSaveSlot");
		}
		if (PlayerPrefs.HasKey("viewGameHelp"))
		{
			viewGameHelp = PlayerPrefs.GetInt("viewGameHelp") == 1;
		}
	}
}
