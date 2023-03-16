using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotate : MonoBehaviour {

    public float speed = 10;

    Vector3 randomSpeed;
    private void Awake() {
        randomSpeed = new Vector3(Random.Range(-speed / 2, speed / 2), Random.Range(-speed / 2, speed / 2), Random.Range(-speed / 2, speed / 2));
    }

    void Update() {
        transform.Rotate(new Vector3(speed + randomSpeed.x, speed + randomSpeed.y, speed + randomSpeed.z) * Time.deltaTime);
    }
}
