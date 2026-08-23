using UnityEngine;

public class SwardBand : MonoBehaviour
{
	public class PreTrail
	{
		public Vector3[] posStart;

		public Vector3[] posEnd;
	}

	private const float baseFps = 0.015f;

	private const int maxSwardTrail = 10;

	private bool alreadySet;

	public GameObject swardTrail;

	public Transform swardDummyStart;

	public Transform swardDummyEnd;

	private Mesh swardTrailMesh;

	private Vector3[] swardTrailVertex = new Vector3[20];

	private Vector2[] swardTrailUV = new Vector2[20];

	private int[] swardTrailTri = new int[54];

	private int swardLastTrail;

	private Vector3 swardTrailFirst;

	private Transform swardTrans;

	private PreTrail[] preTrail;

	private UnitCharactor unitChar;

	private float trailTime;

	private bool isVisible;

	public bool Visible
	{
		get
		{
			return isVisible;
		}
		set
		{
			isVisible = value;
			if (isVisible)
			{
				StartTrail();
			}
			else
			{
				HideTrail();
			}
		}
	}

	private void SetTrailObject()
	{
		swardDummyStart = base.transform.Find("bendstart");
		swardDummyEnd = base.transform.Find("bendend");
		swardTrail = new GameObject("sward_trail");
		swardTrail.AddComponent<MeshRenderer>();
		swardTrail.GetComponent<Renderer>().material = ResourceManager.Load("Character/Materials/weapons", "mtr_trail", typeof(Material)) as Material;
		swardTrans = swardTrail.transform;
		swardTrans.parent = base.transform.root;
		swardTrans.localPosition = new Vector3(0f, 0f, 0f);
		swardTrans.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		swardTrailMesh = swardTrail.AddComponent<MeshFilter>().mesh;
		for (int i = 0; i < 10; i++)
		{
			swardTrailVertex[i * 2] = new Vector3(0f, 0f, 0f);
			swardTrailVertex[i * 2 + 1] = new Vector3(0f, 0f, 1f);
			swardTrailUV[i * 2] = new Vector2((float)i / 9f, 0f);
			swardTrailUV[i * 2 + 1] = new Vector2((float)i / 9f, 1f);
		}
		for (int j = 0; j < 9; j++)
		{
			swardTrailTri[j * 6] = j * 2;
			swardTrailTri[j * 6 + 1] = j * 2 + 1;
			swardTrailTri[j * 6 + 2] = j * 2 + 2;
			swardTrailTri[j * 6 + 3] = j * 2 + 1;
			swardTrailTri[j * 6 + 4] = j * 2 + 3;
			swardTrailTri[j * 6 + 5] = j * 2 + 2;
		}
		swardTrailMesh.vertices = swardTrailVertex;
		swardTrailMesh.uv = swardTrailUV;
		swardTrailMesh.triangles = swardTrailTri;
		alreadySet = true;
	}

	private void Start()
	{
		if (!alreadySet)
		{
			SetTrailObject();
		}
		swardTrail.SetActive(false);
		Visible = false;
	}

	private void OnDestroy()
	{
		Object.Destroy(swardTrail);
	}

	private void ResetSwardTrail()
	{
		Vector3 position = swardTrans.position;
		if (unitChar != null && preTrail != null)
		{
			int preTrailIndex = GetPreTrailIndex();
			for (int i = 0; i < 10; i++)
			{
				swardTrailVertex[i * 2] = preTrail[preTrailIndex].posStart[0];
				swardTrailVertex[i * 2 + 1] = preTrail[preTrailIndex].posEnd[0];
			}
		}
		else
		{
			for (int j = 0; j < 10; j++)
			{
				swardTrailVertex[j * 2] = swardDummyStart.position - position;
				swardTrailVertex[j * 2 + 1] = swardDummyEnd.position - position;
			}
		}
		swardTrailMesh.vertices = swardTrailVertex;
		swardLastTrail = 9;
		trailTime = 0f;
	}

	private void MoveSwardTrail()
	{
		if (swardLastTrail >= 10)
		{
			for (int num = 9; num > 0; num--)
			{
				swardTrailVertex[num * 2] = swardTrailVertex[(num - 1) * 2];
				swardTrailVertex[num * 2 + 1] = swardTrailVertex[(num - 1) * 2 + 1];
			}
			swardLastTrail = 9;
		}
		int num2 = 10 - (swardLastTrail + 1);
		Vector3 position = swardTrans.position;
		if (unitChar != null && preTrail != null)
		{
			int preTrailIndex = GetPreTrailIndex();
			int num3 = (int)(trailTime / 0.015f);
			if (num3 >= preTrail[preTrailIndex].posStart.Length)
			{
				HideTrail();
				return;
			}
			swardTrailVertex[num2 * 2] = preTrail[preTrailIndex].posStart[num3];
			swardTrailVertex[num2 * 2 + 1] = preTrail[preTrailIndex].posEnd[num3];
		}
		else
		{
			swardTrailVertex[num2 * 2] = swardDummyStart.position - position;
			swardTrailVertex[num2 * 2 + 1] = swardDummyEnd.position - position;
		}
		swardTrailMesh.vertices = swardTrailVertex;
		swardLastTrail++;
	}

	private void StartTrail()
	{
		ResetSwardTrail();
		swardTrail.SetActive(true);
	}

	private void HideTrail()
	{
		ResetSwardTrail();
		swardTrail.SetActive(false);
	}

	private void Update()
	{
		if (!isVisible)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		if (deltaTime > 0.03f)
		{
			float num = trailTime;
			for (float num2 = 0f; num2 < deltaTime; num2 += 0.015f)
			{
				trailTime += 0.015f;
				MoveSwardTrail();
			}
			trailTime = num + deltaTime;
		}
		else
		{
			trailTime += deltaTime;
		}
		MoveSwardTrail();
	}

	private int GetPreTrailIndex()
	{
		if (unitChar == null)
		{
			return 0;
		}
		UnitCharactor unitCharactor = unitChar;
		int num = unitCharactor.maxAnimation[6];
		int num2 = unitCharactor.maxAnimation[7];
		int result = 0;
		switch (unitCharactor.animationType)
		{
		case UnitCharactor.AnimationType.attacknormal:
			result = unitCharactor.animationIndex;
			break;
		case UnitCharactor.AnimationType.attackskill:
			result = num + unitCharactor.animationIndex;
			break;
		case UnitCharactor.AnimationType.attackspecial:
			result = num + num2 + unitCharactor.animationIndex;
			break;
		}
		return result;
	}

	public void GeneratePreTrail(UnitCharactor unit)
	{
		unitChar = unit;
		if (!alreadySet)
		{
			SetTrailObject();
		}
		switch (unit.weaponType)
		{
		case UnitCharactor.WeaponType.onehand:
			swardTrail.GetComponent<Renderer>().material.mainTexture = ResourceManager.Load("Character/textures/effect", "tex_onehand", typeof(Texture)) as Texture;
			break;
		case UnitCharactor.WeaponType.doublehand:
			swardTrail.GetComponent<Renderer>().material.mainTexture = ResourceManager.Load("Character/textures/effect", "tex_doublehand", typeof(Texture)) as Texture;
			break;
		case UnitCharactor.WeaponType.bigsword:
			swardTrail.GetComponent<Renderer>().material.mainTexture = ResourceManager.Load("Character/textures/effect", "tex_bighand", typeof(Texture)) as Texture;
			break;
		}
		int num = unit.maxAnimation[6];
		int num2 = unit.maxAnimation[7];
		int num3 = unit.maxAnimation[8];
		int num4 = num + num2 + num3;
		preTrail = new PreTrail[num4];
		int num5 = 0;
		swardTrans = swardTrail.transform;
		swardTrans.parent = unit.transform;
		swardTrans.localPosition = new Vector3(0f, 0f, 0f);
		swardTrans.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		Vector3 position = swardTrans.position;
		for (int i = 0; i < 3; i++)
		{
			int num6 = 0;
			switch (i)
			{
			case 0:
				num6 = num;
				break;
			case 1:
				num6 = num2;
				break;
			default:
				num6 = num3;
				break;
			}
			for (int j = 0; j < num6; j++)
			{
				string empty = string.Empty;
				switch (i)
				{
				case 0:
					empty = unit.GetAnimationName(UnitCharactor.AnimationType.attacknormal, j);
					break;
				case 1:
					empty = unit.GetAnimationName(UnitCharactor.AnimationType.attackskill, j);
					break;
				default:
					empty = unit.GetAnimationName(UnitCharactor.AnimationType.attackspecial, j);
					break;
				}
				AnimationState animationState = unit.GetComponent<Animation>()[empty];
				if (animationState != null)
				{
					unit.GetComponent<Animation>().Play(empty);
					float length = animationState.length;
					float num7 = 0.00375f;
					int num8 = (int)(length / num7);
					preTrail[num5] = new PreTrail();
					preTrail[num5].posStart = new Vector3[num8];
					preTrail[num5].posEnd = new Vector3[num8];
					for (int k = 0; k < num8; k++)
					{
						animationState.time = (float)k * num7;
						unit.GetComponent<Animation>().Sample();
						preTrail[num5].posStart[k] = base.transform.root.worldToLocalMatrix.MultiplyVector(swardDummyStart.position - position);
						preTrail[num5].posEnd[k] = base.transform.root.worldToLocalMatrix.MultiplyVector(swardDummyEnd.position - position);
					}
				}
				num5++;
			}
		}
	}
}
