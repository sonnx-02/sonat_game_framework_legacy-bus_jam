using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProductBindingToIndex : BaseActionProductScript
{
    [SerializeField] private GameObject[] setIndexScripts;
    [SerializeField] private IndexBindingScriptBase[] indexBindingScripts;
    [SerializeField] private Product[] correspond;

    private ISetIndex[] _setIndexScripts;
    
    protected override void Register()
    {
        base.Register();
        _setIndexScripts = Get().ToArray();
    }

    private IEnumerable<ISetIndex> Get()
    {
        foreach (var indexBindingScript in setIndexScripts)
            yield return indexBindingScript.GetComponent<ISetIndex>();
    }
    public override void StartAction(Product p)
    {
        CheckRegister();
        base.StartAction(p);
        int i = -1;
        for (var i1 = 0; i1 < correspond.Length; i1++)
            if (p.quantity == correspond[i1].quantity && p.index == correspond[i1].index)
            {
                i = i1;
                break;
            }

//        #if UNITY_EDITOR
//        if (i == -1)
//            Debug.LogError("not found");
//        #endif
       
        foreach (var indexScript in _setIndexScripts)
            indexScript.SetIndex(i);
        indexBindingScripts.OnChanged(i);
    }
}