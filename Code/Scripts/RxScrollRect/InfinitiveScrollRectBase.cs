using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public abstract class InfinitiveScrollRectBase<T> : BaseReactiveScrollRect<T> where T : RxScrollItemBase
{
    [SerializeField] protected Vector2 size = new Vector2(100, 100);
    [SerializeField] protected RectOffset padding;
    [SerializeField] protected float centeredAdjust;
    [SerializeField] protected float tweenSpeed = 1;
    [SerializeField] protected float delayTween = 1;
    [SerializeField] protected int itemInRow = 1;
    [SerializeField] protected int colRowCanDisplay = 1;

    [SerializeField] protected Vector2 indicateSize = new Vector2(100, 100);
    [SerializeField] protected int[] indicateSlots;
    [SerializeField] protected Vector2 indicateDiff = new Vector2(100, 100);
    private int _lastColRowIndex = -1;

    [MyButtonInt(nameof(TestSize), nameof(TweenTo))] [SerializeField]
    protected int testValueScroll;

    [MyButtonInt(nameof(IndicateIndex), nameof(Load))] [SerializeField]
    private int testValueScroll2;

    public void Load()
    {
        activeItems = GetComponentsInChildren<T>(true);
    }

    public void TestSize(int s)
    {
        SetSize(s);

#if UNITY_EDITOR
        var viewFirsts = content.GetComponentsInChildren<T>(true);
        activeItems = new T[nActive];
        viewFirsts = viewFirsts.ToList().Take(nActive).ToArray();
        if (activeItems.Length != viewFirsts.Length)
        {
            Debug.LogError("view firsts size is smaller than init item, fill view first to match init items count");
            return;
        }

        for (var i = 0; i < activeItems.Length; i++)
        {
            activeItems[i] = viewFirsts[i];
            activeItems[i].Setup(this);
            activeItems[i].SetIndex(i);
            onItemRent?.Invoke(activeItems[i]);
            activeItems[i].name = "item " + i;
            var rt = activeItems[i].GetComponent<RectTransform>();
            if (!isHorizontal)
            {
                if (reverse)
                {
                    rt.anchorMin = new Vector2(0.5f, 0);
                    rt.anchorMax = new Vector2(0.5f, 0);
                }
                else
                {
                    rt.anchorMin = new Vector2(0.5f, 1);
                    rt.anchorMax = new Vector2(0.5f, 1);
                }
            }
            else
            {
                rt.anchorMin = new Vector2(0, 0.5f);
                rt.anchorMax = new Vector2(0, 0.5f);
//                if (reverse)
//                {
//                    rt.anchorMin = new Vector2(0,0.5f);
//                    rt.anchorMax = new Vector2(0,0.5f);
//                }
//                else
//                {
//                    rt.anchorMin = new Vector2(1,.5f);
//                    rt.anchorMax = new Vector2(1,.5f);
//                }
            }
        }

        _lastColRowIndex = 0;
        IndicateIndex(_lastColRowIndex);
#endif
    }

    protected float TotalSize;
    protected int TotalColRow;
    protected int ColRowActive;

    [FormerlySerializedAs("remapPositions")] 
    [SerializeField] private Vector2Int[] exchangePositions;
    [SerializeField] private Vector2Int[] sumUpPositions;

    public override float SetSize(int total)
    {
        if (total == 0)
            Debug.LogError("inits item cannot be this small");
        total = nInit;
        testValueScroll = total;

        TotalColRow = Mathf.CeilToInt(total * 1f / itemInRow);
        ColRowActive = Mathf.CeilToInt(nActive * 1f / itemInRow);
        TotalSize = isHorizontal
            ? TotalColRow * size.x + padding.left + padding.right
            : TotalColRow * size.y + padding.bottom + padding.top;

        indicateDiff = indicateSize - size;
        for (var i = 0; i < indicateSlots.Length; i++)
            if (TotalColRow > indicateSlots[i])
                TotalSize += isHorizontal ? indicateDiff.x : indicateDiff.y;

        TotalSize += sumUpPositions.GetSumUpValue(total);

        var sd = content.sizeDelta;
        if (!isHorizontal)
            sd.y = TotalSize;
        else
            sd.x = TotalSize;

//        if (!isHorizontal)
//        {
//            if (reverse)
//            {
//                content.anchorMin = new Vector2(0.5f,0);
//                content.anchorMax = new Vector2(0.5f,0);
//            }
//            else
//            {
//                content.anchorMin = new Vector2(0.5f,1);
//                content.anchorMax = new Vector2(0.5f,1);
//            }
//        }
//        else
//        {
//            content.anchorMin = new Vector2(0,0.5f);
//            content.anchorMax = new Vector2(0,0.5f);
//        }

        content.sizeDelta = sd;
        return TotalSize;
    }

    public override void IndicateIndex(int index)
    {
        content.anchoredPosition = GetIndicatePos(index);
        WhenValueChanged(true);
    }

    private Vector2 GetIndicatePos(int index)
    {
        var colRow = Mathf.FloorToInt(index * 1f / itemInRow);
        if (colRow + colRowCanDisplay > TotalColRow)
            index -= (colRow + colRowCanDisplay - TotalColRow) * itemInRow;

        var pos = GetAnchorPosNormal(index);
        if (!isHorizontal)
        {
            pos.x = 0;
            pos.y -= reverse ? padding.bottom : padding.top;
        }
        else
        {
            pos.y = 0;
            pos.x -= reverse ? padding.left : padding.right;
        }
        return pos;
    }

    public virtual void TweenTo(int index)
    {
        var target = GetIndicatePos(index);
        var distance = isHorizontal
            ? Mathf.Abs(target.x - content.anchoredPosition.x)
            : Mathf.Abs(target.y - content.anchoredPosition.y);
        content.DOAnchorPos(target, distance / 1000 * tweenSpeed).SetDelay(delayTween);
#if UNITY_EDITOR
        if (!Application.isPlaying)
            content.anchoredPosition = target;
#endif
    }

    [SerializeField] private int adjustColRowIndex = 0;

    protected override void WhenValueChanged(bool force = false)
    {
        int startColRowIndex = 0;
        if (!isHorizontal)
        {
            var anchoredPosition = content.anchoredPosition;
            var pos = (reverse) ? anchoredPosition.y : -anchoredPosition.y;
            float padY = (reverse) ? padding.top : padding.bottom;

            for (int i = 0; i < TotalColRow; i++)
            {
                var iPos = padY + size.y / 2 + i * size.y;
                for (var slot = 0; slot < indicateSlots.Length; slot++)
                    if (i == indicateSlots[slot])
                        iPos += indicateDiff.y;


                if (pos > iPos)
                    startColRowIndex++;
                else
                    break;
            }
        }
        else
        {
            var anchoredPosition = content.anchoredPosition;
            var pos = (reverse) ? anchoredPosition.x : -anchoredPosition.x;
            float padX = (reverse) ? padding.right : padding.left;

            for (int i = 0; i < TotalColRow; i++)
            {
                var iPos = padX + size.x / 2 + i * size.x;
                for (var slot = 0; slot < indicateSlots.Length; slot++)
                    if (i == indicateSlots[slot])
                        iPos += indicateDiff.x;

                if (pos > iPos)
                    startColRowIndex++;
                else
                    break;
            }
        }

        startColRowIndex -= adjustColRowIndex;
        if (startColRowIndex < 0)
            startColRowIndex = 0;

        // if startColRowIndex is more than max posibile when focus index
        if (force && startColRowIndex >= TotalColRow - ColRowActive)
        {
            startColRowIndex = TotalColRow - ColRowActive;
        }

        if (_lastColRowIndex != startColRowIndex && startColRowIndex < TotalColRow - ColRowActive + 1)
        {
            _lastColRowIndex = startColRowIndex;
            for (int i = startColRowIndex; i < startColRowIndex + ColRowActive; i++)
            {
                for (int j = 0; j < itemInRow; j++)
                {
                    var index = i * itemInRow + j;
                    if (j < Total)
                    {
                        var from = startColRowIndex * itemInRow;
                        var to = (startColRowIndex + ColRowActive) * itemInRow;
                        T spawn = GetAnItem(index, from, to);
                    }
                }
            }
        }
    }


    
    public override Vector2 GetItemAnchorPos(int index)
    {
        var remap = exchangePositions.GetExchangeValue(index,false);
        if (reverse)
            return GetAnchorPosReverse(remap);
        return GetAnchorPosNormal(remap);
    }

    private Vector2 GetAnchorPosReverse(int index)
    {
        var colRow = Mathf.FloorToInt(index * 1f / itemInRow);
        var indexInColRow = index % itemInRow;
        Vector2 pos;
        if (!isHorizontal)
        {
            pos = new Vector2(DuongExtensions.GetPosFromCenter(indexInColRow, itemInRow, size.x),
                TotalSize - (colRow * size.y + padding.top));

            for (var slot = 0; slot < indicateSlots.Length; slot++)
                if (colRow >= indicateSlots[slot])
                {
                    pos.y -= indicateDiff.y;
                    if (colRow == indicateSlots[slot])
                        pos.y += indicateDiff.y / 2;
                }
                else
                    break;
            
            pos.y += sumUpPositions.GetSumUpValue(index);
        }
        else
        {
            pos = new Vector2(TotalSize - (colRow * size.x - padding.right),
                DuongExtensions.GetPosFromCenter(indexInColRow, itemInRow, size.y));

            for (var slot = 0; slot < indicateSlots.Length; slot++)
                if (colRow >= indicateSlots[slot])
                {
                    pos.x -= indicateDiff.x;
                    if (colRow == indicateSlots[slot])
                        pos.x += indicateDiff.x / 2;
                }
                else
                    break;
            
            pos.x += sumUpPositions.GetSumUpValue(index);
        }
        return pos;
    }

    private Vector2 GetAnchorPosNormal(int index)
    {
        var colRow = Mathf.FloorToInt(index * 1f / itemInRow);
        var indexInColRow = index % itemInRow;
        Vector2 pos;
        if (!isHorizontal)
        {
            pos = new Vector2(DuongExtensions.GetPosFromCenter(indexInColRow, itemInRow, size.x),
                (colRow * size.y + padding.bottom));

            for (var slot = 0; slot < indicateSlots.Length; slot++)
                if (colRow >= indicateSlots[slot])
                {
                    pos.y += indicateDiff.y;
                    if (colRow == indicateSlots[slot])
                        pos.y -= indicateDiff.y / 2;
                }
                else
                    break;
            
            pos.y += sumUpPositions.GetSumUpValue(index);
        }
        else
        {
            pos = new Vector2((colRow * size.x + padding.right),
                DuongExtensions.GetPosFromCenter(indexInColRow, itemInRow, size.y));
            for (var slot = 0; slot < indicateSlots.Length; slot++)
                if (colRow >= indicateSlots[slot])
                {
                    pos.x += indicateDiff.x;
                    if (colRow == indicateSlots[slot])
                        pos.x -= indicateDiff.x / 2;
                }
                else
                    break;

            pos.x += sumUpPositions.GetSumUpValue(index);
        }
        
        return pos;
    }
}