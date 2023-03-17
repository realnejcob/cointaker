using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDirectionIndicator : MonoBehaviour {
    public Transform arrow;

    public void SetDirection(Direction direction) {
        switch (direction) {
            case Direction.Left:
                arrow.rotation = Quaternion.Euler(Vector3.forward * -270);
                break;
            case Direction.Right:
                arrow.rotation = Quaternion.Euler(Vector3.forward * -90);
                break;
            case Direction.Up:
                arrow.rotation = Quaternion.Euler(Vector3.zero);
                break;
            case Direction.Down:
                arrow.rotation = Quaternion.Euler(Vector3.forward * -180);
                break;
        }
    }

}
