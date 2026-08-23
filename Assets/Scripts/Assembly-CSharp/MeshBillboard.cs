using UnityEngine;

public class MeshBillboard : MonoBehaviour
{
	public bool freezeXZ;

	private Camera mainCam;

	private Transform trans;

	private Transform camTrans;

	private void Start()
	{
		trans = base.transform;
	}

	private void Update()
	{
		if (mainCam == null)
		{
			mainCam = Camera.main;
			if (mainCam != null)
			{
				camTrans = mainCam.transform;
			}
		}
		if (mainCam != null)
		{
			if (freezeXZ)
			{
				Vector3 eulerAngles = Quaternion.LookRotation(camTrans.position - trans.position).eulerAngles;
				eulerAngles.x = 0f;
				eulerAngles.z = 0f;
				trans.rotation = Quaternion.Euler(eulerAngles);
			}
			else
			{
				trans.rotation = Quaternion.LookRotation(camTrans.position - trans.position);
			}
		}
	}
}
