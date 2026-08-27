using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CastleTrap : MonoBehaviour
{
	public class TrapAttr
	{
		public int heroLevel;

		public float interval;

		public float attack_mul;

		public float attack_add;
	}

	private const string path = "Misc/prefeb";

	private const string trapFab = "feb_enermytrap";

	private const float attackRange = 5.5f;

	private const float attackTiming = 0.1f;

	public static List<TrapAttr> trapAttr = new List<TrapAttr>();

	private CharactorManager charManager;

	private StageManager stageManager;

	private GameObject trapEffect;

	private TrapAttr curAttr;

	private Vector3 centerPos = new Vector3(0f, 0f, 0f);

	public void Init(int castleLevel, Vector3 pos, CharactorManager charMan, StageManager stage)
	{
		curAttr = trapAttr[0];
		for (int num = trapAttr.Count - 1; num >= 0; num--)
		{
			if (PlayInfo.heroState.level >= trapAttr[num].heroLevel)
			{
				if (num < trapAttr.Count - 1)
				{
					curAttr = trapAttr[num + 1];
				}
				else
				{
					curAttr = trapAttr[num];
				}
				break;
			}
		}
		charManager = charMan;
		stageManager = stage;
		centerPos = pos;
		trapEffect = Object.Instantiate(ResourceManager.Load("Misc/prefeb", "feb_enermytrap", typeof(GameObject))) as GameObject;
		trapEffect.transform.position = centerPos;
		trapEffect.transform.localRotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
		trapEffect.SetActiveRecursive(false);
		StartCoroutine("CheckForUnitPos");
	}

	private IEnumerator CheckForUnitPos()
	{
		while (true)
		{
			yield return new WaitForSeconds(curAttr.interval);
			if (stageManager.GetAliveUnit(1) <= 0)
			{
				break;
			}
			foreach (UnitCharactor unit in charManager.charactors)
			{
				if (unit.thisCtrl.battleSide == 0 && unit.isAwake && !unit.thisCtrl.isDie)
				{
					float len = Vector3.Distance(centerPos, unit.thisCtrl.thisTrans.position);
					if (len <= 5.5f)
					{
						StartCoroutine("LaunchCastleTrap");
						break;
					}
				}
			}
		}
	}

	private IEnumerator LaunchCastleTrap()
	{
		trapEffect.SetActiveRecursive(true);
		Animation[] componentsInChildren = trapEffect.GetComponentsInChildren<Animation>();
		foreach (Animation ani in componentsInChildren)
		{
			ani.Play();
		}
		PlayInfo.soundManager.Play(ExtSoundManager.Effect3D.efs_boobytrap_enermy, trapEffect.transform.position);
		yield return new WaitForSeconds(0.1f);
		float attack = (float)PlayInfo.heroState.sumAttack * curAttr.attack_mul + curAttr.attack_add;
		foreach (UnitCharactor unit in charManager.charactors)
		{
			if (unit.thisCtrl.battleSide == 0 && unit.isAwake && !unit.thisCtrl.isDie)
			{
				float len = Vector3.Distance(centerPos, unit.thisCtrl.thisTrans.position);
				if (len <= 5.5f)
				{
					bool isKilled = false;
					unit.thisCtrl.Damage(null, centerPos, centerPos, attack, false, true, false, out isKilled);
				}
			}
		}
		yield return new WaitForSeconds(0.5f);
		trapEffect.SetActiveRecursive(false);
	}

	[System.Serializable]
	private class TrapAttrRow
	{
		public int heroLevel;
		public float interval;
		public float attack_mul;
		public float attack_add;
	}

	[System.Serializable]
	private class TrapAttrRowList
	{
		public TrapAttrRow[] items;
	}

	public static void LoadDefault()
	{
		TextAsset textAsset = ResourceManager.Load("GameData", "monster_trap", typeof(TextAsset)) as TextAsset;
		string json = "{\"items\":" + textAsset.text + "}";
		TrapAttrRowList trapAttrRowList = JsonUtility.FromJson<TrapAttrRowList>(json);
		CastleTrap.trapAttr.Clear();
		TrapAttrRow[] items = trapAttrRowList.items;
		foreach (TrapAttrRow row in items)
		{
			TrapAttr trapAttr = new TrapAttr();
			trapAttr.heroLevel = row.heroLevel;
			trapAttr.interval = row.interval;
			trapAttr.attack_mul = row.attack_mul;
			trapAttr.attack_add = row.attack_add;
			CastleTrap.trapAttr.Add(trapAttr);
		}
	}
}
