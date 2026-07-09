using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;

public class PopMessage : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private BaseRxAnimator[] anims;
    [SerializeField] private float timeDisable = 2;

    private IDisposable _checkDisable;
    public PopMessageParent PopMessageParent { get; set; }

    public void Show(string message)
    {
        PopMessageParent.gameObject.SetActive(true);
        gameObject.SetActive(true);
        text.text = message;
        foreach (var baseRxAnimator in anims)
            baseRxAnimator.Animator_InThenOut();

        _checkDisable?.Dispose();
        _checkDisable = Observable.Timer(TimeSpan.FromSeconds(timeDisable))
            .Subscribe(data =>
            {
                gameObject.SetActive(false);
                PopMessageParent.CheckDeactive();
            });
    }
}