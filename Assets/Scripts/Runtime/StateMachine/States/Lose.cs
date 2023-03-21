using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lose : State {
    public Lose(BattleSystem battlesystem) : base(battlesystem) {
    }

    public override IEnumerator Start() {
        BoardManager.Instance.debugView.SetIndicatorText("Lose", Color.green);
        BoardManager.Instance.debugView.EnableRestartGroup(true);
        yield break;
    }
}
