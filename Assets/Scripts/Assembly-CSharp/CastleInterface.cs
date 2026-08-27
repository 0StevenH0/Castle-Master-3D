using UnityEngine;

public class CastleInterface : MonoBehaviour
{
	public int castleIndex;

	public AuiSprite castleIcon;

	public Transform attackArrow;

	public Transform[] redeployArrow;

	public Transform aniRedeploy;

	public AuiSprite[] aniRedeployProgress;

	public AuiSprite iconIsUpgrade;

	public AuiSprite iconIsRecruit;

	public AuiSprite iconIsConstruct;

	public AuiSprite iconCastleName;

	public AuiSprite iconIsLord;

	public AuiSpriteAnimation aniAttackSource;

	public AuiSpriteAnimation aniRedeployTarget;

	public AuiSpriteAnimation[] aniSelected;

	public MapCastleDetail popupCastleDetail;

	public MapAttack popupAttack;

	public MapRedeploy popupRedeploy;

	public MapCancel popupCancel;

	public bool isSelectingAttack;

	public bool isSelectingRedeploy;

	public bool isRedeploySource;

	private int sourceCastle;

	private int targetCastle;

	private int maxRedeployArrow = 6;

	private CastleInfo info;

	private void Start()
	{
		info = PlayInfo.castleManager.castle[castleIndex];
		maxRedeployArrow = redeployArrow.Length;
		iconCastleName.SetFrame(info.index);
		HideAll();
	}

	public void CastleSelected()
	{
		int side = PlayInfo.castleManager.castle[castleIndex].side;
		if (isSelectingAttack)
		{
			if (PlayInfo.castleAI.attackCurrent.attackActive)
			{
				ProcBase.ShowMsg(StringContent.msgFirstNeedDefense.Replace(StringContent.strValue, PlayInfo.castleAI.attackCurrent.attackTo.castleName), MessageView.MsgIcon.military);
				return;
			}
			popupAttack.Show(castleIndex, targetCastle);
		}
		else if (isSelectingRedeploy)
		{
			popupRedeploy.Show(sourceCastle, castleIndex);
		}
		else
		{
			if (popupCancel.gameObject.activeInHierarchy)
			{
				return;
			}
			popupCastleDetail.Show(castleIndex);
		}
		aniSelected[side].visible = true;
		aniSelected[side].StartAnimation(0, false, false);
	}

	public void SetAttackTarget(int targetCastle)
	{
		CastleInfo castleInfo = PlayInfo.castleManager.castle[castleIndex];
		CastleInfo castleInfo2 = PlayInfo.castleManager.castle[targetCastle];
		Vector3 vector = new Vector3(castleInfo.posx, castleInfo.posy, 0f);
		Quaternion localRotation = Quaternion.Euler(Quaternion.FromToRotation(toDirection: new Vector3(castleInfo2.posx, castleInfo2.posy, 0f) - vector, fromDirection: new Vector3(0f, 1f, 0f)).eulerAngles + new Vector3(0f, 0f, 90f));
		attackArrow.localRotation = localRotation;
		attackArrow.gameObject.SetActiveRecursive(true);
		aniAttackSource.visible = true;
		aniAttackSource.StartAnimation(true, false);
		isSelectingAttack = true;
		popupCancel.Show(MapCancel.SelectingMode.attack);
		this.targetCastle = targetCastle;
	}

	public void HideAttackTarget()
	{
		attackArrow.gameObject.SetActiveRecursive(false);
		aniAttackSource.visible = false;
		isSelectingAttack = false;
	}

	public void SetRedeploySource(int[] targetCastles)
	{
		if (!info.isRedeploy)
		{
			int num = targetCastles.Length;
			if (num > maxRedeployArrow)
			{
				num = maxRedeployArrow;
			}
			for (int i = 0; i < num; i++)
			{
				CastleInfo castleInfo = PlayInfo.castleManager.castle[targetCastles[i]];
				Vector3 vector = new Vector3(info.posx, info.posy, 0f);
				Quaternion localRotation = Quaternion.Euler(Quaternion.FromToRotation(toDirection: new Vector3(castleInfo.posx, castleInfo.posy, 0f) - vector, fromDirection: new Vector3(0f, 1f, 0f)).eulerAngles + new Vector3(0f, 0f, 90f));
				redeployArrow[i].localPosition = new Vector3(0f, 0f, 0f);
				redeployArrow[i].localRotation = localRotation;
				redeployArrow[i].gameObject.SetActiveRecursive(true);
				popupCastleDetail.castles[targetCastles[i]].aniRedeployTarget.visible = true;
				popupCastleDetail.castles[targetCastles[i]].aniRedeployTarget.StartAnimation(true, false);
				isRedeploySource = true;
			}
		}
	}

	public void HideRedeploySource()
	{
		if (redeployArrow != null)
		{
			for (int i = 0; i < maxRedeployArrow; i++)
			{
				redeployArrow[i].gameObject.SetActiveRecursive(false);
			}
		}
		aniRedeployTarget.visible = false;
		isRedeploySource = false;
		isSelectingRedeploy = false;
	}

	public void HideRedeploying()
	{
		aniRedeploy.gameObject.SetActiveRecursive(false);
		isRedeploySource = false;
	}

	public void HideStatusPoint()
	{
		iconIsConstruct.visible = false;
		iconIsRecruit.visible = false;
		iconIsUpgrade.visible = false;
	}

	public void HideSelectIcon()
	{
		AuiSpriteAnimation[] array = aniSelected;
		foreach (AuiSprite auiSprite in array)
		{
			auiSprite.visible = false;
		}
	}

	public void HideAll()
	{
		HideAttackTarget();
		HideRedeploySource();
		HideRedeploying();
		HideStatusPoint();
		HideSelectIcon();
	}

	public void SetRedeployTarget(int sourceCastle)
	{
		isSelectingRedeploy = true;
		this.sourceCastle = sourceCastle;
		popupCancel.Show(MapCancel.SelectingMode.redeploy);
	}

	public void Refresh()
	{
		info = PlayInfo.castleManager.castle[castleIndex];
		if (info.isRedeploy)
		{
			CastleInfo castleInfo = PlayInfo.castleManager.castle[info.redeployTargetIndex];
			if (!aniRedeploy.gameObject.activeInHierarchy)
			{
				aniRedeploy.gameObject.SetActiveRecursive(true);
				Vector3 vector = new Vector3(info.posx, info.posy, 0f);
				Quaternion localRotation = Quaternion.Euler(Quaternion.FromToRotation(toDirection: new Vector3(castleInfo.posx, castleInfo.posy, 0f) - vector, fromDirection: new Vector3(0f, 1f, 0f)).eulerAngles + new Vector3(0f, 0f, 90f));
				aniRedeploy.localPosition = new Vector3(0f, 0f, 0f);
				aniRedeploy.localRotation = localRotation;
				for (int i = 0; i < aniRedeployProgress.Length; i++)
				{
					aniRedeployProgress[i].SetFrame(0);
				}
			}
			float num = 1f - info.redeployHour / (PlayInfo.gameRule.redeploySoldierDays * 24f);
			int num2 = (int)((float)aniRedeployProgress.Length * num) + 1;
			if (num2 < 1)
			{
				num2 = 1;
			}
			if (num2 > aniRedeployProgress.Length)
			{
				num2 = aniRedeployProgress.Length;
			}
			for (int j = 0; j < num2; j++)
			{
				aniRedeployProgress[j].SetFrame(1);
			}
		}
		else if (aniRedeploy.gameObject.activeInHierarchy)
		{
			aniRedeploy.gameObject.SetActiveRecursive(false);
		}
		int frame = info.side * 3 + info.level - 1;
		castleIcon.SetFrame(frame);
		if (!isSelectingAttack)
		{
			HideAttackTarget();
		}
		if (!info.isRedeploy && !isRedeploySource && !isSelectingRedeploy)
		{
			HideRedeploySource();
		}
		if (popupCastleDetail.castleIndex != castleIndex)
		{
			HideSelectIcon();
		}
		iconIsUpgrade.visible = info.isUpgrading;
		iconIsRecruit.visible = info.isRecruitSoldier;
		iconIsLord.visible = info.lord != null;
		bool visible = false;
		CastleInfo.BuildingStatus[] buildingStatus = info.buildingStatus;
		foreach (CastleInfo.BuildingStatus buildingStatus2 in buildingStatus)
		{
			if (buildingStatus2 == CastleInfo.BuildingStatus.constructing)
			{
				visible = true;
				break;
			}
		}
		iconIsConstruct.visible = visible;
	}
}
