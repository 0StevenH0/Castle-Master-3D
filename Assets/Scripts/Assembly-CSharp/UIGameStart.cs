using System.Collections;
using UnityEngine;

public class UIGameStart : MonoBehaviour
{
	public TextMesh textTalk;

	private Vector3 transPos;

	private TransformAnimation transAni;

	private void Start()
	{
		if (PlayInfo.battleInfo.isAttack)
		{
			textTalk.text = StringContent.msgBattleStartAttack;
		}
		else
		{
			textTalk.text = StringContent.msgBattleStartDefense;
		}
		transAni = base.gameObject.AddComponent<TransformAnimation>();
		transPos = base.transform.localPosition;
		transAni.moveTo = transPos;
		transAni.moveFrom = transPos;
		transAni.moveFrom.y -= 250f;
		transAni.speed = 500f;
		transAni.delay = 0.5f;
		transAni.StartMove();
		StartCoroutine("WaitForBattleStart");
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	private IEnumerator WaitForBattleStart()
	{
		yield return new WaitForSeconds(3f);
		transAni.moveTo = transPos;
		transAni.moveFrom = transPos;
		transAni.moveTo.y -= 250f;
		transAni.speed = 500f;
		transAni.delay = 0f;
		transAni.StartMove();
		StartCoroutine("WaitForDestory");
	}

	private IEnumerator WaitForDestory()
	{
		yield return new WaitForSeconds(0.5f);
		Object.Destroy(base.gameObject);
	}
}
