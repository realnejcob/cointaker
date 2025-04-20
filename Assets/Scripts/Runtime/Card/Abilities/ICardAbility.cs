using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICardAbility {
    public void OnStack(GameObject target, Space space);
    public void OnDestack(GameObject target, Space space);
}