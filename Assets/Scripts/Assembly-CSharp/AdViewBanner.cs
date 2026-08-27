using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class AdViewBanner : MonoBehaviour
{
	public TextMesh textMessage;

	public GameObject objBanner;

	public GameObject backBanner;

	public AuiSprite iconCurrency;

	public TextMesh textReward;

	public AuiButton buttonClose;

	private AlphaAd alphaAd;

	private AlphaAd.AdList list;

	private AlphaAdManager.OnBannerClose procBannerClose;

	private bool isLinking;

	public void Show(Camera uiCamera, AlphaAd alphaAd, AlphaAd.AdList list, AlphaAdManager.OnBannerClose proc)
	{
		this.alphaAd = alphaAd;
		this.list = list;
		procBannerClose = proc;
		AuiButton.modalActive = true;
		base.gameObject.SetActiveRecursive(true);
		buttonClose.uiCamera = uiCamera;
		buttonClose.enabled = true;
		buttonClose.onButtonClick = OnCloseClick;
		buttonClose.isModal = true;
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
		isLinking = false;
		StartCoroutine("DropShow");
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	private IEnumerator DropShow()
	{
		Vector3 pos = base.transform.position;
		pos.y = 380f;
		base.transform.position = pos;
		yield return 1;
		while (pos.y > 261f)
		{
			pos.y -= Time.deltaTime * 200f;
			base.transform.position = pos;
			yield return 1;
		}
		pos.y = 261f;
		base.transform.position = pos;
		yield return new WaitForSeconds(5f);
		if (!isLinking)
		{
			Hide();
		}
	}

	private IEnumerator DropHide()
	{
		Vector3 pos = base.transform.position;
		while (pos.y < 380f)
		{
			pos.y += Time.deltaTime * 200f;
			base.transform.position = pos;
			yield return 1;
		}
		pos.y = 380f;
		base.transform.position = pos;
		base.gameObject.SetActiveRecursive(false);
		if (procBannerClose != null)
		{
			procBannerClose();
		}
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

	private void Update()
	{
		if (!isLinking && Input.GetMouseButtonDown(0))
		{
			Vector3 mousePosition = Input.mousePosition;
			mousePosition = buttonClose.uiCamera.ScreenToWorldPoint(mousePosition);
			Bounds bounds = backBanner.gameObject.GetComponent<Renderer>().bounds;
			if (mousePosition.x >= bounds.min.x && mousePosition.x <= bounds.max.x && mousePosition.y >= bounds.min.y && mousePosition.y <= bounds.max.y)
			{
				OnBannerClick();
			}
			else if (mousePosition.y < bounds.min.y)
			{
				Hide();
			}
		}
	}

	public void Hide()
	{
		isLinking = true;
		AuiButton.modalActive = false;
		buttonClose.enabled = false;
		StopAllCoroutines();
		StartCoroutine("DropHide");
	}

	private void OnBannerClick()
	{
		isLinking = true;
		buttonClose.enabled = false;
		if (!alphaAd.LinkClick(UserSetting.currentSaveSlot, list.code, OnResultLinkClick))
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
		Hide();
		if (!isSucceed)
		{
			ProcBase.ShowMsg(StringContent.msgAlphaAdError, MessageView.MsgIcon.alert, true);
		}
	}
}
