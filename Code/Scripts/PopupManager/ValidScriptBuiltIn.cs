using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValidScriptBuiltIn : ValidScriptBase
{
    [IndexAsEnum(BuiltInEnumType.BuiltInValidEnum)]
    [SerializeField] private IntBool[] validates;
 

    protected override bool IsThisValid()
    {
        foreach (var intBool in validates)
        {
            var result = RootView.rootView.gameController.CheckBuiltInValid(intBool.value);
            if (intBool.value2)
                result = !result;

            if (!result)
                return false;
        }

        return true;
    }
}
