using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public interface IPool
{
    void Return(PoolItem item);
    void Destroy(PoolItem item);
}



[System.Serializable]
public class Pool<T>  : IPool where T : PoolItem
{
    public T this[int index] => ActiveItems[index];
    public int Count => ActiveItems.Count;
    
    private readonly List<T> _list = new List<T>();
    public readonly List<T> ActiveItems = new List<T>();
    private readonly T _prefab;

    public void Add(IEnumerable<T> add)
    {
        _list.AddRange(add);
    }

    private  Transform _parent;
    private readonly float _checkTime;

    public T Prefab => _prefab;

    public Pool(T prefab, Transform parent,float checkTime = 1f)
    {
        if(prefab == null)
            Debug.LogError("prefab cannot be null");
        _prefab = prefab;
        _parent = parent;
        _checkTime = checkTime;
    }

    public void SetParent(Transform parent)
    {
        _parent = parent;
    }

    public T Rent(Action<T> beforeRent = null)
    {
        T rent = FindOne();
        if(beforeRent != null)
            beforeRent.Invoke(rent);
        rent.gameObject.SetActive(true);
        rent.pool = this;
        rent.transform.SetParent(_parent, false);
        ActiveItems.Add(rent);
        rent.OnRent();
        return rent;
    }
    
  
    public T RentLocalZero(Action<T> beforeRent = null)
    {
        T rent = FindOne();
        if(beforeRent != null)
            beforeRent.Invoke(rent);
        rent.gameObject.SetActive(true);
        rent.pool = this;
        Transform transform;
        (transform = rent.transform).SetParent(_parent, false);
        transform.localPosition = Vector3.zero;
        ActiveItems.Add(rent);
        rent.OnRent();
        return rent;
    }

    public T Rent(Transform parent,Vector3 localPosition,Action<T> beforeRent = null)
    {
        T rent = FindOne();
        if (beforeRent != null)
        {
//            rent.gameObject.SetActive(false);
            beforeRent.Invoke(rent);
        }
        rent.gameObject.SetActive(true);
        rent.pool = this;
        Transform transform;
        (transform = rent.transform).SetParent(parent, false);
        transform.localPosition = rent.overridePosition ? rent.overridePositionAdjust : localPosition;
        transform.rotation = Quaternion.identity;
        ActiveItems.Add(rent);
        rent.OnRent();
        return rent;
    }
    
    public T Rent2(Transform parent,Vector3 localPosition,Action<T> beforeRent = null)
    {
        T rent = FindOne();
        if (beforeRent != null)
        {
//            rent.gameObject.SetActive(false);
            beforeRent.Invoke(rent);
        }
        rent.pool = this;
        Transform transform;
        (transform = rent.transform).SetParent(parent, false);
        transform.localPosition = rent.overridePosition ? rent.overridePositionAdjust : localPosition;
        transform.rotation = Quaternion.identity;
        rent.gameObject.SetActive(true);
        ActiveItems.Add(rent);
        rent.OnRent();
        return rent;
    }
    
    public T RentWorld(Transform parent,Vector3 position,Action<T> beforeRent = null)
    {
        T rent = FindOne();
        if(beforeRent != null)
            beforeRent.Invoke(rent);
        rent.gameObject.SetActive(true);
        rent.pool = this;
        Transform transform;
        (transform = rent.transform).SetParent(parent, false);
        transform.transform.position = rent.overridePosition ? rent.overridePositionAdjust : position;
        ActiveItems.Add(rent);
        rent.OnRent();
        return rent;
    }
    
    public T RentWorld(Transform parent,Vector3 position,Quaternion rotation,Action<T> beforeRent = null)
    {
        T rent = RentWorld(parent, position, beforeRent);
        rent.transform.rotation = rotation;
        return rent;
    }
    
    public void AddList(IList<T> list)
    {
        _list.AddRange(list);
        ActiveItems.AddRange(list);
        foreach (var poolItem in _list)
            poolItem.pool = this;
        ReturnAll();
    }
    
    public void ViewFirst(Transform parent)
    {
        _list.AddRange(parent.GetComponentsInChildren<T>(true));
        ActiveItems.AddRange(parent.GetComponentsInChildren<T>(true));
        foreach (var poolItem in _list)
            poolItem.pool = this;
        ReturnAll();
    }
    public void ViewFirst(IList<T> viewFirst)
    {
        _list.AddRange(viewFirst);
        ActiveItems.AddRange(viewFirst);
        foreach (var poolItem in _list)
            poolItem.pool = this;
        ReturnAll();
    }


    private T FindOne()
    {
        for (var i = 0; i < _list.Count; i++)
            if (_list[i].gameObject.activeSelf == false && _list[i].CheckRent(_checkTime))
  
                return _list[i];
#if UNITY_EDITOR
        if (_prefab == null)
        {
            Debug.LogError("Prefab is null");
        }
#endif
        
        var newOne = Object.Instantiate(_prefab);
        
#if UNITY_EDITOR
        newOne.gameObject.name = _prefab.name +"_"+ _list.Count;
#endif
        _list.Add(newOne);
        return newOne;
    }


    public void Return(PoolItem item)
    {
        item.gameObject.SetActive(false);
        ActiveItems.Remove(item as T);
    }

    public void Destroy(PoolItem item)
    {
        ActiveItems.Remove(item as T);
        _list.Remove(item as T);
        Object.Destroy(item.gameObject);
    }

    public void ReturnAll()
    {
        for (var i = ActiveItems.Count - 1; i >= 0; i--)
            ActiveItems[i].Return();
    }

    public void DestroyAll()
    {
        for (var i = ActiveItems.Count - 1; i >= 0; i--)
            ActiveItems[i].DestroyView();
    }
    
    public void Populate<T2>(IList<T2> list, Action<T, int, T2> func)
    {
        ReturnAll();
        for (int i = 0; i < list.Count; i++)
        {
            T rent = Rent();
            func(rent, i, list[i]);
        }
    }

    public void For<T2>(IList<T2> list, Action<T, int, T2> func)
    {   
        for (var i = 0; i < list.Count; i++)
            func(ActiveItems[i], i, list[i]);
    }
}

