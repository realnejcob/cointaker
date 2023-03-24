using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDirectionIndicator : MonoBehaviour {
    public Transform arrow;

    public void SetDirection(Direction direction) {
        switch (direction) {
            case Direction.LEFT:
                arrow.rotation = Quaternion.Euler(Vector3.forward * -270);
                break;
            case Direction.RIGHT:
                arrow.rotation = Quaternion.Euler(Vector3.forward * -90);
                break;
            case Direction.UP:
                arrow.rotation = Quaternion.Euler(Vector3.zero);
                break;
            case Direction.DOWN:
                arrow.rotation = Quaternion.Euler(Vector3.forward * -180);
                break;
        }
    }

}
