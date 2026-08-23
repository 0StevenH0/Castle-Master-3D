using System.Collections;
using UnityEngine;

public class UIBattleInfo : MonoBehaviour
{
	public TextMesh textAllyUnits;

	public TextMesh textEnemyUnits;

	public TextMesh textBattleTime;

	public TextMesh textCastleName;

	public AuiSprite gaugeGate;

	public StageManager battleStage;

	private void Start()
	{
		gaugeGate.isCrop = true;
		StartCoroutine("RefreshBattleInfo");
		ProcBase.ChangeTextMeshLanguageAllChild(base.transform);
	}

	private IEnumerator RefreshBattleInfo()
	{
		UnitControl castleGate = battleStage.castleGate;
		while (true)
		{
			int allyUnits = 0;
			int enemyUnits = 0;
			int sideAlly = ((!PlayInfo.battleInfo.isAttack) ? 1 : 0);
			int enemyAlly = (PlayInfo.battleInfo.isAttack ? 1 : 0);
			for (int iv = 0; iv < 5; iv++)
			{
				allyUnits += battleStage.stageInfo.aliveUser[sideAlly, iv];
				enemyUnits += battleStage.stageInfo.aliveUser[enemyAlly, iv];
			}
			textAllyUnits.text = allyUnits.ToString();
			textEnemyUnits.text = enemyUnits.ToString();
			textCastleName.text = PlayInfo.battleInfo.defenseCastle.castleName;
			gaugeGate.crop = new Rect(0f, 0f, castleGate.unitState.curHp / (float)castleGate.unitState.sumHp, 1f);
			gaugeGate.SetFrame(0);
			int passTime = (int)((float)PlayInfo.gameRule.battleTimeLimit - battleStage.battleTime);
			if (passTime < 0)
			{
				passTime = 0;
			}
			textBattleTime.text = string.Format("{0:0}:{1:00}", passTime / 60, passTime % 60);
			yield return new WaitForSeconds(0.5f);
		}
	}
}
