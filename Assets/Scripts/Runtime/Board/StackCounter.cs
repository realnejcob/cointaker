using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StackCounter : MonoBehaviour {
    [SerializeField] private GameObject parentGameObject;
    [SerializeField] private TextMeshProUGUI tmpCount;

    public void SetCountText(int count) {
        if (count > 1) {
            parentGameObject.SetActive(true);
            tmpCount.text = $"+{count-1}";
            return;
        }

        parentGameObject.SetActive(false);
    }
}
