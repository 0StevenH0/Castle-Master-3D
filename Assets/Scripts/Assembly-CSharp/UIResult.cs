using UnityEngine;

public class UIResult : MonoBehaviour
{
	public AuiSprite background;

	public AuiButton buttonClose;

	public AuiSprite labelReward;

	public AuiSprite[] pointStar;

	public TextMesh textBonus;

	public TextMesh textFame;

	public TextMesh textXp;

	public TextMesh textGold;

	public TextMesh textGem;

	public Material victoryFont;

	public Material defeatFont;

	private void Start()
	{
		buttonClose.isTop = true;
		buttonClose.onButtonClick = OnCloseClick;
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	private void OnCloseClick(AuiButton sender)
	{
		Hide();
		if (PlayInfo.castleManager.GetCastleCount(0) == 0)
		{
			ProcEnding.isWin = false;
			ProcBase.LoadScene("Scene Ending", ProcLoading.ContentMode.castle);
		}
		else if (PlayInfo.castleManager.GetCastleCount(1) == 0)
		{
			ProcEnding.isWin = true;
			ProcBase.LoadScene("Scene Ending", ProcLoading.ContentMode.castle);
		}
		else
		{
			ProcMain.playStage = StageManager.StageType.castle;
			ProcBase.LoadScene("Scene Main", ProcLoading.ContentMode.castle);
		}
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
		AuiButton.topActive = false;
	}

	public void Show(bool isAttack, bool isVictory, int rewardFame, int rewardXp, int rewardGold, int rewardGem, int point, int bonus)
	{
		if (isVictory)
		{
			background.SetFrame(0);
			labelReward.SetFrame(0);
			textFame.GetComponent<Renderer>().material = victoryFont;
			textXp.GetComponent<Renderer>().material = victoryFont;
			textGold.GetComponent<Renderer>().material = victoryFont;
			textGem.GetComponent<Renderer>().material = victoryFont;
			textBonus.text = "x " + bonus + "%";
		}
		else
		{
			background.SetFrame(1);
			labelReward.SetFrame(1);
			textFame.GetComponent<Renderer>().material = defeatFont;
			textXp.GetComponent<Renderer>().material = defeatFont;
			textGold.GetComponent<Renderer>().material = defeatFont;
			textGem.GetComponent<Renderer>().material = defeatFont;
			textBonus.text = string.Empty;
		}
		for (int i = 0; i < pointStar.Length; i++)
		{
			pointStar[i].SetFrame((i >= point) ? 1 : 0);
		}
		textFame.text = ((rewardFame <= 0) ? string.Empty : "+") + rewardFame;
		textXp.text = ((rewardXp <= 0) ? string.Empty : "+") + rewardXp;
		textGold.text = ((rewardGold <= 0) ? string.Empty : "+") + rewardGold;
		textGem.text = ((rewardGem <= 0) ? string.Empty : "+") + rewardGem;
		base.gameObject.SetActive(true);
		AuiButton.topActive = true;
		if (isVictory)
		{
			PlayInfo.soundManager.Play(ExtSoundManager.Effect2D.efs_victory, new Vector3(0f, 0f, 0f));
		}
		else
		{
			PlayInfo.soundManager.Play(ExtSoundManager.Effect2D.efs_defeat, new Vector3(0f, 0f, 0f));
		}
	}
}
