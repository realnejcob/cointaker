using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurn : State {
    public PlayerTurn(BattleSystem battlesystem) : base(battlesystem) {
    }

    public override IEnumerator Start() {
        BattleSystem.boardManager.stateIndicator.SetIndicatorText("Player Turn");
        yield break;
    }

    public override IEnumerator Action() {
        // Perform action
        // Check if the player has coins to redistribute
            // Redestribute coins
        // If player has all coins, go to win state
        // Else, continue to enemy turn

        BattleSystem.SetState(new EnemyTurn(BattleSystem));
        yield break;
    }
}