using UnityEngine;

public class UIGameHelp : MonoBehaviour
{
	public AuiButton buttonClose;

	public TextMesh helpControl;

	public TextMesh helpCastleDefense;

	public TextMesh helpAllyUnits;

	public TextMesh helpEnemyUnits;

	public TextMesh helpTimeLimit;

	public TextMesh helpChangeWeapon;

	public TextMesh helpSkillAttack;

	public TextMesh helpHPPotion;

	public TextMesh helpMPPotion;

	private void Start()
	{
		buttonClose.onButtonClick = OnCloseClick;
		RefreshDesc();
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	private void RefreshDesc()
	{
		helpControl.text = StringContent.helpControl;
		helpCastleDefense.text = StringContent.helpCastleDefense;
		helpAllyUnits.text = StringContent.helpAllyUnits;
		helpEnemyUnits.text = StringContent.helpEnemyUnits;
		helpTimeLimit.text = StringContent.helpTimeLimit;
		helpChangeWeapon.text = StringContent.helpChangeWeapon;
		helpSkillAttack.text = StringContent.helpSkillAttack;
		helpHPPotion.text = StringContent.helpHPPotion;
		helpMPPotion.text = StringContent.helpMPPotion;
	}

	public void Show()
	{
		base.gameObject.SetActiveRecursively(true);
		RefreshDesc();
	}

	public void Hide()
	{
		base.gameObject.SetActiveRecursively(false);
	}

	private void OnCloseClick(AuiButton sender)
	{
		Hide();
		UserSetting.viewGameHelp = false;
		UserSetting.Save();
	}
}
