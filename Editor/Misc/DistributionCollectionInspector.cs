//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;
//
//[CustomEditor(typeof(DistributionCollection))]
//public class DistributionCollectionInspector : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        DistributionCollection myScript = (DistributionCollection)target;
//        if (GUILayout.Button("Load From Children"))
//        {
//            myScript.Categories = new List<CategoryBase>();
//            for (int i = 0; i < myScript.transform.childCount; i++)
//            {
//                var category = myScript.transform.GetChild(i).GetComponent<CategoryBase>();
//                if (category != null)
//                    myScript.Categories.Add(category);
//            }
//        }
//           
//
//        nTake = EditorGUILayout.IntField("nLevels Take",nTake);
//
//        if (GUILayout.Button("Generate Roll ABall Category"))
//        {
//            var newGO = new GameObject("generated");
//            newGO.transform.SetParent(myScript.transform);
//            var generate = newGO.gameObject.AddComponent<RollABallCategory>();
//
//            int nTakeFromEach = Mathf.CeilToInt(nTake/ myScript.Categories.Count);
//            for (int i=0;i< myScript.Categories.Count;i++)
//            {
//                if (i == myScript.Categories.Count - 1)
//                    nTakeFromEach = nTake - generate.RollABallLevels.Count;
//
//                var takeOut = (myScript.Categories[i] as RollABallCategory).RollABallLevels.ToList();
//                takeOut.Shuffle();
//                takeOut = takeOut.Take(nTakeFromEach).ToList();
//
//                foreach(var level in takeOut)
//                    generate.RollABallLevels.Add(level.CloneAndRotate());
//            }
//        }
//        DrawDefaultInspector();
//    }
//}
