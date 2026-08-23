using System;
using UnityEngine;

public class HeroModel : MonoBehaviour
{
	public enum ClothPart
	{
		head = 0,
		top = 1,
		bottom = 2,
		max = 3
	}

	private static int[] maxClothPart = new int[3] { 9, 9, 9 };

	private static int[][] clothCodeList = new int[3][]
	{
		new int[9] { 200, 201, 202, 203, 204, 205, 206, 207, 208 },
		new int[9] { 210, 211, 212, 213, 214, 215, 216, 217, 218 },
		new int[9] { 220, 221, 222, 223, 224, 225, 226, 227, 228 }
	};

	public GameObject weaponLeft;

	public GameObject weaponRight;

	public SwardBand bandLeft;

	public SwardBand bandRight;

	private int[] clothPart = new int[3];

	public int headIndex
	{
		get
		{
			return clothPart[0];
		}
	}

	public int topIndex
	{
		get
		{
			return clothPart[1];
		}
	}

	public int bottomIndex
	{
		get
		{
			return clothPart[2];
		}
	}

	public void SetClothPartFromCode(ClothPart part, int code)
	{
		for (int i = 0; i < clothCodeList[(int)part].Length; i++)
		{
			if (clothCodeList[(int)part][i] == code)
			{
				SetClothPart(part, i);
				break;
			}
		}
	}

	public void SetClothPart(ClothPart part, int idx)
	{
		int num = maxClothPart[(int)part];
		clothPart[(int)part] = idx;
		for (int i = 0; i < num; i++)
		{
			string text = "msh_" + part.ToString() + string.Format("{0:00}", i + 1);
			Transform transform = base.transform.Find(text);
			if (transform != null)
			{
				transform.gameObject.SetActive(idx == i);
			}
		}
	}

	public void SetCloth(int idxHead, int idxTop, int idxBottom)
	{
		SetClothPart(ClothPart.head, idxHead);
		SetClothPart(ClothPart.top, idxTop);
		SetClothPart(ClothPart.bottom, idxBottom);
	}

	public void RefreshCloth()
	{
		SetClothPart(ClothPart.head, clothPart[0]);
		SetClothPart(ClothPart.top, clothPart[1]);
		SetClothPart(ClothPart.bottom, clothPart[2]);
	}

	public void SetWeapon(WeaponManager manager, UnitCharactor.WeaponType weapon, int code)
	{
		if (weaponLeft != null)
		{
			UnityEngine.Object.Destroy(weaponLeft);
			weaponLeft = null;
		}
		if (weaponRight != null)
		{
			UnityEngine.Object.Destroy(weaponRight);
			weaponRight = null;
		}
		if (bandLeft != null)
		{
			UnityEngine.Object.Destroy(bandLeft.swardTrail);
		}
		if (bandRight != null)
		{
			UnityEngine.Object.Destroy(bandRight.swardTrail);
		}
		bandLeft = null;
		bandRight = null;
		GameObject gameObject = manager.CreateWeapon(weapon, code);
		if (gameObject != null)
		{
			gameObject.AddComponent<SwardBand>();
		}
		if (gameObject != null)
		{
			Vector3 eulerAngles = gameObject.transform.localRotation.eulerAngles;
			switch (weapon)
			{
			case UnitCharactor.WeaponType.onehand:
			{
				GameObject gameObject2 = manager.CreateShield(code);
				if (gameObject2 != null)
				{
					weaponLeft = gameObject2;
					Transform transform4 = FindChildAll(base.transform, "Dummy_shield");
					if (transform4 != null)
					{
						gameObject2.transform.parent = transform4;
					}
				}
				Transform transform5 = FindChildAll(base.transform, "dummy_r_hand01");
				if (transform5 != null)
				{
					gameObject.transform.parent = transform5;
				}
				weaponRight = gameObject;
				break;
			}
			case UnitCharactor.WeaponType.bigsword:
			{
				Transform transform3 = FindChildAll(base.transform, "Dummy_l_dummy01");
				if (transform3 != null)
				{
					gameObject.transform.parent = transform3;
				}
				weaponLeft = gameObject;
				break;
			}
			case UnitCharactor.WeaponType.doublehand:
			{
				weaponLeft = gameObject;
				weaponRight = UnityEngine.Object.Instantiate(gameObject) as GameObject;
				Transform transform = FindChildAll(base.transform, "dummy_r_hand01");
				if (transform != null)
				{
					weaponRight.transform.parent = transform;
				}
				Transform transform2 = FindChildAll(base.transform, "Dummy_l_dummy01");
				if (transform2 != null)
				{
					weaponLeft.transform.parent = transform2;
				}
				break;
			}
			}
			if (weaponLeft != null)
			{
				weaponLeft.transform.localPosition = new Vector3(0f, 0f, 0f);
				weaponLeft.transform.localRotation = Quaternion.Euler(eulerAngles);
				if (weapon == UnitCharactor.WeaponType.doublehand)
				{
					Vector3 euler = eulerAngles;
					euler.y += 180f;
					euler.z += 180f;
					weaponLeft.transform.localRotation = Quaternion.Euler(euler);
				}
				bandLeft = weaponLeft.GetComponent<SwardBand>();
			}
			if (weaponRight != null)
			{
				weaponRight.transform.localPosition = new Vector3(0f, 0f, 0f);
				weaponRight.transform.localRotation = Quaternion.Euler(eulerAngles);
				if (weapon == UnitCharactor.WeaponType.doublehand)
				{
					Vector3 euler2 = eulerAngles;
					euler2.x += 180f;
					euler2.y += 180f;
					weaponRight.transform.localRotation = Quaternion.Euler(euler2);
				}
				bandRight = weaponRight.GetComponent<SwardBand>();
			}
		}
		GC.Collect();
	}

	private Transform FindChildAll(Transform parent, string findName)
	{
		Transform[] componentsInChildren = parent.GetComponentsInChildren<Transform>();
		Transform result = null;
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			if (transform.name.Equals(findName))
			{
				result = transform;
				break;
			}
		}
		return result;
	}
}
