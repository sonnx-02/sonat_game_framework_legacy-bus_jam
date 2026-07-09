using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ActionScriptProductBindingView : BaseActionProductScript
{
    [SerializeField] private ProductSpriteDatabase database;
    [SerializeField] private SpriteRenderer target;
    [SerializeField] private SpriteRenderer[] srTargets;
    [SerializeField] private Image target2;
    [SerializeField] private Image[] imgTargets;
    [SerializeField] private TMP_Text text;
    [SerializeField] private BindingToStringTextChange[] textBinds;
    [SerializeField] private IndexBindingScriptBase[] indexBinds;
    [SerializeField] private IndexBindingScriptBase[] parameterBinds;
    [SerializeField] private IndexBindingScriptBase[] parameter2Binds;

    private ProductSpriteDatabase _spriteDatabase;
    [SerializeField] private bool includeParameter = true;
    public override void StartAction(Product p)
    {
        base.StartAction(p);
        if (_spriteDatabase == null)
            _spriteDatabase = database != null ? database : Kernel.GetDatabase<ProductSpriteDatabase>();

        var data = _spriteDatabase.GetData(p, out int index, includeParameter);
        if (index >= 0)
        {
            indexBinds.OnChanged(index);
            ApplySprite(data.sprite);
            ApplyText(data.text);
        }
        parameterBinds.OnChanged(p.parameter);
        parameter2Binds.OnChanged(p.parameter2);
    }

#if UNITY_EDITOR
    private void Reset()
    {
        if (IsNoComponent())
            LoadComponent();
    }
#endif

    private void LoadComponent()
    {
        target = GetComponent<SpriteRenderer>();
        target2 = GetComponent<Image>();
        text = GetComponent<TMP_Text>();
        textBinds = GetComponentsInChildren<BindingToStringTextChange>();
    }

    private bool IsNoComponent()
    {
        return target == null && target2 == null && text == null;
    }
    
    private void ApplySprite(Sprite sprite)
    {
        if (target != null) target.sprite = sprite;
        if (target2 != null) target2.sprite = sprite;
        foreach (var imgTarget in imgTargets)
            imgTarget.sprite = sprite;
        foreach (var imgTarget in srTargets)
            imgTarget.sprite = sprite;
    }
    
    private void ApplyText(string txt)
    {
        if (text != null) text.text = txt;
        foreach (var bindingToStringTextChange in textBinds)
            bindingToStringTextChange.OnChanged(txt);
    }
}