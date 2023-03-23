using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Array2DEditor
{
    [CustomPropertyDrawer(typeof(Array2DSpaceType))]
    public class Array2DSpaceTypeDrawer : Array2DEnumDrawer<SpaceType> {
        public override void DisplayGrid(Rect position) {
            var cellRect = new Rect(
                position.x, position.y,
                cellSizeProperty.vector2IntValue.x,
                cellSizeProperty.vector2IntValue.y);

            for (var y = 0; y < gridSizeProperty.vector2IntValue.y; y++) {
                for (var x = 0; x < gridSizeProperty.vector2IntValue.x; x++) {
                    var pos = new Rect(cellRect) {
                        x = cellRect.x + (cellRect.width + cellSpacing.x) * x,
                        y = cellRect.y + (cellRect.height + cellSpacing.y) * y
                    };

                    var property = GetRowAt(y).GetArrayElementAtIndex(x);

                    if (property.propertyType == SerializedPropertyType.ObjectReference) {
                        var match = Regex.Match(property.type, @"PPtr<\$(.+)>");
                        if (match.Success) {
                            var objectType = match.Groups[1].ToString();
                            var assemblyName = "UnityEngine";
                            EditorGUI.ObjectField(pos, property, System.Type.GetType($"{assemblyName}.{objectType}, {assemblyName}"), GUIContent.none);
                        }
                    } else {
                        EditorGUI.DrawRect(pos, ColorFromInt.GetColor((SpaceType)property.intValue));
                        EditorGUI.PropertyField(pos, property, GUIContent.none);
                        GUI.color = Color.white;
                    }
                }
            }
        }
    }
    public static class ColorFromInt {
        public static Color GetColor(SpaceType type) {
            switch (type) {
                case SpaceType.NONE:
                    return new Color(1, 1, 1, 0);
                case SpaceType.PLAYER:
                    return new Color(0.5f, 1, 0.5f);
                case SpaceType.PLAYER_SPAWN:
                    return new Color(0.25f, 0.25f, 1);
                case SpaceType.ENEMY:
                    return new Color(1, 0.25f, 0.25f);
                case SpaceType.ENEMY_SPAWN:
                    return new Color(1, 0.75f, 0f);
                case SpaceType.FREE:
                    return Color.white;
                default:
                    return Color.white;
            }
        }
    }
}
