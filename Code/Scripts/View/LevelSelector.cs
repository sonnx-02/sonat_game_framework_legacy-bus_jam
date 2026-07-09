using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelector : LevelSelectorBase
{
    [SerializeField] protected BaseBaseActionScript[] actionCurrentLevel;
    public override void OnShow()
    {
        base.OnShow();
        actionCurrentLevel.StartAction(PlayerData.CurrentLevel);
    }
}
