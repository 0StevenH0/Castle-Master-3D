using UnityEngine;

public class MessageView : MonoBehaviour
{
	public enum MsgButton
	{
		yes = 0,
		no = 1,
		submit = 2,
		cancel = 3
	}

	public enum MsgIcon
	{
		alert = 0,
		question = 1,
		military = 2,
		advisor = 3
	}

	public delegate void OnMessageClickDelegate(MsgButton button);

	private const int maxMsgButton = 4;

	private const float maxDelay = 1.5f;

	private const float _buttonWidth = 194f;

	private float _delay;

	public TextMesh _msg;

	public AuiSprite _icon;

	public AuiSprite _back;

	public GameObject[] _buttonObj;

	public AuiButton[] _buttonSpr;

	public AudioSource _soundErrorMsg;

	private Vector3 _msgPos;

	private bool _visibleMsg;

	private bool _autoHide;

	private MsgButton[] _visibleButtons;

	private float _realShowTime;

	public int messageId;

	public OnMessageClickDelegate onMessageClick;

	public bool visible
	{
		get
		{
			return _visibleMsg;
		}
	}

	private void Start()
	{
		_buttonSpr[0].onButtonClick = OnYesClick;
		_buttonSpr[1].onButtonClick = OnNoClick;
		_buttonSpr[2].onButtonClick = OnSubmitClick;
		_buttonSpr[3].onButtonClick = OnCancelClick;
		_autoHide = false;
		_visibleButtons = null;
		Camera cam = GameObject.Find("UICamera").GetComponent<Camera>();
		AuiButton.SetCameraAllChild(base.transform, cam);
		HideMsg();
	}

	public void MsgVisible(bool visible)
	{
		base.gameObject.SetActive(visible);
	}

	public void ButtonVisible(bool visible)
	{
		GameObject[] buttonObj = _buttonObj;
		foreach (GameObject gameObject in buttonObj)
		{
			gameObject.SetActive(false);
		}
		AuiButton[] buttonSpr = _buttonSpr;
		foreach (AuiButton auiButton in buttonSpr)
		{
			auiButton.gameObject.SetActive(false);
			auiButton.visible = false;
		}
		if (_visibleButtons == null)
		{
			return;
		}
		int num = _visibleButtons.Length - 1;
		float num2 = 198f * (float)num / 2f - 194f + 100f;
		if (num == 0)
		{
			num2 = 0f;
		}
		MsgButton[] visibleButtons = _visibleButtons;
		foreach (MsgButton msgButton in visibleButtons)
		{
			if (visible)
			{
				_buttonObj[(int)msgButton].SetActive(true);
				_buttonSpr[(int)msgButton].gameObject.SetActive(true);
				_buttonSpr[(int)msgButton].visible = true;
				Vector3 position = _buttonObj[(int)msgButton].transform.position;
				position.x = num2;
				_buttonObj[(int)msgButton].transform.position = position;
				position = _buttonSpr[(int)msgButton].transform.position;
				position.x = num2;
				_buttonSpr[(int)msgButton].transform.position = position;
				num2 += 197f;
			}
		}
	}

	public void HideMsg()
	{
		MsgVisible(false);
		_visibleMsg = false;
		AuiButton.modalActive = false;
	}

	public void ShowMsgEx(string st, MsgIcon icon, bool autoHide, MsgButton[] buttons, bool alertSound, OnMessageClickDelegate procResult)
	{
		_msg.text = st;
		_icon.SetFrame((int)icon);
		_visibleButtons = buttons;
		MsgVisible(true);
		ButtonVisible(true);
		onMessageClick = procResult;
		_delay = 0f;
		_autoHide = autoHide;
		_visibleMsg = true;
		_realShowTime = Time.realtimeSinceStartup;
		if (alertSound && _soundErrorMsg != null)
		{
			_soundErrorMsg.volume = (float)UserSetting.volumeEffect / 100f;
			_soundErrorMsg.Play();
		}
		AuiButton.modalActive = true;
		ProcBase.ResetTextWordWarp(_msg, 650f);
	}

	private void Update()
	{
		if (_visibleMsg && _autoHide)
		{
			if (Time.timeScale == 0f)
			{
				_delay = Time.realtimeSinceStartup - _realShowTime;
			}
			else
			{
				_delay += Time.deltaTime;
			}
			if (_delay > 1.5f)
			{
				HideMsg();
			}
		}
		if (_visibleMsg && !_autoHide && Input.GetKeyDown(KeyCode.Escape))
		{
			if (HasVisibleButton(MsgButton.no))
			{
				OnNoClick(null);
			}
			else if (HasVisibleButton(MsgButton.cancel))
			{
				OnCancelClick(null);
			}
			else
			{
				HideMsg();
			}
		}
	}

	private bool HasVisibleButton(MsgButton button)
	{
		if (_visibleButtons == null)
		{
			return false;
		}
		MsgButton[] visibleButtons = _visibleButtons;
		foreach (MsgButton msgButton in visibleButtons)
		{
			if (msgButton == button)
			{
				return true;
			}
		}
		return false;
	}

	public void OnYesClick(AuiButton sender)
	{
		HideMsg();
		if (onMessageClick != null)
		{
			onMessageClick(MsgButton.yes);
		}
	}

	public void OnNoClick(AuiButton sender)
	{
		HideMsg();
		if (onMessageClick != null)
		{
			onMessageClick(MsgButton.no);
		}
	}

	public void OnSubmitClick(AuiButton sender)
	{
		HideMsg();
		if (onMessageClick != null)
		{
			onMessageClick(MsgButton.submit);
		}
	}

	public void OnCancelClick(AuiButton sender)
	{
		HideMsg();
		if (onMessageClick != null)
		{
			onMessageClick(MsgButton.cancel);
		}
	}
}
