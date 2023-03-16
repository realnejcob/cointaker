using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionIndicator : MonoBehaviour {
    public RectTransform pointerLeftRectTransform;
    public RectTransform pointerRightRectTransform;
    public RectTransform pointerUpRectTransform;
    public RectTransform pointerDownRectTransform;

    private void Start() {
        HideAll();
    }

    public void Show(Direction[] directions) {
        foreach (var d in directions) {
            switch (d) {
                case Direction.Left:
                    pointerLeftRectTransform.gameObject.SetActive(true);
                    break;
                case Direction.Right:
                    pointerRightRectTransform.gameObject.SetActive(true);
                    break;
                case Direction.Up:
                    pointerUpRectTransform.gameObject.SetActive(true);
                    break;
                case Direction.Down:
                    pointerDownRectTransform.gameObject.SetActive(true);
                    break;
            }
        }
    }

    public void HideAll() {
        pointerLeftRectTransform.gameObject.SetActive(false);
        pointerRightRectTransform.gameObject.SetActive(false);
        pointerUpRectTransform.gameObject.SetActive(false);
        pointerDownRectTransform.gameObject.SetActive(false);
    }
}

public enum Direction { Left, Right, Up, Down }
