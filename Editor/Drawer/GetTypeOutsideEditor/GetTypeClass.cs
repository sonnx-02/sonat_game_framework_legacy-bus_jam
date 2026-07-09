using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class GetTypeClass : MonoBehaviour
{
    public static Type GetType(string nameSpace,string className)
    {
       
//        var q = from t in Assembly.GetExecutingAssembly().GetTypes()
//                where t.Namespace == "MergeBlock" 
//            select t;
//            
//        Debug.Log(q.ToList().Count);
//            q.ToList().ForEach(t => Console.WriteLine());
//            
//        foreach (var type in q.OrderBy(x =>x.FullName).ToList())
//        {
//            Debug.Log(type.FullName);
//        }
////        
     
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            var type = Type.GetType($"{nameSpace}.{className},{assembly.GetName().Name}");
            if (type != null)
                return type;
        }

        return null;
    }
}

