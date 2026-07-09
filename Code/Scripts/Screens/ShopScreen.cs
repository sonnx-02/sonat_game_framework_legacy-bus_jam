using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.Serialization;

public class ShopScreen : BaseScreen
{
    [SerializeField] private TweenDisplayScript tweenCoin;
    [SerializeField] private TweenDisplayScript tweenGem;
    [SerializeField] private TweenDisplayScript tweenScore;

    [SerializeField] private ToggleScript onReady;
    [SerializeField] private ToggleScript onPlaying;
    [SerializeField] private IndexBindingScriptBase[] startGameLevelBindings; // update only when start game not always correspond with current level
//    public IndexBindingScript[] levelBindings; // always correspond with current level

    protected override void Register()
    {
        base.Register();
        if (tweenCoin != null)
            CoinTween.Subscribe(data => tweenCoin.Handler(data));
        if (tweenGem != null)
            GemTween.Subscribe(data => tweenGem.Handler(data));
        if (tweenScore != null)
            ScoreTween.Subscribe(data => tweenScore.Handler(data));
    }
}
