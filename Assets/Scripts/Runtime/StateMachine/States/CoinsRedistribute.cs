using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsRedistribute : State {
    public CoinsRedistribute(BattleSystem battlesystem) : base(battlesystem) {
    }

    public override IEnumerator Start() {
        BattleSystem.boardManager.stateIndicator.SetIndicatorText("Coins redistribute");
        yield break;
    }

    public override IEnumerator MouseDown() {
        var targetSpace = BattleSystem.targetSpace;

        if (targetSpace == null)
            yield break;

        if (targetSpace.IsFree)
            yield break;

        var topCard = targetSpace.GetTopCard();

        if (topCard.alignment == AlignmentType.OPPONENT)
            yield break;

        topCard.AddCoins(1);
        BattleSystem.coinsReserve -= 1;

        if (BattleSystem.coinsReserve == 0)
            BattleSystem.SetState(new EnemyTurn(BattleSystem));

        yield break;
    }
}
