using UnityEngine;

public class CameraControl : MonoBehaviour
{
	private const float cameraHeight = 6f;

	private const float cameraZOffset = 5f;

	private const float cameraViewWidth = 8f;

	private const float cameraViewHeight = 3f;

	private const float cameraMargin = 4f;

	private const float cameraRotateX = 40f;

	private const float cameraRotateY = 180f;

	private Transform targetTrans;

	private Vector3 minPos;

	private Vector3 maxPos;

	public Camera camUnit;

	private Transform camHandler;

	private float viewRotate;

	private float viewPitch;

	private float zoomLength = 1f;

	private float viewShiftX;

	private float viewShiftY;

	private bool isCameraLimit;

	private float cameraLimitMinX;

	private void Start()
	{
		camUnit = GameObject.Find("Unit Camera").GetComponent<Camera>();
		Quaternion rotation = Quaternion.Euler(new Vector3(40f, 180f, 0f));
		camUnit.transform.rotation = rotation;
	}

	public void SetTarget(UnitControl unit)
	{
		SetTarget(unit, null);
	}

	public void SetTarget(UnitControl unit, Collider back)
	{
		camUnit = GameObject.Find("Unit Camera").GetComponent<Camera>();
		targetTrans = unit.transform;
		camHandler = camUnit.transform.parent;
		if (back != null)
		{
			minPos = back.bounds.min;
			maxPos = back.bounds.max;
			minPos.x += 8f;
			minPos.z += 5.5f;
			maxPos.x -= 8f;
			maxPos.z -= 5.5f;
		}
		Vector3 position = targetTrans.position;
		camHandler.position = position;
		Vector3 vector = new Vector3(viewShiftX, 6f + viewShiftY, 5f);
		camUnit.transform.localPosition = vector * zoomLength;
	}

	public void EnableCameraLimitMinX(float x)
	{
		isCameraLimit = true;
		cameraLimitMinX = x;
	}

	public void DisableCameraLimitMinX()
	{
		isCameraLimit = false;
	}

	public void SetRotate(float rot)
	{
		viewRotate += rot;
		Quaternion rotation = camHandler.rotation;
		Quaternion rotation2 = Quaternion.Euler(new Vector3(viewPitch, viewRotate, 0f));
		camHandler.rotation = rotation2;
		Vector3 position = camUnit.transform.position;
		if (isCameraLimit && position.x < cameraLimitMinX)
		{
			camHandler.rotation = rotation;
		}
	}

	public void SetPitch(float pit)
	{
		viewPitch += pit;
		if (viewPitch < -25f)
		{
			viewPitch = -25f;
		}
		if (viewPitch > 30f)
		{
			viewPitch = 30f;
		}
		Quaternion rotation = camHandler.rotation;
		Quaternion rotation2 = Quaternion.Euler(new Vector3(viewPitch, viewRotate, 0f));
		camHandler.rotation = rotation2;
		Vector3 position = camUnit.transform.position;
		if (isCameraLimit && position.x < cameraLimitMinX)
		{
			camHandler.rotation = rotation;
		}
	}

	public void SetZoomLength(float length)
	{
		zoomLength = length;
	}

	public void SetShiftX(float shift)
	{
		viewShiftX = shift;
	}

	public void SetShiftY(float shift)
	{
		viewShiftY = shift;
	}

	private void Update()
	{
		Vector3 position = targetTrans.position;
		camHandler.position = position;
		if (!isCameraLimit)
		{
			return;
		}
		Vector3 position2 = camUnit.transform.position;
		if (position2.x < cameraLimitMinX)
		{
			float rotate = ((position2.z > position.z) ? 1 : (-1));
			while (camUnit.transform.position.x < cameraLimitMinX)
			{
				SetRotate(rotate);
			}
		}
	}
}
