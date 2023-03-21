using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugView : MonoBehaviour {
    public TextMeshProUGUI stateIndicatorTMP;
    public TextMeshProUGUI coinReserveTMP;
    public GameObject restartGroup;
    public Button restartButton;

    private void Awake() {
        restartButton.onClick.AddListener(() => {
            GameManager.Instance.ReloadScene();
        });

        EnableRestartGroup(false);
    }

    public void SetIndicatorText(string newText, Color color) {
        stateIndicatorTMP.text = $"STATE: {newText}";
        stateIndicatorTMP.color = color;
    }

    public void SetCoinReserveText(string newText) {
        coinReserveTMP.text = $"Coin Reserve [{newText}]";
    }

    public void EnableRestartGroup(bool enable) {
        restartGroup.SetActive(enable);
    }
}
