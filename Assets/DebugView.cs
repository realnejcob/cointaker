using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugView : MonoBehaviour {
    public TextMeshProUGUI stateIndicatorTMP;
    public TextMeshProUGUI coinReserveTMP;

    public void SetIndicatorText(string newText, Color color) {
        stateIndicatorTMP.text = $"STATE: {newText}";
        stateIndicatorTMP.color = color;
    }

    public void SetCoinReserveText(string newText) {
        coinReserveTMP.text = $"Coin Reserve [{newText}]";
    }
}
