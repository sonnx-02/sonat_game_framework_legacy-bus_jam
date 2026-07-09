using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//[System.Serializable]
//public class ObjectAsset
//{
//    public int Id;
//    public Sprite FragmentIcon;
//    public Sprite[] Sprites;
//}



[System.Serializable]
public class RewardGameAsset
{
    public Quantity quantity;
    public Sprite randomSprite; // as -1 index
    public List<Sprite> sprites;
    public string defaultDescription;
    public string minusOneDescription; // for random
}

[System.Serializable]
public class ListSprite
{
//    [IndexAsEnum(BuiltInEnumType.SpriteListEnum)]
//    public int key;
    public List<Sprite> sprites;
    public List<Sprite> sprites2;
    public List<Sprite> sprites3;

    public Sprite GetSprite(int index,GameObject callstack = null)
    {
        if (sprites.Count == 0)
        {
            Debug.LogError("list empty GraphicAssetDatabase",callstack);
            return null;   }

        if (index >= sprites.Count)
        {
            Debug.LogError("out of range GraphicAssetDatabase",callstack);
            return null; 
        }

        if (index < 0)
        {
            Debug.LogError("below zero GraphicAssetDatabase",callstack);
            return null; 
        }

        return sprites[index];
    }
    
    public Sprite GetSprite2(int index)
    {
        if (sprites2.Count == 0)
        {
            Debug.LogError("list empty GraphicAssetDatabase");
            return null;   }

        if (index >= sprites2.Count)
        {
            Debug.LogError("out of range GraphicAssetDatabase");
            return null; 
        }

        return sprites2[index];
    }
    
    public Sprite GetSprite3(int index)
    {
        if (sprites3.Count == 0)
        {
            Debug.LogError("list empty GraphicAssetDatabase");
            return null;   }

        if (index >= sprites3.Count)
        {
            Debug.LogError("out of range GraphicAssetDatabase");
            return null; 
        }

        return sprites3[index];
    }
}

[CreateAssetMenu(fileName = nameof(GraphicAssetDatabase)+".asset", menuName = "Duong/"+nameof(GraphicAssetDatabase))]
public class GraphicAssetDatabase : ScriptableObject
{
     public Sprite[] avatarSprites;
    [ArrayElementTitle(nameof(RewardGameAsset.quantity))]
    public List<RewardGameAsset> rewardGameAssets;
   
    [ArrayLabelIndexAsEnum(BuiltInEnumType.SpriteListEnum)]
    public List<ListSprite> spriteLists;
    
    public Sprite GetSprite(int spriteListEnum,int index,GameObject callStack = null)
    {
        return spriteLists[spriteListEnum].GetSprite(index,callStack);
    }

    
    public Sprite GetRewardImage(Product product)
    {
        return GetRewardImage(product.quantity, product.index);
    }

    public Sprite GetRewardImage(Quantity quantity,int index)
    {
        foreach (var rewardGameAsset in rewardGameAssets)
            if (quantity == rewardGameAsset.quantity)
            {
                if (index < 0)
                    return rewardGameAsset.randomSprite;
                if (index < 0 || index >= rewardGameAsset.sprites.Count)
                    Debug.LogError("Not found asset for product" + quantity + " i=" + index);
                return rewardGameAsset.sprites[index];
            }
        
        Debug.LogError("Cannot find RewardGameAsset for " + quantity);
        return null;
    }

    public string GetPreviewDescription(Quantity quantity, int index)
    {
        foreach (var rewardGameAsset in rewardGameAssets)
            if (quantity == rewardGameAsset.quantity)
            {
                if (index < 0)
                    return rewardGameAsset.minusOneDescription;
                if (index < 0 || index >= rewardGameAsset.sprites.Count)
                    Debug.LogError("Not found aset for product" + quantity + " i=" + index);
                return rewardGameAsset.defaultDescription;
            }


        Debug.LogError("Cannot find RewardGameAsset for" + quantity);
        Debug.Break();

        return null;
    }
}
