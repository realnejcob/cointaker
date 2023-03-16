using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour {
    public static ColorManager Instance;

    public Color lightCard;
    public Color darkCard;
    public Color highlightCard;
    public Color highlightText;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        }

        Instance = this;
    }
}
