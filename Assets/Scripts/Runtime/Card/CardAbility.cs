using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbility {
    public static string GetDescription(Type ability) {
        switch (ability) {
            case Type.None:
                break;
            default:
                break;
        }

        return "";
    }

    public enum Type {
        None = 0
    }
}
