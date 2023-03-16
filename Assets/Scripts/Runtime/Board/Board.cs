using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {
    public Space[,] Spaces { get; private set; }
    [SerializeField] private Space spacePrefab;

    internal void Setup() {
        Spaces = new Space[Grid.rows, Grid.columns];

        for (int x = 0; x < Grid.rows; x++) {
            for (int y = 0; y < Grid.columns; y++) {
                var coordinate = new Vector2Int(x, y);
                Spaces[x, y] = CreateNewSpace(coordinate);
            }
        }
    }

    private Space CreateNewSpace(Vector2Int coordinate) {
        var newSpace = Instantiate(spacePrefab, transform);

        newSpace.coordinate = coordinate;
        newSpace.gameObject.name = $"Space {coordinate}";
        newSpace.transform.position = Grid.GetWorldPosition(newSpace.coordinate);

        newSpace.SetIsMoveable(CheckIfMoveable(coordinate));

        return newSpace;
    }

    private bool CheckIfMoveable(Vector2Int coordinate) {
        if (coordinate == new Vector2Int(0, 0) || coordinate == new Vector2Int(3, 0) || coordinate == new Vector2Int(0, 3) || coordinate == new Vector2Int(3, 3)) {
            return false;
        }

        return true;
    }

    public List<Space> GetMoveableSpaces(Space originSpace) {
        var list = new List<Space>();

        if (GetMoveableSpace(originSpace, new Vector2Int(1, 0), out var spaceRight))
            list.Add(spaceRight);
        if (GetMoveableSpace(originSpace, new Vector2Int(-1, 0), out var spaceLeft))
            list.Add(spaceLeft);
        if (GetMoveableSpace(originSpace, new Vector2Int(0, 1), out var spaceUp))
            list.Add(spaceUp);
        if (GetMoveableSpace(originSpace, new Vector2Int(0, -1), out var spaceDown))
            list.Add(spaceDown);

        return list;
    }

    public bool GetMoveableSpace(Space originSpace, Vector2Int coordinateOffset, out Space space) {
        space = null;

        var coordinateToCheck = originSpace.coordinate + coordinateOffset;
        if (coordinateToCheck.x < 0 || coordinateToCheck.y < 0 || coordinateToCheck.x > Grid.rows-1 || coordinateToCheck.y > Grid.columns - 1) {
            return false;
        }

        space = Spaces[coordinateToCheck.x, coordinateToCheck.y];

        if (!space.GetIsMoveable()) {
            return false;
        }

        return true;
    }
}
