using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Array2DEditor;

[CreateAssetMenu(fileName = "boardLayout_", menuName = "Custom/Create/New Board Layout")]
public class BoardLayout : ScriptableObject {
    public string key;
    public Array2DSpaceType layout;
}
