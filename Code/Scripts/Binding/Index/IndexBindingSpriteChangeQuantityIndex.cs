using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;




public class IndexBindingSpriteChangeQuantity : IndexBindingScript
{
    [SerializeField] private SpriteRenderer target;
    [SerializeField] private SpriteRenderer[] otherTargets;
    [SerializeField] private Image target2;
    [SerializeField] private Image[] otherTargets2;
   
    [SerializeField] private Sprite defaultOutOfRange;
    [SerializeField] private QuantitySprite[] conditions;
 
    [SerializeField] private bool inactiveIfOutRange;
    [SerializeField] private bool clamp;
    [SerializeField] private bool setNativeSize;
    
    public override int Size => conditions.Length;


    protected override void LoadComponent()
    {
        target = GetComponent<SpriteRenderer>();
        target2 = GetComponent<Image>();
    }

    protected override bool IsNoComponent()
    {
        return target == null && target2 == null;
    }


    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        var s = GetSprite((Quantity)computedValue);
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
        
        
        Sprite GetSprite(Quantity val)
        {
            foreach (var quantitySprite in conditions)
            {
                if (quantitySprite.quantity == val)
                    return quantitySprite.sprite;
            }

            return null;
        }
    }
}