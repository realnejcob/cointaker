using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTelegraph : State
{
    public EnemyTelegraph(BattleSystem battlesystem) : base(battlesystem) {
    }

    public override IEnumerator Start() {
        BattleSystem.boardManager.stateIndicator.SetIndicatorText("Enemy telegraph");

        // Pick enemy card(s) to move

        yield return new WaitForSeconds(0.05f);

        BattleSystem.SetState(new PlayerTurn(BattleSystem));
    }
}
