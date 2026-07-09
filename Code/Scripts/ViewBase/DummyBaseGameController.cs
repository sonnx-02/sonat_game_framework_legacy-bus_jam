using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public abstract class DummyBaseGameController : BaseGameController
{
    public override BaseCurrentGameSetting BaseCurrentGameSetting => null;
    protected IntReactiveProperty[] gameSaveProperty;

    protected override void RegisterCurrentGame()
    {
        if (gameSaveProperty == null)
        {
            gameSaveProperty = new IntReactiveProperty[10];
            for (var i = 0; i < gameSaveProperty.Length; i++)
                gameSaveProperty[i] = new IntReactiveProperty();
        }
    }
    
    protected override void StartCurrentGameSave(int playMode = 0)
    {
        base.StartCurrentGameSave(playMode);
        InitGame();
    }



    public override IntReactiveProperty GetGameSaveProperty(int key)
    {
        return GameSaveDisplayProperty[key];
    }

    public override void SaveTemp()
    {
    }

    public override void LoadTemp()
    {
    }

    public override void ClearMovesThenRestartGame()
    {
    }

    protected override void StartPlayerDataLevel()
    {
    }

    public override void StartLevelDirty(object input)
    {
    }

    public override bool IsNotOnAnyAction()
    {
        return false;
    }

    public override void FreeAction()
    {
    }

    protected override void OnInputUpdate()
    {
    }

    public override void Undo()
    {
    }

    public override void UpdateActionTime()
    {
     
    }

    protected override bool IsBoardWin()
    {
        return false;
    }

    protected override bool IsBoardLose()
    {
        return false;
    }

    protected override void SaveLevel()
    {
    }

    protected override void SaveCurrentGameSave()
    {
    }

    protected override bool LoadGameSave()
    {
        return true;
    }

    protected override void DeleteGameSave()
    {
    }

    protected override void UpdateGameSaveDisplay()
    {
    }

    protected override int[] GetGameSaveDisplayPropertyRemap()
    {
       return new int[]
       {
           -999,
           -999,
           -999,
           -999,
           -999,
           -999,
           -999,
           -999,
           -999,
           -999,
           -999,
       };
    }

    protected override void AddSave()
    {
    }

    protected override void ClearMoves()
    {
    }

    public override bool GetCustomCondition(int customGameCondition)
    {
        return true;
    }

    public override Product GetCustomProduct(Product reward)
    {
        return null;
    }

    protected override void ProductGameActionEventHandler(ProductAction product)
    {
      
    }

    public override void HandlerButton(int key, PointParameter parameter)
    {
       
    }
}
