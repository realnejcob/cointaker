using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCard : Card {
    [Space(25)]
    [Header("---")]
    [SerializeField] private MoveDirectionIndicator moveDirectionIndicator;

    public Direction MoveDirection {
        get { return moveDirection; }
        set { moveDirection = SetDirection(value); }
    }

    private Direction moveDirection;

    private Direction SetDirection(Direction direction) {
        moveDirectionIndicator.SetDirection(direction);
        return direction;
    }

    public void FlipDirection() {
        MoveDirection = GetFlippedDirection(MoveDirection);
    }

    public Direction GetFlippedDirection(Direction direction) {
        switch (direction) {
            case Direction.LEFT:
                return Direction.RIGHT;
            case Direction.RIGHT:
                return Direction.LEFT;
            case Direction.UP:
                return Direction.DOWN;
            case Direction.DOWN:
                return Direction.UP;
        }

        return Direction.RIGHT;
    }

    public bool IsNextMovePossible(Direction direction) {
        var nextSpace = BoardManager.Instance.GetSpaceFromDirection(GetSpace(), direction);
        if (nextSpace == null)
            return false;

        if (!nextSpace.GetIsMoveable())
            return false;

        if (!nextSpace.IsFree && nextSpace.GetTopCard().alignment == AlignmentType.ENEMY)
            return false;

        return true;
    }

    public bool HasEmptySpaceInMoveDirection(out List<Space> spacesInDirection) {
        var originSpace = GetSpace();
        var dir = MoveDirection;
        var addedSpaces = new List<Space>();

        switch (dir) {
            case Direction.LEFT:
                for (int x = originSpace.coordinate.x; x >= 0; x--) {
                    addedSpaces.Add(BoardManager.Instance.Board.Spaces[x, originSpace.coordinate.y]);
                }
                break;
            case Direction.RIGHT:
                for (int x = originSpace.coordinate.x; x < Grid.columns; x++) {
                    addedSpaces.Add(BoardManager.Instance.Board.Spaces[x, originSpace.coordinate.y]);
                }
                break;
            case Direction.UP:
                for (int y = originSpace.coordinate.y; y < Grid.rows; y++) {
                    addedSpaces.Add(BoardManager.Instance.Board.Spaces[originSpace.coordinate.x, y]);
                }
                break;
            case Direction.DOWN:
                for (int y = originSpace.coordinate.y; y >= 0; y--) {
                    addedSpaces.Add(BoardManager.Instance.Board.Spaces[originSpace.coordinate.x, y]);
                }
                break;
            default:
                break;
        }

        spacesInDirection = addedSpaces;

        for (int i = 0; i < addedSpaces.Count; i++) {
            if (addedSpaces[i].IsFree && addedSpaces[i].GetIsMoveable()) {
                return true;
            }
        }

        return false;
    }

    public int GetBestMove() {
        int bestMoveValue = -1;
        Direction bestMoveDirection = moveDirection;
        foreach (Direction direction in Enum.GetValues(typeof(Direction))) {
            var nextSpace = BoardManager.Instance.GetSpaceFromDirection(GetSpace(), direction);

            // Negative move value represents null
            if (nextSpace == null)
                continue;

            // Zero represents empty
            if (nextSpace.GetIsMoveable() && nextSpace.IsFree)
                if (bestMoveValue < 0) {
                    bestMoveValue = 0;
                    bestMoveDirection = direction;
                }

            // Positive values represent the power superiority to the player card
            if (!nextSpace.IsFree && nextSpace.GetTopCard().alignment == AlignmentType.PLAYER) {
                int strengthDifference = strength - nextSpace.GetTopCard().GetTotalStrength();
                if (strengthDifference > bestMoveValue) {
                    bestMoveValue = strengthDifference;
                    bestMoveDirection = direction;
                }
            }
        }
        MoveDirection = bestMoveDirection;
        SetDirection(bestMoveDirection);
        return bestMoveValue;
    }

    public override void CardSpecificEliminate() {
        BoardManager.Instance.EnemyCards.Remove(this);

        var enemyAI = BoardManager.Instance.battleSystem.enemyAI;
        if (enemyAI.GetCardWillMove(this)) {
            enemyAI.ResetCardsToMove();
        }
    }
}