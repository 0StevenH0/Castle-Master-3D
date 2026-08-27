using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InventoryManager
{
	public const int maxInventory = 24;

	private List<UnitItem> itemList = new List<UnitItem>();

	private List<UnitItem> itemWeaponList = new List<UnitItem>();

	private List<UnitItem> itemClothList = new List<UnitItem>();

	private List<UnitItem> itemMiscList = new List<UnitItem>();

	public void SetDefault()
	{
		itemList.Clear();
		itemWeaponList.Clear();
		itemClothList.Clear();
		itemMiscList.Clear();
		TextAsset textAsset = ResourceManager.Load("GameData", "player_data", typeof(TextAsset)) as TextAsset;
		string json = "{\"items\":" + textAsset.text + "}";
		PlayerDataEntryList playerDataEntryList = JsonUtility.FromJson<PlayerDataEntryList>(json);
		PlayerDataEntry[] items = playerDataEntryList.items;
		foreach (PlayerDataEntry playerDataEntry in items)
		{
			string text2 = playerDataEntry.type.ToLower().Trim();
			if (text2.Equals("weapon"))
			{
				AddItem(ItemManager.ItemType.weapon, playerDataEntry.value);
			}
			else if (text2.Equals("cloth"))
			{
				AddItem(ItemManager.ItemType.cloth, playerDataEntry.value);
			}
			else if (text2.Equals("itemhp") || text2.Equals("itemmp"))
			{
				UnitItem unitItem = AddItem(ItemManager.ItemType.misc, playerDataEntry.value);
				unitItem.quantity = playerDataEntry.quantity;
			}
		}
	}

	public UnitItem AddItem(ItemManager.ItemType type, int code)
	{
		UnitItem unitItem = null;
		unitItem = PlayInfo.itemManager.FindItem(type, code);
		if (unitItem != null)
		{
			UnitItem unitItem2 = unitItem.Clone();
			itemList.Add(unitItem2);
			switch (type)
			{
			case ItemManager.ItemType.weapon:
				itemWeaponList.Add(unitItem2);
				break;
			case ItemManager.ItemType.cloth:
				itemClothList.Add(unitItem2);
				break;
			case ItemManager.ItemType.misc:
				itemMiscList.Add(unitItem2);
				break;
			}
			return unitItem2;
		}
		return null;
	}

	public void DeleteItem(UnitItem item)
	{
		if (item == null)
		{
			return;
		}
		UnitItem unitItem = null;
		foreach (UnitItem item2 in itemList)
		{
			if (item2.code == item.code)
			{
				unitItem = item2;
				break;
			}
		}
		if (unitItem != null)
		{
			itemList.Remove(unitItem);
		}
		List<UnitItem> list = null;
		switch (item.type)
		{
		case ItemManager.ItemType.weapon:
			list = itemWeaponList;
			break;
		case ItemManager.ItemType.cloth:
			list = itemClothList;
			break;
		case ItemManager.ItemType.misc:
			list = itemMiscList;
			break;
		}
		if (list == null)
		{
			return;
		}
		foreach (UnitItem item3 in list)
		{
			if (item3.code == item.code)
			{
				unitItem = item3;
				break;
			}
		}
		if (unitItem != null)
		{
			list.Remove(unitItem);
		}
	}

	public UnitItem[] GetItemList(ItemManager.ItemType type)
	{
		switch (type)
		{
		case ItemManager.ItemType.weapon:
			return itemWeaponList.ToArray();
		case ItemManager.ItemType.cloth:
			return itemClothList.ToArray();
		case ItemManager.ItemType.misc:
			return itemMiscList.ToArray();
		default:
			return null;
		}
	}

	public UnitItem FindItem(int itemCode)
	{
		foreach (UnitItem item in itemList)
		{
			if (item.code == itemCode)
			{
				return item;
			}
		}
		return null;
	}

	public void Save()
	{
		DataRegistry.KeyData parent = DataRegistry.Set(null, "Inventory", itemList.Count);
		int num = 0;
		foreach (UnitItem item in itemList)
		{
			DataRegistry.KeyData parent2 = DataRegistry.Set(parent, num.ToString(), string.Empty);
			DataRegistry.Set(parent2, "type", (int)item.type);
			DataRegistry.Set(parent2, "code", item.code);
			DataRegistry.Set(parent2, "quantity", item.quantity);
			num++;
		}
	}

	public void Load()
	{
		int keyvalue = 0;
		DataRegistry.KeyData current = null;
		if (!DataRegistry.Get(null, "Inventory", ref keyvalue, ref current))
		{
			return;
		}
		itemList.Clear();
		itemWeaponList.Clear();
		itemClothList.Clear();
		itemMiscList.Clear();
		for (int i = 0; i < keyvalue; i++)
		{
			int keyvalue2 = -1;
			int keyvalue3 = -1;
			int keyvalue4 = -1;
			DataRegistry.KeyData current2 = null;
			string keyvalue5 = string.Empty;
			if (DataRegistry.Get(current, i.ToString(), ref keyvalue5, ref current2))
			{
				DataRegistry.KeyData current3 = null;
				DataRegistry.Get(current2, "type", ref keyvalue2, ref current3);
				DataRegistry.Get(current2, "code", ref keyvalue3, ref current3);
				DataRegistry.Get(current2, "quantity", ref keyvalue4, ref current3);
			}
			if (keyvalue2 > -1 && keyvalue3 > -1 && keyvalue4 > -1)
			{
				UnitItem unitItem = AddItem((ItemManager.ItemType)keyvalue2, keyvalue3);
				unitItem.quantity = keyvalue4;
			}
		}
	}
}
