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

	private class PriestRow
	{
		public int castleCount;

		public int gemCount;

		public int minLoyalty;

		public int maxLoyalty;
	}

	private class SecretaryRow
	{
		public int heroLevel;

		public int gemCount;

		public int minLoyalty;

		public int maxLoyalty;
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
		List<PriestRow> list = new List<PriestRow>();
		TextAsset textAsset = ResourceManager.Load("GameData", "priest_list", typeof(TextAsset)) as TextAsset;
		bool succeed = false;
		string s = DataSecurity.Decrypt(textAsset.text, "surkwjch", out succeed);
		if (!succeed)
		{
			Debug.LogError("Decrypt Error!!");
			return;
		}
		StringReader stringReader = new StringReader(s);
		string text;
		while ((text = stringReader.ReadLine()) != null)
		{
			if (text.Trim().Length != 0)
			{
				char[] separator = new char[1] { '\t' };
				string[] array = text.Split(separator);
				if (array.Length > 1)
				{
					PriestRow priestRow = new PriestRow();
					priestRow.castleCount = int.Parse(array[0]);
					priestRow.gemCount = int.Parse(array[1]);
					priestRow.minLoyalty = int.Parse(array[2]);
					priestRow.maxLoyalty = int.Parse(array[3]);
					list.Add(priestRow);
				}
			}
		}
		priestList = list.ToArray();
		List<SecretaryRow> list2 = new List<SecretaryRow>();
		TextAsset textAsset2 = ResourceManager.Load("GameData", "secretary_list", typeof(TextAsset)) as TextAsset;
		bool succeed2 = false;
		string s2 = DataSecurity.Decrypt(textAsset2.text, "surkwjch", out succeed2);
		if (!succeed2)
		{
			Debug.LogError("Decrypt Error!!");
			return;
		}
		StringReader stringReader2 = new StringReader(s2);
		string text2;
		while ((text2 = stringReader2.ReadLine()) != null)
		{
			if (text2.Trim().Length != 0)
			{
				char[] separator2 = new char[1] { '\t' };
				string[] array2 = text2.Split(separator2);
				if (array2.Length > 1)
				{
					SecretaryRow secretaryRow = new SecretaryRow();
					secretaryRow.heroLevel = int.Parse(array2[0]);
					secretaryRow.gemCount = int.Parse(array2[1]);
					secretaryRow.minLoyalty = int.Parse(array2[2]);
					secretaryRow.maxLoyalty = int.Parse(array2[3]);
					list2.Add(secretaryRow);
				}
			}
		}
		secretaryList = list2.ToArray();
	}

	public void Hide()
	{
		base.gameObject.SetActiveRecursively(false);
	}

	public void Show(NpcActionType type)
	{
		base.gameObject.SetActiveRecursively(true);
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
			panelAfter.SetActiveRecursively(false);
			panelBefore.SetActiveRecursively(true);
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
			panelAfter.SetActiveRecursively(false);
			panelBefore.SetActiveRecursively(true);
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
			panelAfter.SetActiveRecursively(true);
			panelBefore.SetActiveRecursively(false);
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
			panelAfter.SetActiveRecursively(true);
			panelBefore.SetActiveRecursively(false);
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
