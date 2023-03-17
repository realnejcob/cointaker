using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTelegraph : State
{
    public EnemyTelegraph(BattleSystem battlesystem) : base(battlesystem) {
    }

    public override IEnumerator Start() {
        BattleSystem.boardManager.debugView.SetIndicatorText("Enemy telegraph", Color.red);

        yield return new WaitForSeconds(0.25f);

        BattleSystem.enemyAI.ConfigureCardsToMove();

        yield return new WaitForSeconds(0.25f);

        BattleSystem.SetState(new PlayerTurn(BattleSystem));
    }
}
