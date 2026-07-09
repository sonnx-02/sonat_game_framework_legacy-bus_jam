using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationChecker : BaseChecker
{
    [SerializeField] [IndexAsEnum(BuiltInEnumType.NavigationEnum)]
    private int key;
    [SerializeField] private BaseRxAnimator[] anims;
    protected override bool GetCheck()
    {
        return BaseGameController.CheckNavigation(key,true);
    }

    protected override void OnCheck(bool computeCheck)
    {
        base.OnCheck(computeCheck);
        if (computeCheck)
        {
            0.25f.Timer(() =>
            {
                foreach (var baseRxAnimator in anims)
                    baseRxAnimator.Animator_In();
            });
        }
    }
}