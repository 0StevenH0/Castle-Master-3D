using System.Collections;
using UnityEngine;

public class PlugInNetServer : MonoBehaviour
{
	public enum ProcessState
	{
		REQUESTING = 0,
		REQUEST_RESULT_DONE = 1,
		RESPONSE_RESULT_DONE = 2
	}

	public enum ResponseCode_Request
	{
		RESULT_OK = 0,
		RESULT_USER_CANCELED = 1,
		RESULT_ERROR = 2,
		RESULT_SERVICE_UNAVAILABLE = 3,
		RESULT_BILLING_UNAVAILABLE = 4,
		RESULT_ITEM_UNAVAILABLE = 5,
		RESULT_DEVELOPER_ERROR = 6,
		HND_ERR_INIT = 7,
		HND_ERR_AUTH = 8,
		HND_ERR_ITEMQUERY = 9,
		HND_ERR_ITEMINFO = 10,
		HND_ERR_ITEMPURCHASE = 11,
		HND_ERR_DATA = 12,
		RESULT_UNKNOWN_ERR = 13,
		RESULT_JUMIN_DLG_CANCEL = 14,
		RESULT_AP_INFO_DLG_CANCEL = 15,
		RESULT_JOIN_DLG_CANCEL = 16,
		RESULT_AP_DISMISS_DLG_CANCEL = 17,
		A000 = 18,
		A001 = 19,
		A002 = 20,
		A003 = 21,
		A004 = 22,
		A005 = 23,
		A006 = 24,
		A007 = 25,
		A008 = 26,
		A010 = 27,
		A011 = 28,
		A012 = 29,
		A013 = 30,
		A014 = 31,
		A015 = 32,
		B008 = 33,
		C001 = 34,
		C002 = 35,
		D001 = 36,
		D002 = 37,
		E001 = 38,
		E002 = 39,
		I001 = 40
	}

	public enum PurchaseState
	{
		PURCHASED = 0,
		CANCELED = 1,
		REFUNDED = 2
	}

	private bool isAleadyCreate;

	private bool isAleadyStop;

	public string responsePurchaseState = string.Empty;

	public string responseRequestCode = string.Empty;

	public bool Request_BuyItem_In_Google(string productId, string developerPayload)
	{
		return InApp.Request(productId, developerPayload);
	}

	public bool Request_BuyItem_In_TStore(string appId, string pId)
	{
		MonoBehaviour.print("Request_BuyItem_In_TStore:appId:" + appId + ",pId:" + pId);
		InApp.SetID(appId, pId);
		return InApp.Request(pId, string.Empty);
	}

	public bool Request_BuyItem_In_Olleh(string appId, string itemId)
	{
		MonoBehaviour.print("Request_BuyItem_In_TStore:appId:" + appId + ",pId:" + itemId);
		InApp.SetID(appId, itemId);
		return InApp.Request(itemId, string.Empty);
	}

	public bool Request_BuyItem_In_Samsung(string groupId, string itemId)
	{
		MonoBehaviour.print("Request_BuyItem_In_Samsung:groupId:" + groupId + ",pId:" + itemId);
		InApp.SetID(groupId, itemId);
		return InApp.Request(itemId, string.Empty);
	}

	public virtual void Response_Purchased()
	{
	}

	public virtual void Response_NotPurchased()
	{
	}

	public virtual void Response_CanceledRequest()
	{
	}

	private IEnumerator CheckResponse(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		string r = InApp.GetState();
		MonoBehaviour.print("CheckResponse:r:[" + r + "]");
		StopCoroutine("CheckResponse");
		MonoBehaviour.print("RESPONSE_RESULT_DONE.To():r:[" + ProcessState.RESPONSE_RESULT_DONE.ToString() + "]");
		if (r == ProcessState.RESPONSE_RESULT_DONE.ToString())
		{
			MonoBehaviour.print("result ... ");
			responsePurchaseState = InApp.GetResponse_PurchaseState();
			if (responsePurchaseState.ToString() == PurchaseState.PURCHASED.ToString())
			{
				Response_Purchased();
			}
			else
			{
				Response_NotPurchased();
			}
		}
		else if (r == ProcessState.REQUEST_RESULT_DONE.ToString())
		{
			responseRequestCode = InApp.GetRequest_ResponseCode();
			MonoBehaviour.print("responseRequestCode ...:" + responseRequestCode.ToString());
			if (responseRequestCode.ToString() != ResponseCode_Request.RESULT_OK.ToString())
			{
				MonoBehaviour.print("request bill service not ok");
				Response_CanceledRequest();
				MonoBehaviour.print("done ... ");
			}
			else
			{
				MonoBehaviour.print("flag 1 ");
				StartCoroutine("CheckResponse", waitTime);
			}
		}
		else
		{
			MonoBehaviour.print("retry ... ");
			StartCoroutine("CheckResponse", waitTime);
		}
	}

	public virtual void Response_ExitGuide_Yes()
	{
	}

	public virtual void Response_ExitGuide_No()
	{
	}

	private IEnumerator CheckExitGuide(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		StopCoroutine("CheckExitGuide");
		if (InApp.Alert_IsUserClicked())
		{
			if (InApp.Alert_ResultYesNo())
			{
				Response_ExitGuide_Yes();
			}
			else
			{
				Response_ExitGuide_No();
			}
		}
		else
		{
			StartCoroutine("CheckExitGuide", waitTime);
		}
	}

	public void AppCreate()
	{
		MonoBehaviour.print("=> PlugInNetServer:AppCreate()");
		if (!isAleadyCreate)
		{
			InApp.onCreate();
			InApp.bindToMarketBillingService();
			Application.runInBackground = true;
			isAleadyCreate = true;
			MonoBehaviour.print("<= PlugInNetServer:AppCreate()");
		}
	}

	public void AppStop()
	{
		MonoBehaviour.print("PlugInNetServer:AppStop()");
		if (!isAleadyStop)
		{
			InApp.onStop();
			isAleadyStop = true;
		}
	}
}
