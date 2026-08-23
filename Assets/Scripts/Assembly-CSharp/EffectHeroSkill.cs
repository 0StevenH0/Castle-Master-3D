using System.Collections;
using UnityEngine;

public class EffectHeroSkill : MonoBehaviour
{
	public enum ExtraEffectType
	{
		potionHp = 0,
		levelUp = 1
	}

	private const string pathEffect = "Character/prefeb/effect";

	private static string[] fileEffect = new string[15]
	{
		"feb_onehandskill01_effect", "feb_onehandskill02_effect", "feb_onehandskill03_effect", "feb_onehandskill04_effect", "feb_onehandskill05_effect", "feb_doublehandskill01_effect", "feb_doublehandskill02_effect", "feb_doublehandskill03_effect", "feb_doublehandskill04_effect", "feb_doublehandskill05_effect",
		"feb_bigswordskill01_effect", "feb_bigswordskill02_effect", "feb_bigswordskill03_effect", "feb_bigswordskill04_effect", "feb_bigswordskill05_effect"
	};

	private static string[] fileExtra = new string[2] { "feb_hp_potion_effect", "feb_levelup_effect" };

	private GameObject[] effObj;

	private GameObject[] effExtra;

	public void Init(Transform transParent)
	{
		effObj = new GameObject[fileEffect.Length];
		int num = 0;
		string[] array = fileEffect;
		foreach (string filename in array)
		{
			effObj[num] = Object.Instantiate(ResourceManager.Load("Character/prefeb/effect", filename, typeof(GameObject))) as GameObject;
			effObj[num].AddComponent<AdjustAnimationSpeed>();
			effObj[num].transform.parent = transParent;
			effObj[num].SetActiveRecursively(false);
			num++;
		}
		effExtra = new GameObject[fileExtra.Length];
		num = 0;
		string[] array2 = fileExtra;
		foreach (string filename2 in array2)
		{
			effExtra[num] = Object.Instantiate(ResourceManager.Load("Character/prefeb/effect", filename2, typeof(GameObject))) as GameObject;
			effExtra[num].AddComponent<AdjustAnimationSpeed>();
			effExtra[num].transform.parent = transParent;
			effExtra[num].SetActiveRecursively(false);
			num++;
		}
	}

	public void Play(int index)
	{
		float num = 0.5f;
		if (effObj[index].GetComponent<Animation>() != null)
		{
			effObj[index].SetActiveRecursively(true);
			effObj[index].GetComponent<Animation>().Play();
			num = effObj[index].GetComponent<Animation>().clip.length * 4f;
			StartCoroutine(Stop(index, num));
		}
	}

	private IEnumerator Stop(int index, float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		effObj[index].SetActiveRecursively(false);
	}

	public void PlayExtraEffect(ExtraEffectType effType)
	{
		float num = 0.5f;
		if (effExtra[(int)effType].GetComponent<Animation>() != null)
		{
			effExtra[(int)effType].SetActiveRecursively(true);
			effExtra[(int)effType].GetComponent<Animation>().Play();
			num = effExtra[(int)effType].GetComponent<Animation>().clip.length * 4f;
			StartCoroutine(StopExtraEffect((int)effType, num));
		}
	}

	private IEnumerator StopExtraEffect(int index, float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		effExtra[index].SetActiveRecursively(false);
	}

	public void StopAll()
	{
		GameObject[] array = effObj;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActiveRecursively(false);
		}
		GameObject[] array2 = effExtra;
		foreach (GameObject gameObject2 in array2)
		{
			gameObject2.SetActiveRecursively(false);
		}
	}
}
