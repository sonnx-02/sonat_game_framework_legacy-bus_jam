using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class IndexBindingSpriteChangeRawImage : IndexBindingScript
{
    [SerializeField] private RawImage target;
    
    [SerializeField] private Texture[] sprites;
    [SerializeField] private Texture[] others1;
    [SerializeField] private Texture[] others2;
    [SerializeField] private Texture[] others3;
    
    [SerializeField] private bool clamp;

    public override int Size => sprites.Length;

    public virtual Texture[] Sprites
    {
        get
        {
            switch (sharedSetting.index2)
            {
                case 0:
                    return sprites;
                case 1:
                    return others1;
                case 2:
                    return others2;
                case 3:
                    return others3;
                default:
                    return null;
            }
        }
    }

    
    [SerializeField] private RawImage[] otherTargets;
    [SerializeField] private RawImage[] otherTargets2;
    [SerializeField] private bool setNativeSize;
    
    [SerializeField] private Texture defaultOutOfRange;

    [SerializeField] private bool deactiveIfOutRange;

    protected override void LoadComponent()
    {
        target = GetComponent<RawImage>();
   
    }

    protected override bool IsNoComponent()
    {
        return target == null;
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
                target.texture = defaultOutOfRange;
      
        }
        else
        {
            if (target != null)
                target.texture = Sprites[clamp ? computedValue % Sprites.Length : computedValue];

        }

        
        foreach (var temp in otherTargets)
            temp.texture = Sprites[clamp ? computedValue % Sprites.Length : computedValue];
        foreach (var temp in otherTargets2)
            temp.texture = Sprites[clamp ? computedValue % Sprites.Length : computedValue];
    }
}