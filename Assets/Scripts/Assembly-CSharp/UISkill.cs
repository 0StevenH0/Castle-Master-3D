using System.Collections.Generic;
using UnityEngine;

public class UISkill : MonoBehaviour
{
	public AuiButton buttonClose;

	public AuiButton buttonEquip;

	public AuiSprite buttonEquipLabel;

	public AuiButton[] buttonSkill;

	public AuiButton[] buttonWeapon;

	public AuiSprite iconSkillList;

	public AuiSprite skillSelected;

	public AuiSprite[] iconSkillEquip;

	public TextMesh[] skillListLevel;

	public TextMesh textSkillName;

	public TextMesh textSkillLevel;

	public TextMesh textRequireLv;

	public TextMesh[] textAttrName;

	public TextMesh[] textAttrValue;

	public TextMesh textCost;

	public AuiSprite iconCurrency;

	private WeaponManager.HeroWeaponType weaponType;

	private AuiSprite[] skillIcon;

	private int selectedSkillIdx;

	private void Start()
	{
		AuiButton.SetTopAllChild(base.transform);
		buttonClose.onButtonClick = OnCloseClick;
		for (int i = 0; i < buttonWeapon.Length; i++)
		{
			buttonWeapon[i].SetFrame((i == (int)weaponType) ? 1 : 0);
		}
	}

	public void SetWeaponType(WeaponManager.HeroWeaponType type)
	{
		weaponType = type;
		if (skillIcon == null)
		{
			List<AuiSprite> list = new List<AuiSprite>();
			for (int i = 0; i < 5; i++)
			{
				GameObject gameObject = iconSkillList.gameObject;
				if (i > 0)
				{
					gameObject = Object.Instantiate(iconSkillList.gameObject) as GameObject;
				}
				gameObject.transform.parent = iconSkillList.transform.parent;
				AuiSprite component = gameObject.GetComponent<AuiSprite>();
				list.Add(component);
			}
			skillIcon = list.ToArray();
		}
		RefreshSkillList();
		ClearSkillDetail();
		buttonEquip.onButtonClick = OnEquipClick;
		for (int j = 0; j < buttonWeapon.Length; j++)
		{
			buttonWeapon[j].onButtonClick = OnWeaponClick;
			buttonWeapon[j].buttonTag = j;
		}
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	private void RefreshSkillList()
	{
		int num = (int)weaponType;
		for (int i = 0; i < buttonWeapon.Length; i++)
		{
			buttonWeapon[i].SetFrame((i == num) ? 1 : 0);
		}
		for (int j = 0; j < 5; j++)
		{
			int num2 = HeroSkill.skillClass[num][j];
			int num3 = PlayInfo.playerData.skill.skillLevel[num2];
			if (num3 > 0)
			{
				Vector3 localPosition = buttonSkill[j].transform.localPosition;
				localPosition.z = iconSkillList.transform.localPosition.z;
				skillIcon[j].transform.localPosition = localPosition;
				skillIcon[j].visible = true;
				skillIcon[j].SetFrame(num2);
				localPosition = buttonSkill[j].transform.localPosition;
				skillListLevel[j].text = num3.ToString();
				buttonSkill[j].buttonTag = j;
				buttonSkill[j].visible = true;
				buttonSkill[j].onButtonClick = OnSkillClick;
				iconSkillEquip[j].visible = false;
				for (int k = 0; k < PlayInfo.playerData.equipSkill[num].Length; k++)
				{
					if (PlayInfo.playerData.equipSkill[num][k] == num2)
					{
						iconSkillEquip[j].visible = true;
						break;
					}
				}
			}
			else
			{
				skillIcon[j].visible = false;
				skillListLevel[j].text = string.Empty;
				buttonSkill[j].visible = false;
				iconSkillEquip[j].visible = false;
			}
		}
	}

	private void ClearSkillDetail()
	{
		textSkillName.gameObject.active = false;
		textRequireLv.gameObject.active = false;
		textSkillLevel.gameObject.active = false;
		TextMesh[] array = textAttrName;
		foreach (TextMesh textMesh in array)
		{
			textMesh.gameObject.active = false;
		}
		TextMesh[] array2 = textAttrValue;
		foreach (TextMesh textMesh2 in array2)
		{
			textMesh2.gameObject.active = false;
		}
		skillSelected.visible = false;
		textCost.gameObject.active = false;
		iconCurrency.visible = false;
		buttonEquip.visible = false;
		buttonEquipLabel.visible = false;
	}

	private void OnSkillClick(AuiButton sender)
	{
		int num = (selectedSkillIdx = sender.buttonTag);
		ClearSkillDetail();
		Vector3 localPosition = sender.transform.localPosition;
		localPosition.z = skillSelected.transform.localPosition.z;
		skillSelected.transform.localPosition = localPosition;
		skillSelected.visible = true;
		int num2 = (int)weaponType;
		int num3 = HeroSkill.skillClass[num2][num];
		int num4 = num3 * 5 + PlayInfo.playerData.skill.skillLevel[num] - 1;
		HeroSkill.SkillLevelSpec skillLevelSpec = HeroSkill.levelSpec[num4];
		if (skillLevelSpec == null)
		{
			return;
		}
		textSkillName.text = skillLevelSpec.name;
		textSkillName.gameObject.active = true;
		textSkillLevel.text = "Lv. " + skillLevelSpec.skillLevel;
		textSkillLevel.gameObject.active = true;
		textRequireLv.text = StringContent.msgRequireHeroLevel.Replace(StringContent.strValue, skillLevelSpec.requireLevel.ToString());
		textRequireLv.gameObject.active = true;
		textCost.gameObject.active = true;
		iconCurrency.visible = true;
		textCost.text = skillLevelSpec.price.ToString();
		iconCurrency.SetFrame(skillLevelSpec.currency);
		int num5 = 0;
		int num6 = textAttrName.Length;
		if (num5 < num6 && skillLevelSpec.attack != 0)
		{
			textAttrName[num5].text = "ATK";
			textAttrName[num5].gameObject.active = true;
			textAttrValue[num5].text = "+" + skillLevelSpec.attack;
			textAttrValue[num5].gameObject.active = true;
			num5++;
		}
		if (num5 < num6 && skillLevelSpec.hp != 0)
		{
			textAttrName[num5].text = "HP";
			textAttrName[num5].gameObject.active = true;
			textAttrValue[num5].text = "+" + skillLevelSpec.hp;
			textAttrValue[num5].gameObject.active = true;
			num5++;
		}
		if (num5 < num6 && skillLevelSpec.defense != 0)
		{
			textAttrName[num5].text = "DEF";
			textAttrName[num5].gameObject.active = true;
			textAttrValue[num5].text = "+" + skillLevelSpec.defense;
			textAttrValue[num5].gameObject.active = true;
			num5++;
		}
		if (num5 < num6 && skillLevelSpec.length != 0f)
		{
			textAttrName[num5].text = "Length";
			textAttrName[num5].gameObject.active = true;
			textAttrValue[num5].text = skillLevelSpec.length + "m";
			textAttrValue[num5].gameObject.active = true;
			num5++;
		}
		if (num5 < num6 && skillLevelSpec.mp != 0)
		{
			textAttrName[num5].text = "MP";
			textAttrName[num5].gameObject.active = true;
			textAttrValue[num5].text = "-" + skillLevelSpec.mp;
			textAttrValue[num5].gameObject.active = true;
			num5++;
		}
		if (num5 < num6 && skillLevelSpec.colldown != 0f)
		{
			textAttrName[num5].text = "Cooldown";
			textAttrName[num5].gameObject.active = true;
			textAttrValue[num5].text = skillLevelSpec.colldown.ToString();
			textAttrValue[num5].gameObject.active = true;
			num5++;
		}
		if (num5 < num6 && skillLevelSpec.duration != 0f)
		{
			textAttrName[num5].text = "Duration";
			textAttrName[num5].gameObject.active = true;
			textAttrValue[num5].text = skillLevelSpec.duration.ToString();
			textAttrValue[num5].gameObject.active = true;
			num5++;
		}
		buttonEquip.visible = true;
		buttonEquipLabel.visible = true;
		bool flag = false;
		for (int i = 0; i < PlayInfo.playerData.equipSkill[num2].Length; i++)
		{
			if (PlayInfo.playerData.equipSkill[num2][i] == num3)
			{
				flag = true;
				break;
			}
		}
		buttonEquipLabel.SetFrame(flag ? 1 : 0);
	}

	private void OnWeaponClick(AuiButton sender)
	{
		SetWeaponType((WeaponManager.HeroWeaponType)sender.buttonTag);
	}

	private void OnEquipClick(AuiButton sender)
	{
		int num = selectedSkillIdx;
		int num2 = (int)weaponType;
		int num3 = HeroSkill.skillClass[num2][num];
		bool flag = false;
		for (int i = 0; i < PlayInfo.playerData.equipSkill[num2].Length; i++)
		{
			if (PlayInfo.playerData.equipSkill[num2][i] == num3)
			{
				PlayInfo.playerData.equipSkill[num2][i] = -1;
				flag = true;
				buttonEquipLabel.SetFrame((!flag) ? 1 : 0);
				break;
			}
		}
		if (!flag)
		{
			bool flag2 = false;
			for (int j = 0; j < PlayInfo.playerData.equipSkill[num2].Length; j++)
			{
				if (PlayInfo.playerData.equipSkill[num2][j] == -1)
				{
					PlayInfo.playerData.equipSkill[num2][j] = num3;
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				return;
			}
			buttonEquipLabel.SetFrame((!flag) ? 1 : 0);
		}
		RefreshSkillList();
		PlayInfo.soundManager.Play(ExtSoundManager.Effect2D.efs_skill_button, new Vector3(0f, 0f, 0f));
	}

	private void OnCloseClick(AuiButton sender)
	{
		Hide();
	}

	public void Show()
	{
		base.gameObject.SetActiveRecursively(true);
		AuiButton.topActive = true;
		SetWeaponType(WeaponManager.HeroWeaponType.onehand);
	}

	public void Hide()
	{
		base.gameObject.SetActiveRecursively(false);
		AuiButton.topActive = false;
	}
}
