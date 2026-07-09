using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionScriptUpdateProductSprite : BaseActionProductScript
{
    [SerializeField] private Image[] images;
    [SerializeField] private BindingToStringTextChange[] texts;
    [SerializeField] private ProductSpriteDatabase database;
    [SerializeField] private bool includeParameter;
    [SerializeField] private Sprite spriteWhenOutRange;
    [SerializeField] private string textWhenOutRange;

//    [SerializeField] private Product test;
//    public override void LogCustom()
//    {
//        base.LogCustom();
//        Apply(test);
//    }

    public override void StartAction(Product product)
    {
        base.StartAction(product);
        var find = (database != null ? database : Kernel.GetDatabase<ProductSpriteDatabase>())
            .GetData(product, out int i, includeParameter); 

        if (find != null)
        {
            foreach (var image in images)
                image.sprite = find.sprite;
            foreach (var image in texts)
                image.OnChanged(find.text);
        }
        else
        {
            foreach (var image in images)
                image.sprite = spriteWhenOutRange;
            foreach (var image in texts)
                image.OnChanged(textWhenOutRange);
        }
    }
}