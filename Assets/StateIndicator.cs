using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StateIndicator : MonoBehaviour {
    public TextMeshProUGUI indicatorTMP;

    public void SetIndicatorText(string newText) {
        indicatorTMP.text = newText;
    }
}
