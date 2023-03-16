using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinRedistribute : State {
    public CoinRedistribute(BattleSystem battlesystem) : base(battlesystem) {
    }

    public override IEnumerator Start() {
        BattleSystem.boardManager.debugView.SetIndicatorText("Coins redistribute");
        BattleSystem.boardManager.debugView.SetCoinReserveText(BattleSystem.coinsReserve.ToString());
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
        BattleSystem.boardManager.debugView.SetCoinReserveText(BattleSystem.coinsReserve.ToString());

        if (BattleSystem.coinsReserve == 0)
            BattleSystem.SetState(new EnemyTurn(BattleSystem));

        yield break;
    }
}
