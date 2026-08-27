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

	public static bool Request(string productId, string payloadContents)
	{
		return false;
	}

	public static bool bindToMarketBillingService()
	{
		return false;
	}

	public static string GetState()
	{
		return string.Empty;
	}

	public static string GetRequest_ResponseCode()
	{
		return string.Empty;
	}

	public static string GetResponse_PurchaseState()
	{
		return string.Empty;
	}

	public static string GetResponse_ItemId()
	{
		return string.Empty;
	}

	public static string GetResponse_Quantity()
	{
		return string.Empty;
	}

	public static string GetResponse_PurchaseTime()
	{
		return string.Empty;
	}

	public static string GetResponse_DeveloperPayload()
	{
		return string.Empty;
	}

	public static void SetID(string appId, string pId)
	{
	}

	public static void CreateAdMob(string topBottom, string leftRight, string unitId)
	{
	}

	public static void RePosAdMob(string topBottom, string leftRight, string unitId)
	{
	}

	public static void ViewAdMob()
	{
	}

	public static void HideAdMob()
	{
	}

	public static void RenewAdMob()
	{
	}

	public static void ShowFullAdMob(string interstitialId)
	{
	}

	public static void FullAd_SetTestDeviceMode(string testDeviceId)
	{
	}

	public static void FullAd_SetTestDeviceMode()
	{
	}

	public static void CreateAdam(string topBottom, string clientId)
	{
	}

	public static void RePosAdam(string topBottom, string clientId)
	{
	}

	public static void ViewAdam()
	{
	}

	public static void HideAdam()
	{
	}

	public static void SetRefreshTimeAdam(int refreshTime)
	{
	}

	public static void CreateTAd(string topBottom)
	{
	}

	public static void RePosTAd(string topBottom)
	{
	}

	public static void ViewTAd()
	{
	}

	public static void HideTAd()
	{
	}

	public static void ShowFullTAd()
	{
	}

	public static void Alert_SetTxt(string title, string data, string yes, string no)
	{
	}

	public static void Alert_SetTxt(string title, string data, string yes, string no, string icon)
	{
	}

	public static void Alert_ShowDlg()
	{
	}

	public static bool Alert_IsUserClicked()
	{
		return false;
	}

	public static bool Alert_ResultYesNo()
	{
		return false;
	}

	public static void onCreate()
	{
	}

	public static void onStop()
	{
	}
}
