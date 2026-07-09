using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseInputScript<T1,T2> : BaseCurrentGameView<T1,T2> where T1 : BaseGameController where T2 : ScriptableObject
{
    protected Camera cam => GameController.cam;

    public abstract void OnInputUpdate();
    
    protected void CheckBoardWin() => GameController.CheckBoardWin();
}