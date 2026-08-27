using UnityEngine;

public class UIPlayTutorial : MonoBehaviour
{
	public GameObject[] objectStep;

	public GameObject[] buttonStep;

	public TextMesh[] textDesc;

	public TextMesh textGotoMap;

	public TextMesh textGotoTown;

	public Camera uiCamera;

	public InterfaceControl interCtrl;

	public ProcCastle procCastle;

	private int maxStep;

	private int curStep;

	private AuiButtonController buttonController;

	private float buttonSize = 40f;

	private void Start()
	{
		maxStep = objectStep.Length;
		if (procCastle != null)
		{
			buttonController = procCastle.GetComponent<AuiButtonController>();
		}
		if (buttonController != null)
		{
			buttonController.isActive = false;
		}
		if (interCtrl != null)
		{
			interCtrl.UserInputEnable(false);
		}
		for (int i = 0; i < maxStep; i++)
		{
			textDesc[i].text = StringContent.tutorialDesc[i];
		}
		textGotoMap.text = StringContent.tutorialGotoMap;
		textGotoTown.text = StringContent.tutorialGotoTown;
		Show();
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	public void Show()
	{
		base.gameObject.SetActiveRecursive(true);
		for (int i = 0; i < maxStep; i++)
		{
			objectStep[i].SetActiveRecursive(false);
		}
		curStep = 0;
		ShowStep(0);
	}

	private void Hide()
	{
		PlayInfo.playerData.tutorialMode = false;
		procCastle.tutorialActive = false;
		procCastle.uiMap.MapScale();
		if (buttonController != null)
		{
			buttonController.isActive = true;
		}
		base.gameObject.SetActiveRecursive(false);
	}

	private void ShowStep(int step)
	{
		objectStep[curStep].SetActiveRecursive(false);
		curStep = step;
		objectStep[curStep].SetActiveRecursive(true);
	}

	private void Update()
	{
		if (interCtrl != null)
		{
			interCtrl.UserInputEnable(false);
		}
		if (!Input.GetMouseButtonDown(0))
		{
			return;
		}
		Vector3 mousePosition = Input.mousePosition;
		Vector3 a = uiCamera.ScreenToWorldPoint(mousePosition);
		a.z = 0f;
		Vector3 position = buttonStep[curStep].transform.position;
		position.z = 0f;
		if (!(Vector3.Distance(a, position) < buttonSize))
		{
			return;
		}
		int num = curStep + 1;
		if (num < maxStep)
		{
			switch (num)
			{
			case 1:
				procCastle.ShowMap();
				break;
			case 3:
				procCastle.uiMap.popupCastleDetail.Show(0);
				break;
			case 4:
				procCastle.uiMap.popupCastleDetail.Hide();
				break;
			case 5:
				procCastle.uiMap.popupCastleDetail.Show(5);
				break;
			case 6:
				procCastle.uiMap.popupCastleDetail.Hide();
				break;
			}
			ShowStep(num);
		}
		else
		{
			Hide();
		}
	}
}
