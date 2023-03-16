using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State {

    protected BattleSystem BattleSystem;

    public State(BattleSystem battlesystem) { 
        BattleSystem = battlesystem;
    }

    public virtual IEnumerator Start() {
        yield break;
    }

    public virtual IEnumerator MouseUp() {
        yield break;
    }

    public virtual IEnumerator MouseDown() {
        yield break;
    }
}