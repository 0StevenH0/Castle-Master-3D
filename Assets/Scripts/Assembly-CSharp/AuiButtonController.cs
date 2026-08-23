using UnityEngine;

public class AuiButtonController : MonoBehaviour
{
	public bool isActive = true;

	private AuiButton selectedButton;

	private void Update()
	{
		if (!isActive)
		{
			return;
		}
		if (selectedButton != null && !selectedButton.isWaitForButtonUp)
		{
			selectedButton = null;
		}
		if (selectedButton == null && Input.GetMouseButtonDown(0))
		{
			Vector3 mousePosition = Input.mousePosition;
			foreach (AuiButton button in AuiButton.buttonList)
			{
				if (button.visible && button.enabled && button.gameObject.activeInHierarchy && button.CheckButtonDown(mousePosition))
				{
					selectedButton = button;
					break;
				}
			}
		}
		else if (selectedButton != null && Input.GetMouseButtonUp(0))
		{
			selectedButton.ForcedButtonClicked();
			selectedButton = null;
		}
		if (Time.timeScale == 0f && selectedButton != null && selectedButton.isWaitForButtonUp)
		{
			selectedButton.WaitForButtonUp_OnPause();
		}
	}

	public bool CheckForButtonArea(float x, float y)
	{
		foreach (AuiButton button in AuiButton.buttonList)
		{
			if (button.visible && button.enabled && button.gameObject.activeInHierarchy && button.CheckButtonInSide(x, y))
			{
				return true;
			}
		}
		return false;
	}
}
