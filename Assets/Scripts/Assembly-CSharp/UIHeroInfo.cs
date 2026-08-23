using System.Collections;
using UnityEngine;

public class UIHeroInfo : MonoBehaviour
{
	public AuiSprite gaugeHP;

	public AuiSprite gaugeMP;

	public AuiSprite gaugeXP;

	public TextMesh playerLevel;

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
	}

	public void Show(bool autoRefresh)
	{
		base.gameObject.SetActiveRecursively(true);
		gaugeHP.isCrop = true;
		gaugeMP.isCrop = true;
		gaugeXP.isCrop = true;
		if (autoRefresh)
		{
			StartCoroutine("CoroutineHeroInfoRefresh");
		}
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	public void Hide()
	{
		base.gameObject.SetActiveRecursively(false);
		StopAllCoroutines();
	}

	private IEnumerator CoroutineHeroInfoRefresh()
	{
		float befHp = -1f;
		float befMp = -1f;
		float befXp = -1f;
		while (true)
		{
			if (befHp != PlayInfo.heroState.curHp)
			{
				befHp = PlayInfo.heroState.curHp;
				gaugeHP.crop.width = befHp / (float)PlayInfo.heroState.sumHp;
				gaugeHP.SetFrame(0);
			}
			if (befMp != PlayInfo.heroState.curMp)
			{
				befMp = PlayInfo.heroState.curMp;
				gaugeMP.crop.width = befMp / (float)PlayInfo.heroState.sumMp;
				gaugeMP.SetFrame(0);
			}
			if (befXp != PlayInfo.heroState.exp)
			{
				befXp = PlayInfo.heroState.exp;
				gaugeXP.crop.width = befXp / PlayInfo.GetNextExp(PlayInfo.heroState);
				gaugeXP.SetFrame(0);
			}
			playerLevel.text = PlayInfo.heroState.level.ToString();
			yield return new WaitForSeconds(0.2f);
		}
	}
}
