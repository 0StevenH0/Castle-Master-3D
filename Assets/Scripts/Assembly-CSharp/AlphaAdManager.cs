using UnityEngine;

public class AlphaAdManager : MonoBehaviour
{
	private enum AdShowMode
	{
		fullscreen = 0,
		list = 1,
		banner = 2
	}

	public delegate void OnBannerClose();

	public AlphaAd alphaAd;

	public AdViewFull adViewFull;

	public AdViewList adViewList;

	public AdViewBanner adViewBanner;

	public Camera uiCamera;

	private AdShowMode bmode;

	private OnBannerClose procBannerClose;

	public void ShowStartAd(OnBannerClose proc)
	{
		procBannerClose = proc;
		bmode = AdShowMode.fullscreen;
		if (!alphaAd.RequestList(UserSetting.currentSaveSlot, AlphaAd.AdPos.start, OnResultList) && procBannerClose != null)
		{
			procBannerClose();
		}
	}

	public void ShowFinishAd(OnBannerClose proc)
	{
		procBannerClose = proc;
		bmode = AdShowMode.fullscreen;
		if (!alphaAd.RequestList(UserSetting.currentSaveSlot, AlphaAd.AdPos.finish, OnResultList) && procBannerClose != null)
		{
			procBannerClose();
		}
	}

	public void ShowListAd(OnBannerClose proc)
	{
		procBannerClose = proc;
		bmode = AdShowMode.list;
		if (!alphaAd.RequestList(UserSetting.currentSaveSlot, AlphaAd.AdPos.list, OnResultList))
		{
			if (procBannerClose != null)
			{
				procBannerClose();
			}
			ProcBase.ShowMsg(StringContent.msgAlphaAdError, MessageView.MsgIcon.alert, true);
		}
	}

	public void ShowListBanner(OnBannerClose proc)
	{
		procBannerClose = proc;
		bmode = AdShowMode.banner;
		if (!alphaAd.RequestList(UserSetting.currentSaveSlot, AlphaAd.AdPos.list, OnResultList) && procBannerClose != null)
		{
			procBannerClose();
		}
	}

	private void OnResultList(bool isSucceed, int count, AlphaAd.AdList[] adList)
	{
		if (isSucceed)
		{
			if (bmode == AdShowMode.list)
			{
				if (adViewList != null)
				{
					adViewList.Show(uiCamera, alphaAd, adList, procBannerClose);
				}
			}
			else if (bmode == AdShowMode.fullscreen)
			{
				if (adViewFull != null)
				{
					adViewFull.Show(uiCamera, alphaAd, adList[0], procBannerClose);
				}
			}
			else if (bmode == AdShowMode.banner && adViewBanner != null)
			{
				adViewBanner.Show(uiCamera, alphaAd, adList[Random.Range(0, adList.Length)], procBannerClose);
			}
		}
		else
		{
			if (procBannerClose != null)
			{
				procBannerClose();
			}
			if (bmode == AdShowMode.list)
			{
				ProcBase.ShowMsg(StringContent.msgAlphaAdNoMore, MessageView.MsgIcon.alert, true);
			}
		}
	}
}
