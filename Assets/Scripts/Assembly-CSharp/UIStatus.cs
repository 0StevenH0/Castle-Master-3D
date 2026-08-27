using System.Collections;
using UnityEngine;

public class UIStatus : MonoBehaviour
{
	public TextMesh textGameDay;

	public TextMesh textGameMonth;

	public TextMesh textGameYear;

	public TextMesh textGem;

	public TextMesh textGold;

	public GameObject panelAlert;

	public AuiSprite backAlert;

	public TextMesh textAlert;

	public AuiSprite iconCastle;

	public TextMesh castleName;

	public AuiButton buttonGoCastle;

	public UIMap uiMap;

	private void Start()
	{
		StartCoroutine("UpdateStatus");
		panelAlert.SetActiveRecursive(false);
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	private IEnumerator UpdateStatus()
	{
		while (true)
		{
			int day = PlayInfo.gameTime.day % 30;
			int month = PlayInfo.gameTime.day % 360 / 30;
			int year = PlayInfo.gameTime.day / 360 + 1;
			textGameDay.text = (day + 1).ToString();
			textGameMonth.text = StringContent.wordMonth[month];
			textGameYear.text = year.ToString();
			if (UserSetting.language == Language.korean || UserSetting.language == Language.japanese)
			{
				textGameYear.text = StringContent.wordMonth[month];
				textGameMonth.text = year + StringContent.wordYear;
			}
			textGem.text = PlayInfo.playerData.gem.ToString();
			textGold.text = PlayInfo.playerData.gold.ToString();
			yield return new WaitForSeconds(0.5f);
		}
	}

	public void BlinkAlert()
	{
		panelAlert.SetActiveRecursive(true);
		uiMap.noClickArea[3] = ProcBase.GetGameObjectRect(panelAlert.gameObject);
		StartCoroutine("LoopBlinkAlert");
	}

	private IEnumerator LoopBlinkAlert()
	{
		while (true)
		{
			textAlert.gameObject.SetActive(!textAlert.gameObject.activeInHierarchy);
			yield return new WaitForSeconds(0.25f);
		}
	}
}
