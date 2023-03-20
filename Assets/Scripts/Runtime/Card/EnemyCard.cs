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
        switch (MoveDirection) {
            case Direction.Left:
                MoveDirection = Direction.Right;
                break;
            case Direction.Right:
                MoveDirection = Direction.Left;
                break;
            case Direction.Up:
                MoveDirection = Direction.Down;
                break;
            case Direction.Down:
                MoveDirection = Direction.Up;
                break;
        }
    }

    public bool IsNextMovePossible() {
        var nextSpace = BoardManager.Instance.GetSpaceFromDirection(this.GetSpace(), moveDirection);
        if (nextSpace == null)
            return false;

        if (!nextSpace.GetIsMoveable())
            return false;

        if (!nextSpace.IsFree && nextSpace.GetTopCard().alignment == AlignmentType.ENEMY)
            return false;

        return true;
    }
}