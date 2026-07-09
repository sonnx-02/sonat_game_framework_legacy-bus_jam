using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ParameterGameAction
{
    [IndexAsEnum(BuiltInEnumType.ParameterGameActionEnum)]
    public int action;
    public bool valueBool;
    public int valueInt;
    public float valueFloat;
    public Vector3Int valueVec3Int;
    public Vector3 valueVec3;
}


[Serializable]
public struct ParameterTutorialMiniAction
{
    [IndexAsEnum(BuiltInEnumType.TutorialMiniAction)]
    public int action;
    public bool valueBool;
    public int valueInt;
    public float valueFloat;
    public Vector3Int valueVec3Int;
    public Vector3 valueVec3;
}

