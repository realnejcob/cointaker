using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurn : State {
    public EnemyTurn(BattleSystem battlesystem) : base(battlesystem) {
    }

    public override IEnumerator Start() {
        BattleSystem.boardManager.debugView.SetIndicatorText("Enemy Turn", Color.red);

        yield return new WaitForSeconds(0.5f);

        PerformAction(GetAction());

        yield return new WaitForSeconds(0.5f);

        BattleSystem.enemyAI.UnhighlightCardsToMove();

        // Check if player is out of cards, then go to lose state
        // Else, continue game loop starting with telegraph

        BattleSystem.SetState(new EnemyTelegraph(BattleSystem));
    }

    private ActionType GetAction() {
        var cardToMove = BattleSystem.enemyAI.cardsToMove[0];
        if (cardToMove == null)
            return ActionType.NONE;

        var targetSpace = BoardManager.Instance.GetSpaceFromDirection(cardToMove.GetSpace(), cardToMove.MoveDirection);
        if (targetSpace == null)
            return ActionType.NONE;

        if (targetSpace.IsFree)
            return ActionType.MOVE;

        var targetTopCard = targetSpace.GetTopCard();

        switch (targetTopCard.alignment) {
            case AlignmentType.PLAYER:
                return ActionType.ATTACK;
            case AlignmentType.ENEMY:
                return ActionType.SHIFT;
            default:
                break;
        }

        return ActionType.NONE;
    }

    public void PerformAction(ActionType actionType) {
        switch (actionType) {
            case ActionType.MOVE:
                Move();
                break;
            case ActionType.ATTACK:
                Attack();
                break;
            case ActionType.SHIFT:
                break;
            default:
                break;
        }
    }

    public void Move() {
        var cardToMove = BattleSystem.enemyAI.cardsToMove[0];
        var targetSpace = BoardManager.Instance.GetSpaceFromDirection(cardToMove.GetSpace(), cardToMove.MoveDirection);

        cardToMove.GetSpace().RemoveFromSpace(cardToMove);

        cardToMove.transform.SetParent(targetSpace.transform);
        targetSpace.AddToSpace(cardToMove);

        cardToMove.RefreshDesiredPosition();
        cardToMove.StartMoveZoomTween();
    }

    private void Attack() {
        var cardToMove = BattleSystem.enemyAI.cardsToMove[0];
        var targetCard = BoardManager.Instance.GetSpaceFromDirection(cardToMove.GetSpace(), cardToMove.MoveDirection).GetTopCard();

        var targetCoinsCount = targetCard.CoinsCount();
        targetCard.TakeHitPoint(out var isEliminated);
        
        if (isEliminated) {
            cardToMove.AddCoins(targetCoinsCount);
            Move();
        } else {
            StealCoins(cardToMove, targetCard, 1);
        }
    }

    private void StealCoins(Card to, Card from, int amount) {
        if (from.CoinsCount() == 0)
            return;

        from.RemoveCoins(amount);
        to.AddCoins(amount);
    }
}
