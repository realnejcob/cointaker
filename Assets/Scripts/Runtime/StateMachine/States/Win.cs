using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Win : State {
    public Win(BattleSystem battlesystem) : base(battlesystem) {
    }

    public override IEnumerator Start() {
        BoardManager.Instance.debugView.SetIndicatorText("Win", Color.green);
        BoardManager.Instance.debugView.EnableRestartGroup(true);
        BattleSystem.enemyAI.ResetCardsToMove();
        yield break;
    }
}
