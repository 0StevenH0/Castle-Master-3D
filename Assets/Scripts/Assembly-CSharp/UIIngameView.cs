using System.Collections;
using UnityEngine;

public class UIIngameView : MonoBehaviour
{
	public class StatsObject
	{
		public bool isActive;

		public float moveY;

		public Vector3 pos;

		public TextMesh textValue;

		public Color textColor;
	}

	private const int maxDecHP = 20;

	private const int maxIncXP = 5;

	private const int maxEffectList = 10;

	public TextMesh textUnitHPDec;

	public TextMesh textUnitXPInc;

	public AuiSpriteAnimation aniHealing;

	public AuiSpriteAnimation aniDefenseUp;

	public AuiSprite aniLevelUp;

	public AuiSprite iconFinderCastle;

	public AuiSprite iconFinderTrap;

	public AuiSprite iconTrap;

	public AuiSprite iconCritical;

	public GameObject panelFinish;

	public TextMesh textFinish;

	public Camera uiCamera;

	public Camera gameCamera;

	public static UIIngameView self;

	private StatsObject[] statsList;

	private AuiSpriteAnimation[] aniHealingList;

	private AuiSpriteAnimation[] aniDefenseUpList;

	private Transform[] transHealingList;

	private Transform[] transDefenseUpList;

	private float[] offsetHealingList;

	private float[] offsetDefenseUpList;

	private Transform transCastle;

	private Transform transTrap;

	private void Start()
	{
		self = this;
		int num = 25;
		statsList = new StatsObject[num];
		for (int i = 0; i < num; i++)
		{
			TextMesh textMesh = null;
			textMesh = ((i < 20) ? textUnitHPDec : textUnitXPInc);
			StatsObject statsObject = new StatsObject();
			statsObject.textValue = Object.Instantiate(textMesh) as TextMesh;
			Material material = textMesh.GetComponent<Renderer>().material;
			statsObject.textValue.GetComponent<Renderer>().material = null;
			statsObject.textValue.GetComponent<Renderer>().sharedMaterial = Object.Instantiate(material) as Material;
			statsObject.textColor = material.color;
			statsObject.textValue.GetComponent<Renderer>().sharedMaterial.color = statsObject.textColor;
			statsObject.isActive = false;
			statsObject.textValue.transform.parent = textMesh.transform.parent;
			statsObject.textValue.transform.position = textMesh.transform.position;
			statsObject.textValue.gameObject.SetActive(false);
			statsList[i] = statsObject;
		}
		textUnitXPInc.gameObject.SetActive(false);
		textUnitHPDec.gameObject.SetActive(false);
		num = 10;
		aniHealingList = new AuiSpriteAnimation[num];
		aniDefenseUpList = new AuiSpriteAnimation[num];
		transHealingList = new Transform[num];
		transDefenseUpList = new Transform[num];
		offsetHealingList = new float[num];
		offsetDefenseUpList = new float[num];
		for (int j = 0; j < num; j++)
		{
			if (j == 0)
			{
				aniHealingList[j] = aniHealing;
				aniDefenseUpList[j] = aniDefenseUp;
			}
			else
			{
				aniHealingList[j] = Object.Instantiate(aniHealing) as AuiSpriteAnimation;
				aniDefenseUpList[j] = Object.Instantiate(aniDefenseUp) as AuiSpriteAnimation;
				aniHealingList[j].transform.parent = aniHealing.transform.parent;
				aniHealingList[j].transform.position = aniHealing.transform.position;
				aniDefenseUpList[j].transform.parent = aniDefenseUp.transform.parent;
				aniDefenseUpList[j].transform.position = aniDefenseUp.transform.position;
			}
			aniHealingList[j].visible = false;
			aniHealingList[j].gameObject.SetActiveRecursive(false);
			aniDefenseUpList[j].visible = false;
			aniDefenseUpList[j].gameObject.SetActiveRecursive(false);
		}
		iconFinderCastle.visible = false;
		iconFinderTrap.visible = false;
		iconTrap.visible = false;
		iconCritical.visible = false;
		aniLevelUp.visible = false;
		panelFinish.SetActiveRecursive(false);
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	private void Update()
	{
		if (uiCamera == null)
		{
			return;
		}
		int num = 25;
		for (int i = 0; i < num; i++)
		{
			if (!statsList[i].isActive)
			{
				continue;
			}
			StatsObject statsObject = statsList[i];
			statsObject.textColor.a -= Time.deltaTime;
			if (statsObject.textColor.a < 0f)
			{
				statsObject.textValue.gameObject.SetActive(false);
				statsObject.isActive = false;
				continue;
			}
			statsObject.moveY += Time.deltaTime * 100f;
			Vector3 position = gameCamera.WorldToScreenPoint(statsObject.pos);
			if (position.z > 0f)
			{
				Vector3 position2 = uiCamera.ScreenToWorldPoint(position);
				position2.z = statsObject.textValue.transform.position.z;
				position2.y += statsObject.moveY;
				statsObject.textValue.transform.position = position2;
			}
		}
		for (int j = 0; j < 10; j++)
		{
			if (aniHealingList[j].gameObject.activeInHierarchy)
			{
				UpdateHealingAnimation(j);
			}
			if (aniDefenseUpList[j].gameObject.activeInHierarchy)
			{
				UpdateDefenseUpAnimation(j);
			}
		}
		if (transCastle != null)
		{
			UpdateFinderCastle();
		}
		if (transTrap != null)
		{
			UpdateFinderTrap();
		}
	}

	public void SetActiveStats(int index, Vector3 pos, string curValue, float scale)
	{
		if (!statsList[index].isActive)
		{
			StatsObject statsObject = statsList[index];
			statsObject.isActive = true;
			statsObject.textColor.a = 1f;
			statsObject.textValue.text = curValue;
			statsObject.textValue.gameObject.SetActive(true);
			statsObject.pos = pos;
			statsObject.moveY = 0f;
			Vector3 position = gameCamera.WorldToScreenPoint(pos);
			if (position.z > 0f)
			{
				Vector3 position2 = uiCamera.ScreenToWorldPoint(position);
				position2.z = statsObject.textValue.transform.position.z;
				statsObject.textValue.transform.position = position2;
				statsObject.textValue.transform.localScale = new Vector3(scale, scale, 1f);
			}
		}
	}

	public void SetActiveDecHP(Vector3 pos, string curValue)
	{
		if (uiCamera == null || gameCamera == null)
		{
			return;
		}
		for (int i = 0; i < 20; i++)
		{
			if (!statsList[i].isActive)
			{
				SetActiveStats(i, pos, curValue, 1f);
				break;
			}
		}
	}

	public void SetActiveIncXP(Vector3 pos, string curValue, bool critical)
	{
		if (uiCamera == null || gameCamera == null)
		{
			return;
		}
		int num = 25;
		for (int i = 20; i < num; i++)
		{
			if (!statsList[i].isActive)
			{
				SetActiveStats(i, pos, curValue, (!critical) ? 1f : 2f);
				if (critical)
				{
					StopCoroutine("ShowCritical");
					StartCoroutine("ShowCritical", pos);
				}
				break;
			}
		}
	}

	public void SetHealing(Transform trans, float offsetY)
	{
		if (uiCamera == null || gameCamera == null || aniHealingList == null)
		{
			return;
		}
		for (int i = 0; i < 10; i++)
		{
			if (!aniHealingList[i].gameObject.activeInHierarchy)
			{
				transHealingList[i] = trans;
				offsetHealingList[i] = offsetY;
				aniHealingList[i].StartAnimation(0, false, true);
				aniHealingList[i].gameObject.SetActiveRecursive(true);
				UpdateHealingAnimation(i);
				break;
			}
		}
	}

	private IEnumerator ShowCritical(Vector3 pos)
	{
		float scaleMax = 1.5f;
		float scaleMax2 = 1.2f;
		float scale = 0.1f;
		Vector3 scrPos = gameCamera.WorldToScreenPoint(pos);
		if (scrPos.z > 0f)
		{
			Vector3 uiPos = uiCamera.ScreenToWorldPoint(scrPos);
			uiPos.z = iconCritical.transform.position.z;
			uiPos.x += 80f;
			iconCritical.transform.position = uiPos;
			iconCritical.transform.localScale = new Vector3(scale, scale, 0f);
			iconCritical.visible = true;
		}
		while (scale < scaleMax)
		{
			yield return 1;
			scale += Time.deltaTime * 10f;
			if (scale > scaleMax)
			{
				scale = scaleMax;
			}
			iconCritical.transform.localScale = new Vector3(scale, scale, 1f);
		}
		while (scale > scaleMax2)
		{
			yield return 1;
			scale -= Time.deltaTime * 10f;
			if (scale < scaleMax2)
			{
				scale = scaleMax2;
			}
			iconCritical.transform.localScale = new Vector3(scale, scale, 1f);
		}
		yield return new WaitForSeconds(0.5f);
		iconCritical.visible = false;
	}

	public void SetDefenseUp(Transform trans, float offsetY)
	{
		if (uiCamera == null || gameCamera == null || aniDefenseUpList == null)
		{
			return;
		}
		for (int i = 0; i < 10; i++)
		{
			if (!aniDefenseUpList[i].gameObject.activeInHierarchy)
			{
				transDefenseUpList[i] = trans;
				offsetDefenseUpList[i] = offsetY;
				aniDefenseUpList[i].StartAnimation(true, false);
				aniDefenseUpList[i].gameObject.SetActiveRecursive(true);
				UpdateDefenseUpAnimation(i);
				break;
			}
		}
	}

	private void UpdateHealingAnimation(int i)
	{
		bool visible = false;
		Vector3 position = transHealingList[i].position;
		position.y += offsetHealingList[i];
		Vector3 position2 = gameCamera.WorldToScreenPoint(position);
		if (position2.z > 0f)
		{
			Vector3 position3 = uiCamera.ScreenToWorldPoint(position2);
			position3.z = aniHealingList[i].transform.position.z;
			aniHealingList[i].transform.position = position3;
			visible = true;
		}
		aniHealingList[i].visible = visible;
		if (aniHealingList[i].curFrame >= aniHealingList[i].materials.Length - 1)
		{
			aniHealingList[i].visible = false;
			aniHealingList[i].gameObject.SetActiveRecursive(false);
		}
	}

	private void UpdateDefenseUpAnimation(int i)
	{
		bool visible = false;
		Vector3 position = transDefenseUpList[i].position;
		position.y += offsetDefenseUpList[i];
		Vector3 position2 = gameCamera.WorldToScreenPoint(position);
		if (position2.z > 0f)
		{
			Vector3 position3 = uiCamera.ScreenToWorldPoint(position2);
			position3.z = aniDefenseUpList[i].transform.position.z;
			aniDefenseUpList[i].transform.position = position3;
			visible = true;
		}
		aniDefenseUpList[i].visible = visible;
	}

	public void HideDefenseUpAnimationAll()
	{
		for (int i = 0; i < 10; i++)
		{
			if (aniDefenseUpList[i].gameObject.activeInHierarchy)
			{
				aniDefenseUpList[i].visible = false;
				aniDefenseUpList[i].gameObject.SetActiveRecursive(false);
			}
		}
	}

	public void SetCastleTransform(Transform trans)
	{
		transCastle = trans;
	}

	public void SetTrapTransform(Transform trans, BoobyTrap.TrapType curTrap)
	{
		transTrap = trans;
		iconTrap.SetFrame((int)curTrap);
		if (trans == null)
		{
			iconFinderTrap.visible = false;
			iconTrap.visible = false;
		}
	}

	private void UpdateFinderCastle()
	{
		bool visible = false;
		Vector3 position = transCastle.position;
		Vector3 position2 = gameCamera.WorldToScreenPoint(position);
		Vector3 position3 = uiCamera.ScreenToWorldPoint(position2);
		if (position3.x < (float)(-ScreenSize.Width) * 0.5f || position3.x > (float)ScreenSize.Width * 0.5f || position3.y < (float)(-ScreenSize.Height) * 0.5f || position3.y > (float)ScreenSize.Height * 0.5f || position2.z < 0f)
		{
			visible = true;
			int num = 16;
			if (position2.z < 0f)
			{
				position3.y = (float)(-ScreenSize.Height) * 0.5f + (float)num;
				position3.x = 0f - position3.x;
			}
			if (position3.x < (float)(-ScreenSize.Width) * 0.5f + (float)num)
			{
				position3.x = (float)(-ScreenSize.Width) * 0.5f + (float)num;
			}
			if (position3.x > (float)ScreenSize.Width * 0.5f - (float)num)
			{
				position3.x = (float)ScreenSize.Width * 0.5f - (float)num;
			}
			if (position3.y < (float)(-ScreenSize.Height) * 0.5f + (float)num)
			{
				position3.y = (float)(-ScreenSize.Height) * 0.5f + (float)num;
			}
			if (position3.y > (float)ScreenSize.Height * 0.5f - (float)num)
			{
				position3.y = (float)ScreenSize.Height * 0.5f - (float)num;
			}
			position3.z = iconFinderCastle.transform.position.z;
			iconFinderCastle.transform.position = position3;
			Quaternion quaternion = Quaternion.LookRotation(new Vector3(position3.x, 0f, position3.y));
			iconFinderCastle.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f - quaternion.eulerAngles.y));
		}
		iconFinderCastle.visible = visible;
	}

	private void UpdateFinderTrap()
	{
		bool visible = false;
		Vector3 position = transTrap.position;
		Vector3 position2 = gameCamera.WorldToScreenPoint(position);
		Vector3 position3 = uiCamera.ScreenToWorldPoint(position2);
		if (position3.x < (float)(-ScreenSize.Width) * 0.5f || position3.x > (float)ScreenSize.Width * 0.5f || position3.y < (float)(-ScreenSize.Height) * 0.5f || position3.y > (float)ScreenSize.Height * 0.5f || position2.z < 0f)
		{
			visible = true;
			int num = 16;
			if (position2.z < 0f)
			{
				position3.y = (float)(-ScreenSize.Height) * 0.5f + (float)num;
				position3.x = 0f - position3.x;
			}
			if (position3.x < (float)(-ScreenSize.Width) * 0.5f + (float)num)
			{
				position3.x = (float)(-ScreenSize.Width) * 0.5f + (float)num;
			}
			if (position3.x > (float)ScreenSize.Width * 0.5f - (float)num)
			{
				position3.x = (float)ScreenSize.Width * 0.5f - (float)num;
			}
			if (position3.y < (float)(-ScreenSize.Height) * 0.5f + (float)num)
			{
				position3.y = (float)(-ScreenSize.Height) * 0.5f + (float)num;
			}
			if (position3.y > (float)ScreenSize.Height * 0.5f - (float)num)
			{
				position3.y = (float)ScreenSize.Height * 0.5f - (float)num;
			}
			position3.z = iconFinderTrap.transform.position.z;
			iconFinderTrap.transform.position = position3;
			Quaternion quaternion = Quaternion.LookRotation(new Vector3(position3.x, 0f, position3.y));
			iconFinderTrap.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f - quaternion.eulerAngles.y));
			if (position3.x < -200f)
			{
				position3.x += 40f;
			}
			if (position3.x > 200f)
			{
				position3.x -= 40f;
			}
			if (position3.y < -200f)
			{
				position3.y += 40f;
			}
			if (position3.y > 200f)
			{
				position3.y -= 40f;
			}
			iconTrap.transform.position = position3;
		}
		iconFinderTrap.visible = visible;
		iconTrap.visible = visible;
	}

	public void ShowLevelUp()
	{
		StartCoroutine("AnimateLevelUp");
	}

	private IEnumerator AnimateLevelUp()
	{
		aniLevelUp.visible = true;
		Transform trans = aniLevelUp.transform;
		Material matr = aniLevelUp.materials[0];
		Vector3 pos = trans.localPosition;
		pos.y = 60f;
		trans.localPosition = pos;
		Vector3 scale = new Vector3(1f, 1f, 1f);
		Color color = new Color(1f, 1f, 1f, 0f);
		matr.SetColor("_Color", color);
		do
		{
			scale.x -= Time.deltaTime * 1.5f;
			if (scale.x < 0.5f)
			{
				scale.x = 0.5f;
			}
			scale.y = scale.x;
			color.a = 1f - (scale.x - 0.5f) * 2f;
			trans.localScale = scale;
			matr.SetColor("_Color", color);
			yield return 1;
		}
		while (scale.x != 0.5f);
		color.a = 1f;
		matr.SetColor("_Color", color);
		yield return new WaitForSeconds(0.7f);
		do
		{
			scale.x += Time.deltaTime * 1.5f;
			if (scale.x > 1f)
			{
				scale.x = 1f;
			}
			scale.y = scale.x;
			color.a = 1f - (scale.x - 0.5f) * 2f;
			pos.y += Time.deltaTime * 800f;
			trans.localScale = scale;
			trans.localPosition = pos;
			matr.SetColor("_Color", color);
			yield return 1;
		}
		while (scale.x != 1f);
		color.a = 1f;
		matr.SetColor("_Color", color);
		aniLevelUp.visible = false;
	}

	public void ShowFinish(StageManager.ClearMode clearMode)
	{
		switch (clearMode)
		{
		case StageManager.ClearMode.enemyClear:
			textFinish.text = StringContent.msgBattleEnemyCleared;
			break;
		case StageManager.ClearMode.dieHero:
			textFinish.text = StringContent.msgBattleHeroKilled;
			break;
		case StageManager.ClearMode.destroyGate:
			textFinish.text = StringContent.msgBattleDestroyedGate;
			break;
		case StageManager.ClearMode.timesUp:
			textFinish.text = StringContent.msgBattleTimesUp;
			break;
		}
		StartCoroutine("AnimateFinishText");
	}

	private IEnumerator AnimateFinishText()
	{
		panelFinish.SetActiveRecursive(true);
		float topY = -371f;
		float bottomY = -498f;
		Transform trans = panelFinish.transform;
		Vector3 pos = trans.localPosition;
		pos.y = bottomY;
		do
		{
			pos.y += Time.deltaTime * 300f;
			if (pos.y > topY)
			{
				pos.y = topY;
			}
			trans.localPosition = pos;
			yield return 1;
		}
		while (pos.y != topY);
		yield return new WaitForSeconds(2.5f);
		do
		{
			pos.y -= Time.deltaTime * 300f;
			if (pos.y < bottomY)
			{
				pos.y = bottomY;
			}
			trans.localPosition = pos;
			yield return 1;
		}
		while (pos.y != bottomY);
		panelFinish.SetActiveRecursive(false);
	}
}
