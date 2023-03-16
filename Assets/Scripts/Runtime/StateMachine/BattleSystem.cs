using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : StateMachine {
    public BoardManager boardManager;

    [ReadOnly] public Space OriginSpace;
    [ReadOnly] public Space TargetSpace;

    private void Start() {
        SetState(new Begin(this));
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            StartCoroutine(State.MouseDown());
        }
        
        if (Input.GetMouseButtonUp(0)) {
            StartCoroutine(State.MouseUp());
        }
    }
}