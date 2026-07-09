using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class WayPointData
{
    public int id;
    public Vector3 pos;
    public int[] connects;
}


public class WayPoint : PoolItem
{
    [SerializeField] private int id;
    [SerializeField] private List<WayPoint> connects1;

   // [SerializeField] private int[] connectIds;
   public List<WayPoint> Connects1 => connects1;

 
    public int Id
    {
        get => id;
        set => id = value;
    }

    public WayPointData GetData()
    {
        return new WayPointData()
        {
            id = id,
            pos = transform.localPosition,
            connects = connects1.Select(x => x.id).ToArray(),
        };
    }
}