using UnityEngine;

public class WeaponManager : MonoBehaviour
{
	public enum HeroWeaponType
	{
		onehand = 0,
		doublehand = 1,
		bigsword = 2,
		max = 3
	}

	private const string pathWeapon = "Character/prefeb/weapons";

	public GameObject[] febSword;

	public GameObject[] febShield;

	private static int[] weaponCodeList = new int[27]
	{
		100, 101, 102, 103, 104, 105, 106, 107, 108, 110,
		111, 112, 113, 114, 115, 116, 117, 118, 120, 121,
		122, 123, 124, 125, 126, 127, 128
	};

	private static string fileSword = "feb_sword";

	private static string[] fileShield = new string[9] { "feb_shield100", "feb_shield101", "feb_shield102", "feb_shield103", "feb_shield104", "feb_shield105", "feb_shield106", "feb_shield107", "feb_shield108" };

	public void Init()
	{
		febSword = new GameObject[weaponCodeList.Length];
		for (int i = 0; i < weaponCodeList.Length; i++)
		{
			febSword[i] = ResourceManager.Load("Character/prefeb/weapons", fileSword + weaponCodeList[i], typeof(GameObject)) as GameObject;
		}
		febShield = new GameObject[fileShield.Length];
		for (int j = 0; j < fileShield.Length; j++)
		{
			febShield[j] = ResourceManager.Load("Character/prefeb/weapons", fileShield[j], typeof(GameObject)) as GameObject;
		}
	}

	public GameObject CreateWeapon(UnitCharactor.WeaponType weapon, int code)
	{
		GameObject result = null;
		int num = -1;
		for (int i = 0; i < weaponCodeList.Length; i++)
		{
			if (code == weaponCodeList[i])
			{
				num = i;
				break;
			}
		}
		if (num < 0)
		{
			return null;
		}
		switch (weapon)
		{
		case UnitCharactor.WeaponType.onehand:
			if (num >= febSword.Length)
			{
				return null;
			}
			result = Object.Instantiate(febSword[num]) as GameObject;
			break;
		case UnitCharactor.WeaponType.doublehand:
			if (num >= febSword.Length)
			{
				return null;
			}
			result = Object.Instantiate(febSword[num]) as GameObject;
			break;
		case UnitCharactor.WeaponType.bigsword:
			if (num >= febSword.Length)
			{
				return null;
			}
			result = Object.Instantiate(febSword[num]) as GameObject;
			break;
		}
		return result;
	}

	public GameObject CreateShield(int code)
	{
		GameObject gameObject = null;
		int num = -1;
		for (int i = 0; i < weaponCodeList.Length; i++)
		{
			if (code == weaponCodeList[i])
			{
				num = i;
				break;
			}
		}
		if (num < 0)
		{
			return null;
		}
		if (num >= febShield.Length)
		{
			return null;
		}
		return Object.Instantiate(febShield[num]) as GameObject;
	}
}
