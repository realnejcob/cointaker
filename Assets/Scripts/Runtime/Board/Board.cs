using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {
    public Space[,] Spaces { get; private set; }
    public List<Space> PlayerDrawSpaces { get; private set; } = new List<Space>();
    public List<Space> EnemyDrawSpaces { get; private set; } = new List<Space>();

    [SerializeField] private Space spacePrefab;

    private BoardLayout boardLayout;

    internal void Setup(BoardLayout newBoardLayout) {
        boardLayout = newBoardLayout;

        Grid.columns = boardLayout.layout.GridSize.x;
        Grid.rows = boardLayout.layout.GridSize.y;

        Spaces = new Space[Grid.columns, Grid.rows];

        for (int x = 0; x < Grid.columns; x++) {
            for (int y = 0; y < Grid.rows; y++) {
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

        newSpace.SetIsMoveable(CheckIfMoveable(newSpace));

        if (GetSpaceTypeFromSpace(newSpace) == SpaceType.PLAYER_SPAWN) {
            PlayerDrawSpaces.Add(newSpace);
        }

        if (GetSpaceTypeFromSpace(newSpace) == SpaceType.ENEMY_SPAWN) {
            EnemyDrawSpaces.Add(newSpace);
        }

        return newSpace;
    }

    private bool CheckIfMoveable(Space space) {
        if (GetSpaceTypeFromSpace(space) == SpaceType.NONE) {
            return false;
        }

        return true;
    }

    public List<Space> GetAdjacentSpaces(Space originSpace, int iterations = 1) {
        var list = new List<Space>();

        for (int i = 0; i < iterations; i++) {
            if (GetCanMoveToAdjacentSpace(originSpace, new Vector2Int(1, 0), out var spaceRight))
                list.Add(spaceRight);
            if (GetCanMoveToAdjacentSpace(originSpace, new Vector2Int(-1, 0), out var spaceLeft))
                list.Add(spaceLeft);
            if (GetCanMoveToAdjacentSpace(originSpace, new Vector2Int(0, 1), out var spaceUp))
                list.Add(spaceUp);
            if (GetCanMoveToAdjacentSpace(originSpace, new Vector2Int(0, -1), out var spaceDown))
                list.Add(spaceDown);
        }

        return list;
    }

    public List<Space> GetHorizontalSpaces(Space originSpace, int iterations = 1) {
        var list = new List<Space>();

        for (int i = 0; i < iterations; i++) {
            if (GetCanMoveToAdjacentSpace(originSpace, new Vector2Int(1, 0), out var spaceRight))
                list.Add(spaceRight);
            if (GetCanMoveToAdjacentSpace(originSpace, new Vector2Int(-1, 0), out var spaceLeft))
                list.Add(spaceLeft);
        }

        return list;
    }

    public List<Space> GetVerticalSpaces(Space originSpace, int iterations = 1) {
        var list = new List<Space>();

        for (int i = 0; i < iterations; i++) {
            if (GetCanMoveToAdjacentSpace(originSpace, new Vector2Int(0, 1), out var spaceUp))
                list.Add(spaceUp);
            if (GetCanMoveToAdjacentSpace(originSpace, new Vector2Int(0, -1), out var spaceDown))
                list.Add(spaceDown);
        }

        return list;
    }

    private bool GetCanMoveToAdjacentSpace(Space originSpace, Vector2Int coordinateOffset, out Space space) {
        space = null;

        var coordinateToCheck = originSpace.coordinate + coordinateOffset;
        if (coordinateToCheck.x < 0 || coordinateToCheck.y < 0 || coordinateToCheck.x > Grid.columns-1 || coordinateToCheck.y > Grid.rows - 1) {
            return false;
        }

        space = Spaces[coordinateToCheck.x, coordinateToCheck.y];

        if (!space.GetIsMoveable()) {
            return false;
        }

        return true;
    }

    public SpaceType GetSpaceTypeFromSpace(Space space) {
        var reversedCoordinate = GetReversedCoordinate(space.coordinate);
        var type = boardLayout.layout.GetCell(reversedCoordinate.x, reversedCoordinate.y);
        return type;
    }

    private Vector2Int GetReversedCoordinate(Vector2Int coordinate) {
        var tempCoordinate = (boardLayout.layout.GridSize - Vector2Int.one) - coordinate;
        return new Vector2Int(coordinate.x, Mathf.Abs(tempCoordinate.y));
    }
}
