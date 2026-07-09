using UnityEngine;

public interface IActiveTutorialComponents
{
    void CheckActiveOnTutorialStep();
    void StopTut();
}


public abstract class BaseGameTutorial<T1> : BaseFrameworkView, IActiveTutorialComponents where T1 : BaseGameController
{
    [SerializeField] protected ActiveOnTutStep[] activeOnTuts;

    protected abstract BaseTutorialStep CurrentBaseStep { get; }

    public abstract void StopTut();

    protected T1 GameController;

    public void Set(T1 gc)
    {
        GameController = gc;
    }

    public virtual bool InitTut(bool isNextTut)
    {
        if (PlayerData.tutorialStep == -1)
            foreach (var activeOnTutStep in activeOnTuts)
                activeOnTutStep.SetActive(-1, -1, 0);

        return false;
    }

    public void CheckActiveOnTutorialStep()
    {
        foreach (var activeOnTutStep in activeOnTuts)
            activeOnTutStep.SetActive(PlayerData.tutorialStep,
                CurrentBaseStep != null ? CurrentBaseStep.actionEnum : -1, 
                CurrentBaseStep != null ?CurrentBaseStep.tutEnables : 0);
    }
}