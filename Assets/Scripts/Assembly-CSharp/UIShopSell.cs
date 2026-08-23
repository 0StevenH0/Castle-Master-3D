using System.Collections.Generic;
using UnityEngine;

public class UIShopSell : MonoBehaviour
{
	public AuiSprite shopTitle;

	public AuiButton buttonClose;

	public AuiButton buttonSell;

	public AuiSprite buttonSellLabel;

	public AuiButton buttonShopModeBuy;

	public AuiSprite[] iconList;

	public AuiButton[] buttonItem;

	public AuiSprite itemSelected;

	public AuiButton[] buttonSlot;

	public TextMesh[] textQuantity;

	public GameObject[] objectQuantity;

	public TextMesh textItemName;

	public TextMesh textRequireLv;

	public TextMesh[] textAttrName;

	public TextMesh[] textAttrValue;

	public TextMesh textCost;

	public AuiSprite iconCurrency;

	public UIShopBuy uiShopBuy;

	private ItemManager.ItemType itemType;

	private AuiSprite[] itemIcon;

	private int[] itemCode;

	private int itemPerPage = 8;

	private int listTop;

	private int selectedItemIdx;

	private bool isLoadIcon;

	private void LoadIcon()
	{
		if (isLoadIcon)
		{
			return;
		}
		isLoadIcon = true;
		string path = "interface/images/Sprite_pki_item_Materials";
		string text = "mtr_ico_item";
		List<Material> list = new List<Material>();
		List<Material> list2 = new List<Material>();
		List<Material> list3 = new List<Material>();
		for (int i = 0; i < UIInventory.itemCodeList.Length; i++)
		{
			int num = UIInventory.itemCodeList[i];
			string text2 = text;
			if (num < 200)
			{
				text2 = text2 + "weapon" + num;
				Material item = ResourceManager.Load(path, text2, typeof(Material)) as Material;
				list.Add(item);
			}
			else if (num < 300)
			{
				text2 = text2 + "cloth" + num;
				Material item2 = ResourceManager.Load(path, text2, typeof(Material)) as Material;
				list2.Add(item2);
			}
			else
			{
				text2 = text2 + "misc" + num;
				Material item3 = ResourceManager.Load(path, text2, typeof(Material)) as Material;
				list3.Add(item3);
			}
		}
		iconList[0].materials = list.ToArray();
		iconList[1].materials = list2.ToArray();
		iconList[2].materials = list3.ToArray();
	}

	private void Start()
	{
		LoadIcon();
		buttonShopModeBuy.onButtonClick = OnShopModeBuyClick;
		for (int i = 0; i < buttonSlot.Length; i++)
		{
			buttonSlot[i].SetFrame((i == 0) ? 1 : 0);
		}
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	private bool CheckEquip(UnitItem item)
	{
		if (item == null)
		{
			return false;
		}
		UnitItem[] equipWeapon = PlayInfo.playerData.equipWeapon;
		foreach (UnitItem unitItem in equipWeapon)
		{
			if (unitItem != null && item.code == unitItem.code)
			{
				return true;
			}
		}
		UnitItem[] wearCloth = PlayInfo.playerData.wearCloth;
		foreach (UnitItem unitItem2 in wearCloth)
		{
			if (unitItem2 != null && item.code == unitItem2.code)
			{
				return true;
			}
		}
		if (PlayInfo.playerData.slotRing != null && item.code == PlayInfo.playerData.slotRing.code)
		{
			return true;
		}
		return false;
	}

	public void SetShopType(ItemManager.ItemType type)
	{
		LoadIcon();
		itemType = type;
		shopTitle.SetFrame((int)type);
		if (itemIcon != null)
		{
			AuiSprite[] array = itemIcon;
			foreach (AuiSprite auiSprite in array)
			{
				Object.Destroy(auiSprite.gameObject);
			}
			itemIcon = null;
		}
		List<AuiSprite> list = new List<AuiSprite>();
		List<int> list2 = new List<int>();
		UnitItem[] itemList = PlayInfo.inventory.GetItemList(itemType);
		foreach (UnitItem unitItem in itemList)
		{
			if (unitItem.miscType != ItemManager.MiscType.consumable && !CheckEquip(unitItem))
			{
				GameObject gameObject = Object.Instantiate(iconList[(int)itemType].gameObject) as GameObject;
				gameObject.transform.parent = iconList[(int)itemType].transform.parent;
				AuiSprite component = gameObject.GetComponent<AuiSprite>();
				component.SetFrame(PlayInfo.itemManager.FindItemIndex(itemType, unitItem.code));
				list.Add(component);
				list2.Add(unitItem.code);
			}
		}
		itemIcon = list.ToArray();
		itemCode = list2.ToArray();
		itemPerPage = buttonItem.Length;
		listTop = 0;
		RefreshItemList();
		ClearItemDetail();
		int num = 0;
		AuiButton[] array2 = buttonSlot;
		foreach (AuiButton auiButton in array2)
		{
			auiButton.onButtonClick = OnSlotClick;
			auiButton.visible = num * itemPerPage < itemIcon.Length;
			num++;
		}
		buttonSell.onButtonClick = OnSellClick;
	}

	private void RefreshItemList()
	{
		int num = listTop;
		int num2 = listTop / itemPerPage;
		for (int i = 0; i < buttonSlot.Length; i++)
		{
			buttonSlot[i].SetFrame((num2 == i) ? 1 : 0);
		}
		for (int j = 0; j < listTop; j++)
		{
			itemIcon[j].visible = false;
		}
		for (int k = 0; k < itemPerPage; k++)
		{
			buttonItem[k].visible = false;
			objectQuantity[k].SetActiveRecursively(false);
		}
		for (int l = 0; l < itemPerPage; l++)
		{
			if (num >= itemIcon.Length)
			{
				break;
			}
			Vector3 localPosition = buttonItem[l].transform.localPosition;
			localPosition.z = iconList[0].transform.localPosition.z;
			itemIcon[num].transform.localPosition = localPosition;
			itemIcon[num].visible = true;
			buttonItem[l].buttonTag = num;
			buttonItem[l].visible = true;
			buttonItem[l].onButtonClick = OnItemClick;
			objectQuantity[l].SetActiveRecursively(false);
			num++;
		}
		for (int m = listTop + itemPerPage; m < itemIcon.Length; m++)
		{
			itemIcon[m].visible = false;
		}
	}

	private void ClearItemDetail()
	{
		textItemName.gameObject.active = false;
		textRequireLv.gameObject.active = false;
		TextMesh[] array = textAttrName;
		foreach (TextMesh textMesh in array)
		{
			textMesh.gameObject.active = false;
		}
		TextMesh[] array2 = textAttrValue;
		foreach (TextMesh textMesh2 in array2)
		{
			textMesh2.gameObject.active = false;
		}
		itemSelected.visible = false;
		textCost.gameObject.active = false;
		iconCurrency.visible = false;
		buttonSell.visible = false;
		buttonSellLabel.visible = false;
	}

	private void OnShopModeBuyClick(AuiButton sender)
	{
		Hide();
		uiShopBuy.SetShopType(itemType);
		uiShopBuy.Show();
	}

	private void OnSlotClick(AuiButton sender)
	{
		ClearItemDetail();
		int buttonTag = sender.buttonTag;
		listTop = buttonTag * itemPerPage;
		RefreshItemList();
	}

	private void OnItemClick(AuiButton sender)
	{
		int num = (selectedItemIdx = sender.buttonTag);
		ClearItemDetail();
		Vector3 localPosition = sender.transform.localPosition;
		localPosition.z = itemSelected.transform.localPosition.z;
		itemSelected.transform.localPosition = localPosition;
		itemSelected.visible = true;
		UnitItem unitItem = PlayInfo.itemManager.FindItem(itemType, itemCode[num]);
		if (unitItem != null)
		{
			textItemName.text = unitItem.name;
			textItemName.gameObject.active = true;
			textRequireLv.text = "Lv. " + unitItem.requireLevel;
			textRequireLv.gameObject.active = true;
			textCost.gameObject.active = true;
			iconCurrency.visible = true;
			if (unitItem.sellGold > 0)
			{
				textCost.text = unitItem.sellGold.ToString();
				iconCurrency.SetFrame(0);
			}
			int num2 = 0;
			int num3 = textAttrName.Length;
			if (num2 < num3 && unitItem.ability.attack != 0f)
			{
				textAttrName[num2].text = "ATK";
				textAttrName[num2].gameObject.active = true;
				textAttrValue[num2].text = "+" + unitItem.ability.attack;
				textAttrValue[num2].gameObject.active = true;
				num2++;
			}
			if (num2 < num3 && unitItem.ability.defense != 0f)
			{
				textAttrName[num2].text = "DEF";
				textAttrName[num2].gameObject.active = true;
				textAttrValue[num2].text = "+" + unitItem.ability.defense;
				textAttrValue[num2].gameObject.active = true;
				num2++;
			}
			if (num2 < num3 && unitItem.ability.strength != 0f)
			{
				textAttrName[num2].text = "STR";
				textAttrName[num2].gameObject.active = true;
				textAttrValue[num2].text = "+" + unitItem.ability.strength;
				textAttrValue[num2].gameObject.active = true;
				num2++;
			}
			if (num2 < num3 && unitItem.ability.intellectual != 0f)
			{
				textAttrName[num2].text = "INT";
				textAttrName[num2].gameObject.active = true;
				textAttrValue[num2].text = "+" + unitItem.ability.intellectual;
				textAttrValue[num2].gameObject.active = true;
				num2++;
			}
			if (num2 < num3 && unitItem.ability.constitution != 0f)
			{
				textAttrName[num2].text = "CON";
				textAttrName[num2].gameObject.active = true;
				textAttrValue[num2].text = "+" + unitItem.ability.constitution;
				textAttrValue[num2].gameObject.active = true;
				num2++;
			}
			if (num2 < num3 && unitItem.ability.critical != 0f)
			{
				textAttrName[num2].text = "CRI";
				textAttrName[num2].gameObject.active = true;
				textAttrValue[num2].text = "+" + unitItem.ability.critical;
				textAttrValue[num2].gameObject.active = true;
				num2++;
			}
			if (num2 < num3 && unitItem.ability.hp != 0f)
			{
				textAttrName[num2].text = ((unitItem.code < 400) ? "Max HP" : "HP");
				textAttrName[num2].gameObject.active = true;
				textAttrValue[num2].text = "+" + unitItem.ability.hp;
				textAttrValue[num2].gameObject.active = true;
				num2++;
			}
			if (num2 < num3 && unitItem.ability.mp != 0f)
			{
				textAttrName[num2].text = ((unitItem.code < 400) ? "Max MP" : "MP");
				textAttrName[num2].gameObject.active = true;
				textAttrValue[num2].text = "+" + unitItem.ability.mp;
				textAttrValue[num2].gameObject.active = true;
				num2++;
			}
			if (num2 < num3 && unitItem.ability.speed != 0f)
			{
				textAttrName[num2].text = "Speed";
				textAttrName[num2].gameObject.active = true;
				textAttrValue[num2].text = ((!(unitItem.ability.speed > 0f)) ? string.Empty : "+") + unitItem.ability.speed * 100f + "%";
				textAttrValue[num2].gameObject.active = true;
				num2++;
			}
			if (num2 < num3 && unitItem.ability.colltime != 0f)
			{
				textAttrName[num2].text = "Cooltime";
				textAttrName[num2].gameObject.active = true;
				textAttrValue[num2].text = unitItem.ability.colltime + "sec";
				textAttrValue[num2].gameObject.active = true;
				num2++;
			}
			buttonSell.visible = true;
			buttonSellLabel.visible = true;
		}
	}

	private void OnSellClick(AuiButton sender)
	{
		UnitItem unitItem = PlayInfo.itemManager.FindItem(itemType, itemCode[selectedItemIdx]);
		if (CheckEquip(unitItem))
		{
			SetShopType(itemType);
			ProcBase.ShowMsg(StringContent.msgCanNotSellEquipItem, MessageView.MsgIcon.alert);
		}
		else
		{
			ProcBase.ShowMsg(StringContent.msgQuestItemSell.Replace(StringContent.strValue, unitItem.name), MessageView.MsgIcon.question, false, new MessageView.MsgButton[2]
			{
				MessageView.MsgButton.yes,
				MessageView.MsgButton.no
			}, OnMessageSell);
		}
	}

	private void OnMessageSell(MessageView.MsgButton button)
	{
		if (button == MessageView.MsgButton.yes)
		{
			UnitItem unitItem = PlayInfo.itemManager.FindItem(itemType, itemCode[selectedItemIdx]);
			if (CheckEquip(unitItem))
			{
				SetShopType(itemType);
				ProcBase.ShowMsg(StringContent.msgCanNotSellEquipItem, MessageView.MsgIcon.alert);
			}
			else
			{
				PlayInfo.playerData.gold += unitItem.sellGold;
				PlayInfo.inventory.DeleteItem(unitItem);
				SetShopType(itemType);
			}
		}
	}

	public void Show()
	{
		LoadIcon();
		base.gameObject.SetActiveRecursively(true);
		AuiSprite[] array = iconList;
		foreach (AuiSprite auiSprite in array)
		{
			auiSprite.visible = false;
		}
		RefreshItemList();
		ClearItemDetail();
	}

	public void Hide()
	{
		base.gameObject.SetActiveRecursively(false);
	}
}
