using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressIntArrayScript : ProgressScriptInt
{
    [SerializeField] private bool[] claimed;
    [SerializeField] private TMP_Text txtProgress;
    [SerializeField] private TMP_Text txtFragmentProgress;
    [SerializeField] private TMP_Text txtProgressByCurrentTarget;

    [SerializeField] private ProgressScript[] fragmentProgresses;
    
    private int[] _targets;

    public override int Target
    {
        get
        {
            if (_targets != null && _targets.Length > 0)
                return _targets[_targets.Length - 1];
            return -1;
        }
    }

    public override void SetTarget(int value)
    {
        base.SetTarget(value);
        SetTargets(new [] {value});
    }

    public virtual void SetTargets(int[] value)
    {
        _targets = value;
        claimed = new bool[_targets.Length];
    }
    
    public virtual void SetClaimed(bool[] value)
    {
        claimed = value;
    }
    
    public int CurrentIndex { get; private set; }
    protected override void Trigger(int val)
    {
        base.Trigger(val);
        if (Target > 0)
        {
            if(txtProgress != null)
                txtProgress.text = $"{val}/{Target}";

            CurrentIndex = GetIndexOfTarget(val);
            var fragVal = CurrentIndex== 0 ? val : val - _targets[CurrentIndex - 1];
            var fragTarget =  CurrentIndex== 0 ? _targets[CurrentIndex]  : _targets[CurrentIndex] - _targets[CurrentIndex - 1];
            if (txtFragmentProgress != null)
                txtFragmentProgress.text = $"{fragVal}/{fragTarget}";
            if (txtProgressByCurrentTarget != null)
                txtProgressByCurrentTarget.text = $"{val}/{_targets[CurrentIndex] }";
            
            var progressFloat = fragVal * 1f / fragTarget;
            foreach (var progressScript in fragmentProgresses)
                progressScript.OnChanged(progressFloat);
        }
    }

    private int GetIndexOfTarget(int value)
    {
        for (int i = 0; i < _targets.Length; i++)
        {
            if (value < _targets[i])
                return i;
            if (value >= _targets[i] && !claimed[i])
            {
                return i;
            }
        }

        return _targets.Length-1;
    }
}
