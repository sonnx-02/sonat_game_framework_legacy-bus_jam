using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyDataGameChecker : BaseChecker
{
    [SerializeField] [IndexAsEnum(BuiltInEnumType.GameCheckerEnum)] private int key;

    protected override bool GetCheck()
    {
        return DailyData.gameChecker[key];
    }
}
