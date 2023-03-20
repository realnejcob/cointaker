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
        if (cardToMove.alignment == AlignmentType.ENEMY)
            CheckToFlipDirection((EnemyCard)cardToMove);
    }

    private void Attack() {
        var cardToMove = BattleSystem.enemyAI.cardsToMove[0];
        var targetCard = BoardManager.Instance.GetSpaceFromDirection(cardToMove.GetSpace(), cardToMove.MoveDirection).GetTopCard();

        var targetCoinsCount = targetCard.CoinsCount();
        targetCard.TakeHitPoint(out var isEliminated);
        
        if (isEliminated) {
            cardToMove.AddCoins(targetCoinsCount);
            MoveSingleCard();
        } else {
            cardToMove.StealCoins(targetCard, 1);
        }
    }

    private void Shift() {
        if (!CanShift(out var connectedSpaces))
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

    private bool CanShift(out List<Space> s) {
        var originCard = BattleSystem.enemyAI.cardsToMove[0];
        var originSpace = originCard.GetSpace();
        var dir = originCard.MoveDirection;
        var addedSpaces = new List<Space>();

        switch (dir) {
            case Direction.Left:
                for (int x = originSpace.coordinate.x; x >= 0; x--) {
                    addedSpaces.Add(BoardManager.Instance.Board.Spaces[x, originSpace.coordinate.y]);
                }
                break;
            case Direction.Right:
                for (int x = originSpace.coordinate.x; x < Grid.rows; x++) {
                    addedSpaces.Add(BoardManager.Instance.Board.Spaces[x, originSpace.coordinate.y]);
                }
                break;
            case Direction.Up:
                for (int y = originSpace.coordinate.y; y < Grid.columns; y++) {
                    addedSpaces.Add(BoardManager.Instance.Board.Spaces[originSpace.coordinate.x, y]);
                }
                break;
            case Direction.Down:
                for (int y = originSpace.coordinate.y; y >= 0; y--) {
                    addedSpaces.Add(BoardManager.Instance.Board.Spaces[originSpace.coordinate.x, y]);
                }
                break;
            default:
                break;
        }

        s = addedSpaces;

        for (int i = 0; i < addedSpaces.Count; i++) {
            if (addedSpaces[i].IsFree && addedSpaces[i].GetIsMoveable()) {
                return true;
            }
        }

        return false;
    }

    private void CheckToFlipDirection(EnemyCard cardToMove) {
        if (CanShift(out var spaces))
            return;

        if (cardToMove.IsNextMovePossible())
            return;

        cardToMove.FlipDirection();
    }
}
