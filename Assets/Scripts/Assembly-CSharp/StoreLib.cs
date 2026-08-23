using System.Collections;
using UnityEngine;

public class StoreLib : PlugInNetServer
{
	public enum ResultType
	{
		none = 0,
		success = 1,
		failed = 2
	}

	public enum ProductType
	{
		cmdpts = 0,
		gem = 1,
		gold = 2,
		max = 3
	}

	public enum AdPos
	{
		lefttop = 0,
		righttop = 1,
		centertop = 2,
		leftbottom = 3,
		rightbottom = 4,
		centerbottom = 5
	}

	public delegate void OnResultDelegate(bool succeed, ProductType type, int idx);

	public const int resultNone = 0;

	public const int resultSuccess = 1;

	public const int resultFailed = 2;

	public const int idxCommandPointRecharge = 0;

	public const int idxCommandPointMaxUpgrade = 1;

	public const int idxGemA = 0;

	public const int idxGemB = 1;

	public const int idxGoldA = 0;

	public const int idxGoldB = 1;

	public const string appId_TStore = "OA00282959";

	public const string appId_Olleh = "8100FC4A";

	public const string appIdentifier = "com.alphacloud.castlemaster";

	public const string samsungGID = "100000025796";

	public const string appId_TStore_Plus = "OA00289500";

	public const string appId_Olleh_Plus = "810105ED";

	public const string appIdentifier_Plus = "com.alphacloud.castlemasterplus";

	public const string samsungGID_Plus = "100000025797";

	private const string MY_AD_UNIT_ID = "a14fbdd32168052";

	private const string MY_AD_UNIT_ID_PLUS = "a14fc4a6583732b";

	public static string[][] productID_TStore = new string[3][]
	{
		new string[2] { "0900511158", "0900528243" },
		new string[2] { "0900511151", "0900511153" },
		new string[2] { "0900511155", "0900511156" }
	};

	public static string[][] productID_Olleh = new string[3][]
	{
		new string[2] { "005", "006" },
		new string[2] { "001", "002" },
		new string[2] { "003", "004" }
	};

	public static string[][] productID_AppStore = new string[3][]
	{
		new string[2] { "cp0001", "cp0002" },
		new string[2] { "gem0001", "gem0002" },
		new string[2] { "gold0001", "gold0002" }
	};

	public static string[][] productID_Google = new string[3][]
	{
		new string[2]
		{
			string.Empty,
			"006"
		},
		new string[2] { "001", "002" },
		new string[2] { "003", "004" }
	};

	public static string[][] productID_Samsung = new string[3][]
	{
		new string[2]
		{
			string.Empty,
			"000000037734"
		},
		new string[2] { "000000037730", "000000037731" },
		new string[2] { "000000037732", "000000037733" }
	};

	public static string[][] productID_TStore_Plus = new string[3][]
	{
		new string[2]
		{
			string.Empty,
			"0900540684"
		},
		new string[2] { "0900540680", "0900540681" },
		new string[2] { "0900540682", "0900540683" }
	};

	public static string[][] productID_Olleh_Plus = new string[3][]
	{
		new string[2]
		{
			string.Empty,
			"005"
		},
		new string[2] { "001", "002" },
		new string[2] { "003", "004" }
	};

	public static string[][] productID_AppStore_Plus = new string[3][]
	{
		new string[2]
		{
			string.Empty,
			"cp002"
		},
		new string[2] { "gem001", "gem002" },
		new string[2] { "gold001", "gold002" }
	};

	public static string[][] productID_Google_Plus = new string[3][]
	{
		new string[2]
		{
			string.Empty,
			"006"
		},
		new string[2] { "001", "002" },
		new string[2] { "003", "004" }
	};

	public static string[][] productID_Samsung_Plus = new string[3][]
	{
		new string[2]
		{
			string.Empty,
			"000000037739"
		},
		new string[2] { "000000037735", "000000037736" },
		new string[2] { "000000037737", "000000037738" }
	};

	private OnResultDelegate onResult;

	private ProductType prodType;

	private int prodIndex;

	public static StoreLib LoadStoreLib()
	{
		GameObject gameObject = GameObject.Find("StoreLib");
		if (gameObject == null)
		{
			gameObject = new GameObject();
		}
		StoreLib storeLib = gameObject.GetComponent<StoreLib>();
		if (storeLib == null)
		{
			storeLib = gameObject.AddComponent<StoreLib>();
		}
		if (!Application.isEditor)
		{
			InApp.curType = InApp.Type.INAPP_ADMOB;
			storeLib.AppCreate();
		}
		return storeLib;
	}

	public void BuyItem(ProductType type, int idx, OnResultDelegate proc)
	{
		onResult = proc;
		onResult(false, type, idx);
		StartCoroutine("ShowDisabledMsg");
	}

	private IEnumerator ShowDisabledMsg()
	{
		yield return null;
		ProcBase.ShowMsg("In-app purchases are disabled in this build.", MessageView.MsgIcon.alert);
	}

	private IEnumerator WaitForStoreInit()
	{
		yield return 1;
		if (PlusType.isPlus)
		{
			if (StoreType.store == StoreType.Store.tstore)
			{
				Request_BuyItem_In_TStore("OA00289500", productID_TStore_Plus[(int)prodType][prodIndex]);
				StartCoroutine("CheckResponse", 1f);
			}
			else if (StoreType.store == StoreType.Store.olleh)
			{
				Request_BuyItem_In_Olleh("810105ED", productID_Olleh_Plus[(int)prodType][prodIndex]);
				StartCoroutine("CheckResponse", 1f);
			}
			else if (StoreType.store == StoreType.Store.android)
			{
				Request_BuyItem_In_Google(productID_Google_Plus[(int)prodType][prodIndex], "1");
				StartCoroutine("CheckResponse", 1f);
			}
			else if (StoreType.store == StoreType.Store.samsung)
			{
				Request_BuyItem_In_Samsung("100000025797", productID_Samsung_Plus[(int)prodType][prodIndex]);
				StartCoroutine("CheckResponse", 1f);
			}
		}
		else if (StoreType.store == StoreType.Store.tstore)
		{
			Request_BuyItem_In_TStore("OA00282959", productID_TStore[(int)prodType][prodIndex]);
			StartCoroutine("CheckResponse", 1f);
		}
		else if (StoreType.store == StoreType.Store.olleh)
		{
			Request_BuyItem_In_Olleh("8100FC4A", productID_Olleh[(int)prodType][prodIndex]);
			StartCoroutine("CheckResponse", 1f);
		}
		else if (StoreType.store == StoreType.Store.android)
		{
			Request_BuyItem_In_Google(productID_Google[(int)prodType][prodIndex], "1");
			StartCoroutine("CheckResponse", 1f);
		}
		else if (StoreType.store == StoreType.Store.samsung)
		{
			Request_BuyItem_In_Samsung("100000025796", productID_Samsung[(int)prodType][prodIndex]);
			StartCoroutine("CheckResponse", 1f);
		}
	}

	private IEnumerator WaitForPurchase(ProductType type, int idx)
	{
		yield break;
	}

	public void InitAd()
	{
		if (!Application.isEditor)
		{
			InApp.CreateAdMob("top", "left", (!PlusType.isPlus) ? "a14fbdd32168052" : "a14fc4a6583732b");
		}
	}

	public void ShowAd(AdPos adPos)
	{
		if (!Application.isEditor)
		{
			string unitId = ((!PlusType.isPlus) ? "a14fbdd32168052" : "a14fc4a6583732b");
			switch (adPos)
			{
			case AdPos.lefttop:
				InApp.RePosAdMob("top", "left", unitId);
				break;
			case AdPos.righttop:
				InApp.RePosAdMob("top", "right", unitId);
				break;
			case AdPos.centertop:
				InApp.RePosAdMob("top", "center", unitId);
				break;
			case AdPos.leftbottom:
				InApp.RePosAdMob("bottom", "left", unitId);
				break;
			case AdPos.rightbottom:
				InApp.RePosAdMob("bottom", "right", unitId);
				break;
			case AdPos.centerbottom:
				InApp.RePosAdMob("bottom", "center", unitId);
				break;
			}
			InApp.ViewAdMob();
		}
	}

	public void HideAd()
	{
		if (!Application.isEditor)
		{
			InApp.HideAdMob();
		}
	}

	public override void Response_Purchased()
	{
		Debug.Log("Purchased : " + responsePurchaseState);
		onResult(true, prodType, prodIndex);
	}

	public override void Response_NotPurchased()
	{
		Debug.Log("Not Purchased : " + responsePurchaseState);
		onResult(false, prodType, prodIndex);
	}

	public override void Response_CanceledRequest()
	{
		Debug.Log("Canceld Request : " + responseRequestCode);
		onResult(false, prodType, prodIndex);
	}
}
