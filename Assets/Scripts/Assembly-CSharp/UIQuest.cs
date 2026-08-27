using System.Collections;
using UnityEngine;

public class UIQuest : MonoBehaviour
{
	public enum QuestType
	{
		start = 0,
		waiting = 1,
		finish = 2,
		max = 3
	}

	private const string pathFab = "Misc/prefeb";

	private const string pathMtr = "Misc/Materials";

	private const string questFab = "feb_questicon";

	private static string[] questMtr = new string[3] { "mtr_quest_mark_orange", "mtr_quest_mark_gray", "mtr_quest_mark_blue" };

	public AuiSprite iconQuest;

	public AuiButton buttonAccept;

	public AuiSprite buttonAcceptLabel;

	public AuiButton buttonClose;

	public AuiButton buttonProc;

	public AuiSprite buttonProcLabel;

	public Material mtrPlaying;

	public Material mtrSucceed;

	public TextMesh textResult;

	public TextMesh textMsg;

	public TextMesh labelRewards;

	public TextMesh labelTimeLimit;

	public AuiSprite rewardType;

	public TextMesh textRewards;

	public TextMesh textRewardXp;

	public TextMesh textTimeLimit;

	public GameObject panelReward;

	public GameObject panelButton;

	public UnitControl heroUnit;

	public AuiSprite rewardTypeGem;

	public TextMesh textRewardsGem;

	private GameObject npcQuestIcon;

	private Material[] npcQuestMtr;

	private void Start()
	{
		buttonAccept.onButtonClick = OnAcceptClick;
		buttonProc.onButtonClick = OnProcClick;
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	public void Init(GameObject questUnit)
	{
		npcQuestIcon = Object.Instantiate(ResourceManager.Load("Misc/prefeb", "feb_questicon", typeof(GameObject)) as GameObject) as GameObject;
		npcQuestMtr = new Material[3];
		for (int i = 0; i < 3; i++)
		{
			npcQuestMtr[i] = ResourceManager.Load("Misc/Materials", questMtr[i], typeof(Material)) as Material;
		}
		float num = questUnit.GetComponent<Collider>().bounds.size.y + 1.1f;
		Vector3 position = questUnit.transform.position;
		position.y += num;
		npcQuestIcon.transform.position = position;
		npcQuestIcon.GetComponent<Renderer>().material = npcQuestMtr[0];
	}

	public void RefreshQuestIcon()
	{
		QuestManager.Quest curQuest = PlayInfo.questManager.curQuest;
		npcQuestIcon.gameObject.SetActive(curQuest != null);
		if (curQuest != null)
		{
			if (PlayInfo.questManager.result == 1)
			{
				npcQuestIcon.GetComponent<Renderer>().material = npcQuestMtr[0];
			}
			else if (PlayInfo.questManager.result == 2)
			{
				npcQuestIcon.GetComponent<Renderer>().material = npcQuestMtr[1];
			}
			else if (PlayInfo.questManager.result == 3)
			{
				npcQuestIcon.GetComponent<Renderer>().material = npcQuestMtr[2];
			}
		}
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	public void Show()
	{
		base.gameObject.SetActive(true);
		PlayInfo.questManager.ProcessDaily(false);
		Refresh();
	}

	private void Refresh()
	{
		labelRewards.text = StringContent.wordRewards;
		labelTimeLimit.text = StringContent.wordTimeLimit;
		textResult.text = string.Empty;
		iconQuest.SetFrame(0);
		QuestManager.Quest curQuest = PlayInfo.questManager.curQuest;
		if (curQuest == null)
		{
			panelReward.SetActive(false);
			textMsg.text = StringContent.msgQuestSysNone;
			panelButton.SetActive(false);
			return;
		}
		int result = PlayInfo.questManager.result;
		panelReward.SetActive(true);
		panelButton.SetActive(true);
		switch (result)
		{
		case 2:
			iconQuest.SetFrame(1);
			textResult.text = StringContent.msgQuestSysExcute;
			buttonProcLabel.SetFrame(1);
			buttonAccept.visible = false;
			buttonAcceptLabel.visible = false;
			break;
		case 3:
			iconQuest.SetFrame(2);
			textResult.text = StringContent.msgQuestSysSucceed;
			buttonProcLabel.SetFrame(2);
			buttonAccept.visible = false;
			buttonAcceptLabel.visible = false;
			break;
		default:
			iconQuest.SetFrame(0);
			buttonProcLabel.SetFrame(0);
			buttonAccept.visible = true;
			buttonAcceptLabel.visible = true;
			break;
		}
		switch (curQuest.type)
		{
		case QuestManager.QuestType.appoint_lord:
		{
			string newValue6 = string.Empty;
			if (curQuest.conA >= 0 && curQuest.conA < PlayInfo.castleManager.castle.Length)
			{
				newValue6 = PlayInfo.castleManager.castle[curQuest.conA].castleName;
			}
			textMsg.text = StringContent.msgQuestSysAppointLord.Replace(StringContent.strValue, newValue6);
			break;
		}
		case QuestManager.QuestType.upgrade_castle:
		{
			string newValue2 = string.Empty;
			if (curQuest.conA >= 0 && curQuest.conA < PlayInfo.castleManager.castle.Length)
			{
				newValue2 = PlayInfo.castleManager.castle[curQuest.conA].castleName;
			}
			textMsg.text = StringContent.msgQuestSysUpgradeCastle.Replace(StringContent.strValue, newValue2).Replace(StringContent.strValue2, curQuest.conB.ToString());
			break;
		}
		case QuestManager.QuestType.loyalty_castle:
		{
			string newValue5 = string.Empty;
			if (curQuest.conA >= 0 && curQuest.conA < PlayInfo.castleManager.castle.Length)
			{
				newValue5 = PlayInfo.castleManager.castle[curQuest.conA].castleName;
			}
			textMsg.text = StringContent.msgQuestSysLoyaltyCastle.Replace(StringContent.strValue, newValue5).Replace(StringContent.strValue2, curQuest.conB.ToString());
			break;
		}
		case QuestManager.QuestType.hero_stat:
		{
			int conA = curQuest.conA;
			string newValue = string.Empty;
			if (conA >= 0 && conA < 3)
			{
				newValue = ((QuestManager.StatKind)conA).ToString();
			}
			string msgQuestSysHeroStat = StringContent.msgQuestSysHeroStat;
			msgQuestSysHeroStat = msgQuestSysHeroStat.Replace(StringContent.strValue, newValue);
			msgQuestSysHeroStat = msgQuestSysHeroStat.Replace(StringContent.strValue2, curQuest.conB.ToString());
			textMsg.text = msgQuestSysHeroStat;
			break;
		}
		case QuestManager.QuestType.buy_item:
		{
			UnitItem unitItem = PlayInfo.itemManager.FindItem(curQuest.conA);
			string empty2 = string.Empty;
			if (unitItem != null)
			{
				empty2 = unitItem.name;
			}
			string msgQuestSysBuyItem = StringContent.msgQuestSysBuyItem;
			msgQuestSysBuyItem = msgQuestSysBuyItem.Replace(StringContent.strValue, empty2);
			textMsg.text = msgQuestSysBuyItem;
			break;
		}
		case QuestManager.QuestType.training_unit:
		{
			int num = curQuest.conA - 101;
			string newValue3 = string.Empty;
			if (num >= 0 && num < StringContent.wordSoldier.Length)
			{
				newValue3 = StringContent.wordSoldier[num];
			}
			string msgQuestSysTrainingUnit = StringContent.msgQuestSysTrainingUnit;
			msgQuestSysTrainingUnit = msgQuestSysTrainingUnit.Replace(StringContent.strValue, newValue3);
			msgQuestSysTrainingUnit = msgQuestSysTrainingUnit.Replace(StringContent.strValue2, curQuest.conB.ToString());
			textMsg.text = msgQuestSysTrainingUnit;
			break;
		}
		case QuestManager.QuestType.capture_castle:
		{
			string newValue4 = string.Empty;
			if (curQuest.conA >= 0 && curQuest.conA < PlayInfo.castleManager.castle.Length)
			{
				newValue4 = PlayInfo.castleManager.castle[curQuest.conA].castleName;
			}
			string msgQuestSysCaptureCastle = StringContent.msgQuestSysCaptureCastle;
			msgQuestSysCaptureCastle = msgQuestSysCaptureCastle.Replace(StringContent.strValue, newValue4);
			textMsg.text = msgQuestSysCaptureCastle;
			break;
		}
		case QuestManager.QuestType.master_skill:
		{
			string empty = string.Empty;
			if (curQuest.conA >= 0 && curQuest.conA < PlayInfo.playerData.skill.skillLevel.Length)
			{
				empty = HeroSkill.levelSpec[curQuest.conA * 5].name;
			}
			string msgQuestSysMasterSkill = StringContent.msgQuestSysMasterSkill;
			msgQuestSysMasterSkill = msgQuestSysMasterSkill.Replace(StringContent.strValue, empty);
			msgQuestSysMasterSkill = msgQuestSysMasterSkill.Replace(StringContent.strValue2, curQuest.conB.ToString());
			textMsg.text = msgQuestSysMasterSkill;
			break;
		}
		case QuestManager.QuestType.total_lord:
		{
			string msgQuestSysTotalLord = StringContent.msgQuestSysTotalLord;
			msgQuestSysTotalLord = msgQuestSysTotalLord.Replace(StringContent.strValue, curQuest.conA.ToString());
			textMsg.text = msgQuestSysTotalLord;
			break;
		}
		case QuestManager.QuestType.heart_daughter:
		{
			string msgQuestSysHeartDaughter = StringContent.msgQuestSysHeartDaughter;
			msgQuestSysHeartDaughter = msgQuestSysHeartDaughter.Replace(StringContent.strValue, curQuest.conA.ToString());
			textMsg.text = msgQuestSysHeartDaughter;
			break;
		}
		case QuestManager.QuestType.hero_level:
		{
			string msgQuestSysHeroLevel = StringContent.msgQuestSysHeroLevel;
			msgQuestSysHeroLevel = msgQuestSysHeroLevel.Replace(StringContent.strValue, curQuest.conA.ToString());
			textMsg.text = msgQuestSysHeroLevel;
			break;
		}
		}
		textRewardXp.text = curQuest.rewardXp.ToString();
		bool flag2 = curQuest.rewardGold > 0;
		rewardType.gameObject.SetActive(flag2);
		textRewards.gameObject.SetActive(flag2);
		if (flag2)
		{
			textRewards.text = curQuest.rewardGold.ToString();
			rewardType.SetFrame(0);
		}
		bool flag3 = curQuest.rewardGem > 0;
		rewardTypeGem.gameObject.SetActive(flag3);
		textRewardsGem.gameObject.SetActive(flag3);
		if (flag3)
		{
			textRewardsGem.text = curQuest.rewardGem.ToString();
			rewardTypeGem.SetFrame(1);
		}
		int num2 = curQuest.period;
		if (result == 2)
		{
			num2 = PlayInfo.questManager.remaining;
		}
		textTimeLimit.text = num2 + " " + ((num2 <= 1) ? StringContent.wordDay : StringContent.wordDays);
		if (result == 3 || num2 < 1)
		{
			textTimeLimit.text = string.Empty;
		}
		RefreshQuestIcon();
	}

	private void OnAcceptClick(AuiButton sender)
	{
		PlayInfo.questManager.AcceptCurrentQuest();
		Refresh();
	}

	private void OnProcClick(AuiButton sender)
	{
		if (PlayInfo.questManager.result == 0)
		{
			return;
		}
		if (PlayInfo.questManager.result == 1)
		{
			PlayInfo.questManager.DenyCurrentQuest();
		}
		else if (PlayInfo.questManager.result == 2)
		{
			PlayInfo.questManager.DenyCurrentQuest();
		}
		else if (PlayInfo.questManager.result == 3)
		{
			bool isLevelUp = false;
			PlayInfo.questManager.RewardCurrentQuest(out isLevelUp);
			if (isLevelUp)
			{
				heroUnit.effectHeroSkill = heroUnit.gameObject.GetComponent<EffectHeroSkill>();
				heroUnit.effectHeroSkill.PlayExtraEffect(EffectHeroSkill.ExtraEffectType.levelUp);
				PlayInfo.soundManager.Play(ExtSoundManager.Effect2D.efs_levelup_effect, new Vector3(0f, 0f, 0f));
			}
		}
		Refresh();
		StartCoroutine("SearchNewQuest");
	}

	private IEnumerator SearchNewQuest()
	{
		yield return new WaitForSeconds(1f);
		PlayInfo.questManager.ProcessDaily(false);
		Refresh();
	}
}
