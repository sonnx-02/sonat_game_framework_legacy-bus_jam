using DG.Tweening;

public class ActionScriptStopAfterAds : BaseBaseActionScript
{
    public RxTweenScript tweenScript;

    public override void StartAction()
    {
        base.StartAction();

        if (tweenScript != null)
        {
            tweenScript.DisposeTween();
        }
    }
}
