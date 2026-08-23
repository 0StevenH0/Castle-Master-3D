using System.Collections;
using UnityEngine;

public class UIPlayMessage : MonoBehaviour
{
	private const int maXDelay = 20;

	public TextMesh[] textMessage;

	public AuiSprite[] backMessage;

	public Material[] fontColor;

	private int[] timeDelay;

	private void Start()
	{
		if (UserSetting.tabletMode)
		{
			base.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
		}
		timeDelay = new int[textMessage.Length];
		TextMesh[] array = textMessage;
		foreach (TextMesh textMesh in array)
		{
			textMesh.gameObject.SetActive(false);
		}
		AuiSprite[] array2 = backMessage;
		foreach (AuiSprite auiSprite in array2)
		{
			auiSprite.visible = false;
		}
		StartCoroutine("CoroutineRefreshMessage");
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	private IEnumerator CoroutineRefreshMessage()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.5f);
			PlayMessage[] msg = PlayInfo.messageManager.GetMessageNoShown();
			if (msg.Length > 0)
			{
				int newMsg = msg.Length;
				if (newMsg > 0)
				{
					int curMsg = 0;
					int maxMsg = textMessage.Length;
					TextMesh[] array = textMessage;
					foreach (TextMesh tx in array)
					{
						if (tx.gameObject.activeInHierarchy)
						{
							curMsg++;
						}
					}
					if (newMsg > maxMsg)
					{
						newMsg = maxMsg;
					}
					for (int j = curMsg - 1; j >= 0; j--)
					{
						int move = j + newMsg;
						if (move < maxMsg)
						{
							textMessage[move].text = textMessage[j].text;
							textMessage[move].GetComponent<Renderer>().material = textMessage[j].GetComponent<Renderer>().material;
							textMessage[move].gameObject.SetActive(true);
							backMessage[move].visible = true;
							Vector3 pos = backMessage[move].transform.localPosition;
							pos.x = 400f - textMessage[move].GetComponent<Renderer>().bounds.size.x / base.transform.localScale.x;
							if (pos.x < -450f)
							{
								pos.x = -450f;
							}
							backMessage[move].transform.localPosition = pos;
							timeDelay[move] = timeDelay[j];
						}
					}
					for (int k = 0; k < newMsg; k++)
					{
						textMessage[k].text = msg[k].shortMsg;
						textMessage[k].GetComponent<Renderer>().material = fontColor[(int)msg[k].level];
						textMessage[k].gameObject.SetActive(true);
						backMessage[k].visible = true;
						Vector3 pos2 = backMessage[k].transform.localPosition;
						pos2.x = 400f - textMessage[k].GetComponent<Renderer>().bounds.size.x / base.transform.localScale.x;
						if (pos2.x < -450f)
						{
							pos2.x = -450f;
						}
						backMessage[k].transform.localPosition = pos2;
						timeDelay[k] = 0;
						msg[k].isShown = true;
						ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
					}
				}
			}
			for (int i = 0; i < textMessage.Length; i++)
			{
				if (textMessage[i].gameObject.activeInHierarchy)
				{
					timeDelay[i]++;
					if (timeDelay[i] > 20)
					{
						textMessage[i].gameObject.SetActive(false);
						backMessage[i].visible = false;
					}
				}
			}
		}
	}
}
