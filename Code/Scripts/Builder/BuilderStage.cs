using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuilderStage : BaseFrameworkView
{
    [SerializeField] private BoolBindingReactiveProperty bindFinish;
    [SerializeField] private ProgressScript[] bindProgress;
    [SerializeField] private BuilderItem[] items;
    [SerializeField] private IndexBindingScriptBase[] bindState;
    [SerializeField] private BaseBaseActionScript[] actionOnFinishWhole;
    
    protected override void Register()
    {
        base.Register();
        bindFinish.Init();
    }

    public void OnAllFinish()
    {
        foreach (var builderItem in items)
            builderItem.OnAllFinish();
        foreach (var builderItem in actionOnFinishWhole)
            builderItem.StartAction();
    }

    private ArrayInt _target;

    private int _total;
    private int _index;
    private BuilderPlayerData _builderPlayerData;

    public int Total => _total;

    public int Current
    {
        get
        {
            #if UNITY_EDITOR
            if(_index >= _builderPlayerData.stages.Length)
                Debug.LogError("out of range "+_index,gameObject);
            #endif
            return _builderPlayerData.stages[_index];
        }
        set => _builderPlayerData.stages[_index] = value;
    }

    private int CurrentDisplay { get; set; }

    public void SetTarget(ArrayInt target, int stageIndex,BuilderPlayerData playerData)
    {
        _target = target;
        _index = stageIndex;
        _builderPlayerData = playerData;
        CurrentDisplay = Current;
        _total = _target.Sum();
        foreach (var progressScript in bindProgress)
            progressScript.CheckRegister();
        UpdateProgress();
//        UpdateProgress();

        items.SetActiveAll(false);
        
        var inProgress = CurrentInprogress();
        for (var i = 0; i < Mathf.Min(target.values.Length,items.Length); i++)
        {
            items[i].gameObject.SetActive(true);
            items[i].CheckRegister();
            items[i].SetTarget(target.values[i],GetCurrentValue(i),inProgress == i);
        }
        
        int GetCurrentValue(int index)
        {
            int temp = CurrentDisplay;
            for (var i = 0; i < _target.values.Length; i++)
            {
                if (i == index)
                    return Mathf.Clamp(temp, 0, _target.values[i]);
                temp -= _target.values[i];
            }

            return 0;
        }
    }

//    public void SetCurrent(int index, int value)
//    {
//        current = value;
//        items[index].SetCurrent(value);
//        UpdateProgress();
//    }

    public override void LogCustomInt(int value)
    {
        base.LogCustomInt(value);
        Add(value);
    }

    // return amount add-able
    public int Add(int add)
    {
        var toAdd = Mathf.Min(_total - Current, add);
        Current += toAdd;
        var progresses = GetProgressInt().ToArray();
        var inProgress = CurrentInprogress();
        for (var i = 0; i < progresses.Length; i++)
        {
            items[i].SetCurrent(progresses[i]);
            items[i].SetInprogress(inProgress == i);
        }
        return toAdd;
    }

    private Vector2 _delay;
    public int DelayAdd(int add,Vector2 delay)
    {
        _delay = delay;
        if (!_inAdding)
            CurrentDisplay = Current;
        var toAdd = Mathf.Min(_total - Current, add);
        _renewAdd = true;
        Current += toAdd;
        if (!_inAdding)
            StartCoroutine(IeDelayAdd());
        
        return toAdd;
    }

    private bool _inAdding;
    private bool _renewAdd;
    private IEnumerator IeDelayAdd()
    {
        _inAdding = true;
        yield return new WaitForSeconds(_delay.x);
        float t = 0;
        int startValue = CurrentDisplay;
        while (CurrentDisplay < Current)
        {
            if (_renewAdd)
            {
                _renewAdd = false;
                 t = 0;
                 startValue = CurrentDisplay;
            }

            t += Time.deltaTime;
            CurrentDisplay = (int)Mathf.Lerp(startValue, Current, t / _delay.y);
            var progresses = GetProgressInt().ToArray();
            var inProgress = CurrentInprogress();
            for (var i = 0; i < progresses.Length; i++)
            {
                items[i].SetCurrent(progresses[i]);
                items[i].SetInprogress(inProgress == i);
            }
           
            yield return null;
        }
        CurrentDisplay = Current;
        _inAdding = false;
    }
    

//    private IEnumerable<float> GetProgress()
//    {
//        int temp = Current;
//        for (var i = 0; i < _target.values.Length; i++)
//        {
//            if (temp >= _target.values[i])
//            {
//                yield return 1;
//                temp -= _target.values[i];
//            }
//            else
//            {
//                yield return temp * 1f / _target.values[i];
//            }
//        }
//    }

    private IEnumerable<int> GetProgressInt()
    {
        int temp = CurrentDisplay;
        for (var i = 0; i < _target.values.Length; i++)
        {
            if (temp >= _target.values[i])
            {
                yield return _target.values[i];
                temp -= _target.values[i];
            }
            else
            {
                yield return temp;
                temp -= temp;
            }
        }
    }
    
    private int CurrentInprogress()
    {
        int temp = CurrentDisplay;
        for (var i = 0; i < _target.values.Length; i++)
        {
            if (temp < _target.values[i])
                return i;
            temp -=  _target.values[i];
        }

        return -1;
    }

    private void UpdateProgress()
    {
        float progress = CurrentDisplay / Mathf.Max(_total, 1f);
        foreach (var progressScript in bindProgress)
            progressScript.OnChanged(progress);
    }

    public bool IsFinish()
    {
        return Current >= _total;
    }

    private LevelSelectState _state;
    public void SetState(LevelSelectState state)
    {
        _state = state;
        bindState.OnChanged((int)state);
    }

    public bool IsUnlocked()
    {
        return (_state & LevelSelectState.Unlocked) != LevelSelectState.None;
    }
}