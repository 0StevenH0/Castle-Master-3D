using System.IO;
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

	private void LoadSaleList()
	{
		TextAsset textAsset = ResourceManager.Load("GameData", "gem_shop", typeof(TextAsset)) as TextAsset;
		bool succeed = false;
		string s = DataSecurity.Decrypt(textAsset.text, "surkwjch", out succeed);
		if (!succeed)
		{
			Debug.LogError("Decrypt Error!!");
			return;
		}
		StringReader stringReader = new StringReader(s);
		int num = 0;
		string text;
		while ((text = stringReader.ReadLine()) != null)
		{
			if (text.Trim().Length == 0)
			{
				continue;
			}
			char[] separator = new char[1] { '\t' };
			string[] array = text.Split(separator);
			if (array.Length > 1)
			{
				saleQty[num] = int.Parse(array[1]);
				saleMax[num] = int.Parse(array[2]);
				if (StoreType.store == StoreType.Store.appstore || StoreType.store == StoreType.Store.android)
				{
					saleCost[num] = float.Parse(array[3]);
				}
				else
				{
					saleCost[num] = float.Parse(array[4]);
				}
				num++;
				if (num >= 4)
				{
					break;
				}
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
		base.gameObject.SetActive(true);
		AuiButton.mostTopActive = true;
		PlayInfo.gameTime.pause = true;
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
		AuiButton.mostTopActive = false;
		PlayInfo.gameTime.pause = false;
	}
}
