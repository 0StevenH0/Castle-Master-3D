using System.Collections.Generic;
using UnityEngine;

public class CharactorManager : MonoBehaviour
{
	public enum NpcType
	{
		weapon = 0,
		defense = 1,
		item = 2,
		guard = 3,
		captain = 4,
		skill = 5,
		priest = 6,
		secretary = 7,
		daughter = 8,
		max = 9
	}

	public const string pathChar = "Character/prefeb/character";

	public const string fileHero = "feb_hero01";

	private const string pathEffect = "Character/prefeb/effect";

	public GameObject febHero;

	public GameObject[] febMonster;

	public GameObject[] febNpc;

	public GameObject[] febSoldier;

	public GameObject[] febLord;

	public GameObject[] febMonsterGate;

	public GameObject[] febSoldierGate;

	public GameObject febShadow;

	public List<UnitCharactor> charactors = new List<UnitCharactor>();

	public List<Collider> colliders = new List<Collider>();

	public static int[] maxAnimationHero = new int[13]
	{
		1, 2, 1, 1, 1, 1, 2, 7, 0, 0,
		2, 2, 3
	};

	public static int[] maxAnimationNpc = new int[13]
	{
		1, 1, 0, 0, 1, 1, 0, 0, 0, 0,
		0, 0, 0
	};

	public static int[] maxAnimationMonster = new int[13]
	{
		1, 1, 1, 0, 1, 1, 2, 0, 2, 1,
		1, 1, 0
	};

	public static int[] maxAnimationSoldier = new int[13]
	{
		1, 1, 1, 0, 1, 1, 2, 0, 2, 1,
		2, 1, 2
	};

	public static int[] maxAnimationLord = new int[13]
	{
		1, 1, 1, 0, 1, 1, 2, 0, 2, 1,
		2, 1, 2
	};

	public static int[] maxAnimationCastleGate = new int[13]
	{
		1, 1, 0, 0, 0, 0, 0, 0, 0, 1,
		1, 0, 0
	};

	public static int[] monsterIndexCode = new int[17]
	{
		201, 202, 203, 204, 205, 206, 207, 208, 209, 210,
		211, 212, 213, 214, 215, 216, 217
	};

	public static int[] soldierIndexCode = new int[5] { 101, 102, 103, 104, 105 };

	private static string[] fileMonster = new string[17]
	{
		"feb_monster201", "feb_monster202", "feb_monster203", "feb_monster204", "feb_monster205", "feb_monster206", "feb_monster207", "feb_monster208", "feb_monster209", "feb_monster210",
		"feb_monster211", "feb_monster212", "feb_monster213", "feb_monster214", "feb_monster215", "feb_monster216", "feb_monster217"
	};

	private static string[] fileNpc = new string[9] { "feb_weaponmerchant", "feb_defensemerchant", "feb_itemmerchant", "feb_guard", "feb_captain", "feb_skillmaster", "feb_chamberlain", "feb_guide", "feb_steward" };

	private static string[] fileSoldier = new string[5] { "feb_soldier101", "feb_soldier102", "feb_soldier103", "feb_soldier104", "feb_soldier105" };

	private static string[] fileLord = new string[3] { "feb_strategist", "feb_swordsman", "feb_tanker" };

	private static string[] fileMonsterGate = new string[3] { "feb_castlegate01_01", "feb_castlegate01_02", "feb_castlegate01_03" };

	private static string[] fileSoldierGate = new string[3] { "feb_castlegate00_01", "feb_castlegate00_02", "feb_castlegate00_03" };

	private static string fileShadow = "feb_shadow";

	public void Init()
	{
		febHero = ResourceManager.Load("Character/prefeb/character", "feb_hero01", typeof(GameObject)) as GameObject;
		febMonster = new GameObject[fileMonster.Length];
		for (int i = 0; i < fileMonster.Length; i++)
		{
			febMonster[i] = ResourceManager.Load("Character/prefeb/character", fileMonster[i], typeof(GameObject)) as GameObject;
		}
		febNpc = new GameObject[fileNpc.Length];
		for (int j = 0; j < fileNpc.Length; j++)
		{
			febNpc[j] = ResourceManager.Load("Character/prefeb/character", fileNpc[j], typeof(GameObject)) as GameObject;
		}
		febSoldier = new GameObject[fileSoldier.Length];
		for (int k = 0; k < fileSoldier.Length; k++)
		{
			febSoldier[k] = ResourceManager.Load("Character/prefeb/character", fileSoldier[k], typeof(GameObject)) as GameObject;
		}
		febLord = new GameObject[fileLord.Length];
		for (int l = 0; l < fileLord.Length; l++)
		{
			febLord[l] = ResourceManager.Load("Character/prefeb/character", fileLord[l], typeof(GameObject)) as GameObject;
		}
		febMonsterGate = new GameObject[fileMonsterGate.Length];
		for (int m = 0; m < fileMonsterGate.Length; m++)
		{
			febMonsterGate[m] = ResourceManager.Load("Character/prefeb/character", fileMonsterGate[m], typeof(GameObject)) as GameObject;
		}
		febSoldierGate = new GameObject[fileSoldierGate.Length];
		for (int n = 0; n < fileSoldierGate.Length; n++)
		{
			febSoldierGate[n] = ResourceManager.Load("Character/prefeb/character", fileSoldierGate[n], typeof(GameObject)) as GameObject;
		}
		febShadow = ResourceManager.Load("Character/prefeb/effect", fileShadow, typeof(GameObject)) as GameObject;
	}

	public UnitCharactor AddUnitFromCode(Vector3 pos, UnitCharactor.CharactorType type, int code)
	{
		switch (type)
		{
		case UnitCharactor.CharactorType.monster:
		{
			for (int j = 0; j < monsterIndexCode.Length; j++)
			{
				if (code == monsterIndexCode[j])
				{
					return AddUnit(pos, type, j);
				}
			}
			break;
		}
		case UnitCharactor.CharactorType.soldier:
		{
			for (int i = 0; i < soldierIndexCode.Length; i++)
			{
				if (code == soldierIndexCode[i])
				{
					return AddUnit(pos, type, i);
				}
			}
			break;
		}
		}
		return null;
	}

	private UnitCharactor AddUnit(Vector3 pos, UnitCharactor.CharactorType type, int idx)
	{
		return AddUnit(pos, type, false, idx);
	}

	private UnitCharactor AddUnit(Vector3 pos, UnitCharactor.CharactorType type, bool isGate, int idx)
	{
		GameObject gameObject = null;
		switch (type)
		{
		case UnitCharactor.CharactorType.hero:
			gameObject = Object.Instantiate(febHero) as GameObject;
			break;
		case UnitCharactor.CharactorType.monster:
			if (isGate)
			{
				if (idx >= febMonsterGate.Length)
				{
					return null;
				}
				gameObject = Object.Instantiate(febMonsterGate[idx]) as GameObject;
			}
			else
			{
				if (idx >= febMonster.Length)
				{
					return null;
				}
				gameObject = Object.Instantiate(febMonster[idx]) as GameObject;
			}
			break;
		case UnitCharactor.CharactorType.npc:
			if (idx >= febNpc.Length)
			{
				return null;
			}
			gameObject = Object.Instantiate(febNpc[idx]) as GameObject;
			break;
		case UnitCharactor.CharactorType.soldier:
			if (isGate)
			{
				if (idx >= febSoldierGate.Length)
				{
					return null;
				}
				gameObject = Object.Instantiate(febSoldierGate[idx]) as GameObject;
			}
			else
			{
				if (idx >= febSoldier.Length)
				{
					return null;
				}
				gameObject = Object.Instantiate(febSoldier[idx]) as GameObject;
			}
			break;
		case UnitCharactor.CharactorType.lord:
			if (idx >= febLord.Length)
			{
				return null;
			}
			gameObject = Object.Instantiate(febLord[idx]) as GameObject;
			break;
		}
		if (!isGate)
		{
			gameObject.AddComponent<AdjustAnimationSpeed>();
		}
		pos.y = 0f;
		gameObject.transform.position = pos;
		Collider collider = gameObject.GetComponent<BoxCollider>();
		if (collider != null)
		{
			bool flag = false;
			if (UserSetting.quality == UserSetting.GraphicsQuality.fast && !isGate)
			{
				float num = ((!(collider.bounds.size.x < collider.bounds.size.z)) ? collider.bounds.size.z : collider.bounds.size.x);
				float num2 = Mathf.Sqrt(Mathf.Pow(num * 0.5f, 2f) + Mathf.Pow(num * 0.5f, 2f));
				float num3 = collider.bounds.size.y;
				if (num3 * 0.5f < num2)
				{
					num3 = num2 * 2f;
				}
				CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
				capsuleCollider.radius = num2;
				capsuleCollider.height = num3;
				capsuleCollider.center = new Vector3(0f, num3 * 0.5f - 0.05f, 0f);
				Object.Destroy(collider);
				collider = capsuleCollider;
				flag = true;
			}
			if (!flag)
			{
				BoxCollider boxCollider = collider as BoxCollider;
				if (boxCollider.center.y < boxCollider.size.y * 0.5f)
				{
					Vector3 center = boxCollider.center;
					center.y = boxCollider.size.y * 0.5f + 0.01f;
					boxCollider.center = center;
				}
			}
		}
		Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
		if (isGate)
		{
			rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		}
		else
		{
			rigidbody.constraints = (RigidbodyConstraints)116;
		}
		rigidbody.linearDamping = 10f;
		rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
		UnitCharactor unitCharactor = gameObject.AddComponent<UnitCharactor>();
		unitCharactor.charactorManager = this;
		unitCharactor.charType = type;
		unitCharactor.charIdx = idx;
		unitCharactor.isGate = isGate;
		if (isGate)
		{
			unitCharactor.maxAnimation = maxAnimationCastleGate;
		}
		else
		{
			switch (type)
			{
			case UnitCharactor.CharactorType.hero:
				unitCharactor.maxAnimation = maxAnimationHero;
				gameObject.AddComponent<HeroModel>();
				unitCharactor.weaponManager = GetComponent<WeaponManager>();
				break;
			case UnitCharactor.CharactorType.monster:
				unitCharactor.maxAnimation = maxAnimationMonster;
				break;
			case UnitCharactor.CharactorType.npc:
				unitCharactor.maxAnimation = maxAnimationNpc;
				rigidbody.constraints = RigidbodyConstraints.FreezeAll;
				break;
			case UnitCharactor.CharactorType.soldier:
				unitCharactor.maxAnimation = maxAnimationSoldier;
				break;
			case UnitCharactor.CharactorType.lord:
				unitCharactor.maxAnimation = maxAnimationLord;
				break;
			}
			if (UserSetting.quality != 0)
			{
				GameObject gameObject2 = Object.Instantiate(febShadow) as GameObject;
				gameObject2.transform.parent = gameObject.transform;
				gameObject2.transform.localPosition = new Vector3(0f, 0.02f, 0f);
			}
		}
		colliders.Add(collider);
		charactors.Add(unitCharactor);
		UnitControl unitControl = gameObject.AddComponent<UnitControl>();
		unitControl.thisChar = unitCharactor;
		unitCharactor.thisCtrl = unitControl;
		unitControl.Init(null);
		if (!isGate)
		{
			switch (type)
			{
			case UnitCharactor.CharactorType.hero:
			{
				unitControl.SetWeapon(UnitCharactor.WeaponType.onehand, 100);
				EffectHeroSkill effectHeroSkill = gameObject.AddComponent<EffectHeroSkill>();
				effectHeroSkill.Init(gameObject.transform);
				break;
			}
			case UnitCharactor.CharactorType.monster:
				unitControl.SetWeapon(UnitCharactor.WeaponType.fixedunit, 0);
				break;
			case UnitCharactor.CharactorType.npc:
				unitControl.SetWeapon(UnitCharactor.WeaponType.none, 0);
				break;
			case UnitCharactor.CharactorType.soldier:
				unitControl.SetWeapon(UnitCharactor.WeaponType.fixedunit, 0);
				break;
			case UnitCharactor.CharactorType.lord:
				unitControl.SetWeapon(UnitCharactor.WeaponType.fixedunit, 0);
				break;
			}
			LayerManager.SetLayerAllChild(gameObject.transform, LayerManager.layerCharactor);
		}
		if (isGate)
		{
			PlayInfo.soundManager.Play(ExtSoundManager.Effect2D.efs_gate_collapse, new Vector3(0f, 0f, 0f));
		}
		return unitCharactor;
	}

	public UnitCharactor AddHero(Vector3 pos)
	{
		return AddUnit(pos, UnitCharactor.CharactorType.hero, 0);
	}

	public UnitCharactor AddMonster(Vector3 pos, int idx)
	{
		return AddUnit(pos, UnitCharactor.CharactorType.monster, idx);
	}

	public UnitCharactor AddNpc(Vector3 pos, int idx)
	{
		return AddUnit(pos, UnitCharactor.CharactorType.npc, idx);
	}

	public UnitCharactor AddSoldier(Vector3 pos, int idx)
	{
		return AddUnit(pos, UnitCharactor.CharactorType.soldier, idx);
	}

	public UnitCharactor AddLord(Vector3 pos, int idx)
	{
		return AddUnit(pos, UnitCharactor.CharactorType.lord, idx);
	}

	public UnitCharactor AddCastleGate(Vector3 pos, UnitCharactor.CharactorType type, int idx)
	{
		return AddUnit(pos, type, true, idx);
	}

	public void Remove(UnitCharactor obj)
	{
		colliders.Remove(obj.GetComponent<Collider>());
		charactors.Remove(obj);
		Object.Destroy(obj);
	}

	public void Remove(GameObject obj)
	{
		UnitCharactor component = obj.GetComponent<UnitCharactor>();
		Remove(component);
	}

	public UnitCharactor[] GetSortedUnit(Vector3 pos, UnitCharactor.CharactorType charType)
	{
		List<UnitCharactor> list = new List<UnitCharactor>();
		List<float> list2 = new List<float>();
		foreach (UnitCharactor charactor in charactors)
		{
			if (charactor.charType != charType)
			{
				continue;
			}
			float num = Vector3.Distance(pos, charactor.transform.position);
			int num2 = 0;
			bool flag = false;
			foreach (float item in list2)
			{
				float num3 = item;
				if (num < num3)
				{
					flag = true;
					break;
				}
				num2++;
			}
			if (flag)
			{
				list.Insert(num2, charactor);
				list2.Insert(num2, num);
			}
			else
			{
				list.Add(charactor);
				list2.Add(num);
			}
		}
		return list.ToArray();
	}
}
