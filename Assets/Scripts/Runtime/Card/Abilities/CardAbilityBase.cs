using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardAbilityBase : ScriptableObject, ICardAbility {
    public string abilityName;
    public string abilityDescription;

    public abstract void OnStack(GameObject target, Space space);
    public abstract void OnDestack(GameObject target, Space space);
}
