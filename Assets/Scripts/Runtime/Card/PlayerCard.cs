using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCard : Card {
    [Space(25)]
    [Header("---")]
    [SerializeField] private CanvasGroup hitIndicator;
    [SerializeField] private CanvasGroup healIndicator;
    private float fadeSpeed = 2;

    public void MakeHit() {
        hitIndicator.alpha = 1;
        LeanTween.value(1, 0, fadeSpeed).setOnUpdate((float t) => {
            hitIndicator.alpha = t;
        }).setEaseOutCirc();
    }

    public void MakeHeal() {
        healIndicator.alpha = 1;
        LeanTween.value(1, 0, fadeSpeed).setOnUpdate((float t) => {
            healIndicator.alpha = t;
        }).setEaseOutCirc();
    }
}