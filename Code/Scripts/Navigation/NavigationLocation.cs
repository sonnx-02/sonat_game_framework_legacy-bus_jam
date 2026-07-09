using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class NavigationLocation : BaseFocusLocation
{
    [IndexAsEnum(BuiltInEnumType.NavigationEnum)]
    public int navigationEnum;
    public int step;
    public bool isFinish;
}