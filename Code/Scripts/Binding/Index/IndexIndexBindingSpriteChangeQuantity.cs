using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;




public class IndexIndexBindingSpriteChangeQuantity : IndexIndexBindingScriptBase
{
    [SerializeField] private QuantityIndexSprite[] conditions;
    [SerializeField] private SpriteRenderer target;
    [SerializeField] private SpriteRenderer[] otherTargets;
    [SerializeField] private Image target2;
    [SerializeField] private Image[] otherTargets2;
    [SerializeField] private Sprite defaultOutOfRange;
    [SerializeField] private bool inactiveIfOutRange;
    [SerializeField] private bool setNativeSize;
   
    protected override void LoadComponent()
    {
        target = GetComponent<SpriteRenderer>();
        target2 = GetComponent<Image>();
    }


    public override void OnChanged(int value, int value2)
    {
        var s = GetSprite((Quantity)value,value2);
        var outRange = s == null;
        if (inactiveIfOutRange)
        {
            gameObject.SetActive(!outRange);
            if(outRange)
                return;
        }

        if (outRange && defaultOutOfRange != null)
        {
            if (target != null)
                target.sprite = defaultOutOfRange;
            if (target2 != null)
                target2.sprite = defaultOutOfRange;
        }
        else
        {
            if (target != null)
                target.sprite = s;
            if (target2 != null)
            {
                var sprite = s;
                target2.sprite = sprite;
                if(setNativeSize)
                    target2.rectTransform.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
            }
        }


        foreach (var temp in otherTargets)
            temp.sprite = s;
        foreach (var temp in otherTargets2)
            temp.sprite = s;
        
        
        Sprite GetSprite(Quantity val,int val2)
        {
            foreach (var quantitySprite in conditions)
            {
                if (quantitySprite.quantity == val && quantitySprite.index == val2)
                    return quantitySprite.sprite;
            }
            return null;
        }
    }
}