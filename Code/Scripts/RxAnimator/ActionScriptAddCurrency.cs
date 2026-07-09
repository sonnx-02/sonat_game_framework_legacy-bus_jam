using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ActionScriptAddCurrency : BaseActionProductScript
{
    [SerializeField] private GameObject container;
    [SerializeField] private BaseActionProductScript[] productBindings;
    [SerializeField] private TweenDisplayText tweenText;
    [SerializeField] private BaseBaseActionScript[] onStarts;
    [SerializeField] private BaseBaseActionScript[] afterFinish;
    [SerializeField] private BaseRxAnimator[] inAnims;
    [SerializeField] private BaseRxAnimator[] inThenOutAnims;
    [SerializeField] private BaseRxAnimator[] outAnims;
    [SerializeField] private float delayToCountDown = .5f;
    [SerializeField] private float delayToGemAnim = .5f;
    [SerializeField] private float delayToAddCoinText = .3f;
    [SerializeField] private float delayToClose = .25f;
    [SerializeField] private float delayAnimOut = .25f;
    [SerializeField] private TweenGemController gemAnimation;
    [SerializeField] private float minCountDown = .5f;
    [IndexAsEnum(BuiltInEnumType.Quantity)]
    [SerializeField] private IntTransform[] targetByQuantity;
    [SerializeField] private bool clearAfter;

    private Product _product;

//    protected override void DoAction()
//    {
//        base.DoAction();
//        StartAction(Product);
//    }

    public override void StartAction(Product product)
    {
        if (product.amount <= 0)
        {
            if(container != null)
                container.SetActive(false);
            Debug.LogError("amount is zero");
        }
        if(container != null)
            container.SetActive(true);
        
        IsBusy = true;
        onStarts.StartAction();
        SubjectController.CustomEvent.OnNext((int) CustomEvent.DialogAddGemOpen);
        _product = product;
        //PlayerData.Claim(_product.ReverseQuantity(), false);
        foreach (var productBinding in productBindings)
            productBinding.StartAction(product);
        foreach (var baseRxAnimator in inAnims)
            baseRxAnimator.Animator_In();
        foreach (var anim in inThenOutAnims)
            anim.IO_AnimatorInThenOut();

        tweenText.Handler(new TweenCommand()
        {
            TweenType = TextTweenType.Set,
            Value = product.amount
        });

        
        var target =
            ScreenRoot.GetLocation(product.quantity == Quantity.Diamond ? LocationEnum.Gem : LocationEnum.Gold);
        for (var i = 0; i < targetByQuantity.Length; i++)
        {

            if (targetByQuantity[i].value == (int) product.quantity)
            {
                target = targetByQuantity[i].value2 as RectTransform;
                break;
            }
        }

        if(gemAnimation != null)
            delayToGemAnim.Timer(() =>
            {
                if (target != null)
                {
                    gemAnimation.gameObject.SetActive(true);
                    gemAnimation.SetTarget(target);
                    gemAnimation.StartTween();
                }
                else
                {
                    gemAnimation.gameObject.SetActive(false);
                }
            });

        delayAnimOut.Timer(() =>
        {
            foreach (var baseRxAnimator in outAnims)
                baseRxAnimator.Animator_Out();
        });

        delayToCountDown.Timer(() =>
        {
            var timeCount = tweenText.Handler(new TweenCommand()
            {
                TweenType = TextTweenType.TweenTo,
                Value = 0
            });

            if (timeCount < minCountDown)
                timeCount = minCountDown;

            Observable.Timer(TimeSpan.FromSeconds(timeCount + delayToClose))
                .Subscribe(data2 =>
                {
                    IsBusy = false;
                    PlayerData.Save();
                    if(clearAfter && input != null)
                        input.Clear();    
                    afterFinish.StartAction();
                });
        });

        delayToAddCoinText.Timer(() =>
        {
            if (target == null)
                PlayerData.Claim(_product);
            else
                delayToAddCoinText.Timer(() => PlayerData.Claim(_product));
        });
    }
}
