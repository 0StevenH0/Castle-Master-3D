using System.Collections.Generic;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
	private const int itemPerPage = 8;

	public AuiButton buttonClose;

	public AuiButton[] buttonItemList;

	public AuiButton[] buttonItemType = new AuiButton[3];

	public AuiButton[] buttonSlot;

	public AuiButton buttonEquip;

	public AuiSprite buttonEquipLabel;

	public AuiButton[] buttonEquipItem;

	public AuiButton buttonDelete;

	public AuiSprite iconItemList;

	public AuiSprite[] equipSlot;

	public AuiSprite[] iconEquip;

	public AuiSprite iconItemSelect;

	public TextMesh[] textQuantity;

	public GameObject[] objectQuantity;

	public TextMesh[] textEquipQuantity;

	public GameObject[] objectEquipQuantity;

	public TextMesh textHp;

	public TextMesh textMp;

	public TextMesh textXp;

	public TextMesh textLevel;

	public TextMesh textPlayerName;

	public TextMesh[] textAbility;

	public AuiSprite gaugeHp;

	public AuiSprite gaugeMp;

	public AuiSprite gaugeXp;

	public TextMesh textItemName;

	public TextMesh textRequireLv;

	public TextMesh[] textAttrName;

	public TextMesh[] textAttrValue;

	public UnitCharactor thisUnit;

	public HeroModel previewUnit;

	public GameObject previewPanel;

	public GameObject panelStatsAlarm;

	public GameObject panelStatsUp;

	public TextMesh remainStats;

	public AuiSpriteAnimation aniStatsAlarm;

	public AuiButton buttonStatsUpOpen;

	public AuiButton buttonStatsUpClose;

	public AuiButton[] buttonStatsInc;

	public AuiButton buttonStatsUpSubmit;

	public TextMesh[] textStatsInc;

	public TextMesh textStatsPoint;

	public TextMesh textChooseStatsPoint;

	public TextMesh textAttack;

	public TextMesh textMaxMP;

	public TextMesh textMaxHP;

	public TextMesh textSkillAtk;

	public TextMesh textDefense;

	private List<AuiSprite> itemList = new List<AuiSprite>();

	private AuiSprite[] iconEquipWeapon;

	private AuiSprite[] iconWearCloth;

	private AuiSprite iconEquipRing;

	private AuiSprite iconItemSlotHP;

	private AuiSprite iconItemSlotMP;

	private ItemManager.ItemType selectedItemType;

	private int itemScrollTop;

	private UnitItem selectedItem;

	public static int[] itemCodeList = new int[65]
	{
		100, 101, 102, 103, 104, 105, 106, 107, 108, 110,
		111, 112, 113, 114, 115, 116, 117, 118, 120, 121,
		122, 123, 124, 125, 126, 127, 128, 200, 201, 202,
		203, 204, 205, 206, 207, 208, 210, 211, 212, 213,
		214, 215, 216, 217, 218, 220, 221, 222, 223, 224,
		225, 226, 227, 228, 300, 301, 302, 303, 304, 305,
		306, 400, 401, 410, 411
	};

	private bool isMouseDrag;

	private Vector3 posMouseDragStart = new Vector3(0f, 0f, 0f);

	private Vector3 previewDragMin = new Vector3(-293f, -180f, 0f);

	private Vector3 previewDragMax = new Vector3(-74f, 128f, 0f);

	private UnitState stateTemp;

	private int[] incStats;

	private bool isReset;

	private bool isLoadIcon;

	private void LoadIcon()
	{
		if (!isLoadIcon)
		{
			isLoadIcon = true;
			List<Material> list = new List<Material>();
			string path = "interface/images/Sprite_pki_item_Materials";
			string text = "mtr_ico_item";
			for (int i = 0; i < itemCodeList.Length; i++)
			{
				string text2 = text;
				text2 = ((itemCodeList[i] >= 200) ? ((itemCodeList[i] >= 300) ? (text2 + "misc") : (text2 + "cloth")) : (text2 + "weapon"));
				text2 += itemCodeList[i];
				Material item = ResourceManager.Load(path, text2, typeof(Material)) as Material;
				list.Add(item);
			}
			iconItemList.materials = list.ToArray();
		}
	}

	private void Start()
	{
		LoadIcon();
		AuiButton.SetTopAllChild(base.transform);
		for (int i = 0; i < buttonItemType.Length; i++)
		{
			buttonItemType[i].buttonTag = i;
			buttonItemType[i].onButtonClick = OnItemTypeClick;
		}
		OnItemTypeClick(buttonItemType[(int)selectedItemType]);
		for (int j = 0; j < buttonItemList.Length; j++)
		{
			buttonItemList[j].onButtonClick = OnItemListClick;
		}
		for (int k = 0; k < buttonSlot.Length; k++)
		{
			buttonSlot[k].buttonTag = k;
			buttonSlot[k].onButtonClick = OnSlotClock;
		}
		buttonClose.onButtonClick = OnCloseClick;
		buttonEquip.onButtonClick = OnEquipClick;
		buttonDelete.onButtonClick = OnDeleteClick;
		buttonStatsUpOpen.onButtonClick = OnStatsUpOpenClick;
		buttonStatsUpClose.onButtonClick = OnStatsUpCloseClick;
		int num = 0;
		AuiButton[] array = buttonStatsInc;
		foreach (AuiButton auiButton in array)
		{
			auiButton.onButtonClick = OnStatsIncClick;
			auiButton.buttonTag = num;
			num++;
		}
		buttonStatsUpSubmit.onButtonClick = OnStatsUpSubmitClick;
		num = 0;
		AuiButton[] array2 = buttonEquipItem;
		foreach (AuiButton auiButton2 in array2)
		{
			auiButton2.onButtonClick = OnEquipItemClick;
			auiButton2.buttonTag = num;
			num++;
		}
		iconItemList.visible = false;
		ResetIconList();
		ResetIconEquip();
		RefreshItemList();
		RefreshItemDetail();
		RefreshPlayerInfo();
		RefreshPreview();
		textStatsPoint.text = StringContent.wordStatsPoint;
		textChooseStatsPoint.text = StringContent.msgChooseStats;
		textAttack.text = StringContent.wordAttack;
		textMaxMP.text = StringContent.wordMaxMP;
		textMaxHP.text = StringContent.wordMaxHP;
		textSkillAtk.text = StringContent.wordSkillAtk;
		textDefense.text = StringContent.wordDefenseUp;
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	private void ResetIconList()
	{
		isReset = true;
		for (int i = 1; i < itemList.Count; i++)
		{
			Object.DestroyObject(itemList[i].gameObject);
		}
		itemList.Clear();
		UnitItem[] array = PlayInfo.inventory.GetItemList(selectedItemType);
		itemScrollTop = 0;
		for (int j = 0; j < array.Length; j++)
		{
			int num = FindItemIndex(array[j].code);
			if (num >= 0)
			{
				AuiSprite auiSprite = iconItemList;
				if (j > 0)
				{
					auiSprite = Object.Instantiate(iconItemList) as AuiSprite;
				}
				auiSprite.transform.parent = iconItemList.transform.parent;
				auiSprite.SetFrame(num);
				itemList.Add(auiSprite);
			}
		}
	}

	private void ResetIconEquip()
	{
		iconEquipWeapon = new AuiSprite[3];
		for (int i = 0; i < iconEquipWeapon.Length; i++)
		{
			iconEquipWeapon[i] = Object.Instantiate(iconItemList) as AuiSprite;
			iconEquipWeapon[i].transform.parent = iconItemList.transform.root;
		}
		iconWearCloth = new AuiSprite[3];
		for (int j = 0; j < iconEquipWeapon.Length; j++)
		{
			iconWearCloth[j] = Object.Instantiate(iconItemList) as AuiSprite;
			iconWearCloth[j].transform.parent = iconItemList.transform.root;
		}
		iconItemSlotHP = Object.Instantiate(iconItemList) as AuiSprite;
		iconItemSlotHP.transform.parent = iconItemList.transform.root;
		iconItemSlotMP = Object.Instantiate(iconItemList) as AuiSprite;
		iconItemSlotMP.transform.parent = iconItemList.transform.root;
		iconEquipRing = Object.Instantiate(iconItemList) as AuiSprite;
		iconEquipRing.transform.parent = iconItemList.transform.root;
		Vector3 position = equipSlot[0].transform.position;
		iconEquipWeapon[0].transform.position = new Vector3(position.x, position.y, position.z - 1f);
		position = equipSlot[1].transform.position;
		iconEquipWeapon[1].transform.position = new Vector3(position.x, position.y, position.z - 1f);
		position = equipSlot[2].transform.position;
		iconEquipWeapon[2].transform.position = new Vector3(position.x, position.y, position.z - 1f);
		position = equipSlot[3].transform.position;
		iconWearCloth[0].transform.position = new Vector3(position.x, position.y, position.z - 1f);
		position = equipSlot[4].transform.position;
		iconWearCloth[1].transform.position = new Vector3(position.x, position.y, position.z - 1f);
		position = equipSlot[5].transform.position;
		iconWearCloth[2].transform.position = new Vector3(position.x, position.y, position.z - 1f);
		position = equipSlot[6].transform.position;
		iconItemSlotHP.transform.position = new Vector3(position.x, position.y, position.z - 1f);
		position = equipSlot[7].transform.position;
		iconItemSlotMP.transform.position = new Vector3(position.x, position.y, position.z - 1f);
		position = equipSlot[8].transform.position;
		iconEquipRing.transform.position = new Vector3(position.x, position.y, position.z - 1f);
	}

	private void RefreshItemList()
	{
		int num = itemScrollTop;
		int num2 = itemScrollTop / 8;
		for (int i = 0; i < buttonSlot.Length; i++)
		{
			buttonSlot[i].SetFrame((num2 == i) ? 1 : 0);
		}
		iconItemSelect.visible = false;
		AuiSprite[] array = iconEquip;
		foreach (AuiSprite auiSprite in array)
		{
			auiSprite.visible = false;
		}
		for (int k = 0; k < buttonItemList.Length; k++)
		{
			buttonItemList[k].enabled = false;
			objectQuantity[k].gameObject.SetActiveRecursively(false);
		}
		for (int l = 0; l < itemList.Count; l++)
		{
			int num3 = l - num;
			if (l >= num && l < num + 8)
			{
				itemList[l].visible = true;
				Vector3 localPosition = buttonItemList[num3].transform.localPosition;
				localPosition.z -= 1f;
				itemList[l].transform.localPosition = localPosition;
				buttonItemList[num3].enabled = true;
				buttonItemList[num3].buttonTag = itemCodeList[itemList[l].curFrame];
				UnitItem unitItem = PlayInfo.inventory.FindItem(itemCodeList[itemList[l].curFrame]);
				if (unitItem != null && unitItem.quantity > 1)
				{
					textQuantity[num3].text = unitItem.quantity.ToString();
					objectQuantity[num3].gameObject.SetActiveRecursively(true);
				}
				else
				{
					objectQuantity[num3].gameObject.SetActiveRecursively(false);
				}
				if (selectedItem != null && unitItem.code == selectedItem.code)
				{
					iconItemSelect.transform.localPosition = new Vector3(localPosition.x, localPosition.y, localPosition.z - 2f);
					iconItemSelect.visible = true;
				}
				Vector3 localPosition2 = new Vector3(localPosition.x - 25f, localPosition.y + 30f, localPosition.z - 3f);
				int num4 = 0;
				UnitItem[] equipWeapon = PlayInfo.playerData.equipWeapon;
				foreach (UnitItem unitItem2 in equipWeapon)
				{
					if (unitItem2 != null && unitItem2.code == unitItem.code)
					{
						iconEquip[num4].transform.localPosition = localPosition2;
						iconEquip[num4].visible = true;
					}
					num4++;
				}
				num4 = 0;
				UnitItem[] wearCloth = PlayInfo.playerData.wearCloth;
				foreach (UnitItem unitItem3 in wearCloth)
				{
					if (unitItem3 != null && unitItem3.code == unitItem.code)
					{
						iconEquip[num4].transform.localPosition = localPosition2;
						iconEquip[num4].visible = true;
					}
					num4++;
				}
				if (PlayInfo.playerData.slotRing != null && unitItem.code == PlayInfo.playerData.slotRing.code)
				{
					iconEquip[0].transform.localPosition = localPosition2;
					iconEquip[0].visible = true;
				}
				if (PlayInfo.playerData.slotHp != null && unitItem.code == PlayInfo.playerData.slotHp.code)
				{
					iconEquip[1].transform.localPosition = localPosition2;
					iconEquip[1].visible = true;
				}
				if (PlayInfo.playerData.slotMp != null && unitItem.code == PlayInfo.playerData.slotMp.code)
				{
					iconEquip[2].transform.localPosition = localPosition2;
					iconEquip[2].visible = true;
				}
			}
			else
			{
				itemList[l].visible = false;
			}
		}
	}

	private void RefreshPlayerInfo()
	{
		PlayerData playerData = PlayInfo.playerData;
		UnitState heroState = PlayInfo.heroState;
		textLevel.text = "Lv." + heroState.level;
		textHp.text = heroState.curHp + "/" + heroState.sumHp;
		textMp.text = heroState.curMp + "/" + heroState.sumMp;
		textXp.text = (int)(heroState.exp * 100f / PlayInfo.GetNextExp(heroState)) + "%";
		textPlayerName.text = playerData.heroName;
		ProcBase.ResetTextWidth(textPlayerName, 230f);
		gaugeHp.isCrop = true;
		gaugeHp.crop = new Rect(0f, 0f, heroState.curHp / (float)heroState.sumHp, 1f);
		gaugeHp.SetFrame(0);
		gaugeMp.isCrop = true;
		gaugeMp.crop = new Rect(0f, 0f, heroState.curMp / (float)heroState.sumMp, 1f);
		gaugeMp.SetFrame(0);
		gaugeXp.isCrop = true;
		gaugeXp.crop = new Rect(0f, 0f, heroState.exp / PlayInfo.GetNextExp(heroState), 1f);
		gaugeXp.SetFrame(0);
		textAbility[0].text = heroState.fame.ToString();
		textAbility[1].text = heroState.sumAttack.ToString();
		textAbility[2].text = heroState.sumDefense.ToString();
		textAbility[3].text = heroState.sumStr.ToString();
		textAbility[4].text = heroState.sumInt.ToString();
		textAbility[5].text = heroState.sumCon.ToString();
		textAbility[6].text = heroState.sumCri.ToString();
		for (int i = 0; i < playerData.equipWeapon.Length; i++)
		{
			iconEquipWeapon[i].visible = playerData.equipWeapon[i] != null;
			if (playerData.equipWeapon[i] != null)
			{
				int num = FindItemIndex(playerData.equipWeapon[i].code);
				if (num > -1)
				{
					iconEquipWeapon[i].SetFrame(num);
				}
			}
		}
		for (int j = 0; j < playerData.wearCloth.Length; j++)
		{
			iconWearCloth[j].visible = playerData.wearCloth[j] != null;
			if (playerData.wearCloth[j] != null)
			{
				int num2 = FindItemIndex(playerData.wearCloth[j].code);
				if (num2 > -1)
				{
					iconWearCloth[j].SetFrame(num2);
				}
			}
		}
		iconItemSlotHP.visible = playerData.slotHp != null;
		objectEquipQuantity[0].SetActiveRecursively(playerData.slotHp != null);
		if (playerData.slotHp != null)
		{
			int num3 = FindItemIndex(playerData.slotHp.code);
			if (num3 > -1)
			{
				iconItemSlotHP.SetFrame(num3);
			}
			textEquipQuantity[0].text = playerData.slotHp.quantity.ToString();
		}
		iconItemSlotMP.visible = playerData.slotMp != null;
		objectEquipQuantity[1].SetActiveRecursively(playerData.slotMp != null);
		if (playerData.slotMp != null)
		{
			int num4 = FindItemIndex(playerData.slotMp.code);
			if (num4 > -1)
			{
				iconItemSlotMP.SetFrame(num4);
			}
			textEquipQuantity[1].text = playerData.slotMp.quantity.ToString();
		}
		iconEquipRing.visible = playerData.slotRing != null;
		if (playerData.slotRing != null)
		{
			int num5 = FindItemIndex(playerData.slotRing.code);
			if (num5 > -1)
			{
				iconEquipRing.SetFrame(num5);
			}
		}
	}

	private void RefreshItemDetail()
	{
		UnitItem unitItem = selectedItem;
		bool flag = unitItem != null;
		textItemName.gameObject.active = flag;
		textRequireLv.gameObject.active = flag;
		buttonEquip.visible = flag;
		buttonEquipLabel.visible = flag;
		buttonDelete.visible = flag;
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
		if (flag)
		{
			if (unitItem.type == ItemManager.ItemType.cloth)
			{
				buttonEquipLabel.SetFrame(1);
			}
			else
			{
				buttonEquipLabel.SetFrame(0);
			}
			textItemName.text = unitItem.name;
			textRequireLv.text = "Lv." + unitItem.requireLevel;
			int num = 0;
			int num2 = textAttrName.Length;
			if (num < num2 && unitItem.ability.attack != 0f)
			{
				textAttrName[num].text = "ATK";
				textAttrName[num].gameObject.active = true;
				textAttrValue[num].text = "+" + unitItem.ability.attack;
				textAttrValue[num].gameObject.active = true;
				num++;
			}
			if (num < num2 && unitItem.ability.defense != 0f)
			{
				textAttrName[num].text = "DEF";
				textAttrName[num].gameObject.active = true;
				textAttrValue[num].text = "+" + unitItem.ability.defense;
				textAttrValue[num].gameObject.active = true;
				num++;
			}
			if (num < num2 && unitItem.ability.strength != 0f)
			{
				textAttrName[num].text = "STR";
				textAttrName[num].gameObject.active = true;
				textAttrValue[num].text = "+" + unitItem.ability.strength;
				textAttrValue[num].gameObject.active = true;
				num++;
			}
			if (num < num2 && unitItem.ability.intellectual != 0f)
			{
				textAttrName[num].text = "INT";
				textAttrName[num].gameObject.active = true;
				textAttrValue[num].text = "+" + unitItem.ability.intellectual;
				textAttrValue[num].gameObject.active = true;
				num++;
			}
			if (num < num2 && unitItem.ability.constitution != 0f)
			{
				textAttrName[num].text = "CON";
				textAttrName[num].gameObject.active = true;
				textAttrValue[num].text = "+" + unitItem.ability.constitution;
				textAttrValue[num].gameObject.active = true;
				num++;
			}
			if (num < num2 && unitItem.ability.critical != 0f)
			{
				textAttrName[num].text = "CRI";
				textAttrName[num].gameObject.active = true;
				textAttrValue[num].text = "+" + unitItem.ability.critical;
				textAttrValue[num].gameObject.active = true;
				num++;
			}
			if (num < num2 && unitItem.ability.hp != 0f)
			{
				textAttrName[num].text = ((unitItem.code < 400) ? "Max HP" : "HP");
				textAttrName[num].gameObject.active = true;
				textAttrValue[num].text = "+" + unitItem.ability.hp;
				textAttrValue[num].gameObject.active = true;
				num++;
			}
			if (num < num2 && unitItem.ability.mp != 0f)
			{
				textAttrName[num].text = ((unitItem.code < 400) ? "Max MP" : "MP");
				textAttrName[num].gameObject.active = true;
				textAttrValue[num].text = "+" + unitItem.ability.mp;
				textAttrValue[num].gameObject.active = true;
				num++;
			}
			if (num < num2 && unitItem.ability.speed != 0f)
			{
				textAttrName[num].text = "Speed";
				textAttrName[num].gameObject.active = true;
				textAttrValue[num].text = ((!(unitItem.ability.speed > 0f)) ? string.Empty : "+") + unitItem.ability.speed * 100f + "%";
				textAttrValue[num].gameObject.active = true;
				num++;
			}
			if (num < num2 && unitItem.ability.colltime != 0f)
			{
				textAttrName[num].text = "Cooltime";
				textAttrName[num].gameObject.active = true;
				textAttrValue[num].text = unitItem.ability.colltime + "sec";
				textAttrValue[num].gameObject.active = true;
				num++;
			}
		}
	}

	private int FindItemIndex(int code)
	{
		for (int i = 0; i < itemCodeList.Length; i++)
		{
			if (itemCodeList[i] == code)
			{
				return i;
			}
		}
		return -1;
	}

	public void Show()
	{
		LoadIcon();
		base.gameObject.SetActiveRecursively(true);
		AuiButton.topActive = true;
		if (isReset)
		{
			ResetIconList();
			RefreshItemList();
			RefreshItemDetail();
			RefreshPlayerInfo();
			RefreshPreview();
		}
		RefreshStatsAlarm();
		panelStatsUp.gameObject.SetActiveRecursively(false);
	}

	private void RefreshPreview()
	{
		if (thisUnit != null)
		{
			if (previewUnit == null)
			{
				GameObject gameObject = Object.Instantiate(ResourceManager.Load("Character/prefeb/character", "feb_hero01", typeof(GameObject))) as GameObject;
				gameObject.transform.parent = previewPanel.transform;
				gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
				gameObject.gameObject.AddComponent<AdjustAnimationSpeed>();
				previewUnit = gameObject.gameObject.AddComponent<HeroModel>();
				LayerManager.SetLayerAllChild(gameObject.transform, LayerManager.layerPreviewInven);
			}
			previewUnit.SetCloth(thisUnit.heroModel.headIndex, thisUnit.heroModel.topIndex, thisUnit.heroModel.bottomIndex);
			previewUnit.SetWeapon(thisUnit.weaponManager, thisUnit.weaponType, thisUnit.weaponCode);
			LayerManager.SetLayerAllChild(previewUnit.transform, LayerManager.layerPreviewInven);
		}
	}

	private void RefreshStatsAlarm()
	{
		bool flag = PlayInfo.heroState.statsPoint > 0;
		panelStatsAlarm.gameObject.SetActiveRecursively(flag);
		if (flag)
		{
			remainStats.text = PlayInfo.heroState.statsPoint.ToString();
			aniStatsAlarm.StartAnimation(true, false);
		}
	}

	public void Hide()
	{
		base.gameObject.SetActiveRecursively(false);
		AuiButton.topActive = false;
	}

	public void OnSlotClock(AuiButton sender)
	{
		itemScrollTop = sender.buttonTag * 8;
		selectedItem = null;
		RefreshItemList();
		RefreshItemDetail();
	}

	private void OnEquipItemClick(AuiButton sender)
	{
		int buttonTag = sender.buttonTag;
		UnitItem unitItem = PlayInfo.playerData.equipWeapon[buttonTag];
		if (unitItem != null)
		{
			PlayInfo.heroState.curWeaponSlot = 0;
			thisUnit.thisCtrl.SetWeapon(unitItem.weaponType, unitItem.code);
			RefreshPreview();
			RefreshPlayerInfo();
		}
	}

	public void OnEquipClick(AuiButton sender)
	{
		UnitItem unitItem = selectedItem;
		if (selectedItem == null)
		{
			return;
		}
		if (unitItem != null)
		{
			if (PlayInfo.heroState.level < unitItem.requireLevel)
			{
				ProcBase.ShowMsg(StringContent.msgDoNotEquip, MessageView.MsgIcon.alert);
				return;
			}
			switch (unitItem.type)
			{
			case ItemManager.ItemType.weapon:
				thisUnit.thisCtrl.SetWeapon(unitItem.weaponType, unitItem.code);
				RefreshPreview();
				switch (unitItem.weaponType)
				{
				case UnitCharactor.WeaponType.onehand:
					PlayInfo.playerData.equipWeapon[0] = unitItem;
					PlayInfo.heroState.curWeaponSlot = 0;
					break;
				case UnitCharactor.WeaponType.doublehand:
					PlayInfo.playerData.equipWeapon[1] = unitItem;
					PlayInfo.heroState.curWeaponSlot = 1;
					break;
				case UnitCharactor.WeaponType.bigsword:
					PlayInfo.playerData.equipWeapon[2] = unitItem;
					PlayInfo.heroState.curWeaponSlot = 2;
					break;
				}
				break;
			case ItemManager.ItemType.cloth:
				thisUnit.heroModel.SetClothPartFromCode(unitItem.clothPart, unitItem.code);
				RefreshPreview();
				PlayInfo.playerData.wearCloth[(int)unitItem.clothPart] = unitItem;
				break;
			case ItemManager.ItemType.misc:
				if (unitItem.miscType == ItemManager.MiscType.ring)
				{
					PlayInfo.playerData.slotRing = unitItem;
				}
				else if (unitItem.code == 401 || unitItem.code == 400)
				{
					PlayInfo.playerData.slotHp = PlayInfo.inventory.FindItem(unitItem.code);
				}
				else if (unitItem.code == 411 || unitItem.code == 410)
				{
					PlayInfo.playerData.slotMp = PlayInfo.inventory.FindItem(unitItem.code);
				}
				break;
			}
		}
		RefreshItemList();
		RefreshPlayerInfo();
		PlayInfo.soundManager.Play(ExtSoundManager.Effect2D.efs_item_equip, new Vector3(0f, 0f, 0f));
	}

	public void OnDeleteClick(AuiButton sender)
	{
		UnitItem unitItem = selectedItem;
		if (selectedItem == null)
		{
			return;
		}
		if (unitItem.code == 401 || unitItem.code == 411 || unitItem.code == 400 || unitItem.code == 410)
		{
			ProcBase.ShowMsg(StringContent.msgCanNotDeleteConsumableItem, MessageView.MsgIcon.alert);
			return;
		}
		for (int i = 0; i < PlayInfo.playerData.equipWeapon.Length; i++)
		{
			if (PlayInfo.playerData.equipWeapon[i] != null && PlayInfo.playerData.equipWeapon[i].code == unitItem.code)
			{
				ProcBase.ShowMsg(StringContent.msgCanNotDeleteEquipItem, MessageView.MsgIcon.alert);
				return;
			}
		}
		for (int j = 0; j < PlayInfo.playerData.wearCloth.Length; j++)
		{
			if (PlayInfo.playerData.wearCloth[j] != null && PlayInfo.playerData.wearCloth[j].code == unitItem.code)
			{
				ProcBase.ShowMsg(StringContent.msgCanNotDeleteEquipItem, MessageView.MsgIcon.alert);
				return;
			}
		}
		if (PlayInfo.playerData.slotRing != null && PlayInfo.playerData.slotRing.code == unitItem.code)
		{
			ProcBase.ShowMsg(StringContent.msgCanNotDeleteEquipItem, MessageView.MsgIcon.alert);
			return;
		}
		ProcBase.ShowMsg(StringContent.msgQuestionItemDelete.Replace(StringContent.strValue, unitItem.name), MessageView.MsgIcon.question, false, new MessageView.MsgButton[2]
		{
			MessageView.MsgButton.yes,
			MessageView.MsgButton.no
		}, OnDeleteSubmitClick);
	}

	public void OnDeleteSubmitClick(MessageView.MsgButton msgButton)
	{
		if (msgButton == MessageView.MsgButton.yes)
		{
			UnitItem item = selectedItem;
			PlayInfo.inventory.DeleteItem(item);
			isReset = false;
			ResetIconList();
			RefreshItemList();
			RefreshItemDetail();
			RefreshPlayerInfo();
			RefreshPreview();
		}
	}

	public void OnItemTypeClick(AuiButton sender)
	{
		selectedItemType = (ItemManager.ItemType)sender.buttonTag;
		itemScrollTop = 0;
		selectedItem = null;
		for (int i = 0; i < buttonItemType.Length; i++)
		{
			buttonItemType[i].enabled = sender.buttonTag != i;
			buttonItemType[i].SetFrame((sender.buttonTag == i) ? 1 : 0);
		}
		isReset = false;
		ResetIconList();
		RefreshItemList();
		RefreshItemDetail();
	}

	public void OnItemListClick(AuiButton sender)
	{
		UnitItem unitItem = PlayInfo.itemManager.FindItem(selectedItemType, sender.buttonTag);
		selectedItem = unitItem;
		RefreshItemList();
		RefreshItemDetail();
	}

	private void OnStatsUpOpenClick(AuiButton sender)
	{
		ShowStatsUp();
	}

	private void OnStatsUpCloseClick(AuiButton sender)
	{
		RefreshStatsAlarm();
		HideStatsUp();
	}

	private void OnStatsIncClick(AuiButton sender)
	{
		if (stateTemp.statsPoint > 0)
		{
			switch (sender.buttonTag)
			{
			case 0:
				stateTemp.strength += 1f;
				break;
			case 1:
				stateTemp.intellectual += 1f;
				break;
			case 2:
				stateTemp.constitution += 1f;
				break;
			}
			incStats[sender.buttonTag]++;
			stateTemp.statsPoint--;
			RefreshStatsTemp();
		}
	}

	private void OnStatsUpSubmitClick(AuiButton sender)
	{
		if (stateTemp.statsPoint != PlayInfo.heroState.statsPoint)
		{
			PlayInfo.heroState.strength = stateTemp.strength;
			PlayInfo.heroState.intellectual = stateTemp.intellectual;
			PlayInfo.heroState.constitution = stateTemp.constitution;
			PlayInfo.heroState.statsPoint = stateTemp.statsPoint;
		}
		RefreshStatsAlarm();
		HideStatsUp();
	}

	private void OnCloseClick(AuiButton sender)
	{
		Hide();
	}

	private void ShowStatsUp()
	{
		AuiButton.SetEnableAll(base.transform, false);
		buttonStatsUpClose.enabled = true;
		AuiButton[] array = buttonStatsInc;
		foreach (AuiButton auiButton in array)
		{
			auiButton.enabled = true;
		}
		buttonStatsUpSubmit.enabled = true;
		stateTemp = PlayInfo.heroState.Clone();
		stateTemp.playerData = PlayInfo.heroState.playerData;
		incStats = new int[textStatsInc.Length];
		for (int j = 0; j < incStats.Length; j++)
		{
			incStats[j] = 0;
		}
		RefreshStatsTemp();
		panelStatsUp.gameObject.SetActiveRecursively(true);
	}

	private void HideStatsUp()
	{
		AuiButton.SetEnableAll(base.transform, true);
		panelStatsUp.gameObject.SetActiveRecursively(false);
		RefreshPlayerInfo();
	}

	private void RefreshStatsTemp()
	{
		textAbility[0].text = stateTemp.fame.ToString();
		textAbility[1].text = stateTemp.sumAttack.ToString();
		textAbility[2].text = stateTemp.sumDefense.ToString();
		textAbility[3].text = stateTemp.sumStr.ToString();
		textAbility[4].text = stateTemp.sumInt.ToString();
		textAbility[5].text = stateTemp.sumCon.ToString();
		textAbility[6].text = stateTemp.sumCri.ToString();
		remainStats.text = stateTemp.statsPoint.ToString();
		for (int i = 0; i < textStatsInc.Length; i++)
		{
			textStatsInc[i].text = ((incStats[i] <= 0) ? string.Empty : ("+" + incStats[i]));
		}
	}

	private void Update()
	{
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
}
