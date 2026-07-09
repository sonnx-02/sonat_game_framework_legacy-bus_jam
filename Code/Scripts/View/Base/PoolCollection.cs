using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public struct RentButton
{
}


public class IndexPoolCollection<T> where T : PoolItem
{
    private Dictionary<int, Pool<T>> _pools;

    public Pool<T> this[int index]
    {
        get
        {
            #if UNITY_EDITOR
            if(!_pools.ContainsKey(index))
                Debug.LogError("not found key"+index);
            #endif
       
            return _pools[index];
        }
    }

    public int CountObjectInPoolIndex(int index)
    {
        return _pools[index].Count;
    }

    public int CountPools() => _pools.Count;

    public T Get(int poolIndex, int itemIndex)
    {
        return _pools[poolIndex][itemIndex];
    }

    private bool _initialized;

    public IndexPoolCollection(int[] keys, T[] prefab, Transform parent = null, float minTime = 1)
    {
        _pools = new Dictionary<int, Pool<T>>();
        for (var index = 0; index < keys.Length; index++)
        {
            var key = keys[index];
            var pool = new Pool<T>(prefab[index], parent, minTime);
            _pools.Add(key, pool);
        }

        _initialized = true;
    }

    public void ViewFirst(int index, Transform parent = null)
    {
        _pools[index].ViewFirst(parent);
    }


    public T Rent(int index)
    {
        return _pools[index].Rent();
    }

    public void ReturnAll()
    {
        foreach (var pool in _pools)
        {
            pool.Value.ReturnAll();
        }
    }

    public void DestroyAll()
    {
        foreach (var pool in _pools)
        {
            pool.Value.DestroyAll();
        }
    }
}

[Serializable]
public class ListPrefab<T> where T : PoolItem
{
     public T[] prefabs;
}

[Serializable]
public class CollectionPrefab<T> where T : PoolItem
{
    public ListPrefab<T>[] collections;
}

[System.Serializable]
public class PoolCollection<T> where T : PoolItem
{
    [SerializeField] private Transform container;
    [SerializeField] private Transform[] otherContainers;
    [SerializeField] private T[] prefabs;

    private T[] _collections;
    
    private Pool<T>[] _pools;
    private Dictionary<int, Pool<T>> _dictionary;

    public Transform Container => container;

    public Transform[] OtherContainers => otherContainers;
    public T[] Collections => _collections;
    // ReSharper disable once InconsistentNaming
    public Pool<T>[] pools
    {
        get
        {
            if (!_initialized)
                Debug.LogError("not initialized");
            return _pools;
        }
    }
    

    public int Count(int index)
    {
        return _pools[index].Count;
    }

    public int CountAll()
    {
       return _pools.Sum(x => x.Count);
    }

    public T Get(int poolIndex, int itemIndex)
    {
        return _pools[poolIndex][itemIndex];
    }

    private bool _initialized;

    private Transform GetContainer(int i)
    {
        if (i >= 0 && i < otherContainers.Length)
            return otherContainers[i];
        return container;
    }

    // =============== LAZY
    private float _minTime;
    public void LazyInit(int length,float minTime = 0)
    {
        _createDict = true;
        _minTime = minTime;
        _pools = new Pool<T>[length];
        _dictionary = new Dictionary<int, Pool<T>>();
        _initialized = true;
    }
    public void LazyInit(int dictKey,int index,T prefab)
    {
        _pools[index] = new Pool<T>(prefab,null , _minTime);
        _dictionary.Add(dictKey, _pools[index]);
    }

    public bool IsLazyInitializedByIndex(int index) => _pools[index] != null;
    public bool IsLazyInitializedByDictKey(int dictKey) => _dictionary.ContainsKey(dictKey);

    
    // ============ end lazy
    
    private bool _createDict;
    
    public void Init(T[] viewFirst, float minTime = 0,bool createDict = false)
    {
        Init(minTime, createDict);
        foreach (var poolItem in viewFirst)
            poolItem.gameObject.SetActive(false);
        _pools[0].Add(viewFirst);
    }

    public void Init(float minTime = 0,bool createDict = false)
    {
        DoInit(prefabs, minTime, createDict);
    }

    private void DoInit(IEnumerable<T> init,float minTime = 0,bool createDict = false)
    {
        _collections = init.ToArray();
        _createDict = createDict;
        _pools = new Pool<T>[_collections.Length];
        if (createDict)
            _dictionary = new Dictionary<int, Pool<T>>();
        for (var i = 0; i < _collections.Length; i++)
        {
            _pools[i] = new Pool<T>(_collections[i], GetContainer(i), minTime);
            if (createDict)
                _dictionary.Add(_collections[i].Index,_pools[i]);
        }
        _initialized = true;
    }
    
    public void OverrideContainer(Transform overrideContainer)
    {
        foreach (var pool in _pools)
            pool.SetParent(overrideContainer);
    }

  
    public void ViewFirst(int index)
    {
        #if UNITY_EDITOR
            if(_pools.Length == 0)
                Debug.LogError("no pool colletion prefab sets");
        #endif
        _pools[index].ViewFirst(GetContainer(index));
    }

    public Pool<T> GetPool(int key)
    {
        return _dictionary[key];
    }

    public T RentDict(int key)
    {
        return _dictionary[key].Rent();
    }

    public T Rent(int index)
    {
        return _pools[index].Rent();
    }

    public void ReturnAll()
    {
        if(_initialized)
            for (var index = 0; index < _pools.Length; index++)
            {
                var pool = _pools[index];
                if (pool != null)
                    pool.ReturnAll();
            }
    }

    public void DestroyAll()
    {
        foreach (var pool in _pools)
        {
            pool.DestroyAll();
        }
    }

    public IEnumerable<T> GetActives()
    {
        foreach (var pool in pools)
        {
            foreach (var poolActiveItem in pool.ActiveItems)
            {
                yield return poolActiveItem;
            }
        }
    }
}