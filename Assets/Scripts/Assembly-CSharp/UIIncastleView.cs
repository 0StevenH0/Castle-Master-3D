using UnityEngine;

public class UIIncastleView : MonoBehaviour
{
	public class NameLabel
	{
		public bool isActive;

		public Vector3 pos;

		public AuiSprite backName;

		public TextMesh textName;

		public Transform trans;
	}

	private const int maxNameLabel = 20;

	private const float offsetY = 20f;

	public AuiSprite backName;

	public TextMesh textName;

	public Camera uiCamera;

	public Camera gameCamera;

	public static UIIncastleView self;

	private NameLabel[] nameLabels;

	private void Update()
	{
		if (uiCamera == null)
		{
			return;
		}
		int num = 20;
		for (int i = 0; i < num; i++)
		{
			if (nameLabels[i].isActive)
			{
				NameLabel nameLabel = nameLabels[i];
				nameLabel.pos = nameLabel.trans.position;
				nameLabel.pos.y += nameLabel.trans.GetComponent<Collider>().bounds.max.y;
				Vector3 position = gameCamera.WorldToScreenPoint(nameLabel.pos);
				if (position.z > 0f)
				{
					Vector3 localPosition = uiCamera.ScreenToWorldPoint(position);
					localPosition.y += 20f;
					localPosition.z = position.z / 5f;
					nameLabel.textName.transform.localPosition = localPosition;
					localPosition.z += 0.5f;
					nameLabel.backName.transform.localPosition = localPosition;
					nameLabel.textName.gameObject.SetActive(true);
					nameLabel.backName.gameObject.SetActiveRecursive(true);
				}
				else
				{
					nameLabel.textName.gameObject.SetActive(false);
					nameLabel.backName.gameObject.SetActiveRecursive(false);
				}
			}
		}
	}

	private void SetActiveNameLabelFromIndex(int index, Transform trans, string strName)
	{
		if (!nameLabels[index].isActive)
		{
			NameLabel nameLabel = nameLabels[index];
			nameLabel.isActive = true;
			nameLabel.textName.text = strName;
			nameLabel.trans = trans;
			ResetLabelPosition(index);
		}
	}

	private void ResetLabelPosition(int index)
	{
		if (!nameLabels[index].isActive)
		{
			NameLabel nameLabel = nameLabels[index];
			Vector3 position = (nameLabel.pos = nameLabel.trans.position);
			nameLabel.pos.y += nameLabel.trans.GetComponent<Collider>().bounds.max.y;
			Vector3 position2 = gameCamera.WorldToScreenPoint(position);
			if (position2.z > 0f)
			{
				Vector3 position3 = uiCamera.ScreenToWorldPoint(position2);
				position3.y += 20f;
				position3.z = nameLabel.textName.transform.position.z;
				nameLabel.textName.transform.position = position3;
				position3.z = nameLabel.backName.transform.position.z;
				nameLabel.backName.transform.position = position3;
				nameLabel.textName.gameObject.SetActive(true);
				nameLabel.backName.gameObject.SetActiveRecursive(true);
			}
		}
	}

	public void Init()
	{
		self = this;
		int num = 20;
		nameLabels = new NameLabel[num];
		for (int i = 0; i < num; i++)
		{
			NameLabel nameLabel = new NameLabel();
			nameLabel.isActive = false;
			nameLabel.textName = Object.Instantiate(textName) as TextMesh;
			nameLabel.textName.transform.parent = textName.transform.parent;
			nameLabel.textName.transform.position = textName.transform.position;
			nameLabel.textName.gameObject.SetActive(false);
			nameLabel.backName = Object.Instantiate(backName) as AuiSprite;
			nameLabel.backName.transform.parent = backName.transform.parent;
			nameLabel.backName.transform.position = backName.transform.position;
			nameLabel.backName.gameObject.SetActiveRecursive(false);
			nameLabels[i] = nameLabel;
		}
		backName.gameObject.SetActiveRecursive(false);
		textName.gameObject.SetActive(false);
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	public void SetActiveNameLabel(Transform trans, string strName)
	{
		if (uiCamera == null || gameCamera == null)
		{
			return;
		}
		for (int i = 0; i < 20; i++)
		{
			if (!nameLabels[i].isActive)
			{
				SetActiveNameLabelFromIndex(i, trans, strName);
				break;
			}
		}
	}
}
