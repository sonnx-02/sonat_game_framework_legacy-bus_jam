using UnityEngine;
using UnityEngine.UI;

public class IndexBindingSpriteChange : IndexBindingScript
{
    [SerializeField] private SpriteRenderer target;
    [SerializeField] private Image target2;
    [SerializeField] private PresetSpriteDatabase database;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Sprite[] others1;
    [SerializeField] private Sprite[] others2;
    [SerializeField] private Sprite[] others3;
    
    [SerializeField] private bool clamp;

    public override int Size => sprites.Length;

    public virtual Sprite[] Sprites
    {
        get
        {
            switch (sharedSetting.index2)
            {
                case 0:
                    if (database != null)
                        return database.Sprites;
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

    
    [SerializeField] private SpriteRenderer[] otherTargets;
    [SerializeField] private Image[] otherTargets2;
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
                    Debug.LogError("out of range:"+computedValue,gameObject);
#endif

                var i = clamp ? computedValue % Sprites.Length : computedValue;
                var sprite = Sprites[i];
                target2.sprite = sprite;
           
                if(setNativeSize)
                    target2.rectTransform.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
            }
        }

        
        foreach (var temp in otherTargets)
            temp.sprite = Sprites[clamp ? computedValue % Sprites.Length : computedValue];
        foreach (var temp in otherTargets2)
            temp.sprite = Sprites[clamp ? computedValue % Sprites.Length : computedValue];
    }
}