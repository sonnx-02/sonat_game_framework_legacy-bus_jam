using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;


[Serializable]
public class LongSerializableDictionary : BaseSerializableDictionary
{
    public long defaultValue;
    public List<long> items;

    // id/index
    private Dictionary<int, int> _dictionary = new Dictionary<int, int>();

    public long this[int key]
    {
        get
        {
            if (Exist(key))
                return items[_dictionary[key]];

            return defaultValue;
        }
        set
        {
            if (Exist(key))
            {
                items[_dictionary[key]] = value;
            }
            else
            {
                keys.Add(key);
                items.Add(value);
                _dictionary.Add(key, keys.IndexOf(key));
            }
            DataUpdated.OnNext(key);
        }
    }

    public override bool Exist(int key)
    {
        if (_dictionary.ContainsKey(key))
            return true;

        var index = keys.IndexOf(key);
        if (index >= 0)
        {
            _dictionary.Add(key, index);
            return true;
        }

        return false;
    }

    public LongSerializableDictionary()
    {
        keys = new List<int>();
        items = new List<long>();
    }

    public LongSerializableDictionary Clone()
    {
        return new LongSerializableDictionary()
        {
            keys = keys.ToList(),
            items = items.ToList(),
        };
    }

    public override void ClearDict()
    {
        keys.Clear();
        items.Clear();
        _dictionary = new Dictionary<int, int>();
    }
}

[Serializable]
public class BooleanSerializableDictionary : BaseSerializableDictionary
{
    public bool defaultValue;
    public List<bool> items;
    
    

    // id/index
    private Dictionary<int, int> _dictionary = new Dictionary<int, int>();

    public bool this[int key]
    {
        get
        {
            if (Exist(key))
                return items[_dictionary[key]];

            return defaultValue;
        }
        set
        {
            if (Exist(key))
            {
                items[_dictionary[key]] = value;
            }
            else
            {
                keys.Add(key);
                items.Add(value);
                _dictionary.Add(key, keys.IndexOf(key));
            }

            DataUpdated.OnNext(key);
        }
    }

    public override bool Exist(int key)
    {
        if (_dictionary.ContainsKey(key))
            return true;

        var index = keys.IndexOf(key);
        if (index >= 0)
        {
            _dictionary.Add(key, index);
            return true;
        }

        return false;
    }

    public BooleanSerializableDictionary()
    {
        keys = new List<int>();
        items = new List<bool>();
    }

    public BooleanSerializableDictionary Clone()
    {
        return new BooleanSerializableDictionary()
        {
            keys = keys.ToList(),
            items = items.ToList(),
        };
    }

    public override void ClearDict()
    {
        keys.Clear();
        items.Clear();
        _dictionary = new Dictionary<int, int>();
    }
}



[System.Serializable]
public class FloatSerializableDictionary : BaseSerializableDictionary
{
    private float _currentValue;
    private readonly float _defaultValue;
    public List<float> items = new List<float>();
    private Dictionary<int, int> _dictionary = new Dictionary<int, int>();

    public float Count => items.Count;

    public float CountValue => items.Sum();

    
    public FloatSerializableDictionary()
    {
    }

    public FloatSerializableDictionary(float currValue, float defaultVal = -1)
    {
        _currentValue = currValue;
        _defaultValue = defaultVal;
    }

    public float Current
    {
        get => _currentValue;
        set
        {
            if (Math.Abs(_currentValue - value) > 0.001f)
            {
                _currentValue = value;
            }
        }
    }

    // id/index
    // value of subject index
    public float this[int key]
    {
        get
        {
            if (Exist(key))
                return items[_dictionary[key]];

            return _defaultValue;
        }
        set
        {
            if (Exist(key))
            {
                if (items[_dictionary[key]] != value)
                {
                    items[_dictionary[key]] = value;
                    DataUpdated.OnNext(key);
                }
            }
            else
            {
                keys.Add(key);
                items.Add(value);
                _dictionary.Add(key, keys.IndexOf(key));
                DataUpdated.OnNext(key);
            }
        }
    }

    public override bool Exist(int key)
    {
        if (_dictionary.Count > items.Count)
        {
            Debug.LogError("What the fuck 2");
            //Debug.LogError(key + "_dictionary[key]" + _dictionary[key] + " Items" + Items.Count);
            items.ForEach(x => Debug.Log("Item" + x));
            keys.ForEach(x => Debug.Log("Key" + x));
            foreach (var keyValuePair in _dictionary)
            {
                Debug.Log(keyValuePair.Key + "/" + keyValuePair.Value);
            }

            Debug.Break();
        }

        if (_dictionary.ContainsKey(key))
            return true;
        // for existing
        var index = keys.IndexOf(key);
        if (index >= 0)
        {
            _dictionary.Add(key, index);
            return true;
        }

        return false;
    }


    public FloatSerializableDictionary Clone()
    {
        return new FloatSerializableDictionary()
        {
            keys = keys.ToList(),
            items = items.ToList(),
        };
    }

    public override void ClearDict()
    {
        keys.Clear();
        items.Clear();
        _dictionary = new Dictionary<int, int>();
    }
}



[System.Serializable]
public class IntegerSerializableDictionary : BaseSerializableDictionary
{
    private int _currentValue;
    private readonly int _defaultValue;
    public List<int> items = new List<int>();
    private Dictionary<int, int> _dictionary = new Dictionary<int, int>();

    public int Count => items.Count;

    public int CountValue => items.Sum();

    public int Max => keys.Count == 0 ? -1 : keys.Max();
    
    public IntegerSerializableDictionary()
    {
    }

    public IntegerSerializableDictionary(int currValue, int defaultVal = -1)
    {
        _currentValue = currValue;
        _defaultValue = defaultVal;
    }

    public int Current
    {
        get => _currentValue;
        set
        {
            if (_currentValue != value)
            {
                _currentValue = value;
                CurrentUpdated.OnNext(_currentValue);
            }
        }
    }

    // id/index
    // value of subject index
    public int this[int key]
    {
        get
        {
            if (Exist(key))
                return items[_dictionary[key]];

            return _defaultValue;
        }
        set
        {
            if (Exist(key))
            {
                if (items[_dictionary[key]] != value)
                {
                    items[_dictionary[key]] = value;
                    DataUpdated.OnNext(key);
                }
            }
            else
            {
                keys.Add(key);
                items.Add(value);
                _dictionary.Add(key, keys.IndexOf(key));
                DataUpdated.OnNext(key);
            }
        }
    }

    public override bool Exist(int key)
    {
        if (_dictionary.Count > items.Count)
        {
            Debug.LogError("What the fuck 2");
            //Debug.LogError(key + "_dictionary[key]" + _dictionary[key] + " Items" + Items.Count);
            items.ForEach(x => Debug.Log("Item" + x));
            keys.ForEach(x => Debug.Log("Key" + x));
            foreach (var keyValuePair in _dictionary)
            {
                Debug.Log(keyValuePair.Key + "/" + keyValuePair.Value);
            }

            Debug.Break();
        }

        if (_dictionary.ContainsKey(key))
            return true;
        // for existing
        var index = keys.IndexOf(key);
        if (index >= 0)
        {
            _dictionary.Add(key, index);
            return true;
        }

        return false;
    }


    public IntegerSerializableDictionary Clone()
    {
        return new IntegerSerializableDictionary()
        {
            keys = keys.ToList(),
            items = items.ToList(),
        };
    }

    public override void ClearDict()
    {
        keys.Clear();
        items.Clear();
        _dictionary = new Dictionary<int, int>();
    }

    public int Sum() => items.Sum();

    public bool AnyValue()
    {
        return items.Sum() > 0;
    }
}

[System.Serializable]
public abstract class BaseSerializableDictionary
{
    [SerializeField]
    protected List<int> keys = new List<int>();
    public abstract void ClearDict();

    public List<int> Keys => keys;

    public virtual bool Exist(int key)
    {
        return keys.Contains(key);
    }

    public readonly Subject<int> DataUpdated = new Subject<int>();
    public readonly Subject<int> CurrentUpdated = new Subject<int>();
}


[System.Serializable]
public class DictStringString
{
    public List<string> keys = new List<string>();
    public List<string> values = new List<string>();

    public int Count => keys.Count;


    public virtual bool Exist(string key)
    {
        return keys.Contains(key);
    }

    public virtual string Get(string key)
    {
        for (var i = 0; i < keys.Count; i++)
        {
            if (keys[i] == key)
                return values[i];
        }
    
        Debug.LogError("not found");
        return string.Empty;
    }
    
    public virtual void Set(string key,string value)
    {
        for (var i = 0; i < keys.Count; i++)
        {
            if (keys[i] == key)
                 values[i] = value;
        }
    
        keys.Add(key);
        values.Add(value);
    }

    public bool Equal(DictStringString other)
    {
        if (other.Count != Count)
            return false;
        for (var i = 0; i < other.keys.Count; i++)
        {
            if (!Exist(other.keys[i]))
                return false;
            if (Get(other.keys[i]) != other.values[i])
                return false;
        }

        return true;
    }
}


