using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntValueRetrieverGameSave : IntValueRetrieverBase
{
    [IndexAsEnum(BuiltInEnumType.GameSaveKey)]
    [SerializeField] private int[] keys;
    public override int Length => keys.Length;

    public override int this[int index] => RootView.rootView.gameController.GetGameSaveValue(keys[index]); 
}
