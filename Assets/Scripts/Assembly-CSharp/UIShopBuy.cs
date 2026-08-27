using System.Collections.Generic;
using UnityEngine;

public class UIShopBuy : MonoBehaviour
{
	[System.Serializable]
	private class ShopDisplayRow
	{
		public string itemType;

		public string itemName;

		public int itemCode;

		public int saleCount;
	}

	[System.Serializable]
	private class ShopDisplayRowWrapper
	{
		public ShopDisplayRow[] items;
	}

	public AuiSprite shopTitle;

	public AuiButton buttonClose;

	public AuiButton buttonBuy;

	public AuiSprite buttonBuyLabel;

	public AuiButton buttonPreview;

	public AuiSprite buttonPreviewLabel;

	public AuiButton buttonPreviewClose;

	public AuiButton buttonShopModeSell;

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

	public GameObject previewPanel;

	public GameObject previewParent;

	public HeroModel previewUnit;

	public UnitCharactor thisUnit;

	public UIShopSell uiShopSell;

	public ProcCastle procCastle;

	private ItemManager.ItemType itemType;

	private AuiSprite[] itemIcon;

	private int[] itemCode;

	private int[] saleCount;

	private int itemPerPage = 8;

	private int listTop;

	private int selectedItemIdx;

	private bool isMouseDrag;

	private Vector3 posMouseDragStart = new Vector3(0f, 0f, 0f);

	private Vector3 previewDragMin = new Vector3(-293f, -120f, 0f);

	private Vector3 previewDragMax = new Vector3(-74f, 128f, 0f);

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
		buttonShopModeSell.onButtonClick = OnShopModeSellClick;
		for (int i = 0; i < buttonSlot.Length; i++)
		{
			buttonSlot[i].SetFrame((i == 0) ? 1 : 0);
		}
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	public void SetShopType(ItemManager.ItemType type)
	{
		LoadIcon();
		this.itemType = type;
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
		TextAsset textAsset = ResourceManager.Load("GameData", "shop_display", typeof(TextAsset)) as TextAsset;
		string json = "{\"items\":" + textAsset.text + "}";
		ShopDisplayRowWrapper shopDisplayRowWrapper = JsonUtility.FromJson<ShopDisplayRowWrapper>(json);
		List<AuiSprite> list = new List<AuiSprite>();
		List<int> list2 = new List<int>();
		List<int> list3 = new List<int>();
		int num = 0;
		ShopDisplayRow[] items = shopDisplayRowWrapper.items;
		foreach (ShopDisplayRow shopDisplayRow in items)
		{
			string text2 = shopDisplayRow.itemType.Trim();
			ItemManager.ItemType itemType = ItemManager.ItemType.weapon;
			if (text2.Equals("weapon"))
			{
				itemType = ItemManager.ItemType.weapon;
			}
			else if (text2.Equals("cloth"))
			{
				itemType = ItemManager.ItemType.cloth;
			}
			else if (text2.Equals("misc"))
			{
				itemType = ItemManager.ItemType.misc;
			}
			if (itemType == this.itemType)
			{
				GameObject gameObject = Object.Instantiate(iconList[(int)this.itemType].gameObject) as GameObject;
				gameObject.transform.parent = iconList[(int)this.itemType].transform.parent;
				int num2 = shopDisplayRow.itemCode;
				int item = shopDisplayRow.saleCount;
				AuiSprite component = gameObject.GetComponent<AuiSprite>();
				component.SetFrame(PlayInfo.itemManager.FindItemIndex(this.itemType, num2));
				list.Add(component);
				list2.Add(num2);
				list3.Add(item);
				num++;
			}
		}
		itemIcon = list.ToArray();
		itemCode = list2.ToArray();
		saleCount = list3.ToArray();
		itemPerPage = buttonItem.Length;
		listTop = 0;
		RefreshItemList();
		ClearItemDetail();
		int num3 = 0;
		AuiButton[] array3 = buttonSlot;
		foreach (AuiButton auiButton in array3)
		{
			auiButton.onButtonClick = OnSlotClick;
			auiButton.visible = num3 * itemPerPage < itemIcon.Length;
			num3++;
		}
		buttonBuy.onButtonClick = OnBuyClick;
		buttonPreview.onButtonClick = OnPreviewClick;
		buttonPreviewClose.onButtonClick = OnPreviewCloseClick;
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
			objectQuantity[k].SetActiveRecursive(false);
		}
		for (int l = 0; l < itemPerPage; l++)
		{
			Vector3 localPosition = buttonItem[l].transform.localPosition;
			localPosition.z = iconList[0].transform.localPosition.z;
			itemIcon[num].transform.localPosition = localPosition;
			itemIcon[num].visible = true;
			buttonItem[l].buttonTag = num;
			buttonItem[l].visible = true;
			buttonItem[l].onButtonClick = OnItemClick;
			if (saleCount[num] > 1)
			{
				textQuantity[l].text = saleCount[num].ToString();
				objectQuantity[l].SetActiveRecursive(true);
			}
			else
			{
				objectQuantity[l].SetActiveRecursive(false);
			}
			num++;
			if (num >= itemIcon.Length)
			{
				break;
			}
		}
		for (int m = listTop + itemPerPage; m < itemIcon.Length; m++)
		{
			itemIcon[m].visible = false;
		}
	}

	private void ClearItemDetail()
	{
		textItemName.gameObject.SetActive(false);
		textRequireLv.gameObject.SetActive(false);
		TextMesh[] array = textAttrName;
		foreach (TextMesh textMesh in array)
		{
			textMesh.gameObject.SetActive(false);
		}
		TextMesh[] array2 = textAttrValue;
		foreach (TextMesh textMesh2 in array2)
		{
			textMesh2.gameObject.SetActive(false);
		}
		itemSelected.visible = false;
		textCost.gameObject.SetActive(false);
		iconCurrency.visible = false;
		previewPanel.SetActiveRecursive(false);
		buttonBuy.visible = false;
		buttonBuyLabel.visible = false;
		buttonPreview.visible = false;
		buttonPreviewLabel.visible = false;
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
			textItemName.gameObject.SetActive(true);
			textRequireLv.text = "Lv. " + unitItem.requireLevel;
			textRequireLv.gameObject.SetActive(true);
			textCost.gameObject.SetActive(true);
			iconCurrency.visible = true;
			if (unitItem.costGold > 0)
			{
				textCost.text = (unitItem.costGold * saleCount[num]).ToString();
				iconCurrency.SetFrame(0);
			}
			else if (unitItem.costGem > 0)
			{
				textCost.text = (unitItem.costGem * saleCount[num]).ToString();
				iconCurrency.SetFrame(1);
			}
			int num2 = 0;
			int num3 = textAttrName.Length;
			if (num2 < num3 && unitItem.ability.attack != 0f)
			{
				textAttrName[num2].text = "ATK";
				textAttrName[num2].gameObject.SetActive(true);
				textAttrValue[num2].text = "+" + unitItem.ability.attack;
				textAttrValue[num2].gameObject.SetActive(true);
				num2++;
			}
			if (num2 < num3 && unitItem.ability.defense != 0f)
			{
				textAttrName[num2].text = "DEF";
				textAttrName[num2].gameObject.SetActive(true);
				textAttrValue[num2].text = "+" + unitItem.ability.defense;
				textAttrValue[num2].gameObject.SetActive(true);
				num2++;
			}
			if (num2 < num3 && unitItem.ability.strength != 0f)
			{
				textAttrName[num2].text = "STR";
				textAttrName[num2].gameObject.SetActive(true);
				textAttrValue[num2].text = "+" + unitItem.ability.strength;
				textAttrValue[num2].gameObject.SetActive(true);
				num2++;
			}
			if (num2 < num3 && unitItem.ability.intellectual != 0f)
			{
				textAttrName[num2].text = "INT";
				textAttrName[num2].gameObject.SetActive(true);
				textAttrValue[num2].text = "+" + unitItem.ability.intellectual;
				textAttrValue[num2].gameObject.SetActive(true);
				num2++;
			}
			if (num2 < num3 && unitItem.ability.constitution != 0f)
			{
				textAttrName[num2].text = "CON";
				textAttrName[num2].gameObject.SetActive(true);
				textAttrValue[num2].text = "+" + unitItem.ability.constitution;
				textAttrValue[num2].gameObject.SetActive(true);
				num2++;
			}
			if (num2 < num3 && unitItem.ability.critical != 0f)
			{
				textAttrName[num2].text = "CRI";
				textAttrName[num2].gameObject.SetActive(true);
				textAttrValue[num2].text = "+" + unitItem.ability.critical;
				textAttrValue[num2].gameObject.SetActive(true);
				num2++;
			}
			if (num2 < num3 && unitItem.ability.hp != 0f)
			{
				textAttrName[num2].text = ((unitItem.code < 400) ? "Max HP" : "HP");
				textAttrName[num2].gameObject.SetActive(true);
				textAttrValue[num2].text = "+" + unitItem.ability.hp;
				textAttrValue[num2].gameObject.SetActive(true);
				num2++;
			}
			if (num2 < num3 && unitItem.ability.mp != 0f)
			{
				textAttrName[num2].text = ((unitItem.code < 400) ? "Max MP" : "MP");
				textAttrName[num2].gameObject.SetActive(true);
				textAttrValue[num2].text = "+" + unitItem.ability.mp;
				textAttrValue[num2].gameObject.SetActive(true);
				num2++;
			}
			if (num2 < num3 && unitItem.ability.speed != 0f)
			{
				textAttrName[num2].text = "Speed";
				textAttrName[num2].gameObject.SetActive(true);
				textAttrValue[num2].text = ((!(unitItem.ability.speed > 0f)) ? string.Empty : "+") + unitItem.ability.speed * 100f + "%";
				textAttrValue[num2].gameObject.SetActive(true);
				num2++;
			}
			if (num2 < num3 && unitItem.ability.colltime != 0f)
			{
				textAttrName[num2].text = "Cooltime";
				textAttrName[num2].gameObject.SetActive(true);
				textAttrValue[num2].text = unitItem.ability.colltime + "sec";
				textAttrValue[num2].gameObject.SetActive(true);
				num2++;
			}
			buttonBuy.visible = true;
			buttonBuyLabel.visible = true;
			if (itemType == ItemManager.ItemType.misc)
			{
				Vector3 localPosition2 = buttonBuy.transform.localPosition;
				localPosition2.x = 0f;
				buttonBuy.transform.localPosition = localPosition2;
				localPosition2.z -= 1f;
				buttonBuyLabel.transform.localPosition = localPosition2;
			}
			else
			{
				Vector3 localPosition3 = buttonBuy.transform.localPosition;
				localPosition3.x = -94f;
				buttonBuy.transform.localPosition = localPosition3;
				localPosition3.z -= 1f;
				buttonBuyLabel.transform.localPosition = localPosition3;
				buttonPreview.visible = true;
				buttonPreviewLabel.visible = true;
			}
		}
	}

	private void OnPreviewClick(AuiButton sender)
	{
		if (itemType != ItemManager.ItemType.misc)
		{
			previewPanel.SetActiveRecursive(true);
			if (previewUnit == null)
			{
				GameObject gameObject = Object.Instantiate(ResourceManager.Load("Character/prefeb/character", "feb_hero01", typeof(GameObject))) as GameObject;
				gameObject.transform.parent = previewParent.transform;
				gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
				gameObject.gameObject.AddComponent<AdjustAnimationSpeed>();
				previewUnit = gameObject.gameObject.AddComponent<HeroModel>();
				LayerManager.SetLayerAllChild(gameObject.transform, LayerManager.layerPreviewInven);
			}
			if (itemType == ItemManager.ItemType.weapon)
			{
				previewUnit.SetCloth(thisUnit.heroModel.headIndex, thisUnit.heroModel.topIndex, thisUnit.heroModel.bottomIndex);
				UnitItem unitItem = PlayInfo.itemManager.FindItem(itemType, itemCode[selectedItemIdx]);
				previewUnit.SetWeapon(thisUnit.weaponManager, unitItem.weaponType, unitItem.code);
			}
			else if (itemType == ItemManager.ItemType.cloth)
			{
				previewUnit.SetCloth(thisUnit.heroModel.headIndex, thisUnit.heroModel.topIndex, thisUnit.heroModel.bottomIndex);
				UnitItem unitItem2 = PlayInfo.itemManager.FindItem(itemType, itemCode[selectedItemIdx]);
				previewUnit.SetClothPartFromCode(unitItem2.clothPart, itemCode[selectedItemIdx]);
				previewUnit.SetWeapon(thisUnit.weaponManager, thisUnit.weaponType, thisUnit.weaponCode);
			}
			LayerManager.SetLayerAllChild(previewUnit.transform, LayerManager.layerPreviewShop);
			Bounds gameObjectBound = ProcBase.GetGameObjectBound(previewPanel);
			previewDragMin = gameObjectBound.min;
			previewDragMax = gameObjectBound.max;
		}
	}

	private void OnPreviewCloseClick(AuiButton sender)
	{
		previewPanel.SetActiveRecursive(false);
	}

	private void Update()
	{
		if (!previewPanel.gameObject.activeInHierarchy)
		{
			return;
		}
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 mousePosition = Input.mousePosition;
			mousePosition = buttonClose.uiCamera.ScreenToWorldPoint(mousePosition);
			if (mousePosition.x > previewDragMin.x && mousePosition.x < previewDragMax.x && mousePosition.y > previewDragMin.y && mousePosition.y < previewDragMax.y)
			{
				isMouseDrag = true;
				posMouseDragStart = mousePosition;
			}
		}
		else if (Input.GetMouseButtonUp(0))
		{
			isMouseDrag = false;
		}
		if (isMouseDrag)
		{
			Vector3 mousePosition2 = Input.mousePosition;
			mousePosition2 = buttonClose.uiCamera.ScreenToWorldPoint(mousePosition2);
			float num = posMouseDragStart.x - mousePosition2.x;
			Vector3 eulerAngles = previewUnit.transform.localRotation.eulerAngles;
			eulerAngles.y += num;
			posMouseDragStart = mousePosition2;
			previewUnit.transform.localRotation = Quaternion.Euler(eulerAngles);
		}
	}

	private void OnBuyClick(AuiButton sender)
	{
		UnitItem unitItem = PlayInfo.itemManager.FindItem(itemType, itemCode[selectedItemIdx]);
		if (unitItem.requireLevel > PlayInfo.heroState.level)
		{
			ProcBase.ShowMsg(StringContent.msgRequireHeroLevel.Replace(StringContent.strValue, unitItem.requireLevel.ToString()), MessageView.MsgIcon.alert);
			return;
		}
		if (unitItem.costGold * saleCount[selectedItemIdx] > PlayInfo.playerData.gold)
		{
			ShowGotoGoldShop();
			return;
		}
		if (unitItem.costGem * saleCount[selectedItemIdx] > PlayInfo.playerData.gem)
		{
			ShowGotoGemShop();
			return;
		}
		if ((itemType != ItemManager.ItemType.misc || unitItem.miscType != ItemManager.MiscType.consumable) && PlayInfo.inventory.FindItem(unitItem.code) != null)
		{
			ProcBase.ShowMsg(StringContent.msgAlreadyBoughtItem, MessageView.MsgIcon.alert);
			return;
		}
		int num = PlayInfo.inventory.GetItemList(itemType).Length;
		if (num >= 24)
		{
			ProcBase.ShowMsg(StringContent.msgNotEnoughInventory, MessageView.MsgIcon.alert);
			return;
		}
		if (PlayInfo.playerData.gold > 0)
		{
			PlayInfo.playerData.gold -= unitItem.costGold * saleCount[selectedItemIdx];
		}
		if (PlayInfo.playerData.gem > 0)
		{
			PlayInfo.playerData.gem -= unitItem.costGem * saleCount[selectedItemIdx];
		}
		if (itemType == ItemManager.ItemType.misc && unitItem.miscType == ItemManager.MiscType.consumable)
		{
			UnitItem unitItem2 = PlayInfo.inventory.FindItem(unitItem.code);
			if (unitItem2 == null)
			{
				unitItem2 = PlayInfo.inventory.AddItem(itemType, unitItem.code);
				unitItem2.quantity = saleCount[selectedItemIdx];
			}
			else
			{
				unitItem2.quantity += saleCount[selectedItemIdx];
			}
		}
		else
		{
			PlayInfo.inventory.AddItem(itemType, unitItem.code);
		}
		PlayInfo.soundManager.Play(ExtSoundManager.Effect2D.efs_itembuy, new Vector3(0f, 0f, 0f));
	}

	private void OnShopModeSellClick(AuiButton sender)
	{
		Hide();
		uiShopSell.SetShopType(itemType);
		uiShopSell.Show();
	}

	public void Show()
	{
		LoadIcon();
		base.gameObject.SetActiveRecursive(true);
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
		base.gameObject.SetActiveRecursive(false);
	}

	private void ShowGotoGoldShop()
	{
		ProcBase.ShowMsg(StringContent.msgNotEnoughGold, MessageView.MsgIcon.alert, new MessageView.MsgButton[2]
		{
			MessageView.MsgButton.yes,
			MessageView.MsgButton.no
		}, OnMessageGotoGemShop);
	}

	private void ShowGotoGemShop()
	{
		ProcBase.ShowMsg(StringContent.msgNotEnoughGem, MessageView.MsgIcon.alert, new MessageView.MsgButton[2]
		{
			MessageView.MsgButton.yes,
			MessageView.MsgButton.no
		}, OnMessageGotoGemShop);
	}

	private void OnMessageGotoGemShop(MessageView.MsgButton button)
	{
		if (button == MessageView.MsgButton.yes)
		{
			Hide();
			procCastle.ShowMap();
			procCastle.uiGemShop.Show();
		}
	}
}
