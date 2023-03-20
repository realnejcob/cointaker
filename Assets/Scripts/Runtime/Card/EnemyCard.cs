using System;
using System.Collections;
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
            case Direction.Left:
                return Direction.Right;
            case Direction.Right:
                return Direction.Left;
            case Direction.Up:
                return Direction.Down;
            case Direction.Down:
                return Direction.Up;
        }

        return Direction.Right;
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
            case Direction.Left:
                for (int x = originSpace.coordinate.x; x >= 0; x--) {
                    addedSpaces.Add(BoardManager.Instance.Board.Spaces[x, originSpace.coordinate.y]);
                }
                break;
            case Direction.Right:
                for (int x = originSpace.coordinate.x; x < Grid.columns; x++) {
                    addedSpaces.Add(BoardManager.Instance.Board.Spaces[x, originSpace.coordinate.y]);
                }
                break;
            case Direction.Up:
                for (int y = originSpace.coordinate.y; y < Grid.rows; y++) {
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

        spacesInDirection = addedSpaces;

        for (int i = 0; i < addedSpaces.Count; i++) {
            if (addedSpaces[i].IsFree && addedSpaces[i].GetIsMoveable()) {
                return true;
            }
        }

        return false;
    }
}