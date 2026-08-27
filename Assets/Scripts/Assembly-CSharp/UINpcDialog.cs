using UnityEngine;

public class UINpcDialog : MonoBehaviour
{
	public enum DialogAlign
	{
		left = 0,
		middle = 1,
		right = 2
	}

	public AuiButton buttonYes;

	public AuiButton buttonNo;

	public AuiSprite buttonYesLabel;

	public AuiSprite buttonNoLabel;

	public TextMesh textMsg;

	public void Show(string message, DialogAlign align, AuiButton.OnButtonClick onYes, AuiButton.OnButtonClick onNo)
	{
		base.gameObject.SetActiveRecursive(true);
		buttonYes.onButtonClick = onYes;
		buttonNo.onButtonClick = onNo;
		buttonYes.visible = onYes != null;
		buttonNo.visible = onNo != null;
		buttonYesLabel.visible = onYes != null;
		buttonNoLabel.visible = onNo != null;
		textMsg.text = message;
		Vector3 localPosition = textMsg.transform.localPosition;
		localPosition.y = ((onYes == null && onNo == null) ? 20 : 50);
		textMsg.transform.localPosition = localPosition;
		localPosition = base.transform.position;
		switch (align)
		{
		case DialogAlign.left:
			localPosition.x = -258f;
			break;
		case DialogAlign.middle:
			localPosition.x = 0f;
			break;
		case DialogAlign.right:
			localPosition.x = 258f;
			break;
		}
		base.transform.position = localPosition;
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	public void Hide()
	{
		base.gameObject.SetActiveRecursive(false);
	}
}
