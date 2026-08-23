using System.Collections;
using UnityEngine;

public class UIMap : MonoBehaviour
{
	public const int maxNoClickArea = 4;

	public const int areaTopMenu = 0;

	public const int areaControlBox = 1;

	public const int areaCancelBox = 2;

	public const int areaAttackAlert = 3;

	public CastleInterface fabCastle;

	public Transform mapTrans;

	public GameObject controlBox;

	public AuiButton buttonControlOnOff;

	public AuiSpriteNumber textCmdPtsCur;

	public AuiSpriteNumber textCmdPtsMax;

	public AuiSprite barCmdPts;

	public AuiButton buttonCmdPts;

	public AuiButton buttonTraining;

	public AuiButton buttonLoards;

	public AuiButton buttonGemShop;

	public AuiButton buttonMessage;

	public TextMesh textMsgCount;

	public AuiSpriteAnimation aniAttacked;

	public AuiSpriteAnimation aniBattleWin;

	public AuiSpriteAnimation aniBattleLose;

	public MapAttack popupAttack;

	public MapRedeploy popupRedeploy;

	public MapSpy popupSpy;

	public MapCastleDetail popupCastleDetail;

	public MapRecruitSoldier popupRecruitSoldier;

	public MapEconomy popupEconomy;

	public MapUpgrade popupUpgrade;

	public MapCancel popupCancel;

	public MapMercy popupMercy;

	public MapPolitics popupPolitics;

	public ProcCastle procCastle;

	private CastleInterface[] castles;

	private bool mouseDrag;

	private Vector3 mouseDragStartPos;

	private Vector3 mapBefPos;

	private Camera uiCamera;

	public Rect[] noClickArea = new Rect[4];

	private float mapScale = 1f;

	private float mapScaleMax = 1f;

	private float mapScaleMin = 0.5f;

	private bool isMapScaling;

	private float touchLength;

	private bool touchActive;

	private float mapSizeX = 4096f;

	private float mapSizeY = 2048f;

	private float mapMarginX = 1568f;

	private float mapMarginY = 704f;

	public void Start()
	{
		uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();
		mapScale = 1f;
		isMapScaling = false;
		ResetMapMargin();
		if (!PlayInfo.playerData.tutorialMode)
		{
			MapScale();
		}
		int num = PlayInfo.castleManager.castle.Length;
		Vector3 localPosition = fabCastle.transform.localPosition;
		CastleInfo[] castle = PlayInfo.castleManager.castle;
		castles = new CastleInterface[num];
		for (int i = 0; i < num; i++)
		{
			if (i == 0)
			{
				castles[i] = fabCastle;
			}
			else
			{
				castles[i] = Object.Instantiate(fabCastle) as CastleInterface;
				castles[i].transform.parent = fabCastle.transform.parent;
			}
			localPosition.x = castle[i].posx;
			localPosition.y = castle[i].posy;
			castles[i].transform.localPosition = localPosition;
			castles[i].castleIndex = i;
		}
		RefershCastleInfo();
		buttonControlOnOff.onButtonClick = OnControlBoxOnOff;
		buttonControlOnOff.buttonTag = 1;
		popupAttack.Hide();
		popupRedeploy.Hide();
		popupSpy.Hide();
		popupCastleDetail.Hide();
		popupRecruitSoldier.Hide();
		popupEconomy.Hide();
		popupUpgrade.Hide();
		popupCancel.Hide();
		popupMercy.Hide();
		popupPolitics.Hide();
		popupEconomy.uiMap = this;
		popupCastleDetail.castles = castles;
		popupCastleDetail.procCastle = procCastle;
		buttonCmdPts.onButtonClick = OnCommandPointClick;
		buttonTraining.onButtonClick = OnTrainingClick;
		buttonLoards.onButtonClick = OnLoardsClick;
		buttonGemShop.onButtonClick = OnGemShopClick;
		buttonMessage.onButtonClick = OnMessageClick;
		noClickArea = new Rect[4];
		Rect rect = new Rect(-480f, 256f, 960f, 64f);
		noClickArea[0] = rect;
		noClickArea[3] = rect;
		rect = ProcBase.GetGameObjectRect(controlBox.gameObject);
		noClickArea[1] = rect;
		rect = ProcBase.GetGameObjectRect(popupCancel.gameObject);
		noClickArea[2] = rect;
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	public void MapScale()
	{
		if (UserSetting.tabletMode)
		{
			isMapScaling = true;
			StartCoroutine("WaitForMapScaling");
		}
	}

	private void ResetMapMargin()
	{
		mapMarginX = (mapSizeX * mapScale - (float)ScreenSize.Width) * 0.5f;
		mapMarginY = (mapSizeY * mapScale - (float)ScreenSize.Height) * 0.5f;
	}

	public void RefershCastleInfo()
	{
		int num = PlayInfo.castleManager.castle.Length;
		for (int i = 0; i < num; i++)
		{
			if (castles != null)
			{
				CastleInterface castleInterface = castles[i];
				castleInterface.Refresh();
			}
		}
	}

	public bool CheckInNoClickArea(float x, float y)
	{
		Rect[] array = noClickArea;
		for (int i = 0; i < array.Length; i++)
		{
			Rect rect = array[i];
			if (x >= rect.xMin && x <= rect.xMax && y >= rect.yMin && y <= rect.yMax)
			{
				return true;
			}
		}
		return false;
	}

	private IEnumerator WaitForMapScaling()
	{
		bool zoomIn = mapScale < mapScaleMax;
		float befScale = mapScale;
		Vector3 pos = mapTrans.localPosition;
		while (isMapScaling)
		{
			yield return 1;
			Vector3 newPos = mapTrans.localPosition;
			if (zoomIn)
			{
				mapScale += Time.deltaTime * 2f;
				if (mapScale > mapScaleMax)
				{
					mapScale = mapScaleMax;
					isMapScaling = false;
				}
				mapTrans.localScale = new Vector3(mapScale, mapScale, 1f);
				if (mapScale > befScale)
				{
					newPos.x = pos.x * (mapScale * 2f);
					newPos.y = pos.y * (mapScale * 2f);
					mapTrans.localPosition = newPos;
				}
			}
			else
			{
				mapScale -= Time.deltaTime * 2f;
				if (mapScale < mapScaleMin)
				{
					mapScale = mapScaleMin;
					isMapScaling = false;
				}
				mapTrans.localScale = new Vector3(mapScale, mapScale, 1f);
				newPos.x = pos.x * mapScale;
				newPos.y = pos.y * mapScale;
				mapTrans.localPosition = newPos;
			}
			ResetMapMargin();
			if (newPos.x < 0f - mapMarginX)
			{
				newPos.x = 0f - mapMarginX;
			}
			if (newPos.x > mapMarginX)
			{
				newPos.x = mapMarginX;
			}
			if (newPos.y < 0f - mapMarginY)
			{
				newPos.y = 0f - mapMarginY;
			}
			if (newPos.y > mapMarginY)
			{
				newPos.y = mapMarginY;
			}
			mapTrans.localPosition = newPos;
		}
		yield return new WaitForSeconds(0.1f);
	}

	private void Update()
	{
		if (AuiButton.modalActive || AuiButton.mostTopActive || AuiButton.topActive || PlayInfo.playerData.tutorialMode || isMapScaling)
		{
			return;
		}
		if (Application.isEditor && Input.GetKeyDown(KeyCode.A))
		{
			isMapScaling = true;
			StartCoroutine("WaitForMapScaling");
			return;
		}
		if (Input.touchCount > 1)
		{
			float num = Vector3.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
			if (touchActive)
			{
				if ((mapScale < mapScaleMax && num > touchLength * 1.2f) || (mapScale > mapScaleMin && num < touchLength * 0.8f))
				{
					isMapScaling = true;
					StartCoroutine("WaitForMapScaling");
					touchActive = false;
				}
			}
			else
			{
				touchActive = true;
				touchLength = num;
			}
			return;
		}
		touchActive = false;
		if (Input.GetMouseButtonDown(0))
		{
			if (popupCastleDetail.gameObject.active)
			{
				return;
			}
			mouseDragStartPos = uiCamera.ScreenToWorldPoint(Input.mousePosition);
			if (!AuiButton.modalActive && !CheckInNoClickArea(mouseDragStartPos.x, mouseDragStartPos.y))
			{
				mouseDrag = true;
				mapBefPos = mapTrans.position;
			}
		}
		else if (Input.GetMouseButtonUp(0))
		{
			if (popupCastleDetail.gameObject.active)
			{
				return;
			}
			mouseDrag = false;
			Vector3 a = uiCamera.ScreenToWorldPoint(Input.mousePosition);
			a.z = 0f;
			mouseDragStartPos.z = 0f;
			if (Vector3.Distance(a, mouseDragStartPos) < 20f && !CheckInNoClickArea(a.x, a.y))
			{
				CastleInterface[] array = castles;
				foreach (CastleInterface castleInterface in array)
				{
					Vector3 position = castleInterface.transform.position;
					position.z = 0f;
					if (Vector3.Distance(a, position) < 60f)
					{
						castleInterface.CastleSelected();
						break;
					}
				}
			}
		}
		if (mouseDrag)
		{
			Vector3 vector = uiCamera.ScreenToWorldPoint(Input.mousePosition);
			Vector3 position2 = vector - mouseDragStartPos + mapBefPos;
			if (position2.x < 0f - mapMarginX)
			{
				position2.x = 0f - mapMarginX;
			}
			if (position2.x > mapMarginX)
			{
				position2.x = mapMarginX;
			}
			if (position2.y < 0f - mapMarginY)
			{
				position2.y = 0f - mapMarginY;
			}
			if (position2.y > mapMarginY)
			{
				position2.y = mapMarginY;
			}
			mapTrans.position = position2;
		}
	}

	private void OnControlBoxOnOff(AuiButton sender)
	{
		if (buttonControlOnOff.buttonTag == 0)
		{
			buttonControlOnOff.buttonTag = 1;
			TransformAnimation transformAnimation = controlBox.gameObject.GetComponent<TransformAnimation>();
			if (transformAnimation == null)
			{
				transformAnimation = controlBox.gameObject.AddComponent<TransformAnimation>();
			}
			Vector3 vector = (transformAnimation.moveFrom = controlBox.transform.localPosition);
			transformAnimation.moveTo = new Vector3(vector.x, -236f, vector.z);
			transformAnimation.speed = 800f;
			transformAnimation.StartMove();
		}
		else
		{
			buttonControlOnOff.buttonTag = 0;
			TransformAnimation transformAnimation2 = controlBox.gameObject.GetComponent<TransformAnimation>();
			if (transformAnimation2 == null)
			{
				transformAnimation2 = controlBox.gameObject.AddComponent<TransformAnimation>();
			}
			Vector3 vector2 = (transformAnimation2.moveFrom = controlBox.transform.localPosition);
			transformAnimation2.moveTo = new Vector3(vector2.x, -440f, vector2.z);
			transformAnimation2.speed = 800f;
			transformAnimation2.StartMove();
		}
		StartCoroutine("CoroutineWaitControlBoxMove");
	}

	private IEnumerator CoroutineWaitControlBoxMove()
	{
		TransformAnimation transAni;
		do
		{
			yield return new WaitForSeconds(0.5f);
			transAni = controlBox.gameObject.GetComponent<TransformAnimation>();
			if (transAni == null)
			{
				yield break;
			}
		}
		while (transAni.isMoving);
		Rect rc = ProcBase.GetGameObjectRect(controlBox.gameObject);
		noClickArea[1] = rc;
	}

	public void OnCommandPointClick(AuiButton sender)
	{
		procCastle.uiCmdPtsShop.Show();
	}

	private void OnTrainingClick(AuiButton sender)
	{
		procCastle.uiTraining.Show();
	}

	private void OnLoardsClick(AuiButton sender)
	{
		procCastle.uiLord.Show();
	}

	private void OnGemShopClick(AuiButton sender)
	{
		procCastle.uiGemShop.Show();
	}

	private void OnMessageClick(AuiButton sender)
	{
		procCastle.uiMessageNote.Show();
	}

	private void UpdateStatus()
	{
		textCmdPtsCur.SetValue(PlayInfo.playerData.cmdPts);
		textCmdPtsMax.SetValue(PlayInfo.playerData.maxCmdPts);
		barCmdPts.isCrop = true;
		barCmdPts.crop.width = (float)PlayInfo.playerData.cmdPts / (float)PlayInfo.playerData.maxCmdPts;
		barCmdPts.SetFrame(0);
		int num = PlayInfo.messageManager.GetNewCount();
		if (num > 99)
		{
			num = 99;
		}
		textMsgCount.text = num.ToString();
		RefershCastleInfo();
	}

	private IEnumerator CoroutineUpdateStatus()
	{
		while (true)
		{
			UpdateStatus();
			yield return new WaitForSeconds(0.5f);
		}
	}

	public void Show()
	{
		base.gameObject.SetActiveRecursively(true);
		RefershCastleInfo();
		popupAttack.Hide();
		popupRedeploy.Hide();
		popupSpy.Hide();
		popupCastleDetail.Hide();
		popupRecruitSoldier.Hide();
		popupEconomy.Hide();
		popupUpgrade.Hide();
		popupCancel.Hide();
		popupMercy.Hide();
		popupPolitics.Hide();
		aniAttacked.visible = false;
		aniBattleWin.visible = false;
		aniBattleLose.visible = false;
		StartCoroutine("CoroutineUpdateStatus");
	}

	public void Hide()
	{
		base.gameObject.SetActiveRecursively(false);
	}

	public void DisableCastleTargetMode()
	{
		if (castles != null)
		{
			for (int i = 0; i < castles.Length; i++)
			{
				castles[i].HideAll();
			}
		}
		popupCancel.Hide();
	}

	public void MoveToLastHumanCastle()
	{
		CastleInfo castleInfo = null;
		for (int num = PlayInfo.castleManager.castle.Length - 1; num >= 0; num--)
		{
			if (PlayInfo.castleManager.castle[num].side == 0)
			{
				castleInfo = PlayInfo.castleManager.castle[num];
				break;
			}
		}
		if (castleInfo != null)
		{
			MoveMapFromPos(0f - castleInfo.posx, 0f - castleInfo.posy);
		}
	}

	public void MoveToLastBattleCastle()
	{
		CastleInfo castleInfo = null;
		if (PlayInfo.battleInfo != null && PlayInfo.battleInfo.attackCastle != null && PlayInfo.battleInfo.defenseCastle != null && PlayInfo.battleInfo.battleFinish)
		{
			PlayInfo.battleInfo.battleFinish = false;
			castleInfo = ((!PlayInfo.battleInfo.battleWin) ? ((!PlayInfo.battleInfo.isAttack) ? PlayInfo.battleInfo.defenseCastle : PlayInfo.battleInfo.attackCastle) : PlayInfo.battleInfo.defenseCastle);
			MoveMapFromPos(0f - castleInfo.posx, 0f - castleInfo.posy);
		}
	}

	public void MoveToCastle(int castleIndex)
	{
		CastleInfo castleInfo = PlayInfo.castleManager.castle[castleIndex];
		MoveMapFromPos(0f - castleInfo.posx, 0f - castleInfo.posy);
	}

	private void MoveMapFromPos(float x, float y)
	{
		Vector3 position = mapTrans.position;
		position.x = x * mapScale;
		position.y = y * mapScale;
		if (position.x < 0f - mapMarginX)
		{
			position.x = 0f - mapMarginX;
		}
		if (position.x > mapMarginX)
		{
			position.x = mapMarginX;
		}
		if (position.y < 0f - mapMarginY)
		{
			position.y = 0f - mapMarginY;
		}
		if (position.y > mapMarginY)
		{
			position.y = mapMarginY;
		}
		mapTrans.position = position;
	}
}
