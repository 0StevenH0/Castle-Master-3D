using System.Collections.Generic;
using UnityEngine;

public class CastleAI
{
	private class AICastle
	{
		public int side;

		public CastleInfo castle;

		public int attackSum;

		public int nearAttackSum;

		public List<AICastle> near = new List<AICastle>();

		public List<AICastle> nearAlly = new List<AICastle>();

		public List<AICastle> nearEnemy = new List<AICastle>();

		public bool allowBattle;
	}

	public class AIAttack
	{
		public bool attackActive;

		public CastleInfo attackFrom;

		public CastleInfo attackTo;

		public void Reset()
		{
			attackActive = false;
			attackFrom = null;
			attackTo = null;
		}
	}

	private const int minRedeployDays = 1;

	private const int maxRedeployDays = 8;

	private static float[] unitDiv = new float[5] { 0.4f, 0.3f, 0.15f, 0.1f, 0.05f };

	private int sideAI = 1;

	private List<AICastle> aiCastles = new List<AICastle>();

	private int redeployDay;

	private int attackDay;

	public AIAttack attackReserve = new AIAttack();

	public AIAttack attackCurrent = new AIAttack();

	public int attackCountDown;

	public void Init()
	{
		CastleInfo[] castle = PlayInfo.castleManager.castle;
		foreach (CastleInfo castleInfo in castle)
		{
			AICastle aICastle = new AICastle();
			aICastle.side = castleInfo.side;
			aICastle.castle = castleInfo;
			aICastle.attackSum = 0;
			aiCastles.Add(aICastle);
		}
		foreach (AICastle aiCastle in aiCastles)
		{
			foreach (CastleInfo item in aiCastle.castle.nearCastle)
			{
				aiCastle.near.Add(aiCastles[item.index]);
			}
		}
		redeployDay = Random.Range(1, 9);
	}

	public void Reset()
	{
		foreach (AICastle aiCastle in aiCastles)
		{
			aiCastle.side = aiCastle.castle.side;
			aiCastle.nearAlly.Clear();
			aiCastle.nearEnemy.Clear();
			aiCastle.attackSum = 0;
			aiCastle.allowBattle = false;
			if (aiCastle.side != sideAI)
			{
				continue;
			}
			foreach (AICastle item in aiCastle.near)
			{
				if (item.castle.side == sideAI)
				{
					aiCastle.nearAlly.Add(item);
				}
				else
				{
					aiCastle.nearEnemy.Add(item);
				}
			}
			aiCastle.allowBattle = aiCastle.nearEnemy.Count > 0;
		}
	}

	private void ComputeRedeploy()
	{
		foreach (AICastle aiCastle in aiCastles)
		{
			if (aiCastle.side != sideAI || !aiCastle.allowBattle || Random.Range(0, 100) < 50)
			{
				continue;
			}
			int currentUnitTotal = aiCastle.castle.GetCurrentUnitTotal();
			int num = 0;
			foreach (AICastle item in aiCastle.nearEnemy)
			{
				if (item.castle.side != sideAI)
				{
					int currentUnitTotal2 = item.castle.GetCurrentUnitTotal();
					if (num < currentUnitTotal2)
					{
						num = currentUnitTotal2;
					}
				}
			}
			num = (int)((float)num * 0.8f);
			if (currentUnitTotal >= num)
			{
				continue;
			}
			int num2 = (int)((float)currentUnitTotal * 0.2f);
			if (num2 < 10)
			{
				num2 = 10;
			}
			if (num2 + currentUnitTotal > CastleInfo.levelDefault[aiCastle.castle.level - 1].maxUnit)
			{
				num2 = CastleInfo.levelDefault[aiCastle.castle.level - 1].maxUnit - currentUnitTotal;
			}
			if (num2 <= 0)
			{
				continue;
			}
			for (int i = 0; i < 5; i++)
			{
				int num3 = (int)((float)num2 * unitDiv[i]);
				if (num3 > 0)
				{
					aiCastle.castle.unitCount[i] += num3;
				}
			}
			if (aiCastle.castle.index == PlayInfo.castleManager.castle.Length - 1 && aiCastle.castle.unitCount[4] > 1)
			{
				aiCastle.castle.unitCount[4] = 1;
			}
		}
		redeployDay = Random.Range(1, 9);
	}

	private int GetAttackSum(CastleInfo castle)
	{
		int num = 0;
		if (castle.side == 0)
		{
			for (int i = 0; i < 5; i++)
			{
				num += PlayInfo.humanMilitary.GetUnitState(i).sumAttack * castle.unitCount[i];
			}
			if (castle.lord != null)
			{
				num += castle.lord.atk * 2;
			}
			num += PlayInfo.heroState.sumAttack * 5;
		}
		else
		{
			for (int j = 0; j < 5; j++)
			{
				num += PlayInfo.monsterMilitary.GetUnitStateFromCode(castle.monsterCode[j]).sumAttack * castle.unitCount[j];
			}
		}
		return num;
	}

	private void ComputeAttack()
	{
		if (attackReserve.attackActive || attackCurrent.attackActive || PlayInfo.heroState.level < 5 || PlayInfo.castleManager.GetCastleCount(1) < 2)
		{
			return;
		}
		foreach (AICastle aiCastle2 in aiCastles)
		{
			aiCastle2.attackSum = GetAttackSum(aiCastle2.castle);
		}
		List<AICastle> list = new List<AICastle>();
		List<AICastle> list2 = new List<AICastle>();
		foreach (AICastle aiCastle in aiCastles)
		{
			if (aiCastle.side != 1 || aiCastle.nearEnemy.Count <= 0)
			{
				continue;
			}
			aiCastle.nearAttackSum = 0;
			foreach (AICastle human in aiCastle.nearEnemy)
			{
				aiCastle.nearAttackSum += human.attackSum;
				if (aiCastle.attackSum > human.attackSum * 3)
				{
					AICastle aICastle = list2.Find((AICastle find) => human.castle.index == find.castle.index);
					if (aICastle == null)
					{
						list2.Add(human);
					}
					aICastle = list2.Find((AICastle find) => aiCastle.castle.index == find.castle.index);
					if (aICastle == null)
					{
						list.Add(aiCastle);
					}
				}
			}
		}
		AICastle aICastle2 = null;
		foreach (AICastle item in list)
		{
			if (aICastle2 == null)
			{
				aICastle2 = item;
			}
			else if (aICastle2.nearAttackSum > item.nearAttackSum)
			{
				aICastle2 = item;
			}
		}
		AICastle aICastle3 = null;
		foreach (AICastle item2 in list2)
		{
			if (aICastle3 == null)
			{
				aICastle3 = item2;
			}
			else if (aICastle3.attackSum > item2.attackSum)
			{
				aICastle3 = item2;
			}
		}
		AICastle aICastle4 = null;
		if (aICastle3 != null)
		{
			foreach (AICastle item3 in aICastle3.nearEnemy)
			{
				if (aICastle4 == null)
				{
					aICastle4 = item3;
				}
				else if (aICastle4.attackSum < item3.attackSum)
				{
					aICastle4 = item3;
				}
			}
		}
		AICastle aICastle5 = null;
		if (aICastle4 != null && aICastle2 != null)
		{
			aICastle5 = ((aICastle4.attackSum <= aICastle2.attackSum) ? aICastle2 : aICastle4);
		}
		else if (aICastle4 != null)
		{
			aICastle5 = aICastle4;
		}
		else if (aICastle2 != null)
		{
			aICastle5 = aICastle2;
		}
		if (aICastle5 != null)
		{
			AICastle aICastle6 = null;
			foreach (AICastle item4 in aICastle5.nearEnemy)
			{
				if (aICastle6 == null)
				{
					aICastle6 = item4;
				}
				else if (aICastle6.attackSum > item4.attackSum)
				{
					aICastle6 = item4;
				}
				else if (aICastle6.attackSum == item4.attackSum && Random.Range(0, 2) == 1)
				{
					aICastle6 = item4;
				}
			}
			if (aICastle6 != null)
			{
				attackReserve.attackActive = true;
				attackReserve.attackFrom = aICastle5.castle;
				attackReserve.attackTo = aICastle6.castle;
			}
		}
		if (aICastle5 != null || PlayInfo.castleManager.GetCastleCount(1) <= 3)
		{
			return;
		}
		int num = Random.Range(0, 150);
		if (num >= 10)
		{
			return;
		}
		int num2 = 0;
		foreach (AICastle aiCastle3 in aiCastles)
		{
			if (aiCastle3.side == 1 && aiCastle3.nearEnemy.Count > 0)
			{
				int currentUnitTotal = aiCastle3.castle.GetCurrentUnitTotal();
				if (num2 < currentUnitTotal)
				{
					num2 = currentUnitTotal;
					aICastle5 = aiCastle3;
				}
			}
		}
		AICastle aICastle7 = null;
		if (aICastle5 != null)
		{
			num2 = int.MaxValue;
			foreach (AICastle item5 in aICastle5.nearEnemy)
			{
				if (item5.side == 0)
				{
					int currentUnitTotal2 = item5.castle.GetCurrentUnitTotal();
					if (num2 > currentUnitTotal2)
					{
						num2 = currentUnitTotal2;
						aICastle7 = item5;
					}
				}
			}
		}
		if (aICastle5 != null && aICastle7 != null)
		{
			attackReserve.attackActive = true;
			attackReserve.attackFrom = aICastle5.castle;
			attackReserve.attackTo = aICastle7.castle;
		}
	}

	public void SetBattleUnitCount()
	{
		PlayInfo.battleInfo = new PlayInfo.BattleInfo();
		PlayInfo.battleInfo.attackCastle = PlayInfo.castleAI.attackCurrent.attackFrom;
		PlayInfo.battleInfo.defenseCastle = PlayInfo.castleAI.attackCurrent.attackTo;
		PlayInfo.battleInfo.attackUnits = new int[5];
		PlayInfo.battleInfo.defenseUnits = new int[5];
		int num = 0;
		foreach (CastleInfo item in PlayInfo.battleInfo.attackCastle.nearCastle)
		{
			if (item.side == 0)
			{
				int attackSum = GetAttackSum(item);
				if (attackSum > num)
				{
					num = attackSum;
				}
			}
		}
		int attackSum2 = GetAttackSum(PlayInfo.battleInfo.attackCastle);
		int attackSum3 = GetAttackSum(PlayInfo.battleInfo.defenseCastle);
		int num2 = attackSum3 * 2;
		if (attackSum2 > attackSum3 * 3)
		{
			num2 = ((attackSum2 - attackSum3 * 2 >= num * 2) ? (attackSum2 - num * 2) : (attackSum3 * 2));
			if (num2 < attackSum3 * 2)
			{
				num2 = attackSum3 * 2;
			}
		}
		if (Random.Range(0, 100) < 30)
		{
			num2 = (int)((float)num2 * Random.Range(1.1f, 2f));
		}
		if (num2 > attackSum2)
		{
			num2 = attackSum2;
		}
		int num3 = 0;
		for (int i = 0; i < 5; i++)
		{
			PlayInfo.battleInfo.attackUnits[i] = (int)Mathf.Ceil((float)PlayInfo.battleInfo.attackCastle.unitCount[i] * (float)num2 / (float)attackSum2);
			if (PlayInfo.battleInfo.attackUnits[i] > PlayInfo.battleInfo.attackCastle.unitCount[i])
			{
				PlayInfo.battleInfo.attackUnits[i] = PlayInfo.battleInfo.attackCastle.unitCount[i];
			}
			if (PlayInfo.battleInfo.attackUnits[i] < 0)
			{
				PlayInfo.battleInfo.attackUnits[i] = 0;
			}
			num3 += PlayInfo.battleInfo.attackUnits[i];
		}
		if (PlayInfo.battleInfo.attackCastle.index >= PlayInfo.castleManager.castle.Length - 1)
		{
			num3 -= PlayInfo.battleInfo.attackUnits[4];
			PlayInfo.battleInfo.attackUnits[4] = 0;
		}
		if (num3 > 300)
		{
			int num4 = num3 - 300;
			for (int j = 0; j < 5; j++)
			{
				if (num4 > PlayInfo.battleInfo.attackUnits[j])
				{
					num4 -= PlayInfo.battleInfo.attackUnits[j];
					PlayInfo.battleInfo.attackUnits[j] = 0;
				}
				else
				{
					PlayInfo.battleInfo.attackUnits[j] -= num4;
					num4 = 0;
				}
				if (num4 <= 0)
				{
					break;
				}
			}
		}
		for (int k = 0; k < 5; k++)
		{
			PlayInfo.battleInfo.attackCastle.unitCount[k] -= PlayInfo.battleInfo.attackUnits[k];
			if (PlayInfo.battleInfo.attackCastle.unitCount[k] < 0)
			{
				PlayInfo.battleInfo.attackCastle.unitCount[k] = 0;
			}
		}
		PlayInfo.battleInfo.defenseUnits = new int[5];
		for (int l = 0; l < 5; l++)
		{
			PlayInfo.battleInfo.defenseUnits[l] = PlayInfo.battleInfo.defenseCastle.unitCount[l];
		}
		int currentUnitTotal = PlayInfo.battleInfo.defenseCastle.GetCurrentUnitTotal();
		int num5 = 0;
		if (currentUnitTotal > 300)
		{
			for (int m = 0; m < 5; m++)
			{
				int num6 = (int)Mathf.Ceil(300f * (float)PlayInfo.battleInfo.defenseCastle.unitCount[m] / (float)currentUnitTotal);
				num5 += num6;
				if (num5 > 300)
				{
					num6 -= num5 - 300;
				}
				if (num6 < 0)
				{
					num6 = 0;
				}
				if (num6 > PlayInfo.battleInfo.defenseCastle.unitCount[m])
				{
					num6 = PlayInfo.battleInfo.defenseCastle.unitCount[m];
				}
				PlayInfo.battleInfo.defenseUnits[m] = num6;
			}
		}
		for (int n = 0; n < 5; n++)
		{
			if (PlayInfo.battleInfo.defenseUnits[n] < 0)
			{
				PlayInfo.battleInfo.defenseUnits[n] = 0;
			}
			PlayInfo.battleInfo.defenseCastle.unitCount[n] -= PlayInfo.battleInfo.defenseUnits[n];
			if (PlayInfo.battleInfo.defenseCastle.unitCount[n] < 0)
			{
				PlayInfo.battleInfo.defenseCastle.unitCount[n] = 0;
			}
		}
		PlayInfo.battleInfo.isAttack = false;
		PlayInfo.castleAI.attackCurrent.Reset();
	}

	public void Save()
	{
		DataRegistry.KeyData parent = DataRegistry.Set(null, "CastleAI_Reserve", string.Empty);
		DataRegistry.Set(parent, "redeployDay", redeployDay);
		DataRegistry.Set(parent, "attackDay", attackDay);
		DataRegistry.Set(parent, "attackActive", attackReserve.attackActive);
		if (attackReserve.attackFrom != null)
		{
			DataRegistry.Set(parent, "attackFrom", attackReserve.attackFrom.index);
		}
		if (attackReserve.attackTo != null)
		{
			DataRegistry.Set(parent, "attackTo", attackReserve.attackTo.index);
		}
		parent = DataRegistry.Set(null, "CastleAI_Active", string.Empty);
		DataRegistry.Set(parent, "attackActive", attackCurrent.attackActive);
		if (attackCurrent.attackFrom != null)
		{
			DataRegistry.Set(parent, "attackFrom", attackCurrent.attackFrom.index);
		}
		if (attackCurrent.attackTo != null)
		{
			DataRegistry.Set(parent, "attackTo", attackCurrent.attackTo.index);
		}
		DataRegistry.Set(parent, "attackCountDown", attackCountDown);
	}

	public void Load()
	{
		string keyvalue = string.Empty;
		DataRegistry.KeyData current = null;
		if (DataRegistry.Get(null, "CastleAI_Reserve", ref keyvalue, ref current))
		{
			DataRegistry.KeyData current2 = null;
			DataRegistry.Get(current, "redeployDay", ref redeployDay, ref current2);
			DataRegistry.Get(current, "attackDay", ref attackDay, ref current2);
			int keyvalue2 = 0;
			DataRegistry.Get(current, "attackActive", ref attackReserve.attackActive, ref current2);
			if (attackReserve.attackActive)
			{
				DataRegistry.Get(current, "attackFrom", ref keyvalue2, ref current2);
				attackReserve.attackFrom = PlayInfo.castleManager.castle[keyvalue2];
				DataRegistry.Get(current, "attackTo", ref keyvalue2, ref current2);
				attackReserve.attackTo = PlayInfo.castleManager.castle[keyvalue2];
			}
			bool flag = DataRegistry.Get(null, "CastleAI_Active", ref keyvalue, ref current);
			DataRegistry.Get(current, "attackActive", ref attackCurrent.attackActive, ref current2);
			if (attackCurrent.attackActive)
			{
				DataRegistry.Get(current, "attackFrom", ref keyvalue2, ref current2);
				attackCurrent.attackFrom = PlayInfo.castleManager.castle[keyvalue2];
				DataRegistry.Get(current, "attackTo", ref keyvalue2, ref current2);
				attackCurrent.attackTo = PlayInfo.castleManager.castle[keyvalue2];
			}
			DataRegistry.Get(current, "attackCountDown", ref attackCountDown, ref current2);
		}
	}

	public void Compute()
	{
		redeployDay--;
		if (redeployDay <= 0)
		{
			ComputeRedeploy();
		}
		attackDay++;
		int num = 100;
		for (int num2 = PlayInfo.monsterAttackInterval.Count - 1; num2 >= 0; num2--)
		{
			if (PlayInfo.heroState.level > PlayInfo.monsterAttackInterval[num2].heroLevel)
			{
				num = ((num2 >= PlayInfo.monsterAttackInterval.Count - 1) ? PlayInfo.monsterAttackInterval[num2].days : PlayInfo.monsterAttackInterval[num2 + 1].days);
				break;
			}
		}
		if (attackDay >= num)
		{
			attackDay = 0;
			ComputeAttack();
		}
	}
}
