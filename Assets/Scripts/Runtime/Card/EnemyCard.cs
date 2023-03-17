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
}