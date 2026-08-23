using System.Collections;
using UnityEngine;

public class UnitCharactor : MonoBehaviour
{
	public enum CharactorType
	{
		hero = 0,
		monster = 1,
		npc = 2,
		soldier = 3,
		lord = 4,
		max = 5
	}

	public enum WeaponType
	{
		none = 0,
		onehand = 1,
		doublehand = 2,
		bigsword = 3,
		fixedunit = 4,
		max = 5
	}

	public enum AnimationType
	{
		idle = 0,
		idle_event = 1,
		prepare = 2,
		prepare_event = 3,
		walk = 4,
		run = 5,
		attacknormal = 6,
		attackskill = 7,
		attackspecial = 8,
		damage = 9,
		die = 10,
		standup = 11,
		win = 12,
		max = 13
	}

	public enum AttackMode
	{
		normal = 0,
		skill = 1,
		special = 2,
		max = 3
	}

	public enum MoveMode
	{
		slip = 0,
		walk = 1,
		run = 2,
		max = 3
	}

	public const float defaultRunSpeed = 5f;

	public UnitControl thisCtrl;

	public int[] maxAnimation;

	public WeaponType weaponType;

	public int weaponCode;

	public AnimationType animationType;

	public int animationIndex;

	protected AttackMode attackMode;

	protected int attackIndex;

	protected MoveMode moveMode = MoveMode.walk;

	protected UnitControl.MotionType currentMotion;

	public bool isAwake = true;

	public bool isRemove;

	public bool isGate;

	public CharactorManager charactorManager;

	public CharactorType charType;

	public int charIdx;

	public float animationLength;

	public bool allowMove = true;

	public bool allowAttack = true;

	public bool allowDamage = true;

	public float walkSpeed = 1.2f;

	public float runSpeed = 5f;

	public HeroModel heroModel;

	public WeaponManager weaponManager;

	public float boundRadius = 1f;

	private AudioSource efsWalk;

	private AudioSource efsSource;

	public WeaponType currentWeaponType
	{
		get
		{
			return weaponType;
		}
	}

	public AttackMode currentAttackMode
	{
		get
		{
			return attackMode;
		}
	}

	public int currentAttackIndex
	{
		get
		{
			return attackIndex;
		}
	}

	public bool isIdle
	{
		get
		{
			return animationType == AnimationType.idle || animationType == AnimationType.prepare || animationType == AnimationType.idle_event || animationType == AnimationType.prepare_event;
		}
	}

	public bool isMoving
	{
		get
		{
			return animationType == AnimationType.run || animationType == AnimationType.walk;
		}
	}

	private void Start()
	{
		Bounds bounds = base.gameObject.GetComponent<Collider>().bounds;
		float num = ((!(bounds.size.x > bounds.size.z)) ? bounds.size.z : bounds.size.x);
		boundRadius = num;
		efsSource = base.gameObject.AddComponent<AudioSource>();
	}

	public void SetWeapon(WeaponType weapon, int code)
	{
		if (weaponType == weapon && weaponCode == code)
		{
			return;
		}
		weaponType = weapon;
		weaponCode = code;
		if (charType == CharactorType.hero)
		{
			if (heroModel == null)
			{
				heroModel = base.gameObject.GetComponent<HeroModel>();
			}
			if (heroModel != null && weaponManager != null)
			{
				heroModel.SetWeapon(weaponManager, weapon, code);
				if (heroModel.bandLeft != null)
				{
					heroModel.bandLeft.GeneratePreTrail(this);
				}
				if (heroModel.bandRight != null)
				{
					heroModel.bandRight.GeneratePreTrail(this);
				}
			}
			UnitItem unitItem = PlayInfo.itemManager.FindItem(ItemManager.ItemType.weapon, code);
			if (unitItem != null)
			{
				runSpeed = 5f * thisCtrl.unitState.speed * (1f + unitItem.ability.speed);
				if (animationType == AnimationType.run)
				{
					thisCtrl.ChangeMoveSpeed(runSpeed);
				}
			}
		}
		SetAnimation();
	}

	public void SetAttackMode(AttackMode mode, int idx)
	{
		attackMode = mode;
		attackIndex = idx;
	}

	public void SetMoveMode(MoveMode mode)
	{
		if (moveMode != mode)
		{
			moveMode = mode;
			if (currentMotion == UnitControl.MotionType.move)
			{
				ResetMotion();
			}
		}
	}

	public void SetMotionType(UnitControl.MotionType motion)
	{
		currentMotion = motion;
		ResetMotion();
	}

	private void RandomAnimationIndex()
	{
		int num = (int)animationType;
		int num2 = maxAnimation[num];
		if (num2 > 1)
		{
			animationIndex = Random.Range(0, num2);
		}
		else
		{
			animationIndex = 0;
		}
	}

	private void ResetMotion()
	{
		if (efsWalk != null && (currentMotion != UnitControl.MotionType.move || moveMode != MoveMode.run))
		{
			efsWalk.Stop();
		}
		switch (currentMotion)
		{
		case UnitControl.MotionType.idle:
			animationType = AnimationType.idle;
			RandomAnimationIndex();
			break;
		case UnitControl.MotionType.prepare:
			animationType = AnimationType.prepare;
			RandomAnimationIndex();
			break;
		case UnitControl.MotionType.move:
			switch (moveMode)
			{
			case MoveMode.slip:
				animationType = AnimationType.damage;
				break;
			case MoveMode.walk:
				animationType = AnimationType.walk;
				break;
			case MoveMode.run:
				animationType = AnimationType.run;
				if (!thisCtrl.isMainHero)
				{
					break;
				}
				if (efsWalk == null)
				{
					if (ProcMain.stageManager.stageInfo.stageType == StageManager.StageType.castle)
					{
						efsWalk = PlayInfo.soundManager.Play(ExtSoundManager.Effect2D.efs_me_townwalk, thisCtrl.thisTrans.position, true);
					}
					else
					{
						efsWalk = PlayInfo.soundManager.Play(ExtSoundManager.Effect2D.efs_me_battlewalk, thisCtrl.thisTrans.position, true);
					}
					efsWalk.volume = (float)UserSetting.volumeEffect / 100f;
				}
				else if (!efsWalk.isPlaying)
				{
					efsWalk.volume = (float)UserSetting.volumeEffect / 100f;
					efsWalk.Play();
				}
				break;
			}
			RandomAnimationIndex();
			break;
		case UnitControl.MotionType.attack:
			switch (attackMode)
			{
			case AttackMode.normal:
				animationType = AnimationType.attacknormal;
				RandomAnimationIndex();
				PlayAttackSound();
				break;
			case AttackMode.skill:
				animationType = AnimationType.attackskill;
				animationIndex = attackIndex;
				if (!thisCtrl.isMainHero)
				{
					break;
				}
				switch (weaponType)
				{
				case WeaponType.onehand:
					switch (attackIndex)
					{
					case 0:
						PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect2D.efs_onehand_skill1, efsSource, 0f);
						break;
					case 1:
						PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect2D.efs_onehand_skill2, efsSource, 0f);
						break;
					case 2:
						PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect2D.efs_onehand_skill3, efsSource, 0f);
						break;
					case 3:
						PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect2D.efs_onehand_skill4, efsSource, 0f);
						break;
					case 4:
						PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect2D.efs_onehand_skill5, efsSource, 0f);
						break;
					}
					break;
				case WeaponType.doublehand:
					switch (attackIndex)
					{
					case 0:
						PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect2D.efs_doublehand_skill1, efsSource, 0f);
						break;
					case 1:
						PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect2D.efs_doublehand_skill2, efsSource, 0f);
						break;
					case 2:
						PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect2D.efs_doublehand_skill3, efsSource, 0f);
						break;
					case 3:
						PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect2D.efs_doublehand_skill4, efsSource, 0f);
						break;
					case 4:
						PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect2D.efs_doublehand_skill5, efsSource, 0f);
						break;
					}
					break;
				case WeaponType.bigsword:
					switch (attackIndex)
					{
					case 0:
						PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect2D.efs_bigsword_skill1, efsSource, 0f);
						break;
					case 1:
						PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect2D.efs_bigsword_skill2, efsSource, 0f);
						break;
					case 2:
						PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect2D.efs_bigsword_skill3, efsSource, 0f);
						break;
					case 3:
						PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect2D.efs_bigsword_skill4, efsSource, 0f);
						break;
					case 4:
						PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect2D.efs_bigsword_skill5, efsSource, 0f);
						break;
					}
					break;
				}
				break;
			case AttackMode.special:
				animationType = AnimationType.attackspecial;
				animationIndex = attackIndex;
				break;
			}
			break;
		case UnitControl.MotionType.damage:
			animationType = AnimationType.damage;
			RandomAnimationIndex();
			break;
		case UnitControl.MotionType.die:
			animationType = AnimationType.die;
			RandomAnimationIndex();
			break;
		case UnitControl.MotionType.standup:
			animationType = AnimationType.standup;
			RandomAnimationIndex();
			break;
		case UnitControl.MotionType.succeed:
			animationType = AnimationType.win;
			RandomAnimationIndex();
			break;
		}
		int num = maxAnimation[(int)animationType];
		if (num == 0)
		{
			currentMotion = UnitControl.MotionType.idle;
			animationType = AnimationType.idle;
			animationIndex = 0;
		}
		else if (animationIndex >= num)
		{
			animationIndex = num - 1;
		}
		SetAnimation();
	}

	private void PlayAttackSound()
	{
		if (thisCtrl.isMainHero)
		{
			switch (weaponType)
			{
			case WeaponType.onehand:
				PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect2D.efs_onehand_1hit, efsSource, 0f);
				break;
			case WeaponType.doublehand:
				PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect2D.efs_twin_1hit, efsSource, 0f);
				break;
			case WeaponType.bigsword:
				PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect2D.efs_twohand_1hit, efsSource, 0f);
				break;
			}
		}
		else if (charType == CharactorType.lord)
		{
			switch (charIdx)
			{
			case 0:
				PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect3D.efs_strategist_atk, efsSource, 0f);
				break;
			case 1:
				PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect3D.efs_swordsman_atk, efsSource, 0f);
				break;
			case 2:
				PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect3D.efs_tanker_atk, efsSource, 0f);
				break;
			}
		}
		else if (charType == CharactorType.soldier)
		{
			float[] attackTimingSec = AttackSystem.GetAttackTimingSec(charType, charIdx, WeaponType.fixedunit, AttackMode.normal, animationIndex);
			float delay = attackTimingSec[0] - 0.2f;
			switch (charIdx)
			{
			case 0:
				PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect3D.efs_spearman_atk, efsSource, delay);
				break;
			case 1:
				PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect3D.efs_scout_atk, efsSource, delay);
				break;
			case 2:
				PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect3D.efs_warrior_atk, efsSource, delay);
				break;
			case 3:
				PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect3D.efs_gladiator_atk, efsSource, delay);
				break;
			case 4:
				PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect3D.efs_knight_atk, efsSource, delay);
				break;
			}
		}
		else if (charType == CharactorType.monster)
		{
			float[] attackTimingSec2 = AttackSystem.GetAttackTimingSec(charType, charIdx, WeaponType.fixedunit, AttackMode.normal, animationIndex);
			float delay2 = attackTimingSec2[0] - 0.2f;
			switch (charIdx)
			{
			case 0:
				PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect3D.efs_mon_atk1, efsSource, delay2);
				break;
			case 1:
			case 2:
				PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect3D.efs_mon_atk2, efsSource, delay2);
				break;
			case 3:
			case 4:
				PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect3D.efs_mon_atk3, efsSource, delay2);
				break;
			case 7:
			case 10:
			case 12:
			case 14:
				PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect3D.efs_mon_atk4, efsSource, delay2);
				break;
			case 9:
				PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect3D.efs_mon_atk5, efsSource, delay2);
				break;
			case 11:
			case 13:
				PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect3D.efs_mon_atk6, efsSource, delay2);
				break;
			case 8:
			case 15:
				PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect3D.efs_mon_atk7, efsSource, delay2);
				break;
			case 16:
				PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect3D.efs_mon_atk8, efsSource, delay2);
				break;
			case 5:
			case 6:
				PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect3D.efs_mon_atk9, efsSource, delay2);
				break;
			}
		}
	}

	public void PlayDieSound()
	{
		if (isGate)
		{
			PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect2D.efs_gate_collapse, efsSource, 0f);
			return;
		}
		switch (charType)
		{
		case CharactorType.hero:
			PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect2D.efs_me_death, efsSource, 0f);
			break;
		case CharactorType.lord:
			switch (charIdx)
			{
			case 0:
				PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect3D.efs_strategist_death, efsSource, 0f);
				break;
			case 1:
				PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect3D.efs_swordsman_death, efsSource, 0f);
				break;
			case 2:
				PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect3D.efs_tanker_death, efsSource, 0f);
				break;
			}
			break;
		case CharactorType.soldier:
			switch (charIdx)
			{
			case 0:
				PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect3D.efs_spearman_death, efsSource, 0f);
				break;
			case 1:
				PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect3D.efs_scout_death, efsSource, 0f);
				break;
			case 2:
				PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect3D.efs_warrior_death, efsSource, 0f);
				break;
			case 3:
				PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect3D.efs_gladiator_death, efsSource, 0f);
				break;
			case 4:
				PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect3D.efs_knight_death, efsSource, 0f);
				break;
			}
			break;
		case CharactorType.monster:
			PlayInfo.soundManager.PlaySource((ExtSoundManager.Effect3D)(19 + charIdx), efsSource, 0f);
			break;
		case CharactorType.npc:
			break;
		}
	}

	public void PlayDamageSound()
	{
		if (isGate)
		{
			PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect3D.efs_hit_gate, efsSource, 0f);
			return;
		}
		CharactorType charactorType = charType;
		if (charactorType == CharactorType.monster)
		{
			switch (charIdx)
			{
			case 8:
			case 10:
			case 11:
			case 15:
			case 16:
				PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect3D.efs_mon_hit2, efsSource, 0f);
				break;
			default:
				PlayInfo.soundManager.PlaySource(ExtSoundManager.Effect3D.efs_mon_hit1, efsSource, 0f);
				break;
			}
		}
	}

	private bool isAnimation(AnimationType type)
	{
		int num = maxAnimation[(int)type];
		return num > 0;
	}

	public string GetAnimationName(AnimationType type, int aniIdx)
	{
		string empty = string.Empty;
		int num = maxAnimation[(int)type];
		empty = ((weaponType != WeaponType.fixedunit && weaponType != 0) ? (weaponType.ToString() + "_" + type) : type.ToString());
		if (num == 0)
		{
			return string.Empty;
		}
		if (aniIdx >= num)
		{
			aniIdx = num - 1;
		}
		if (num > 1)
		{
			empty += string.Format("{0:00}", aniIdx + 1);
		}
		return empty;
	}

	private void SetAnimation()
	{
		string animationName = GetAnimationName(animationType, animationIndex);
		bool flag = false;
		AnimationState animationState = null;
		Animation animation = base.GetComponent<Animation>();
		if (isGate || animation == null)
		{
			Animation[] componentsInChildren = GetComponentsInChildren<Animation>();
			if (componentsInChildren != null && componentsInChildren.Length > 0)
			{
				animation = componentsInChildren[0];
			}
		}
		animationState = animation[animationName];
		if (animationState != null)
		{
			flag = true;
		}
		StopCoroutine("WaitForAnimationFinishEvent");
		if (!flag)
		{
			return;
		}
		animationLength = animationState.length / 0.25f;
		if (animationType == AnimationType.idle || animationType == AnimationType.prepare)
		{
			animation.CrossFade(animationName);
			StopCoroutine("IdlePrepareEvent");
			StartCoroutine("IdlePrepareEvent");
		}
		else if (animationType == AnimationType.win)
		{
			animation.CrossFade(animationName);
		}
		else
		{
			if (animationType == AnimationType.attacknormal || animationType == AnimationType.attackskill || animationType == AnimationType.attackspecial)
			{
				animation[animationName].time = 0f;
			}
			animation.Play(animationName);
			if (animationType != AnimationType.run && animationType != AnimationType.walk && animationType != AnimationType.die)
			{
				StartCoroutine("WaitForAnimationFinishEvent");
			}
		}
		if (animationType == AnimationType.attacknormal || animationType == AnimationType.attackskill || animationType == AnimationType.attackspecial)
		{
			if (heroModel != null)
			{
				if (heroModel.bandLeft != null)
				{
					heroModel.bandLeft.Visible = true;
				}
				if (heroModel.bandRight != null)
				{
					heroModel.bandRight.Visible = true;
				}
			}
		}
		else if (heroModel != null)
		{
			if (heroModel.bandLeft != null && heroModel.bandLeft.Visible)
			{
				heroModel.bandLeft.Visible = false;
			}
			if (heroModel.bandRight != null && heroModel.bandRight.Visible)
			{
				heroModel.bandRight.Visible = false;
			}
		}
	}

	private IEnumerator WaitForAnimationFinishEvent()
	{
		yield return new WaitForSeconds(animationLength);
		if (thisCtrl != null)
		{
			thisCtrl.OnAnimationFinish();
		}
	}

	private IEnumerator IdlePrepareEvent()
	{
		float eventDelay = 2f + (float)Random.Range(0, 5);
		yield return new WaitForSeconds(eventDelay);
		if (animationType != 0 && animationType != AnimationType.prepare)
		{
			yield break;
		}
		AnimationType aniType2 = ((animationType == AnimationType.idle) ? AnimationType.idle_event : AnimationType.prepare_event);
		if (!isAnimation(aniType2))
		{
			yield break;
		}
		animationType = aniType2;
		RandomAnimationIndex();
		SetAnimation();
		yield return new WaitForSeconds(animationLength);
		if (animationType == AnimationType.idle_event || animationType == AnimationType.prepare_event)
		{
			aniType2 = ((animationType != AnimationType.idle_event) ? AnimationType.prepare : AnimationType.idle);
			if (isAnimation(aniType2))
			{
				animationType = aniType2;
				RandomAnimationIndex();
				SetAnimation();
			}
		}
	}
}
