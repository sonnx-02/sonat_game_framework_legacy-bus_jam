using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptUpdateIndexScriptByValueRetriever : BaseBaseActionScript
{
    [SerializeField] private IndexBindingScriptBase[] indexBindings;
    [SerializeField] private IntValueRetrieverBase valueRetriever;
    public override void StartAction()
    {
        base.StartAction();
        indexBindings.OnChanged(valueRetriever.Value);
    }
}
