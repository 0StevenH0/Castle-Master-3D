public class UnitItem
{
	public ItemManager.ItemType type;

	public string name;

	public UnitCharactor.WeaponType weaponType;

	public HeroModel.ClothPart clothPart;

	public ItemManager.MiscType miscType;

	public int code;

	public int requireLevel;

	public int costGold;

	public int costGem;

	public int sellGold;

	public Ability ability;

	public int quantity = 1;

	public UnitItem Clone()
	{
		UnitItem unitItem = new UnitItem();
		unitItem.type = type;
		unitItem.name = name;
		unitItem.weaponType = weaponType;
		unitItem.clothPart = clothPart;
		unitItem.miscType = miscType;
		unitItem.code = code;
		unitItem.requireLevel = requireLevel;
		unitItem.costGold = costGold;
		unitItem.costGem = costGem;
		unitItem.sellGold = costGem;
		if (ability != null)
		{
			unitItem.ability = ability.Clone();
		}
		return unitItem;
	}
}
