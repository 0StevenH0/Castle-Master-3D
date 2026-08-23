using System.Collections.Generic;
using UnityEngine;

public class InterfaceControl : MonoBehaviour
{
	public delegate void OnFollowFinish(UnitCharactor unitCtrl);

	public const KeyCode keyChangeWeapon1 = KeyCode.Alpha1;

	public const KeyCode keyChangeWeapon2 = KeyCode.Alpha2;

	public const KeyCode keyChangeWeapon3 = KeyCode.Alpha3;

	public const KeyCode keyChangeWeaponToggle = KeyCode.Tab;

	public const KeyCode keyAttack = KeyCode.Space;

	public const KeyCode keySkill1 = KeyCode.Z;

	public const KeyCode keySkill2 = KeyCode.X;

	public const KeyCode keySkill3 = KeyCode.C;

	public const KeyCode keySkill4 = KeyCode.V;

	public const KeyCode keySkill5 = KeyCode.B;

	public const KeyCode keySkill6 = KeyCode.N;

	public const KeyCode keySkill7 = KeyCode.M;

	public UnitControl ctrlUnit;

	public AuiButtonController buttonController;

	private Camera camGame;

	private Camera camUI;

	private List<Rect> uiArea = new List<Rect>();

	private bool enableStatus = true;

	private bool dualTouchActive;

	private float dualTouchLen;

	public OnFollowFinish onFollowFinish;

	private Transform targetTrans;

	private UnitCharactor targetUnit;

	private GameObject feNpcSelectEffect;

	private GameObject febMonsterSelectEffect;

	private Transform febTargetSelected;

	private Transform febPickTerrain;

	private bool isRightMouseDrag;

	private Vector3 posRightMouseDragStart = new Vector3(0f, 0f, 0f);

	public Camera uiCamera
	{
		get
		{
			return camUI;
		}
	}

	private void Start()
	{
		febMonsterSelectEffect = Object.Instantiate(ResourceManager.Load("Character/prefeb/effect", "feb_monster_select_effect", typeof(GameObject))) as GameObject;
		febMonsterSelectEffect.AddComponent<AdjustAnimationSpeed>();
		feNpcSelectEffect = Object.Instantiate(ResourceManager.Load("Character/prefeb/effect", "feb_npc_select_effect", typeof(GameObject))) as GameObject;
		feNpcSelectEffect.AddComponent<AdjustAnimationSpeed>();
		febTargetSelected = febMonsterSelectEffect.transform;
		feNpcSelectEffect.SetActive(false);
		febMonsterSelectEffect.SetActive(false);
		GameObject gameObject = Object.Instantiate(ResourceManager.Load("Character/prefeb/effect", "feb_pick_terrain_effect", typeof(GameObject))) as GameObject;
		gameObject.AddComponent<AdjustAnimationSpeed>();
		febPickTerrain = gameObject.transform;
		gameObject.SetActive(false);
	}

	public void SetTargetSelected(UnitCharactor unit)
	{
		targetUnit = unit;
		targetTrans = unit.thisCtrl.thisTrans;
		if (targetUnit.charType == UnitCharactor.CharactorType.npc)
		{
			febTargetSelected = feNpcSelectEffect.transform;
			febMonsterSelectEffect.SetActive(false);
		}
		else
		{
			febTargetSelected = febMonsterSelectEffect.transform;
			feNpcSelectEffect.SetActive(false);
		}
		febTargetSelected.gameObject.SetActive(true);
	}

	public void SetUnit(UnitControl unit)
	{
		ctrlUnit = unit;
		camGame = Camera.main;
		GameObject gameObject = GameObject.Find("UICamera");
		if (gameObject != null)
		{
			camUI = gameObject.GetComponent<Camera>();
		}
	}

	public void AddUIArea(Rect rc)
	{
		uiArea.Add(rc);
	}

	public void ClearUIArea()
	{
		uiArea.Clear();
	}

	public void UserInputEnable(bool state)
	{
		enableStatus = state;
	}

	private void Update()
	{
		if (!enableStatus || AuiButton.modalActive || AuiButton.mostTopActive || AuiButton.topActive)
		{
			return;
		}
		if (buttonController != null && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0)))
		{
			Vector3 mousePosition = Input.mousePosition;
			if (buttonController.CheckForButtonArea(mousePosition.x, mousePosition.y))
			{
				return;
			}
		}
		if (Input.touchCount == 2)
		{
			float num = Vector3.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
			if (dualTouchActive)
			{
				float num2 = camGame.fieldOfView * dualTouchLen / num;
				if (num2 < 30f)
				{
					num2 = 30f;
				}
				if (num2 > 60f)
				{
					num2 = 60f;
				}
				camGame.fieldOfView = num2;
			}
			else
			{
				dualTouchActive = true;
				dualTouchLen = num;
			}
			return;
		}
		float axis = Input.GetAxis("Mouse ScrollWheel");
		if (axis != 0f)
		{
			float num6 = camGame.fieldOfView - axis * 40f;
			if (num6 < 30f)
			{
				num6 = 30f;
			}
			if (num6 > 60f)
			{
				num6 = 60f;
			}
			camGame.fieldOfView = num6;
		}
		if (Input.GetMouseButtonDown(1))
		{
			isRightMouseDrag = true;
			posRightMouseDragStart = Input.mousePosition;
		}
		if (Input.GetMouseButtonUp(1))
		{
			isRightMouseDrag = false;
		}
		if (isRightMouseDrag && Input.GetMouseButton(1))
		{
			Vector3 mousePositionRight = Input.mousePosition;
			float num4 = (mousePositionRight.x - posRightMouseDragStart.x) * 0.5f;
			float num5 = (mousePositionRight.y - posRightMouseDragStart.y) * 0.5f;
			if (num4 != 0f)
			{
				posRightMouseDragStart = mousePositionRight;
				base.gameObject.GetComponent<CameraControl>().SetRotate(num4);
			}
			if (num5 != 0f)
			{
				posRightMouseDragStart = mousePositionRight;
				base.gameObject.GetComponent<CameraControl>().SetPitch(num5);
			}
		}
		dualTouchActive = false;
		if (Input.GetMouseButtonUp(0))
		{
			Vector3 mousePosition3 = Input.mousePosition;
			if (camUI != null && uiArea.Count > 0)
			{
				Vector3 vector = camUI.ScreenToWorldPoint(mousePosition3);
				foreach (Rect item in uiArea)
				{
					if (vector.x > item.xMin && vector.x < item.xMax && vector.y > item.yMin && vector.y < item.yMax)
					{
						return;
					}
				}
			}
			Ray ray = camGame.ScreenPointToRay(mousePosition3);
			float distance = 1000f;
			bool flag3 = false;
			RaycastHit hitInfo;
			foreach (Collider colliderCharacter in ctrlUnit.colliderCharacters)
			{
				if (colliderCharacter.Raycast(ray, out hitInfo, distance))
				{
					if (colliderCharacter.gameObject == base.gameObject)
					{
						break;
					}
					UnitCharactor component = colliderCharacter.GetComponent<UnitCharactor>();
					if (!component.isAwake)
					{
						break;
					}
					if (component.charType == UnitCharactor.CharactorType.npc || component.charType == UnitCharactor.CharactorType.monster)
					{
						flag3 = true;
						ctrlUnit.SetTarget(component, onFollowFinish);
						targetUnit = component;
						targetTrans = component.transform;
						SetTargetSelected(component);
						break;
					}
				}
			}
			if (!flag3 && ctrlUnit.colliderBackGround.Raycast(ray, out hitInfo, distance))
			{
				ctrlUnit.ClearTarget();
				Vector3 point = hitInfo.point;
				point.y = ctrlUnit.transform.localPosition.y;
				ctrlUnit.RunTo(point);
				febPickTerrain.position = point;
				febPickTerrain.GetComponent<Animation>().Rewind();
				febPickTerrain.GetComponent<Animation>().Play();
				febPickTerrain.gameObject.SetActive(true);
			}
		}
		if (targetUnit != null)
		{
			if (targetUnit.thisCtrl.isDie || targetUnit.thisCtrl.unitIdentify != ctrlUnit.targetIdentify)
			{
				targetUnit = null;
				targetTrans = null;
				febTargetSelected.gameObject.SetActive(false);
			}
			else
			{
				Vector3 position = targetTrans.position;
				position.y += 0.02f;
				febTargetSelected.position = position;
			}
		}
	}
}
