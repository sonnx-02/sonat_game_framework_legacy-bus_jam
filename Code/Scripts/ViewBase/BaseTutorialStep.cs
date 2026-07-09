using UnityEngine;
using UnityEngine.Serialization;

public class BaseTutorialStep : ScriptableObject
{
    [FormerlySerializedAs("stepType")] [IndexAsEnum(BuiltInEnumType.TutorialActionEnum)]
    public int actionEnum; // enum name of tutorial type, like drag block, rotate block, etc
    [IndexAsEnum(BuiltInEnumType.TutorialActionParam)] [FormerlySerializedAs("stepType2")] 
    public int actionParam;
    [IndexAsEnumFlag(BuiltInEnumType.TutorialEnableFlag)] 
    public int tutEnables;
    public int enable; 
    public bool actionOnly; // only doing GameAction in actionOnStart, ex : turn something on, to prepare for next tutorial

    public ParameterTutorialMiniAction[] actionOnStart; // action when start tutorial
    public ParameterTutorialMiniAction[] actionBeforeNext; // action doing before next to next tutorial
    public bool stopWhenNext; // when next will stop tut, use indexStopTut value
    public int indexStopTut = -1; // below -1 should be waitIndex of next Tut
    public int waitIndex = 0; // waiting
    public int nextTutAdd = 1; // when next tut add this value
    public int handNodeIndex; // for use and hand animation node
    public bool skipable = true; // button skipable show or not
}