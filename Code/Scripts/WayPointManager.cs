using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class WayPointHelper
{
   
}

[System.Serializable]
public class WayPointDatabase
{
   public WayPointData[] data;
}


public class WayPointManager : IndexBindingScript
{
   [SerializeField] private List<WayPointDatabase> database;
  
   [SerializeField] WayPoint[] wayPoints;
   [SerializeField] WayPoint prefabs;

   [MyButton(nameof(LoadChildren),nameof(Add2Direction),nameof(SetIdAndRename))] [SerializeField] private int test123;

   private Pool<WayPoint> pool;

   protected override void Register()
   {
      base.Register();
      pool = new Pool<WayPoint>(prefabs,null,0);
      pool.ViewFirst(wayPoints);
   }

   [MyButtonRange(0,5,nameof(Load),null,"Load")] [SerializeField]   private int testLoad1as;
   [MyButtonRange(0,5,nameof(Add),null,"Add")] [SerializeField]   private int testLoad2as;

   [MyButton(nameof(Rent))] [SerializeField]
   private int testLoadsdfsdf;

   public void Rent()
   {
      var rent = pool.Rent(transform, Vector3.zero);
      #if UNITY_EDITOR
      UnityEditor.Selection.objects = new Object[]{rent.gameObject};
      #endif
   }
   
   public void Load(int index)
   {
      if(!Application.isPlaying)
         return;
      pool.ReturnAll();
      foreach (var wayPointData in database[index].data)
      {
         var rent = pool.Rent(transform, wayPointData.pos);
         rent.Id = wayPointData.id;
         rent.Connects1.Clear();
      }

      for (var i = 0; i < database[index].data.Length; i++)
      {
         var rent = pool.ActiveItems[i];
         foreach (var connect in database[index].data[i].connects)
            rent.Connects1.Add(pool.ActiveItems.Find(x => x.Id == connect));
      }

      wayPoints = pool.ActiveItems.ToArray();
      Add2Direction();
   }

   public void Add(int index)
   {
      if (index < database.Count)
         database[index] = GetData();
      else
         database.Add(GetData());
   }

   private WayPointDatabase GetData()
   {
      return new WayPointDatabase()
      {
         data = wayPoints.Select(x => x.GetData()).ToArray(),
      };
   }

   private void LoadChildren()
   {
   
      wayPoints = GetComponentsInChildren<WayPoint>();
   }

   private void Add2Direction()
   {
      foreach (var wayPoint in wayPoints)
         foreach (var point in wayPoint.Connects1)
            point.Connects1.AddDistinct(wayPoint);
   }

   private void SetIdAndRename()
   {
      for (var index = 0; index < wayPoints.Length; index++)
      {
         var wayPoint = wayPoints[index];
         wayPoint.Id = index;
         wayPoint.gameObject.name = "WayPoint (" + index+")";
;      }
   }

   public IEnumerable<Vector3> FindPos(Vector3 from, Vector3 target)
   {
      var fromWayPoint = ClosestToTarget(from);
      yield return fromWayPoint.transform.position;
      
      var targetWayPoint = ClosestToTarget(target);
      yield return targetWayPoint.transform.position;
   }

   private WayPoint ClosestToTarget(Vector3 target)
   {
      float min = float.MaxValue;
      WayPoint closest = null;
      foreach (var wayPoint in wayPoints)
      {
         var distance = Vector3.Distance(wayPoint.transform.position, target);
         if (distance < min)
         {
            min = distance;
            closest = wayPoint;
         }
      }

      return closest;
   }
}
