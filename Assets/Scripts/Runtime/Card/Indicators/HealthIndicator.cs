using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthIndicator : MonoBehaviour {
    [SerializeField] private GameObject[] hitpoints;

    private void Start() {
        ResetHitpoints();
    }

    public void ResetHitpoints() {
        foreach (var hitpoint in hitpoints) {
            hitpoint.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void UpdateIndicator(int hitPoints) {
        ResetHitpoints();

        for (int i = 0; i < hitPoints; i++) {
            hitpoints[i].transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
