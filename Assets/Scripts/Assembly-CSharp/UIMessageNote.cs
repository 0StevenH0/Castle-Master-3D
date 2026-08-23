using System.Collections;
using UnityEngine;

public class UIMessageNote : MonoBehaviour
{
	public AuiButton buttonClose;

	public AuiButton buttonScroll;

	public MessageItem messageItem;

	private float itemHeight = 100f;

	private int itemPerPage = 5;

	private int topItem;

	private int maxItem;

	private MessageItem[] messages;

	private bool isScroll;

	private float scrollMin = -270f;

	private float scrollMax = 168f;

	private Vector3 scrollStart;

	private Vector3 befPos;

	private float scrollAreaMinX;

	private float scrollButtonSize = 26f;

	private void Start()
	{
		buttonClose.isTop = true;
		buttonScroll.isTop = true;
		buttonClose.onButtonClick = OnCloseClick;
		scrollAreaMinX = buttonScroll.transform.position.x - scrollButtonSize;
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	private void OnCloseClick(AuiButton sender)
	{
		Hide();
	}

	private void Update()
	{
		if (isScroll)
		{
			if (Input.GetMouseButtonUp(0))
			{
				isScroll = false;
			}
			if (isScroll && maxItem > itemPerPage)
			{
				Vector3 mousePosition = Input.mousePosition;
				Vector3 vector = buttonClose.uiCamera.ScreenToWorldPoint(mousePosition);
				float num = scrollStart.y + vector.y - befPos.y;
				if (num < scrollMin)
				{
					num = scrollMin;
				}
				if (num > scrollMax)
				{
					num = scrollMax;
				}
				buttonScroll.transform.position = new Vector3(scrollStart.x, num, scrollStart.z);
				float num2 = scrollMax - scrollMin;
				int num3 = maxItem - itemPerPage;
				int num4 = (int)((scrollMax - num) * (float)num3 / num2);
				if (num4 < 0)
				{
					num4 = 0;
				}
				if (num4 >= num3)
				{
					num4 = num3 - 1;
				}
				topItem = num4;
				RefreshList();
			}
		}
		else if (Input.GetMouseButtonDown(0))
		{
			Vector3 mousePosition2 = Input.mousePosition;
			Vector3 vector2 = buttonClose.uiCamera.ScreenToWorldPoint(mousePosition2);
			if (vector2.x > scrollAreaMinX && vector2.y > scrollMin - 10f && vector2.y < scrollMax + scrollButtonSize)
			{
				isScroll = true;
				scrollStart = buttonScroll.transform.position;
				befPos = vector2;
			}
		}
	}

	private void RefreshList()
	{
		PlayMessage[] messageAll = PlayInfo.messageManager.GetMessageAll();
		maxItem = messageAll.Length;
		if (topItem + itemPerPage > maxItem)
		{
			topItem = maxItem - itemPerPage;
			if (topItem < 0)
			{
				topItem = 0;
			}
		}
		int num = topItem + itemPerPage;
		if (num > maxItem)
		{
			num = maxItem;
		}
		int num2 = 0;
		for (int i = topItem; i < num; i++)
		{
			int num3 = maxItem - (i + 1);
			messages[num2].gameObject.SetActive(true);
			int num4 = messageAll[num3].type;
			if (num4 >= messages[num2].iconMsgType.materials.Length)
			{
				num4 = 0;
			}
			messages[num2].iconMsgType.SetFrame(num4);
			messages[num2].textDateTimeTitle.text = PlayInfo.gameTime.GetDateString(messageAll[num3].day, messageAll[num3].hour);
			messages[num2].textMessage.text = messageAll[num3].content;
			num2++;
		}
		for (int j = num2; j < itemPerPage; j++)
		{
			messages[j].gameObject.SetActive(false);
		}
	}

	private IEnumerator WaitForRefresh()
	{
		PlayMessage bef = PlayInfo.messageManager.GetLastMessage();
		while (true)
		{
			yield return new WaitForSeconds(0.1f);
			PlayMessage last = PlayInfo.messageManager.GetLastMessage();
			if (bef != last)
			{
				RefreshList();
				PlayInfo.messageManager.ResetNewCount();
			}
		}
	}

	public void Show()
	{
		base.gameObject.SetActive(true);
		AuiButton.topActive = true;
		if (messages == null)
		{
			messages = new MessageItem[itemPerPage];
			messages[0] = messageItem;
			for (int i = 1; i < itemPerPage; i++)
			{
				messages[i] = Object.Instantiate(messageItem) as MessageItem;
				messages[i].transform.parent = messageItem.transform.parent;
				Vector3 localPosition = messageItem.transform.localPosition;
				localPosition.y -= (float)i * itemHeight;
				messages[i].transform.localPosition = localPosition;
			}
		}
		topItem = 0;
		Vector3 position = buttonScroll.transform.position;
		buttonScroll.transform.position = new Vector3(position.x, scrollMax, position.z);
		RefreshList();
		PlayInfo.messageManager.ResetNewCount();
		StartCoroutine("WaitForRefresh");
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
		AuiButton.topActive = false;
	}
}
