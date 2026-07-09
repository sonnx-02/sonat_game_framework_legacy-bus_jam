using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

//
//[System.Serializable]
//public class PointWithValue : Point
//{
//    public int value;
//
//    public PointWithValue()
//    {
//
//    }
//
//    public PointWithValue(int col, int row, int value)
//    {
//        base.col = col;
//        base.row = row;
//        this.value = value;
//    }
//}

[System.Serializable]
public class PointWithBools : Point
{
    public bool[] value;

    public int CountTrue => value.Count(x => x);

    public PointWithBools()
    {
    }

    public PointWithBools(int col, int row, bool[] value)
    {
        base.col = col;
        base.row = row;
        this.value = value;
    }
}


[System.Serializable]
public class PointWithVector3 : Point
{
    public Vector3 value;

    public PointWithVector3(int col, int row, Vector3 value)
    {
        base.col = col;
        base.row = row;
        this.value = value;
    }

    public static PointWithVector3 From(Point p, Vector3 vec)
    {
        return new PointWithVector3(p, vec);
    }

    public PointWithVector3(Point p, Vector3 value)
    {
        base.col = p.col;
        base.row = p.row;
        this.value = value;
    }

    public PointWithVector3 Clone2()
    {
        return new PointWithVector3(col, row, value);
    }

    public static PointWithVector3 Create2(int c, int r)
    {
        return new PointWithVector3(c, r, RootView.rootView.gameController.GetPos(Create(c, r)));
    }

    public static PointWithVector3 Create2(Vector2Int vec)
    {
        return new PointWithVector3(vec.x, vec.y, RootView.rootView.gameController.GetPos(Create(vec.x, vec.y)));
    }

    public static PointWithVector3 Create2(Point p)
    {
        return new PointWithVector3(p, RootView.rootView.gameController.GetPos(p));
    }
}

[System.Serializable]
public class CheckPointWithVector3 : Point
{
    public bool flag;
    public Vector3 value;

    public CheckPointWithVector3(int col, int row, bool flag, Vector3 value)
    {
        base.col = col;
        base.row = row;
        this.flag = flag;
        this.value = value;
    }
}


[System.Serializable]
public class PointBoolean : Point
{
    public bool value;

    public PointBoolean()
    {
    }

    public PointBoolean(int col, int row, bool value)
    {
        base.col = col;
        base.row = row;
        this.value = value;
    }

    public PointBoolean Clone2()
    {
        return new PointBoolean(col, row, value);
    }
}

[Serializable]
public class ListPointParameterBaseSerialized
{
    public List<int> serialized = new List<int>();

    public List<PointParameterBase> GetData()
    {
        return GetIeData().ToList();
    }

    public ListPointParameterBaseSerialized()
    {
    }

    public ListPointParameterBaseSerialized(IEnumerable<PointParameterBase> points)
    {
        if (points != null)
            AddRange(points);
    }

    public void Set(IEnumerable<PointParameterBase> points)
    {
        ClearAll();
        AddRange(points);
    }

    public IEnumerable<PointParameterBase> GetIeData()
    {
        if (serialized.Count >= 6)
            for (var i = 0; i < serialized.Count; i += 6)
            {
                yield return new PointParameterBase(
                    serialized[i + 0],
                    serialized[i + 1],
                    serialized[i + 2],
                    serialized[i + 3],
                    serialized[i + 4],
                    serialized[i + 5]
                );
            }
    }

    private int Length => serialized.Count / 6;

    private PointParameterBase this[int i]
    {
        get =>
            new PointParameterBase(
                serialized[i + 0],
                serialized[i + 1],
                serialized[i + 2],
                serialized[i + 3],
                serialized[i + 4],
                serialized[i + 5]);
        set
        {
            serialized[i + 0] = value.col;
            serialized[i + 1] = value.row;
            serialized[i + 2] = value.value;
            serialized[i + 3] = value.value2;
            serialized[i + 4] = value.value3;
            serialized[i + 5] = value.value4;
        }
    }

    public void Add(PointParameterBase value)
    {
        serialized.Add(value.col);
        serialized.Add(value.row);
        serialized.Add(value.value);
        serialized.Add(value.value2);
        serialized.Add(value.value3);
        serialized.Add(value.value4);
    }

    public void AddRange(IEnumerable<PointParameterBase> values)
    {
        foreach (var value in values)
        {
            serialized.Add(value.col);
            serialized.Add(value.row);
            serialized.Add(value.value);
            serialized.Add(value.value2);
            serialized.Add(value.value3);
            serialized.Add(value.value4);
        }
    }

    public void ClearAll()
    {
        serialized.Clear();
    }

    public void RemoveAt(int key)
    {
        for (int i = 6 - 1; i >= 0; i--)
        {
            serialized.RemoveAt(key * 6 + i);
        }
    }

    public ListPointParameterBaseSerialized Clone()
    {
        return new ListPointParameterBaseSerialized()
        {
            serialized = serialized.ToList()
        };
    }

    public void Set(PointParameterBase pointParameterBase, int key)
    {
        serialized[key * 6 + 0] = pointParameterBase.col;
        serialized[key * 6 + 1] = pointParameterBase.row;
        serialized[key * 6 + 2] = pointParameterBase.value;
        serialized[key * 6 + 3] = pointParameterBase.value2;
        serialized[key * 6 + 4] = pointParameterBase.value3;
        serialized[key * 6 + 5] = pointParameterBase.value4;
    }
    
    
    
    public override bool Equals(object other)
    {
        if (other == null)   return false;
        var cast = other as ListPointParameterBaseSerialized;
        if (cast == null) return false;
        return cast.serialized.IsEqual(serialized);
    }

    public static bool operator ==(ListPointParameterBaseSerialized emp1, ListPointParameterBaseSerialized emp2)
    {
        if (ReferenceEquals(emp1, emp2)) return true;
        if (ReferenceEquals(emp1, null)) return false;
        if (ReferenceEquals(emp2, null)) return false;
        return emp1.Equals(emp2);
    }
    

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool operator !=(ListPointParameterBaseSerialized emp1, ListPointParameterBaseSerialized emp2)
    {
        if (ReferenceEquals(emp1, emp2)) return false;
        if (ReferenceEquals(emp1, null)) return true;
        if (ReferenceEquals(emp2, null)) return true;
        return !emp1.Equals(emp2);
    }
}

[System.Serializable]
public class PointParameterBase : Point
{
    [FormerlySerializedAs("z")] public int value;
    public int value2;
    public int value3;
    public int value4;
    public int z => value;


    public bool IsEqual(PointParameterBase other)
    {
        return other == this
               && value == other.value
               && value2 == other.value2
               && value3 == other.value3
               && value4 == other.value4
            ;
    }

    public int this[int key]
    {
        get
        {
            if (key == 0)
                return value;
            if (key == 1)
                return value2;
            if (key == 2)
                return value3;
            if (key == 3)
                return value4;
            Debug.LogError("out of range");
            return int.MinValue;
        }

        set
        {
            if (key == 0)
                this.value = value;
            else if (key == 1)
                value2 = value;
            else  if (key == 2)
                value3 = value;
            else  if (key == 3)
                value4 = value;
            else  Debug.LogError("out of range");
        }
    }

    public bool BoolValue
    {
        get => value == 1;
        set => this.value = value ? 1 : 0;
    }

    public bool BoolValue2
    {
        get => value2 == 1;
        set => value2 = value ? 1 : 0;
    }

    public bool BoolValue3
    {
        get => value3 == 1;
        set => value3 = value ? 1 : 0;
    }

    public bool BoolValue4
    {
        get => value4 == 1;
        set => value4 = value ? 1 : 0;
    }


    public float FloatValue
    {
        get => value / 1000f;
        set => this.value = (int) (value * 1000f);
    }

    public float FloatValue2
    {
        get => value2 / 1000f;
        set => this.value2 = (int) (value * 1000f);
    }

    public float FloatValue3
    {
        get => value3 / 1000f;
        set => this.value3 = (int) (value * 1000f);
    }

    public float FloatValue4
    {
        get => value4 / 1000f;
        set => this.value4 = (int) (value * 1000f);
    }

    public Vector3Int Vec3
    {
        get => new Vector3Int(value, value2, value3);
        set
        {
            this.value = value.x;
            value2 = value.y;
            value3 = value.z;
        }
    }


    public Vector2Int Vec2
    {
        get => new Vector2Int(value, value2);
        set
        {
            this.value = value.x;
            value2 = value.y;
        }
    }

    public Vector3 Vec3Float
    {
        get => new Vector3(FloatValue, FloatValue2, FloatValue3);
        set
        {
            FloatValue = value.x;
            FloatValue2 = value.y;
            FloatValue3 = value.z;
        }
    }

    public Vector2 Vec2Float
    {
        get => new Vector2(FloatValue, FloatValue2);
        set
        {
            FloatValue = value.x;
            FloatValue2 = value.y;
        }
    }

    public PointParameterBase(int col, int row, int value, int value2, int value3, int value4)
    {
        this.col = col;
        this.row = row;
        this.value = value;
        this.value2 = value2;
        this.value3 = value3;
        this.value4 = value4;
    }

    public PointParameterBase(int col, int row, int value, int value2, int value3)
    {
        this.col = col;
        this.row = row;
        this.value = value;
        this.value2 = value2;
        this.value3 = value3;
    }

    public PointParameterBase(int col, int row, int value, int value2)
    {
        this.col = col;
        this.row = row;
        this.value = value;
        this.value2 = value2;
    }

    public PointParameterBase(int col, int row, int value)
    {
        this.col = col;
        this.row = row;
        this.value = value;
    }

    public PointParameterBase(int col, int row, bool value, bool value2, bool value3, bool value4)
    {
        this.col = col;
        this.row = row;
        this.value = value ? 1 : 0;
        this.value2 = value2 ? 1 : 0;
        this.value3 = value3 ? 1 : 0;
        this.value4 = value4 ? 1 : 0;
    }

    public PointParameterBase(int col, int row, bool value, bool value2)
    {
        this.col = col;
        this.row = row;
        this.value = value ? 1 : 0;
        this.value2 = value2 ? 1 : 0;
    }

    public PointParameterBase(int col, int row, bool value)
    {
        this.col = col;
        this.row = row;
        this.value = value ? 1 : 0;
    }

    public PointParameterBase(int col, int row, float value, float value2, float value3, float value4)
    {
        this.col = col;
        this.row = row;
        this.value = (int) (value * 1000f);
        this.value2 = (int) (value2 * 1000f);
        this.value3 = (int) (value3 * 1000f);
        this.value4 = (int) (value4 * 1000f);
    }

    public PointParameterBase(int col, int row, float value, float value2, float value3)
    {
        this.col = col;
        this.row = row;
        this.value = (int) (value * 1000f);
        this.value2 = (int) (value2 * 1000f);
        this.value3 = (int) (value3 * 1000f);
    }

    public PointParameterBase(int col, int row, float value, float value2)
    {
        this.col = col;
        this.row = row;
        this.value = (int) (value * 1000f);
        this.value2 = (int) (value2 * 1000f);
    }

    public PointParameterBase(int col, int row, float value)
    {
        this.col = col;
        this.row = row;
        this.value = (int) (value * 1000f);
    }

    public PointParameterBase(int col, int row)
    {
        this.col = col;
        this.row = row;
    }

    //
    public PointParameterBase(Point point, int value, int value2, int value3, int value4)
    {
        col = point.col;
        row = point.row;
        this.value = value;
        this.value2 = value2;
        this.value3 = value3;
        this.value4 = value4;
    }

    public PointParameterBase(Point point, int value, int value2, int value3)
    {
        col = point.col;
        row = point.row;
        this.value = value;
        this.value2 = value2;
        this.value3 = value3;
    }

    public PointParameterBase(Point point, int value, int value2)
    {
        col = point.col;
        row = point.row;
        this.value = value;
        this.value2 = value2;
    }

    public PointParameterBase(Point point, int value)
    {
        col = point.col;
        row = point.row;
        this.value = value;
    }

    public PointParameterBase(Point point, bool value, bool value2, bool value3, bool value4)
    {
        col = point.col;
        row = point.row;
        this.value = value ? 1 : 0;
        this.value2 = value2 ? 1 : 0;
        this.value3 = value3 ? 1 : 0;
        this.value4 = value4 ? 1 : 0;
    }

    public PointParameterBase(Point point, bool value, bool value2)
    {
        col = point.col;
        row = point.row;
        this.value = value ? 1 : 0;
        this.value2 = value2 ? 1 : 0;
    }

    public PointParameterBase(Point point, bool value)
    {
        col = point.col;
        row = point.row;
        this.value = value ? 1 : 0;
    }

    public PointParameterBase(Point point, float value, float value2, float value3, float value4)
    {
        col = point.col;
        row = point.row;
        this.value = (int) (value * 1000f);
        this.value2 = (int) (value2 * 1000f);
        this.value3 = (int) (value3 * 1000f);
        this.value4 = (int) (value4 * 1000f);
    }

    public PointParameterBase(Point point, float value, float value2, float value3)
    {
        col = point.col;
        row = point.row;
        this.value = (int) (value * 1000f);
        this.value2 = (int) (value2 * 1000f);
        this.value3 = (int) (value3 * 1000f);
    }

    public PointParameterBase(Point point, float value, float value2)
    {
        col = point.col;
        row = point.row;
        this.value = (int) (value * 1000f);
        this.value2 = (int) (value2 * 1000f);
    }

    public PointParameterBase(Point point, float value)
    {
        col = point.col;
        row = point.row;
        this.value = (int) (value * 1000f);
    }

    public PointParameterBase(Point point)
    {
        col = point.col;
        row = point.row;
    }

    public PointParameterBase()
    {
    }

    public PointParameterBase CloneCurrentBase()
    {
        return new PointParameterBase(col, row, value, value2, value3, value4);
    }
}


[System.Serializable]
public class PointParameter : PointParameterBase
{
    public BuiltInEnumType enumType;
    public int enumValue;

    public PointParameter(BuiltInEnumType type, int val) : base()
    {
        SetEnum(type, val);
    }

    public PointParameter(BuiltInEnumType type, int key, int value, int value2) : base()
    {
        SetEnum(type, key);
        this.value = value;
        this.value2 = value2;
    }

    public PointParameter(int col, int row, int value, int value2) : base(col, row, value, value2)
    {
    }

    public PointParameter(int col, int row, int value, int value2, int value3, int value4) : base(col, row, value,
        value2)
    {
        this.value3 = value3;
        this.value4 = value4;
    }

    public PointParameter(int col, int row, int value) : base(col, row, value)
    {
    }

    public PointParameter(Point point, int value, int value2) : base(point, value, value2)
    {
    }
    
    public PointParameter(Point point, int value, int value2,int value3,int value4) : base(point, value, value2,value3,value4)
    {
    }


    public PointParameter(Point point, int value) : base(point, value)
    {
    }

    public PointParameter()
    {
    }


    public void SetEnum(BuiltInEnumType type, int val)
    {
        enumType = type;
        enumValue = val;
    }

    public PointParameter CloneCurrent()
    {
        return new PointParameter()
        {
            enumType = enumType,
            enumValue = enumValue,
            col = col,
            row = row,
            value = value,
            value2 = value2,
            value3 = value3,
            value4 = value4
        };
    }

    public void CopyValueFrom(PointParameterBase p)
    {
        value = p.value;
        value2 = p.value2;
        value3 = p.value3;
        value3 = p.value3;
        value4 = p.value4;
    }

    
}

[System.Serializable]
public class ValuesPoint : Point
{
    public int[] values;
}