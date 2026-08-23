using System.Collections;
using UnityEngine;

public class ProcEnding : ProcBase
{
	public const string sceneName = "Scene Ending";

	public GameObject panelStep1;

	public GameObject panelStep2;

	public GameObject panelStep3;

	public GameObject panelStep4;

	public GameObject panelTalk1;

	public GameObject panelTalk3;

	public TextMesh textStep1;

	public TextMesh textStep2;

	public TextMesh textStep3;

	public TextMesh textStep4;

	public AuiSpriteAnimation aniBeforeSelect;

	public AuiSpriteAnimation aniAfterSelect;

	public AuiButton buttonSkip1;

	public AuiButton buttonSkip2;

	public static bool isWin = true;

	private bool isClickedBall;

	public override void OnStart()
	{
		buttonSkip1.onButtonClick = OnSkip1Click;
		buttonSkip2.onButtonClick = OnSkip2Click;
		AudioSource audioSource = base.gameObject.AddComponent<AudioSource>();
		audioSource.clip = ResourceManager.Load("Sound/bgm", "bgm_title", typeof(AudioClip)) as AudioClip;
		audioSource.loop = true;
		audioSource.volume = (float)UserSetting.volumeBgm / 100f;
		audioSource.Play();
		UserSetting.currentBgmSound = audioSource;
		ProcMain.InitGame();
		panelStep1.SetActive(false);
		panelStep2.SetActive(false);
		panelStep3.SetActive(false);
		panelStep4.SetActive(false);
		if (isWin)
		{
			panelStep1.SetActive(true);
			StartCoroutine("MoveUpTalkPanel", panelTalk1);
			textStep1.text = StringContent.msgEndingSuccess1;
			textStep2.text = StringContent.msgEndingSuccess2;
			textStep3.text = StringContent.msgEndingSuccess3;
			textStep4.text = StringContent.msgEndingSuccess4;
		}
		else
		{
			panelStep3.SetActive(true);
			StartCoroutine("MoveUpTalkPanel", panelTalk3);
			textStep3.text = StringContent.msgEndingFail1;
			textStep4.text = StringContent.msgEndingFail2;
		}
		ProcBase.ChangeTextMeshLanguage();
	}

	private void OnSkip1Click(AuiButton sender)
	{
		if (isWin)
		{
			panelStep1.SetActive(false);
			panelStep2.SetActive(true);
		}
		ProcBase.ChangeTextMeshLanguage();
	}

	private void OnSkip2Click(AuiButton sender)
	{
		if (isWin)
		{
			panelStep2.SetActive(false);
			panelStep3.SetActive(true);
			aniBeforeSelect.visible = true;
			aniAfterSelect.visible = false;
			aniBeforeSelect.StartAnimation(true, false);
		}
		ProcBase.ChangeTextMeshLanguage();
	}

	private IEnumerator MoveUpTalkPanel(GameObject panel)
	{
		Vector3 pos = panel.transform.localPosition;
		pos.y = -128f;
		panel.transform.localPosition = pos;
		yield return new WaitForSeconds(0.5f);
		while (pos.y < 0f)
		{
			pos.y += Time.deltaTime * 200f;
			if (pos.y > 0f)
			{
				pos.y = 0f;
			}
			panel.transform.localPosition = pos;
			yield return 1;
		}
	}

	private IEnumerator MoveDownTalkPanel(GameObject panel)
	{
		Vector3 pos = panel.transform.localPosition;
		pos.y = 0f;
		while (pos.y > -128f)
		{
			pos.y -= Time.deltaTime * 200f;
			if (pos.y < -128f)
			{
				pos.y = -128f;
			}
			panel.transform.localPosition = pos;
			yield return 1;
		}
	}

	private void Update()
	{
		if (isClickedBall || !panelStep3.activeInHierarchy || !Input.GetMouseButtonDown(0))
		{
			return;
		}
		Vector3 mousePosition = Input.mousePosition;
		if (buttonSkip1.uiCamera != null)
		{
			Vector3 vector = buttonSkip1.uiCamera.ScreenToWorldPoint(mousePosition);
			if (vector.x >= -75f && vector.x <= 70f && vector.y >= -140f && vector.y <= 38f)
			{
				aniBeforeSelect.visible = false;
				aniAfterSelect.visible = true;
				aniBeforeSelect.StartAnimation(0, false, true);
				isClickedBall = true;
				StartCoroutine("WaitForLastStep");
			}
		}
	}

	private IEnumerator WaitForLastStep()
	{
		StopCoroutine("MoveUpTalkPanel");
		StartCoroutine("MoveDownTalkPanel", panelTalk3);
		yield return new WaitForSeconds(1f);
		panelStep3.SetActive(false);
		panelStep4.SetActive(true);
		ProcBase.ChangeTextMeshLanguage();
		PlayInfo.soundManager.Play(ExtSoundManager.Effect2D.efs_rebirth, Vector3.zero);
		yield return new WaitForSeconds(2f);
		PlayInfo.Reinit();
		ProcMain.playStage = StageManager.StageType.castle;
		ProcBase.LoadScene("Scene Main", ProcLoading.ContentMode.castle);
	}
}
