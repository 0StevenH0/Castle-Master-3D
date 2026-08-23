using System.IO;
using UnityEngine;

public class UICmdPtsShop : MonoBehaviour
{
	private const int upgradePoint = 100;

	public AuiButton buttonClose;

	public AuiButton buttonFreeCharge;

	public AuiButton buttonRecharge;

	public AuiButton buttonUpgrade;

	public TextMesh textGemRecharge;

	public TextMesh textGemUpgrade;

	public AuiSprite barCmdGauge;

	public AuiSpriteNumber numCurrent;

	public AuiSpriteNumber numMax;

	public TextMesh labelCurrent;

	public TextMesh labelRecharge;

	public TextMesh labelRechargeDesc;

	public TextMesh labelUpgrade;

	public TextMesh labelUpgradeDesc;

	public UIGemShop uiGemShop;

	private float rechargeCost;

	private float upgradeCost;

	private int upgradeMax;

	private StoreLib storeLib;

	private AppUtcTime appUtcTime;

	private void Start()
	{
		LoadSaleList();
		buttonClose.onButtonClick = OnCloseClick;
		buttonClose.isMostTop = true;
		buttonFreeCharge.onButtonClick = OnFreeChargeClick;
		buttonFreeCharge.isMostTop = true;
		buttonRecharge.onButtonClick = OnRechargeClick;
		buttonRecharge.isMostTop = true;
		buttonUpgrade.onButtonClick = OnUpgradeClick;
		buttonUpgrade.isMostTop = true;
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	private void LoadSaleList()
	{
		TextAsset textAsset = ResourceManager.Load("GameData", "cmdpts_shop", typeof(TextAsset)) as TextAsset;
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
			if (array.Length <= 1)
			{
				continue;
			}
			if (num == 0)
			{
				if (StoreType.store == StoreType.Store.appstore || StoreType.store == StoreType.Store.android)
				{
					rechargeCost = float.Parse(array[2]);
				}
				else
				{
					rechargeCost = float.Parse(array[3]);
				}
			}
			else
			{
				if (StoreType.store == StoreType.Store.appstore || StoreType.store == StoreType.Store.android)
				{
					upgradeCost = float.Parse(array[2]);
				}
				else
				{
					upgradeCost = float.Parse(array[3]);
				}
				upgradeMax = int.Parse(array[1]);
			}
			num++;
		}
		string text2 = StringContent.signDollar;
		if (StoreType.store != 0 && StoreType.store != StoreType.Store.android)
		{
			text2 = StringContent.signWon;
		}
		textGemRecharge.text = rechargeCost.ToString();
		textGemUpgrade.text = text2 + " " + upgradeCost;
	}

	private void OnRechargeClick(AuiButton sender)
	{
		if (PlayInfo.playerData.maxCmdPts == PlayInfo.playerData.cmdPts)
		{
			ProcBase.ShowMsg(StringContent.msgCmdPtsResultAlready100, MessageView.MsgIcon.alert);
		}
		else if ((float)PlayInfo.playerData.gem < rechargeCost)
		{
			ProcBase.ShowMsg(StringContent.msgNotEnoughGem, MessageView.MsgIcon.alert, new MessageView.MsgButton[2]
			{
				MessageView.MsgButton.yes,
				MessageView.MsgButton.no
			}, OnMessageGotoGemShop);
		}
		else
		{
			string msgPaymentReCmdPts = StringContent.msgPaymentReCmdPts;
			msgPaymentReCmdPts = msgPaymentReCmdPts.Replace(StringContent.strValue, rechargeCost.ToString());
			ProcBase.ShowMsg(msgPaymentReCmdPts, MessageView.MsgIcon.question, false, new MessageView.MsgButton[2]
			{
				MessageView.MsgButton.yes,
				MessageView.MsgButton.no
			}, OnRechargeMessage);
		}
	}

	private void OnMessageGotoGemShop(MessageView.MsgButton button)
	{
		if (button == MessageView.MsgButton.yes)
		{
			Hide();
			uiGemShop.Show();
		}
	}

	private void OnResultRecharge(bool succeed, StoreLib.ProductType type, int idx)
	{
		FinishPayment();
		if (succeed)
		{
			PlayInfo.playerData.cmdPts = PlayInfo.playerData.maxCmdPts;
			PlayInfo.Save();
			RefreshGauge();
			ProcBase.ShowMsg(StringContent.msgCmdPtsFinishRecharge, MessageView.MsgIcon.alert);
		}
		else
		{
			ProcBase.HideMsgView();
		}
	}

	private void OnUpgradeClick(AuiButton sender)
	{
		if (PlayInfo.playerData.maxCmdPts >= upgradeMax)
		{
			ProcBase.ShowMsg(StringContent.msgCmdPtsResultCannotExtand, MessageView.MsgIcon.alert);
		}
		else if (StoreType.store == StoreType.Store.tstore || StoreType.store == StoreType.Store.olleh || StoreType.store == StoreType.Store.samsung)
		{
			string msgPaymentReMaxCmdPts = StringContent.msgPaymentReMaxCmdPts;
			msgPaymentReMaxCmdPts = msgPaymentReMaxCmdPts.Replace(StringContent.strValue, 100.ToString());
			msgPaymentReMaxCmdPts = msgPaymentReMaxCmdPts.Replace(StringContent.strValue2, upgradeCost.ToString());
			ProcBase.ShowMsg(msgPaymentReMaxCmdPts, MessageView.MsgIcon.question, false, new MessageView.MsgButton[2]
			{
				MessageView.MsgButton.yes,
				MessageView.MsgButton.no
			}, OnMaxUpgradeMessage);
		}
		else
		{
			StartPayment();
			if (storeLib == null)
			{
				storeLib = StoreLib.LoadStoreLib();
			}
			storeLib.BuyItem(StoreLib.ProductType.cmdpts, 1, OnResultMaxUpgrade);
		}
	}

	private void OnResultMaxUpgrade(bool succeed, StoreLib.ProductType type, int idx)
	{
		FinishPayment();
		if (succeed)
		{
			PlayInfo.playerData.maxCmdPts += 100;
			PlayInfo.Save();
			RefreshGauge();
			ProcBase.ShowMsg(StringContent.msgCmdPtsFinishUpgrade, MessageView.MsgIcon.alert);
		}
		else
		{
			ProcBase.HideMsgView();
		}
	}

	private void OnRechargeMessage(MessageView.MsgButton button)
	{
		if (button == MessageView.MsgButton.yes)
		{
			PlayInfo.playerData.cmdPts = PlayInfo.playerData.maxCmdPts;
			PlayInfo.playerData.gem -= (int)rechargeCost;
			PlayInfo.Save();
			RefreshGauge();
		}
	}

	private void OnMaxUpgradeMessage(MessageView.MsgButton button)
	{
		if (button == MessageView.MsgButton.yes)
		{
			StartPayment();
			if (storeLib == null)
			{
				storeLib = StoreLib.LoadStoreLib();
			}
			storeLib.BuyItem(StoreLib.ProductType.cmdpts, 1, OnResultMaxUpgrade);
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

	private void OnFreeChargeClick(AuiButton sender)
	{
		if (PlayInfo.playerData.cmdPts >= PlayInfo.playerData.maxCmdPts)
		{
			ProcBase.ShowMsg(StringContent.msgCmdPtsResultAlready100, MessageView.MsgIcon.alert);
			return;
		}
		int commandPointInc = PlayInfo.playerData.GetCommandPointInc();
		if (commandPointInc == 0)
		{
			int minutesLeft = PlayInfo.playerData.GetMinutesUntilFreeCharge();
			ProcBase.ShowMsg(StringContent.msgFreeChargeWaitInfo.Replace(StringContent.strValue, minutesLeft.ToString()), MessageView.MsgIcon.alert);
			return;
		}
		ProcBase.ShowMsg(StringContent.msgFreeChargePoint.Replace(StringContent.strValue, commandPointInc.ToString()), MessageView.MsgIcon.question, false, new MessageView.MsgButton[2]
		{
			MessageView.MsgButton.yes,
			MessageView.MsgButton.no
		}, OnFreeChargeMessage);
	}

	private void OnFreeChargeMessage(MessageView.MsgButton button)
	{
		if (button == MessageView.MsgButton.yes)
		{
			StartPayment();
			if (appUtcTime == null)
			{
				appUtcTime = base.gameObject.AddComponent<AppUtcTime>();
			}
			appUtcTime.UpdateUtcTime(OnResultUpdateUtcTime);
		}
	}

	public void OnResultUpdateUtcTime(bool isSucceed)
	{
		FinishPayment();
		if (isSucceed)
		{
			int num = PlayInfo.playerData.CheckCommandPointInc();
			if (num > 0)
			{
				ProcBase.ShowMsg(StringContent.msgFreeChargeSucceed.Replace(StringContent.strValue, num.ToString()), MessageView.MsgIcon.alert);
				RefreshGauge();
			}
			else
			{
				ProcBase.ShowMsg(StringContent.msgFreeChargeFailed, MessageView.MsgIcon.alert);
			}
		}
		else
		{
			ProcBase.ShowMsg(StringContent.msgFreeChargeFailed, MessageView.MsgIcon.alert);
		}
	}

	private void OnCloseClick(AuiButton sender)
	{
		Hide();
	}

	private void RefreshGauge()
	{
		numCurrent.SetValue(PlayInfo.playerData.cmdPts);
		numMax.SetValue(PlayInfo.playerData.maxCmdPts);
		barCmdGauge.isCrop = true;
		barCmdGauge.crop.width = (float)PlayInfo.playerData.cmdPts / (float)PlayInfo.playerData.maxCmdPts;
		barCmdGauge.SetFrame(0);
	}

	public void Show()
	{
		base.gameObject.SetActiveRecursively(true);
		labelCurrent.text = StringContent.wordCurrent;
		labelRecharge.text = StringContent.msgCmdPtsRecharge100;
		labelRechargeDesc.text = StringContent.msgCmdPtsRechargeDesc;
		labelUpgrade.text = StringContent.msgCmdPtsUpgrade100;
		labelUpgradeDesc.text = StringContent.msgCmdPtsUpgradeDesc;
		RefreshGauge();
		AuiButton.mostTopActive = true;
		PlayInfo.gameTime.pause = true;
	}

	public void Hide()
	{
		base.gameObject.SetActiveRecursively(false);
		AuiButton.mostTopActive = false;
		PlayInfo.gameTime.pause = false;
	}
}
