using UnityEngine;

public class UIGameMenu : MonoBehaviour
{
	public AuiButton buttonMap;

	public AuiButton buttonInventory;

	public AuiButton buttonSkill;

	public AuiButton buttonMenu;

	public AuiButton buttonCastle;

	public void Show()
	{
		base.gameObject.SetActiveRecursively(true);
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	public void Hide()
	{
		base.gameObject.SetActiveRecursively(false);
	}
}
