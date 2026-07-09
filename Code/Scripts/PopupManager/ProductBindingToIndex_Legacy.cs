using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProductBindingToIndex_Legacy : BaseActionProductScript
{
    [SerializeField] private GameObject[] setIndexScripts;
    [SerializeField] private IndexBindingScriptBase[] indexBindingScripts;
    [SerializeField] private Product[] correspond;

//    [SerializeField] private bool bindIndex; // use bindIndex or not
//    [SerializeField] private int[] indexList; // index is In List
//    [SerializeField] private bool indexIsExcept;

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
        int i = 0;
        for (var i1 = 0; i1 < correspond.Length; i1++)
            if (p.quantity == correspond[i1].quantity && p.index == correspond[i1].index)
            {
                i = i1;
                break;
            }

       
        foreach (var indexScript in _setIndexScripts)
            indexScript.SetIndex(i);
        indexBindingScripts.OnChanged(i);
    }
}