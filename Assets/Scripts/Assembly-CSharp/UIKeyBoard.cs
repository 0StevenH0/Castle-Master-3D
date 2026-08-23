using UnityEngine;

public class UIKeyBoard : MonoBehaviour
{
	public AuiButton[] buttonAlpha;

	public AuiButton[] buttonNumber;

	public AuiButton buttonEnter;

	public AuiButton buttonShift;

	public AuiButton buttonBack;

	public AuiButton buttonSpace;

	public AuiSprite labelShift;

	public bool allowInput = true;

	public int maxLength = 10;

	public string textValue = string.Empty;

	private bool isUpper;

	private void Start()
	{
		int num = 65;
		AuiButton[] array = buttonAlpha;
		foreach (AuiButton auiButton in array)
		{
			auiButton.buttonTag = num;
			auiButton.onButtonClick = OnKeyClick;
			num++;
		}
		int num2 = 48;
		AuiButton[] array2 = buttonNumber;
		foreach (AuiButton auiButton2 in array2)
		{
			auiButton2.buttonTag = num2;
			auiButton2.onButtonClick = OnKeyClick;
			num2++;
		}
		buttonShift.onButtonClick = OnShiftClick;
		buttonBack.onButtonClick = OnBackClick;
		buttonSpace.onButtonClick = OnKeyClick;
		buttonSpace.buttonTag = 32;
		UpdateAlpha();
	}

	private void OnKeyClick(AuiButton sender)
	{
		if (allowInput && textValue.Length < maxLength)
		{
			string text = ((char)sender.buttonTag).ToString();
			if (!isUpper)
			{
				text = text.ToLower();
			}
			textValue += text;
		}
	}

	private void OnEnterClick(AuiButton sender)
	{
	}

	private void OnShiftClick(AuiButton sender)
	{
		isUpper = !isUpper;
		UpdateAlpha();
	}

	private void OnBackClick(AuiButton sender)
	{
		if (textValue.Length != 0)
		{
			textValue = textValue.Substring(0, textValue.Length - 1);
		}
	}

	private void UpdateAlpha()
	{
		TextMesh[] componentsInChildren = base.gameObject.GetComponentsInChildren<TextMesh>();
		foreach (TextMesh textMesh in componentsInChildren)
		{
			if (textMesh.text.Length == 1)
			{
				if (isUpper)
				{
					textMesh.text = textMesh.text.ToUpper();
				}
				else
				{
					textMesh.text = textMesh.text.ToLower();
				}
			}
		}
		labelShift.SetFrame(isUpper ? 1 : 0);
	}
}
