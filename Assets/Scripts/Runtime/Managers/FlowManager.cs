using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowManager : MonoBehaviour {
    public static FlowManager Instance;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        }

        Instance = this;
    }
}