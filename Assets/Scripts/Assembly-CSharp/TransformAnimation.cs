using UnityEngine;

public class TransformAnimation : MonoBehaviour
{
	public Vector3 moveFrom = Vector3.zero;

	public Vector3 moveTo = Vector3.zero;

	public float speed = 1f;

	public bool autoStart;

	public bool autoRemoveComponent;

	public float delay;

	public bool isMoving;

	private Vector3 dir;

	private float length;

	private Transform trans;

	private float waitTime;

	private Vector3 movePos;

	private void Start()
	{
		if (autoStart)
		{
			StartMove();
		}
	}

	public void StartMove()
	{
		if (speed != 0f)
		{
			float num = Vector3.Distance(moveFrom, moveTo);
			if (!(num <= 0f))
			{
				dir = moveTo - moveFrom;
				dir = dir * speed / num;
				length = num;
				trans = base.transform;
				waitTime = 0f;
				movePos = moveFrom;
				isMoving = true;
				base.enabled = true;
			}
		}
	}

	private void Update()
	{
		if (!isMoving)
		{
			return;
		}
		waitTime += Time.deltaTime;
		if (waitTime < delay)
		{
			return;
		}
		movePos += dir * Time.deltaTime;
		Vector3 vector = movePos;
		float num = Vector3.Distance(moveFrom, vector);
		if (num >= length)
		{
			vector = moveTo;
			isMoving = false;
		}
		trans.localPosition = vector;
		if (!isMoving)
		{
			base.enabled = false;
			if (autoRemoveComponent)
			{
				Object.Destroy(this);
			}
		}
	}
}
