using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UINpcAction : MonoBehaviour
{
	public enum NpcActionType
	{
		priest = 0,
		secretary = 1
	}

	[System.Serializable]
	private class PriestRow
	{
		public int castleCount;

		public int gemCount;

		public int minLoyalty;

		public int maxLoyalty;
	}

	[System.Serializable]
	private class SecretaryRow
	{
		public int heroLevel;

		public int gemCount;

		public int minLoyalty;

		public int maxLoyalty;
	}

	[System.Serializable]
	private class PriestRowListWrapper
	{
		public PriestRow[] items;
	}

	[System.Serializable]
	private class SecretaryRowListWrapper
	{
		public SecretaryRow[] items;
	}

	public AuiButton buttonSubmit;

	public AuiButton buttonCancel;

	public AuiButton buttonClose;

	public TextMesh textDesc;

	public TextMesh textGem;

	public TextMesh labelLoyalty;

	public TextMesh textLoyalty;

	public GameObject panelBefore;

	public GameObject panelAfter;

	public ProcCastle procCastle;

	public NpcActionType actionType;

	private int currentIdx;

	private static PriestRow[] priestList;

	private static SecretaryRow[] secretaryList;

	private void Start()
	{
		buttonSubmit.onButtonClick = OnSubmitClick;
		buttonCancel.onButtonClick = OnCancelClick;
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	public static void Init()
	{
		TextAsset textAsset = ResourceManager.Load("GameData", "priest_list", typeof(TextAsset)) as TextAsset;
		string json = "{\"items\":" + textAsset.text + "}";
		PriestRowListWrapper priestRowListWrapper = JsonUtility.FromJson<PriestRowListWrapper>(json);
		priestList = priestRowListWrapper.items;
		TextAsset textAsset2 = ResourceManager.Load("GameData", "secretary_list", typeof(TextAsset)) as TextAsset;
		string json2 = "{\"items\":" + textAsset2.text + "}";
		SecretaryRowListWrapper secretaryRowListWrapper = JsonUtility.FromJson<SecretaryRowListWrapper>(json2);
		secretaryList = secretaryRowListWrapper.items;
	}

	public void Hide()
	{
		base.gameObject.SetActiveRecursive(false);
	}

	public void Show(NpcActionType type)
	{
		base.gameObject.SetActiveRecursive(true);
		actionType = type;
		labelLoyalty.text = StringContent.wordLoyalty;
		if (actionType == NpcActionType.priest)
		{
			currentIdx = priestList.Length - 1;
			for (int i = 0; i < priestList.Length; i++)
			{
				if (PlayInfo.castleManager.GetCastleCount(0) <= priestList[i].castleCount)
				{
					currentIdx = i;
					break;
				}
			}
			textGem.text = "-" + priestList[currentIdx].gemCount;
			textDesc.text = StringContent.msgNpcQuestCitizenLoyalty;
			panelAfter.SetActiveRecursive(false);
			panelBefore.SetActiveRecursive(true);
		}
		else
		{
			if (actionType != NpcActionType.secretary)
			{
				return;
			}
			currentIdx = secretaryList.Length - 1;
			for (int j = 0; j < secretaryList.Length; j++)
			{
				if (PlayInfo.heroState.level <= secretaryList[j].heroLevel)
				{
					currentIdx = j;
					break;
				}
			}
			textGem.text = "-" + secretaryList[currentIdx].gemCount;
			textDesc.text = StringContent.msgNpcQuestLordLoyalty;
			panelAfter.SetActiveRecursive(false);
			panelBefore.SetActiveRecursive(true);
		}
	}

	private void OnSubmitClick(AuiButton sender)
	{
		if (actionType == NpcActionType.priest)
		{
			if (PlayInfo.playerData.gem < priestList[currentIdx].gemCount)
			{
				ShowGotoGemShop();
				return;
			}
			PlayInfo.playerData.gem -= priestList[currentIdx].gemCount;
			int num = Random.Range(priestList[currentIdx].minLoyalty, priestList[currentIdx].maxLoyalty);
			CastleInfo[] castle = PlayInfo.castleManager.castle;
			foreach (CastleInfo castleInfo in castle)
			{
				if (castleInfo.side == 0 && castleInfo.loyalty < 100)
				{
					castleInfo.loyalty += num;
					if (castleInfo.loyalty > 100)
					{
						castleInfo.loyalty = 100;
					}
				}
			}
			textLoyalty.text = "+" + num;
			textDesc.text = StringContent.msgNpcRiseCitizenLoyalty;
			panelAfter.SetActiveRecursive(true);
			panelBefore.SetActiveRecursive(false);
			PlayInfo.Save();
		}
		else
		{
			if (actionType != NpcActionType.secretary)
			{
				return;
			}
			if (PlayInfo.playerData.gem < secretaryList[currentIdx].gemCount)
			{
				ShowGotoGemShop();
				return;
			}
			PlayInfo.playerData.gem -= secretaryList[currentIdx].gemCount;
			int num2 = Random.Range(secretaryList[currentIdx].minLoyalty, secretaryList[currentIdx].maxLoyalty);
			foreach (LordManager.Lord item in PlayInfo.lordManager.list)
			{
				if (item.loyalty < 100)
				{
					item.loyalty += num2;
					if (item.loyalty > 100)
					{
						item.loyalty = 100;
					}
				}
			}
			textLoyalty.text = "+" + num2;
			textDesc.text = StringContent.msgNpcRiseLordLoyalty;
			panelAfter.SetActiveRecursive(true);
			panelBefore.SetActiveRecursive(false);
			PlayInfo.Save();
		}
	}

	private void OnCancelClick(AuiButton sender)
	{
		if (buttonClose.onButtonClick != null)
		{
			buttonClose.onButtonClick(sender);
		}
	}

	private void ShowGotoGemShop()
	{
		ProcBase.ShowMsg(StringContent.msgNotEnoughGem, MessageView.MsgIcon.alert, new MessageView.MsgButton[2]
		{
			MessageView.MsgButton.yes,
			MessageView.MsgButton.no
		}, OnMessageGotoGemShop);
	}

	private void OnMessageGotoGemShop(MessageView.MsgButton button)
	{
		if (button == MessageView.MsgButton.yes)
		{
			Hide();
			procCastle.ShowMap();
			procCastle.uiGemShop.Show();
		}
	}
}
