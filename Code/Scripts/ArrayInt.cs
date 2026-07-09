using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

[Serializable]
public class ArrayBool
{
    public bool[] values;
}

[Serializable]
public class ArrayPoint
{
    public PointParameter parameter;
    public int[] cols;
    public int[] rows;

    public Point this[int index]
    {
        get => new Point(cols[index], rows[index]);
        set
        {
            cols[index] = value.col;
            rows[index] = value.row;
            
        }
    }
    
    public void SetPoint(int index, Point p)
    {
        if (index < Length)
            this[index] = p;
        else
        {
            cols = cols.ArrayAdd(p.col).ToArray();
            rows = rows.ArrayAdd(p.row).ToArray();
        }
    }

    public ArrayPoint()
    {
        
    }
    
    public ArrayPoint(PointParameter parameter,int[] cols,int[] rows)
    {
        this.parameter = parameter.CloneCurrent();
        this.cols = cols.ToArray();
        this.rows = rows.ToArray();
    }
    
    public ArrayPoint(PointParameter parameter,IList<Point> points)
    {
        this.parameter = parameter.CloneCurrent();
        cols = new int[points.Count];
        rows = new int[points.Count];
        for (var i = 0; i < points.Count; i++)
            this[i] = points[i];
    }
    
    public ArrayPoint(int[] cols,int[] rows)
    {
        this.cols = cols.ToArray();
        this.rows = rows.ToArray();
    }
    
    public ArrayPoint(IList<Point> points)
    {
        cols = new int[points.Count];
        rows = new int[points.Count];
        for (var i = 0; i < points.Count; i++)
            this[i] = points[i];
    }

    public int Length => cols.Length;

    public ArrayPoint Clone() => new ArrayPoint(parameter.CloneCurrent(),cols,rows);

  
}

[Serializable]
public class ArrayInt
{
  
    public int[] values;

    public int Sum() => values.Sum();

    public ArrayInt(int[] values)
    {
        this.values = values;
    }

    public ArrayInt(int value1, int value2, int value3, int value4)
    {
        values = new[] {value1, value2, value3, value4};
    }
    
    public ArrayInt Clone()
    {
        return new ArrayInt(values.ToArray());
    }
    
    public int GetRandom()
    {
        if (values.Length == 1)
            return 0;

        float totalWeight = 0;
        float[] weightValues = new float[values.Length];
        for (var i = 0; i < values.Length; i++)
        {
            totalWeight += values[i];
            weightValues[i] = totalWeight;
        }

        float randomValue = Random.Range(0f, totalWeight);

        for (int i = 0; i < weightValues.Length; i++)
            if (randomValue < weightValues[i])
                return i;

        return -1;
    }

    public string toString()
    {
        return string.Join('-',values);
    }
}
