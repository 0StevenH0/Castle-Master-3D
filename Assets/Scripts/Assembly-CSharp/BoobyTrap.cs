using System.Collections;
using System.IO;
using UnityEngine;

public class BoobyTrap : MonoBehaviour
{
	public enum TrapType
	{
		lightning = 0,
		tornado = 1,
		slowtime = 2,
		meteorite = 3,
		thornbush = 4,
		wrathgaia = 5,
		ghost = 6,
		max = 7
	}

	public class TrapAttr
	{
		public float iconAppearLength;

		public float iconWaitTime;

		public int iconHeroLevel;

		public float iconInterval;

		public float appearLength;

		public float duration;

		public float interval;

		public float fromCastle;

		public float attackLength;

		public bool knockback;

		public float attack_mul;

		public float attack_add;

		public float unitSlowRate;

		public float unitSlotTime;
	}

	private const string path = "Misc/prefeb";

	private const float tornadoContinueAttackDelay = 0.5f;

	public static TrapAttr[] trapAttr = new TrapAttr[7];

	private static float[] trapDamageTiming = new float[7] { 0.2f, 0.1f, 0.1f, 0.4f, 0.2f, 1.2f, 0f };

	private static float[] trapAnimationTime = new float[7] { 1.1f, 3f, 2f, 1.3f, 2f, 2f, 2f };

	private static string[] trapFab = new string[7] { "feb_thunderbolt", "feb_tornado", "feb_slowtime", "feb_meteor", "feb_creeper", "feb_gaiarage", "feb_ghost" };

	private static string[] tapIcon = new string[7] { "feb_icon_thunderbolt", "feb_icon_tornado", "feb_icon_slowtime", "feb_icon_meteor", "feb_icon_creeper", "feb_icon_gaiarage", "feb_icon_ghost" };

	public UIIngameView uiIngameView;

	private TrapType curTrapType;

	private GameObject trapPoint;

	private GameObject[] trapEffect;

	private Transform heroTrans;

	private Vector3 trapPos;

	private GameObject[] preTrapIcon = new GameObject[7];

	private GameObject[][] preTrapObject = new GameObject[7][];

	public void PreLordTrapObject(TrapType type)
	{
		TrapAttr trapAttr = BoobyTrap.trapAttr[(int)type];
		if (preTrapIcon[(int)type] != null)
		{
			return;
		}
		int num = 1;
		if (trapAttr.interval != 0f)
		{
			num = (int)(trapAttr.duration / trapAttr.interval);
			if (num < 1)
			{
				num = 1;
			}
		}
		if (type == TrapType.ghost)
		{
			num = 10;
		}
		preTrapObject[(int)type] = new GameObject[num];
		GameObject original = ResourceManager.Load("Misc/prefeb", trapFab[(int)type], typeof(GameObject)) as GameObject;
		for (int i = 0; i < num; i++)
		{
			preTrapObject[(int)type][i] = Object.Instantiate(original) as GameObject;
			preTrapObject[(int)type][i].transform.position = new Vector3(-200f, 0f, 0f);
		}
		preTrapIcon[(int)type] = Object.Instantiate(ResourceManager.Load("Misc/prefeb", tapIcon[(int)type], typeof(GameObject))) as GameObject;
		preTrapIcon[(int)type].AddComponent<AdjustAnimationSpeed>();
		preTrapIcon[(int)type].transform.position = new Vector3(-200f, 0f, 0f);
	}

	public void HideTrapObject()
	{
		for (int i = 0; i < 7; i++)
		{
			if (preTrapIcon[i] != null)
			{
				preTrapIcon[i].SetActiveRecursively(false);
			}
			if (preTrapObject[i] != null)
			{
				for (int j = 0; j < preTrapObject[i].Length; j++)
				{
					preTrapObject[i][j].SetActiveRecursively(false);
				}
			}
		}
	}

	public void Init(TrapType type, Transform heroTrans, Collider ground)
	{
		curTrapType = type;
		this.heroTrans = heroTrans;
		Vector3 position = heroTrans.position;
		if (trapPoint != null)
		{
			trapPoint.SetActiveRecursively(false);
		}
		trapPoint = preTrapIcon[(int)type];
		TrapAttr trapAttr = BoobyTrap.trapAttr[(int)curTrapType];
		float num = 2f;
		if (num >= trapAttr.iconAppearLength)
		{
			num = 0f;
		}
		float y = Random.Range(0, 360);
		float z = Random.Range(2f, trapAttr.iconAppearLength);
		Vector3 vector = new Vector3(0f, 0f, z);
		vector = Quaternion.Euler(new Vector3(0f, y, 0f)) * vector;
		float num2 = ground.bounds.min.x + 10f;
		float num3 = ground.bounds.max.x - 10f;
		float num4 = ground.bounds.min.z + 10f;
		float num5 = ground.bounds.max.z - 10f;
		vector.x = position.x + vector.x;
		vector.z = position.z + vector.z;
		if (vector.x < num2)
		{
			vector.x = num2;
		}
		if (vector.x > num3)
		{
			vector.x = num3;
		}
		if (vector.z < num4)
		{
			vector.z = num4;
		}
		if (vector.z > num5)
		{
			vector.z = num5;
		}
		trapPos = vector;
		trapPoint.transform.position = vector;
		trapPoint.SetActiveRecursively(true);
		uiIngameView.SetTrapTransform(trapPoint.transform, type);
		StartCoroutine("CheckForHeroPos");
	}

	private IEnumerator CheckForHeroPos()
	{
		float step = 0.2f;
		float pass = 0f;
		while (true)
		{
			float len = Vector3.Distance(trapPoint.transform.position, heroTrans.position);
			if (len < 1f)
			{
				trapPoint.SetActiveRecursively(false);
				ActiveBoobyTrap();
				break;
			}
			yield return new WaitForSeconds(step);
			pass += step;
			if (pass > trapAttr[(int)curTrapType].iconWaitTime)
			{
				trapPoint.SetActiveRecursively(false);
				break;
			}
		}
		uiIngameView.SetTrapTransform(null, TrapType.lightning);
	}

	private void ActiveBoobyTrap()
	{
		trapEffect = preTrapObject[(int)curTrapType];
		StartCoroutine("LaunchBoobyTrap");
	}

	private void PlayAllChild(GameObject obj)
	{
		if (obj.GetComponent<Animation>() != null)
		{
			obj.GetComponent<Animation>().Play();
		}
		UVAnimation component = obj.GetComponent<UVAnimation>();
		if (component != null)
		{
			component.Play();
		}
		int childCount = obj.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			PlayAllChild(obj.transform.GetChild(i).gameObject);
		}
	}

	private IEnumerator LaunchBoobyTrap()
	{
		TrapAttr attr = trapAttr[(int)curTrapType];
		float passTime = attr.duration;
		ExtSoundManager.Effect3D efSound = ExtSoundManager.Effect3D.efs_boobytrap_lightning;
		switch (curTrapType)
		{
		case TrapType.lightning:
			efSound = ExtSoundManager.Effect3D.efs_boobytrap_lightning;
			break;
		case TrapType.tornado:
			efSound = ExtSoundManager.Effect3D.efs_boobytrap_tornado;
			break;
		case TrapType.slowtime:
			efSound = ExtSoundManager.Effect3D.efs_boobytrap_slowtime;
			break;
		case TrapType.meteorite:
			efSound = ExtSoundManager.Effect3D.efs_boobytrap_meteorite;
			break;
		case TrapType.thornbush:
			efSound = ExtSoundManager.Effect3D.efs_boobytrap_thornbush;
			break;
		case TrapType.wrathgaia:
			efSound = ExtSoundManager.Effect3D.efs_boobytrap_gaia;
			break;
		case TrapType.ghost:
			efSound = ExtSoundManager.Effect3D.efs_boobytrap_ghost;
			break;
		}
		if (curTrapType == TrapType.ghost)
		{
			StartCoroutine("RunGhost");
			yield break;
		}
		int i = 0;
		GameObject[] array = trapEffect;
		foreach (GameObject obj in array)
		{
			Vector3 pos = trapPos;
			pos.x += Random.Range(0f - attr.appearLength, attr.appearLength);
			pos.z += Random.Range(0f - attr.appearLength, attr.appearLength);
			obj.transform.position = pos;
			obj.SetActiveRecursively(true);
			PlayAllChild(obj);
			StartCoroutine(HideDelay(obj, trapAnimationTime[(int)curTrapType]));
			PlayInfo.soundManager.Play(efSound, pos);
			StartCoroutine(ProcessDamage(pos));
			if (attr.interval == 0f || passTime <= 0f)
			{
				break;
			}
			yield return new WaitForSeconds(attr.interval);
			passTime -= attr.interval;
			i++;
		}
	}

	private IEnumerator HideDelay(GameObject obj, float delay)
	{
		yield return new WaitForSeconds(delay);
		obj.SetActiveRecursively(false);
	}

	private IEnumerator RunGhost()
	{
		TrapAttr attr = trapAttr[(int)curTrapType];
		float startX = -57f;
		float startZ = 50f;
		float endZ = -50f;
		float stepX = attr.fromCastle / (float)trapEffect.Length;
		float damageTime = 0.2f;
		Vector3 posBorn = new Vector3(startX, 0f, startZ);
		GameObject[] array = trapEffect;
		foreach (GameObject obj in array)
		{
			posBorn.x += stepX;
			posBorn.z = startZ + (float)Random.Range(0, 20);
			obj.transform.position = posBorn;
			obj.transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
			obj.SetActiveRecursively(true);
		}
		while (true)
		{
			float moveZ = Time.deltaTime * 15f;
			bool isDamage = false;
			damageTime -= Time.deltaTime;
			if (damageTime <= 0f)
			{
				damageTime = 0.2f;
				isDamage = true;
			}
			Vector3 pos = new Vector3(0f, 0f, 0f);
			GameObject[] array2 = trapEffect;
			foreach (GameObject obj2 in array2)
			{
				pos = obj2.transform.position;
				pos.z -= moveZ;
				obj2.transform.position = pos;
				if (isDamage)
				{
					StartCoroutine(ProcessDamage(pos));
				}
			}
			if (pos.z < endZ)
			{
				break;
			}
			yield return 1;
		}
		GameObject[] array3 = trapEffect;
		foreach (GameObject obj3 in array3)
		{
			obj3.SetActiveRecursively(false);
		}
	}

	private IEnumerator ProcessDamage(Vector3 pos)
	{
		TrapAttr attr = trapAttr[(int)curTrapType];
		float delay = trapDamageTiming[(int)curTrapType];
		float attack = (float)PlayInfo.heroState.sumAttack * attr.attack_mul + attr.attack_add;
		int count = 1;
		if (curTrapType == TrapType.tornado)
		{
			count = (int)(trapAnimationTime[(int)curTrapType] / 0.5f);
		}
		if (delay > 0f)
		{
			yield return new WaitForSeconds(delay);
		}
		for (int i = 0; i < count; i++)
		{
			CharactorManager charManager = GetComponent<CharactorManager>();
			foreach (UnitCharactor ch in charManager.charactors)
			{
				bool isKilled = false;
				if (ch.charType != UnitCharactor.CharactorType.monster)
				{
					continue;
				}
				float len = Vector3.Distance(ch.thisCtrl.thisTrans.position, pos);
				if (len < attr.attackLength)
				{
					if (curTrapType == TrapType.slowtime)
					{
						ch.thisCtrl.SlowMove(attr.unitSlowRate, attr.unitSlotTime);
					}
					ch.thisCtrl.Damage(null, pos, pos, attack, false, attr.knockback, false, out isKilled);
				}
			}
			if (i < count - 1 && curTrapType == TrapType.tornado)
			{
				yield return new WaitForSeconds(0.5f);
			}
		}
	}

	public static void LoadDefault()
	{
		TextAsset textAsset = ResourceManager.Load("GameData", "booby_trap", typeof(TextAsset)) as TextAsset;
		bool succeed = false;
		string s = DataSecurity.Decrypt(textAsset.text, "surkwjch", out succeed);
		if (!succeed)
		{
			Debug.LogError("Decrypt Error!!");
			return;
		}
		StringReader stringReader = new StringReader(s);
		int num = 0;
		string text;
		while ((text = stringReader.ReadLine()) != null)
		{
			if (text.Trim().Length != 0)
			{
				char[] separator = new char[1] { '\t' };
				string[] array = text.Split(separator);
				if (array.Length > 1)
				{
					TrapAttr trapAttr = new TrapAttr();
					trapAttr.iconAppearLength = float.Parse(array[1]);
					trapAttr.iconWaitTime = float.Parse(array[2]);
					trapAttr.iconHeroLevel = int.Parse(array[3]);
					trapAttr.iconInterval = float.Parse(array[4]);
					trapAttr.appearLength = float.Parse(array[5]);
					trapAttr.duration = float.Parse(array[6]);
					trapAttr.interval = float.Parse(array[7]);
					trapAttr.fromCastle = float.Parse(array[8]);
					trapAttr.attackLength = float.Parse(array[9]);
					trapAttr.knockback = int.Parse(array[10]) == 1;
					trapAttr.attack_mul = float.Parse(array[11]);
					trapAttr.attack_add = float.Parse(array[12]);
					trapAttr.unitSlowRate = float.Parse(array[13]);
					trapAttr.unitSlotTime = float.Parse(array[14]);
					BoobyTrap.trapAttr[num] = trapAttr;
					num++;
				}
			}
		}
	}
}
