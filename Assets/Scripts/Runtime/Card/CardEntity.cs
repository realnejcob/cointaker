using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New CardEntity", menuName = "Custom/Create/New Card Entity")]
public class CardEntity : ScriptableObject {
    [ReadOnly] public string id;

    [Space(25)]

    public string displayName;
    [TextArea] public string description;
    public int strength;
    public List<CardAbilityBase> cardAbilities;
    public Sprite cardGraphic;

    private void Awake() {
        InitializeGUID();
    }

    private void InitializeGUID() {
        if (id == null) {
            id = Guid.NewGuid().ToString();
        }
    }
}
