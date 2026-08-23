using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuiButton : AuiSprite
{
	public class NoClickArea
	{
		public int layer;

		public Rect area;
	}

	public delegate void OnButtonClick(AuiButton sender);

	private const int materialNormal = 0;

	private const int materialDown = 1;

	public Camera uiCamera;

	public OnButtonClick onButtonClick;

	public int buttonTag;

	public bool isTop;

	public bool isMostTop;

	public bool isModal;

	public bool isClickOnPause;

	private Vector3 posMin = new Vector3(0f, 0f);

	private Vector3 posMax = new Vector3(0f, 0f);

	private float timeClickOnPause;

	public static bool topActive = false;

	public static bool mostTopActive = false;

	public static bool modalActive = false;

	public static List<AuiButton> buttonList = new List<AuiButton>();

	public bool isWaitForButtonUp;

	private void Start()
	{
		OnStart();
		MakeSpriteObject();
		if (frameCount < 2)
		{
			Debug.LogError("Set Sprite GameObjects!! [AuiButton]");
			return;
		}
		if (uiCamera == null)
		{
			uiCamera = Camera.main;
		}
		if (uiCamera == null)
		{
			Debug.LogWarning("Set UICamera!! [AuiButton]");
		}
		ResetRect();
		VisibleNormal();
		buttonList.Add(this);
	}

	private void OnDestroy()
	{
		buttonList.Remove(this);
	}

	public bool CheckButtonInSide(float x, float y)
	{
		if (x >= posMin.x && x <= posMax.x && y >= posMin.y && y <= posMax.y)
		{
			return true;
		}
		return false;
	}

	public bool CheckButtonDown(Vector3 posScreen)
	{
		if (modalActive && !isModal)
		{
			return false;
		}
		if (mostTopActive && !isMostTop && !isModal)
		{
			return false;
		}
		if (topActive && !isTop && !isMostTop && !isModal)
		{
			return false;
		}
		ResetRect();
		if (posScreen.x >= posMin.x && posScreen.x <= posMax.x && posScreen.y >= posMin.y && posScreen.y <= posMax.y)
		{
			if (base.gameObject.activeInHierarchy)
			{
				VisibleDown();
				isWaitForButtonUp = true;
				if (Time.timeScale == 0f)
				{
					isClickOnPause = true;
					timeClickOnPause = Time.realtimeSinceStartup;
				}
				else
				{
					StartCoroutine("WaitForButtonUp");
				}
			}
			return true;
		}
		return false;
	}

	public void WaitForButtonUp_OnPause()
	{
		if (isClickOnPause)
		{
			if (!isWaitForButtonUp)
			{
				isClickOnPause = false;
			}
			else if (Time.realtimeSinceStartup - timeClickOnPause > 0.1f)
			{
				VisibleNormal();
				ForcedButtonClicked();
				isClickOnPause = false;
			}
		}
	}

	public void ForcedButtonClicked()
	{
		if (isWaitForButtonUp)
		{
			isWaitForButtonUp = false;
			VisibleNormal();
			if (onButtonClick != null)
			{
				onButtonClick(this);
			}
		}
	}

	private IEnumerator WaitForButtonUp()
	{
		yield return new WaitForSeconds(0.1f);
		if (isWaitForButtonUp)
		{
			VisibleNormal();
			yield return 1;
			if (isWaitForButtonUp)
			{
				ForcedButtonClicked();
			}
		}
	}

	private void VisibleNormal()
	{
		SetFrame(0);
	}

	private void VisibleDown()
	{
		SetFrame(1);
	}

	public void ResetRect()
	{
		if (!(uiCamera == null))
		{
			Transform transform = sprTrasform;
			Vector3 lossyScale = base.transform.lossyScale;
			posMin = transform.position - orgSize * 0.5f * lossyScale.x;
			posMax = transform.position + orgSize * 0.5f * lossyScale.y;
			Vector3 vector = uiCamera.WorldToScreenPoint(posMin);
			Vector3 vector2 = uiCamera.WorldToScreenPoint(posMax);
			posMin.x = Mathf.Min(vector.x, vector2.x);
			posMax.x = Mathf.Max(vector.x, vector2.x);
			posMin.y = Mathf.Min(vector.y, vector2.y);
			posMax.y = Mathf.Max(vector.y, vector2.y);
		}
	}

	public static void SetCameraAllChild(Transform trans, Camera cam)
	{
		foreach (Transform tran in trans)
		{
			AuiButton component = tran.GetComponent<AuiButton>();
			if (component != null)
			{
				component.uiCamera = cam;
			}
			if (tran.childCount > 0)
			{
				SetCameraAllChild(tran, cam);
			}
		}
	}

	public static void SetTopAllChild(Transform trans)
	{
		foreach (Transform tran in trans)
		{
			AuiButton component = tran.GetComponent<AuiButton>();
			if (component != null)
			{
				component.isTop = true;
			}
			if (tran.childCount > 0)
			{
				SetTopAllChild(tran);
			}
		}
	}

	public static void SetMostTopAllChild(Transform trans)
	{
		foreach (Transform tran in trans)
		{
			AuiButton component = tran.GetComponent<AuiButton>();
			if (component != null)
			{
				component.isMostTop = true;
			}
			if (tran.childCount > 0)
			{
				SetMostTopAllChild(tran);
			}
		}
	}

	public static void SetModalAllChild(Transform trans)
	{
		foreach (Transform tran in trans)
		{
			AuiButton component = tran.GetComponent<AuiButton>();
			if (component != null)
			{
				component.isModal = true;
			}
			if (tran.childCount > 0)
			{
				SetModalAllChild(tran);
			}
		}
	}

	public static void SetEnableAll(Transform trans, bool enable)
	{
		foreach (Transform tran in trans)
		{
			AuiButton component = tran.GetComponent<AuiButton>();
			if (component != null)
			{
				component.enabled = enable;
			}
			if (tran.childCount > 0)
			{
				SetEnableAll(tran, enable);
			}
		}
	}
}
