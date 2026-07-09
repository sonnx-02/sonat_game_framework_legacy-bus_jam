using System;
using System.Collections.Generic;
using System.Linq;
using TMPro.SpriteAssetUtilities;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public struct CurveXyz
{
    [SerializeField] public string name;
    [SerializeField] public AnimationCurve curveScaleX;
    [SerializeField] public AnimationCurve curveScaleY;
    [SerializeField] public AnimationCurve curveScaleZ;
}

[Serializable]
[CreateAssetMenu(fileName = "XyzCurveDatabase.asset", menuName = "Duong/XyzCurveDatabase")]
public class XyzCurveDatabase : ScriptableObject
{
    [SerializeField] private CurveXyz[] curves;
    public CurveXyz[] Curves => curves;
}