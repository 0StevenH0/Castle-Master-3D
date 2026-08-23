using UnityEngine;

public class InApp
{
	public enum Type
	{
		ALL = 0,
		INAPP_ONLY = 1,
		INAPP_ADMOB = 2,
		ADMOB = 3,
		ADAM = 4,
		TAD = 5
	}

	public static Type curType;

	public static AndroidJavaClass inApp;

	public static AndroidJavaClass adAdMob;

	public static bool createAdMob_b;

	public static AndroidJavaClass adAdam;

	public static bool createAdam_b;

	public static AndroidJavaClass adTAd;

	public static bool createTAd_b;

	public static AndroidJavaClass myAlert;

	public static bool Request(string productId, string payloadContents)
	{
		if (inApp == null)
		{
			return false;
		}
		Debug.Log("Unity:RequestInApp:productId:" + productId + ",payloadContents:" + payloadContents);
		return inApp.CallStatic<bool>("RequestInApp", new object[2] { productId, payloadContents });
	}

	public static bool bindToMarketBillingService()
	{
		if (inApp == null)
		{
			return false;
		}
		return inApp.CallStatic<bool>("bindToMarketBillingService", new object[0]);
	}

	public static string GetState()
	{
		return inApp.CallStatic<string>("GetState", new object[0]);
	}

	public static string GetRequest_ResponseCode()
	{
		return inApp.CallStatic<string>("GetResult_Request_responseCode", new object[0]);
	}

	public static string GetResponse_PurchaseState()
	{
		return inApp.CallStatic<string>("GetResult_Response_purchaseState", new object[0]);
	}

	public static string GetResponse_ItemId()
	{
		return inApp.CallStatic<string>("GetResult_Response_itemId", new object[0]);
	}

	public static string GetResponse_Quantity()
	{
		return inApp.CallStatic<string>("GetResult_Response_quantity", new object[0]);
	}

	public static string GetResponse_PurchaseTime()
	{
		return inApp.CallStatic<string>("GetResult_Response_purchaseTime", new object[0]);
	}

	public static string GetResponse_DeveloperPayload()
	{
		return inApp.CallStatic<string>("GetResult_Response_developerPayload", new object[0]);
	}

	public static void SetID(string appId, string pId)
	{
		Debug.Log("SetID:appId:" + appId + ",pId:" + pId);
		if (inApp != null)
		{
			inApp.CallStatic("SetID", appId, pId);
		}
	}

	public static void CreateAdMob(string topBottom, string leftRight, string unitId)
	{
		if (!createAdMob_b)
		{
			SetAdMob_UnitID(topBottom, leftRight, unitId);
			MakeAdMob();
			createAdMob_b = true;
		}
	}

	public static void RePosAdMob(string topBottom, string leftRight, string unitId)
	{
		if (createAdMob_b)
		{
			SetAdMob_UnitID(topBottom, leftRight, unitId);
			RePosAdMob();
		}
	}

	private static void SetAdMob_UnitID(string topBottom, string leftRight, string unitId)
	{
		Debug.Log("SetID:topBottom:" + topBottom + ",leftRight:" + leftRight + ",unitId:" + unitId);
		adAdMob.CallStatic("SetAdMob_UnitID", topBottom, leftRight, unitId);
	}

	private static void MakeAdMob()
	{
		adAdMob.CallStatic("MakeAdMob");
	}

	private static void RePosAdMob()
	{
		adAdMob.CallStatic("RePosAdMob");
	}

	public static void ViewAdMob()
	{
		if (createAdMob_b)
		{
			adAdMob.CallStatic("ViewAdMob");
		}
	}

	public static void HideAdMob()
	{
		if (createAdMob_b)
		{
			adAdMob.CallStatic("HideAdMob");
		}
	}

	public static void RenewAdMob()
	{
		if (createAdMob_b)
		{
			adAdMob.CallStatic("RenewAdMob");
		}
	}

	public static void ShowFullAdMob(string interstitialId)
	{
		Debug.Log("ShowFullAdMob:interstitialId:" + interstitialId);
		adAdMob.CallStatic("ShowFullAdMob", interstitialId);
	}

	public static void FullAd_SetTestDeviceMode(string testDeviceId)
	{
		Debug.Log("FullAd_SetTestDeviceMode:testDeviceId:" + testDeviceId);
		adAdMob.CallStatic("FullAd_SetTestDeviceMode", testDeviceId);
	}

	public static void FullAd_SetTestDeviceMode()
	{
		FullAd_SetTestDeviceMode(string.Empty);
	}

	public static void CreateAdam(string topBottom, string clientId)
	{
		if (!createAdam_b)
		{
			SetAdam_ClientID(topBottom, clientId);
			MakeAdam();
			createAdam_b = true;
		}
	}

	public static void RePosAdam(string topBottom, string clientId)
	{
		if (createAdam_b)
		{
			SetAdam_ClientID(topBottom, clientId);
			RePosAdam();
		}
	}

	private static void SetAdam_ClientID(string topBottom, string clientId)
	{
		Debug.Log("SetID:topBottom:" + topBottom + ",clientId:" + clientId);
		adAdam.CallStatic("SetAdam_ClientID", topBottom, clientId);
	}

	private static void MakeAdam()
	{
		adAdam.CallStatic("MakeAdam");
	}

	private static void RePosAdam()
	{
		adAdam.CallStatic("RePosAdam");
	}

	public static void ViewAdam()
	{
		if (createAdam_b)
		{
			adAdam.CallStatic("ViewAdam");
		}
	}

	public static void HideAdam()
	{
		if (createAdam_b)
		{
			adAdam.CallStatic("HideAdam");
		}
	}

	public static void SetRefreshTimeAdam(int refreshTime)
	{
		if (createAdam_b)
		{
			adAdam.CallStatic("SetRefreshTimeAdam", refreshTime);
		}
	}

	public static void CreateTAd(string topBottom)
	{
		Debug.Log("CreateTAd:topBottom:" + topBottom);
		if (!createTAd_b)
		{
			SetTAd_ClientID(topBottom);
			MakeTAd();
			createTAd_b = true;
		}
	}

	public static void RePosTAd(string topBottom)
	{
		if (createTAd_b)
		{
			SetTAd_ClientID(topBottom);
			RePosTAd();
		}
	}

	private static void SetTAd_ClientID(string topBottom)
	{
		Debug.Log("SetID:topBottom:" + topBottom);
		adTAd.CallStatic("SetTAd_ClientID", topBottom);
	}

	private static void MakeTAd()
	{
		adTAd.CallStatic("MakeTAd");
	}

	private static void RePosTAd()
	{
		adTAd.CallStatic("RePosTAd");
	}

	public static void ViewTAd()
	{
		if (createTAd_b)
		{
			adTAd.CallStatic("ViewTAd");
		}
	}

	public static void HideTAd()
	{
		if (createTAd_b)
		{
			adTAd.CallStatic("HideTAd");
		}
	}

	public static void ShowFullTAd()
	{
		adTAd.CallStatic("ShowFullTAd");
	}

	public static void Alert_SetTxt(string title, string data, string yes, string no)
	{
		Alert_SetTxt(title, data, yes, no, string.Empty);
	}

	public static void Alert_SetTxt(string title, string data, string yes, string no, string icon)
	{
		myAlert.CallStatic("Alert_SetTxt", title, data, yes, no, icon);
	}

	public static void Alert_ShowDlg()
	{
		myAlert.CallStatic("Alert_ShowDlg");
	}

	public static bool Alert_IsUserClicked()
	{
		return myAlert.CallStatic<bool>("Alert_IsUserClicked", new object[0]);
	}

	public static bool Alert_ResultYesNo()
	{
		return myAlert.CallStatic<bool>("Alert_ResultYesNo", new object[0]);
	}

	public static void onCreate()
	{
		Debug.Log("=> InApp:onCreate()");
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				myAlert = new AndroidJavaClass("com.alphac.unity3d.MyAlert");
				myAlert.CallStatic("onCreate", androidJavaObject);
				switch (curType)
				{
				case Type.INAPP_ONLY:
					Debug.Log("INAPP_ONLY");
					inApp = new AndroidJavaClass("com.alphac.unity3d.AlphaCInApp");
					inApp.CallStatic("onCreate", androidJavaObject);
					break;
				case Type.ADMOB:
					Debug.Log("ADMOB");
					adAdMob = new AndroidJavaClass("com.alphac.unity3d.AdAdMob");
					adAdMob.CallStatic("onCreate", androidJavaObject);
					break;
				case Type.TAD:
					Debug.Log("TAD");
					adTAd = new AndroidJavaClass("com.alphac.unity3d.AdTAd");
					adTAd.CallStatic("onCreate", androidJavaObject);
					break;
				case Type.INAPP_ADMOB:
					Debug.Log("INAPP_ADMOB");
					inApp = new AndroidJavaClass("com.alphac.unity3d.AlphaCInApp");
					adAdMob = new AndroidJavaClass("com.alphac.unity3d.AdAdMob");
					inApp.CallStatic("onCreate", androidJavaObject);
					adAdMob.CallStatic("onCreate", androidJavaObject);
					break;
				case Type.ADAM:
					Debug.Log("ADAM");
					adAdam = new AndroidJavaClass("com.alphac.unity3d.AdAdam");
					adAdam.CallStatic("onCreate", androidJavaObject);
					break;
				default:
					Debug.Log("ALL");
					inApp = new AndroidJavaClass("com.alphac.unity3d.AlphaCInApp");
					adAdMob = new AndroidJavaClass("com.alphac.unity3d.AdAdMob");
					adAdam = new AndroidJavaClass("com.alphac.unity3d.AdAdam");
					adTAd = new AndroidJavaClass("com.alphac.unity3d.AdTAd");
					inApp.CallStatic("onCreate", androidJavaObject);
					adAdMob.CallStatic("onCreate", androidJavaObject);
					adAdam.CallStatic("onCreate", androidJavaObject);
					adTAd.CallStatic("onCreate", androidJavaObject);
					break;
				}
			}
		}
		Debug.Log("<= InApp:onCreate()");
	}

	public static void onStop()
	{
		if (inApp != null)
		{
			inApp.CallStatic("onStop");
		}
	}
}
