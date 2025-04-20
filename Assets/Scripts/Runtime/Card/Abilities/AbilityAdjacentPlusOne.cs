using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Adjacent Stay", menuName = "Custom/Create/Ability/Adjacent Stay")]
public class AbilityAdjacentStay : CardAbilityBase {
    public int amount = 1;
    public int iterations = 1;
    public AdjacentType type;

    public override void OnStack(GameObject target, Space space) {
        var adjacentSpaces = new List<Space>();

        switch (type) {
            case AdjacentType.ALL_DIRECTION:
                adjacentSpaces = BoardManager.Instance.Board.GetAdjacentSpaces(space, iterations);
                break;
            case AdjacentType.HORIZONTAL:
                adjacentSpaces = BoardManager.Instance.Board.GetHorizontalSpaces(space, iterations);
                break;
            case AdjacentType.VERTICAL:
                adjacentSpaces = BoardManager.Instance.Board.GetVerticalSpaces(space, iterations);
                break;
            default:
                break;
        }

        foreach (Space sp in adjacentSpaces) {
            sp.TempStrength += amount;
        }
    }

    public override void OnDestack(GameObject target, Space space) {
        var adjacentSpaces = new List<Space>();

        switch (type) {
            case AdjacentType.ALL_DIRECTION:
                adjacentSpaces = BoardManager.Instance.Board.GetAdjacentSpaces(space, iterations);
                break;
            case AdjacentType.HORIZONTAL:
                adjacentSpaces = BoardManager.Instance.Board.GetHorizontalSpaces(space, iterations);
                break;
            case AdjacentType.VERTICAL:
                adjacentSpaces = BoardManager.Instance.Board.GetVerticalSpaces(space, iterations);
                break;
            default:
                break;
        }

        foreach (Space sp in adjacentSpaces) {
            sp.TempStrength -= amount;
        }
    }
}

public enum AdjacentType {
    ALL_DIRECTION,
    HORIZONTAL,
    VERTICAL
}
