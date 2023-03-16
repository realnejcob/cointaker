using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurn : State {
    public EnemyTurn(BattleSystem battlesystem) : base(battlesystem) {
    }

    public override IEnumerator Start() {
        BattleSystem.boardManager.stateIndicator.SetIndicatorText("Enemy Turn");

        // Perform action(s) of card(s) chosen in the enemy telegraph state

        yield return new WaitForSeconds(1);

        // Check if player is out of cards, then go to lose state
        // Else, continue game loop starting with telegraph

        BattleSystem.SetState(new EnemyTelegraph(BattleSystem));
    }
}
