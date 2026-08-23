using System.Collections.Generic;
using UnityEngine;

public class AuiSpriteAnimation : AuiSprite
{
	public delegate void OnEvent();

	public float drawFPS = 1f;

	public bool isLoop = true;

	public bool isAutoHide;

	public bool isPlaying = true;

	private float passTime;

	private float frameDelay;

	private int[] eventFrames;

	private OnEvent[] onEvents;

	public void SetEvent(OnEvent eventProc, int eventFrame)
	{
		List<int> list = new List<int>(eventFrames);
		list.Add(eventFrame);
		eventFrames = list.ToArray();
		List<OnEvent> list2 = new List<OnEvent>(onEvents);
		list2.Add(eventProc);
		onEvents = list2.ToArray();
	}

	private void Start()
	{
		OnStart();
		frameDelay = 1f / drawFPS;
		MakeSpriteObject();
	}

	private void Update()
	{
		if (!isVisible || !isPlaying)
		{
			return;
		}
		frameDelay = 1f / drawFPS;
		passTime += Time.deltaTime;
		if (!(passTime >= frameDelay))
		{
			return;
		}
		passTime -= frameDelay;
		int num = curFrame + 1;
		if (eventFrames != null)
		{
			for (int i = 0; i < eventFrames.Length; i++)
			{
				if (num == eventFrames[i])
				{
					onEvents[i]();
				}
			}
		}
		if (num >= frameCount)
		{
			if (isLoop)
			{
				curFrame = 0;
			}
			else
			{
				StopAnimation(isAutoHide);
			}
			num = curFrame;
		}
		SetFrame(num);
	}

	public void StartAnimation(bool loop, bool autoHide)
	{
		isLoop = loop;
		isAutoHide = autoHide;
		isPlaying = true;
		isVisible = true;
		if (sprObject != null)
		{
			sprObject.SetActive(true);
		}
	}

	public void StartAnimation(int startFrame, bool loop, bool autoHide)
	{
		SetFrame(startFrame);
		StartAnimation(loop, autoHide);
	}

	public void StopAnimation(bool hide)
	{
		if (hide)
		{
			if (sprObject != null)
			{
				sprObject.SetActive(false);
			}
			isVisible = false;
		}
		isPlaying = false;
	}
}
