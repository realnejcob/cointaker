using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Begin : State
{
    public Begin(BattleSystem battlesystem) : base(battlesystem) {
    }

    public override IEnumerator Start() {
        BattleSystem.boardManager.debugView.SetIndicatorText("Started", Color.white);
        BattleSystem.boardManager.CreateBoard();

        yield return new WaitForSeconds(0.25f);

        BattleSystem.boardManager.debugView.SetIndicatorText("Waiting...", Color.white);

        yield return new WaitForSeconds(1f);

        //If coin flipping mechanic is used, this would be the place to start that state

        BattleSystem.SetState(new EnemyTelegraph(BattleSystem));
    }
}