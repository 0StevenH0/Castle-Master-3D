using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class StageManager : MonoBehaviour
{
	public enum StageType
	{
		castle = 0,
		battle = 1
	}

	public class StageInfo
	{
		public string stageId;

		public int continentIdx;

		public StageType stageType;

		public GroundManager.GroundType groundType;

		public int groundIdx;

		public Vector2 mapPos;

		public Vector3 playerPos;

		public Vector3 playerAttackPos;

		public int staticUnitCount;

		public UnitInfo[] staticUnit;

		public Vector3[,] regenPos = new Vector3[2, 5];

		public Vector3[] minPos = new Vector3[2];

		public Vector3[] maxPos = new Vector3[2];

		public int[,] appearUser;

		public int[,] aliveUser;

		public int[,] firstUser;

		public bool[] allowRegen = new bool[2];

		public Vector3 castleGatePos;
	}

	public class UnitInfo
	{
		public UnitCharactor.CharactorType charactorType;

		public int charactorIdx;

		public Vector3 pos;

		public float rotationY;

		public bool movable;
	}

	public enum ClearMode
	{
		destroyGate = 0,
		timesUp = 1,
		enemyClear = 2,
		dieHero = 3
	}

	public class BonusTable
	{
		public int maxPoint;

		public int bonusRate = 100;

		public int starCount;
	}

	public const int battleAttack = 0;

	public const int battleDefense = 1;

	public const int maxBattleSide = 2;

	public const int maxStage = 1;

	public const int maxContinent = 3;

	public const int maxStaticUnit = 20;

	public const int maxUnitRegenPos = 5;

	public const int maxBattleLimit = 30;

	public const int maxBattleLimit_low = 20;

	private const float unitOffsetY = 0f;

	[System.NonSerialized]
	public StageInfo stageInfo;

	public CharactorManager charactorManager;

	public WeaponManager weaponManager;

	public UnitControl castleGate;

	public UIResult uiResult;

	public float battleTime;

	private UnitCharactor hero;

	private UnitCharactor lordUnit;

	private UnitControl[] staticUnitCtrl;

	private GameObject backGround;

	private Collider groundCollider;

	private BattleAI aiSoldier;

	private BattleAI aiMonster;

	private UIIncastleView uiIncastleView;

	private ProcBattle procBattle;

	private BoobyTrap boobyTrap;

	public StageInfo LoadStageInfo(StageType id)
	{
		StageInfo stageInfo = new StageInfo();
		stageInfo.staticUnit = new UnitInfo[20];
		stageInfo.regenPos = new Vector3[2, 5];
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		TextAsset textAsset = ResourceManager.Load("GameData", "stageinfo_" + id, typeof(TextAsset)) as TextAsset;
		bool succeed = false;
		string s = DataSecurity.Decrypt(textAsset.text, "surkwjch", out succeed);
		if (!succeed)
		{
			Debug.LogError("Decrypt Error!!");
			return null;
		}
		StringReader stringReader = new StringReader(s);
		string text;
		while ((text = stringReader.ReadLine()) != null)
		{
			if (text.Trim().Length == 0)
			{
				continue;
			}
			char[] separator = new char[1] { '\t' };
			string[] array = text.Split(separator);
			if (array.Length == 0)
			{
				continue;
			}
			string text2 = array[0].Trim();
			if (text2.Equals("stage_id"))
			{
				stageInfo.stageId = array[1].Trim();
			}
			else if (text2.Equals("continent_id"))
			{
				stageInfo.continentIdx = int.Parse(array[1].Trim());
			}
			else if (text2.Equals("stage_type"))
			{
				stageInfo.stageType = (StageType)int.Parse(array[1].Trim());
			}
			else if (text2.Equals("ground"))
			{
				stageInfo.groundType = (GroundManager.GroundType)int.Parse(array[1].Trim());
			}
			else if (text2.Equals("ground_id"))
			{
				stageInfo.groundIdx = int.Parse(array[1].Trim());
			}
			else if (text2.Equals("player_position"))
			{
				stageInfo.playerPos.x = float.Parse(array[1].Trim());
				stageInfo.playerPos.y = 0f;
				stageInfo.playerPos.z = float.Parse(array[2].Trim());
			}
			else if (text2.Equals("player_attackpos"))
			{
				stageInfo.playerAttackPos.x = float.Parse(array[1].Trim());
				stageInfo.playerAttackPos.y = 0f;
				stageInfo.playerAttackPos.z = float.Parse(array[2].Trim());
			}
			else if (text2.Equals("map_position"))
			{
				stageInfo.mapPos.x = float.Parse(array[1].Trim());
				stageInfo.mapPos.y = float.Parse(array[2].Trim());
			}
			else if (text2.Equals("static_unit"))
			{
				stageInfo.staticUnit[num] = new UnitInfo();
				stageInfo.staticUnit[num].charactorType = (UnitCharactor.CharactorType)int.Parse(array[1].Trim());
				stageInfo.staticUnit[num].charactorIdx = int.Parse(array[2].Trim());
				stageInfo.staticUnit[num].pos.x = float.Parse(array[3].Trim());
				stageInfo.staticUnit[num].pos.y = 0f;
				stageInfo.staticUnit[num].pos.z = float.Parse(array[4].Trim());
				stageInfo.staticUnit[num].rotationY = float.Parse(array[5].Trim());
				stageInfo.staticUnit[num].movable = int.Parse(array[6].Trim()) == 1;
				num++;
			}
			else if (text2.Equals("defense_regen_pos"))
			{
				if (num2 < 5)
				{
					stageInfo.regenPos[1, num2] = default(Vector3);
					stageInfo.regenPos[1, num2].x = float.Parse(array[1].Trim());
					stageInfo.regenPos[1, num2].y = 0f;
					stageInfo.regenPos[1, num2].z = float.Parse(array[2].Trim());
					num2++;
				}
			}
			else if (text2.Equals("attack_regen_pos"))
			{
				if (num3 < 5)
				{
					stageInfo.regenPos[0, num3] = default(Vector3);
					stageInfo.regenPos[0, num3].x = float.Parse(array[1].Trim());
					stageInfo.regenPos[0, num3].y = 0f;
					stageInfo.regenPos[0, num3].z = float.Parse(array[2].Trim());
					num3++;
				}
			}
			else if (text2.Equals("defense_base_minpos"))
			{
				stageInfo.minPos[1] = default(Vector3);
				stageInfo.minPos[1].x = float.Parse(array[1].Trim());
				stageInfo.minPos[1].y = 0f;
				stageInfo.minPos[1].z = float.Parse(array[2].Trim());
			}
			else if (text2.Equals("defense_base_maxpos"))
			{
				stageInfo.maxPos[1] = default(Vector3);
				stageInfo.maxPos[1].x = float.Parse(array[1].Trim());
				stageInfo.maxPos[1].y = 0f;
				stageInfo.maxPos[1].z = float.Parse(array[2].Trim());
			}
			else if (text2.Equals("attack_base_minpos"))
			{
				stageInfo.minPos[0] = default(Vector3);
				stageInfo.minPos[0].x = float.Parse(array[1].Trim());
				stageInfo.minPos[0].y = 0f;
				stageInfo.minPos[0].z = float.Parse(array[2].Trim());
			}
			else if (text2.Equals("attack_base_maxpos"))
			{
				stageInfo.maxPos[0] = default(Vector3);
				stageInfo.maxPos[0].x = float.Parse(array[1].Trim());
				stageInfo.maxPos[0].y = 0f;
				stageInfo.maxPos[0].z = float.Parse(array[2].Trim());
			}
			else if (text2.Equals("castle_gate"))
			{
				stageInfo.castleGatePos = default(Vector3);
				stageInfo.castleGatePos.x = float.Parse(array[1].Trim());
				stageInfo.castleGatePos.y = 0f;
				stageInfo.castleGatePos.z = float.Parse(array[2].Trim());
			}
		}
		stageInfo.staticUnitCount = num;
		return stageInfo;
	}

	public void GenStage(StageType id)
	{
		stageInfo = LoadStageInfo(id);
		if (stageInfo.stageType == StageType.castle)
		{
			backGround = GroundManager.GenGround(stageInfo.groundType, stageInfo.groundIdx, 0, 1);
		}
		else
		{
			backGround = GroundManager.GenGround(stageInfo.groundType, PlayInfo.battleInfo.defenseCastle.groundId, PlayInfo.battleInfo.defenseCastle.side, PlayInfo.battleInfo.defenseCastle.level);
		}
		groundCollider = GroundManager.GetGroundCollider(backGround);
		hero = charactorManager.AddHero(new Vector3(0f, 0f, 0f));
		HeroModel component = hero.GetComponent<HeroModel>();
		component.SetClothPartFromCode(HeroModel.ClothPart.head, PlayInfo.playerData.wearCloth[0].code);
		component.SetClothPartFromCode(HeroModel.ClothPart.top, PlayInfo.playerData.wearCloth[1].code);
		component.SetClothPartFromCode(HeroModel.ClothPart.bottom, PlayInfo.playerData.wearCloth[2].code);
		component.SetWeapon(weaponManager, UnitCharactor.WeaponType.onehand, PlayInfo.playerData.equipWeapon[0].code);
		UnitControl component2 = hero.gameObject.GetComponent<UnitControl>();
		component2.weaponManager = weaponManager;
		component2.colliderBackGround = groundCollider;
		component2.colliderCharacters = charactorManager.colliders;
		component2.isMainHero = true;
		component2.Init(null);
		component2.SetWeapon(UnitCharactor.WeaponType.onehand, PlayInfo.playerData.equipWeapon[0].code);
		if (stageInfo.stageType == StageType.battle && PlayInfo.battleInfo.isAttack)
		{
			component2.transform.position = stageInfo.playerAttackPos;
		}
		else
		{
			component2.transform.position = stageInfo.playerPos;
		}
		component2.Idle();
		InterfaceControl interfaceControl = base.gameObject.AddComponent<InterfaceControl>();
		interfaceControl.SetUnit(component2);
		CameraControl cameraControl = base.gameObject.AddComponent<CameraControl>();
		cameraControl.SetTarget(component2, groundCollider);
		staticUnitCtrl = new UnitControl[stageInfo.staticUnitCount];
		for (int i = 0; i < stageInfo.staticUnitCount; i++)
		{
			UnitInfo unitInfo = stageInfo.staticUnit[i];
			if (unitInfo.charactorType == UnitCharactor.CharactorType.monster)
			{
				UnitCharactor unitCharactor = charactorManager.AddMonster(unitInfo.pos, unitInfo.charactorIdx);
				UnitControl component3 = unitCharactor.gameObject.GetComponent<UnitControl>();
				component3.colliderBackGround = groundCollider;
				staticUnitCtrl[i] = component3;
			}
			else if (unitInfo.charactorType == UnitCharactor.CharactorType.npc)
			{
				UnitCharactor unitCharactor2 = charactorManager.AddNpc(unitInfo.pos, unitInfo.charactorIdx);
				UnitControl component4 = unitCharactor2.gameObject.GetComponent<UnitControl>();
				component4.colliderBackGround = groundCollider;
				component4.transform.localRotation = Quaternion.Euler(new Vector3(0f, unitInfo.rotationY, 0f));
				staticUnitCtrl[i] = component4;
			}
			else if (unitInfo.charactorType == UnitCharactor.CharactorType.soldier)
			{
				UnitCharactor unitCharactor3 = charactorManager.AddSoldier(unitInfo.pos, unitInfo.charactorIdx);
				UnitControl component5 = unitCharactor3.gameObject.GetComponent<UnitControl>();
				component5.colliderBackGround = groundCollider;
				component5.transform.localRotation = Quaternion.Euler(new Vector3(0f, unitInfo.rotationY, 0f));
				staticUnitCtrl[i] = component5;
			}
		}
		if (stageInfo.stageType == StageType.castle)
		{
			ProcCastle procCastle = base.gameObject.AddComponent<ProcCastle>();
			procCastle.interControl = interfaceControl;
			procCastle.cameraControl = cameraControl;
			GameObject questUnit = null;
			GameObject loveUnit = null;
			UnitControl[] array = staticUnitCtrl;
			foreach (UnitControl unitControl in array)
			{
				if (unitControl.thisChar.charType == UnitCharactor.CharactorType.npc)
				{
					if (unitControl.thisChar.charIdx == 4)
					{
						questUnit = unitControl.gameObject;
					}
					else if (unitControl.thisChar.charIdx == 8)
					{
						loveUnit = unitControl.gameObject;
					}
				}
			}
			procCastle.questUnit = questUnit;
			procCastle.loveUnit = loveUnit;
			GameObject gameObject = GameObject.Find("feb_incastleview");
			if (gameObject == null)
			{
				GameObject original = ResourceManager.Load("interface/prefabs", "feb_incastleview", typeof(GameObject)) as GameObject;
				gameObject = Object.Instantiate(original) as GameObject;
			}
			uiIncastleView = gameObject.GetComponent<UIIncastleView>();
			uiIncastleView.uiCamera = interfaceControl.uiCamera;
			uiIncastleView.gameCamera = cameraControl.camUnit;
			uiIncastleView.Init();
			cameraControl.DisableCameraLimitMinX();
			StartCoroutine("AutoWalkNPC");
		}
		else
		{
			if (stageInfo.stageType != StageType.battle)
			{
				return;
			}
			CastleInfo castleInfo = ((!PlayInfo.battleInfo.isAttack) ? PlayInfo.battleInfo.defenseCastle : PlayInfo.battleInfo.attackCastle);
			if (castleInfo.lord != null)
			{
				UnitState unitState = new UnitState();
				unitState.attack = castleInfo.lord.atk;
				unitState.defense = castleInfo.lord.def;
				unitState.strength = castleInfo.lord.str;
				unitState.intellectual = castleInfo.lord.inte;
				unitState.constitution = castleInfo.lord.con;
				unitState.baseHp = castleInfo.lord.hp;
				unitState.critical = castleInfo.lord.critical;
				unitState.speed = castleInfo.lord.speed;
				if (castleInfo.lord.splash > 0f)
				{
					unitState.splashAttack = true;
					unitState.splashRange = (int)castleInfo.lord.splash;
					unitState.splashLength = castleInfo.lord.splashLength;
				}
				lordUnit = charactorManager.AddLord(new Vector3(0f, 0f, 0f), (int)castleInfo.lord.type);
				UnitControl component6 = lordUnit.gameObject.GetComponent<UnitControl>();
				component6.colliderBackGround = groundCollider;
				component6.colliderCharacters = charactorManager.colliders;
				component6.Init(unitState);
				component6.battleSide = ((!PlayInfo.battleInfo.isAttack) ? 1 : 0);
				Vector3 position = component2.transform.position;
				position.z += 4f;
				component6.transform.position = position;
				component6.Idle();
			}
			GenBattleStartUnit();
			hero.thisCtrl.battleSide = ((!PlayInfo.battleInfo.isAttack) ? 1 : 0);
			hero.thisCtrl.SetRotation((!PlayInfo.battleInfo.isAttack) ? 90 : (-90));
			stageInfo.allowRegen[0] = true;
			stageInfo.allowRegen[1] = true;
			procBattle = base.gameObject.AddComponent<ProcBattle>();
			procBattle.interControl = interfaceControl;
			procBattle.cameraControl = cameraControl;
			procBattle.battleStage = this;
			aiSoldier = new BattleAI();
			aiSoldier.Init(charactorManager, castleGate, (!PlayInfo.battleInfo.isAttack) ? 1 : 0, this);
			aiMonster = new BattleAI();
			aiMonster.Init(charactorManager, castleGate, PlayInfo.battleInfo.isAttack ? 1 : 0, this);
			boobyTrap = base.gameObject.AddComponent<BoobyTrap>();
			StartCoroutine("ActiveBoobyTrap");
			if (PlayInfo.battleInfo.isAttack)
			{
				CastleTrap castleTrap = base.gameObject.AddComponent<CastleTrap>();
				castleTrap.Init(PlayInfo.battleInfo.defenseCastle.level, stageInfo.castleGatePos, charactorManager, this);
			}
			battleTime = 0f;
			StartCoroutine("CountBattleTime");
			cameraControl.EnableCameraLimitMinX(castleGate.transform.position.x);
			StartCoroutine("WaitForStartupHeroWeapon");
			StartCoroutine("CheckForStageClear");
		}
	}

	private IEnumerator ActiveBoobyTrap()
	{
		yield return new WaitForSeconds(1f);
		boobyTrap.uiIngameView = procBattle.uiIngameView;
		List<BoobyTrap.TrapType> typeList = new List<BoobyTrap.TrapType>();
		List<BoobyTrap.TrapType> appearType = new List<BoobyTrap.TrapType>();
		for (int i = 0; i < 7; i++)
		{
			if (PlayInfo.heroState.level >= BoobyTrap.trapAttr[i].iconHeroLevel)
			{
				typeList.Add((BoobyTrap.TrapType)i);
			}
		}
		float firstTime = 15 + Random.Range(0, 10);
		float restTime = (float)PlayInfo.gameRule.battleTimeLimit - firstTime;
		while (restTime > 0f)
		{
			BoobyTrap.TrapType trap = typeList[Random.Range(0, typeList.Count)];
			float nextTrapTime = BoobyTrap.trapAttr[(int)trap].iconInterval;
			restTime -= nextTrapTime;
			appearType.Add(trap);
			boobyTrap.PreLordTrapObject(trap);
		}
		yield return 1;
		boobyTrap.HideTrapObject();
		int currentTrap = 0;
		yield return new WaitForSeconds(firstTime);
		while (true)
		{
			boobyTrap.Init(appearType[currentTrap], hero.transform, groundCollider);
			float nextTrapTime2 = BoobyTrap.trapAttr[(int)appearType[currentTrap]].iconInterval + Random.Range(-5f, 5f);
			if (nextTrapTime2 < 0f)
			{
				nextTrapTime2 = 5f;
			}
			currentTrap++;
			if (currentTrap >= appearType.Count)
			{
				break;
			}
			yield return new WaitForSeconds(nextTrapTime2);
		}
	}

	private IEnumerator WaitForStartupHeroWeapon()
	{
		yield return 1;
		hero.thisCtrl.SetWeapon(UnitCharactor.WeaponType.doublehand, 0);
		hero.thisCtrl.SetWeapon(UnitCharactor.WeaponType.onehand, PlayInfo.playerData.equipWeapon[0].code);
		if (lordUnit != null)
		{
			lordUnit.transform.rotation = hero.transform.rotation;
		}
	}

	private void GenBattleStartUnit()
	{
		int[] array = new int[30]
		{
			0, 0, 0, 0, 0, 1, 1, 1, 2, 2,
			2, 3, 3, 4, 4, 0, 0, 0, 0, 0,
			1, 1, 1, 2, 2, 2, 3, 3, 4, 4
		};
		int[] array2 = new int[20]
		{
			0, 0, 1, 1, 2, 2, 3, 3, 4, 4,
			0, 0, 1, 1, 2, 2, 3, 3, 4, 4
		};
		int[] array3 = new int[30]
		{
			2, 1, 3, 0, 4, 11, 13, 10, 14, 12,
			20, 24, 21, 23, 22, 7, 6, 8, 5, 9,
			16, 18, 15, 19, 17, 25, 29, 26, 28, 27
		};
		int[] array4 = new int[30]
		{
			4, 5, 3, 6, 2, 7, 1, 8, 0, 9,
			14, 15, 13, 16, 12, 17, 11, 18, 10, 19,
			24, 25, 23, 26, 22, 27, 21, 28, 20, 29
		};
		int[] array5 = new int[20]
		{
			2, 7, 1, 8, 0, 9, 12, 17, 13, 16,
			3, 6, 4, 5, 10, 19, 11, 18, 14, 15
		};
		Vector3[,] array6 = new Vector3[2, 30];
		stageInfo.appearUser = new int[2, 5];
		stageInfo.aliveUser = new int[2, 5];
		stageInfo.firstUser = new int[2, 5];
		int[] array7 = new int[2];
		for (int i = 0; i < 5; i++)
		{
			stageInfo.appearUser[0, i] = PlayInfo.battleInfo.attackUnits[i];
			stageInfo.appearUser[1, i] = PlayInfo.battleInfo.defenseUnits[i];
			stageInfo.aliveUser[0, i] = PlayInfo.battleInfo.attackUnits[i];
			stageInfo.aliveUser[1, i] = PlayInfo.battleInfo.defenseUnits[i];
			stageInfo.firstUser[0, i] = PlayInfo.battleInfo.attackUnits[i];
			stageInfo.firstUser[1, i] = PlayInfo.battleInfo.defenseUnits[i];
			array7[0] += PlayInfo.battleInfo.attackUnits[i];
			array7[1] += PlayInfo.battleInfo.defenseUnits[i];
		}
		int num = 10;
		int num2 = 30 / num;
		float num3 = (stageInfo.maxPos[0].x - stageInfo.minPos[0].x) / (float)(num2 - 1);
		float num4 = (stageInfo.maxPos[0].z - stageInfo.minPos[0].z) / (float)(num - 1);
		int num5 = 0;
		for (int j = 0; j < num2; j++)
		{
			Vector3 vector = new Vector3(0f, 0f, 0f);
			vector.x = stageInfo.minPos[0].x + (float)j * num3;
			for (int k = 0; k < num; k++)
			{
				vector.z = stageInfo.maxPos[0].z - (float)k * num4;
				array6[0, num5] = vector;
				num5++;
			}
		}
		num3 = (stageInfo.maxPos[1].x - stageInfo.minPos[1].x) / (float)(num2 - 1);
		num4 = (stageInfo.maxPos[1].z - stageInfo.minPos[1].z) / (float)(num - 1);
		num5 = 0;
		for (int l = 0; l < num2; l++)
		{
			Vector3 vector2 = new Vector3(0f, 0f, 0f);
			vector2.x = stageInfo.maxPos[1].x - (float)l * num3;
			for (int m = 0; m < num; m++)
			{
				vector2.z = stageInfo.maxPos[1].z - (float)m * num4;
				array6[1, num5] = vector2;
				num5++;
			}
		}
		bool flag = UserSetting.quality == UserSetting.GraphicsQuality.fast;
		for (int n = 0; n < 2; n++)
		{
			int num6 = 30;
			if (flag)
			{
				num6 = 20;
			}
			for (int num7 = 0; num7 < num6; num7++)
			{
				int num8;
				for (num8 = ((!flag) ? array[num7] : array2[num7]); stageInfo.appearUser[n, num8] <= 0; num8++)
				{
					if (num8 >= 4)
					{
						num8 = ((!flag) ? array[num7] : array2[num7]);
						break;
					}
				}
				while (stageInfo.appearUser[n, num8] <= 0)
				{
					if (num8 > 0)
					{
						num8--;
						continue;
					}
					num8 = ((!flag) ? array[num7] : array2[num7]);
					break;
				}
				if (stageInfo.appearUser[n, num8] <= 0)
				{
					break;
				}
				stageInfo.appearUser[n, num8]--;
				Vector3 pos = array6[n, array3[num7]];
				if (flag)
				{
					pos = array6[n, array5[num7]];
				}
				else if (array7[n] < 20)
				{
					pos = array6[n, array4[num7]];
				}
				pos.x += ((float)Random.Range(0, 100) / 100f - 0.5f) * 0.5f;
				pos.z += (float)Random.Range(0, 100) / 100f - 0.5f;
				AddBattleUnit(pos, n, (n != 0) ? PlayInfo.battleInfo.defenseCastle : PlayInfo.battleInfo.attackCastle, num8);
			}
		}
		castleGate = AddCastleGate(stageInfo.castleGatePos, PlayInfo.battleInfo.defenseCastle);
	}

	private UnitControl AddCastleGate(Vector3 pos, CastleInfo castle)
	{
		UnitCharactor unitCharactor = ((castle.side != 0) ? charactorManager.AddCastleGate(pos, UnitCharactor.CharactorType.monster, castle.level - 1) : charactorManager.AddCastleGate(pos, UnitCharactor.CharactorType.soldier, castle.level - 1));
		UnitControl thisCtrl = unitCharactor.thisCtrl;
		thisCtrl.colliderBackGround = groundCollider;
		thisCtrl.SetRotation(90f);
		thisCtrl.unitState.curHp = castle.GetCastleDefense();
		thisCtrl.unitState.baseHp = castle.GetCastleDefense();
		thisCtrl.thisChar.allowAttack = false;
		thisCtrl.thisChar.allowMove = false;
		thisCtrl.battleSide = 1;
		return unitCharactor.thisCtrl;
	}

	private void AddBattleUnit(Vector3 pos, int side, CastleInfo castle, int unitIdx)
	{
		UnitCharactor unitCharactor = ((PlayInfo.battleInfo.attackCastle == PlayInfo.battleInfo.defenseCastle) ? ((side != 0) ? charactorManager.AddUnitFromCode(pos, UnitCharactor.CharactorType.monster, castle.monsterCode[unitIdx]) : charactorManager.AddSoldier(pos, unitIdx)) : ((castle.side != 0) ? charactorManager.AddUnitFromCode(pos, UnitCharactor.CharactorType.monster, castle.monsterCode[unitIdx]) : charactorManager.AddSoldier(pos, unitIdx)));
		UnitControl component = unitCharactor.gameObject.GetComponent<UnitControl>();
		component.colliderBackGround = groundCollider;
		component.SetRotation((side != 0) ? 90 : (-90));
		component.unitSlot = unitIdx;
		component.battleSide = side;
	}

	private void RegenBattleUnit(UnitCharactor.CharactorType type, UnitControl ctl, int side)
	{
		int num = 0;
		int num2 = 0;
		if (type == UnitCharactor.CharactorType.soldier)
		{
			num2 = ctl.thisChar.charIdx;
		}
		else
		{
			int charIdx = ctl.thisChar.charIdx;
			int num3 = CharactorManager.monsterIndexCode[charIdx];
			if (side == 0)
			{
				for (int i = 0; i < 5; i++)
				{
					if (PlayInfo.battleInfo.attackCastle.monsterCode[i] == num3)
					{
						num2 = i;
						break;
					}
				}
			}
			else
			{
				for (int j = 0; j < 5; j++)
				{
					if (PlayInfo.battleInfo.defenseCastle.monsterCode[j] == num3)
					{
						num2 = j;
						break;
					}
				}
			}
		}
		for (num = num2; stageInfo.appearUser[side, num] <= 0 && num < 4; num++)
		{
		}
		while (stageInfo.appearUser[side, num] <= 0 && num > 0)
		{
			num--;
		}
		if (stageInfo.appearUser[side, num] <= 0)
		{
			stageInfo.allowRegen[side] = false;
			return;
		}
		stageInfo.appearUser[side, num]--;
		if (num == num2)
		{
			int num4 = Random.Range(0, 5);
			Vector3 position = stageInfo.regenPos[side, num4];
			position.z += ((float)Random.Range(0, 20) - 10f) / 10f;
			ctl.gameObject.SetActive(true);
			ctl.Init(null);
			ctl.SetPosition(position);
			ctl.SetRotation((side != 0) ? 90 : (-90));
		}
		else
		{
			ctl.thisChar.isRemove = true;
			int num5 = Random.Range(0, 5);
			AddBattleUnit(stageInfo.regenPos[side, num5], side, (side != 0) ? PlayInfo.battleInfo.defenseCastle : PlayInfo.battleInfo.attackCastle, num);
		}
	}

	public void DecAliveUser(UnitControl ctl, int side)
	{
		stageInfo.aliveUser[side, ctl.unitSlot]--;
		if (stageInfo.aliveUser[side, ctl.unitSlot] < 0)
		{
			stageInfo.aliveUser[side, ctl.unitSlot] = 0;
		}
	}

	private IEnumerator CheckForStageClear()
	{
		yield return new WaitForSeconds(1f);
		if (PlayInfo.battleInfo.isAttack)
		{
			PlayInfo.soundManager.Play(ExtSoundManager.Effect2D.efs_battle_attack, new Vector3(0f, 0f, 0f));
		}
		else
		{
			PlayInfo.soundManager.Play(ExtSoundManager.Effect2D.efs_battle_defense, new Vector3(0f, 0f, 0f));
		}
		procBattle.uiIngameView.SetCastleTransform(castleGate.transform);
		int result;
		ClearMode clearMode;
		do
		{
			yield return new WaitForSeconds(0.5f);
			int count = charactorManager.charactors.Count;
			for (int i = 0; i < count; i++)
			{
				UnitCharactor unit = charactorManager.charactors[i];
				if (!unit.isGate && unit.charType != 0 && !unit.isRemove && !unit.isAwake)
				{
					UnitControl ctl = unit.thisCtrl;
					RegenBattleUnit(unit.charType, ctl, ctl.battleSide);
				}
			}
			if (lordUnit != null && !lordUnit.isAwake)
			{
				lordUnit.gameObject.SetActive(true);
				lordUnit.thisCtrl.unitState.curHp = lordUnit.thisCtrl.unitState.sumHp;
				lordUnit.thisCtrl.isDie = false;
				lordUnit.thisCtrl.isAwake = true;
				lordUnit.isAwake = true;
				int idx = Random.Range(0, 5);
				lordUnit.thisCtrl.thisTrans.position = stageInfo.regenPos[lordUnit.thisCtrl.battleSide, idx];
				lordUnit.thisCtrl.ForcedIdle();
			}
			aiSoldier.Update();
			aiMonster.Update();
			result = 0;
			clearMode = ClearMode.destroyGate;
			if (castleGate.isDie)
			{
				result = (PlayInfo.battleInfo.isAttack ? 1 : (-1));
				clearMode = ClearMode.destroyGate;
			}
			if (result == 0 && hero.thisCtrl.isDie)
			{
				result = -1;
				clearMode = ClearMode.dieHero;
			}
			if (result == 0 && !PlayInfo.battleInfo.isAttack && GetAliveUnit(0) <= 0)
			{
				result = 1;
				clearMode = ClearMode.enemyClear;
			}
			if (battleTime >= (float)PlayInfo.gameRule.battleTimeLimit && result == 0)
			{
				result = ((!PlayInfo.battleInfo.isAttack) ? 1 : (-1));
				clearMode = ClearMode.timesUp;
			}
		}
		while (result == 0);
		StartCoroutine(FinishBattle(result, clearMode));
	}

	public int GetAliveUnit(int battleSide)
	{
		int num = 0;
		for (int i = 0; i < 5; i++)
		{
			num += stageInfo.aliveUser[battleSide, i];
		}
		return num;
	}

	private BonusTable[] LoadBonusTable()
	{
		TextAsset textAsset = ResourceManager.Load("GameData", "battle_point", typeof(TextAsset)) as TextAsset;
		bool succeed = false;
		string s = DataSecurity.Decrypt(textAsset.text, "surkwjch", out succeed);
		if (!succeed)
		{
			Debug.LogError("Decrypt Error!!");
			return null;
		}
		StringReader stringReader = new StringReader(s);
		List<BonusTable> list = new List<BonusTable>();
		string text;
		while ((text = stringReader.ReadLine()) != null)
		{
			if (text.Trim().Length != 0)
			{
				char[] separator = new char[1] { '\t' };
				string[] array = text.Split(separator);
				if (array.Length > 1)
				{
					BonusTable bonusTable = new BonusTable();
					bonusTable.maxPoint = int.Parse(array[0]);
					bonusTable.bonusRate = int.Parse(array[1]);
					bonusTable.starCount = int.Parse(array[2]);
					list.Add(bonusTable);
				}
			}
		}
		return list.ToArray();
	}

	private IEnumerator FinishBattle(int result, ClearMode clearMode)
	{
		int rewardFame = 0;
		float rewardXp = 0f;
		int rewardGold = 0;
		int rewardGem = 0;
		int bonusPoint = 0;
		int bonusRate = 100;
		BonusTable[] bonusTable = LoadBonusTable();
		StopCoroutine("CountBattleTime");
		bool isFirstBattle = PlayInfo.battleInfo.attackCastle == PlayInfo.battleInfo.defenseCastle;
		if (isFirstBattle)
		{
			CastleInfo castle = PlayInfo.battleInfo.defenseCastle;
			castle.side = 0;
			castle.Reset();
			castle.SetAutoUpgrade();
			for (int i = 0; i < 5; i++)
			{
				castle.unitCount[i] = stageInfo.aliveUser[0, i];
			}
			PlayInfo.soundManager.Play(ExtSoundManager.Effect2D.efs_crowd_cheer, hero.thisCtrl.thisTrans.position);
		}
		else if (result > 0)
		{
			rewardFame = PlayInfo.battleRewardVictory.fame;
			rewardGem = PlayInfo.battleRewardVictory.gem;
			PlayInfo.heroState.fame += rewardFame;
			PlayInfo.playerData.gem += rewardGem;
			int killUnit = 0;
			int sideAlly = ((!PlayInfo.battleInfo.isAttack) ? 1 : 0);
			int sideMonster = (PlayInfo.battleInfo.isAttack ? 1 : 0);
			for (int n = 0; n < 5; n++)
			{
				killUnit += stageInfo.firstUser[sideMonster, n] - stageInfo.aliveUser[sideMonster, n];
			}
			rewardXp = (float)killUnit * PlayInfo.battleRewardVictory.xp_a + (float)PlayInfo.heroState.level * PlayInfo.battleRewardVictory.xp_b + PlayInfo.battleRewardVictory.xp_inc;
			rewardGold = (int)((float)killUnit * PlayInfo.battleRewardVictory.gold_a + (float)PlayInfo.heroState.level * PlayInfo.battleRewardVictory.gold_b + PlayInfo.battleRewardVictory.gold_inc);
			bonusPoint = bonusTable[bonusTable.Length - 1].starCount;
			bonusRate = bonusTable[bonusTable.Length - 1].bonusRate;
			int point = (int)(battleTime + (float)(killUnit * 2));
			for (int m = 0; m < bonusTable.Length; m++)
			{
				if (point <= bonusTable[m].maxPoint)
				{
					bonusPoint = bonusTable[m].starCount;
					bonusRate = bonusTable[m].bonusRate;
					break;
				}
			}
			rewardXp = (int)(rewardXp * (float)bonusRate / 100f);
			rewardGold = (int)((float)(rewardGold * bonusRate) / 100f);
			bool isLevelUp = false;
			PlayInfo.IncUnitExp(PlayInfo.heroState, rewardXp, out isLevelUp);
			PlayInfo.playerData.gold += rewardGold;
			CastleInfo castle5 = PlayInfo.battleInfo.defenseCastle;
			if (PlayInfo.battleInfo.isAttack)
			{
				castle5.side = 0;
				castle5.Reset();
				castle5.SetAutoUpgrade();
			}
			for (int l = 0; l < 5; l++)
			{
				castle5.unitCount[l] += stageInfo.aliveUser[sideAlly, l];
			}
			if (isLevelUp)
			{
				if (hero.thisCtrl.effectHeroSkill == null)
				{
					hero.thisCtrl.effectHeroSkill = hero.gameObject.GetComponent<EffectHeroSkill>();
				}
				hero.thisCtrl.effectHeroSkill.PlayExtraEffect(EffectHeroSkill.ExtraEffectType.levelUp);
				PlayInfo.soundManager.Play(ExtSoundManager.Effect2D.efs_levelup_effect, new Vector3(0f, 0f, 0f));
				UIIngameView.self.ShowLevelUp();
			}
			PlayInfo.soundManager.Play(ExtSoundManager.Effect2D.efs_crowd_cheer, hero.thisCtrl.thisTrans.position);
			foreach (UnitCharactor unit2 in charactorManager.charactors)
			{
				if (unit2.charType == UnitCharactor.CharactorType.soldier || unit2.charType == UnitCharactor.CharactorType.lord)
				{
					if (unit2.isAwake)
					{
						unit2.thisCtrl.Victory();
					}
				}
				else if (unit2.charType == UnitCharactor.CharactorType.monster && unit2.isAwake)
				{
					unit2.thisCtrl.ClearTarget();
				}
			}
		}
		else
		{
			rewardFame = PlayInfo.battleRewardDepeat.fame;
			rewardGem = 0;
			PlayInfo.heroState.fame += rewardFame;
			if (PlayInfo.heroState.fame < 0)
			{
				PlayInfo.heroState.fame = 0;
			}
			int sideAlly2 = ((!PlayInfo.battleInfo.isAttack) ? 1 : 0);
			rewardXp = 0f - ((float)PlayInfo.heroState.level * PlayInfo.battleRewardDepeat.xp_b + PlayInfo.battleRewardDepeat.xp_inc);
			rewardGold = 0;
			PlayInfo.heroState.exp += rewardXp;
			if (PlayInfo.heroState.exp < 0f)
			{
				PlayInfo.heroState.exp = 0f;
			}
			if (PlayInfo.battleInfo.isAttack)
			{
				CastleInfo castle4 = PlayInfo.battleInfo.attackCastle;
				for (int k = 0; k < 5; k++)
				{
					castle4.unitCount[k] += stageInfo.aliveUser[sideAlly2, k];
				}
			}
			else
			{
				CastleInfo castle3 = PlayInfo.battleInfo.defenseCastle;
				if (castle3.lord != null)
				{
					string msgContent3 = StringContent.msgMinLoyaltyRunAwayInCastle;
					string msgShort3 = StringContent.msgLordRunAway;
					msgContent3 = msgContent3.Replace(StringContent.strValue, castle3.lord.name);
					msgShort3 = msgShort3.Replace(StringContent.strValue, castle3.lord.name);
					msgContent3 = msgContent3.Replace(StringContent.strValue2, castle3.castleName);
					msgShort3 = msgShort3 + " (" + castle3.castleName + ")";
					PlayInfo.messageManager.Add(2, PlayMessage.MessagLevel.alert, msgContent3, msgShort3);
					PlayInfo.lordManager.list.Remove(castle3.lord);
					castle3.lord = null;
				}
			}
			int sideEnemy = (PlayInfo.battleInfo.isAttack ? 1 : 0);
			CastleInfo castle2 = PlayInfo.battleInfo.defenseCastle;
			if (sideEnemy == 0)
			{
				castle2.side = 1;
				castle2.Reset();
				castle2.SetAutoUpgrade();
			}
			for (int j = 0; j < 5; j++)
			{
				castle2.unitCount[j] += stageInfo.aliveUser[sideEnemy, j];
			}
			foreach (UnitCharactor unit in charactorManager.charactors)
			{
				if (unit.isAwake)
				{
					unit.thisCtrl.ClearTarget();
				}
			}
		}
		hero.thisCtrl.ClearTarget();
		hero.allowAttack = false;
		PlayInfo.battleInfo.battleFinish = true;
		PlayInfo.battleInfo.battleWin = result > 0;
		PlayInfo.Save();
		UIIngameView.self.ShowFinish(clearMode);
		yield return new WaitForSeconds(3.5f);
		if (isFirstBattle)
		{
			ProcMain.playStage = StageType.castle;
			ProcBase.LoadScene("Scene Main", ProcLoading.ContentMode.castle);
		}
		else
		{
			uiResult.Show(PlayInfo.battleInfo.isAttack, result > 0, rewardFame, (int)rewardXp, rewardGold, rewardGem, bonusPoint, bonusRate);
		}
	}

	private IEnumerator CountBattleTime()
	{
		do
		{
			yield return new WaitForSeconds(1f);
			if (PlayInfo.battleInfo.battleFinish)
			{
				yield break;
			}
			battleTime += 1f;
		}
		while (!(battleTime > (float)PlayInfo.gameRule.battleTimeLimit));
		battleTime = PlayInfo.gameRule.battleTimeLimit;
	}

	private IEnumerator AutoWalkNPC()
	{
		while (true)
		{
			for (int i = 0; i < stageInfo.staticUnitCount; i++)
			{
				if (stageInfo.staticUnit[i].movable && staticUnitCtrl[i].isMovable && staticUnitCtrl[i].currentMotion == UnitControl.MotionType.idle && Random.Range(0, 10) < 5)
				{
					float x = (float)(Random.Range(0, 12) - 6) + stageInfo.staticUnit[i].pos.x;
					float z = (float)(Random.Range(0, 12) - 6) + stageInfo.staticUnit[i].pos.z;
					staticUnitCtrl[i].WalkTo(new Vector3(x, 0f, z), true);
					staticUnitCtrl[i].GetComponent<Rigidbody>().constraints = (RigidbodyConstraints)116;
				}
			}
			yield return new WaitForSeconds(3f);
		}
	}
}
