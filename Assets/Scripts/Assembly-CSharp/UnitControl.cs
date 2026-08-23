using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitControl : MonoBehaviour
{
	public enum MotionType
	{
		idle = 0,
		prepare = 1,
		move = 2,
		attack = 3,
		damage = 4,
		die = 5,
		standup = 6,
		succeed = 7
	}

	public Collider colliderBackGround;

	public WeaponManager weaponManager;

	public List<Collider> colliderCharacters;

	public MotionType currentMotion;

	public bool isAwake = true;

	public InterfaceControl.OnFollowFinish onFollowFinish;

	public int unitIdentify;

	public Transform thisTrans;

	public UnitCharactor thisChar;

	public bool isMainHero;

	public int targetIdentify;

	public UnitControl targetCtrl;

	protected Vector3 targetPos;

	protected Vector3 targetDir;

	protected Vector3 currentPos;

	protected bool isFollow;

	protected bool isRotate;

	protected bool isMove;

	public bool isDie;

	public bool isMovable = true;

	protected bool isAutoAttack = true;

	protected UnitControl knockCtrl;

	protected bool isWaitNextMotion;

	protected float timeWaitNextMotion;

	public int battleSide;

	public bool defenseAssist;

	public int unitSlot;

	protected UnitCharactor.WeaponType nextWeaponType;

	protected int nextWeaponCode;

	protected Vector3 nextPos;

	protected float nextSpeed;

	protected bool nextDirRotate;

	protected UnitCharactor.MoveMode nextMoveMode;

	protected MotionType nextMotion;

	protected UnitCharactor.AttackMode nextAttackMode;

	protected int nextAttackIndex;

	protected HeroSkill.SkillLevelSpec nextSkillSpec;

	public UnitCharactor.WeaponType weaponType;

	public int weaponCode;

	protected HeroSkill.SkillLevelSpec curSkillSpec;

	protected float attackPower = 80f;

	protected float defensePower;

	protected float splashAttackRange = 1.5f;

	public EffectHeroSkill effectHeroSkill;

	public UnitState unitState = new UnitState();

	protected float moveSpeed;

	protected float moveLen;

	protected float lastBattleTime;

	private float downSpeed;

	private static int identifyCount;

	public void Init(UnitState state)
	{
		unitIdentify = identifyCount++;
		thisTrans = base.transform;
		isAwake = true;
		isFollow = false;
		isRotate = false;
		isMove = false;
		isDie = false;
		defenseAssist = false;
		knockCtrl = null;
		downSpeed = 0f;
		thisChar.isAwake = true;
		thisChar.isRemove = false;
		if (thisChar.GetComponent<Collider>() != null)
		{
			thisChar.GetComponent<Collider>().enabled = true;
		}
		currentMotion = MotionType.idle;
		ClearTarget();
		if (thisChar.isGate)
		{
			unitState = new UnitState();
		}
		else
		{
			if (thisChar.charType == UnitCharactor.CharactorType.soldier)
			{
				unitState = PlayInfo.humanMilitary.GetUnitState(thisChar.charIdx).Clone();
			}
			else if (thisChar.charType == UnitCharactor.CharactorType.monster)
			{
				unitState = PlayInfo.monsterMilitary.GetUnitState(thisChar.charIdx).Clone();
			}
			else if (thisChar.charType == UnitCharactor.CharactorType.lord)
			{
				unitState = state;
			}
			else if (thisChar.charType == UnitCharactor.CharactorType.hero)
			{
				unitState = PlayInfo.heroState;
				if (state != null)
				{
					unitState = state;
				}
			}
			if (unitState == null)
			{
				unitState = new UnitState();
			}
			unitState.curHp = unitState.sumHp;
			unitState.curMp = unitState.sumMp;
			attackPower = unitState.sumAttack;
			defensePower = unitState.sumDefense;
			thisChar.runSpeed = 5f * unitState.speed;
		}
		lastBattleTime = -1000f;
		curSkillSpec = null;
		SetAttackMode(UnitCharactor.AttackMode.normal, 0);
		ForcedIdle();
	}

	public void SetPosition(Vector3 pos)
	{
		thisTrans.localPosition = pos;
	}

	public void SetRotation(float rotateY)
	{
		thisTrans.localRotation = Quaternion.Euler(0f, rotateY, 0f);
	}

	public void SetPositionTo(Vector3 pos)
	{
		thisTrans.localPosition += pos;
	}

	public void SetRotationTo(float rotateY)
	{
		Vector3 eulerAngles = thisTrans.localRotation.eulerAngles;
		eulerAngles.y += rotateY;
		thisTrans.localRotation = Quaternion.Euler(eulerAngles);
	}

	public void SetRotationTo(Vector3 pos)
	{
		Vector3 forward = pos - thisTrans.position;
		if (forward.x != 0f || forward.y != 0f || forward.z != 0f)
		{
			Quaternion rotation = Quaternion.LookRotation(forward);
			thisTrans.rotation = rotation;
		}
	}

	public void ClearTarget()
	{
		SetTarget(null, null);
	}

	public void SetTarget(UnitCharactor target, InterfaceControl.OnFollowFinish procFollowFinish)
	{
		SetTargetControl((!(target != null)) ? null : target.thisCtrl, procFollowFinish);
	}

	public void SetTargetControl(UnitControl target, InterfaceControl.OnFollowFinish procFollowFinish)
	{
		if (!thisChar.allowAttack)
		{
			return;
		}
		if (target != null)
		{
			targetCtrl = target;
			targetIdentify = target.unitIdentify;
			isFollow = true;
			if (targetCtrl != null)
			{
				SetRotationTo(targetCtrl.thisTrans.position);
			}
			onFollowFinish = procFollowFinish;
			StopCoroutine("CoroutineFollowUnit");
			StartCoroutine("CoroutineFollowUnit");
		}
		else
		{
			targetIdentify = -1;
			targetCtrl = null;
			isFollow = false;
			onFollowFinish = null;
		}
	}

	public void ChangeMoveSpeed(float speed)
	{
		moveSpeed = speed;
		targetDir.Normalize();
		targetDir *= moveSpeed;
	}

	public void MoveTo(Vector3 pos, bool rotateToTarget, float speed)
	{
		if (thisChar.allowMove)
		{
			targetPos = pos;
			currentPos = base.transform.localPosition;
			if (downSpeed > 0f)
			{
				moveSpeed = downSpeed;
			}
			else
			{
				moveSpeed = speed;
			}
			targetDir = targetPos - currentPos;
			targetDir.y = 0f;
			targetDir.Normalize();
			targetDir *= moveSpeed;
			moveLen = Vector3.Distance(targetPos, currentPos);
			if (rotateToTarget)
			{
				SetRotationTo(targetPos);
			}
			isMove = true;
		}
	}

	public void Idle()
	{
		if (Time.timeSinceLevelLoad - lastBattleTime < 10f)
		{
			Prepare();
		}
		else
		{
			SetNextMotion(MotionType.idle);
		}
	}

	public void Prepare()
	{
		SetNextMotion(MotionType.prepare);
	}

	public void ForcedIdle()
	{
		if (isAwake)
		{
			ClearTarget();
			isFollow = false;
			isMove = false;
			StopAllCoroutines();
			thisChar.SetMotionType(MotionType.idle);
		}
	}

	public void WalkTo(Vector3 pos, bool rotateToTarget)
	{
		if (thisChar.allowMove)
		{
			nextPos = pos;
			nextSpeed = thisChar.walkSpeed;
			nextDirRotate = rotateToTarget;
			nextMoveMode = UnitCharactor.MoveMode.walk;
			SetNextMotion(MotionType.move);
			if (thisChar.isIdle || thisChar.isMoving)
			{
				PlayNextMotion();
			}
		}
	}

	public void RunTo(Vector3 pos)
	{
		if (thisChar.allowMove)
		{
			nextPos = pos;
			nextSpeed = thisChar.runSpeed;
			nextDirRotate = true;
			nextMoveMode = UnitCharactor.MoveMode.run;
			SetNextMotion(MotionType.move);
			if (thisChar.isIdle || thisChar.isMoving)
			{
				PlayNextMotion();
			}
		}
	}

	public void FastMove(float length)
	{
		Vector3 direction = thisTrans.rotation * new Vector3(0f, 0f, 1f);
		direction.Normalize();
		if (!(base.GetComponent<Rigidbody>() != null))
		{
			return;
		}
		RaycastHit[] array = Physics.RaycastAll(thisTrans.position, direction, length, 1);
		if (array != null)
		{
			RaycastHit[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				RaycastHit raycastHit = array2[i];
				if (length > raycastHit.distance)
				{
					length = raycastHit.distance;
				}
			}
		}
		direction = thisTrans.rotation * new Vector3(0f, 0f, length);
		Vector3 pos = direction + thisTrans.position;
		MoveTo(pos, false, 40f);
	}

	public void SetWeapon(UnitCharactor.WeaponType weapon, int code)
	{
		if (weapon == weaponType && code == weaponCode)
		{
			nextWeaponType = weapon;
			nextWeaponCode = code;
			return;
		}
		switch (weapon)
		{
		case UnitCharactor.WeaponType.onehand:
			unitState.curWeaponSlot = 0;
			break;
		case UnitCharactor.WeaponType.doublehand:
			unitState.curWeaponSlot = 1;
			break;
		case UnitCharactor.WeaponType.bigsword:
			unitState.curWeaponSlot = 2;
			break;
		}
		nextWeaponType = weapon;
		nextWeaponCode = code;
		attackPower = unitState.sumAttack;
		defensePower = unitState.sumDefense;
		if (thisChar.isIdle || thisChar.isMoving)
		{
			weaponType = weapon;
			weaponCode = code;
			thisChar.SetWeapon(weaponType, code);
		}
	}

	public void SetAttackMode(UnitCharactor.AttackMode atk, int idx)
	{
		nextAttackMode = atk;
		nextAttackIndex = idx;
	}

	public void Attack()
	{
		Attack(null);
	}

	public void Attack(HeroSkill.SkillLevelSpec spec)
	{
		if (thisChar.allowAttack && weaponType != 0 && ((thisChar.charType != UnitCharactor.CharactorType.monster && thisChar.charType != UnitCharactor.CharactorType.soldier && thisChar.charType != UnitCharactor.CharactorType.lord) || !(targetCtrl == null)))
		{
			lastBattleTime = Time.timeSinceLevelLoad;
			nextSkillSpec = spec;
			SetNextMotion(MotionType.attack);
			if (thisChar.isIdle || thisChar.isMoving || spec != null)
			{
				isMove = false;
				currentMotion = MotionType.attack;
				PlayNextMotion();
			}
		}
	}

	public void Damage(UnitControl fromUnit, Vector3 fromPos, Vector3 attackPos, float power, bool knockdown, bool knockback, bool critical, out bool isKilled)
	{
		isKilled = false;
		if (isDie)
		{
			isKilled = true;
		}
		else
		{
			if (!thisChar.allowDamage)
			{
				return;
			}
			if (critical)
			{
				power *= 1.5f;
				knockback = true;
			}
			lastBattleTime = Time.timeSinceLevelLoad;
			thisChar.PlayDamageSound();
			if (knockback || knockdown)
			{
				Vector3 vector = ((!(fromUnit == null)) ? (thisTrans.position - fromUnit.thisTrans.position) : (thisTrans.position - fromPos));
				vector.y = 0f;
				vector.Normalize();
				vector *= 5f;
				base.GetComponent<Rigidbody>().AddForceAtPosition(vector * 600f, (!(fromUnit == null)) ? fromUnit.thisTrans.position : fromPos);
				knockCtrl = fromUnit;
				SetNextMotion(MotionType.damage);
				PlayNextMotion();
			}
			float num = power - defensePower;
			if (num < 1f)
			{
				num = 1f;
			}
			if (PlayInfo.battleInfo.attackCastle == PlayInfo.battleInfo.defenseCastle && battleSide == 0)
			{
				num = 0f;
			}
			unitState.curHp -= num;
			if (unitState.curHp < 0f)
			{
				num += unitState.curHp;
			}
			if (fromUnit != null && fromUnit.isMainHero && UIIngameView.self != null)
			{
				Vector3 position = thisTrans.position;
				position.y += base.GetComponent<Collider>().bounds.max.y;
				float y = fromUnit.GetComponent<Collider>().bounds.max.y;
				if (position.y > y + 1f)
				{
					position.y = y + 1f;
				}
				int num2 = (int)num;
				if (num2 < 1)
				{
					num2 = 1;
				}
				UIIngameView.self.SetActiveIncXP(position, "-" + num2, critical);
			}
			if (isMainHero && UIIngameView.self != null)
			{
				Vector3 position2 = thisTrans.position;
				position2.y += base.GetComponent<Collider>().bounds.max.y;
				int num3 = (int)num;
				if (num3 < 1)
				{
					num3 = 1;
				}
				UIIngameView.self.SetActiveDecHP(position2, "-" + num3);
			}
			if (unitState.curHp <= 0f)
			{
				unitState.curHp = 0f;
				isKilled = true;
				Die(fromPos);
			}
			else if (fromUnit != null && (!isMainHero || (!isMove && !(targetCtrl != null) && currentMotion != MotionType.attack)))
			{
				if (targetCtrl == null)
				{
					SetTargetControl(fromUnit, null);
				}
				else if (Vector3.Distance(targetCtrl.thisTrans.position, thisTrans.position) > Vector3.Distance(fromUnit.thisTrans.position, thisTrans.position))
				{
					SetTargetControl(fromUnit, null);
				}
			}
			if (!(fromUnit != null) || !fromUnit.isMainHero)
			{
				return;
			}
			float num4 = 0f;
			num4 = ((float)unitState.level + 5f) * ((float)unitState.rankHp / 2f) + (float)unitState.level;
			num4 = num4 * num / (float)unitState.sumHp;
			if (num4 < 1f)
			{
				num4 = 1f;
			}
			bool isLevelUp = false;
			PlayInfo.IncUnitExp(PlayInfo.heroState, num4, out isLevelUp);
			if (isLevelUp)
			{
				if (fromUnit.effectHeroSkill == null)
				{
					fromUnit.effectHeroSkill = fromUnit.gameObject.GetComponent<EffectHeroSkill>();
				}
				fromUnit.effectHeroSkill.PlayExtraEffect(EffectHeroSkill.ExtraEffectType.levelUp);
				PlayInfo.soundManager.Play(ExtSoundManager.Effect2D.efs_levelup_effect, new Vector3(0f, 0f, 0f));
				UIIngameView.self.ShowLevelUp();
			}
		}
	}

	public void Healed(float incHp)
	{
		if (!isDie)
		{
			unitState.curHp += incHp;
			if (unitState.curHp > (float)unitState.sumHp)
			{
				unitState.curHp = unitState.sumHp;
			}
			UIIngameView.self.SetHealing(thisTrans, base.GetComponent<Collider>().bounds.size.y);
		}
	}

	public void DefenseUp(float incDef, float duration)
	{
		if (!isDie)
		{
			defensePower = (float)unitState.sumDefense + incDef;
			UIIngameView.self.SetDefenseUp(thisTrans, base.GetComponent<Collider>().bounds.size.y);
			StopCoroutine("CoroutineDefenseUp");
			StartCoroutine("CoroutineDefenseUp", duration);
		}
	}

	private IEnumerator CoroutineDefenseUp(float duration)
	{
		yield return new WaitForSeconds(duration);
		if (!isDie && isAwake)
		{
			defensePower = unitState.sumDefense;
		}
	}

	private IEnumerator CoroutineDefenseUpFinish(float duration)
	{
		yield return new WaitForSeconds(duration);
		UIIngameView.self.HideDefenseUpAnimationAll();
	}

	public void SlowMove(float rate, float duration)
	{
		if (!isDie)
		{
			downSpeed = thisChar.runSpeed * (100f - rate) / 100f;
			ChangeMoveSpeed(downSpeed);
			StopCoroutine("CoroutineSlowMoveFinish");
			StartCoroutine("CoroutineSlowMoveFinish", duration);
		}
	}

	private IEnumerator CoroutineSlowMoveFinish(float duration)
	{
		yield return new WaitForSeconds(duration);
		ChangeMoveSpeed(thisChar.runSpeed);
		downSpeed = 0f;
	}

	public void Die(Vector3 fromPos)
	{
		if (!isDie)
		{
			if (thisChar.charType != 0)
			{
				ProcMain.stageManager.DecAliveUser(this, battleSide);
			}
			SetNextMotion(MotionType.die);
			PlayNextMotion();
			if (!thisChar.isGate)
			{
				Vector3 vector = thisTrans.position - fromPos;
				vector.Normalize();
				vector *= 2f;
				vector += thisTrans.position;
				SetRotationTo(vector);
				MoveTo(vector, false, 7f);
			}
			isDie = true;
			base.GetComponent<Collider>().enabled = false;
			StopAllCoroutines();
			thisChar.PlayDieSound();
			StartCoroutine("CoroutineWaitDisappear");
		}
	}

	public void Victory()
	{
		if (thisChar.charType != 0 && isAwake)
		{
			ClearTarget();
			isFollow = false;
			isMove = false;
			StopAllCoroutines();
			thisChar.SetMotionType(MotionType.succeed);
		}
	}

	public void StandUp()
	{
		SetNextMotion(MotionType.standup);
	}

	public void OnAnimationFinish()
	{
		PlayNextMotion();
	}

	private void SetNextMotion(MotionType motion)
	{
		isWaitNextMotion = true;
		nextMotion = motion;
	}

	private void PlayNextMotion()
	{
		if (isDie || !isWaitNextMotion)
		{
			return;
		}
		isWaitNextMotion = false;
		currentMotion = nextMotion;
		if (weaponType != nextWeaponType || weaponCode != nextWeaponCode)
		{
			weaponType = nextWeaponType;
			weaponCode = nextWeaponCode;
			thisChar.SetWeapon(weaponType, weaponCode);
		}
		switch (currentMotion)
		{
		case MotionType.move:
			MoveTo(nextPos, nextDirRotate, nextSpeed);
			thisChar.SetMoveMode(nextMoveMode);
			Idle();
			break;
		case MotionType.damage:
			Prepare();
			StopCoroutine("WaitForKnockBack");
			StartCoroutine("WaitForKnockBack");
			break;
		case MotionType.attack:
			thisChar.SetAttackMode(nextAttackMode, nextAttackIndex);
			curSkillSpec = nextSkillSpec;
			if (nextAttackMode == UnitCharactor.AttackMode.skill)
			{
				SetAttackMode(UnitCharactor.AttackMode.normal, 0);
			}
			Prepare();
			StopCoroutine("CoroutineWaitAttackImpact");
			StartCoroutine("CoroutineWaitAttackImpact");
			break;
		}
		thisChar.SetMotionType(currentMotion);
	}

	private void Update()
	{
		if (!isAwake || !isMove)
		{
			return;
		}
		currentPos = base.transform.localPosition;
		currentPos += targetDir * Time.deltaTime;
		moveLen -= moveSpeed * Time.deltaTime;
		if (moveLen <= 0f)
		{
			if (moveLen < 0f)
			{
				currentPos += moveLen * targetDir.normalized;
			}
			isMove = false;
		}
		float num = Vector3.Distance(currentPos, base.transform.localPosition);
		if (num != 0f)
		{
			RaycastHit[] array = Physics.RaycastAll(base.transform.localPosition, (currentPos - base.transform.localPosition).normalized, num, LayerManager.layerDefault);
			if (array != null && array.Length > 0)
			{
				float num2 = num;
				RaycastHit[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					RaycastHit raycastHit = array2[i];
					if (num2 > raycastHit.distance)
					{
						num2 = raycastHit.distance;
					}
				}
				currentPos = base.transform.localPosition + (currentPos - base.transform.localPosition) * (num2 / num);
			}
			base.transform.localPosition = currentPos;
		}
		if (!isMove)
		{
			PlayNextMotion();
		}
	}

	private IEnumerator CoroutineFollowUnit()
	{
		while (isFollow)
		{
			if (targetCtrl == null)
			{
				ClearTarget();
				break;
			}
			if (!targetCtrl.isAwake)
			{
				ClearTarget();
				break;
			}
			if (targetCtrl.isDie)
			{
				ClearTarget();
				break;
			}
			if (targetCtrl.unitIdentify != targetIdentify)
			{
				ClearTarget();
				break;
			}
			Vector3 pos = targetCtrl.transform.position;
			float curLen = Vector3.Distance(pos, thisTrans.position);
			float nearLen = thisChar.boundRadius + targetCtrl.thisChar.boundRadius * 1.07f;
			SetRotationTo(targetCtrl.thisTrans.position);
			if (targetCtrl.thisChar.charType == UnitCharactor.CharactorType.npc)
			{
				nearLen += 0.5f;
			}
			else if (targetCtrl.battleSide == battleSide)
			{
				nearLen += 0.3f;
			}
			bool checkMove = false;
			if (curLen > nearLen)
			{
				checkMove = true;
			}
			if (targetCtrl.thisChar.isGate && curLen <= nearLen && targetCtrl.thisTrans.position.x + thisChar.boundRadius * 2f < thisTrans.position.x)
			{
				checkMove = true;
			}
			if (checkMove)
			{
				if (targetPos != pos || currentMotion != MotionType.move)
				{
					RunTo(pos);
				}
			}
			else
			{
				if (isMove)
				{
					Idle();
					if (thisChar.isIdle || thisChar.isMoving)
					{
						isMove = false;
						PlayNextMotion();
					}
				}
				if (onFollowFinish != null)
				{
					onFollowFinish(targetCtrl.thisChar);
				}
				if (battleSide != targetCtrl.battleSide && isAutoAttack && currentMotion != MotionType.attack)
				{
					Attack();
				}
				if (targetCtrl.thisChar.charType == UnitCharactor.CharactorType.npc)
				{
					isFollow = false;
					break;
				}
				yield return new WaitForSeconds(0.5f);
			}
			yield return new WaitForSeconds(0.2f);
		}
	}

	private IEnumerator CoroutineWaitAttackImpact()
	{
		float[] attackTiming = new float[1];
		bool attackSplash = false;
		UnitCharactor.AttackMode atkMode = thisChar.currentAttackMode;
		int atkIndex = thisChar.currentAttackIndex;
		attackTiming = AttackSystem.GetAttackTimingSec(thisChar.charType, thisChar.charIdx, thisChar.currentWeaponType, atkMode, atkIndex);
		float extraAttack = 0f;
		float splashLength = 1f;
		float splashRange = 0f;
		if (unitState.splashAttack)
		{
			attackSplash = true;
			splashLength = unitState.splashLength;
			splashRange = unitState.splashRange;
		}
		HeroSkill.SkillLevelSpec skillSpec = curSkillSpec;
		if (skillSpec == null)
		{
			if (thisChar.weaponType == UnitCharactor.WeaponType.bigsword)
			{
				attackSplash = true;
				splashLength = thisChar.boundRadius * 3f;
				splashRange = 90f;
			}
		}
		else
		{
			attackSplash = skillSpec.splash;
			extraAttack = skillSpec.attack;
			splashLength = skillSpec.length;
			splashRange = skillSpec.range * 0.5f;
		}
		int skillMode = 0;
		if (atkMode == UnitCharactor.AttackMode.skill)
		{
			int skillIdx = (int)(thisChar.currentWeaponType - 1) * 5 + atkIndex;
			switch ((HeroSkill.SkillType)skillIdx)
			{
			case HeroSkill.SkillType.skillHealing:
				skillMode = 1;
				break;
			case HeroSkill.SkillType.skillDefenseUp:
				skillMode = 2;
				break;
			case HeroSkill.SkillType.skillRush:
				skillMode = 3;
				break;
			}
			if (isMainHero)
			{
				if (effectHeroSkill == null)
				{
					effectHeroSkill = base.gameObject.GetComponent<EffectHeroSkill>();
				}
				effectHeroSkill.Play(skillIdx);
			}
		}
		for (int j = attackTiming.Length - 1; j > 0; j--)
		{
			attackTiming[j] -= attackTiming[j - 1];
		}
		bool isCritical = false;
		if (isMainHero && atkMode == UnitCharactor.AttackMode.normal && Random.Range(0, 100) < unitState.sumCri)
		{
			isCritical = true;
		}
		for (int i = 0; i < attackTiming.Length; i++)
		{
			yield return new WaitForSeconds(attackTiming[i]);
			if (atkMode != UnitCharactor.AttackMode.skill && (targetCtrl == null || !targetCtrl.isAwake || targetCtrl.isDie || targetIdentify != targetCtrl.unitIdentify))
			{
				break;
			}
			if (skillMode == 3)
			{
				if (skillSpec != null)
				{
					FastMove(skillSpec.length);
				}
				continue;
			}
			float skillDamage = attackPower + extraAttack + unitState.intellectual / 3f;
			bool knockback = false;
			if (skillSpec != null && skillSpec.knockback && i == attackTiming.Length - 1)
			{
				knockback = true;
			}
			if (skillMode == 0 && isFollow && targetCtrl != null)
			{
				bool isKilled2 = false;
				float len2 = Vector3.Distance(b: targetCtrl.thisTrans.position, a: thisTrans.position);
				float attLen = (thisChar.boundRadius + targetCtrl.thisChar.boundRadius) * 1.4f;
				if (attackSplash)
				{
					attLen = splashLength;
				}
				if (len2 <= attLen)
				{
					targetCtrl.Damage(this, thisTrans.position, thisTrans.position, skillDamage, false, knockback, isCritical, out isKilled2);
					if (isKilled2)
					{
						ClearTarget();
						if (isMainHero && i == attackTiming.Length - 1)
						{
							UnitCharactor[] mons = thisChar.charactorManager.GetSortedUnit(thisTrans.position, UnitCharactor.CharactorType.monster);
							UnitCharactor[] array = mons;
							foreach (UnitCharactor unitChar2 in array)
							{
								if (unitChar2.thisCtrl.isAwake && !unitChar2.thisCtrl.isDie)
								{
									SetTarget(unitChar2, null);
									InterfaceControl interControl = thisChar.charactorManager.GetComponent<InterfaceControl>();
									if (interControl != null)
									{
										interControl.SetTargetSelected(unitChar2);
									}
									break;
								}
							}
						}
					}
				}
			}
			if (skillMode == 1 && skillSpec != null)
			{
				Healed(skillSpec.hp);
			}
			if (skillMode == 2)
			{
				if (skillSpec != null)
				{
					DefenseUp(skillSpec.defense, skillSpec.duration);
				}
				StopCoroutine("CoroutineDefenseUpFinish");
				StartCoroutine("CoroutineDefenseUpFinish", skillSpec.duration);
			}
			if (!attackSplash)
			{
				continue;
			}
			bool isKilled = false;
			float myDir = thisTrans.rotation.eulerAngles.y;
			foreach (UnitCharactor unitChar in thisChar.charactorManager.charactors)
			{
				if (unitChar == thisChar || unitChar == targetCtrl || (skillMode == 0 && unitChar.thisCtrl.battleSide == battleSide) || ((skillMode == 1 || skillMode == 2) && unitChar.thisCtrl.battleSide != battleSide) || ((skillMode == 1 || skillMode == 2) && unitChar.isGate))
				{
					continue;
				}
				Transform trans = unitChar.thisCtrl.thisTrans;
				float len = Vector3.Distance(thisTrans.position, trans.position);
				if (!(len <= splashLength))
				{
					continue;
				}
				float targetDir = Quaternion.LookRotation(trans.position - thisTrans.position).eulerAngles.y;
				float dirCuv = Mathf.Abs((int)(targetDir - myDir));
				if (dirCuv > 180f)
				{
					dirCuv = 360f - dirCuv;
				}
				if (!(dirCuv <= splashRange))
				{
					continue;
				}
				switch (skillMode)
				{
				case 0:
					unitChar.thisCtrl.Damage(this, thisTrans.position, thisTrans.position, skillDamage, false, knockback, isCritical, out isKilled);
					break;
				case 1:
					if (unitChar.thisCtrl.battleSide == battleSide && skillSpec != null && !unitChar.isGate)
					{
						unitChar.thisCtrl.Healed(skillSpec.hp);
					}
					break;
				case 2:
					if (unitChar.thisCtrl.battleSide == battleSide && skillSpec != null && !unitChar.isGate)
					{
						unitChar.thisCtrl.DefenseUp(skillSpec.defense, skillSpec.duration);
					}
					break;
				}
			}
		}
	}

	private IEnumerator WaitForKnockBack()
	{
		yield return new WaitForSeconds(1f);
		if (knockCtrl != null)
		{
			SetTargetControl(knockCtrl, null);
			knockCtrl = null;
		}
	}

	private IEnumerator CoroutineWaitDisappear()
	{
		if (thisChar.charType == UnitCharactor.CharactorType.lord)
		{
			yield return new WaitForSeconds(LordManager.lordDefault[thisChar.charIdx].regenTime);
		}
		else
		{
			yield return new WaitForSeconds(3f);
		}
		if (!isMainHero && !thisChar.isGate)
		{
			base.gameObject.SetActive(false);
		}
		thisChar.isAwake = false;
		isAwake = false;
	}
}
