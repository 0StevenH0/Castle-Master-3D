using System.Collections.Generic;
using UnityEngine;

public class UILord : MonoBehaviour
{
	private const int rowPerList = 5;

	private float rowHeight = 100f;

	public AuiButton buttonClose;

	public AuiSprite iconScroll;

	public LordRow objectLord;

	public PopLordReward popupReward;

	public ProcCastle procCastle;

	private int topRow;

	private int maxRow;

	private LordRow[] lords;

	private List<LordManager.Lord> retainerList;

	private bool isScroll;

	private float scrollMin = -269f;

	private float scrollMax = 169f;

	private Vector3 scrollStart;

	private Vector3 befPos;

	private void Start()
	{
		buttonClose.isTop = true;
		buttonClose.onButtonClick = OnCloseClick;
		ResetLordList();
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			isScroll = true;
		}
		if (Input.GetMouseButtonUp(0))
		{
			isScroll = false;
		}
		if (!isScroll)
		{
			return;
		}
		Vector3 mousePosition = Input.mousePosition;
		Vector3 vector = buttonClose.uiCamera.ScreenToWorldPoint(mousePosition);
		if (vector.x >= 420f && vector.y >= scrollMin && vector.y <= scrollMax)
		{
			float num = scrollMax - scrollMin;
			int num2 = maxRow + 1 - 5;
			if (num2 < 1)
			{
				num2 = 1;
			}
			int num3 = (int)((scrollMax - vector.y) / num * (float)num2);
			if (num3 > num2)
			{
				num3 = num2;
			}
			if (num3 < 0)
			{
				num3 = 0;
			}
			Vector3 position = iconScroll.transform.position;
			position.y = vector.y;
			iconScroll.transform.position = position;
			if (num3 != topRow)
			{
				topRow = num3;
				Refresh();
			}
		}
	}

	public void SetButtonEnableAll(bool enable)
	{
		AuiButton.SetEnableAll(base.transform, enable);
	}

	private void ResetLordList()
	{
		if (lords != null)
		{
			return;
		}
		Vector3 localPosition = objectLord.transform.localPosition;
		lords = new LordRow[5];
		for (int i = 0; i < 5; i++)
		{
			if (i == 0)
			{
				lords[i] = objectLord;
			}
			else
			{
				lords[i] = Object.Instantiate(objectLord) as LordRow;
				lords[i].transform.parent = objectLord.transform.parent;
			}
			lords[i].transform.localPosition = localPosition;
			localPosition.y -= rowHeight;
		}
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
		AuiButton.topActive = false;
	}

	public void Show()
	{
		base.gameObject.SetActive(true);
		AuiButton.topActive = true;
		popupReward.Hide();
		retainerList = PlayInfo.lordManager.list;
		ResetLordList();
		Refresh();
	}

	public void Refresh()
	{
		maxRow = retainerList.Count;
		for (int i = 0; i < 5; i++)
		{
			int num = topRow + i;
			lords[i].visible = num < maxRow;
			if (num < maxRow)
			{
				lords[i].SetLord(retainerList[num]);
				lords[i].buttonReward.buttonTag = num;
				lords[i].buttonReward.onButtonClick = OnRewardClick;
			}
		}
	}

	private void OnCloseClick(AuiButton sender)
	{
		Hide();
	}

	private void OnRewardClick(AuiButton sender)
	{
		int buttonTag = sender.buttonTag;
		if (retainerList[buttonTag].loyalty >= 100)
		{
			ProcBase.ShowMsg(StringContent.msgMaxLimitLoyalty, MessageView.MsgIcon.alert);
			return;
		}
		SetButtonEnableAll(false);
		popupReward.Show(retainerList[buttonTag]);
	}
}
