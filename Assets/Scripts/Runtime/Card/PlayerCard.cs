using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCard : Card {
    [Space(25)]
    [Header("---")]
    [SerializeField] private CanvasGroup feedbackIndicator;
    private Image feedbackIndicatorImage;
    private float fadeSpeed = 1;

    public void FeedbackFlash(FeedbackIndicatorType type) {
        if (feedbackIndicatorImage == null)
            feedbackIndicatorImage = feedbackIndicator.GetComponent<Image>();

        var color = GetIndicatorColor(type);
        feedbackIndicatorImage.color = color;

        feedbackIndicator.alpha = 1;
        LeanTween.value(1, 0, fadeSpeed).setOnUpdate((float t) => {
            feedbackIndicator.alpha = t;
        }).setEaseOutCirc();
    }

    private Color32 GetIndicatorColor(FeedbackIndicatorType type) {
        switch (type) {
            case FeedbackIndicatorType.HIT:
                return new Color32(243, 75, 70, 255);
            case FeedbackIndicatorType.HEAL:
                return new Color32(208, 253, 118, 255);
            case FeedbackIndicatorType.SPAWN:
                return Color.white;
            default:
                return Color.white;
        }
    }
}

public enum FeedbackIndicatorType {
    HIT,
    HEAL,
    SPAWN
}