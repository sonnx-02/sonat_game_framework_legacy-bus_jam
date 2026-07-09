using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;


[Serializable]
public class MyAnimatorTrigger
{
    [SerializeField] [IndexAsEnum(BuiltInEnumType.AnimationTriggerEnum)]
    public int trigger;

    [SerializeField] private string[] triggerNames = {"idle"};

    public int TriggerEnum => trigger;
    public int Count => triggerNames.Length;

    public float randomTime = 1f;

    public string GetTriggerName(int i) => triggerNames[i];

    public void SetTriggerNames(string[] n)
    {
        triggerNames = n;
    }
}

[Serializable]
public class MyAnimatorSequenceAction
{
    [SerializeField] [IndexAsEnum(BuiltInEnumType.AnimationTriggerEnum)]
    public int trigger; // trigger name
    public Point2Float randomDelayStart;
    public IntFloat[] randomIndex = {new IntFloat(0,1)}; // index and finish time
    public ArrayInt[] randomNextAction = {new ArrayInt(new int[0]),};
}

[Serializable]
public class MyAnimatorSequence
{
    public int startActionIndex; // the action will start from
    [ArrayElementIndexTitleAttribute(nameof(MyAnimatorSequenceAction.trigger), BuiltInEnumType.AnimationTriggerEnum)]
    public MyAnimatorSequenceAction[] actions;
    
    public bool ValidateNextAction()
    {
        foreach (var action in actions)
        {
            for (var index = 0; index < action.randomNextAction.Length; index++)
                foreach (var arrayIntValue in action.randomNextAction[index].values)
                    if (arrayIntValue >= actions.Length)
                        return false;
            
        }
        return true;
    }
}


[Serializable]
public class CharacterAnimatorController
{
    // single animation
    [MyButtonInt(new[] {nameof(TestTriggerAnimator)}, BuiltInEnumType.AnimationTriggerEnum)]
    public IntInt testAnims;
    // sequence
    [MyButtonInt(new[] {nameof(StartSequences)}, BuiltInEnumType.AnimationSequenceEnum)]
    public int testAnims2;
    
    [ArrayElementIndexTitleAttribute(nameof(MyAnimatorTrigger.trigger), BuiltInEnumType.AnimationTriggerEnum)]
    [SerializeField]
    private MyAnimatorTrigger[] triggers;

  

    [ArrayElementIndexTitleAttribute2(BuiltInEnumType.AnimationSequenceEnum)] [SerializeField]
    private MyAnimatorSequence[] sequences;


    private Vector2Int _lastAnim = new Vector2Int(-1, -1);
    public Vector2Int LastAnim => _lastAnim;

    public void ResetAllTrigger()
    {
        _lastAnim = new Vector2Int(-1, -1);
        if(_inject != null)
            _inject.ResetAllAnimatorTriggers();
    }

    private Animator _inject;

    public void Init(Animator animator)
    {
        _inject = animator;
#if UNITY_EDITOR
        for (var i = 0; i < sequences.Length; i++)
           if(!sequences[i].ValidateNextAction())
               Debug.Log($"Error in Sequence{i}/NextAction, value >= actions.Length");
#endif
    }

    [MyButtonInt(new[] {nameof(StartSequences)})]
    private IDisposable _sequenceDisposable;

    public void StartSequences(int sequence)
    {
        _lastAnim.x = -1;
        _lastAnim.y = -1;

        _sequenceDisposable?.Dispose();
        _sequenceDisposable = Observable.FromCoroutine(() => IeSequence(sequences[sequence]))
            .TakeUntilDisable(_inject).Subscribe();
    }

    private IEnumerator IeSequence(MyAnimatorSequence sequence)
    {
        var currentIndex = sequence.startActionIndex;

        var currentAction = sequence.actions[currentIndex];
        while (true)
        {
            var delayStart = currentAction.randomDelayStart.GetRandom();
            if (delayStart > 0)
                yield return delayStart.ToWaitForSeconds();

            var indexOfRandom = UnityEngine.Random.Range(0, currentAction.randomIndex.Length);
            _inject.SetTrigger(GetTrigger(currentAction.trigger)
                .GetTriggerName(currentAction.randomIndex[indexOfRandom].value));


            if (currentAction.randomIndex[indexOfRandom].value2 > 0)
                yield return currentAction.randomIndex[indexOfRandom].value2.ToWaitForSeconds();

            if (currentAction.randomNextAction[indexOfRandom].values.Length == 0)
                break; // stop sequence when no next action
            currentIndex = currentAction.randomNextAction[indexOfRandom].values.TakeRandom();
            currentAction = sequence.actions[currentIndex];
        }
    }


    private void TestTriggerAnimator(int anim, int index)
    {
        TriggerAnimator(anim, index);
    }
    

    private void TriggerAnimator(int triggerEnum, int index, bool force = true)
    {
        if (force || _lastAnim.x != triggerEnum || _lastAnim.y != index)
        {
            if(_inject == null)
                Debug.LogError("animator null");
            var getTrigger = GetTrigger(triggerEnum);
            if (getTrigger == null)
            {
                Debug.LogError("not found trigger "+triggerEnum);
                return;
            }
            
            _inject.SetTrigger(getTrigger.GetTriggerName(index));
            _lastAnim.x = triggerEnum;
            _lastAnim.y = index;
        }
    }
 
   

    public void TriggerAnimatorRandom(AnimSequenceAction triggerEnum, bool force = true)
    {
        _sequenceDisposable?.Dispose();
        _sequenceDisposable = Observable.FromCoroutine(() => IeTriggerAnimatorRandom(triggerEnum, force))
            .TakeUntilDisable(_inject).Subscribe();
    }

    private IEnumerator IeTriggerAnimatorRandom(AnimSequenceAction triggerEnum, bool force = true)
    {
        TriggerAnimatorRandom(triggerEnum.action, 0, force);
        for (var i = 0; i < triggerEnum.NextActions.Length; i++)
        {
            yield return new WaitForSeconds(triggerEnum.NextActions[i].value2);
            TriggerAnimatorRandom(triggerEnum.NextActions[i].value, 0, force);
        }
    }
    
    public void TriggerAnimator(int triggerEnum, bool force = true)
    {
        _sequenceDisposable?.Dispose();
        TriggerAnimator(triggerEnum, 0, force);
    }

//    public void TriggerAnimatorByIndex(int triggerIndex, bool force = true)
//    {
//        _sequenceDisposable?.Dispose();
//        TriggerAnimator(triggerEnum, 0, force);
//    }

    public void TriggerAnimatorRandom(int triggerEnum, int from = 0, bool force = true)
    {
        var trigger = GetTrigger(triggerEnum);
        TriggerAnimator(triggerEnum, UnityEngine.Random.Range(0, trigger.Count), force);
    }

//        public void TriggerAnimatorRandomLoop(int triggerEnum,MonoBehaviour script,int from = 1)
//        {
//            _randomDisposable?.Dispose();
//            TriggerAnimator(triggerEnum, 0);
//            var trigger = GetTrigger(triggerEnum);
//            var count = trigger.Count;
//            if (count > 1)
//            {
//                _randomDisposable = Observable.Interval(TimeSpan.FromSeconds(trigger.randomTime))
//                    .TakeUntilDisable(script)
//                    .Subscribe(data =>
//                    {
//                        int random = UnityEngine.Random.Range(from, count);
//                        _inject.SetTrigger(trigger.GetTriggerName(random));
//                    });
//            }
//        }
//        

    public void ResetTrigger(int triggerEnum)
    {
        _inject.ResetTrigger(GetTrigger(triggerEnum).GetTriggerName(triggerEnum));
    }

    private MyAnimatorTrigger GetTrigger(int index)
    {
        foreach (var myAnimatorTrigger in triggers)
            if (myAnimatorTrigger.TriggerEnum == index)
                return myAnimatorTrigger;

        return null;
    }
}

[System.Serializable]
public class AnimSequenceAction
{
    [IndexAsEnum(BuiltInEnumType.AnimationTriggerEnum)]
    public int action;

    [SerializeField] [IndexAsEnum(BuiltInEnumType.AnimationTriggerEnum)]
    private IntFloat[] nextActions;

    public IntFloat[] NextActions => nextActions;
}