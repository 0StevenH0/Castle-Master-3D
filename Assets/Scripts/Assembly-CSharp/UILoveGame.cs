using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UILoveGame : MonoBehaviour
{
	public enum QuestType
	{
		start = 0,
		waiting = 1,
		max = 2
	}

	public enum QuestAnswer
	{
		answerA = 0,
		answerB = 1,
		max = 2
	}

	public enum QuestViewMode
	{
		first = 0,
		quest = 1,
		result = 2,
		finish = 3,
		max = 4
	}

	public class LoveQuest
	{
		public string quest = string.Empty;

		public string[] answer = new string[2];
	}

	public class HeartPoint
	{
		public int requireGem;

		public int rewardGold;

		public int pointMax;

		public int pointInc;

		public int pointDec;
	}

	private const string pathFab = "Misc/prefeb";

	private const string pathMtr = "Misc/Materials";

	private const string questFab = "feb_questicon";

	public const int maxHeart = 9;

	public const int rewardWeapon = 128;

	private static string[] questMtr = new string[2] { "mtr_heart_mark_gray", "mtr_heart_mark_pink" };

	public AuiButton[] buttonAnswer;

	public AuiButton buttonClose;

	public TextMesh textDesc;

	public TextMesh textAnswerA;

	public TextMesh textAnswerB;

	public TextMesh textLoveInc;

	public TextMesh textLoveDec;

	public TextMesh textRewardGold;

	public TextMesh textRequiredGem;

	public AuiSpriteAnimation aniHeart;

	public AuiSpriteAnimation[] aniRewardWeapon;

	public AuiSprite[] listHeart;

	public GameObject panelClose;

	public GameObject panelReward;

	public GameObject panelRequired;

	public ProcCastle procCastle;

	private GameObject npcQuestIcon;

	private Material[] npcQuestMtr;

	private bool isAllowQuest = true;

	private QuestViewMode viewMode;

	private static LoveQuest[] questList;

	private static HeartPoint[] heartPoint;

	private void Start()
	{
		for (int i = 0; i < buttonAnswer.Length; i++)
		{
			buttonAnswer[i].onButtonClick = OnAnswerClick;
			buttonAnswer[i].buttonTag = i;
		}
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	public void InitNPC(GameObject questUnit)
	{
		npcQuestIcon = Object.Instantiate(ResourceManager.Load("Misc/prefeb", "feb_questicon", typeof(GameObject)) as GameObject) as GameObject;
		npcQuestMtr = new Material[2];
		for (int i = 0; i < 2; i++)
		{
			npcQuestMtr[i] = ResourceManager.Load("Misc/Materials", questMtr[i], typeof(Material)) as Material;
		}
		float num = questUnit.GetComponent<Collider>().bounds.size.y + 1f;
		Vector3 position = questUnit.transform.position;
		position.y += num;
		npcQuestIcon.transform.position = position;
		npcQuestIcon.transform.parent = questUnit.transform;
		npcQuestIcon.GetComponent<Renderer>().material = npcQuestMtr[1];
	}

	public void RandomAllowQuest()
	{
		if (viewMode == QuestViewMode.quest || viewMode == QuestViewMode.result)
		{
			bool flag = Random.Range(0, 2) == 0;
			if (flag != isAllowQuest && npcQuestIcon != null)
			{
				npcQuestIcon.GetComponent<Renderer>().material = npcQuestMtr[flag ? 1 : 0];
			}
			isAllowQuest = flag;
		}
	}

	public static void Init()
	{
		List<LoveQuest> list = new List<LoveQuest>();
		TextAsset textAsset = ResourceManager.Load("GameData", "lovegame_quest_" + UserSetting.language, typeof(TextAsset)) as TextAsset;
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
			if (text.Trim().Length != 1)
			{
				char[] separator = new char[1] { '\t' };
				string[] array = text.Split(separator);
				if (array.Length > 1)
				{
					LoveQuest loveQuest = new LoveQuest();
					loveQuest.quest = array[0];
					loveQuest.answer[0] = array[1];
					loveQuest.answer[1] = array[2];
					list.Add(loveQuest);
				}
			}
		}
		questList = list.ToArray();
		List<HeartPoint> list2 = new List<HeartPoint>();
		TextAsset textAsset2 = ResourceManager.Load("GameData", "lovegame_point", typeof(TextAsset)) as TextAsset;
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
					HeartPoint heartPoint = new HeartPoint();
					heartPoint.requireGem = int.Parse(array2[1]);
					heartPoint.rewardGold = int.Parse(array2[2]);
					heartPoint.pointMax = int.Parse(array2[3]);
					heartPoint.pointInc = int.Parse(array2[4]);
					heartPoint.pointDec = int.Parse(array2[5]);
					list2.Add(heartPoint);
				}
			}
		}
		UILoveGame.heartPoint = list2.ToArray();
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	public void Show()
	{
		base.gameObject.SetActive(true);
		viewMode = QuestViewMode.quest;
		textLoveInc.text = string.Empty;
		textLoveDec.text = string.Empty;
		aniHeart.visible = false;
		panelClose.SetActive(false);
		panelReward.SetActive(false);
		panelRequired.SetActive(false);
		for (int i = 0; i < aniRewardWeapon.Length; i++)
		{
			aniRewardWeapon[i].visible = false;
		}
		Refresh();
	}

	private void Refresh()
	{
		if (!PlayInfo.playerData.startLoveGame)
		{
			viewMode = QuestViewMode.first;
		}
		if (PlayInfo.playerData.countHeart >= 9)
		{
			viewMode = QuestViewMode.finish;
		}
		RefreshGauge();
		panelClose.SetActive(false);
		panelReward.SetActive(false);
		switch (viewMode)
		{
		case QuestViewMode.quest:
			if (isAllowQuest)
			{
				if (PlayInfo.playerData.countHeart == 8)
				{
					textDesc.text = StringContent.msgLoveGameGiveGem + "\n\n";
					textAnswerA.text = StringContent.wordYes;
					textAnswerB.text = StringContent.wordNo;
					buttonAnswer[0].visible = true;
					buttonAnswer[1].visible = true;
					textRequiredGem.text = "-" + heartPoint[PlayInfo.playerData.countHeart].requireGem;
					panelRequired.SetActive(true);
				}
				else
				{
					int num = Random.Range(0, questList.Length);
					textDesc.text = questList[num].quest;
					textAnswerA.text = questList[num].answer[0];
					textAnswerB.text = questList[num].answer[1];
					buttonAnswer[0].visible = true;
					buttonAnswer[1].visible = true;
				}
			}
			else
			{
				textDesc.text = StringContent.msgLoveGameNoQuest;
				textAnswerA.text = string.Empty;
				textAnswerB.text = string.Empty;
				buttonAnswer[0].visible = false;
				buttonAnswer[1].visible = false;
				panelClose.SetActive(true);
			}
			break;
		case QuestViewMode.first:
			textDesc.text = StringContent.msgLoveGameFirstClick;
			textAnswerA.text = StringContent.wordPropose;
			textAnswerB.text = StringContent.wordLater;
			buttonAnswer[0].visible = true;
			buttonAnswer[1].visible = true;
			break;
		case QuestViewMode.finish:
			textDesc.text = StringContent.msgLoveGameNoMore;
			textAnswerA.text = string.Empty;
			textAnswerB.text = string.Empty;
			buttonAnswer[0].visible = false;
			buttonAnswer[1].visible = false;
			panelClose.SetActive(true);
			break;
		case QuestViewMode.result:
			break;
		}
	}

	private void RefreshGauge()
	{
		int countHeart = PlayInfo.playerData.countHeart;
		float num = 1f;
		if (countHeart < listHeart.Length)
		{
			num = PlayInfo.playerData.loveProgress / (float)heartPoint[countHeart].pointMax;
		}
		if (num < 0f)
		{
			num = 0f;
		}
		if (num > 1f)
		{
			num = 1f;
		}
		for (int i = 0; i < listHeart.Length; i++)
		{
			listHeart[i].visible = i < PlayInfo.playerData.countHeart + 1;
			listHeart[i].isCrop = true;
			listHeart[i].crop.height = 1f;
			listHeart[i].SetFrame(0);
		}
		if (countHeart < listHeart.Length)
		{
			listHeart[countHeart].isCrop = true;
			listHeart[countHeart].crop.height = num;
			listHeart[countHeart].SetFrame(0);
		}
	}

	private void OnAnswerClick(AuiButton sender)
	{
		int buttonTag = sender.buttonTag;
		if (buttonTag == 1 && viewMode == QuestViewMode.first)
		{
			if (buttonClose.onButtonClick != null)
			{
				buttonClose.onButtonClick(sender);
			}
		}
		else if (PlayInfo.playerData.startLoveGame)
		{
			int num = PlayInfo.playerData.countHeart;
			if (num >= 9)
			{
				num = 8;
			}
			bool flag = buttonTag == Random.Range(0, 2);
			bool flag2 = false;
			if (PlayInfo.playerData.countHeart == 8)
			{
				flag = buttonTag == 0;
				flag2 = true;
				if (flag)
				{
					if (PlayInfo.playerData.gem < heartPoint[num].requireGem)
					{
						ShowGotoGemShop();
						return;
					}
					if (PlayInfo.inventory.GetItemList(ItemManager.ItemType.weapon).Length >= 24)
					{
						ProcBase.ShowMsg(StringContent.msgNotEnoughInventory, MessageView.MsgIcon.alert);
						return;
					}
					if (PlayInfo.inventory.FindItem(128) != null)
					{
						return;
					}
					PlayInfo.playerData.gem -= heartPoint[num].requireGem;
					if (PlayInfo.playerData.gem < 0)
					{
						PlayInfo.playerData.gem = 0;
					}
				}
				panelRequired.SetActive(false);
			}
			if (flag)
			{
				int pointInc = heartPoint[num].pointInc;
				textDesc.text = StringContent.msgLoveGameGoodAnswer;
				PlayInfo.playerData.loveProgress += pointInc;
				if (PlayInfo.playerData.loveProgress >= (float)heartPoint[num].pointMax)
				{
					PlayInfo.playerData.loveProgress = 0f;
					PlayInfo.playerData.countHeart++;
					if (PlayInfo.playerData.countHeart < 9)
					{
						textRewardGold.text = "+" + heartPoint[num].rewardGold;
						panelReward.SetActive(true);
						textDesc.text = StringContent.msgLoveGameGetHeart;
						PlayInfo.playerData.gold += heartPoint[num].rewardGold;
						PlayInfo.Save();
					}
					else
					{
						textDesc.text = StringContent.msgLoveGameSuccess;
						PlayInfo.inventory.AddItem(ItemManager.ItemType.weapon, 128);
						PlayInfo.Save();
						StartCoroutine("AnimateRewardWeapon");
					}
				}
				RefreshGauge();
				textLoveInc.text = StringContent.wordLove + " +" + pointInc;
				textLoveDec.text = string.Empty;
				Vector3 position = textLoveInc.transform.position;
				position.x = listHeart[num].transform.position.x;
				textLoveInc.transform.position = position;
				textLoveDec.transform.position = position;
				position = aniHeart.transform.position;
				position.x = listHeart[num].transform.position.x;
				aniHeart.transform.position = position;
				aniHeart.visible = true;
				aniHeart.StartAnimation(0, false, true);
				StartCoroutine("AnimateLoveRate");
				PlayInfo.soundManager.Play(ExtSoundManager.Effect2D.efs_princess_heart, new Vector3(0f, 0f, 0f));
			}
			else
			{
				int pointDec = heartPoint[num].pointDec;
				PlayInfo.playerData.loveProgress += pointDec;
				if (PlayInfo.playerData.loveProgress < 0f)
				{
					PlayInfo.playerData.loveProgress = 0f;
				}
				textDesc.text = StringContent.msgLoveGameBadAnswer;
				RefreshGauge();
				if (!flag2)
				{
					textLoveInc.text = string.Empty;
					textLoveDec.text = StringContent.wordLove + " " + pointDec;
					Vector3 position2 = textLoveInc.transform.position;
					position2.x = listHeart[num].transform.position.x;
					textLoveInc.transform.position = position2;
					textLoveDec.transform.position = position2;
					StartCoroutine("AnimateLoveRate");
				}
				PlayInfo.soundManager.Play(ExtSoundManager.Effect2D.efs_princess_x, new Vector3(0f, 0f, 0f));
			}
			textAnswerA.text = string.Empty;
			textAnswerB.text = string.Empty;
			buttonAnswer[0].visible = false;
			buttonAnswer[1].visible = false;
			panelClose.SetActive(true);
			isAllowQuest = false;
			npcQuestIcon.GetComponent<Renderer>().material = npcQuestMtr[isAllowQuest ? 1 : 0];
		}
		else
		{
			PlayInfo.playerData.startLoveGame = true;
			viewMode = QuestViewMode.quest;
			isAllowQuest = true;
			Refresh();
		}
	}

	private IEnumerator AnimateLoveRate()
	{
		float scale = 0.1f;
		while (scale < 1f)
		{
			scale += Time.deltaTime * 3f;
			if (scale > 1f)
			{
				scale = 1f;
			}
			textLoveInc.transform.localScale = new Vector3(scale, scale, 1f);
			textLoveDec.transform.localScale = new Vector3(scale, scale, 1f);
			yield return 1;
		}
		yield return new WaitForSeconds(1f);
		textLoveInc.text = string.Empty;
		textLoveDec.text = string.Empty;
	}

	private IEnumerator AnimateRewardWeapon()
	{
		for (int i = 0; i < aniRewardWeapon.Length; i++)
		{
			aniRewardWeapon[i].visible = true;
			aniRewardWeapon[i].StartAnimation(0, false, true);
			yield return new WaitForSeconds(0.2f);
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
