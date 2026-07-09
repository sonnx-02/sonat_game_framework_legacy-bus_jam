using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;



public class IndexBindingProductSpriteDatabase : IndexBindingScript
{
    [SerializeField] private ProductAssetBind product;

    [SerializeField] private SpriteRenderer target;
    [SerializeField] private Image target2;
    [SerializeField] private SpriteRenderer[] otherTargets;
    [SerializeField] private Image[] otherTargets2;
    [SerializeField] private bool setNativeSize;
    [IndexAsEnum(BuiltInEnumType.CustomSessionDataProperty)]
    [SerializeField] private int updateSessionKey;
    [SerializeField] private PropertyBindTime bindTime;


    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        if ((bindTime & PropertyBindTime.AlwaysSubscribe) != 0)
            updateSessionKey.GetSessionDataCustomProperty().Subscribe(OnValueChanged);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if ((bindTime & PropertyBindTime.OnEnable) != 0)
            OnValueChanged(updateSessionKey.GetSessionDataCustomProperty().Value);
    }

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
        var db = Kernel.GetDatabase<ProductSpriteDatabase>();
        var sprite = db.GetData(product, out int index, product.includeParameter).sprite;

        if (target != null)
            target.sprite = sprite;
        if (target2 != null)
        {
            target2.sprite = sprite;
            if (setNativeSize)
                target2.rectTransform.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
        }


        foreach (var temp in otherTargets)
            temp.sprite = sprite;
        foreach (var temp in otherTargets2)
            temp.sprite = sprite;
    }
}