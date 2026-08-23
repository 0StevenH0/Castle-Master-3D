using UnityEngine;

public class LordRow : MonoBehaviour
{
	public TextMesh textLevel;

	public AuiSprite iconLord;

	public TextMesh textName;

	public TextMesh textLoyalty;

	public TextMesh textFame;

	public TextMesh textAtk;

	public TextMesh textInt;

	public TextMesh textHp;

	public GameObject panelAppointed;

	public AuiButton buttonReward;

	public AuiSprite buttonRewardLabel;

	public AuiSprite iconCastle;

	public TextMesh textCastleName;

	protected LordManager.Lord curlord;

	public bool visible
	{
		get
		{
			return base.gameObject.active;
		}
		set
		{
			base.gameObject.SetActiveRecursively(value);
		}
	}

	private void Start()
	{
		if (buttonReward != null)
		{
			buttonReward.isTop = true;
		}
	}

	public void SetLord(LordManager.Lord revalue)
	{
		curlord = revalue;
		textLevel.text = curlord.level.ToString();
		iconLord.SetFrame((int)curlord.type);
		textName.text = curlord.type.ToString();
		textLoyalty.text = curlord.loyalty.ToString();
		textFame.text = curlord.fame.ToString();
		textAtk.text = curlord.atk.ToString();
		textInt.text = curlord.inte.ToString();
		textHp.text = curlord.hp.ToString();
		iconCastle.SetFrame(PlayInfo.castleManager.castle[curlord.castleIndex].level - 1);
		textCastleName.text = PlayInfo.castleManager.castle[curlord.castleIndex].castleName;
		buttonReward.visible = true;
		buttonRewardLabel.visible = true;
	}
}
