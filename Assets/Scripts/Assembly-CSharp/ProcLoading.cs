using UnityEngine;

public class ProcLoading : MonoBehaviour
{
	public enum ContentMode
	{
		mainmenu = 0,
		castle = 1,
		battle = 2
	}

	public AuiSpriteAnimation aniProgress;

	public TextMesh textDesc;

	public AuiSprite imgDesc;

	[System.NonSerialized]
	public AsyncOperation loadingOperation;

	private static GameObject befLoading;

	private StoreLib storeLib;

	private bool isLoaded;

	private float loadTime;

	public void Show(ContentMode mode)
	{
		if (befLoading != null)
		{
			Object.Destroy(befLoading);
		}
		befLoading = base.gameObject;
		switch (mode)
		{
		case ContentMode.mainmenu:
			textDesc.text = LoadingContent.GetRandomStringCastle();
			break;
		case ContentMode.castle:
			textDesc.text = LoadingContent.GetRandomStringCastle();
			break;
		case ContentMode.battle:
			textDesc.text = LoadingContent.GetRandomStringBattle();
			break;
		}
		aniProgress.StartAnimation(true, false);
		isLoaded = false;
		loadTime = 0f;
		imgDesc.SetFrame(Random.Range(0, imgDesc.materials.Length));
		Object.DontDestroyOnLoad(this);
		storeLib = base.gameObject.AddComponent<StoreLib>();
		if (UserSetting.tabletMode)
		{
			storeLib.ShowAd(StoreLib.AdPos.centertop);
		}
		else
		{
			storeLib.ShowAd(StoreLib.AdPos.righttop);
		}
		ProcBase.ChangeTextMeshLanguage();
	}

	private void Update()
	{
		if (loadingOperation == null || loadingOperation.isDone)
		{
			isLoaded = true;
		}
		if (isLoaded)
		{
			loadTime += Time.deltaTime;
			if (loadTime > 0.5f)
			{
				befLoading = null;
				Object.Destroy(base.gameObject);
				storeLib.HideAd();
			}
		}
	}
}
