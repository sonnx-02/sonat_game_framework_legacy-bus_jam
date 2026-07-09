using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;



public class PlayerDataText : IndexPropertySourceRetriever
{
    [SerializeField] private TweenDisplayText tweenText;
    [SerializeField] private bool tween;
 
    private void OnEnable()
    {
        if (tweenText != null)
        {
            tweenText.SetTo(Property.Value);
            Property.TakeUntilDisable(this)
                .Subscribe(data =>
                {
//                if (tweenText.TrueValue > data)
//                    tweenText.SetTo(data);
//                else 
                    if (tween)
                        tweenText.TweenTo(data);
                    else
                        tweenText.SetTo(data);
                });
        }
    }
}