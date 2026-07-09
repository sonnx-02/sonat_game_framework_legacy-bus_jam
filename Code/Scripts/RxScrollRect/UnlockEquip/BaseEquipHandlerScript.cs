using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEquipHandlerScript : BaseFrameworkView
{
    public abstract void Handler(UnlockEquipItem scrollItem, int unlockEquipDataEnum);
//    {
//       var data = PlayerData.equipData[unlockEquipDataEnum];
//    }
}
