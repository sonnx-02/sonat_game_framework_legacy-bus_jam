
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public interface IReactiveScrollRect
{
    Vector2 GetItemAnchorPos(int value);
}

[RequireComponent(typeof(ScrollRect))]
public abstract class BaseReactiveScrollRect <T> : BaseFrameworkView,IReactiveScrollRect where T : RxScrollItemBase
{
    [SerializeField] protected bool isHorizontal;
    [SerializeField] protected bool reverse;
    [SerializeField] protected int nInit = 50;
    [SerializeField] protected int nActive = 5;
    [SerializeField] protected PoolCollection<T> items;
    

    [DisplayAsDisable] [SerializeField] public T[] activeItems;
    private UILineBetweenScript[] _activeLines;
    public abstract Vector2 GetItemAnchorPos(int index);
    
    [SerializeField] protected RectTransform content;
    [SerializeField] protected RectTransform lineParent;
    [SerializeField] protected UILineBetweenScript linePrefab;


    [SerializeField] protected Vector2Int[] rules;
    // default is always zero
    private readonly IntegerSerializableDictionary _dictRules = new IntegerSerializableDictionary(0,0);

    public void OverrideRules(Vector2Int[] others)
    {
        rules = others;
        LoadRules();
    }

    private void LoadRules()
    {
        _dictRules.ClearDict();
        foreach (var vector2Int in rules)
            _dictRules[vector2Int.x] = vector2Int.y;
    }

    [SerializeField] protected bool autoInit = true;
    protected override void Register()
    {
        base.Register();
        if (autoInit)
        {
            LoadRules();
            items.Init();
            items.ViewFirst(0); // other item please do not pre-placed in container
            InitItems(nInit);
        }
    }

    public void Populate(int total,Action<T,int> onRent, Vector2Int[] newRules = null)  
    {
        if(newRules != null)
            OverrideRules(newRules);
        LoadRules();
        items.Init();
        items.ViewFirst(0); // other item please do not pre-placed in container
        InitItems(total);
        for (var i = 0; i < total; i++)
            onRent.Invoke(activeItems[i],activeItems[i].Index);
    }

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
    }

    protected int Total;
    public Action<T> onItemClick;
    public Action<T> onItemRent;

    protected virtual void InitItems(int size)
    {
        Total = size;
        content.pivot = !isHorizontal ? new Vector2(.5f, reverse ? 1 : 0) : new Vector2(reverse ? 1 : 0, .5f);
        SetSize(size);
        items.ReturnAll();
        activeItems = new T[nActive];
    
        for (var i = 0; i < activeItems.Length; i++)
        {
            T item = items.pools[_dictRules[i]].Rent(content, Vector3.zero);
            item.SetPoolIndex(_dictRules[i]);
            activeItems[i] = item;
            // update pos is on this, true is first time
            if (item.Setup(this))
                foreach (var button in activeItems[i].Buttons)
                    button.onClick.AddListener(() =>
                    {
                        if(onItemClick != null)
                            onItemClick(item);
                        else
                        {
                            Debug.LogError("onItemClick not set");
                        }
                    });
            item.SetIndex(i);
            onItemRent?.Invoke(item);
        }

        if (linePrefab != null)
        {
            var viewFirsts2 = content.GetComponentsInChildren<UILineBetweenScript>(true);
            _activeLines = new UILineBetweenScript[nActive];
            for (var i = 0; i < activeItems.Length; i++)
            {
                _activeLines[i] = i < viewFirsts2.Length ? viewFirsts2[i] : Instantiate(linePrefab, lineParent);
            }
        }
        GetComponent<ScrollRect>().onValueChanged.AddListener(t => WhenValueChanged());
    }

    public abstract float SetSize(int size);
    
    public void InvokeItemRentAll()
    {
        for (var i = 0; i < activeItems.Length; i++)
        {
            var rxScrollItem = activeItems[i];
            WhenItemRent(rxScrollItem,i);
        }
    }

    public abstract void IndicateIndex(int index);

    public void InvokeOnChangedAll()
    {
        CheckRegister();
        foreach (var rxScrollItem in activeItems)
            rxScrollItem.OnValueChanged();
    }

    public virtual void WhenItemRent(T item,int indexInList)
    {
        onItemRent?.Invoke(item);
        if (linePrefab != null)
        {
            _activeLines[indexInList].gameObject.SetActive(item.Index > 0);
            if (item.Index  > 0)
                _activeLines[indexInList].Set(GetPosOfPreviousItem(item),item.RectTransform.anchoredPosition);
        }
    }

    public virtual Vector2 GetPosOfPreviousItem(T item)
    {
        for (var i = 0; i < activeItems.Length; i++)
            if (activeItems[i].Index == item.Index -1)
                return activeItems[i].RectTransform.anchoredPosition;

        return item.RectTransform.anchoredPosition;
    }

    /// <summary>
    /// from to is clamp
    /// </summary>
    /// <param name="index"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    protected virtual T GetAnItem(int index, int from, int to)
    {
        
//        // update pos is on this, true is first time
//        if (item.Setup(this))
//        {
//            foreach (var button in activeItems[i].Buttons)
//                button.onClick.AddListener(() => onItemClick(item));
//            onItemRent?.Invoke(item);
//        }

        for (var i = 0; i < activeItems.Length; i++)
            if (activeItems[i].Index == index)
            {
                activeItems[i].name = "item " + index;
                var rent = activeItems[i];
                WhenItemRent(rent,i);
                return rent;
            }
        
        // old work
        for (var i = 0; i < activeItems.Length; i++)
            if (activeItems[i].Index < from || activeItems[i].Index >= to)
            {
                if (activeItems[i].PoolIndex != _dictRules[index])
                {
                    activeItems[i].Return();
                    
                    T item = items.pools[_dictRules[index]].Rent(content, Vector3.zero);
                    item.SetPoolIndex(_dictRules[index]);
                    if (item.Setup(this))
                        foreach (var button in activeItems[i].Buttons)
                            button.onClick.AddListener(() => onItemClick(item));
                    activeItems[i] = item;
                }
                    
                activeItems[i].SetIndex(index);
                onItemRent?.Invoke(activeItems[i]);
                activeItems[i].name = "item " + index;
                WhenItemRent(activeItems[i],i);
                return activeItems[i];
            }
        return null;
    }

    protected abstract void WhenValueChanged(bool force = false);
}
