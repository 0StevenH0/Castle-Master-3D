using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class AdViewFull : MonoBehaviour
{
	public TextMesh textMessage;

	public GameObject objBanner;

	public AuiSprite iconCurrency;

	public TextMesh textReward;

	public AuiButton buttonYes;

	public AuiButton buttonNo;

	private AlphaAd alphaAd;

	private AlphaAd.AdList list;

	private AlphaAdManager.OnBannerClose procBannerClose;

	private bool isError;

	public void Show(Camera uiCamera, AlphaAd alphaAd, AlphaAd.AdList list, AlphaAdManager.OnBannerClose proc)
	{
		this.alphaAd = alphaAd;
		this.list = list;
		procBannerClose = proc;
		AuiButton.modalActive = true;
		base.gameObject.SetActiveRecursive(true);
		buttonYes.uiCamera = uiCamera;
		buttonNo.uiCamera = uiCamera;
		buttonYes.isModal = true;
		buttonNo.isModal = true;
		buttonYes.enabled = true;
		buttonNo.enabled = true;
		buttonYes.onButtonClick = OnYesClick;
		buttonNo.onButtonClick = OnNoClick;
		objBanner.gameObject.SetActive(false);
		textMessage.gameObject.SetActive(true);
		textMessage.text = list.explain.Replace("<br/>", "\n");
		textReward.text = list.item_cnt.ToString();
		switch (list.item_code)
		{
		case "1001":
			iconCurrency.SetFrame(0);
			break;
		case "1002":
			iconCurrency.SetFrame(1);
			break;
		case "1003":
			iconCurrency.SetFrame(2);
			break;
		}
		if (list.adv_img.Length > 0)
		{
			StartCoroutine(LoadBannerImage(list.adv_img));
		}
		isError = false;
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	private IEnumerator LoadBannerImage(string url)
	{
		UnityWebRequest web = UnityWebRequestTexture.GetTexture(url);
		yield return web.SendWebRequest();
		if (web.result == UnityWebRequest.Result.Success)
		{
			objBanner.GetComponent<Renderer>().material.mainTexture = ((DownloadHandlerTexture)web.downloadHandler).texture;
			objBanner.gameObject.SetActive(true);
			textMessage.gameObject.SetActive(false);
		}
	}

	public void Hide()
	{
		AuiButton.modalActive = false;
		StopAllCoroutines();
		base.gameObject.SetActiveRecursive(false);
		if (isError)
		{
			ProcBase.ShowMsg(StringContent.msgAlphaAdError, MessageView.MsgIcon.alert, true);
		}
		if (procBannerClose != null)
		{
			procBannerClose();
		}
	}

	private void OnYesClick(AuiButton sender)
	{
		buttonYes.enabled = false;
		buttonNo.enabled = false;
		if (!alphaAd.LinkClick(UserSetting.currentSaveSlot, list.code, OnResultLinkClick))
		{
			Hide();
			isError = true;
		}
	}

	private void OnNoClick(AuiButton sender)
	{
		Hide();
	}

	private void OnResultLinkClick(bool isSucceed)
	{
		if (isSucceed)
		{
			switch (list.item_code)
			{
			case "1001":
				PlayInfo.playerData.gem += int.Parse(list.item_cnt);
				break;
			case "1002":
				PlayInfo.playerData.gold += int.Parse(list.item_cnt);
				break;
			case "1003":
				PlayInfo.playerData.cmdPts += int.Parse(list.item_cnt);
				break;
			}
			Application.OpenURL(list.link);
		}
		else
		{
			isError = true;
		}
		Hide();
	}
}
