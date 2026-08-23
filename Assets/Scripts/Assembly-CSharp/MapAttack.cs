using UnityEngine;

public class MapAttack : MonoBehaviour
{
	private const int maxScroll = 214;

	private const int minScroll = 35;

	private const int sizeScroll = 179;

	public AuiButton buttonSubmit;

	public AuiButton buttonCancel;

	public TextMesh castleNameFrom;

	public TextMesh castleNameTo;

	public TextMesh commandPtr;

	public TextMesh maxUnit;

	public TextMesh curUnit;

	public TextMesh labelMaxUnit;

	public TextMesh labelCurUnit;

	public AttackUnit[] objUnit;

	public UIMap uiMap;

	private int castleIndex;

	private int targetCastleIndex;

	private int selectUnit = -1;

	private bool isScrolling;

	private Vector3 scrollStartPos;

	private Vector3 scrollObjectPos;

	private int[] attackUnitCount = new int[5];

	private void Start()
	{
		buttonSubmit.isTop = true;
		buttonCancel.isTop = true;
		buttonSubmit.onButtonClick = OnSubmitClick;
		buttonCancel.onButtonClick = OnCancelClick;
		for (int i = 0; i < 5; i++)
		{
			objUnit[i].buttonCount.buttonTag = i;
			objUnit[i].buttonCount.onButtonClick = OnUnitScrollClick;
			objUnit[i].buttonCount.isTop = true;
		}
		maxUnit.text = 300.ToString();
		labelMaxUnit.text = StringContent.wordMaxUnits;
		labelCurUnit.text = StringContent.wordCurrentUnits;
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	private int GetTotalUnit()
	{
		int num = 0;
		for (int i = 0; i < 5; i++)
		{
			num += attackUnitCount[i];
		}
		return num;
	}

	private void OnSubmitClick(AuiButton sender)
	{
		if (PlayInfo.castleAI.attackCurrent.attackActive)
		{
			ProcBase.ShowMsg(StringContent.msgFirstNeedDefense.Replace(StringContent.strValue, PlayInfo.castleAI.attackCurrent.attackTo.castleName), MessageView.MsgIcon.military);
			return;
		}
		int totalUnit = GetTotalUnit();
		if (totalUnit <= 0)
		{
			ProcBase.ShowMsg(StringContent.msgNoSelectAttackUnit, MessageView.MsgIcon.military);
			return;
		}
		if (totalUnit > 300)
		{
			ProcBase.ShowMsg(StringContent.msgAttackExceedMaxLimit, MessageView.MsgIcon.military);
			return;
		}
		if (PlayInfo.playerData.cmdPts < PlayInfo.gameRule.cmdPtsAttack)
		{
			ProcBase.ShowMsg(StringContent.msgNotEnoughCmdPts, MessageView.MsgIcon.question, false, new MessageView.MsgButton[2]
			{
				MessageView.MsgButton.yes,
				MessageView.MsgButton.no
			}, OnCommandPointMessage);
			return;
		}
		PlayInfo.playerData.cmdPts -= PlayInfo.gameRule.cmdPtsAttack;
		if (PlayInfo.playerData.cmdPts < 0)
		{
			PlayInfo.playerData.cmdPts = 0;
		}
		PlayInfo.battleInfo = new PlayInfo.BattleInfo();
		PlayInfo.battleInfo.attackCastle = PlayInfo.castleManager.castle[castleIndex];
		PlayInfo.battleInfo.attackUnits = new int[5];
		for (int i = 0; i < 5; i++)
		{
			PlayInfo.battleInfo.attackUnits[i] = attackUnitCount[i];
			PlayInfo.battleInfo.attackCastle.unitCount[i] -= PlayInfo.battleInfo.attackUnits[i];
		}
		PlayInfo.battleInfo.defenseCastle = PlayInfo.castleManager.castle[targetCastleIndex];
		PlayInfo.battleInfo.defenseUnits = new int[5];
		for (int j = 0; j < 5; j++)
		{
			PlayInfo.battleInfo.defenseUnits[j] = PlayInfo.battleInfo.defenseCastle.unitCount[j];
			PlayInfo.battleInfo.defenseCastle.unitCount[j] -= PlayInfo.battleInfo.defenseUnits[j];
		}
		PlayInfo.battleInfo.isAttack = true;
		Hide();
		ProcMain.playStage = StageManager.StageType.battle;
		ProcBase.LoadScene("Scene Main", ProcLoading.ContentMode.battle);
	}

	private void OnCommandPointMessage(MessageView.MsgButton button)
	{
		if (button == MessageView.MsgButton.yes)
		{
			uiMap.OnCommandPointClick(null);
		}
	}

	private void OnCancelClick(AuiButton sender)
	{
		uiMap.DisableCastleTargetMode();
		Hide();
	}

	private void OnUnitScrollClick(AuiButton sender)
	{
		selectUnit = sender.buttonTag;
		scrollObjectPos = objUnit[selectUnit].buttonCount.transform.localPosition;
	}

	private void Update()
	{
		if (AuiButton.modalActive || AuiButton.mostTopActive)
		{
			return;
		}
		if (Input.GetMouseButtonDown(0))
		{
			isScrolling = true;
			Vector3 mousePosition = Input.mousePosition;
			scrollStartPos = buttonSubmit.uiCamera.ScreenToWorldPoint(mousePosition);
		}
		else if (Input.GetMouseButtonUp(0))
		{
			isScrolling = false;
			selectUnit = -1;
		}
		if (!isScrolling || selectUnit <= -1)
		{
			return;
		}
		Vector3 mousePosition2 = Input.mousePosition;
		mousePosition2 = buttonSubmit.uiCamera.ScreenToWorldPoint(mousePosition2);
		if (mousePosition2.x != scrollStartPos.x)
		{
			float num = mousePosition2.x - scrollStartPos.x;
			Vector3 localPosition = scrollObjectPos;
			localPosition.x += num;
			if (localPosition.x > 214f)
			{
				localPosition.x = 214f;
			}
			if (localPosition.x < 35f)
			{
				localPosition.x = 35f;
			}
			CastleInfo castleInfo = PlayInfo.castleManager.castle[castleIndex];
			int num2 = (int)((localPosition.x - 35f) / 179f * (float)(castleInfo.unitCount[selectUnit] + 1));
			if (num2 > castleInfo.unitCount[selectUnit])
			{
				num2 = castleInfo.unitCount[selectUnit];
			}
			objUnit[selectUnit].addCount.text = num2.ToString();
			attackUnitCount[selectUnit] = num2;
			objUnit[selectUnit].buttonCount.transform.localPosition = localPosition;
			curUnit.text = GetTotalUnit().ToString();
		}
	}

	private void SetScrollPos(int selectUnit)
	{
		CastleInfo castleInfo = PlayInfo.castleManager.castle[castleIndex];
		Vector3 localPosition = objUnit[selectUnit].buttonCount.transform.localPosition;
		localPosition.x = 35f + (float)attackUnitCount[selectUnit] * 179f / (float)castleInfo.unitCount[selectUnit];
		objUnit[selectUnit].buttonCount.transform.localPosition = localPosition;
	}

	public void Hide()
	{
		base.gameObject.SetActiveRecursively(false);
		AuiButton.topActive = false;
	}

	public void Show(int castleIndex, int targetCastle)
	{
		this.castleIndex = castleIndex;
		targetCastleIndex = targetCastle;
		uiMap.DisableCastleTargetMode();
		base.gameObject.SetActiveRecursively(true);
		AuiButton.topActive = true;
		CastleInfo castleInfo = PlayInfo.castleManager.castle[castleIndex];
		castleNameFrom.text = castleInfo.castleName;
		castleNameTo.text = PlayInfo.castleManager.castle[targetCastle].castleName;
		int num = 0;
		for (int i = 0; i < 5; i++)
		{
			num += castleInfo.unitCount[i];
		}
		for (int num2 = 4; num2 >= 0; num2--)
		{
			int num3 = castleInfo.unitCount[num2];
			if (num3 > num)
			{
				num3 = num;
			}
			num -= num3;
			attackUnitCount[num2] = num3;
		}
		for (int j = 0; j < 5; j++)
		{
			bool flag = castleInfo.unitCount[j] > 0;
			objUnit[j].unitBlank.SetFrame(j);
			objUnit[j].unitBlank.visible = !flag;
			objUnit[j].unitIcon.visible = flag;
			objUnit[j].unitLevel.gameObject.active = flag;
			objUnit[j].unitCount.gameObject.active = flag;
			objUnit[j].buttonCount.visible = flag;
			if (flag)
			{
				if (castleInfo.side == 0)
				{
					objUnit[j].unitIcon.SetFrame(j);
					objUnit[j].unitLevel.text = PlayInfo.humanMilitary.GetUnitState(j).level.ToString();
				}
				else
				{
					objUnit[j].unitIcon.SetFrame(5 + castleInfo.monsterCode[j] - 201);
					objUnit[j].unitLevel.text = PlayInfo.monsterMilitary.GetUnitStateFromCode(castleInfo.monsterCode[j]).level.ToString();
				}
				objUnit[j].unitCount.text = castleInfo.unitCount[j].ToString();
				objUnit[j].addCount.text = attackUnitCount[j].ToString();
				SetScrollPos(j);
			}
		}
		curUnit.text = GetTotalUnit().ToString();
		commandPtr.text = PlayInfo.gameRule.cmdPtsAttack.ToString();
	}
}
