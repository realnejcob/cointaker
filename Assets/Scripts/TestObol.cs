using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObol : MonoBehaviour {
    Rigidbody rb;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate() {
        rb.AddForce(Vector3.forward * 20, ForceMode.Acceleration);
    }
}
