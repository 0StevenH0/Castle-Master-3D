using UnityEngine;

public class UIGameSlot : MonoBehaviour
{
	private const int idxHp = 0;

	private const int idxMp = 1;

	public GameObject[] objItem;

	public GameObject[] objSkill;

	public AuiButton[] buttonItem;

	public AuiButton[] buttonSkill;

	public AuiSprite[] iconItem;

	public AuiSprite[] iconSkill;

	public AuiSpriteAnimation[] aniItemCoolTime;

	public AuiSpriteAnimation[] aniSkillCoolTime;

	public AuiButton[] buttonWeaponChange;

	public GameObject[] itemQuantity;

	public TextMesh[] textQuantity;

	public GameObject objPotion;

	public GameObject objWeaponSkill;

	public WeaponManager.HeroWeaponType weaponType;

	public UnitControl heroUnit;

	private float[] skillCoolTime;

	public void Reset()
	{
		buttonItem[0].onButtonClick = OnHpClick;
		buttonItem[1].onButtonClick = OnMpClick;
		int num = 0;
		AuiButton[] array = buttonSkill;
		foreach (AuiButton auiButton in array)
		{
			auiButton.onButtonClick = OnSkillClick;
			auiButton.buttonTag = num;
			num++;
		}
		int num2 = 0;
		AuiButton[] array2 = buttonWeaponChange;
		foreach (AuiButton auiButton2 in array2)
		{
			auiButton2.onButtonClick = OnWeaponChangeClick;
			auiButton2.buttonTag = num2;
			num2++;
		}
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	public void Refresh()
	{
		bool flag = PlayInfo.playerData.slotHp != null;
		if (flag && PlayInfo.playerData.slotHp.quantity == 0)
		{
			flag = false;
		}
		iconItem[0].visible = flag;
		itemQuantity[0].SetActive(flag);
		if (flag)
		{
			iconItem[0].SetFrame((PlayInfo.playerData.slotHp.code == 401) ? 1 : 0);
			textQuantity[0].text = PlayInfo.playerData.slotHp.quantity.ToString();
		}
		flag = PlayInfo.playerData.slotMp != null;
		if (flag && PlayInfo.playerData.slotMp.quantity == 0)
		{
			flag = false;
		}
		iconItem[1].visible = flag;
		itemQuantity[1].SetActive(flag);
		if (flag)
		{
			iconItem[1].SetFrame((PlayInfo.playerData.slotMp.code != 411) ? 2 : 3);
			textQuantity[1].text = PlayInfo.playerData.slotMp.quantity.ToString();
		}
		int num = (int)weaponType;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < 3; i++)
		{
			int num4 = PlayInfo.playerData.equipSkill[num][i];
			if (num4 > -1)
			{
				num2++;
			}
			objSkill[i].SetActive(false);
		}
		num3 = objSkill.Length - num2;
		for (int j = 0; j < 3; j++)
		{
			int num5 = PlayInfo.playerData.equipSkill[num][j];
			objSkill[j].SetActive(num5 > -1);
			if (num5 > -1)
			{
				num3++;
				iconSkill[j].SetFrame(num5);
				buttonSkill[j].buttonTag = j;
				int num6 = PlayInfo.playerData.skill.skillLevel[num5];
				HeroSkill.SkillLevelSpec skillLevelSpec = HeroSkill.GetSkillLevelSpec(num5, num6 - 1);
				SetSkillCoolTime(j, num5, skillCoolTime[num5], skillLevelSpec.colldown);
			}
		}
	}

	private void SetSkillCoolTime(int slot, int skillIndex, float curCoolTime, float maxCoolTime)
	{
		if (curCoolTime >= 0f)
		{
			aniSkillCoolTime[slot].visible = true;
			int num = aniSkillCoolTime[slot].materials.Length;
			int num2 = (int)((float)num * (maxCoolTime - curCoolTime) / maxCoolTime);
			if (num2 < 0)
			{
				num2 = 0;
			}
			if (num2 >= num)
			{
				num2 = num - 1;
			}
			aniSkillCoolTime[slot].drawFPS = (float)num / maxCoolTime;
			aniSkillCoolTime[slot].SetFrame(num2);
			aniSkillCoolTime[slot].StartAnimation(false, true);
		}
		else
		{
			aniSkillCoolTime[slot].visible = false;
		}
	}

	private void OnHpClick(AuiButton sender)
	{
		if (aniItemCoolTime[0].visible || PlayInfo.heroState.curHp == 0f || PlayInfo.playerData.slotHp == null)
		{
			return;
		}
		if (PlayInfo.playerData.slotHp.quantity > 0)
		{
			PlayInfo.heroState.curHp += PlayInfo.playerData.slotHp.ability.hp;
			PlayInfo.playerData.slotHp.quantity--;
			if (PlayInfo.playerData.slotHp.quantity < 0)
			{
				PlayInfo.playerData.slotHp.quantity = 0;
			}
			if (PlayInfo.heroState.curHp > (float)PlayInfo.heroState.sumHp)
			{
				PlayInfo.heroState.curHp = PlayInfo.heroState.sumHp;
			}
			aniItemCoolTime[0].visible = true;
			aniItemCoolTime[0].drawFPS = (float)aniItemCoolTime[0].materials.Length / PlayInfo.playerData.slotHp.ability.colltime;
			aniItemCoolTime[0].StartAnimation(0, false, true);
			PlayInfo.soundManager.Play(ExtSoundManager.Effect2D.efs_drink_potion, new Vector3(0f, 0f, 0f));
			if (heroUnit != null)
			{
				if (heroUnit.effectHeroSkill == null)
				{
					heroUnit.effectHeroSkill = heroUnit.gameObject.GetComponent<EffectHeroSkill>();
				}
				heroUnit.effectHeroSkill.PlayExtraEffect(EffectHeroSkill.ExtraEffectType.potionHp);
			}
		}
		textQuantity[0].text = PlayInfo.playerData.slotHp.quantity.ToString();
		if (PlayInfo.playerData.slotHp.quantity == 0)
		{
			iconItem[0].visible = false;
			itemQuantity[0].SetActive(false);
		}
	}

	private void OnMpClick(AuiButton sender)
	{
		if (aniItemCoolTime[1].visible || PlayInfo.heroState.curHp == 0f || PlayInfo.playerData.slotMp == null)
		{
			return;
		}
		if (PlayInfo.playerData.slotMp.quantity > 0)
		{
			PlayInfo.heroState.curMp += PlayInfo.playerData.slotMp.ability.mp;
			PlayInfo.playerData.slotMp.quantity--;
			if (PlayInfo.playerData.slotMp.quantity < 0)
			{
				PlayInfo.playerData.slotMp.quantity = 0;
			}
			if (PlayInfo.heroState.curMp > (float)PlayInfo.heroState.sumMp)
			{
				PlayInfo.heroState.curMp = PlayInfo.heroState.sumMp;
			}
			aniItemCoolTime[1].visible = true;
			aniItemCoolTime[1].drawFPS = (float)aniItemCoolTime[1].materials.Length / PlayInfo.playerData.slotMp.ability.colltime;
			aniItemCoolTime[1].StartAnimation(0, false, true);
			PlayInfo.soundManager.Play(ExtSoundManager.Effect2D.efs_drink_potion, new Vector3(0f, 0f, 0f));
			if (heroUnit != null)
			{
				if (heroUnit.effectHeroSkill == null)
				{
					heroUnit.effectHeroSkill = heroUnit.gameObject.GetComponent<EffectHeroSkill>();
				}
				heroUnit.effectHeroSkill.PlayExtraEffect(EffectHeroSkill.ExtraEffectType.potionHp);
			}
		}
		textQuantity[1].text = PlayInfo.playerData.slotMp.quantity.ToString();
		if (PlayInfo.playerData.slotMp.quantity == 0)
		{
			iconItem[1].visible = false;
			itemQuantity[1].SetActive(false);
		}
	}

	private void OnSkillClick(AuiButton sender)
	{
		int buttonTag = sender.buttonTag;
		int num = (int)weaponType;
		int num2 = PlayInfo.playerData.equipSkill[num][buttonTag];
		if (num2 > -1 && skillCoolTime[num2] < 0f)
		{
			int num3 = PlayInfo.playerData.skill.skillLevel[num2];
			HeroSkill.SkillType skillType = (HeroSkill.SkillType)num2;
			HeroSkill.SkillLevelSpec skillLevelSpec = HeroSkill.levelSpec[num2 * 5 + num3 - 1];
			if ((float)skillLevelSpec.mp <= PlayInfo.heroState.curMp)
			{
				PlayInfo.heroState.curMp -= skillLevelSpec.mp;
				heroUnit.SetAttackMode(UnitCharactor.AttackMode.skill, (int)skillType % 5);
				heroUnit.Attack(skillLevelSpec);
				skillCoolTime[num2] = skillLevelSpec.colldown;
				SetSkillCoolTime(buttonTag, num2, skillLevelSpec.colldown, skillLevelSpec.colldown);
			}
		}
	}

	public void Show()
	{
		base.gameObject.SetActive(true);
		Vector3 localScale = new Vector3(1f, 1f, 1f);
		if (!UserSetting.tabletMode)
		{
			localScale = new Vector3(1.3f, 1.3f, 1f);
		}
		objPotion.transform.localScale = localScale;
		objWeaponSkill.transform.localScale = localScale;
		skillCoolTime = new float[15];
		for (int i = 0; i < 15; i++)
		{
			skillCoolTime[i] = -1f;
		}
		weaponType = WeaponManager.HeroWeaponType.onehand;
		SetWeaponIcon(0);
		Reset();
		Refresh();
		aniItemCoolTime[0].visible = false;
		aniItemCoolTime[1].visible = false;
		for (int j = 0; j < 3; j++)
		{
			aniSkillCoolTime[j].visible = false;
		}
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			OnHpClick(buttonItem[0]);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			OnMpClick(buttonItem[1]);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			OnSkillClick(buttonSkill[0]);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			OnSkillClick(buttonSkill[1]);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			OnSkillClick(buttonSkill[2]);
		}
		for (int i = 0; i < skillCoolTime.Length; i++)
		{
			if (skillCoolTime[i] > 0f)
			{
				skillCoolTime[i] -= Time.deltaTime;
				if (skillCoolTime[i] <= 0f)
				{
					skillCoolTime[i] = -1f;
				}
			}
		}
	}

	private void OnWeaponChangeClick(AuiButton sender)
	{
		int buttonTag = sender.buttonTag;
		buttonTag++;
		if (buttonTag > PlayInfo.playerData.equipWeapon.Length - 1)
		{
			buttonTag = 0;
		}
		while (PlayInfo.playerData.equipWeapon[buttonTag] == null)
		{
			buttonTag++;
			if (buttonTag > PlayInfo.playerData.equipWeapon.Length - 1)
			{
				buttonTag = 0;
			}
		}
		UnitCharactor.WeaponType weapon = UnitCharactor.WeaponType.onehand;
		switch (buttonTag)
		{
		case 0:
			weapon = UnitCharactor.WeaponType.onehand;
			break;
		case 1:
			weapon = UnitCharactor.WeaponType.doublehand;
			break;
		case 2:
			weapon = UnitCharactor.WeaponType.bigsword;
			break;
		}
		heroUnit.SetWeapon(weapon, PlayInfo.playerData.equipWeapon[buttonTag].code);
		weaponType = (WeaponManager.HeroWeaponType)buttonTag;
		Refresh();
		SetWeaponIcon(buttonTag);
	}

	private void SetWeaponIcon(int idx)
	{
		for (int i = 0; i < buttonWeaponChange.Length; i++)
		{
			buttonWeaponChange[i].visible = idx == i;
		}
	}
}
