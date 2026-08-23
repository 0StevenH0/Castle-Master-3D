using UnityEngine;

public class ExtSoundManager
{
	public enum Effect3D
	{
		efs_spearman_atk = 0,
		efs_scout_atk = 1,
		efs_warrior_atk = 2,
		efs_gladiator_atk = 3,
		efs_knight_atk = 4,
		efs_spearman_death = 5,
		efs_scout_death = 6,
		efs_warrior_death = 7,
		efs_gladiator_death = 8,
		efs_knight_death = 9,
		efs_mon_atk1 = 10,
		efs_mon_atk2 = 11,
		efs_mon_atk3 = 12,
		efs_mon_atk4 = 13,
		efs_mon_atk5 = 14,
		efs_mon_atk6 = 15,
		efs_mon_atk7 = 16,
		efs_mon_atk8 = 17,
		efs_mon_atk9 = 18,
		efs_mon_death1 = 19,
		efs_mon_death2 = 20,
		efs_mon_death3 = 21,
		efs_mon_death4 = 22,
		efs_mon_death5 = 23,
		efs_mon_death6 = 24,
		efs_mon_death7 = 25,
		efs_mon_death8 = 26,
		efs_mon_death9 = 27,
		efs_mon_death10 = 28,
		efs_mon_death11 = 29,
		efs_mon_death12 = 30,
		efs_mon_death13 = 31,
		efs_mon_death14 = 32,
		efs_mon_death15 = 33,
		efs_mon_death16 = 34,
		efs_mon_death17 = 35,
		efs_strategist_atk = 36,
		efs_strategist_death = 37,
		efs_swordsman_atk = 38,
		efs_swordsman_death = 39,
		efs_tanker_atk = 40,
		efs_tanker_death = 41,
		efs_hit_gate = 42,
		efs_mon_hit1 = 43,
		efs_mon_hit2 = 44,
		efs_boobytrap_gaia = 45,
		efs_boobytrap_ghost = 46,
		efs_boobytrap_lightning = 47,
		efs_boobytrap_meteorite = 48,
		efs_boobytrap_slowtime = 49,
		efs_boobytrap_thornbush = 50,
		efs_boobytrap_tornado = 51,
		efs_boobytrap_enermy = 52,
		max = 53
	}

	public enum Effect2D
	{
		efs_battle_attack = 0,
		efs_battle_defense = 1,
		efs_victory = 2,
		efs_defeat = 3,
		efs_drink_potion = 4,
		efs_item_equip = 5,
		efs_onehand_1hit = 6,
		efs_twin_1hit = 7,
		efs_twohand_1hit = 8,
		efs_crowd_cheer = 9,
		efs_onehand_skill1 = 10,
		efs_onehand_skill2 = 11,
		efs_onehand_skill3 = 12,
		efs_onehand_skill4 = 13,
		efs_onehand_skill5 = 14,
		efs_doublehand_skill1 = 15,
		efs_doublehand_skill2 = 16,
		efs_doublehand_skill3 = 17,
		efs_doublehand_skill4 = 18,
		efs_doublehand_skill5 = 19,
		efs_bigsword_skill1 = 20,
		efs_bigsword_skill2 = 21,
		efs_bigsword_skill3 = 22,
		efs_bigsword_skill4 = 23,
		efs_bigsword_skill5 = 24,
		efs_gate_collapse = 25,
		efs_me_townwalk = 26,
		efs_me_battlewalk = 27,
		efs_accessory = 28,
		efs_armor = 29,
		efs_weapon = 30,
		efs_captain = 31,
		efs_skillmaster = 32,
		efs_gatekeeper = 33,
		efs_levelup_effect = 34,
		efs_itembuy = 35,
		efs_zd = 36,
		efs_princess_heart = 37,
		efs_princess_x = 38,
		efs_priest = 39,
		efs_secretary = 40,
		efs_skill_button = 41,
		efs_rebirth = 42,
		efs_me_death = 43,
		efs_castle_attack = 44,
		max = 45
	}

	private const string sound3DPath = "Sound/effect3d";

	private const string sound2DPath = "Sound/effect2d";

	private const float defaultMinDistance = 3f;

	private const float defaultMaxDistance = 40f;

	private const int maxMultiSound = 5;

	private AudioClip[] audioClip3D;

	private AudioClip[] audioClip2D;

	private AudioSource[] audioSource = new AudioSource[5];

	public void Init()
	{
		int num = 53;
		audioClip3D = new AudioClip[num];
		for (int i = 0; i < num; i++)
		{
			audioClip3D[i] = ResourceManager.Load("Sound/effect3d", ((Effect3D)i).ToString(), typeof(AudioClip)) as AudioClip;
		}
		num = 45;
		audioClip2D = new AudioClip[num];
		for (int j = 0; j < num; j++)
		{
			audioClip2D[j] = ResourceManager.Load("Sound/effect2d", ((Effect2D)j).ToString(), typeof(AudioClip)) as AudioClip;
		}
	}

	public int GetEmptySource()
	{
		for (int i = 0; i < 5; i++)
		{
			if (audioSource[i] == null)
			{
				return i;
			}
			if (!audioSource[i].isPlaying)
			{
				audioSource[i].enabled = false;
				return i;
			}
		}
		return -1;
	}

	public void Play(Effect3D sound, Vector3 pos)
	{
		Play(audioClip3D[(int)sound], pos, 0f, false);
	}

	public void Play(Effect2D sound, Vector3 pos)
	{
		Play(audioClip2D[(int)sound], pos, 0f, false);
	}

	public void Play(Effect3D sound, Vector3 pos, float delay)
	{
		if (delay < 0f)
		{
			delay = 0f;
		}
		Play(audioClip3D[(int)sound], pos, delay, false);
	}

	public void Play(Effect2D sound, Vector3 pos, float delay)
	{
		if (delay < 0f)
		{
			delay = 0f;
		}
		Play(audioClip2D[(int)sound], pos, delay, false);
	}

	public AudioSource Play(Effect3D sound, Vector3 pos, bool loop)
	{
		return Play(audioClip3D[(int)sound], pos, 0f, loop);
	}

	public AudioSource Play(Effect2D sound, Vector3 pos, bool loop)
	{
		return Play(audioClip2D[(int)sound], pos, 0f, loop);
	}

	private AudioSource Play(AudioClip clip, Vector3 pos, float delay, bool loop)
	{
		GameObject gameObject = new GameObject("OneShotAudio");
		gameObject.transform.position = pos;
		AudioSource audioSource = gameObject.AddComponent<AudioSource>();
		audioSource.minDistance = 3f;
		audioSource.maxDistance = 40f * ((UserSetting.quality != 0) ? 1f : 0.5f);
		audioSource.clip = clip;
		audioSource.volume = (float)UserSetting.volumeEffect / 100f;
		if (loop)
		{
			audioSource.loop = true;
		}
		audioSource.Play((ulong)((float)clip.samples * delay));
		if (!loop)
		{
			Object.Destroy(gameObject, clip.length);
		}
		return audioSource;
	}

	public void PlaySource(Effect3D sound, AudioSource src, float delay)
	{
		if (UserSetting.quality == UserSetting.GraphicsQuality.fast)
		{
			int emptySource = GetEmptySource();
			if (emptySource == -1)
			{
				return;
			}
			audioSource[emptySource] = src;
		}
		PlaySource(audioClip3D[(int)sound], src, delay);
	}

	public void PlaySource(Effect2D sound, AudioSource src, float delay)
	{
		PlaySource(audioClip2D[(int)sound], src, delay);
	}

	private void PlaySource(AudioClip clip, AudioSource src, float delay)
	{
		src.enabled = true;
		src.minDistance = 3f;
		src.maxDistance = 40f * ((UserSetting.quality != 0) ? 1f : 0.5f);
		src.clip = clip;
		src.volume = (float)UserSetting.volumeEffect / 100f;
		src.Play((ulong)((float)clip.samples * delay));
	}
}
