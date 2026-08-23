using System.Collections.Generic;
using UnityEngine;

public class BattleAI
{
	public class UnitData
	{
		public UnitControl unitCtl;

		public int areax;

		public int areaz;

		public int assistCount;
	}

	private const float maxDefenseLength = 50f;

	private CharactorManager charactorManager;

	private StageManager stageManager;

	private int sideAlly;

	private int sideEnemy;

	private UnitControl castleGate;

	private Vector3 gatePos = new Vector3(0f, 0f, 0f);

	public void Init(CharactorManager charManager, UnitControl gate, int battleSide, StageManager stage)
	{
		charactorManager = charManager;
		stageManager = stage;
		sideAlly = battleSide;
		sideEnemy = ((battleSide == 0) ? 1 : 0);
		castleGate = gate;
		gatePos = castleGate.thisTrans.position;
	}

	private void UpdateDefense()
	{
		int num = (int)(gatePos.x + 50f);
		List<UnitData> list = new List<UnitData>();
		List<UnitData> list2 = new List<UnitData>();
		List<UnitData> list3 = new List<UnitData>();
		int aliveUnit = stageManager.GetAliveUnit(sideEnemy);
		int aliveUnit2 = stageManager.GetAliveUnit(sideAlly);
		int num2 = 5;
		if (aliveUnit < aliveUnit2 / 4 || aliveUnit < 10)
		{
			num = 1000;
			num2 = 15;
		}
		UnitCharactor unitCharactor = null;
		foreach (UnitCharactor charactor in charactorManager.charactors)
		{
			if (charactor.thisCtrl == castleGate)
			{
				continue;
			}
			if (charactor.thisCtrl.isMainHero)
			{
				unitCharactor = charactor;
				continue;
			}
			UnitControl thisCtrl = charactor.thisCtrl;
			if (thisCtrl.battleSide == sideEnemy && thisCtrl.isAwake && !thisCtrl.isDie)
			{
				int num3 = (int)thisCtrl.thisTrans.position.x;
				if (num3 <= num)
				{
					UnitData unitData = new UnitData();
					unitData.unitCtl = thisCtrl;
					unitData.areax = num3;
					unitData.areaz = (int)thisCtrl.thisTrans.position.z;
					list.Add(unitData);
				}
			}
			if (thisCtrl.battleSide == sideAlly && thisCtrl.isAwake && !thisCtrl.isDie)
			{
				UnitData unitData2 = new UnitData();
				unitData2.unitCtl = thisCtrl;
				unitData2.areax = (int)thisCtrl.thisTrans.position.x;
				unitData2.areaz = (int)thisCtrl.thisTrans.position.z;
				if (thisCtrl.targetCtrl == null || thisCtrl.defenseAssist)
				{
					list2.Add(unitData2);
				}
				else
				{
					list3.Add(unitData2);
				}
			}
		}
		if (unitCharactor != null)
		{
			UnitControl thisCtrl2 = unitCharactor.thisCtrl;
			if (thisCtrl2.battleSide == sideEnemy && thisCtrl2.isAwake && !thisCtrl2.isDie)
			{
				int num4 = (int)thisCtrl2.thisTrans.position.x;
				if (num4 <= num)
				{
					UnitData unitData3 = new UnitData();
					unitData3.unitCtl = thisCtrl2;
					unitData3.areax = num4;
					unitData3.areaz = (int)thisCtrl2.thisTrans.position.z;
					list.Add(unitData3);
				}
			}
		}
		list.Sort((UnitData a, UnitData b) => a.areax.CompareTo(b.areax));
		foreach (UnitData item in list2)
		{
			if ((float)item.areax > (float)num - 5f && item.unitCtl.currentMotion == UnitControl.MotionType.idle)
			{
				item.unitCtl.ClearTarget();
				item.unitCtl.RunTo(new Vector3(item.unitCtl.thisTrans.position.x - 5f, 0f, item.unitCtl.thisTrans.position.z));
				item.unitCtl.defenseAssist = false;
			}
		}
		foreach (UnitData item2 in list)
		{
			UnitData unitData4 = null;
			int num5 = int.MaxValue;
			bool flag = false;
			foreach (UnitData item3 in list3)
			{
				if (!item3.unitCtl.defenseAssist && item3.unitCtl.targetCtrl == item2.unitCtl)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				continue;
			}
			foreach (UnitData item4 in list2)
			{
				int num6 = (item4.areax - item2.areax) * (item4.areax - item2.areax) + (item4.areaz - item2.areaz) * (item4.areaz - item2.areaz);
				if (num6 < num5)
				{
					unitData4 = item4;
					num5 = num6;
				}
			}
			if (unitData4 != null)
			{
				list2.Remove(unitData4);
				if (!(item2.unitCtl == unitData4.unitCtl.targetCtrl) || item2.unitCtl.unitIdentify != unitData4.unitCtl.targetIdentify)
				{
					unitData4.unitCtl.SetTarget(item2.unitCtl.thisChar, null);
					unitData4.unitCtl.defenseAssist = false;
					item2.assistCount = 0;
				}
			}
		}
		foreach (UnitData item5 in list2)
		{
			UnitData unitData5 = null;
			int num7 = int.MaxValue;
			foreach (UnitData item6 in list)
			{
				int num8 = (item5.areax - item6.areax) * (item5.areax - item6.areax) + (item5.areaz - item6.areaz) * (item5.areaz - item6.areaz);
				if (num8 < num7)
				{
					num7 = num8;
					unitData5 = item6;
				}
			}
			if (unitData5 != null)
			{
				unitData5.assistCount++;
				item5.unitCtl.SetTarget(unitData5.unitCtl.thisChar, null);
				item5.unitCtl.defenseAssist = true;
				if (unitData5.assistCount > num2)
				{
					list.Remove(unitData5);
				}
			}
		}
	}

	public void UpdateAttack()
	{
		int num = (int)(gatePos.x + 50f);
		List<UnitData> list = new List<UnitData>();
		List<UnitData> list2 = new List<UnitData>();
		foreach (UnitCharactor charactor in charactorManager.charactors)
		{
			if (charactor.thisCtrl == castleGate)
			{
				continue;
			}
			UnitControl thisCtrl = charactor.thisCtrl;
			if (thisCtrl.battleSide == sideEnemy)
			{
				if (thisCtrl.isAwake && !thisCtrl.isDie)
				{
					UnitData unitData = new UnitData();
					unitData.unitCtl = thisCtrl;
					unitData.areax = (int)thisCtrl.thisTrans.position.x;
					unitData.areaz = (int)thisCtrl.thisTrans.position.z;
					unitData.assistCount = 0;
					list2.Add(unitData);
				}
			}
			else if (!charactor.thisCtrl.isMainHero && thisCtrl.isAwake && !thisCtrl.isDie)
			{
				UnitData unitData2 = new UnitData();
				unitData2.unitCtl = thisCtrl;
				unitData2.areax = (int)thisCtrl.thisTrans.position.x;
				unitData2.areaz = (int)thisCtrl.thisTrans.position.z;
				list.Add(unitData2);
			}
		}
		List<UnitData> list3 = new List<UnitData>();
		foreach (UnitData item in list)
		{
			if (!(item.unitCtl.targetCtrl != null))
			{
				continue;
			}
			foreach (UnitData item2 in list2)
			{
				if (item.unitCtl.targetCtrl == item2.unitCtl && item2.assistCount < 3)
				{
					item2.assistCount++;
					if (item2.assistCount > 2)
					{
						list3.Add(item2);
					}
				}
			}
		}
		foreach (UnitData item3 in list3)
		{
			list2.Remove(item3);
		}
		foreach (UnitCharactor charactor2 in charactorManager.charactors)
		{
			if (charactor2.thisCtrl == castleGate || charactor2.thisCtrl.isMainHero)
			{
				continue;
			}
			UnitControl thisCtrl2 = charactor2.thisCtrl;
			if (thisCtrl2.battleSide != sideAlly || !thisCtrl2.isAwake || thisCtrl2.isDie)
			{
				continue;
			}
			bool flag = thisCtrl2.targetCtrl == null;
			if (thisCtrl2.targetCtrl != null && thisCtrl2.targetCtrl.thisChar.isGate)
			{
				flag = true;
			}
			if (!flag)
			{
				continue;
			}
			int num2 = int.MaxValue;
			UnitData unitData3 = null;
			int num3 = (int)thisCtrl2.thisTrans.position.x;
			int num4 = (int)thisCtrl2.thisTrans.position.z;
			foreach (UnitData item4 in list2)
			{
				int num5 = (num3 - item4.areax) * (num3 - item4.areax) + (num4 - item4.areaz) * (num4 - item4.areaz);
				if (num5 <= 20 && num2 > num5)
				{
					num2 = num5;
					unitData3 = item4;
				}
			}
			if (unitData3 != null)
			{
				thisCtrl2.SetTarget(unitData3.unitCtl.thisChar, null);
				list2.Remove(unitData3);
			}
			else if (thisCtrl2.thisTrans.position.x > (float)num)
			{
				if (thisCtrl2.currentMotion != UnitControl.MotionType.move)
				{
					Vector3 position = thisCtrl2.thisTrans.position;
					position.x = (float)num - 10f;
					thisCtrl2.RunTo(position);
				}
			}
			else
			{
				thisCtrl2.SetTarget(castleGate.thisChar, null);
			}
		}
	}

	public void Update()
	{
		if (sideAlly == 1)
		{
			UpdateDefense();
		}
		else
		{
			UpdateAttack();
		}
	}
}
