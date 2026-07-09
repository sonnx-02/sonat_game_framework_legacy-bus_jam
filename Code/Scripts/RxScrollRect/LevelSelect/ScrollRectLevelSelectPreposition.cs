using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


[RequireComponent(typeof(ScrollRect))]
public class ScrollRectLevelSelectPreposition : BaseReactiveScrollRect<LevelSelect>
{
    [SerializeField] private RxScrollItemIndicate indicate;
    [SerializeField] private Vector2 padding = new Vector2(100, 100);
    [SerializeField] private Vector2[] positions;
    [SerializeField] private float tweenSpeed = 1;
    [SerializeField] private float delayTween = 1;


    [SerializeField] protected int testLoad;

    [MyButton(nameof(TestLoadPositions))] [SerializeField]
    protected int testProduct123;

    public void TestLoadPositions()
    {
        var selects = content.GetComponentsInChildren<LevelSelect>(true);
        // ReSharper disable once PossibleNullReferenceException
        positions = selects.Select(x => (x.transform as RectTransform).anchoredPosition).ToArray();
    }

    [MyButtonInt(nameof(IndicateIndex))] [SerializeField]
    private int testValueScroll2;

    public override float SetSize(int size)
    {
        var sd = content.sizeDelta;
        if (!isHorizontal)
            sd.y = padding.x + padding.y + positions[positions.Length - 1].y;
        else
            sd.x = padding.x + padding.y + positions[positions.Length - 1].x;

        content.sizeDelta = sd;
        if (!isHorizontal)
            return padding.x + padding.y + positions[positions.Length - 1].y;
        else
           return padding.x + padding.y + positions[positions.Length - 1].x;
    }

    public override void IndicateIndex(int index)
    {
        content.anchoredPosition = GetIndicatePos(index);
        WhenValueChanged(true);
    }

    public override Vector2 GetItemAnchorPos(int index)
    {
        if (!reverse) // neu xuoi chieu
        {
            if (!isHorizontal) // neu la chieu doc
                return new Vector2(0, padding.x) + positions[index];
            return new Vector2(padding.x, 0) + positions[index];
        }

        if (!isHorizontal) // neu la chieu doc
            return new Vector2(0, padding.y) + positions[index];
        return new Vector2(padding.y, 0) + positions[index];
    }

    private Vector2 GetIndicatePos(int index)
    {
        return GetItemAnchorPos(index);
    }

    public void TweenTo(int index)
    {
        var target = GetIndicatePos(index);
        var distance = isHorizontal
            ? Mathf.Abs(target.x - content.anchoredPosition.x)
            : Mathf.Abs(target.y - content.anchoredPosition.y);
        content.DOAnchorPos(target, distance / 1000 * tweenSpeed).SetDelay(delayTween);
    }

    private int _lastColRowIndex = -1;

    protected override void WhenValueChanged(bool force = false)
    {
        int startColRowIndex = 0;
        var posY = -content.anchoredPosition.y;
        for (var i = 0; i < positions.Length; i++)
        {
            if (posY > positions[i].y)
                startColRowIndex = i;
            else
                break;
        }

        if (_lastColRowIndex != startColRowIndex && startColRowIndex < positions.Length - nActive + 1)
        {
            _lastColRowIndex = startColRowIndex;
            for (int i = startColRowIndex; i < startColRowIndex + nActive; i++)
            {
                LevelSelect spawn = GetAnItem(i, startColRowIndex, startColRowIndex + nActive);
            }
        }
    }
}