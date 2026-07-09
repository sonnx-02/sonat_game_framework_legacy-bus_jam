using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class IndexBindingSpriteChangeSimple : IndexBindingScript
{
    [SerializeField] private SpriteRenderer target;
    [SerializeField] private Image target2;
    [SerializeField] private PresetSpriteDatabase database;
    [SerializeField] private Sprite[] sprites;
    
    [SerializeField] private bool clamp;

    public override int Size => sprites.Length;

    protected virtual Sprite[] Sprites
    {
        get
        {
            if (database != null)
                return database.Sprites;
            return sprites;
        }
    }
    
    [SerializeField] private bool setNativeSize;
    [SerializeField] private Sprite defaultOutOfRange;
    [SerializeField] private bool deactiveIfOutRange;

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
        var outRange = computedValue >= Sprites.Length || computedValue < 0;
        if (deactiveIfOutRange)
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
                target.sprite = Sprites[clamp ? computedValue % Sprites.Length : computedValue];
            if (target2 != null)
            {
#if UNITY_EDITOR
            
                if((clamp ? computedValue % Sprites.Length : computedValue) > Sprites.Length)
                    Debug.LogError("out of range",gameObject);
#endif
            
                var sprite = Sprites[clamp ? computedValue % Sprites.Length : computedValue];
                target2.sprite = sprite;
           
                if(setNativeSize)
                    target2.rectTransform.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
            }
        }
    }
}