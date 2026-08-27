using UnityEngine;

public class UIGemShop : MonoBehaviour
{
	private const int maxSaleCount = 4;

	private const int cntGemItem = 2;

	public AuiButton buttonClose;

	public AuiButton[] buttonBuy;

	public TextMesh[] textQty;

	public TextMesh[] textCost;

	public AuiButton freeCharge;

	private int[] saleQty = new int[4];

	private int[] saleMax = new int[4];

	private float[] saleCost = new float[4];

	private StoreLib storeLib;

	private int buyCode;

	private void Start()
	{
		LoadSaleList();
		buttonClose.onButtonClick = OnCloseClick;
		buttonClose.isMostTop = true;
		freeCharge.onButtonClick = OnFreeChargeClick;
		freeCharge.isMostTop = true;
		for (int i = 0; i < 4; i++)
		{
			buttonBuy[i].onButtonClick = OnBuyClick;
			buttonBuy[i].buttonTag = i;
			buttonBuy[i].isMostTop = true;
		}
		ActiveButton(true);
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	private void ActiveButton(bool act)
	{
		buttonClose.enabled = act;
		freeCharge.enabled = act;
		for (int i = 0; i < 4; i++)
		{
			buttonBuy[i].enabled = act;
		}
	}

	[System.Serializable]
	private class GemShopRow
	{
		public string currency;

		public int qty;

		public int max;

		public float cost_usd;

		public int cost_krw;
	}

	[System.Serializable]
	private class GemShopRowList
	{
		public GemShopRow[] items;
	}

	private void LoadSaleList()
	{
		TextAsset textAsset = ResourceManager.Load("GameData", "gem_shop", typeof(TextAsset)) as TextAsset;
		if (textAsset == null || string.IsNullOrEmpty(textAsset.text))
		{
			Debug.LogError("Decrypt Error!!");
			return;
		}
		GemShopRowList gemShopRowList = JsonUtility.FromJson<GemShopRowList>("{\"items\":" + textAsset.text + "}");
		int num = 0;
		GemShopRow[] items = gemShopRowList.items;
		foreach (GemShopRow item in items)
		{
			saleQty[num] = item.qty;
			saleMax[num] = item.max;
			if (StoreType.store == StoreType.Store.appstore || StoreType.store == StoreType.Store.android)
			{
				saleCost[num] = item.cost_usd;
			}
			else
			{
				saleCost[num] = item.cost_krw;
			}
			num++;
			if (num >= 4)
			{
				break;
			}
		}
		string text2 = StringContent.signDollar;
		if (StoreType.store != 0 && StoreType.store != StoreType.Store.android)
		{
			text2 = StringContent.signWon;
		}
		for (int i = 0; i < 4; i++)
		{
			textQty[i].text = "x " + saleQty[i];
			textCost[i].text = text2 + " " + saleCost[i];
		}
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	private void OnBuyClick(AuiButton sender)
	{
		int num = (buyCode = sender.buttonTag);
		if (num < 2)
		{
			if (PlayInfo.playerData.gem > saleMax[num])
			{
				ProcBase.ShowMsg(StringContent.msgGemShopResultCannotBuy, MessageView.MsgIcon.alert);
			}
			else if (StoreType.store == StoreType.Store.tstore || StoreType.store == StoreType.Store.olleh || StoreType.store == StoreType.Store.samsung)
			{
				string msgPaymentReGemGold = StringContent.msgPaymentReGemGold;
				msgPaymentReGemGold = msgPaymentReGemGold.Replace(StringContent.strValue, StringContent.wordGem);
				msgPaymentReGemGold = msgPaymentReGemGold.Replace(StringContent.strValue2, saleQty[num].ToString());
				msgPaymentReGemGold = msgPaymentReGemGold.Replace(StringContent.strValue3, saleCost[num].ToString());
				ProcBase.ShowMsg(msgPaymentReGemGold, MessageView.MsgIcon.question, false, new MessageView.MsgButton[2]
				{
					MessageView.MsgButton.yes,
					MessageView.MsgButton.no
				}, OnBuyMessage);
			}
			else
			{
				StartPayment();
				if (storeLib == null)
				{
					storeLib = StoreLib.LoadStoreLib();
				}
				storeLib.BuyItem(StoreLib.ProductType.gem, num, OnResultPayment);
			}
		}
		else if (PlayInfo.playerData.gold > saleMax[num])
		{
			ProcBase.ShowMsg(StringContent.msgGemShopResultCannotBuy, MessageView.MsgIcon.alert);
		}
		else if (StoreType.store == StoreType.Store.tstore || StoreType.store == StoreType.Store.olleh || StoreType.store == StoreType.Store.samsung)
		{
			string msgPaymentReGemGold2 = StringContent.msgPaymentReGemGold;
			msgPaymentReGemGold2 = msgPaymentReGemGold2.Replace(StringContent.strValue, StringContent.wordGold);
			msgPaymentReGemGold2 = msgPaymentReGemGold2.Replace(StringContent.strValue2, saleQty[num].ToString());
			msgPaymentReGemGold2 = msgPaymentReGemGold2.Replace(StringContent.strValue3, saleCost[num].ToString());
			ProcBase.ShowMsg(msgPaymentReGemGold2, MessageView.MsgIcon.question, false, new MessageView.MsgButton[2]
			{
				MessageView.MsgButton.yes,
				MessageView.MsgButton.no
			}, OnBuyMessage);
		}
		else
		{
			StartPayment();
			if (storeLib == null)
			{
				storeLib = StoreLib.LoadStoreLib();
			}
			storeLib.BuyItem(StoreLib.ProductType.gold, num - 2, OnResultPayment);
		}
	}

	private void OnBuyMessage(MessageView.MsgButton button)
	{
		if (button == MessageView.MsgButton.yes)
		{
			StartPayment();
			if (storeLib == null)
			{
				storeLib = StoreLib.LoadStoreLib();
			}
			if (buyCode < 2)
			{
				storeLib.BuyItem(StoreLib.ProductType.gem, buyCode, OnResultPayment);
			}
			else
			{
				storeLib.BuyItem(StoreLib.ProductType.gold, buyCode - 2, OnResultPayment);
			}
		}
	}

	private void OnResultPayment(bool succeed, StoreLib.ProductType type, int idx)
	{
		FinishPayment();
		if (!succeed)
		{
			ProcBase.HideMsgView();
			return;
		}
		int num = -1;
		switch (type)
		{
		case StoreLib.ProductType.gem:
			num = idx;
			break;
		case StoreLib.ProductType.gold:
			num = idx + 2;
			break;
		}
		if (num >= 0)
		{
			if (num < 2)
			{
				PlayInfo.playerData.gem += saleQty[num];
				PlayInfo.Save();
				ProcBase.ShowMsg(StringContent.msgGemShopFinishBuyGem.Replace(StringContent.strValue, saleQty[num].ToString()), MessageView.MsgIcon.alert);
			}
			else
			{
				PlayInfo.playerData.gold += saleQty[num];
				PlayInfo.Save();
				ProcBase.ShowMsg(StringContent.msgGemShopFinishBuyGold.Replace(StringContent.strValue, saleQty[num].ToString()), MessageView.MsgIcon.alert);
			}
		}
	}

	private void StartPayment()
	{
		AuiButton.SetEnableAll(base.transform, false);
	}

	private void FinishPayment()
	{
		AuiButton.SetEnableAll(base.transform, true);
	}

	private void OnCloseClick(AuiButton sender)
	{
		Hide();
	}

	private void OnFreeChargeClick(AuiButton sender)
	{
		GameObject gameObject = GameObject.Find("AlphaAdManager");
		if (!(gameObject == null))
		{
			AlphaAdManager component = gameObject.GetComponent<AlphaAdManager>();
			if (!(component == null))
			{
				ActiveButton(false);
				component.ShowListAd(OnBannerClose);
			}
		}
	}

	private void OnBannerClose()
	{
		ActiveButton(true);
	}

	public void Show()
	{
		base.gameObject.SetActiveRecursive(true);
		AuiButton.mostTopActive = true;
		PlayInfo.gameTime.pause = true;
	}

	public void Hide()
	{
		base.gameObject.SetActiveRecursive(false);
		AuiButton.mostTopActive = false;
		PlayInfo.gameTime.pause = false;
	}
}
