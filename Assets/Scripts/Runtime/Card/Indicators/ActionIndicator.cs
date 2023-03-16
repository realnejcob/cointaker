using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionIndicator : MonoBehaviour {
    public RectTransform pointerRectTransform;

    private void Start() {
        Hide();
    }

    public void SetToDirection(Direction direction) {
        switch (direction) {
            case Direction.Left:
                pointerRectTransform.sizeDelta = new Vector2(1.75f, pointerRectTransform.rect.height);
                pointerRectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                break;
            case Direction.Right:
                pointerRectTransform.sizeDelta = new Vector2(1.75f, pointerRectTransform.rect.height);
                pointerRectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                break;
            case Direction.Up:
                pointerRectTransform.sizeDelta = new Vector2(2.25f, pointerRectTransform.rect.height);
                pointerRectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                break;
            case Direction.Down:
                pointerRectTransform.sizeDelta = new Vector2(2.25f, pointerRectTransform.rect.height);
                pointerRectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 270));
                break;
            default:
                break;
        }
    }

    public void Show() {
        pointerRectTransform.gameObject.SetActive(true);
    }

    public void Hide() {
        pointerRectTransform.gameObject.SetActive(false);
    }
}

public enum Direction { Left, Right, Up, Down }
