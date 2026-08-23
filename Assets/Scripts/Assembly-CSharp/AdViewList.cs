using System.Collections;
using UnityEngine;

public class AdViewList : MonoBehaviour
{
	private const int maxLink = 4;

	public TextMesh textTitle;

	public TextMesh textDesc;

	public AdViewLink objLink;

	public AuiButton buttonClose;

	private AdViewLink[] linkList;

	private AlphaAd alphaAd;

	private AlphaAd.AdList[] adList;

	private AlphaAdManager.OnBannerClose procBannerClose;

	private int selectedIndex;

	private bool isLinking;

	public void Show(Camera uiCamera, AlphaAd alphaAd, AlphaAd.AdList[] list, AlphaAdManager.OnBannerClose proc)
	{
		textTitle.text = StringContent.msgAlphaAdTitle;
		textDesc.text = StringContent.msgAlphaAdNotify;
		this.alphaAd = alphaAd;
		this.adList = list;
		procBannerClose = proc;
		AuiButton.modalActive = true;
		base.gameObject.SetActiveRecursively(true);
		buttonClose.uiCamera = uiCamera;
		buttonClose.isModal = true;
		buttonClose.enabled = true;
		buttonClose.onButtonClick = OnCloseClick;
		if (linkList == null)
		{
			linkList = new AdViewLink[4];
			for (int i = 0; i < 4; i++)
			{
				if (i == 0)
				{
					linkList[i] = objLink;
				}
				else
				{
					linkList[i] = Object.Instantiate(objLink) as AdViewLink;
					linkList[i].transform.parent = objLink.transform.parent;
					Vector3 localPosition = objLink.transform.localPosition;
					localPosition.y = objLink.transform.localPosition.y - (float)(i * 120);
					linkList[i].transform.localPosition = localPosition;
					linkList[i].imgBanner.GetComponent<Renderer>().sharedMaterial = objLink.imgBanner.GetComponent<Renderer>().material;
				}
				linkList[i].bannerBound = linkList[i].imgBanner.GetComponent<Renderer>().bounds;
			}
		}
		int num = 0;
		AlphaAd.AdList[] array = this.adList;
		foreach (AlphaAd.AdList adList in array)
		{
			linkList[num].gameObject.SetActiveRecursively(true);
			linkList[num].imgBanner.active = false;
			linkList[num].textMessage.gameObject.active = true;
			linkList[num].textReward.gameObject.active = true;
			linkList[num].textMessage.text = adList.explain.Replace("<br/>", "\n");
			linkList[num].textReward.text = adList.item_cnt.ToString();
			switch (adList.item_code)
			{
			case "1001":
				linkList[num].iconCurrency.SetFrame(0);
				break;
			case "1002":
				linkList[num].iconCurrency.SetFrame(1);
				break;
			case "1003":
				linkList[num].iconCurrency.SetFrame(2);
				break;
			}
			num++;
			if (num >= 4)
			{
				break;
			}
		}
		for (int k = num; k < linkList.Length; k++)
		{
			linkList[k].gameObject.SetActiveRecursively(false);
		}
		isLinking = false;
		StartCoroutine("LoadBannerImage");
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	private IEnumerator LoadBannerImage()
	{
		int num = 0;
		AlphaAd.AdList[] array = adList;
		foreach (AlphaAd.AdList item in array)
		{
			if (item.adv_img.Length > 0)
			{
				WWW web = new WWW(item.adv_img);
				yield return web;
				if (web.error == null && web.texture != null)
				{
					linkList[num].imgBanner.GetComponent<Renderer>().material.mainTexture = web.texture;
					linkList[num].imgBanner.gameObject.active = true;
					linkList[num].textMessage.gameObject.active = false;
				}
			}
			num++;
			if (num >= 4)
			{
				break;
			}
		}
	}

	private void Update()
	{
		if (isLinking || !Input.GetMouseButtonDown(0))
		{
			return;
		}
		Vector3 mousePosition = Input.mousePosition;
		mousePosition = buttonClose.uiCamera.ScreenToWorldPoint(mousePosition);
		int num = 0;
		AdViewLink[] array = linkList;
		foreach (AdViewLink adViewLink in array)
		{
			if (num >= adList.Length)
			{
				break;
			}
			Bounds bannerBound = adViewLink.bannerBound;
			if (mousePosition.x >= bannerBound.min.x && mousePosition.x <= bannerBound.max.x && mousePosition.y >= bannerBound.min.y && mousePosition.y <= bannerBound.max.y)
			{
				OnBannerClick(num);
				break;
			}
			num++;
			if (num >= 4)
			{
				break;
			}
		}
	}

	public void Hide()
	{
		AuiButton.modalActive = false;
		StopAllCoroutines();
		base.gameObject.SetActiveRecursively(false);
		if (procBannerClose != null)
		{
			procBannerClose();
		}
	}

	private void OnBannerClick(int num)
	{
		isLinking = true;
		selectedIndex = num;
		if (!alphaAd.LinkClick(UserSetting.currentSaveSlot, adList[num].code, OnResultLinkClick))
		{
			Hide();
			ProcBase.ShowMsg(StringContent.msgAlphaAdError, MessageView.MsgIcon.alert, true);
		}
	}

	private void OnCloseClick(AuiButton sender)
	{
		Hide();
	}

	private void OnResultLinkClick(bool isSucceed)
	{
		if (isSucceed)
		{
			switch (adList[selectedIndex].item_code)
			{
			case "1001":
				PlayInfo.playerData.gem += int.Parse(adList[selectedIndex].item_cnt);
				break;
			case "1002":
				PlayInfo.playerData.gold += int.Parse(adList[selectedIndex].item_cnt);
				break;
			case "1003":
				PlayInfo.playerData.cmdPts += int.Parse(adList[selectedIndex].item_cnt);
				break;
			}
			Application.OpenURL(adList[selectedIndex].link);
		}
		Hide();
		if (!isSucceed)
		{
			ProcBase.ShowMsg(StringContent.msgAlphaAdError, MessageView.MsgIcon.alert, true);
		}
	}
}
