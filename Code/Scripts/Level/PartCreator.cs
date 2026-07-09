//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//#if UNITY_EDITOR
//using UnityEditor;
//#endif
//using UnityEngine;
//using UnityEngine.Serialization;
//
//
//public class PartView
//{
//    public readonly List<BlockView> blocks = new List<BlockView>();
//    public readonly List<PlatformView> platforms = new List<PlatformView>();
//    public readonly List<CoinView> coins = new List<CoinView>();
//    public readonly List<ImpedimentView> impediments = new List<ImpedimentView>();
//    public readonly List<CurtainView> curtains = new List<CurtainView>();
//    public readonly List<SlopeView> slopes = new List<SlopeView>();
//}
//
//public class PartCreator : MonoBehaviour
//{
//    public LevelDatabase database;
//    public PoolManager poolManager;
//    public LevelCreator levelCreator;
//    public int Count => database.parts.Count;
//    [MyButtonIntSelect(new[] {nameof(GetPart), nameof(LoadPart), nameof(InitPart), nameof(SavePart)}, nameof(Count))]
//    public int test1;
//
//    public void RefreshAssets(int part)
//    {
//        AssetDatabase.SaveAssets();
//        AssetDatabase.Refresh();
//    }
//
//    [SerializeField] private LevelPart tempPart;
//
//    [MyButtonInt(nameof(AdjustLength))] public int distance;
//
//    public void AdjustLength(int value)
//    {
//        tempPart.distance = value;
//        levelCreator.InitLevel(tempPart.ToLevel());
//    }
//
//    public void LoadPart(int index)
//    {
//        tempPart = database.parts[index].Clone();
//        Debug.Log("loaded");
//    }
//
//    public void InitPart()
//    {
//        poolManager.ReturnAll();
//        levelCreator.InitLevel(tempPart.ToLevel());
//        distance = tempPart.distance;
//    }
//
//
//    public void GetPart(int index)
//    {
//        tempPart = new LevelPart()
//        {
//            blocks = GetBlocks().ToArray(),
//            coins = GetCoins().ToArray(),
//            curtains = GetCurtain().ToArray(),
//            impediments = GetImpediment().ToArray(),
//            platforms = GetPlatform().ToArray(),
//            slopes = GetSlope().ToArray(),
//        };
//        tempPart.distance = distance;
//    }
//
//
//    public void SavePart(int part)
//    {
//#if UNITY_EDITOR
//
//        if (tempPart.distance < 1)
//        {
//            Debug.LogError("set distance");
//            return;
//        }
//
//        if (part == database.parts.Count)
//            database.parts.Add(tempPart);
//        else
//            database.parts[part] = tempPart;
//
//        EditorUtility.SetDirty(database);
//        AssetDatabase.SaveAssets();
//        AssetDatabase.Refresh();
//#endif
//    }
//
//    IEnumerable<LevelBlock> GetBlocks()
//    {
//        foreach (var pool in poolManager.blocks.pools)
//            for (var i = 0; i < pool.Count; i++)
//            {
//                if (pool[i].gameObject.activeInHierarchy)
//                    yield return new LevelBlock()
//                    {
//                        id = pool[i].id,
//                        color = pool[i].color.Value,
//                        position = pool[i].transform.position,
//                    };
//            }
//    }
//
//    IEnumerable<LevelCoin> GetCoins()
//    {
//        foreach (var pool in poolManager.coins.pools)
//            for (var i = 0; i < pool.Count; i++)
//                yield return new LevelCoin()
//                {
//                    id = pool[i].id,
//                    position = pool[i].transform.position
//                };
//    }
//
//    IEnumerable<LevelPlatform> GetPlatform()
//    {
//        foreach (var pool in poolManager.platforms.pools)
//            for (var i = 0; i < pool.Count; i++)
//                yield return new LevelPlatform()
//                {
//                    id = pool[i].id,
//                    position = pool[i].transform.position,
//                    scale = pool[i].transform.localScale,
//                };
//    }
//
//    IEnumerable<LevelCurtain> GetCurtain()
//    {
//        foreach (var pool in poolManager.curtains.pools)
//            for (var i = 0; i < pool.Count; i++)
//                yield return new LevelCurtain()
//                {
//                    id = pool[i].id,
//                    position = pool[i].transform.position,
//                    color = pool[i].color.Value,
//                };
//    }
//
//    IEnumerable<LevelImpediment> GetImpediment()
//    {
//        foreach (var pool in poolManager.impediments.pools)
//            for (var i = 0; i < pool.Count; i++)
//                yield return new LevelImpediment()
//                {
//                    id = pool[i].id,
//                    position = pool[i].transform.position,
//                };
//    }
//
//    IEnumerable<LevelSlope> GetSlope()
//    {
//        foreach (var pool in poolManager.slopes.pools)
//            for (var i = 0; i < pool.Count; i++)
//                yield return new LevelSlope()
//                {
//                    id = pool[i].id,
//                    position = pool[i].transform.position,
//                };
//    }
//}