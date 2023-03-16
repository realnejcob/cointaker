using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : StateMachine {
    public BoardManager boardManager;

    private void Start() {
        SetState(new Begin(this));
    }
}