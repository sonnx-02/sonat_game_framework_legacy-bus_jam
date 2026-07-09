using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DialogAddCurrency : DialogAddCurrencyBase
{
    [SerializeField] private BaseActionProductScript[] productBindings;
    [SerializeField] private TweenDisplayText tweenText;
    [SerializeField] private BaseBaseActionScript[] onStarts;
    [SerializeField] private BaseRxAnimator[] inAnims;
    [SerializeField] private BaseRxAnimator[] inThenOutAnims;
    [SerializeField] private BaseRxAnimator[] outAnims;

    [SerializeField] private float delayToCountDown = .5f;
    [SerializeField] private float delayToGemAnim = .5f;
    [SerializeField] private float delayToAddCoinText = .3f;
    [SerializeField] private float delayToClose = .25f;
    [SerializeField] private float delayAnimOut = .25f;
    [SerializeField] private TweenGemController gemAnimation;
    [SerializeField] private BaseActionScript[] gemAnims;
    [SerializeField] private float minCountDown = .5f;
    [IndexAsEnum(BuiltInEnumType.Quantity)]
    [SerializeField] private IntTransform[] targetByQuantity;
    private Product _product;

    public override void Show(Product product)
    {
        base.Show(product);
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

        if(tweenText != null)
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
                    gemAnimation.gameObject.SetActive(false);
            });
        gemAnims.StartAction();

        delayAnimOut.Timer(() =>
        {
            foreach (var baseRxAnimator in outAnims)
                baseRxAnimator.Animator_Out();
        });

        delayToCountDown.Timer(() =>
        {
            var timeCount = 0f;
            if(tweenText != null)
                 timeCount = tweenText.Handler(new TweenCommand()
                {
                    TweenType = TextTweenType.TweenTo,
                    Value = 0
                });

            if (timeCount < minCountDown)
                timeCount = minCountDown;

            Observable.Timer(TimeSpan.FromSeconds(timeCount + delayToClose))
                .Subscribe(data2 =>
                {
                    Close();
                    PlayerData.Save();
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