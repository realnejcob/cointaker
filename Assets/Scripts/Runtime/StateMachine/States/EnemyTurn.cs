using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyTurn : State {
    public EnemyTurn(BattleSystem battlesystem) : base(battlesystem) {
    }

    public override IEnumerator Start() {
        BattleSystem.boardManager.debugView.SetIndicatorText("Enemy Turn", Color.red);

        yield return new WaitForSeconds(0.5f);

        PerformAction(GetAction());

        yield return new WaitForSeconds(0.5f);

        BattleSystem.enemyAI.ResetCardsToMove();

        yield return new WaitForSeconds(0.25f);

        BoardManager.Instance.DrawCards();

        CheckAllCardsToFlipDirection();

        // Check if player is out of cards, then go to lose state
        // Else, continue game loop starting with telegraph

        BattleSystem.SetState(new EnemyTelegraph(BattleSystem));
    }

    private ActionType GetAction() {
        if (!BattleSystem.enemyAI.HasCardsToMove())
            return ActionType.NONE;

        var cardToMove = BattleSystem.enemyAI.cardsToMove[0];
        var targetSpace = BoardManager.Instance.GetSpaceFromDirection(cardToMove.GetSpace(), cardToMove.MoveDirection);

        if (targetSpace == null)
            return ActionType.NONE;

        if (!targetSpace.GetIsMoveable())
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
                MoveSingleCard();
                break;
            case ActionType.ATTACK:
                Attack();
                break;
            case ActionType.SHIFT:
                Shift();
                break;
            default:
                break;
        }
    }

    public void MoveSingleCard() {
        var cardToMove = BattleSystem.enemyAI.cardsToMove[0];
        var targetSpace = BoardManager.Instance.GetSpaceFromDirection(cardToMove.GetSpace(), cardToMove.MoveDirection);
        Move(cardToMove, targetSpace);
    }

    private void Move(Card cardToMove, Space targetSpace) {
        cardToMove.Move(targetSpace);
    }

    private void Attack() {
        var cardToMove = BattleSystem.enemyAI.cardsToMove[0];
        var targetCard = BoardManager.Instance.GetSpaceFromDirection(cardToMove.GetSpace(), cardToMove.MoveDirection).GetTopCard();

        var cardsLeftOnTargetSpace = targetCard.GetSpace().CardsCount-1;
        var targetCardCoinsCount = targetCard.CoinsCount();
        targetCard.TakeHitPoint(out var isEliminated);

        if (isEliminated && cardsLeftOnTargetSpace <= 0) {
            cardToMove.AddCoins(targetCardCoinsCount);
            MoveSingleCard();
            return;
        }

        if (isEliminated && cardsLeftOnTargetSpace > 0) {
            cardToMove.AddCoins(targetCardCoinsCount);
            return;
        }

        cardToMove.StealCoins(targetCard, 1);
    }

    private void Shift() {
        if (!BattleSystem.enemyAI.cardsToMove[0].HasEmptySpaceInMoveDirection(out var connectedSpaces))
            return;

        var spacesToShift = new List<Space>();
        for (int i = 0; i < connectedSpaces.Count; i++) {
            var space = connectedSpaces[i];
            spacesToShift.Add(space);
            if (connectedSpaces[i + 1].IsFree && connectedSpaces[i + 1].GetIsMoveable())
                break;
        }

        var dir = BattleSystem.enemyAI.cardsToMove[0].MoveDirection;
        spacesToShift.Reverse();

        for (int i = 0; i < spacesToShift.Count; i++) {
            var cardsToMove = new List<Card>();
            foreach (var cardOnSpace in spacesToShift[i].Cards) {
                cardsToMove.Add(cardOnSpace);
            }

            foreach (var card in cardsToMove) {
                var targetSpace = BoardManager.Instance.GetSpaceFromDirection(spacesToShift[i], dir);
                Move(card, targetSpace);
            }
        }
    }

    private void CheckAllCardsToFlipDirection() {
        var enemyCards = BoardManager.Instance.EnemyCards;

        foreach (var card in enemyCards) {
            CheckToFlipDirection(card);
        }
    }

    private void CheckToFlipDirection(EnemyCard cardToMove) {
        var moveDirection = cardToMove.MoveDirection;

        if (cardToMove.HasEmptySpaceInMoveDirection(out var spaces))
            return;

        if (cardToMove.IsNextMovePossible(moveDirection))
            return;

        if (!cardToMove.IsNextMovePossible(cardToMove.GetFlippedDirection(moveDirection)))
            return;

        cardToMove.FlipDirection();
    }
}
