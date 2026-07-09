using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetStringSharedEquipUnlockName : BaseGetStringScript
{
    public override string GetString()
    {
        return BuiltInEnumType.UnlockEquipDataEnum.GetName(SharedProduct.parameter);
    }
}
