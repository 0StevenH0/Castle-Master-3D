using UnityEngine;

public class MapCancel : MonoBehaviour
{
	public enum SelectingMode
	{
		redeploy = 0,
		attack = 1
	}

	public TextMesh textModeDesc;

	public AuiButton buttonCancel;

	public UIMap uiMap;

	private void Start()
	{
		buttonCancel.onButtonClick = OnCancelClick;
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	private void OnCancelClick(AuiButton sender)
	{
		uiMap.DisableCastleTargetMode();
	}

	public void Show(SelectingMode selMode)
	{
		base.gameObject.SetActive(true);
		switch (selMode)
		{
		case SelectingMode.redeploy:
			textModeDesc.text = StringContent.msgSelectCastleForRedploy;
			break;
		case SelectingMode.attack:
			textModeDesc.text = StringContent.msgSelectCastleForAttack;
			break;
		}
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
	}
}
