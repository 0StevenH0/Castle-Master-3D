using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ItemManager
{
	public enum ItemType
	{
		weapon = 0,
		cloth = 1,
		misc = 2,
		max = 3
	}

	public enum MiscType
	{
		ring = 0,
		consumable = 1,
		max = 2
	}

	public const int codeHpSmall = 400;

	public const int codeMpSmall = 410;

	public const int codeHpLarge = 401;

	public const int codeMpLarge = 411;

	public UnitItem[] listAll;

	public UnitItem[] listWeapon;

	public UnitItem[] listCloth;

	public UnitItem[] listMisc;

	public void LoadDefault()
	{
		TextAsset textAsset = ResourceManager.Load("GameData", "item_list", typeof(TextAsset)) as TextAsset;
		bool succeed = false;
		string s = DataSecurity.Decrypt(textAsset.text, "surkwjch", out succeed);
		if (!succeed)
		{
			Debug.LogError("Decrypt Error!!");
			return;
		}
		StringReader stringReader = new StringReader(s);
		List<UnitItem> list = new List<UnitItem>();
		List<UnitItem> list2 = new List<UnitItem>();
		List<UnitItem> list3 = new List<UnitItem>();
		List<UnitItem> list4 = new List<UnitItem>();
		string text;
		while ((text = stringReader.ReadLine()) != null)
		{
			if (text.Trim().Length == 0)
			{
				continue;
			}
			char[] separator = new char[1] { '\t' };
			string[] array = text.Split(separator);
			if (array.Length > 1)
			{
				UnitItem unitItem = new UnitItem();
				if (array[0].Equals("weapon"))
				{
					unitItem.type = ItemType.weapon;
				}
				else if (array[0].Equals("cloth"))
				{
					unitItem.type = ItemType.cloth;
				}
				else if (array[0].Equals("misc"))
				{
					unitItem.type = ItemType.misc;
				}
				unitItem.name = array[1];
				unitItem.code = int.Parse(array[3]);
				if (array[2].Equals("onehand"))
				{
					unitItem.weaponType = UnitCharactor.WeaponType.onehand;
				}
				else if (array[2].Equals("doublehand"))
				{
					unitItem.weaponType = UnitCharactor.WeaponType.doublehand;
				}
				else if (array[2].Equals("bigsword"))
				{
					unitItem.weaponType = UnitCharactor.WeaponType.bigsword;
				}
				if (array[2].Equals("head"))
				{
					unitItem.clothPart = HeroModel.ClothPart.head;
				}
				else if (array[2].Equals("top"))
				{
					unitItem.clothPart = HeroModel.ClothPart.top;
				}
				else if (array[2].Equals("bottom"))
				{
					unitItem.clothPart = HeroModel.ClothPart.bottom;
				}
				if (array[2].Equals("ring"))
				{
					unitItem.miscType = MiscType.ring;
				}
				else if (array[2].Equals("consumable"))
				{
					unitItem.miscType = MiscType.consumable;
				}
				unitItem.requireLevel = int.Parse(array[4]);
				unitItem.costGold = int.Parse(array[5]);
				unitItem.costGem = int.Parse(array[6]);
				unitItem.sellGold = int.Parse(array[7]);
				unitItem.ability = new Ability();
				unitItem.ability.attack = float.Parse(array[8]);
				unitItem.ability.defense = float.Parse(array[9]);
				unitItem.ability.strength = float.Parse(array[10]);
				unitItem.ability.intellectual = float.Parse(array[11]);
				unitItem.ability.constitution = float.Parse(array[12]);
				unitItem.ability.critical = float.Parse(array[13]);
				unitItem.ability.speed = float.Parse(array[14]);
				unitItem.ability.colltime = float.Parse(array[15]);
				unitItem.ability.hp = float.Parse(array[16]);
				unitItem.ability.mp = float.Parse(array[17]);
				list.Add(unitItem);
				switch (unitItem.type)
				{
				case ItemType.weapon:
					list2.Add(unitItem);
					break;
				case ItemType.cloth:
					list3.Add(unitItem);
					break;
				case ItemType.misc:
					list4.Add(unitItem);
					break;
				}
			}
		}
		listAll = list.ToArray();
		listWeapon = list2.ToArray();
		listCloth = list3.ToArray();
		listMisc = list4.ToArray();
	}

	public UnitItem FindItem(ItemType type, int code)
	{
		UnitItem[] array = null;
		if (type == ItemType.weapon)
		{
			array = listWeapon;
		}
		if (type == ItemType.cloth)
		{
			array = listCloth;
		}
		if (type == ItemType.misc)
		{
			array = listMisc;
		}
		UnitItem[] array2 = array;
		foreach (UnitItem unitItem in array2)
		{
			if (code == unitItem.code)
			{
				return unitItem;
			}
		}
		return null;
	}

	public UnitItem FindItem(int code)
	{
		UnitItem[] array = listAll;
		foreach (UnitItem unitItem in array)
		{
			if (code == unitItem.code)
			{
				return unitItem;
			}
		}
		return null;
	}

	public int FindItemIndex(ItemType type, int code)
	{
		UnitItem[] array = null;
		if (type == ItemType.weapon)
		{
			array = listWeapon;
		}
		if (type == ItemType.cloth)
		{
			array = listCloth;
		}
		if (type == ItemType.misc)
		{
			array = listMisc;
		}
		int num = 0;
		UnitItem[] array2 = array;
		foreach (UnitItem unitItem in array2)
		{
			if (code == unitItem.code)
			{
				return num;
			}
			num++;
		}
		return 0;
	}
}
