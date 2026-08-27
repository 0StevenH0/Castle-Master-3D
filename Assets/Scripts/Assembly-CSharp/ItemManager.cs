using System.Collections.Generic;
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

	[System.Serializable]
	private class ItemRow
	{
		public string type;

		public string name;

		public string subtype;

		public int code;

		public int requireLevel;

		public int costGold;

		public int costGem;

		public int sellGold;

		public float attack;

		public float defense;

		public float strength;

		public float intellectual;

		public float constitution;

		public float critical;

		public float speed;

		public float colltime;

		public float hp;

		public float mp;
	}

	[System.Serializable]
	private class ItemRowList
	{
		public ItemRow[] items;
	}

	public void LoadDefault()
	{
		TextAsset textAsset = ResourceManager.Load("GameData", "item_list", typeof(TextAsset)) as TextAsset;
		if (textAsset == null || string.IsNullOrEmpty(textAsset.text))
		{
			Debug.LogError("Decrypt Error!!");
			return;
		}
		ItemRowList itemRowList = JsonUtility.FromJson<ItemRowList>("{\"items\":" + textAsset.text + "}");
		List<UnitItem> list = new List<UnitItem>();
		List<UnitItem> list2 = new List<UnitItem>();
		List<UnitItem> list3 = new List<UnitItem>();
		List<UnitItem> list4 = new List<UnitItem>();
		ItemRow[] items = itemRowList.items;
		foreach (ItemRow row in items)
		{
			UnitItem unitItem = new UnitItem();
			if (row.type.Equals("weapon"))
			{
				unitItem.type = ItemType.weapon;
			}
			else if (row.type.Equals("cloth"))
			{
				unitItem.type = ItemType.cloth;
			}
			else if (row.type.Equals("misc"))
			{
				unitItem.type = ItemType.misc;
			}
			unitItem.name = row.name;
			unitItem.code = row.code;
			if (row.subtype.Equals("onehand"))
			{
				unitItem.weaponType = UnitCharactor.WeaponType.onehand;
			}
			else if (row.subtype.Equals("doublehand"))
			{
				unitItem.weaponType = UnitCharactor.WeaponType.doublehand;
			}
			else if (row.subtype.Equals("bigsword"))
			{
				unitItem.weaponType = UnitCharactor.WeaponType.bigsword;
			}
			if (row.subtype.Equals("head"))
			{
				unitItem.clothPart = HeroModel.ClothPart.head;
			}
			else if (row.subtype.Equals("top"))
			{
				unitItem.clothPart = HeroModel.ClothPart.top;
			}
			else if (row.subtype.Equals("bottom"))
			{
				unitItem.clothPart = HeroModel.ClothPart.bottom;
			}
			if (row.subtype.Equals("ring"))
			{
				unitItem.miscType = MiscType.ring;
			}
			else if (row.subtype.Equals("consumable"))
			{
				unitItem.miscType = MiscType.consumable;
			}
			unitItem.requireLevel = row.requireLevel;
			unitItem.costGold = row.costGold;
			unitItem.costGem = row.costGem;
			unitItem.sellGold = row.sellGold;
			unitItem.ability = new Ability();
			unitItem.ability.attack = row.attack;
			unitItem.ability.defense = row.defense;
			unitItem.ability.strength = row.strength;
			unitItem.ability.intellectual = row.intellectual;
			unitItem.ability.constitution = row.constitution;
			unitItem.ability.critical = row.critical;
			unitItem.ability.speed = row.speed;
			unitItem.ability.colltime = row.colltime;
			unitItem.ability.hp = row.hp;
			unitItem.ability.mp = row.mp;
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
