using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid {
    public static Vector2 cellSpacing = new Vector2(2.75f, 3.75f);
    public static int rows = 4;
    public static int columns = 4;

    public static Vector2 GetWorldPosition(Vector2 coordinate) {
        if (coordinate.x > rows-1 || coordinate.y > columns - 1) {
            throw new System.Exception("COORDINATE EXEEDS GRID SIZE! Coordinate vector is zero indexed!");
        }

        var xOffsetToCenter = (rows - 1) * cellSpacing.x / 2;
        var yOffsetToCenter = (columns - 1) * cellSpacing.y / 2;

        var xCellPosition = coordinate.x * cellSpacing.x - xOffsetToCenter;
        var yCellPosition = coordinate.y * cellSpacing.y - yOffsetToCenter;

        return new Vector2(xCellPosition, yCellPosition);
    }

    public static void DrawGizmo() {
        for (int x = 0; x < rows; x++) {
            for (int y = 0; y < columns; y++) {
                var curCellPosition = GetWorldPosition(new Vector2(x,y));
                Gizmos.color = Color.red;
                Gizmos.DrawCube(new Vector3(curCellPosition.x, curCellPosition.y, 0), Vector3.one * 0.5f);
            }
        }
    }
}
