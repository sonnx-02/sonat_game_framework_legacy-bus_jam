using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UiBackgroundPaging : BaseFrameworkView
{
    [SerializeField] private Transform container;
    [SerializeField] private Image[] images;


    public void Set(int current)
    {
        _current = current;
        container.localPosition = images[_current].transform.localPosition.FlipX();
    }
    
    public void Move(int current,float timeMove,Ease ease)
    {    
        _current = current;
        container.DOKill();
        container.DOLocalMoveX(-images[_current].transform.localPosition.x, timeMove).SetEase(ease);
    }

    private int _current;
    public void Previous()
    {
        var next = Mathf.Clamp(_current - 1, 0, images.Length - 1);
        if (next != _current)
        {
//            _current = 
        }
    }
    public void Next()
    {
        var next = Mathf.Clamp(_current + 1, 0, images.Length - 1);
        if (next != _current)
        {
            
        }
    }
}
