using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : StateMachine {
    public BoardManager boardManager;
    public EnemyAI enemyAI;

    [ReadOnly] public Space originSpace;
    [ReadOnly] public Space targetSpace;
    [ReadOnly] public int coinsReserve;

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

    public void SkipState() {
        StartCoroutine(State.Skip());
    }
}