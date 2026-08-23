using UnityEngine;

public class UIPlayMenu : MonoBehaviour
{
	public AuiButton buttonResume;

	public AuiButton buttonOption;

	public AuiButton buttonTutorial;

	public AuiButton buttonExit;

	public AuiSprite buttonExitLabel;

	public AuiSprite buttonTutorialLabel;

	public ProcBattle procBattle;

	public ProcCastle procCastle;

	private bool isBattle;

	private void Start()
	{
		buttonResume.isMostTop = true;
		buttonOption.isMostTop = true;
		buttonTutorial.isMostTop = true;
		buttonExit.isMostTop = true;
		buttonResume.onButtonClick = OnResume;
		buttonOption.onButtonClick = OnOption;
		buttonTutorial.onButtonClick = OnTutorialClick;
		buttonExit.onButtonClick = OnExit;
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	public void Show(bool isBattle)
	{
		AuiButton.mostTopActive = true;
		base.gameObject.SetActiveRecursively(true);
		this.isBattle = isBattle;
		if (isBattle)
		{
			Time.timeScale = 0f;
			buttonExit.visible = false;
			buttonExitLabel.visible = false;
			buttonTutorialLabel.SetFrame(1);
		}
		else
		{
			PlayInfo.gameTime.pause = true;
			buttonExit.visible = true;
			buttonExitLabel.visible = true;
			buttonTutorialLabel.SetFrame(0);
		}
	}

	public void Hide()
	{
		AuiButton.mostTopActive = false;
		base.gameObject.SetActiveRecursively(false);
		if (isBattle)
		{
			Time.timeScale = 1f;
		}
		else
		{
			PlayInfo.gameTime.pause = false;
		}
	}

	private void OnResume(AuiButton sender)
	{
		Hide();
	}

	private void OnTutorialClick(AuiButton sender)
	{
		if (isBattle)
		{
			UserSetting.viewGameHelp = true;
			UserSetting.Save();
			procBattle.uiGameHelp.Show();
		}
		else
		{
			procCastle.uiTutorial.Show();
		}
	}

	private void OnOption(AuiButton sender)
	{
		if (isBattle)
		{
			procBattle.uiUserSetting.Show();
		}
		else
		{
			procCastle.uiUserSetting.Show();
		}
	}

	private void OnExit(AuiButton sender)
	{
		PlayInfo.Save();
		Hide();
		if (!isBattle)
		{
			ProcBase.LoadScene("Scene Title", ProcLoading.ContentMode.mainmenu);
		}
	}
}
