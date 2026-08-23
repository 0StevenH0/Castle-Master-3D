using UnityEngine;

public class UITutorial : MonoBehaviour
{
	private const string path = "interface/images/tutorial";

	private const string filename = "img_tutorial_";

	private const int maxPage = 4;

	public GameObject infoImage;

	public AuiButton buttonClose;

	private int curPage;

	private Texture[] texTutorial = new Texture[4];

	private void Start()
	{
		buttonClose.onButtonClick = OnCloseClick;
		buttonClose.isModal = true;
	}

	public void Show()
	{
		AuiButton.modalActive = true;
		base.gameObject.SetActiveRecursively(true);
		for (int i = 0; i < 4; i++)
		{
			string text = "img_tutorial_" + string.Format("{0:00}", i + 1) + "_" + UserSetting.language;
			texTutorial[i] = ResourceManager.Load("interface/images/tutorial", text, typeof(Texture)) as Texture;
		}
		curPage = 0;
		infoImage.GetComponent<Renderer>().sharedMaterial.mainTexture = texTutorial[curPage];
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	public void Hide()
	{
		AuiButton.modalActive = false;
		base.gameObject.SetActiveRecursively(false);
	}

	private void Update()
	{
		if (!Input.GetMouseButton(0))
		{
			return;
		}
		Vector3 mousePosition = Input.mousePosition;
		mousePosition = buttonClose.uiCamera.ScreenToWorldPoint(mousePosition);
		if (mousePosition.x >= 400f && mousePosition.y < 236f)
		{
			if (mousePosition.y > 97f)
			{
				curPage = 0;
			}
			else if (mousePosition.y > -42f)
			{
				curPage = 1;
			}
			else if (mousePosition.y > -181f)
			{
				curPage = 2;
			}
			else
			{
				curPage = 3;
			}
			infoImage.GetComponent<Renderer>().sharedMaterial.mainTexture = texTutorial[curPage];
		}
	}

	private void OnCloseClick(AuiButton sender)
	{
		Hide();
	}
}
