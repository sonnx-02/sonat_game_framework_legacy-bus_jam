using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public struct KnotData
{
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 tangleIn;
    public Vector3 tangleOut;

    public KnotData(Vector3 pos, Vector3 rot,Vector3 tangleIn,Vector3 tangleOut)
    {
        position = pos;
        rotation = rot;
        this.tangleIn = tangleIn;
        this.tangleOut = tangleOut;
    }
    
    public KnotData Reverse()
    {
        return new KnotData(position,rotation + new Vector3(0, 180, 0), tangleIn, tangleOut);
    }

    public KnotData Rotate90()
    {
        var rotate = Quaternion.Euler(0,90,0);
        Vector3 pos = new Vector3(position.z,0,-position.x);
        Vector3 rot = rotation + new Vector3(0, -90, 0);
        return new KnotData(pos,rot, -tangleIn,- tangleOut);
    }
    
    public KnotData Flip()
    {
        Vector3 pos = new Vector3(-position.x,0,position.z);
        Vector3 rot = Quaternion.Euler(rotation).MirrorOnXAxis().eulerAngles;
        return new KnotData(pos,rot, tangleIn, tangleOut);
    }
}

[Serializable]
public class SplineData
{
    public KnotData[] knots;

    public SplineData Reverse()
    {
        return new SplineData()
        {
            knots =  ReverseData().ToArray()
        };
       
    }

    IEnumerable<KnotData> ReverseData()
    {
        for (var i = knots.Length - 1; i >= 0; i--)
            yield return knots[i].Reverse();
    }
}

[Serializable]
public class SplineSerializeData
{
    public List<SplineData> splines = new List<SplineData>();
    

    public KnotData[] Spline
    {
        get
        {
            if(splines == null)
                splines = new List<SplineData>();
            if(splines.Count == 0)
                splines.Add(new SplineData());
            return splines[0].knots;
        }
        set
        {
            if(splines == null)
                splines = new List<SplineData>();
            if(splines.Count == 0)
                splines.Add(new SplineData());
            splines[0].knots = value;
        }
    }
}
