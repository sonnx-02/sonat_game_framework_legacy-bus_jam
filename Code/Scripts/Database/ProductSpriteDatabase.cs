using System;
using System.Collections.Generic;
using System.Linq;
using TMPro.SpriteAssetUtilities;
using UnityEngine;
using UnityEngine.Serialization;



[Serializable]
[CreateAssetMenu(fileName = "ProductSpriteDatabase.asset", menuName = "Duong/ProductSpriteDatabase")]
public class ProductSpriteDatabase : ScriptableObject
{
    [SerializeField] private ProductSprite[] sprites;
    
    public ProductSprite GetData(Product key,out int index,bool includeParameter = false)
    {
        index = -1;
        for (var i = 0; i < sprites.Length; i++)
        {
            var item = sprites[i];
            if (item.quantity == key.quantity && item.index == key.index &&
                (!includeParameter || item.parameter == key.parameter))
            {
                index = i;
                return item;
            }
        }
        #if UNITY_EDITOR
//            Debug.LogError("Not found: "+key.quantity + "/"+ key.index+ "/"+ key.parameter);
        #endif
        return null;
    }
}