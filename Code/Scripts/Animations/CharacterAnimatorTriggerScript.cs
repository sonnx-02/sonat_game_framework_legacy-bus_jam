using UnityEngine;

public class CharacterAnimatorTriggerScript : BaseBaseActionScript
{
    [SerializeField] protected Animator animator;
    [SerializeField] private string[] triggerNames;

    public void TriggerAnim(string trigger)
    {
        foreach (var triggerName in triggerNames)
            animator.ResetTrigger(triggerName);
        animator.SetTrigger(trigger);
    }

    public void ResetTrigger(string triggerName)
    {
        animator.ResetTrigger(triggerName);
    }

    public void ResetAllTrigger()
    {
        foreach (var triggerName in triggerNames)
            animator.ResetTrigger(triggerName);
    }
    public override void StartAction(int index)
    {
        base.StartAction(index);
        foreach (var triggerName in triggerNames)
            animator.ResetTrigger(triggerName);
        animator.SetTrigger(triggerNames[index]);
    }
  
#if UNITY_EDITOR
    private void Reset()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }
    #endif
}
