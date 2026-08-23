using System.Collections.Generic;
using UnityEngine;

public class UISkillMaster : MonoBehaviour
{
	public AuiButton buttonClose;

	public AuiButton buttonBuy;

	public AuiSprite buttonBuyLabel;

	public AuiButton[] buttonSkill;

	public AuiButton[] buttonWeapon;

	public AuiSprite iconSkillList;

	public AuiSprite iconSkillDisable;

	public AuiSprite skillSelected;

	public TextMesh[] skillListLevel;

	public TextMesh textSkillName;

	public TextMesh textSkillLevel;

	public TextMesh textRequireLv;

	public TextMesh[] textAttrName;

	public TextMesh[] textAttrValue;

	public TextMesh textCost;

	public AuiSprite iconCurrency;

	public ProcCastle procCastle;

	private WeaponManager.HeroWeaponType weaponType;

	private AuiSprite[] skillIcon;

	private AuiSprite[] skillDisable;

	private int selectedSkillIdx;

	private void Start()
	{
		for (int i = 0; i < buttonWeapon.Length; i++)
		{
			buttonWeapon[i].SetFrame((i == (int)weaponType) ? 1 : 0);
		}
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	public void SetWeaponType(WeaponManager.HeroWeaponType type)
	{
		weaponType = type;
		for (int i = 0; i < buttonWeapon.Length; i++)
		{
			buttonWeapon[i].SetFrame((i == (int)type) ? 1 : 0);
		}
		if (skillIcon == null)
		{
			List<AuiSprite> list = new List<AuiSprite>();
			List<AuiSprite> list2 = new List<AuiSprite>();
			for (int j = 0; j < 5; j++)
			{
				GameObject gameObject = iconSkillList.gameObject;
				if (j > 0)
				{
					gameObject = Object.Instantiate(iconSkillList.gameObject) as GameObject;
				}
				gameObject.transform.parent = iconSkillList.transform.parent;
				AuiSprite component = gameObject.GetComponent<AuiSprite>();
				list.Add(component);
				gameObject = iconSkillDisable.gameObject;
				if (j > 0)
				{
					gameObject = Object.Instantiate(iconSkillDisable.gameObject) as GameObject;
				}
				gameObject.transform.parent = iconSkillList.transform.parent;
				component = gameObject.GetComponent<AuiSprite>();
				list2.Add(component);
			}
			skillIcon = list.ToArray();
			skillDisable = list2.ToArray();
		}
		RefreshSkillList();
		ClearSkillDetail();
		buttonBuy.onButtonClick = OnBuyClick;
		for (int k = 0; k < buttonWeapon.Length; k++)
		{
			buttonWeapon[k].onButtonClick = OnWeaponClick;
			buttonWeapon[k].buttonTag = k;
		}
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	private void RefreshSkillList()
	{
		int num = (int)weaponType;
		for (int i = 0; i < 5; i++)
		{
			int num2 = HeroSkill.skillClass[num][i];
			int num3 = PlayInfo.playerData.skill.skillLevel[num2];
			Vector3 localPosition = buttonSkill[i].transform.localPosition;
			localPosition.z = iconSkillList.transform.localPosition.z;
			skillIcon[i].transform.localPosition = localPosition;
			skillIcon[i].visible = true;
			skillIcon[i].SetFrame(num2);
			if (num3 < 5)
			{
				localPosition = buttonSkill[i].transform.localPosition;
				skillListLevel[i].text = (num3 + 1).ToString();
				buttonSkill[i].buttonTag = i;
				buttonSkill[i].visible = true;
				buttonSkill[i].onButtonClick = OnSkillClick;
				skillDisable[i].visible = false;
			}
			else
			{
				localPosition.z -= 1f;
				skillDisable[i].transform.localPosition = localPosition;
				skillDisable[i].visible = true;
				skillListLevel[i].text = string.Empty;
				buttonSkill[i].visible = false;
			}
		}
	}

	private void ClearSkillDetail()
	{
		textSkillName.gameObject.SetActive(false);
		textRequireLv.gameObject.SetActive(false);
		textSkillLevel.gameObject.SetActive(false);
		TextMesh[] array = textAttrName;
		foreach (TextMesh textMesh in array)
		{
			textMesh.gameObject.SetActive(false);
		}
		TextMesh[] array2 = textAttrValue;
		foreach (TextMesh textMesh2 in array2)
		{
			textMesh2.gameObject.SetActive(false);
		}
		skillSelected.visible = false;
		textCost.gameObject.SetActive(false);
		iconCurrency.visible = false;
		buttonBuy.visible = false;
		buttonBuyLabel.visible = false;
	}

	private void OnSkillClick(AuiButton sender)
	{
		int buttonTag = sender.buttonTag;
		selectedSkillIdx = buttonTag;
		RefreshSkillDetail();
		Vector3 localPosition = sender.transform.localPosition;
		localPosition.z = skillSelected.transform.localPosition.z;
		skillSelected.transform.localPosition = localPosition;
		skillSelected.visible = true;
	}

	private void RefreshSkillDetail()
	{
		int num = selectedSkillIdx;
		ClearSkillDetail();
		int num2 = (int)weaponType;
		int num3 = HeroSkill.skillClass[num2][num];
		int num4 = num3 * 5 + PlayInfo.playerData.skill.skillLevel[num3];
		HeroSkill.SkillLevelSpec skillLevelSpec = HeroSkill.levelSpec[num4];
		if (skillLevelSpec != null)
		{
			textSkillName.text = skillLevelSpec.name;
			textSkillName.gameObject.SetActive(true);
			textSkillLevel.text = "Lv. " + skillLevelSpec.skillLevel;
			textSkillLevel.gameObject.SetActive(true);
			textRequireLv.text = StringContent.msgRequireHeroLevel.Replace(StringContent.strValue, skillLevelSpec.requireLevel.ToString());
			textRequireLv.gameObject.SetActive(true);
			textCost.gameObject.SetActive(true);
			iconCurrency.visible = true;
			textCost.text = skillLevelSpec.price.ToString();
			iconCurrency.SetFrame(skillLevelSpec.currency);
			int num5 = 0;
			int num6 = textAttrName.Length;
			if (num5 < num6 && skillLevelSpec.attack != 0)
			{
				textAttrName[num5].text = "ATK";
				textAttrName[num5].gameObject.SetActive(true);
				textAttrValue[num5].text = "+" + skillLevelSpec.attack;
				textAttrValue[num5].gameObject.SetActive(true);
				num5++;
			}
			if (num5 < num6 && skillLevelSpec.hp != 0)
			{
				textAttrName[num5].text = "HP";
				textAttrName[num5].gameObject.SetActive(true);
				textAttrValue[num5].text = "+" + skillLevelSpec.hp;
				textAttrValue[num5].gameObject.SetActive(true);
				num5++;
			}
			if (num5 < num6 && skillLevelSpec.defense != 0)
			{
				textAttrName[num5].text = "DEF";
				textAttrName[num5].gameObject.SetActive(true);
				textAttrValue[num5].text = "+" + skillLevelSpec.defense;
				textAttrValue[num5].gameObject.SetActive(true);
				num5++;
			}
			if (num5 < num6 && skillLevelSpec.length != 0f)
			{
				textAttrName[num5].text = "Length";
				textAttrName[num5].gameObject.SetActive(true);
				textAttrValue[num5].text = skillLevelSpec.length + "m";
				textAttrValue[num5].gameObject.SetActive(true);
				num5++;
			}
			if (num5 < num6 && skillLevelSpec.mp != 0)
			{
				textAttrName[num5].text = "MP";
				textAttrName[num5].gameObject.SetActive(true);
				textAttrValue[num5].text = "-" + skillLevelSpec.mp;
				textAttrValue[num5].gameObject.SetActive(true);
				num5++;
			}
			if (num5 < num6 && skillLevelSpec.colldown != 0f)
			{
				textAttrName[num5].text = "Cooldown";
				textAttrName[num5].gameObject.SetActive(true);
				textAttrValue[num5].text = skillLevelSpec.colldown.ToString();
				textAttrValue[num5].gameObject.SetActive(true);
				num5++;
			}
			if (num5 < num6 && skillLevelSpec.duration != 0f)
			{
				textAttrName[num5].text = "Duration";
				textAttrName[num5].gameObject.SetActive(true);
				textAttrValue[num5].text = skillLevelSpec.duration.ToString();
				textAttrValue[num5].gameObject.SetActive(true);
				num5++;
			}
			buttonBuy.visible = true;
			buttonBuyLabel.visible = true;
		}
	}

	private void OnWeaponClick(AuiButton sender)
	{
		SetWeaponType((WeaponManager.HeroWeaponType)sender.buttonTag);
	}

	private void OnBuyClick(AuiButton sender)
	{
		int num = selectedSkillIdx;
		int num2 = (int)weaponType;
		int num3 = HeroSkill.skillClass[num2][num];
		if (PlayInfo.playerData.skill.skillLevel[num3] >= 5)
		{
			return;
		}
		int num4 = num3 * 5 + PlayInfo.playerData.skill.skillLevel[num3];
		HeroSkill.SkillLevelSpec skillLevelSpec = HeroSkill.levelSpec[num4];
		if (skillLevelSpec.requireLevel > PlayInfo.heroState.level)
		{
			ProcBase.ShowMsg(StringContent.msgRequireHeroLevel.Replace(StringContent.strValue, skillLevelSpec.requireLevel.ToString()), MessageView.MsgIcon.alert);
			return;
		}
		if (skillLevelSpec.currency == 0)
		{
			if (skillLevelSpec.price > PlayInfo.playerData.gold)
			{
				ShowGotoGoldShop();
				return;
			}
			PlayInfo.playerData.gold -= skillLevelSpec.price;
		}
		else
		{
			if (skillLevelSpec.price > PlayInfo.playerData.gem)
			{
				ShowGotoGemShop();
				return;
			}
			PlayInfo.playerData.gem -= skillLevelSpec.price;
		}
		PlayInfo.playerData.skill.skillLevel[num3] = skillLevelSpec.skillLevel;
		PlayInfo.soundManager.Play(ExtSoundManager.Effect2D.efs_itembuy, new Vector3(0f, 0f, 0f));
		bool flag = false;
		for (int i = 0; i < PlayInfo.playerData.equipSkill[num2].Length; i++)
		{
			if (PlayInfo.playerData.equipSkill[num2][i] == num3)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			for (int j = 0; j < PlayInfo.playerData.equipSkill[num2].Length; j++)
			{
				if (PlayInfo.playerData.equipSkill[num2][j] == -1)
				{
					PlayInfo.playerData.equipSkill[num2][j] = num3;
					break;
				}
			}
		}
		RefreshSkillList();
		RefreshSkillDetail();
		if (PlayInfo.playerData.skill.skillLevel[num3] < 5)
		{
			skillSelected.visible = true;
		}
		else
		{
			ClearSkillDetail();
		}
	}

	public void Show()
	{
		base.gameObject.SetActive(true);
		SetWeaponType(WeaponManager.HeroWeaponType.onehand);
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	private void ShowGotoGoldShop()
	{
		ProcBase.ShowMsg(StringContent.msgNotEnoughGold, MessageView.MsgIcon.alert, new MessageView.MsgButton[2]
		{
			MessageView.MsgButton.yes,
			MessageView.MsgButton.no
		}, OnMessageGotoGemShop);
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
